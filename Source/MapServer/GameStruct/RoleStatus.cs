using System;

namespace GameStruct
{
	// Token: 0x02000042 RID: 66
	public class RoleStatus
	{
		// Token: 0x06000185 RID: 389 RVA: 0x00010518 File Offset: 0x0000E718
		public RoleStatus()
		{
			this.nStatus = (this.nTime = 0);
			this.nLastTick = Environment.TickCount;
		}

		// Token: 0x040002F3 RID: 755
		public const int STATUS_NORMAL = 0;

		// Token: 0x040002F4 RID: 756
		public const int STATUS_DIE = 1;

		// Token: 0x040002F5 RID: 757
		public const int STATUS_CRIME = 2;

		// Token: 0x040002F6 RID: 758
		public const int STATUS_POISON = 3;

		// Token: 0x040002F7 RID: 759
		public const int STATUS_TEAMLEADER = 4;

		// Token: 0x040002F8 RID: 760
		public const int STATUS_PKVALUE = 5;

		// Token: 0x040002F9 RID: 761
		public const int STATUS_DETACH_BADLY = 6;

		// Token: 0x040002FA RID: 762
		public const int STATUS_DETACH_ALL = 7;

		// Token: 0x040002FB RID: 763
		public const int STATUS_VAMPIRE = 8;

		// Token: 0x040002FC RID: 764
		public const int STATUS_DISAPPEARING = 9;

		// Token: 0x040002FD RID: 765
		public const int STATUS_MAGICDEFENCE = 10;

		// Token: 0x040002FE RID: 766
		public const int STATUS_SUPER_MDEF = 11;

		// Token: 0x040002FF RID: 767
		public const int STATUS_ATTACK = 12;

		// Token: 0x04000300 RID: 768
		public const int STATUS_REFLECT = 13;

		// Token: 0x04000301 RID: 769
		public const int STATUS_HIDDEN = 14;

		// Token: 0x04000302 RID: 770
		public const int STATUS_MAGICDAMAGE = 15;

		// Token: 0x04000303 RID: 771
		public const int STATUS_ATKSPEED = 16;

		// Token: 0x04000304 RID: 772
		public const int STATUS_LURKER = 17;

		// Token: 0x04000305 RID: 773
		public const int STATUS_SYNCRIME = 18;

		// Token: 0x04000306 RID: 774
		public const int STATUS_REFLECTMAGIC = 19;

		// Token: 0x04000307 RID: 775
		public const int STATUS_SUPER_DEF = 20;

		// Token: 0x04000308 RID: 776
		public const int STATUS_SUPER_ATK = 21;

		// Token: 0x04000309 RID: 777
		public const int STATUS_SUPER_MATK = 22;

		// Token: 0x0400030A RID: 778
		public const int STATUS_STOP = 23;

		// Token: 0x0400030B RID: 779
		public const int STATUS_DEFENCE1 = 24;

		// Token: 0x0400030C RID: 780
		public const int STATUS_DEFENCE2 = 25;

		// Token: 0x0400030D RID: 781
		public const int STATUS_DEFENCE3 = 26;

		// Token: 0x0400030E RID: 782
		public const int STATUS_FREEZE = 27;

		// Token: 0x0400030F RID: 783
		public const int STATUS_SMOKE = 28;

		// Token: 0x04000310 RID: 784
		public const int STATUS_DARKNESS = 29;

		// Token: 0x04000311 RID: 785
		public const int STATUS_PALSY = 30;

		// Token: 0x04000312 RID: 786
		public const int STATUS_TEAM_BEGIN = 31;

		// Token: 0x04000313 RID: 787
		public const int STATUS_TEAMHEALTH = 31;

		// Token: 0x04000314 RID: 788
		public const int STATUS_TEAMATTACK = 32;

		// Token: 0x04000315 RID: 789
		public const int STATUS_TEAMDEFENCE = 33;

