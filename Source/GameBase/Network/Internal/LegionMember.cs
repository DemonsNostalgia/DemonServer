using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200002A RID: 42
	public class LegionMember
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x00005C4A File Offset: 0x00003E4A
		public LegionMember()
		{
			this.id = 0U;
			this.boChange = false;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005C63 File Offset: 0x00003E63
		public void Create(PackIn inpack)
		{
			this.id = inpack.ReadUInt32();
			this.player_id = inpack.ReadInt32();
			this.members_name = inpack.ReadString();
			this.money = inpack.ReadLong();
			this.emoney = inpack.ReadLong();
			this.rank = inpack.ReadInt16();
			this.boChange = inpack.ReadBool();
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005C98 File Offset: 0x00003E98
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.player_id);
			packetOut.WriteString(this.members_name);
			packetOut.WriteLong(this.money);
			packetOut.WriteLong(this.emoney);
			packetOut.WriteInt16(this.rank);
			packetOut.WriteBool(this.boChange);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000105 RID: 261
		public uint id;

		public int player_id;

		// Token: 0x04000106 RID: 262
		public string members_name;

		// Token: 0x04000107 RID: 263
		public long money;

		public long emoney;

		// Token: 0x04000108 RID: 264
		public short rank;

		// Token: 0x04000109 RID: 265
		public bool boChange;
	}
}
