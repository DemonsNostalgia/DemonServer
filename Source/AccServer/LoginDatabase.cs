using System;
using MySql.Data.MySqlClient;

namespace AccServer
{
	internal static class LoginDatabase
	{
		private static string m_ConnectionString;

		public static void Initialize(string ip, int port, string user, string password, string database)
		{
			MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
			{
				Server = ip,
				Port = (uint)port,
				UserID = user,
				Password = password,
				Database = database,
				CharacterSet = "utf8mb4",
				Pooling = true,
				MinimumPoolSize = 0,
				MaximumPoolSize = 20,
				ConnectionLifeTime = 0,
				ConnectionTimeout = 5
			};
			LoginDatabase.m_ConnectionString = builder.ConnectionString;
			LoginDatabase.EnsureTicketTable();
		}

		public static bool TryAuthenticateAndIssueTicket(
			string account,
			string password,
			string serverName,
			out int accountId)
		{
			if (string.IsNullOrEmpty(LoginDatabase.m_ConnectionString))
			{
				throw new InvalidOperationException("The login database has not been initialized.");
			}

			accountId = -1;
			using (MySqlConnection connection = new MySqlConnection(LoginDatabase.m_ConnectionString))
			{
				connection.Open();
				using (MySqlTransaction transaction = connection.BeginTransaction())
				{
					using (MySqlCommand authenticate = new MySqlCommand(
						"SELECT id FROM account " +
						"WHERE account = @account " +
						"AND BINARY password = BINARY @password LIMIT 1",
						connection,
						transaction))
					{
						authenticate.Parameters.AddWithValue("@account", account);
						authenticate.Parameters.AddWithValue("@password", password);
						object result = authenticate.ExecuteScalar();
						if (result == null || result == DBNull.Value)
						{
							transaction.Rollback();
							return false;
						}

						accountId = Convert.ToInt32(result);
					}

					using (MySqlCommand issueTicket = new MySqlCommand(
						"INSERT INTO cq_login_ticket " +
						"(account, account_id, server_name, expires_at) " +
						"VALUES (@account, @accountId, @serverName, " +
						"DATE_ADD(UTC_TIMESTAMP(6), INTERVAL 60 SECOND)) " +
						"ON DUPLICATE KEY UPDATE " +
						"account_id = VALUES(account_id), " +
						"server_name = VALUES(server_name), " +
						"expires_at = VALUES(expires_at)",
						connection,
						transaction))
					{
						issueTicket.Parameters.AddWithValue("@account", account);
						issueTicket.Parameters.AddWithValue("@accountId", accountId);
						issueTicket.Parameters.AddWithValue("@serverName", serverName);
						issueTicket.ExecuteNonQuery();
					}

					transaction.Commit();
					return true;
				}
			}
		}

		private static void EnsureTicketTable()
		{
			using (MySqlConnection connection = new MySqlConnection(LoginDatabase.m_ConnectionString))
			using (MySqlCommand command = new MySqlCommand(
				"CREATE TABLE IF NOT EXISTS cq_login_ticket (" +
				"account VARCHAR(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL," +
				"account_id INT UNSIGNED NOT NULL," +
				"server_name VARCHAR(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL," +
				"expires_at DATETIME(6) NOT NULL," +
				"PRIMARY KEY (account)," +
				"KEY ix_cq_login_ticket_expires (expires_at)" +
				") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",
				connection))
			{
				connection.Open();
				command.ExecuteNonQuery();
			}
		}
	}
}
