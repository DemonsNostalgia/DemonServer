using System;

namespace MapServer
{
	// Token: 0x020000A9 RID: 169
	public class TempPlayObject
	{
		// Token: 0x0600045B RID: 1115 RVA: 0x00033620 File Offset: 0x00031820
		public TempPlayObject()
		{
			this.key = (this.key2 = 0);
			this.play = null;
			this.isRole = false;
		}

		// Token: 0x04000698 RID: 1688
		public int key;

		// Token: 0x04000699 RID: 1689
		public int key2;

		// Token: 0x0400069A RID: 1690
		public bool isRole;

		// Token: 0x0400069B RID: 1691
		public int accountid;

		// Token: 0x0400069C RID: 1692
		public PlayerObject play;
	}
}
