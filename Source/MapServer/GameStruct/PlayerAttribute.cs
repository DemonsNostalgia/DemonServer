using System;

namespace GameStruct
{
	// Token: 0x02000026 RID: 38
	public class PlayerAttribute
	{
		// Token: 0x06000143 RID: 323 RVA: 0x0000F444 File Offset: 0x0000D644
		public PlayerAttribute()
		{
			this.resetAttr();
			this.account_id = 0;
			this.lookface = 0U;
			this.profession = 0;
			this.hair = 0U;
			this.level = 0;
			this.exp = 0;
			this.life = (this.mana = 0U);
			this.exp_max = 0UL;
			this.pk = 0;
			this.gold = (this.gamegold = 0);
			this.sp = 0;
			this.sp_max = 100;
			this.pk_mode = 1;
			this.mapid = 0U;
			this.guanjue = 0UL;
			this.sAccount = "";
			this.godlevel = 0;
			this.maxeudemon = 2;
			this.vip = 0;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000F4F8 File Offset: 0x0000D6F8
		public void resetAttr()
		{
			this.doage = (this.hitrate = 0U);
			this.attack = (this.attack_max = (this.magic_attack = (this.magic_attack_max = 0U)));
			this.life_max = (this.mana_max = 0U);
			this.defense = (this.magic_defense = 0U);
		}

		// Token: 0x0400017B RID: 379
		public int account_id;

		// Token: 0x0400017C RID: 380
		public int player_id;

		// Token: 0x0400017D RID: 381
		public uint attack;

		// Token: 0x0400017E RID: 382
		public uint attack_max;

		// Token: 0x0400017F RID: 383
		public uint magic_attack;

		// Token: 0x04000180 RID: 384
		public uint magic_attack_max;

		// Token: 0x04000181 RID: 385
		public uint lookface;

		// Token: 0x04000182 RID: 386
		public byte profession;

		// Token: 0x04000183 RID: 387
		public uint hair;

		// Token: 0x04000184 RID: 388
		public byte level;

		// Token: 0x04000185 RID: 389
		public int exp;

		// Token: 0x04000186 RID: 390
		public ulong exp_max;

		// Token: 0x04000187 RID: 391
		public uint life;

		// Token: 0x04000188 RID: 392
		public uint life_max;

		// Token: 0x04000189 RID: 393
		public uint mana;

		// Token: 0x0400018A RID: 394
		public uint mana_max;

		// Token: 0x0400018B RID: 395
		public uint doage;

		// Token: 0x0400018C RID: 396
		public uint hitrate;

		// Token: 0x0400018D RID: 397
		public uint defense;

		// Token: 0x0400018E RID: 398
		public uint magic_defense;

		// Token: 0x0400018F RID: 399
		public int sp;

		// Token: 0x04000190 RID: 400
		public int sp_max;

		// Token: 0x04000191 RID: 401
		public int gold;

		// Token: 0x04000192 RID: 402
		public int gamegold;

		// Token: 0x04000193 RID: 403
		public long stronggold;

		// Token: 0x04000194 RID: 404
		public short pk;

		// Token: 0x04000195 RID: 405
		public byte pk_mode;

		// Token: 0x04000196 RID: 406
		public uint mapid;

		// Token: 0x04000197 RID: 407
		public ulong guanjue;

		// Token: 0x04000198 RID: 408
		public byte godlevel;

		// Token: 0x04000199 RID: 409
		public byte maxeudemon;

		public byte vip;

		// Token: 0x0400019A RID: 410
		public string sAccount;
	}
}
