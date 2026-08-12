using System;

namespace GameStruct
{
	// Token: 0x02000020 RID: 32
	public class MonsterInfo
	{
		// Token: 0x0600013C RID: 316 RVA: 0x0000EF90 File Offset: 0x0000D190
		public MonsterInfo()
		{
			this.id = 0U;
			this.name = "";
			this.ai = 0;
			this.lookface = 0U;
			this.level = 0;
			this.life = (this.mana = 0);
			this.attack_max = (this.attack_min = (this.defense = 0U));
			this.dodge = (this.range = (this.attack_speed = (this.move_speed = 0)));
			this.drop_group = 0U;
			this.eudemon_type = 0;
			this.die_scripte_id = 0U;
		}

		// Token: 0x040000CD RID: 205
		public uint id;

		// Token: 0x040000CE RID: 206
		public string name;

		// Token: 0x040000CF RID: 207
		public int ai;

		// Token: 0x040000D0 RID: 208
		public uint lookface;

		// Token: 0x040000D1 RID: 209
		public ushort level;

		// Token: 0x040000D2 RID: 210
		public int life;

		// Token: 0x040000D3 RID: 211
		public int mana;

		// Token: 0x040000D4 RID: 212
		public uint attack_min;

		// Token: 0x040000D5 RID: 213
		public uint attack_max;

		// Token: 0x040000D6 RID: 214
		public uint defense;

		// Token: 0x040000D7 RID: 215
		public ushort dodge;

		// Token: 0x040000D8 RID: 216
		public ushort range;

		// Token: 0x040000D9 RID: 217
		public ushort attack_speed;

		// Token: 0x040000DA RID: 218
		public ushort move_speed;

		// Token: 0x040000DB RID: 219
		public uint drop_group;

		// Token: 0x040000DC RID: 220
		public int eudemon_type;

		// Token: 0x040000DD RID: 221
		public uint die_scripte_id;
	}
}
