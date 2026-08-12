using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200007C RID: 124
	public class MsgEquipOperation : BaseMsg
	{
		// Token: 0x0600026E RID: 622 RVA: 0x000197E8 File Offset: 0x000179E8
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.type = packIn.ReadUInt32();
				this.itemid = packIn.ReadUInt32();
				this.materialid = packIn.ReadUInt32();
				if (!packIn.IsComplete())
				{
					this.param = packIn.ReadUInt32();
					if (!packIn.IsComplete())
					{
						this.param1 = packIn.ReadUInt32();
					}
				}
			}
		}

		// Token: 0x04000573 RID: 1395
		public const uint EQUIPSTRONG = 131075U;

		// Token: 0x04000574 RID: 1396
		public const uint EQUIPSTRONGEX = 131078U;

		// Token: 0x04000575 RID: 1397
		public const uint EQUIP_GODEXP = 131079U;

		// Token: 0x04000576 RID: 1398
		public const uint EQUIPLEVEL = 131076U;

		// Token: 0x04000577 RID: 1399
		public const uint EQUIPQUALITY = 131074U;

		// Token: 0x04000578 RID: 1400
		public const uint MAMIC_ADD_GOD = 131081U;

		// Token: 0x04000579 RID: 1401
		public const uint GEMSET = 262220U;

		// Token: 0x0400057A RID: 1402
		public const uint GEMFUSION = 65826U;

		// Token: 0x0400057B RID: 1403
		public const uint GEMREPLACE = 458838U;

		// Token: 0x0400057C RID: 1404
		public const uint GUANJUE_GOLD = 65747U;

		// Token: 0x0400057D RID: 1405
		public const uint GUANJUE_GAMEGOLD = 65750U;

		// Token: 0x0400057E RID: 1406
		public const uint EXIT_GAME = 65753U;

		// Token: 0x0400057F RID: 1407
		public uint type;

		// Token: 0x04000580 RID: 1408
		public uint itemid;

		// Token: 0x04000581 RID: 1409
		public uint materialid;

		// Token: 0x04000582 RID: 1410
		public uint param;

		// Token: 0x04000583 RID: 1411
		public uint param1;
	}
}
