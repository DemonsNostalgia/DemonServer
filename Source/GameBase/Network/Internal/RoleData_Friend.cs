using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000023 RID: 35
	public class RoleData_Friend
	{
		// Token: 0x0600008E RID: 142 RVA: 0x0000569D File Offset: 0x0000389D
		public RoleData_Friend()
		{
			this.friendid = 0U;
			this.friendname = "";
			this.id = 0;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000056C4 File Offset: 0x000038C4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteInt32(this.id);
			packetOut.WriteByte(this.friendtype);
			packetOut.WriteUInt32(this.friendid);
			packetOut.WriteString(this.friendname);
			return packetOut.GetBuffer();
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005717 File Offset: 0x00003917
		public void Create(byte[] msg, PackIn inpack)
		{
			this.id = inpack.ReadInt32();
			this.friendtype = inpack.ReadByte();
			this.friendid = inpack.ReadUInt32();
			this.friendname = inpack.ReadString();
		}

		// Token: 0x040000EE RID: 238
		public int id;

		// Token: 0x040000EF RID: 239
		public byte friendtype;

		// Token: 0x040000F0 RID: 240
		public uint friendid;

		// Token: 0x040000F1 RID: 241
		public string friendname;
	}
}
