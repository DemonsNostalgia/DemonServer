using System;

namespace GameStruct
{
	// Token: 0x02000021 RID: 33
	public class ItemTypeInfo
	{
		// Token: 0x0600013D RID: 317 RVA: 0x0000F030 File Offset: 0x0000D230
		public ItemTypeInfo()
		{
			this.id = 0U;
			this.name = "";
			this.req_profession = 0;
			this.req_level = (this.req_sex = 0);
			this.attack_min = (this.attack_max = 0U);
			this.defense = (this.magic_defense = 0U);
			this.magic_attack_min = (this.magic_attck_max = 0U);
			this.dodge = (this.hitrate = 0U);
			this.amount = 0;
			this.info = "";
			this.actionid = 0U;
			this.price = 0;
			this.monster_type = 0U;
			this.client_monopoly = 0;
			this.client_monopoly_known = false;
		}

		public bool IsClientMonopolyItem()
		{
			return this.client_monopoly_known &&
				(this.client_monopoly == 2 || this.client_monopoly == 3);
		}

		// Token: 0x040000DE RID: 222
		public uint id;

		// Token: 0x040000DF RID: 223
		public string name;

		// Token: 0x040000E0 RID: 224
		public byte req_profession;

		// Token: 0x040000E1 RID: 225
		public byte req_level;

		// Token: 0x040000E2 RID: 226
		public byte req_sex;

		// Token: 0x040000E3 RID: 227
		public uint attack_min;

		// Token: 0x040000E4 RID: 228
		public uint attack_max;

		// Token: 0x040000E5 RID: 229
		public uint defense;

		// Token: 0x040000E6 RID: 230
		public uint magic_defense;

		// Token: 0x040000E7 RID: 231
		public uint magic_attack_min;

		// Token: 0x040000E8 RID: 232
		public uint magic_attck_max;

		// Token: 0x040000E9 RID: 233
		public uint dodge;

		// Token: 0x040000EA RID: 234
		public uint hitrate;

		// Token: 0x040000EB RID: 235
		public ushort amount;

		// Token: 0x040000EC RID: 236
		public ushort amount_limit;

		// Token: 0x040000ED RID: 237
		public uint actionid;

		// Token: 0x040000EE RID: 238
		public uint monster_type;

		public ushort client_monopoly;

		public bool client_monopoly_known;

		// Token: 0x040000EF RID: 239
		public int price;

		// Token: 0x040000F0 RID: 240
		public string info;
	}
}
