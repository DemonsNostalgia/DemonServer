using System;
using System.Collections.Generic;

namespace GameStruct
{
	// Token: 0x02000018 RID: 24
	public class DropItemInfo
	{
		// Token: 0x06000130 RID: 304 RVA: 0x0000ED3C File Offset: 0x0000CF3C
		public DropItemInfo()
		{
			this.groupid = 0U;
			this.listitem = new List<DropItemClass>();
			this.listrate = new List<uint>();
			this.listamount = new List<uint>();
		}

		// Token: 0x04000099 RID: 153
		public uint groupid;

		// Token: 0x0400009A RID: 154
		public List<DropItemClass> listitem;

		// Token: 0x0400009B RID: 155
		public List<uint> listamount;

		// Token: 0x0400009C RID: 156
		public List<uint> listrate;
	}
}
