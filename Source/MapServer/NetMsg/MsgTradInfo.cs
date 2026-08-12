using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000085 RID: 133
	public class MsgTradInfo : BaseMsg
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0001A2E5 File Offset: 0x000184E5
		public MsgTradInfo()
		{
			this.mMsgLen = 16;
			this.mParam = 1056;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0001A304 File Offset: 0x00018504
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.typeid = packIn.ReadUInt32();
				this.type = packIn.ReadInt16();
				this.fightpower = packIn.ReadInt16();
				this.level = packIn.ReadInt16();
				this.param = packIn.ReadInt16();
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0001A370 File Offset: 0x00018570
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.typeid);
			packetOut.WriteInt16(this.type);
			packetOut.WriteInt16(this.fightpower);
			packetOut.WriteInt16(this.level);
			packetOut.WriteInt16(this.param);
			return packetOut.Flush();
		}

		// Token: 0x040005CC RID: 1484
		public const byte REQUEST_TRAD = 1;

		// Token: 0x040005CD RID: 1485
		public const byte QUIT_TRAD = 2;

		// Token: 0x040005CE RID: 1486
		public const byte ITEM_TRAD = 6;

		// Token: 0x040005CF RID: 1487
		public const byte GOLD_TRAD = 7;

		// Token: 0x040005D0 RID: 1488
		public const byte SURE_TRAD = 10;

		public const byte ADD_ITEM_FAILED = 11;

		// Token: 0x040005D1 RID: 1489
		public const byte GAMEGOLD_TRAD = 13;

		// Token: 0x040005D2 RID: 1490
		public uint typeid;

		// Token: 0x040005D3 RID: 1491
		public short type;

		// Token: 0x040005D4 RID: 1492
		public short fightpower;

		// Token: 0x040005D5 RID: 1493
		public short level;

		// Token: 0x040005D6 RID: 1494
		public short param;
	}
}
