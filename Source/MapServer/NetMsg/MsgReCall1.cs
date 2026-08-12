using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000075 RID: 117
	public class MsgReCall1 : BaseMsg
	{
		// Token: 0x0600024F RID: 591 RVA: 0x00017E8A File Offset: 0x0001608A
		public MsgReCall1()
		{
			this.mParam = 1010;
			this.mMsgLen = 28;
			this.type = 2;
			this.tag = 9535;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00017EBA File Offset: 0x000160BA
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00017EC8 File Offset: 0x000160C8
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.mapid);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt32(this.mapid);
			packetOut.WriteInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x04000545 RID: 1349
		public int mapid;

		// Token: 0x04000546 RID: 1350
		public uint roleid;

		// Token: 0x04000547 RID: 1351
		public short x;

		// Token: 0x04000548 RID: 1352
		public short y;

		// Token: 0x04000549 RID: 1353
		public int type;

		// Token: 0x0400054A RID: 1354
		public int tag;
	}
}
