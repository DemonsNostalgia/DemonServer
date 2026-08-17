using System;

namespace GameBase.Network.Internal
{
	// Token: 0x02000012 RID: 18
	public class RoleInfo
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00003B04 File Offset: 0x00001D04
		public RoleInfo(byte[] msg = null)
		{
			this.mParam = 112;
			this.gameid = 0U;
			this.isRole = false;
			this.mKey = 0;
			this.mKey1 = 0;
			this.accountid = 0;
			this.sAccount = "";
			this.name = "";
			this.lookface = 0U;
			this.hair = 0U;
			this.lv = 0;
			this.exp = 0U;
			this.life = 0U;
			this.mana = 0U;
			this.profession = 0;
			this.pk = 0;
			this.gold = 0;
			this.gamegold = 0;
			this.mapid = 0;
			this.x = 0;
			this.y = 0;
			this.playerid = 0;
			this.hotkey = "";
			this.guanjue = 0UL;
			this.godlevel = 0;
			this.godship = 0;
			this.godtype = 0;
			this.maxeudemon = 2;
			this.vip = 0;
			this.wardrobeHairs = new System.Collections.Generic.List<uint>();
			this.wardrobeAvatars = new System.Collections.Generic.List<uint>();
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				this.mParam = packIn.ReadUInt16();
				this.gameid = packIn.ReadUInt32();
				this.mKey = packIn.ReadInt32();
				this.mKey1 = packIn.ReadInt32();
				this.accountid = packIn.ReadInt32();
				this.sAccount = packIn.ReadString();
				this.playerid = packIn.ReadInt32();
				this.isRole = packIn.ReadBool();
				if (this.isRole)
				{
					this.name = packIn.ReadString();
					this.lookface = packIn.ReadUInt32();
					this.hair = packIn.ReadUInt32();
					this.lv = packIn.ReadByte();
					this.exp = packIn.ReadUInt32();
					this.life = packIn.ReadUInt32();
					this.mana = packIn.ReadUInt32();
					this.profession = packIn.ReadByte();
					this.pk = packIn.ReadInt16();
					this.gold = packIn.ReadInt32();
					this.gamegold = packIn.ReadInt32();
					this.stronggold = packIn.ReadInt32();
					this.mapid = packIn.ReadInt32();
					this.x = packIn.ReadInt16();
					this.y = packIn.ReadInt16();
					this.hotkey = packIn.ReadString();
					this.guanjue = packIn.ReadULong();
					this.godlevel = packIn.ReadInt32();
					this.godship = packIn.ReadByte();
					this.godtype = packIn.ReadByte();
					this.maxeudemon = packIn.ReadByte();
					this.vip = packIn.ReadByte();
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
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003D48 File Offset: 0x00001F48
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteInt32(this.mKey);
			packetOut.WriteInt32(this.mKey1);
			packetOut.WriteInt32(this.accountid);
			packetOut.WriteString(this.sAccount);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteBool(this.isRole);
			packetOut.WriteString(this.name);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteUInt32(this.hair);
			packetOut.WriteByte(this.lv);
			packetOut.WriteUInt32(this.exp);
			packetOut.WriteUInt32(this.life);
			packetOut.WriteUInt32(this.mana);
			packetOut.WriteByte(this.profession);
			packetOut.WriteInt16(this.pk);
			packetOut.WriteInt32(this.gold);
			packetOut.WriteInt32(this.gamegold);
			packetOut.WriteInt32(this.stronggold);
			packetOut.WriteInt32(this.mapid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteString(this.hotkey);
			packetOut.WriteULong(this.guanjue);
			packetOut.WriteInt32(this.godlevel);
			packetOut.WriteByte(this.godship);
			packetOut.WriteByte(this.godtype);
			packetOut.WriteByte(this.maxeudemon);
			packetOut.WriteByte(this.vip);
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

		// Token: 0x04000050 RID: 80
		public ushort mParam;

		// Token: 0x04000051 RID: 81
		public bool isRole;

		// Token: 0x04000052 RID: 82
		public uint gameid;

		// Token: 0x04000053 RID: 83
		public int mKey;

		// Token: 0x04000054 RID: 84
		public int mKey1;

		// Token: 0x04000055 RID: 85
		public int accountid;

		// Token: 0x04000056 RID: 86
		public string sAccount;

		// Token: 0x04000057 RID: 87
		public int playerid;

		// Token: 0x04000058 RID: 88
		public string name;

		// Token: 0x04000059 RID: 89
		public uint lookface;

		// Token: 0x0400005A RID: 90
		public uint hair;

		// Token: 0x0400005B RID: 91
		public byte lv;

		// Token: 0x0400005C RID: 92
		public uint exp;

		// Token: 0x0400005D RID: 93
		public uint life;

		// Token: 0x0400005E RID: 94
		public uint mana;

		// Token: 0x0400005F RID: 95
		public byte profession;

		// Token: 0x04000060 RID: 96
		public short pk;

		// Token: 0x04000061 RID: 97
		public int gold;

		// Token: 0x04000062 RID: 98
		public int gamegold;

		// Token: 0x04000063 RID: 99
		public int stronggold;

		// Token: 0x04000064 RID: 100
		public int mapid;

		// Token: 0x04000065 RID: 101
		public short x;

		// Token: 0x04000066 RID: 102
		public short y;

		// Token: 0x04000067 RID: 103
		public string hotkey;

		// Token: 0x04000068 RID: 104
		public ulong guanjue;

		// Token: 0x04000069 RID: 105
		public int godlevel;

		public byte godship;

		public byte godtype;

		// Token: 0x0400006A RID: 106
		public byte maxeudemon;

		public byte vip;

		public System.Collections.Generic.List<uint> wardrobeHairs;

		public System.Collections.Generic.List<uint> wardrobeAvatars;
	}
}
