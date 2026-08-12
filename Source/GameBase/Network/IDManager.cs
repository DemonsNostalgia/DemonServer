using System;

namespace GameBase.Network
{
	// Token: 0x02000008 RID: 8
	public class IDManager
	{
		// Token: 0x0600002F RID: 47 RVA: 0x00003234 File Offset: 0x00001434
		public static uint CreateGameId()
		{
			IDManager._id += 1U;
			return IDManager._id;
		}

		// Token: 0x04000014 RID: 20
		private static uint _id = 0U;
	}
}
