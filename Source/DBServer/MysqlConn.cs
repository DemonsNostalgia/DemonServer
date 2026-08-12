using System;
using System.Data;
using GameBase.Config;
using MySql.Data.MySqlClient;

namespace DBServer
{
	// Token: 0x02000007 RID: 7
	public class MysqlConn
	{
		// Token: 0x0600002F RID: 47 RVA: 0x000032E8 File Offset: 0x000014E8
		public static bool Connect(string ip, int port, string user, string paswd, string database)
		{
			try
			{
				MysqlConn.msIP = ip;
				MysqlConn.mnPort = port;
				MysqlConn.msUser = user;
				MysqlConn.msPaswd = paswd;
				MysqlConn.msDatabase = database;
				MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder
				{
					Server = ip,
					Port = (uint)port,
					UserID = user,
					Password = paswd,
					Database = database,
					CharacterSet = "utf8mb4",
					Pooling = true,
					MinimumPoolSize = 0,
					MaximumPoolSize = 100,
					ConnectionLifeTime = 0
				};
				MysqlConn.conn = new MySqlConnection(connectionString.ConnectionString);
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("MySQL connection error: " + ex.Message);
				return false;
			}
			return true;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000338C File Offset: 0x0000158C
		public static MySqlConnection GetConn()
		{
			return MysqlConn.conn;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000033A4 File Offset: 0x000015A4
		public static void Conn_Open()
		{
			try
			{
				if (MysqlConn.conn.State != ConnectionState.Open)
				{
					MysqlConn.conn.Close();
				}
				MysqlConn.conn.Open();
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("Failed to open the MySQL connection: " + ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				throw;
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000342C File Offset: 0x0000162C
		public static void Conn_Close()
		{
			if (MysqlConn.conn.State == ConnectionState.Open)
			{
				MysqlConn.conn.Close();
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000345C File Offset: 0x0000165C
		public static void Dispose()
		{
			if (MysqlConn.conn != null)
			{
				MysqlConn.conn.Dispose();
			}
		}

		// Token: 0x04000027 RID: 39
		private static MySqlConnection conn = null;

		// Token: 0x04000028 RID: 40
		private static string msIP = "";

		// Token: 0x04000029 RID: 41
		private static int mnPort = 0;

		// Token: 0x0400002A RID: 42
		private static string msUser;

		// Token: 0x0400002B RID: 43
		private static string msPaswd;

		// Token: 0x0400002C RID: 44
		private static string msDatabase;
	}
}
