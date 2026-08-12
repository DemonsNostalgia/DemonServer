using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000020 RID: 32
	public class KickGamePlay
	{
		// Token: 0x0600007F RID: 127 RVA: 0x00004F6F File Offset: 0x0000316F
		public KickGamePlay()
		{
			this.mParam = 127;
			this.accountid = 0;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004F8C File Offset: 0x0000318C
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.accountid = packIn.ReadInt32();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004FB4 File Offset: 0x000031B4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.accountid);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000C3 RID: 195
		public ushort mParam;

		// Token: 0x040000C4 RID: 196
		public int accountid;
	}
}
