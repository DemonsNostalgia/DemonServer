using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000014 RID: 20
	public class QueryRoleName
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00003FE0 File Offset: 0x000021E0
		public QueryRoleName()
		{
			this.mParam = 114;
			this.gameid = 0U;
			this.name = "";
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004008 File Offset: 0x00002208
		public void Create(byte[] data)
		{
			PackIn packIn = new PackIn(data);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.name = packIn.ReadString();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x0000403C File Offset: 0x0000223C
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteString(this.name);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000070 RID: 112
		public ushort mParam;

		// Token: 0x04000071 RID: 113
		public uint gameid;

		// Token: 0x04000072 RID: 114
		public string name;
	}
}
