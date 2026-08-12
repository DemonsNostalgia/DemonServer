using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000013 RID: 19
	public class RoleInfo_Ret
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00003EE0 File Offset: 0x000020E0
		public RoleInfo_Ret()
		{
			this.mParam = 113;
			this.gameid = 0U;
			this.key = (this.key2 = 0);
			this.accountid = 0;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003F1C File Offset: 0x0000211C
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.key = packIn.ReadInt32();
			this.key2 = packIn.ReadInt32();
			this.accountid = packIn.ReadInt32();
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003F68 File Offset: 0x00002168
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteInt32(this.accountid);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x0400006B RID: 107
		public ushort mParam;

		// Token: 0x0400006C RID: 108
		public uint gameid;

		// Token: 0x0400006D RID: 109
		public int key;

		// Token: 0x0400006E RID: 110
		public int key2;

		// Token: 0x0400006F RID: 111
		public int accountid;
	}
}
