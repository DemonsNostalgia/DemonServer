using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005F RID: 95
	public class MsgClearObjectInfo : BaseMsg
	{
		// Token: 0x06000202 RID: 514 RVA: 0x00015B54 File Offset: 0x00013D54
		public MsgClearObjectInfo()
		{
			this.mMsgLen = 28;
			this.mParam = 1010;
			this.time = 0;
			this.id = 0U;
			this.x = (this.y = 0);
			this.param = 0;
			this.mapid = 0U;
			this.tag = 9545U;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x00015BB4 File Offset: 0x00013DB4
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null && msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.time = packIn.ReadInt32();
				this.id = packIn.ReadUInt32();
				this.x = packIn.ReadUInt16();
				this.y = packIn.ReadUInt16();
				this.param = packIn.ReadInt32();
				this.mapid = packIn.ReadUInt32();
				this.tag = packIn.ReadUInt32();
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x00015C4C File Offset: 0x00013E4C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt16(this.x);
			packetOut.WriteUInt16(this.y);
			packetOut.WriteInt32(this.param);
			packetOut.WriteUInt32(this.mapid);
			packetOut.WriteUInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x04000410 RID: 1040
		public int time;

		// Token: 0x04000411 RID: 1041
		public uint id;

		// Token: 0x04000412 RID: 1042
		public ushort x;

		// Token: 0x04000413 RID: 1043
		public ushort y;

		// Token: 0x04000414 RID: 1044
		public int param;

		// Token: 0x04000415 RID: 1045
		public uint mapid;

		// Token: 0x04000416 RID: 1046
		public uint tag;
	}
}
