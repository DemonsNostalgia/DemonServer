using System;
using System.Collections.Generic;

namespace MapServer
{
	// Token: 0x0200004D RID: 77
	public class MapPath
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x00013084 File Offset: 0x00011284
		public MapPath(uint nWidth, uint nHeight)
		{
			this.Queue = new TLink();
			this.Width = nWidth;
			this.Height = nHeight;
			this.mMapData = new byte[(int)((UIntPtr)nHeight), (int)((UIntPtr)nWidth)];
			this.mPassPoint = null;
			int num = 0;
			while ((long)num < (long)((ulong)this.Height))
			{
				int num2 = 0;
				while ((long)num2 < (long)((ulong)this.Width))
				{
					this.mMapData[num, num2] = 1;
					num2++;
				}
				num++;
			}
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0001310C File Offset: 0x0001130C
		public void SetPointMask(short x, short y, byte tag)
		{
			if (x >= 0 && y >= 0)
			{
				if ((long)x < (long)((ulong)this.Width) && (long)y < (long)((ulong)this.Height))
				{
					this.mMapData[(int)y, (int)x] = tag;
				}
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00013160 File Offset: 0x00011360
		public List<FindPoint> FindPath(short scr_x, short scr_y, short dest_x, short dest_y)
		{
			List<FindPoint> result;
			if (this.mMapData[(int)dest_y, (int)dest_x] == 0)
			{
				result = null;
			}
			else if (scr_x == dest_x && scr_y == dest_y)
			{
				result = null;
			}
			else
			{
				if (this.mPassPoint == null)
				{
					this.mPassPoint = new uint[this.Height * this.Width];
				}
				for (int i = 0; i < this.mPassPoint.Length; i++)
				{
					this.mPassPoint[i] = uint.MaxValue;
				}
				this.Init_Queue();
				this.Enter_Queue(new TTree
				{
					x = scr_x,
					y = scr_y,
					h = 0,
					Father = null
				}, this.judge((int)scr_x, (int)scr_y, (int)dest_x, (int)dest_y));
				int num = 0;
				int num2 = 0;
				TTree ttree;
				for (;;)
				{
					ttree = this.Get_From_Queue();
					num++;
					if (num == 86610)
					{
						num = 0;
					}
					if (ttree == null)
					{
						break;
					}
					num2++;
					short x = ttree.x;
					short y = ttree.y;
					if (x == dest_x && y == dest_y)
					{
						break;
					}
					this.Trytile(x, (short)(y - 1), dest_x, dest_y, ttree, 0);
					this.Trytile((short)(x + 1), (short)(y - 1), dest_x, dest_y, ttree, 1);
					this.Trytile((short)(x + 1), y, dest_x, dest_y, ttree, 2);
					this.Trytile((short)(x + 1), (short)(y + 1), dest_x, dest_y, ttree, 3);
					this.Trytile(x, (short)(y + 1), dest_x, dest_y, ttree, 4);
					this.Trytile((short)(x - 1), (short)(y + 1), dest_x, dest_y, ttree, 5);
					this.Trytile((short)(x - 1), y, dest_x, dest_y, ttree, 6);
					this.Trytile((short)(x - 1), (short)(y - 1), dest_x, dest_y, ttree, 7);
				}
				if (ttree == null)
				{
					result = null;
				}
				else
				{
					List<FindPoint> list = new List<FindPoint>();
					FindPoint item;
					item.x = ttree.x;
					item.y = ttree.y;
					list.Add(item);
					for (ttree = ttree.Father; ttree != null; ttree = ttree.Father)
					{
						item.x = ttree.x;
						item.y = ttree.y;
						list.Add(item);
					}
					result = list;
				}
			}
			return result;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x000133E4 File Offset: 0x000115E4
		private TTree Get_From_Queue()
		{
			TTree node = this.Queue.next.node;
			TLink next = this.Queue.next.next;
			this.Queue.next = null;
			this.Queue.next = next;
			return node;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00013434 File Offset: 0x00011634
		private void Init_Queue()
		{
			this.Queue = new TLink();
			this.Queue.node = null;
			this.Queue.f = -1;
			this.Queue.next = new TLink();
			this.Queue.next.f = 268435455;
			this.Queue.next.node = null;
			this.Queue.next.next = null;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x000134AC File Offset: 0x000116AC
		private void Enter_Queue(TTree node, int f)
		{
			TLink tlink = this.Queue;
			TLink tlink2 = tlink;
			while (f > tlink.f)
			{
				tlink2 = tlink;
				tlink = tlink.next;
				if (tlink == null)
				{
					break;
				}
			}
			tlink2.next = new TLink
			{
				f = f,
				node = node,
				next = tlink
			};
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0001350C File Offset: 0x0001170C
		private int judge(int x, int y, int end_x, int end_y)
		{
			int value = end_x - x;
			int value2 = end_y - y;
			return Math.Abs(value) + Math.Abs(value2);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00013534 File Offset: 0x00011734
		private bool Trytile(short x, short y, short end_x, short end_y, TTree father, byte dir)
		{
			bool flag = false;
			bool result;
			if (this.mMapData[(int)y, (int)x] == 0)
			{
				result = flag;
			}
			else
			{
				for (TTree ttree = father; ttree != null; ttree = ttree.Father)
				{
					if (x == ttree.x && y == ttree.y)
					{
						return false;
					}
				}
				uint num;
				if (dir == 0 || dir == 2 || dir == 4 || dir == 6)
				{
					num = (uint)(father.h + 10);
				}
				else
				{
					num = (uint)(father.h + 14);
				}
				if (num >= this.mPassPoint[(int)(checked((IntPtr)(unchecked((long)x * (long)((ulong)this.Height) + (long)y))))])
				{
					result = false;
				}
				else
				{
					this.mPassPoint[(int)(checked((IntPtr)(unchecked((long)x * (long)((ulong)this.Height) + (long)y))))] = num;
					TTree ttree = new TTree();
					ttree.Father = father;
					ttree.h = (int)num;
					ttree.x = x;
					ttree.y = y;
					ttree.dir = dir;
					this.Enter_Queue(ttree, ttree.h + this.judge((int)x, (int)y, (int)end_x, (int)end_y));
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0400036B RID: 875
		public const byte MASK_OPEN = 1;

		// Token: 0x0400036C RID: 876
		public const byte MASK_CLOSE = 0;

		// Token: 0x0400036D RID: 877
		private uint Width;

		// Token: 0x0400036E RID: 878
		public uint Height;

		// Token: 0x0400036F RID: 879
		private byte[,] mMapData;

		// Token: 0x04000370 RID: 880
		private TLink Queue;

		// Token: 0x04000371 RID: 881
		private uint[] mPassPoint;
	}
}
