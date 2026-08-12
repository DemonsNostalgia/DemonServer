using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006E RID: 110
	public class MsgChangeMapInfo : BaseMsg
	{
		// Token: 0x06000237 RID: 567 RVA: 0x00017798 File Offset: 0x00015998
		public MsgChangeMapInfo(uint _roleid, uint _mapid, short _x, short _y, byte _dir)
		{
			this.roleid = _roleid;
			this.mapid = _mapid;
			this.x = _x;
			this.y = _y;
			this.dir = _dir;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x000177C8 File Offset: 0x000159C8
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x000177D4 File Offset: 0x000159D4
		public byte[] GetMap1Info()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			this.mMsgLen = 28;
			this.mParam = 1010;
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.mapid);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32((int)this.dir);
			packetOut.WriteUInt32(this.mapid);
			packetOut.WriteInt32(9535);
			return packetOut.Flush();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x00017880 File Offset: 0x00015A80
		public byte[] GetMap2Info()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			this.mMsgLen = 24;
			this.mParam = 1010;
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(0U);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32((int)this.dir);
			packetOut.WriteInt32(-1);
			packetOut.WriteInt32(9567);
			return null;
		}

		// Token: 0x0400050E RID: 1294
		public uint roleid;

		// Token: 0x0400050F RID: 1295
		public uint mapid;

		// Token: 0x04000510 RID: 1296
		public short x;

		// Token: 0x04000511 RID: 1297
		public short y;

		// Token: 0x04000512 RID: 1298
		public byte dir;
	}
}
