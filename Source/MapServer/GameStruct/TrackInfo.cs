using System;

namespace GameStruct
{
	// Token: 0x02000019 RID: 25
	public class TrackInfo
	{
		// Token: 0x06000131 RID: 305 RVA: 0x0000ED70 File Offset: 0x0000CF70
		public TrackInfo()
		{
			this.id = (this.id_next = 0U);
			this.direction = (this.step = (this.alt = 0));
			this.power = (this.apply_ms = 0);
			this.action = 0U;
		}

		// Token: 0x0400009D RID: 157
		public uint id;

		// Token: 0x0400009E RID: 158
		public uint id_next;

		// Token: 0x0400009F RID: 159
		public byte direction;

		// Token: 0x040000A0 RID: 160
		public byte step;

		// Token: 0x040000A1 RID: 161
		public byte alt;

		// Token: 0x040000A2 RID: 162
		public uint action;

		// Token: 0x040000A3 RID: 163
		public int power;

		// Token: 0x040000A4 RID: 164
		public int apply_ms;
	}
}
