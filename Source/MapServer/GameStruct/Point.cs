using System;

namespace GameStruct
{
	// Token: 0x02000024 RID: 36
	public class Point
	{
		// Token: 0x06000140 RID: 320 RVA: 0x0000F290 File Offset: 0x0000D490
		public Point()
		{
			this.x = (this.y = 0);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000F2B8 File Offset: 0x0000D4B8
		public bool CheckVisualDistance(short xx, short yy, int distance = 15)
		{
			int num = Math.Abs((int)(xx - this.x));
			int num2 = Math.Abs((int)(yy - this.y));
			return num <= distance && num2 <= distance;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000F2FC File Offset: 0x0000D4FC
		public bool CheckFanDistance(Point pos, Point magicPos, int distance = 15)
		{
			bool result;
			if (!this.CheckVisualDistance(pos.x, pos.y, distance))
			{
				result = false;
			}
			else
			{
				byte dirByPos = DIR.GetDirByPos(this.x, this.y, magicPos.x, magicPos.y);
				byte[] array = new byte[3];
				switch (dirByPos)
				{
				case 0:
					array[0] = 7;
					array[1] = 0;
					array[2] = 1;
					break;
				case 1:
					array[0] = 0;
					array[1] = 1;
					array[2] = 2;
					break;
				case 2:
					array[0] = 1;
					array[1] = 2;
					array[2] = 3;
					break;
				case 3:
					array[0] = 2;
					array[1] = 3;
					array[2] = 4;
					break;
				case 4:
					array[0] = 3;
					array[1] = 4;
					array[2] = 5;
					break;
				case 5:
					array[0] = 4;
					array[1] = 5;
					array[2] = 6;
					break;
				case 6:
					array[0] = 5;
					array[1] = 6;
					array[2] = 7;
					break;
				case 7:
					array[0] = 6;
					array[1] = 7;
					array[2] = 0;
					break;
				}
				byte dirByPos2 = DIR.GetDirByPos(this.x, this.y, pos.x, pos.y);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == dirByPos2)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04000178 RID: 376
		public short x;

		// Token: 0x04000179 RID: 377
		public short y;
	}
}
