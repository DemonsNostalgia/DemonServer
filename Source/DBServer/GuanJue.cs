using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network.Internal;
using MySql.Data.MySqlClient;

namespace DBServer
{
	// Token: 0x02000003 RID: 3
	public class GuanJue
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002158 File Offset: 0x00000358
		public static GuanJue GetInstance()
		{
			if (GuanJue.mInstnce == null)
			{
				GuanJue.mInstnce = new GuanJue();
			}
			return GuanJue.mInstnce;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000218A File Offset: 0x0000038A
		public GuanJue()
		{
			this.mListInfo = new List<GuanJueInfo>();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021A0 File Offset: 0x000003A0
		public void DB_Load()
		{
			string cmdText = string.Format("select id,name,guanjue from cq_user ORDER BY guanjue DESC", new object[0]);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			int num = 0;
			while (mySqlDataReader.Read())
			{
				if (!mySqlDataReader.HasRows)
				{
					break;
				}
				ulong @uint = mySqlDataReader.GetUInt64("guanjue");
				if (@uint == 0UL)
				{
					break;
				}
				GuanJueInfo guanJueInfo = new GuanJueInfo();
				guanJueInfo.guanjue = @uint;
				guanJueInfo.id = mySqlDataReader.GetUInt32("id");
				guanJueInfo.name = mySqlDataReader.GetString("name");
				guanJueInfo.name = Coding.Latin1ToGB2312(guanJueInfo.name);
				this.mListInfo.Add(guanJueInfo);
				num++;
				if (num > 50)
				{
					break;
				}
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002298 File Offset: 0x00000498
		public void SendData(int mapid = 0)
		{
			GUANJUEINFO guanjueinfo = new GUANJUEINFO();
			for (int i = 0; i < this.mListInfo.Count; i++)
			{
				guanjueinfo.list_item.Add(this.mListInfo[i]);
			}
			SessionManager.Instance().SendMapServer(mapid, guanjueinfo.GetBuffer());
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000022F4 File Offset: 0x000004F4
		public void UpdateGuanJueInfo(GuanJueInfo info)
		{
			for (int i = 0; i < this.mListInfo.Count; i++)
			{
				if (this.mListInfo[i].id == info.id)
				{
					this.mListInfo.RemoveAt(i);
					break;
				}
			}
			bool flag = false;
			for (int i = 0; i < this.mListInfo.Count; i++)
			{
				if (info.guanjue > this.mListInfo[i].guanjue)
				{
					this.mListInfo.Insert(i, info);
					flag = true;
					break;
				}
			}
			if (!flag && this.mListInfo.Count < 50)
			{
				this.mListInfo.Add(info);
			}
		}

		// Token: 0x04000003 RID: 3
		private static GuanJue mInstnce = null;

		// Token: 0x04000004 RID: 4
		private List<GuanJueInfo> mListInfo;
	}
}
