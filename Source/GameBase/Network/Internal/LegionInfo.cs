using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x0200002B RID: 43
	public class LegionInfo
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x00005CEB File Offset: 0x00003EEB
		public LegionInfo()
		{
			this.list_member = new List<LegionMember>();
			this.name = "";
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005D0C File Offset: 0x00003F0C
		public void Create(PackIn inpack)
		{
			this.id = inpack.ReadUInt32();
			this.name = inpack.ReadString();
			this.title = inpack.ReadByte();
			this.leader_id = inpack.ReadInt32();
			this.leader_name = inpack.ReadString();
			this.money = inpack.ReadLong();
			this.notice = inpack.ReadString();
			int num = inpack.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				LegionMember legionMember = new LegionMember();
				legionMember.Create(inpack);
				this.list_member.Add(legionMember);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005DA4 File Offset: 0x00003FA4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteString(this.name);
			packetOut.WriteByte(this.title);
			packetOut.WriteInt32(this.leader_id);
			packetOut.WriteString(this.leader_name);
			packetOut.WriteLong(this.money);
			packetOut.WriteString(this.notice);
			packetOut.WriteInt32(this.list_member.Count);
			for (int i = 0; i < this.list_member.Count; i++)
			{
				packetOut.WriteBuff(this.list_member[i].GetBuffer());
			}
			return packetOut.GetBuffer();
		}

		// Token: 0x0400010A RID: 266
		public uint id;

		// Token: 0x0400010B RID: 267
		public string name;

		// Token: 0x0400010C RID: 268
		public byte title;

		// Token: 0x0400010D RID: 269
		public int leader_id;

		// Token: 0x0400010E RID: 270
		public string leader_name;

		// Token: 0x0400010F RID: 271
		public long money;

		// Token: 0x04000110 RID: 272
		public string notice;

		// Token: 0x04000111 RID: 273
		public List<LegionMember> list_member;
	}
}
