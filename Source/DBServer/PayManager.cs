using System;
using System.Collections.Generic;
using GameBase.Network.Internal;
using MySql.Data.MySqlClient;

namespace DBServer
{
	// Token: 0x0200000A RID: 10
	public class PayManager
	{
		// Token: 0x06000051 RID: 81 RVA: 0x0000562A File Offset: 0x0000382A
		public PayManager()
		{
			this.mLoadPayRecTick = Environment.TickCount;
			this.mDicPayInfo = new Dictionary<string, PayRecInfo>();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000564C File Offset: 0x0000384C
		public static PayManager Instance()
		{
			if (PayManager.mInstance == null)
			{
				PayManager.mInstance = new PayManager();
			}
			return PayManager.mInstance;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005680 File Offset: 0x00003880
		public void SendData(int mapid = 0)
		{
			foreach (PayRecInfo info in this.mDicPayInfo.Values)
			{
				this.UpdateMapServer(mapid, info);
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000056E4 File Offset: 0x000038E4
		public void DB_Load()
		{
			string cmdText = string.Format("select * from cq_payrec where state = 0", new object[0]);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				if (!mySqlDataReader.HasRows)
				{
					break;
				}
				int @int = mySqlDataReader.GetInt32("id");
				bool flag = false;
				foreach (PayRecInfo payRecInfo in this.mDicPayInfo.Values)
				{
					if (payRecInfo.id == @int)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					PayRecInfo payRecInfo2 = new PayRecInfo();
					if (!mySqlDataReader.HasRows)
					{
						break;
					}
					payRecInfo2.id = mySqlDataReader.GetInt32("id");
					payRecInfo2.money = mySqlDataReader.GetInt32("money");
					payRecInfo2.order = mySqlDataReader.GetString("order");
					payRecInfo2.account = mySqlDataReader.GetString("account");
					if (this.mDicPayInfo.ContainsKey(payRecInfo2.account))
					{
						this.mDicPayInfo[payRecInfo2.account].money += payRecInfo2.money;
					}
					else
					{
						this.mDicPayInfo[payRecInfo2.account] = payRecInfo2;
					}
					this.UpdateMapServer(0, this.mDicPayInfo[payRecInfo2.account]);
				}
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000058B0 File Offset: 0x00003AB0
		private void UpdateMapServer(int mapid, PayRecInfo info)
		{
			PackPayRecInfo packPayRecInfo = new PackPayRecInfo();
			packPayRecInfo.order = info.order;
			packPayRecInfo.account = info.account;
			packPayRecInfo.money = info.money;
			packPayRecInfo.id = info.id;
			SessionManager.Instance().SendMapServer(mapid, packPayRecInfo.GetBuffer());
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00005908 File Offset: 0x00003B08
		public void SetPayTag(string sAccount)
		{
			if (this.mDicPayInfo.ContainsKey(sAccount))
			{
				this.mDicPayInfo.Remove(sAccount);
			}
			string cmdText = string.Format("update cq_payrec set account='{0}',state=1", sAccount);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			mySqlCommand.ExecuteNonQuery();
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000596C File Offset: 0x00003B6C
		public void Run()
		{
			if (Environment.TickCount - this.mLoadPayRecTick > 10000)
			{
				this.mLoadPayRecTick = Environment.TickCount;
				this.DB_Load();
			}
		}

		// Token: 0x04000031 RID: 49
		private static PayManager mInstance = null;

		// Token: 0x04000032 RID: 50
		private int mLoadPayRecTick;

		// Token: 0x04000033 RID: 51
		private Dictionary<string, PayRecInfo> mDicPayInfo;
	}
}
