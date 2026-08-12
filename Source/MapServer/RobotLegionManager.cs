using System;
using System.Collections.Generic;

namespace MapServer
{
	// Token: 0x0200009B RID: 155
	public class RobotLegionManager
	{
		// Token: 0x060003EB RID: 1003 RVA: 0x0002E158 File Offset: 0x0002C358
		public static RobotLegionManager GetInstance()
		{
			if (RobotLegionManager.mInstance == null)
			{
				RobotLegionManager.mInstance = new RobotLegionManager();
			}
			return RobotLegionManager.mInstance;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0002E18A File Offset: 0x0002C38A
		public RobotLegionManager()
		{
			this.mDicLegion = new Dictionary<string, uint>();
		}

		// Token: 0x060003ED RID: 1005 RVA: 0x0002E1A0 File Offset: 0x0002C3A0
		public void CreateLegion(string legion_name)
		{
			if (!this.mDicLegion.ContainsKey(legion_name))
			{
				this.mDicLegion[legion_name] = RobotLegionManager.legion_start_id;
				RobotLegionManager.legion_start_id += 1U;
			}
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x0002E1E4 File Offset: 0x0002C3E4
		public uint GetLegionId(string legion_name)
		{
			uint result;
			if (this.mDicLegion.ContainsKey(legion_name))
			{
				result = this.mDicLegion[legion_name];
			}
			else
			{
				result = 0U;
			}
			return result;
		}

		// Token: 0x04000664 RID: 1636
		private static uint legion_start_id = 100000U;

		// Token: 0x04000665 RID: 1637
		private static RobotLegionManager mInstance = null;

		// Token: 0x04000666 RID: 1638
		public Dictionary<string, uint> mDicLegion;
	}
}
