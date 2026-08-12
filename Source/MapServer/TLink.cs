using System;

namespace MapServer
{
	// Token: 0x0200004C RID: 76
	internal class TLink
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x00013064 File Offset: 0x00011264
		public TLink()
		{
			this.node = null;
			this.next = null;
			this.f = 0;
		}

		// Token: 0x04000368 RID: 872
		public TTree node;

		// Token: 0x04000369 RID: 873
		public int f;

		// Token: 0x0400036A RID: 874
		public TLink next;
	}
}
