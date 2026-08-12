using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000019 RID: 25
	public class AddRoleData_Item_Ret
	{
		// Token: 0x06000067 RID: 103 RVA: 0x0000465C File Offset: 0x0000285C
		public AddRoleData_Item_Ret()
		{
			this.mParam = 121;
			this.gameid = (this.sordid = (this.id = 0U));
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004694 File Offset: 0x00002894
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.sordid = packIn.ReadUInt32();
			this.id = packIn.ReadUInt32();
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000046D4 File Offset: 0x000028D4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteUInt32(this.sordid);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000096 RID: 150
		public ushort mParam;

		// Token: 0x04000097 RID: 151
		public uint gameid;

		// Token: 0x04000098 RID: 152
		public uint sordid;

		// Token: 0x04000099 RID: 153
		public uint id;
	}
}
