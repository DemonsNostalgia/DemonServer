using System;

namespace GameStruct
{
	// Token: 0x02000028 RID: 40
	public class CRect
	{
		// Token: 0x06000146 RID: 326 RVA: 0x0000F560 File Offset: 0x0000D760
		public CRect(int xx = 0, int yy = 0, int w = 0, int h = 0)
		{
			this.x = xx;
			this.y = yy;
			this.width = w + this.x;
			this.height = h + this.x;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000F598 File Offset: 0x0000D798
		public bool Check(int xx, int yy)
		{
			return xx >= this.x && xx <= this.width && yy >= this.y && yy <= this.height;
		}

		// Token: 0x0400019D RID: 413
		public int x;

		// Token: 0x0400019E RID: 414
		public int y;

		// Token: 0x0400019F RID: 415
		public int width;

		// Token: 0x040001A0 RID: 416
		public int height;
	}
}
