using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000027 RID: 39
	public class PackPayRecInfo
	{
		// Token: 0x0600009C RID: 156 RVA: 0x000059C9 File Offset: 0x00003BC9
		public PackPayRecInfo()
		{
			this.mparam = 139;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000059E0 File Offset: 0x00003BE0
		public void Creaet(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadInt16();
			this.id = packIn.ReadInt32();
			this.account = packIn.ReadString();
			this.order = packIn.ReadString();
			this.money = packIn.ReadInt32();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005A2C File Offset: 0x00003C2C
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mparam);
			packetOut.WriteInt32(this.id);
			packetOut.WriteString(this.account);
			packetOut.WriteString(this.order);
			packetOut.WriteInt32(this.money);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000FC RID: 252
		public ushort mparam;

		// Token: 0x040000FD RID: 253
		public int id;

		// Token: 0x040000FE RID: 254
		public string order;

		// Token: 0x040000FF RID: 255
		public string account;

		// Token: 0x04000100 RID: 256
		public int money;
	}
}
