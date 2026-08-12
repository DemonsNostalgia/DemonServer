using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x02000021 RID: 33
	public class RoleData_Eudemon
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00005008 File Offset: 0x00003208
		public uint GetTypeID()
		{
			return this.typeid;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005020 File Offset: 0x00003220
		public RoleData_Eudemon()
		{
			this.id = 0U;
			this.itemid = 0U;
			this.name = "";
			this.phyatk_grow_rate = 0f;
			this.phyatk_grow_rate_max = 0f;
			this.magicatk_grow_rate = 0f;
			this.magicatk_grow_rate_max = 0f;
			this.life_grow_rate = 0f;
			this.defense_grow_rate = 0f;
			this.magicdef_grow_rate = 0f;
			this.init_life = (this.init_life = 0);
			this.init_atk_min = 0;
			this.init_atk_max = 0;
			this.init_magicatk_min = 0;
			this.init_magicatk_max = 0;
			this.init_defense = 0;
			this.init_magicdef = 0;
			this.luck = 0;
			this.intimacy = 0;
			this.level = 0;
			this.card = 0;
			this.exp = 0;
			this.quality = 0;
			this.wuxing = 0;
			this.recall_count = 0;
			this.bDie = false;
			this.mListMagicInfo = new List<MagicInfo>();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00005120 File Offset: 0x00003320
		public void Create(byte[] msg = null, PackIn _inpack = null)
		{
			PackIn packIn;
			if (msg != null)
			{
				packIn = new PackIn(msg);
			}
			else
			{
				packIn = _inpack;
			}
			this.id = packIn.ReadUInt32();
			this.itemid = packIn.ReadUInt32();
			this.name = packIn.ReadString();
			this.phyatk_grow_rate = packIn.ReadFloat();
			this.phyatk_grow_rate_max = packIn.ReadFloat();
			this.magicatk_grow_rate = packIn.ReadFloat();
			this.magicatk_grow_rate_max = packIn.ReadFloat();
			this.life_grow_rate = packIn.ReadFloat();
			this.defense_grow_rate = packIn.ReadFloat();
			this.magicdef_grow_rate = packIn.ReadFloat();
			this.init_life = packIn.ReadInt32();
			this.init_atk_min = packIn.ReadInt32();
			this.init_atk_max = packIn.ReadInt32();
			this.init_magicatk_min = packIn.ReadInt32();
			this.init_magicatk_max = packIn.ReadInt32();
			this.init_defense = packIn.ReadInt32();
			this.init_magicdef = packIn.ReadInt32();
			this.luck = packIn.ReadInt32();
			this.intimacy = packIn.ReadInt32();
			this.level = packIn.ReadInt16();
			this.card = packIn.ReadInt32();
			this.exp = packIn.ReadInt32();
			this.quality = packIn.ReadInt32();
			this.recall_count = packIn.ReadInt32();
			this.wuxing = packIn.ReadInt32();
			int num = packIn.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				MagicInfo magicInfo = new MagicInfo();
				magicInfo.id = packIn.ReadInt32();
				magicInfo.magicid = packIn.ReadUInt32();
				magicInfo.exp = packIn.ReadUInt32();
				this.mListMagicInfo.Add(magicInfo);
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000052C8 File Offset: 0x000034C8
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.itemid);
			packetOut.WriteString(this.name);
			packetOut.WriteFloat(this.phyatk_grow_rate);
			packetOut.WriteFloat(this.phyatk_grow_rate_max);
			packetOut.WriteFloat(this.magicatk_grow_rate);
			packetOut.WriteFloat(this.magicatk_grow_rate_max);
			packetOut.WriteFloat(this.life_grow_rate);
			packetOut.WriteFloat(this.defense_grow_rate);
			packetOut.WriteFloat(this.magicdef_grow_rate);
			packetOut.WriteInt32(this.init_life);
			packetOut.WriteInt32(this.init_atk_min);
			packetOut.WriteInt32(this.init_atk_max);
			packetOut.WriteInt32(this.init_magicatk_min);
			packetOut.WriteInt32(this.init_magicatk_max);
			packetOut.WriteInt32(this.init_defense);
			packetOut.WriteInt32(this.init_magicdef);
			packetOut.WriteInt32(this.luck);
			packetOut.WriteInt32(this.intimacy);
			packetOut.WriteInt16(this.level);
			packetOut.WriteInt32(this.card);
			packetOut.WriteInt32(this.exp);
			packetOut.WriteInt32(this.quality);
			packetOut.WriteInt32(this.recall_count);
			packetOut.WriteInt32(this.wuxing);
			packetOut.WriteInt32(this.mListMagicInfo.Count);
			for (int i = 0; i < this.mListMagicInfo.Count; i++)
			{
				packetOut.WriteInt32(this.mListMagicInfo[i].id);
				packetOut.WriteUInt32(this.mListMagicInfo[i].magicid);
				packetOut.WriteUInt32(this.mListMagicInfo[i].exp);
			}
			return packetOut.GetBuffer();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000054A4 File Offset: 0x000036A4
		public int GetInitAtk()
		{
			string value = this.init_atk_min.ToString() + this.init_atk_max.ToString();
			return Convert.ToInt32(value);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000054D8 File Offset: 0x000036D8
		public int GetInitMagicAtk()
		{
			string value = this.init_magicatk_min.ToString() + this.init_magicatk_max.ToString();
			return Convert.ToInt32(value);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000550C File Offset: 0x0000370C
		public int GetInitDefense()
		{
			string value = this.init_defense.ToString() + this.init_magicdef.ToString();
			return Convert.ToInt32(value);
		}

		// Token: 0x040000C5 RID: 197
		public uint id;

		// Token: 0x040000C6 RID: 198
		public uint itemid;

		// Token: 0x040000C7 RID: 199
		public string name;

		// Token: 0x040000C8 RID: 200
		public float phyatk_grow_rate;

		// Token: 0x040000C9 RID: 201
		public float phyatk_grow_rate_max;

		// Token: 0x040000CA RID: 202
		public float magicatk_grow_rate;

		// Token: 0x040000CB RID: 203
		public float magicatk_grow_rate_max;

		// Token: 0x040000CC RID: 204
		public float life_grow_rate;

		// Token: 0x040000CD RID: 205
		public float defense_grow_rate;

		// Token: 0x040000CE RID: 206
		public float magicdef_grow_rate;

		// Token: 0x040000CF RID: 207
		public int init_life;

		// Token: 0x040000D0 RID: 208
		public int init_atk_min;

		// Token: 0x040000D1 RID: 209
		public int init_atk_max;

		// Token: 0x040000D2 RID: 210
		public int init_magicatk_min;

		// Token: 0x040000D3 RID: 211
		public int init_magicatk_max;

		// Token: 0x040000D4 RID: 212
		public int init_defense;

		// Token: 0x040000D5 RID: 213
		public int init_magicdef;

		// Token: 0x040000D6 RID: 214
		public int luck;

		// Token: 0x040000D7 RID: 215
		public int intimacy;

		// Token: 0x040000D8 RID: 216
		public short level;

		// Token: 0x040000D9 RID: 217
		public int card;

		// Token: 0x040000DA RID: 218
		public int exp;

		// Token: 0x040000DB RID: 219
		public int quality;

		// Token: 0x040000DC RID: 220
		public int wuxing;

		// Token: 0x040000DD RID: 221
		public int recall_count;

		// Token: 0x040000DE RID: 222
		public List<MagicInfo> mListMagicInfo;

		// Token: 0x040000DF RID: 223
		public uint typeid;

		// Token: 0x040000E0 RID: 224
		public int life_max;

		// Token: 0x040000E1 RID: 225
		public int life;

		// Token: 0x040000E2 RID: 226
		public int atk_min;

		// Token: 0x040000E3 RID: 227
		public int atk_max;

		// Token: 0x040000E4 RID: 228
		public int magicatk_max;

		// Token: 0x040000E5 RID: 229
		public int magicatk_min;

		// Token: 0x040000E6 RID: 230
		public int defense;

		// Token: 0x040000E7 RID: 231
		public int magicdef;

		// Token: 0x040000E8 RID: 232
		public bool bDie;
	}
}
