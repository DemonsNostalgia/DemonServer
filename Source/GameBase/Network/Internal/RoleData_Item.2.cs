using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001E RID: 30
	public class RoleData_Item
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00004BFC File Offset: 0x00002DFC
		public RoleData_Item()
		{
			this.forgename = "";
			this.playerid = 0;
			this.itemid = (this.gem1 = (this.gem2 = 0U));
			this.stronglv = 0;
			this.amount = 0;
			this.postion = 0;
			this.id = 0U;
			this.war_ghost_exp = 0;
			this.di_attack = 0;
			this.shui_attack = 0;
			this.huo_attack = 0;
			this.feng_attack = 0;
			this.property = 0;
			this.gem3 = 0U;
			this.god_strong = 0;
			this.god_exp = 0;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004C98 File Offset: 0x00002E98
		public void Create(byte[] msg = null, PackIn _inpack = null)
		{
			PackIn packIn;
			if (_inpack != null)
			{
				packIn = _inpack;
			}
			else
			{
				packIn = new PackIn(msg);
			}
			this.id = packIn.ReadUInt32();
			this.playerid = packIn.ReadInt32();
			this.itemid = packIn.ReadUInt32();
			this.postion = packIn.ReadUInt16();
			this.stronglv = packIn.ReadByte();
			this.gem1 = packIn.ReadUInt32();
			this.gem2 = packIn.ReadUInt32();
			this.forgename = packIn.ReadString();
			this.amount = packIn.ReadUInt16();
			this.war_ghost_exp = packIn.ReadInt32();
			this.di_attack = packIn.ReadByte();
			this.shui_attack = packIn.ReadByte();
			this.huo_attack = packIn.ReadByte();
			this.feng_attack = packIn.ReadByte();
			this.property = packIn.ReadInt32();
			this.gem3 = packIn.ReadUInt32();
			this.god_strong = packIn.ReadInt32();
			this.god_exp = packIn.ReadInt32();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004D98 File Offset: 0x00002F98
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteUInt32(this.itemid);
			packetOut.WriteUInt16(this.postion);
			packetOut.WriteByte(this.stronglv);
			packetOut.WriteUInt32(this.gem1);
			packetOut.WriteUInt32(this.gem2);
			packetOut.WriteString(this.forgename);
			packetOut.WriteUInt16(this.amount);
			packetOut.WriteInt32(this.war_ghost_exp);
			packetOut.WriteByte(this.di_attack);
			packetOut.WriteByte(this.shui_attack);
			packetOut.WriteByte(this.huo_attack);
			packetOut.WriteByte(this.feng_attack);
			packetOut.WriteInt32(this.property);
			packetOut.WriteUInt32(this.gem3);
			packetOut.WriteInt32(this.god_strong);
			packetOut.WriteInt32(this.god_exp);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000AD RID: 173
		public uint id;

		// Token: 0x040000AE RID: 174
		public int playerid;

		// Token: 0x040000AF RID: 175
		public uint itemid;

		// Token: 0x040000B0 RID: 176
		public ushort postion;

		// Token: 0x040000B1 RID: 177
		public byte stronglv;

		// Token: 0x040000B2 RID: 178
		public uint gem1;

		// Token: 0x040000B3 RID: 179
		public uint gem2;

		// Token: 0x040000B4 RID: 180
		public string forgename;

		// Token: 0x040000B5 RID: 181
		public ushort amount;

		// Token: 0x040000B6 RID: 182
		public int war_ghost_exp;

		// Token: 0x040000B7 RID: 183
		public byte di_attack;

		// Token: 0x040000B8 RID: 184
		public byte shui_attack;

		// Token: 0x040000B9 RID: 185
		public byte huo_attack;

		// Token: 0x040000BA RID: 186
		public byte feng_attack;

		// Token: 0x040000BB RID: 187
		public int property;

		// Token: 0x040000BC RID: 188
		public uint gem3;

		// Token: 0x040000BD RID: 189
		public int god_strong;

		// Token: 0x040000BE RID: 190
		public int god_exp;
	}
}
