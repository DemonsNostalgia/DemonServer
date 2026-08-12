using System;

namespace GameStruct
{
	// Token: 0x0200001E RID: 30
	public class NPCInfo
	{
		// Token: 0x0600013A RID: 314 RVA: 0x0000EEDC File Offset: 0x0000D0DC
		public NPCInfo()
		{
			this.id = 0U;
			this.name = "";
			this.mapid = 0U;
			this.x = 0;
			this.y = 0;
			this.ScriptPath = "";
			this.ScriptID = 0U;
		}

		// Token: 0x040000BC RID: 188
		public uint id;

		// Token: 0x040000BD RID: 189
		public string name;

		// Token: 0x040000BE RID: 190
		public uint mapid;

		// Token: 0x040000BF RID: 191
		public short x;

		// Token: 0x040000C0 RID: 192
		public short y;

		// Token: 0x040000C1 RID: 193
		public int lookface;

		// Token: 0x040000C2 RID: 194
		public string ScriptPath;

		// Token: 0x040000C3 RID: 195
		public uint ScriptID;
	}
}
