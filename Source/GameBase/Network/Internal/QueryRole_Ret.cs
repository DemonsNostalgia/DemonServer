using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000011 RID: 17
	public class QueryRole_Ret
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00003A04 File Offset: 0x00001C04
		public QueryRole_Ret()
		{
			this.mParam = 12;
			this.gameid = 0U;
			this.key = (this.key2 = 0);
			this.ret = 0;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003A40 File Offset: 0x00001C40
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.key = packIn.ReadInt32();
			this.key2 = packIn.ReadInt32();
			this.ret = packIn.ReadByte();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003A8C File Offset: 0x00001C8C
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteByte(this.ret);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x0400004B RID: 75
		public ushort mParam;

		// Token: 0x0400004C RID: 76
		public uint gameid;

		// Token: 0x0400004D RID: 77
		public int key;

		// Token: 0x0400004E RID: 78
		public int key2;

		// Token: 0x0400004F RID: 79
		public byte ret;
	}
}
