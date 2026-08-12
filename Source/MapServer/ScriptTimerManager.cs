using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;

namespace MapServer
{
	// Token: 0x020000A0 RID: 160
	public class ScriptTimerManager
	{
		// Token: 0x0600042B RID: 1067 RVA: 0x000320B0 File Offset: 0x000302B0
		public static ScriptTimerManager Instance()
		{
			if (ScriptTimerManager.mInstance == null)
			{
				ScriptTimerManager.mInstance = new ScriptTimerManager();
			}
			return ScriptTimerManager.mInstance;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000320E4 File Offset: 0x000302E4
		public ScriptTimerManager()
		{
			this.mListInfo = new List<ScriptTimerInfo>();
			this.mClearTagTick = Environment.TickCount;
			this.mRunTick = Environment.TickCount;
			this.mListPlayTimeOut = new List<PlayTimeOut>();
			this.mPlayTimeOut = new TimeOut();
			this.mPlayTimeOut.SetInterval(1000);
			this.mPlayTimeOut.Update();
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00032150 File Offset: 0x00030350
		public bool Load()
		{
			VerPacket verPacket = ConfigManager.Instance().GetVerPacket();
			string text = verPacket.LoadFileToText("data/config/ScriptTimer.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				ScriptTimerInfo scriptTimerInfo = new ScriptTimerInfo();
				scriptTimerInfo.name = csvFile.GetFieldInfoToValue(i, "name");
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "year");
				scriptTimerInfo.year = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "month");
				scriptTimerInfo.month = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "day");
				scriptTimerInfo.day = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "hour");
				scriptTimerInfo.hour = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "minute");
				scriptTimerInfo.minute = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "second");
				scriptTimerInfo.second = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "script_id");
				scriptTimerInfo.script_id = Convert.ToUInt32(fieldInfoToValue);
				scriptTimerInfo.bTag = false;
				this.mListInfo.Add(scriptTimerInfo);
			}
			return true;
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x00032294 File Offset: 0x00030494
		public void Run()
		{
			if (Environment.TickCount - this.mRunTick > 30000)
			{
				this.mRunTick = Environment.TickCount;
				for (int i = 0; i < this.mListInfo.Count; i++)
				{
					if (DateTime.Now.Year == 0 || DateTime.Now.Year == this.mListInfo[i].year)
					{
						if (DateTime.Now.Month == 0 || DateTime.Now.Month == this.mListInfo[i].month)
						{
							if (DateTime.Now.Day == 0 || DateTime.Now.Day == this.mListInfo[i].day)
							{
								if (DateTime.Now.Hour == this.mListInfo[i].hour)
								{
									if (DateTime.Now.Minute == this.mListInfo[i].minute)
									{
										ScripteManager.Instance().ExecuteAction(this.mListInfo[i].script_id, null);
										this.mListInfo[i].bTag = true;
									}
								}
							}
						}
					}
				}
			}
			if (Environment.TickCount - this.mClearTagTick > 1800000)
			{
				this.mClearTagTick = Environment.TickCount;
				for (int i = 0; i < this.mListInfo.Count; i++)
				{
					if (DateTime.Now.Hour != this.mListInfo[i].hour && this.mListInfo[i].bTag)
					{
						this.mListInfo[i].bTag = false;
					}
				}
			}
			if (this.mPlayTimeOut.ToNextTime() && this.mListPlayTimeOut.Count > 0)
			{
				int j = this.mListPlayTimeOut.Count;
				while (j > 0)
				{
					j--;
					if (this.mListPlayTimeOut[j].TimeOut.IsToNextTime() && this.mListPlayTimeOut[j].IsOnline && this.mListPlayTimeOut[j].callback_scripte_id > 0U)
					{
						PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToPlayerId(this.mListPlayTimeOut[j].id);
						if (playerObject != null)
						{
							ScripteManager.Instance().ExecuteAction(this.mListPlayTimeOut[j].callback_scripte_id, playerObject);
							this.mListPlayTimeOut.RemoveAt(j);
						}
					}
				}
			}
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x000325A4 File Offset: 0x000307A4
		public bool AddPlayerTimeOut(int time_id, int id, int time, uint callback_scripte_id)
		{
			if (id <= 0)
			{
				Log.Instance().WriteLog("Failed to add role timer; invalid ID: " + id.ToString() + " Callback: " + callback_scripte_id.ToString());
			}
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].time_id == time_id)
				{
					return false;
				}
			}
			PlayTimeOut playTimeOut = new PlayTimeOut();
			playTimeOut.time_id = time_id;
			playTimeOut.id = id;
			playTimeOut.callback_scripte_id = callback_scripte_id;
			playTimeOut.TimeOut.SetInterval(time);
			playTimeOut.TimeOut.Update();
			this.mListPlayTimeOut.Add(playTimeOut);
			return true;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00032668 File Offset: 0x00030868
		public int GetPlayerTimeOutS(int time_id, int id)
		{
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].time_id == time_id && this.mListPlayTimeOut[i].id == id)
				{
					return this.mListPlayTimeOut[i].TimeOut.GetTimeOutMS() / 1000;
				}
			}
			return 0;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000326EC File Offset: 0x000308EC
		public bool CheckPlayerTimeOut(int time_id, int id)
		{
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].time_id == time_id && this.mListPlayTimeOut[i].id == id)
				{
					return this.mListPlayTimeOut[i].TimeOut.IsToNextTime();
				}
			}
			return false;
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00032768 File Offset: 0x00030968
		public void DeletePlayerTimeOut(int time_id, int id)
		{
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].time_id == time_id && this.mListPlayTimeOut[i].id == id)
				{
					this.mListPlayTimeOut.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x000327D8 File Offset: 0x000309D8
		public void PlayerExitGame(int id)
		{
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].id == id)
				{
					this.mListPlayTimeOut[i].IsOnline = false;
				}
			}
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00032834 File Offset: 0x00030A34
		public void PlayerEnterGame(int id)
		{
			for (int i = 0; i < this.mListPlayTimeOut.Count; i++)
			{
				if (this.mListPlayTimeOut[i].id == id)
				{
					this.mListPlayTimeOut[i].IsOnline = true;
				}
			}
		}

		// Token: 0x0400067B RID: 1659
		private static ScriptTimerManager mInstance = null;

		// Token: 0x0400067C RID: 1660
		private List<ScriptTimerInfo> mListInfo;

		// Token: 0x0400067D RID: 1661
		private int mClearTagTick;

		// Token: 0x0400067E RID: 1662
		private int mRunTick;

		// Token: 0x0400067F RID: 1663
		private List<PlayTimeOut> mListPlayTimeOut;

		// Token: 0x04000680 RID: 1664
		private TimeOut mPlayTimeOut;
	}
}
