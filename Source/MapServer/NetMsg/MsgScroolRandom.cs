using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000074 RID: 116
	public class MsgScroolRandom : BaseMsg
	{
		// Token: 0x0600024C RID: 588 RVA: 0x00017DA6 File Offset: 0x00015FA6
		public MsgScroolRandom()
		{
			this.mParam = 1010;
			this.mMsgLen = 28;
			this.type = 2;
			this.tag = 9623;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x00017DD6 File Offset: 0x00015FD6
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600024E RID: 590 RVA: 0x00017DE4 File Offset: 0x00015FE4
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt16(this._x);
			packetOut.WriteInt16(this._y);
			packetOut.WriteInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x0400053D RID: 1341
		public int time;

		// Token: 0x0400053E RID: 1342
		public uint roleid;

		// Token: 0x0400053F RID: 1343
		public short x;

		// Token: 0x04000540 RID: 1344
		public short y;

		// Token: 0x04000541 RID: 1345
		public int type;

		// Token: 0x04000542 RID: 1346
		public short _x;

		// Token: 0x04000543 RID: 1347
		public short _y;

		// Token: 0x04000544 RID: 1348
		public int tag;
	}
}
