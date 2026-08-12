using System;
using MySql.Data.MySqlClient;

namespace LoginServer
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
		}

		public static bool TryConsumeLoginTicket(
			string account,
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
					using (MySqlCommand findTicket = new MySqlCommand(
						"SELECT account_id FROM cq_login_ticket " +
						"WHERE account = @account AND server_name = @serverName " +
						"AND expires_at >= UTC_TIMESTAMP(6) LIMIT 1 FOR UPDATE",
						connection,
						transaction))
					{
						findTicket.Parameters.AddWithValue("@account", account);
						findTicket.Parameters.AddWithValue("@serverName", serverName);
						object result = findTicket.ExecuteScalar();
						if (result == null || result == DBNull.Value)
						{
							transaction.Rollback();
							return false;
						}

						accountId = Convert.ToInt32(result);
					}

					using (MySqlCommand consumeTicket = new MySqlCommand(
						"DELETE FROM cq_login_ticket WHERE account = @account",
						connection,
						transaction))
					{
						consumeTicket.Parameters.AddWithValue("@account", account);
						consumeTicket.ExecuteNonQuery();
					}

					transaction.Commit();
					return true;
				}
			}
		}

		public static bool TryAuthenticateCredentials(
			string account,
			string password,
			out int accountId)
		{
			if (string.IsNullOrEmpty(LoginDatabase.m_ConnectionString))
			{
				throw new InvalidOperationException("The login database has not been initialized.");
			}

			accountId = -1;
			using (MySqlConnection connection = new MySqlConnection(LoginDatabase.m_ConnectionString))
			using (MySqlCommand authenticate = new MySqlCommand(
				"SELECT id FROM account " +
				"WHERE account = @account " +
				"AND BINARY password = BINARY @password LIMIT 1",
				connection))
			{
				authenticate.Parameters.AddWithValue("@account", account);
				authenticate.Parameters.AddWithValue("@password", password);
				connection.Open();
				object result = authenticate.ExecuteScalar();
				if (result == null || result == DBNull.Value)
				{
					return false;
				}

				accountId = Convert.ToInt32(result);
				return true;
			}
		}
	}
}
