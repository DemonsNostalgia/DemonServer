using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000025 RID: 37
	public class GuanJueInfo
	{
		// Token: 0x06000096 RID: 150 RVA: 0x000058A5 File Offset: 0x00003AA5
		public GuanJueInfo()
		{
			this.id = 0U;
			this.guanjue = 0UL;
			this.name = "";
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000058CA File Offset: 0x00003ACA
		public void Create(PackIn inpack)
		{
			this.id = inpack.ReadUInt32();
			this.name = inpack.ReadString();
			this.guanjue = inpack.ReadULong();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000058F4 File Offset: 0x00003AF4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteString(this.name);
			packetOut.WriteULong(this.guanjue);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000F7 RID: 247
		public uint id;

		// Token: 0x040000F8 RID: 248
		public string name;

		// Token: 0x040000F9 RID: 249
		public ulong guanjue;
	}
}
