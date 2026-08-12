using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000026 RID: 38
	public class PackUpdatePayRecInfo
	{
		// Token: 0x06000099 RID: 153 RVA: 0x0000593A File Offset: 0x00003B3A
		public PackUpdatePayRecInfo()
		{
			this.mparam = 140;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005950 File Offset: 0x00003B50
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadInt16();
			this.account = packIn.ReadString();
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005978 File Offset: 0x00003B78
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mparam);
			packetOut.WriteString(this.account);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000FA RID: 250
		public ushort mparam;

		// Token: 0x040000FB RID: 251
		public string account;
	}
}
