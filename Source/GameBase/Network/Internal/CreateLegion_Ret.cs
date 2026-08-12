using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200002E RID: 46
	public class CreateLegion_Ret
	{
		// Token: 0x060000B3 RID: 179 RVA: 0x00006033 File Offset: 0x00004233
		public CreateLegion_Ret()
		{
			this.mParam = 138;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000604C File Offset: 0x0000424C
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.ret = packIn.ReadByte();
			this.play_id = packIn.ReadInt32();
			this.legion_id = packIn.ReadInt32();
			this.money = packIn.ReadLong();
			this.boss_id = packIn.ReadUInt32();
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x000060A8 File Offset: 0x000042A8
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteByte(this.ret);
			packetOut.WriteInt32(this.play_id);
			packetOut.WriteInt32(this.legion_id);
			packetOut.WriteLong(this.money);
			packetOut.WriteUInt32(this.boss_id);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000117 RID: 279
		public ushort mParam;

		// Token: 0x04000118 RID: 280
		public byte ret;

		// Token: 0x04000119 RID: 281
		public int play_id;

		// Token: 0x0400011A RID: 282
		public int legion_id;

		// Token: 0x0400011B RID: 283
		public long money;

		// Token: 0x0400011C RID: 284
		public uint boss_id;
	}
}
