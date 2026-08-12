using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200000F RID: 15
	public class OpenLoginSession
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00003816 File Offset: 0x00001A16
		public OpenLoginSession()
		{
			this.mParam = 111;
			this.mType = 2;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000383C File Offset: 0x00001A3C
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

		// Token: 0x04000043 RID: 67
		public ushort mParam;

		// Token: 0x04000044 RID: 68
		public byte mType;

		// Token: 0x04000045 RID: 69
		public string text = "LoginServer";
	}
}
