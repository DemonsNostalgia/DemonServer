using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001A RID: 26
	public class AddRoleData_Item
	{
		// Token: 0x0600006A RID: 106 RVA: 0x0000473F File Offset: 0x0000293F
		public AddRoleData_Item()
		{
			this.mParam = 120;
			this.item = new RoleData_Item();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004760 File Offset: 0x00002960
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.sortid = packIn.ReadUInt32();
			this.item.Create(null, packIn);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000047A4 File Offset: 0x000029A4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteUInt32(this.sortid);
			packetOut.WriteBuff(this.item.GetBuffer());
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x0400009A RID: 154
		public ushort mParam;

		// Token: 0x0400009B RID: 155
		public uint gameid;

		// Token: 0x0400009C RID: 156
		public uint sortid;

		// Token: 0x0400009D RID: 157
		public RoleData_Item item;
	}
}
