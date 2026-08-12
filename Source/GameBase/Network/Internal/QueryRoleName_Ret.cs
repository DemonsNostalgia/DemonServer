using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000015 RID: 21
	public class QueryRoleName_Ret
	{
		// Token: 0x0600005B RID: 91 RVA: 0x0000409A File Offset: 0x0000229A
		public QueryRoleName_Ret()
		{
			this.mParam = 115;
			this.gameid = 0U;
			this.tag = false;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000040BC File Offset: 0x000022BC
		public void Create(byte[] data)
		{
			PackIn packIn = new PackIn(data);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.tag = packIn.ReadBool();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000040F0 File Offset: 0x000022F0
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteBool(this.tag);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000073 RID: 115
		public ushort mParam;

		// Token: 0x04000074 RID: 116
		public uint gameid;

		// Token: 0x04000075 RID: 117
		public bool tag;
	}
}
