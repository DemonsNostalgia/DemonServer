using System;
using System.Collections.Generic;
using GameBase.Config;
using GameStruct;

namespace MapServer
{
	// Token: 0x02000007 RID: 7
	public class IDManager
	{
		// Token: 0x06000066 RID: 102 RVA: 0x000046C4 File Offset: 0x000028C4
		public static void RecoveryTypeID(uint id, byte type)
		{
			RecoveryID item;
			item.nID = id;
			item.bType = type;
			IDManager.mListRecovery.Add(item);
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000046F0 File Offset: 0x000028F0
		public static uint CreateTypeId(byte type)
		{
			uint result = 0U;
			for (int i = 0; i < IDManager.mListRecovery.Count; i++)
			{
				if (IDManager.mListRecovery[i].bType == type)
				{
					result = IDManager.mListRecovery[i].nID;
					IDManager.mListRecovery.RemoveAt(i);
					return result;
				}
			}
			switch (type)
			{
			case 1:
				result = IDManager.npc_id;
				IDManager.npc_id += 1U;
				break;
			case 2:
				if (IDManager.playser_id > IDManager.player_end_id)
				{
					Log.Instance().WriteLog("Failed to generate a player type ID; the server must be restarted.");
					return 0U;
				}
				result = IDManager.playser_id;
				IDManager.playser_id += 1U;
				break;
			case 3:
				if (IDManager.monster_id > IDManager.monster_end_id)
				{
					Log.Instance().WriteLog("Failed to generate a monster type ID; the ID range is exhausted.");
					return 0U;
				}
				result = IDManager.monster_start_id;
				IDManager.monster_start_id += 1U;
				break;
			case 4:
				if (IDManager.eudemon_id > IDManager.eudemon_end_id)
				{
					Log.Instance().WriteLog("Failed to generate an Eudemon type ID; the server must be restarted.");
					return 0U;
				}
				result = IDManager.eudemon_id;
				IDManager.eudemon_id += 1U;
				break;
			case 7:
				if (IDManager.guardknight_id > IDManager.guardknight_end_id)
				{
					Log.Instance().WriteLog("Failed to generate a guard type ID; the server must be restarted.");
					return 0U;
				}
				result = IDManager.guardknight_id;
				IDManager.guardknight_id += 1U;
				break;
			case 8:
				if (IDManager.effect_id > IDManager.effect_end_id)
				{
					Log.Instance().WriteLog("Failed to generate an effect type ID; the server must be restarted.");
					return 0U;
				}
				result = IDManager.effect_id;
				IDManager.effect_id += 1U;
				break;
			}
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000048E8 File Offset: 0x00002AE8
		public static int CreateEudemonCard()
		{
			int num = IRandom.Random(5, 10);
			string text = IRandom.Random(1, 9).ToString();
			for (int i = 1; i < num; i++)
			{
				text += IRandom.Random(0, 9).ToString();
			}
			return Convert.ToInt32(text);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004948 File Offset: 0x00002B48
		public static int GetEudemonWuxing()
		{
			int num = IRandom.Random(1, 5);
			if (num == 5)
			{
				if (IRandom.Random(1, 100) < 50)
				{
					num = IRandom.Random(1, 5);
				}
			}
			return num;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004990 File Offset: 0x00002B90
		public static uint CreateId()
		{
			IDManager._id += 1U;
			return IDManager._id;
		}

		// Token: 0x0400002D RID: 45
		private static uint player_start_id = 1000000U;

		// Token: 0x0400002E RID: 46
		private static uint player_end_id = 1999999999U;

		// Token: 0x0400002F RID: 47
		private static uint playser_id = IDManager.player_start_id;

		// Token: 0x04000030 RID: 48
		private static uint monster_start_id = 400001U;

		// Token: 0x04000031 RID: 49
		private static uint monster_end_id = 599999U;

		// Token: 0x04000032 RID: 50
		private static uint monster_id = IDManager.monster_start_id;

		// Token: 0x04000033 RID: 51
		private static uint guardknight_start_id = 700001U;

		// Token: 0x04000034 RID: 52
		private static uint guardknight_end_id = 899999U;

		// Token: 0x04000035 RID: 53
		private static uint guardknight_id = IDManager.guardknight_start_id;

		// Token: 0x04000036 RID: 54
		public static uint eudemon_start_id = 2000000000U;

		// Token: 0x04000037 RID: 55
		private static uint eudemon_end_id = 3999999999U;

		// Token: 0x04000038 RID: 56
		private static uint eudemon_id = IDManager.eudemon_start_id;

		// Token: 0x04000039 RID: 57
		private static uint npc_start_id = 400001U;

		// Token: 0x0400003A RID: 58
		private static uint npc_id = IDManager.npc_start_id;

		// Token: 0x0400003B RID: 59
		private static uint effect_start_id = 50001U;

		// Token: 0x0400003C RID: 60
		private static uint effect_end_id = 69999U;

		// Token: 0x0400003D RID: 61
		private static uint effect_id = IDManager.effect_start_id;

		// Token: 0x0400003E RID: 62
		private static uint _id = 0U;

		// Token: 0x0400003F RID: 63
		private static List<RecoveryID> mListRecovery = new List<RecoveryID>();
	}
}
