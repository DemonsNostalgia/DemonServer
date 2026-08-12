using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000016 RID: 22
	public class CreateRole
	{
		// Token: 0x0600005E RID: 94 RVA: 0x0000414E File Offset: 0x0000234E
		public CreateRole()
		{
			this.mParam = 115;
			this.gameid = 0U;
			this.accountid = 0;
			this.name = "";
			this.lookface = 0U;
			this.profession = 0;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004188 File Offset: 0x00002388
		public void Create(byte[] data)
		{
			PackIn packIn = new PackIn(data);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.accountid = packIn.ReadInt32();
			this.name = packIn.ReadString();
			this.lookface = packIn.ReadUInt32();
			this.profession = packIn.ReadByte();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000041E0 File Offset: 0x000023E0
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteInt32(this.accountid);
			packetOut.WriteString(this.name);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteByte(this.profession);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000076 RID: 118
		public ushort mParam;

		// Token: 0x04000077 RID: 119
		public uint gameid;

		// Token: 0x04000078 RID: 120
		public int accountid;

		// Token: 0x04000079 RID: 121
		public string name;

		// Token: 0x0400007A RID: 122
		public uint lookface;

		// Token: 0x0400007B RID: 123
		public byte profession;
	}
}
