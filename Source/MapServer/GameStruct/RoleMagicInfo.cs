using System;

namespace GameStruct
{
	// Token: 0x0200002E RID: 46
	public class RoleMagicInfo
	{
		// Token: 0x0600015D RID: 349 RVA: 0x0000FBDF File Offset: 0x0000DDDF
		public RoleMagicInfo()
		{
			this.magicid = 0U;
			this.level = 0;
			this.exp = 0U;
		}

		// Token: 0x04000209 RID: 521
		public int id;

		// Token: 0x0400020A RID: 522
		public uint magicid;

		// Token: 0x0400020B RID: 523
		public byte level;

		// Token: 0x0400020C RID: 524
		public uint exp;
	}
}