		// Token: 0x04000316 RID: 790
		public const int STATUS_TEAMSPEED = 34;

		// Token: 0x04000317 RID: 791
		public const int STATUS_TEAMEXP = 35;

		// Token: 0x04000318 RID: 792
		public const int STATUS_TEAMSPIRIT = 36;

		// Token: 0x04000319 RID: 793
		public const int STATUS_TEAMCLEAN = 37;

		// Token: 0x0400031A RID: 794
		public const int STATUS_TEAM_END = 37;

		// Token: 0x0400031B RID: 795
		public const int STATUS_SLOWDOWN1 = 38;

		// Token: 0x0400031C RID: 796
		public const int STATUS_SLOWDOWN2 = 39;

		// Token: 0x0400031D RID: 797
		public const int STATUS_MAXLIFE = 40;

		// Token: 0x0400031E RID: 798
		public const int STATUS_MAXENERGY = 41;

		// Token: 0x0400031F RID: 799
		public const int STATUS_DEF2ATK = 42;

		// Token: 0x04000320 RID: 800
		public const int STATUS_ADD_EXP = 43;

		// Token: 0x04000321 RID: 801
		public const int STATUS_DMG2LIFE = 44;

		// Token: 0x04000322 RID: 802
		public const int STATUS_ATTRACT_MONSTER = 45;

		// Token: 0x04000323 RID: 803
		public const int STATUS_XPFULL = 46;

		// Token: 0x04000324 RID: 804
		public const int STATUS_XPFULL_ATTACK = 47;

		// Token: 0x04000325 RID: 805
		public const int STATUS_MOLONGSHOUHU = 99;

		// Token: 0x04000326 RID: 806
		public const int STATUS_STEALTH = 100;

		// Token: 0x04000327 RID: 807
		public const int STATUS_FLY = 101;

		// Token: 0x04000328 RID: 808
		public const int STATUS_YUANSUZHANGKONG = 102;

		// Token: 0x04000329 RID: 809
		public const int STATUS_JUYANSHENGDUN = 103;

		// Token: 0x0400032A RID: 810
		public const int STATUS_HEILONGWU = 104;

		// Token: 0x0400032B RID: 811
		public const int STATUS_ANSHAXIELONG = 105;

		// Token: 0x0400032C RID: 812
		public const int STATUS_MINGGUOSHENGNV = 106;

		// Token: 0x0400032D RID: 813
		public const int STATUS_WANGNIANWULING = 107;

		// Token: 0x0400032E RID: 814
		public const int STATUS_MIXINSHU = 120;

		// Token: 0x0400032F RID: 815
		public const int STATUS_WUDI = 1000;

		// Token: 0x04000330 RID: 816
		public const int STATUS_RED = 1001;

		// Token: 0x04000331 RID: 817
		public const int STATUS_BLOCK = 1002;

		// Token: 0x04000332 RID: 818
		public const int STATUS_HUASHENWANGLING = 1003;

		// Token: 0x04000333 RID: 819
		public const int STATUS_HUASHENWUSHI = 1004;

		// Token: 0x04000334 RID: 820
		public const int STATUS_SHENYUANELING = 1005;

		// Token: 0x04000335 RID: 821
		public const int STATUS_DIYUXIEFU = 1006;

		// Token: 0x04000336 RID: 822
		public const int STATUS_SHIHUNWULING = 1007;

		// Token: 0x04000337 RID: 823
		public const int STATUS_ZHAOHUANWUHUAN = 1008;

		// Token: 0x04000338 RID: 824
		public const int STATUS_XUEXI = 1009;

		// Token: 0x04000339 RID: 825
		public const int STATUS_PTICH = 1010;

		// Token: 0x0400033A RID: 826
		public const int STATUS_LIMIT = 65535;

		// Token: 0x0400033B RID: 827
		public int nStatus;

		// Token: 0x0400033C RID: 828
		public int nTime;

		// Token: 0x0400033D RID: 829
		public int nLastTick;
	}
}
