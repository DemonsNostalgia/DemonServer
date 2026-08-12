using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200007E RID: 126
	public class MsgStrongPack : BaseMsg
	{
		// Token: 0x06000273 RID: 627 RVA: 0x00019918 File Offset: 0x00017B18
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.tick = packIn.ReadInt32();
				this.type = packIn.ReadByte();
				this.param = packIn.ReadByte();
				this.param3 = packIn.ReadInt16();
				this.param1 = packIn.ReadInt32();
				this.param2 = packIn.ReadInt32();
				this.itemid = packIn.ReadUInt32();
			}
		}

		// Token: 0x0400058A RID: 1418
		public const byte STRONGPACK_TYPE = 10;

		public const byte WEAPON_SOUL_PACKAGE_TYPE = 133;

		public const byte FASHION_PACKAGE_TYPE = 145;

		public const byte MOUNT_PACKAGE_TYPE = 146;

		public const byte PACKAGE_REFRESH = 0;

		public const byte PACKAGE_CHECK_IN = 1;

		public const byte PACKAGE_CHECK_OUT = 2;

		// Token: 0x0400058D RID: 1421
		public const byte STRONGPACK_TYPE_SAVE = 1;

		// Token: 0x0400058E RID: 1422
		public const byte STRONGPACK_TYPE_GIVE = 2;

		// Token: 0x0400058F RID: 1423
		public int tick;

		// Token: 0x04000590 RID: 1424
		public byte type;

		// Token: 0x04000591 RID: 1425
		public byte param;

		// Token: 0x04000592 RID: 1426
		public short param3;

		// Token: 0x04000593 RID: 1427
		public int param1;

		// Token: 0x04000594 RID: 1428
		public int param2;

		// Token: 0x04000595 RID: 1429
		public uint itemid;
	}
}
