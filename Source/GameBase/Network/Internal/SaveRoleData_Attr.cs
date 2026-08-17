using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000018 RID: 24
	public class SaveRoleData_Attr
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000433C File Offset: 0x0000253C
		public SaveRoleData_Attr()
		{
			this.mParam = 117;
			this.accountid = 0;
			this.name = "";
			this.lookface = 0U;
			this.hair = 0U;
			this.level = 0;
			this.exp = 0;
			this.life = 0U;
			this.mana = 0U;
			this.profession = 0;
			this.pk = 0;
			this.gold = 0L;
			this.gamegold = 0L;
			this.mapid = 0U;
			this.x = 0;
			this.y = 0;
			this.hotkey = "";
			this.guanjue = 0UL;
			this.godlevel = 0;
			this.godship = 0;
			this.godtype = 0;
			this.maxeudemon = 2;
			this.wardrobeHairs = new System.Collections.Generic.List<uint>();
			this.wardrobeAvatars = new System.Collections.Generic.List<uint>();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000043EC File Offset: 0x000025EC
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.accountid = packIn.ReadInt32();
			this.IsExit = packIn.ReadBool();
			this.name = packIn.ReadString();
			this.lookface = packIn.ReadUInt32();
			this.hair = packIn.ReadUInt32();
			this.level = packIn.ReadByte();
			this.exp = packIn.ReadInt32();
			this.life = packIn.ReadUInt32();
			this.mana = packIn.ReadUInt32();
			this.profession = packIn.ReadByte();
			this.pk = packIn.ReadInt16();
			this.gold = packIn.ReadLong();
			this.gamegold = packIn.ReadLong();
			this.stronggold = packIn.ReadLong();
			this.mapid = packIn.ReadUInt32();
			this.x = packIn.ReadInt16();
			this.y = packIn.ReadInt16();
			this.hotkey = packIn.ReadString();
			this.guanjue = packIn.ReadULong();
			this.godlevel = packIn.ReadByte();
			this.godship = packIn.ReadByte();
			this.godtype = packIn.ReadByte();
			this.maxeudemon = packIn.ReadByte();
			ushort wardrobeHairCount = packIn.ReadUInt16();
			for (int i = 0; i < wardrobeHairCount; i++)
			{
				this.wardrobeHairs.Add(packIn.ReadUInt32());
			}
			ushort wardrobeAvatarCount = packIn.ReadUInt16();
			for (int i = 0; i < wardrobeAvatarCount; i++)
			{
				this.wardrobeAvatars.Add(packIn.ReadUInt32());
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004504 File Offset: 0x00002704
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.accountid);
			packetOut.WriteBool(this.IsExit);
			packetOut.WriteString(this.name);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteUInt32(this.hair);
			packetOut.WriteByte(this.level);
			packetOut.WriteInt32(this.exp);
			packetOut.WriteUInt32(this.life);
			packetOut.WriteUInt32(this.mana);
			packetOut.WriteByte(this.profession);
			packetOut.WriteInt16(this.pk);
			packetOut.WriteLong(this.gold);
			packetOut.WriteLong(this.gamegold);
			packetOut.WriteLong(this.stronggold);
			packetOut.WriteUInt32(this.mapid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteString(this.hotkey);
			packetOut.WriteULong(this.guanjue);
			packetOut.WriteByte(this.godlevel);
			packetOut.WriteByte(this.godship);
			packetOut.WriteByte(this.godtype);
			packetOut.WriteByte(this.maxeudemon);
			packetOut.WriteUInt16(unchecked((ushort)this.wardrobeHairs.Count));
			foreach (uint styleId in this.wardrobeHairs)
			{
				packetOut.WriteUInt32(styleId);
			}
			packetOut.WriteUInt16(unchecked((ushort)this.wardrobeAvatars.Count));
			foreach (uint styleId in this.wardrobeAvatars)
			{
				packetOut.WriteUInt32(styleId);
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000080 RID: 128
		public ushort mParam;

		// Token: 0x04000081 RID: 129
		public int accountid;

		// Token: 0x04000082 RID: 130
		public bool IsExit;

		// Token: 0x04000083 RID: 131
		public string name;

		// Token: 0x04000084 RID: 132
		public uint lookface;

		// Token: 0x04000085 RID: 133
		public uint hair;

		// Token: 0x04000086 RID: 134
		public byte level;

		// Token: 0x04000087 RID: 135
		public int exp;

		// Token: 0x04000088 RID: 136
		public uint life;

		// Token: 0x04000089 RID: 137
		public uint mana;

		// Token: 0x0400008A RID: 138
		public byte profession;

		// Token: 0x0400008B RID: 139
		public short pk;

		// Token: 0x0400008C RID: 140
		public long gold;

		// Token: 0x0400008D RID: 141
		public long gamegold;

		// Token: 0x0400008E RID: 142
		public long stronggold;

		// Token: 0x0400008F RID: 143
		public uint mapid;

		// Token: 0x04000090 RID: 144
		public short x;

		// Token: 0x04000091 RID: 145
		public short y;

		// Token: 0x04000092 RID: 146
		public ulong guanjue;

		// Token: 0x04000093 RID: 147
		public byte godlevel;

		public byte godship;

		public byte godtype;

		// Token: 0x04000094 RID: 148
		public byte maxeudemon;

		// Token: 0x04000095 RID: 149
		public string hotkey;

		public System.Collections.Generic.List<uint> wardrobeHairs;

		public System.Collections.Generic.List<uint> wardrobeAvatars;
	}
}
