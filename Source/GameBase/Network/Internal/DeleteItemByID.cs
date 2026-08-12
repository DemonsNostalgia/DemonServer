using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001F RID: 31
	public class DeleteItemByID
	{
		// Token: 0x0600007C RID: 124 RVA: 0x00004EA1 File Offset: 0x000030A1
		public DeleteItemByID()
		{
			this.mParam = 119;
			this.playerid = 0;
			this.id = 0U;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004EC4 File Offset: 0x000030C4
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.playerid = packIn.ReadInt32();
			this.id = packIn.ReadUInt32();
			this.postion = packIn.ReadUInt16();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004F04 File Offset: 0x00003104
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt16(this.postion);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000BF RID: 191
		public ushort mParam;

		// Token: 0x040000C0 RID: 192
		public int playerid;

		// Token: 0x040000C1 RID: 193
		public uint id;

		// Token: 0x040000C2 RID: 194
		public ushort postion;
	}
}
