using System;

namespace GameStruct
{
	// Token: 0x0200001B RID: 27
	public class MapGateInfo
	{
		// Token: 0x06000132 RID: 306 RVA: 0x0000EDC6 File Offset: 0x0000CFC6
		public MapGateInfo()
		{
			this.src_mapid = 0U;
			this.src_x = 0;
			this.src_y = 0;
			this.target_mapid = 0U;
			this.target_x = 0;
			this.target_y = 0;
			this.dis = 0;
		}

		// Token: 0x040000AC RID: 172
		public uint src_mapid;

		// Token: 0x040000AD RID: 173
		public short src_x;

		// Token: 0x040000AE RID: 174
		public short src_y;

		// Token: 0x040000AF RID: 175
		public uint target_mapid;

		// Token: 0x040000B0 RID: 176
		public short target_x;

		// Token: 0x040000B1 RID: 177
		public short target_y;

		// Token: 0x040000B2 RID: 178
		public int dis;
	}
}
