using System;

namespace GameStruct
{
	// Token: 0x02000038 RID: 56
	public class ItemAdditionInfo
	{
		// Token: 0x0600017E RID: 382 RVA: 0x000103D8 File Offset: 0x0000E5D8
		public ItemAdditionInfo()
		{
			this.level = 0;
			this.type = 0;
			this.life = 0U;
			this.max_attack = 0U;
			this.min_attack = 0U;
			this.defense = 0U;
			this.max_magic_attack = 0U;
			this.min_magic_attack = 0U;
			this.magic_defense = 0U;
			this.dodge = 0U;
		}

		// Token: 0x04000294 RID: 660
		public byte level;

		// Token: 0x04000295 RID: 661
		public byte type;

		// Token: 0x04000296 RID: 662
		public uint life;

		// Token: 0x04000297 RID: 663
		public uint max_attack;

		// Token: 0x04000298 RID: 664
		public uint min_attack;

		// Token: 0x04000299 RID: 665
		public uint defense;

		// Token: 0x0400029A RID: 666
		public uint max_magic_attack;

		// Token: 0x0400029B RID: 667
		public uint min_magic_attack;

		// Token: 0x0400029C RID: 668
		public uint magic_defense;

		// Token: 0x0400029D RID: 669
		public uint dodge;
	}
}
