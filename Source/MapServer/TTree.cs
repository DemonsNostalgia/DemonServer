using System;

namespace MapServer
{
	// Token: 0x0200004B RID: 75
	internal class TTree
	{
		// Token: 0x060001B4 RID: 436 RVA: 0x00013036 File Offset: 0x00011236
		public TTree()
		{
			this.h = 0;
			this.x = 0;
			this.y = 0;
			this.dir = 0;
			this.Father = null;
		}

		// Token: 0x04000363 RID: 867
		public int h;

		// Token: 0x04000364 RID: 868
		public short x;

		// Token: 0x04000365 RID: 869
		public short y;

		// Token: 0x04000366 RID: 870
		public byte dir;

		// Token: 0x04000367 RID: 871
		public TTree Father;
	}
}
