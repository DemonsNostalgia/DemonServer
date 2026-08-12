using System;

namespace GameStruct
{
	// Token: 0x0200003C RID: 60
	public class LookFaceInfo
	{
		// Token: 0x06000180 RID: 384 RVA: 0x0001043F File Offset: 0x0000E63F
		public LookFaceInfo()
		{
			this.name = "";
			this.price = 0;
			this.itemid = 0U;
			this.lookfaceid = 0;
		}

		// Token: 0x040002C1 RID: 705
		public uint itemid;

		// Token: 0x040002C2 RID: 706
		public int lookfaceid;

		// Token: 0x040002C3 RID: 707
		public string name;

		// Token: 0x040002C4 RID: 708
		public int price;
	}
}
