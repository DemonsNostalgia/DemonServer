using System;

namespace GameStruct
{
	// Token: 0x0200001F RID: 31
	public class GeneratorInfo
	{
		// Token: 0x0600013B RID: 315 RVA: 0x0000EF2C File Offset: 0x0000D12C
		public GeneratorInfo()
		{
			this.mapid = (this.bound_x = (this.bound_y = (this.bound_cx = (this.bound_cy = (this.amount = (this.time = (this.monsterid = 0U)))))));
			this.dir = 0;
		}

		// Token: 0x040000C4 RID: 196
		public uint mapid;

		// Token: 0x040000C5 RID: 197
		public uint bound_x;

		// Token: 0x040000C6 RID: 198
		public uint bound_y;

		// Token: 0x040000C7 RID: 199
		public uint bound_cx;

		// Token: 0x040000C8 RID: 200
		public uint bound_cy;

		// Token: 0x040000C9 RID: 201
		public uint amount;

		// Token: 0x040000CA RID: 202
		public uint time;

		// Token: 0x040000CB RID: 203
		public uint monsterid;

		// Token: 0x040000CC RID: 204
		public byte dir;
	}
}
