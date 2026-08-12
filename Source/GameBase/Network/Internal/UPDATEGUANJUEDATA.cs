using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000029 RID: 41
	public class UPDATEGUANJUEDATA
	{
		// Token: 0x060000A2 RID: 162 RVA: 0x00005BA6 File Offset: 0x00003DA6
		public UPDATEGUANJUEDATA()
		{
			this.mparam = 133;
			this.info = new GuanJueInfo();
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005BC8 File Offset: 0x00003DC8
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.info.Create(packIn);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005BF4 File Offset: 0x00003DF4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mparam);
			packetOut.WriteBuff(this.info.GetBuffer());
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000103 RID: 259
		public ushort mparam;

		// Token: 0x04000104 RID: 260
		public GuanJueInfo info;
	}
}
