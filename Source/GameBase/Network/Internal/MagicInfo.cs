using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001D RID: 29
	public class MagicInfo
	{
		// Token: 0x06000078 RID: 120 RVA: 0x00004BC0 File Offset: 0x00002DC0
		public MagicInfo()
		{
			this.id = (this.ownerid = 0);
			this.magicid = 0U;
			this.level = 0;
			this.exp = 0U;
		}

		// Token: 0x040000A8 RID: 168
		public int id;

		// Token: 0x040000A9 RID: 169
		public int ownerid;

		// Token: 0x040000AA RID: 170
		public uint magicid;

		// Token: 0x040000AB RID: 171
		public byte level;

		// Token: 0x040000AC RID: 172
		public uint exp;
	}
}
