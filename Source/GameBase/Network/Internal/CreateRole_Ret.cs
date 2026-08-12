using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000017 RID: 23
	public class CreateRole_Ret
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00004265 File Offset: 0x00002465
		public CreateRole_Ret()
		{
			this.mParam = 116;
			this.playerid = 0;
			this.gameid = 0U;
			this.tag = false;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004290 File Offset: 0x00002490
		public void Create(byte[] data)
		{
			PackIn packIn = new PackIn(data);
			packIn.ReadUInt16();
			this.playerid = packIn.ReadInt32();
			this.gameid = packIn.ReadUInt32();
			this.tag = packIn.ReadBool();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000042D0 File Offset: 0x000024D0
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteBool(this.tag);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x0400007C RID: 124
		public ushort mParam;

		// Token: 0x0400007D RID: 125
		public int playerid;

		// Token: 0x0400007E RID: 126
		public uint gameid;

		// Token: 0x0400007F RID: 127
		public bool tag;
	}
}
