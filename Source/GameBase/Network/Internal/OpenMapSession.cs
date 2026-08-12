using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200000E RID: 14
	public class OpenMapSession
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00003791 File Offset: 0x00001991
		public OpenMapSession()
		{
			this.mParam = 111;
			this.mType = 5;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000037B8 File Offset: 0x000019B8
		public byte[] GetBuff()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteByte(this.mType);
			packetOut.WriteString(this.text);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000040 RID: 64
		public ushort mParam;

		// Token: 0x04000041 RID: 65
		public byte mType;

		// Token: 0x04000042 RID: 66
		public string text = "MapServer";
	}
}
