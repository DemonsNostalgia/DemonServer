using System;

namespace GameStruct
{
	// Token: 0x02000029 RID: 41
	public class ActionInfo
	{
		// Token: 0x06000148 RID: 328 RVA: 0x0000F5DC File Offset: 0x0000D7DC
		public ActionInfo()
		{
			this.id = (this.id_next = (this.id_nextfail = (this.type = (this.data = 0U))));
			this.param = "";
		}

		// Token: 0x040001A1 RID: 417
		public uint id;

		// Token: 0x040001A2 RID: 418
		public uint id_next;

		// Token: 0x040001A3 RID: 419
		public uint id_nextfail;

		// Token: 0x040001A4 RID: 420
		public uint type;

		// Token: 0x040001A5 RID: 421
		public uint data;

		// Token: 0x040001A6 RID: 422
		public string param;
	}
}
