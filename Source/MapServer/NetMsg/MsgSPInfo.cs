using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000059 RID: 89
	public class MsgSPInfo : BaseMsg
	{
		// Token: 0x060001EC RID: 492 RVA: 0x00014F11 File Offset: 0x00013111
		public MsgSPInfo()
		{
			this.mMsgLen = 20;
			this.mParam = 1011;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00014F30 File Offset: 0x00013130
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.id = packIn.ReadUInt32();
				this.type = packIn.ReadInt32();
				this.param = packIn.ReadInt32();
				this.sp = packIn.ReadInt32();
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00014F9C File Offset: 0x0001319C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.sp);
			return packetOut.Flush();
		}

		// Token: 0x040003E2 RID: 994
		public uint id;

		// Token: 0x040003E3 RID: 995
		public int type;

		// Token: 0x040003E4 RID: 996
		public int param;

		// Token: 0x040003E5 RID: 997
		public int sp;
	}
}
