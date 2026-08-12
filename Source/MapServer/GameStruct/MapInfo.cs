using System;

namespace GameStruct
{
	// Token: 0x02000016 RID: 22
	public class MapInfo
	{
		// Token: 0x0600012E RID: 302 RVA: 0x0000ECDC File Offset: 0x0000CEDC
		public MapInfo()
		{
			this.id = 0U;
			this.name = (this.dmappath = "");
			this.recallid = (uint)(this.recallx = (this.recally = 0));
		}

		// Token: 0x04000091 RID: 145
		public uint id;

		// Token: 0x04000092 RID: 146
		public string name;

		// Token: 0x04000093 RID: 147
		public string dmappath;

		// Token: 0x04000094 RID: 148
		public uint recallid;

		// Token: 0x04000095 RID: 149
		public ushort recallx;

		// Token: 0x04000096 RID: 150
		public ushort recally;

		// Token: 0x04000097 RID: 151
		public bool issnows;
	}
}
