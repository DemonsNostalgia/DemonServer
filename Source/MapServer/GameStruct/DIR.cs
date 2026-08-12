using System;
using MapServer;

namespace GameStruct
{
	// Token: 0x0200002B RID: 43
	public class DIR
	{
		// Token: 0x0600014A RID: 330 RVA: 0x0000F630 File Offset: 0x0000D830
		public static byte Random_Dir()
		{
			return (byte)DIR.rd.Next(0, 7);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000F650 File Offset: 0x0000D850
		public static bool Random_Walk(BaseObject obj, ref byte dir, ref short x, ref short y)
		{
			byte b = 0;
			x = obj.GetCurrentX();
			y = obj.GetCurrentY();
			while (b < 10)
			{
				dir = DIR.Random_Dir();
				switch (dir)
				{
				case 0:
					x -= 1;
					y += 1;
					break;
				case 1:
					x -= 1;
					break;
				case 2:
					x -= 1;
					y -= 1;
					break;
				case 3:
					y -= 1;
					break;
				case 4:
					x += 1;
					y -= 1;
					break;
				case 5:
					x += 1;
					break;
				case 6:
					x += 1;
					y += 1;
					break;
				case 7:
					y += 1;
					break;
				}
				if (obj.GetGameMap().CanMove(x, y))
				{
					return true;
				}
				b += 1;
			}
			return false;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000F750 File Offset: 0x0000D950
		public static byte GetDirByPos(short nFromX, short nFromY, short nToX, short nToY)
		{
			byte result;
			if (nFromX < nToX)
			{
				if (nFromY < nToY)
				{
					result = 7;
				}
				else if (nFromY > nToY)
				{
					result = 5;
				}
				else
				{
					result = 6;
				}
			}
			else if (nFromX > nToX)
			{
				if (nFromY < nToY)
				{
					result = 1;
				}
				else if (nFromY > nToY)
				{
					result = 3;
				}
				else
				{
					result = 2;
				}
			}
			else if (nFromY < nToY)
			{
				result = 0;
			}
			else if (nFromY > nToY)
			{
				result = 4;
			}
			else
			{
				result = 8;
			}
			return result;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000F7E0 File Offset: 0x0000D9E0
		public static byte GetNextDir(short srcx, short srcy, short destx, short desty)
		{
			byte result;
			if (destx - srcx < 0 && desty - srcy > 0)
			{
				result = 0;
			}
			else if (destx - srcx < 0 && desty == srcy)
			{
				result = 1;
			}
			else if (destx - srcx < 0 && desty - srcy < 0)
			{
				result = 2;
			}
			else if (destx == srcx && desty - srcy < 0)
			{
				result = 3;
			}
			else if (destx - srcx > 0 && desty - srcy < 0)
			{
				result = 4;
			}
			else if (destx - srcx > 0 && desty == srcy)
			{
				result = 5;
			}
			else if (destx - srcx > 0 && desty - srcy > 0)
			{
				result = 6;
			}
			else
			{
				result = 7;
			}
			return result;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000F8B8 File Offset: 0x0000DAB8
		public static bool GetNexPoint(BaseObject obj, ref short x, ref short y)
		{
			byte dir = obj.GetDir();
			short currentX = obj.GetCurrentX();
			short currentY = obj.GetCurrentY();
			x = (short)(currentX + DIR._DELTA_X[(int)dir]);
			y = (short)(currentY + DIR._DELTA_Y[(int)dir]);
			return obj.GetGameMap().CanMove(x, y) && (x != currentX || y != currentY);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000F92C File Offset: 0x0000DB2C
		public static byte GetAgainstDir(byte dir)
		{
			byte result;
			switch (dir)
			{
			case 0:
				result = 4;
				break;
			case 1:
				result = 4;
				break;
			case 2:
				result = 6;
				break;
			case 3:
				result = 7;
				break;
			case 4:
				result = 0;
				break;
			case 5:
				result = 1;
				break;
			case 6:
				result = 2;
				break;
			case 7:
				result = 3;
				break;
			default:
				result = 8;
				break;
			}
			return result;
		}

		// Token: 0x040001E6 RID: 486
		public const byte LEFT_DOWN = 0;

		// Token: 0x040001E7 RID: 487
		public const byte LEFT = 1;

		// Token: 0x040001E8 RID: 488
		public const byte LEFT_UP = 2;

		// Token: 0x040001E9 RID: 489
		public const byte UP = 3;

		// Token: 0x040001EA RID: 490
		public const byte RIGHT_UP = 4;

		// Token: 0x040001EB RID: 491
		public const byte RIGHT = 5;

		// Token: 0x040001EC RID: 492
		public const byte RIGHT_DOWN = 6;

		// Token: 0x040001ED RID: 493
		public const byte DOWN = 7;

		// Token: 0x040001EE RID: 494
		public const byte MOVEMODE_WALK = 0;

		// Token: 0x040001EF RID: 495
		public const byte MOVEMODE_RUN = 1;

		// Token: 0x040001F0 RID: 496
		public const byte MOVEMODE_SHIFT = 2;

		// Token: 0x040001F1 RID: 497
		public const byte MOVEMODE_JUMP = 3;

		// Token: 0x040001F2 RID: 498
		public const byte MOVEMODE_TRANS = 4;

		// Token: 0x040001F3 RID: 499
		public const byte MOVEMODE_CHGMAP = 5;

		// Token: 0x040001F4 RID: 500
		public const byte MOVEMODE_JUMPMAGICATTCK = 6;

		// Token: 0x040001F5 RID: 501
		public const byte MOVEMODE_COLLIDE = 7;

		// Token: 0x040001F6 RID: 502
		public const byte MOVEMODE_SYNCHRO = 8;

		// Token: 0x040001F7 RID: 503
		public const byte MOVEMODE_TRACK = 9;

		// Token: 0x040001F8 RID: 504
		public const byte MOVEMODE_RUN_DIR0 = 20;

		// Token: 0x040001F9 RID: 505
		public const byte MAX_DIRSIZE = 8;

		// Token: 0x040001FA RID: 506
		public const byte MOVEMODE_RUN_DIR7 = 27;

		// Token: 0x040001FB RID: 507
		public static short[] _DELTA_X = new short[]
		{
			0,
			-1,
			-1,
			-1,
			0,
			1,
			1,
			1,
			0
		};

		// Token: 0x040001FC RID: 508
		public static short[] _DELTA_Y = new short[]
		{
			1,
			1,
			0,
			-1,
			-1,
			-1,
			0,
			1,
			0
		};

		// Token: 0x040001FD RID: 509
		private static Random rd = new Random();
	}
}
