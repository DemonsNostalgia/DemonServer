using System;

namespace MapServer
{
	// Token: 0x0200000A RID: 10
	public class RefreshObject
	{
		// Token: 0x06000072 RID: 114 RVA: 0x00004D41 File Offset: 0x00002F41
		public RefreshObject()
		{
			this.Reset();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004D53 File Offset: 0x00002F53
		public void Reset()
		{
			this.bRefreshTag = false;
			this.obj = null;
		}

		// Token: 0x04000043 RID: 67
		public bool bRefreshTag;

		// Token: 0x04000044 RID: 68
		public BaseObject obj;
	}
}
