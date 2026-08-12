using System;

namespace GameBase.Network
{
	// Token: 0x02000033 RID: 51
	public class PacketProtoco
	{
		// Token: 0x04000128 RID: 296
		public const ushort S_NOTICE = 1004;

		// Token: 0x04000129 RID: 297
		public const ushort S_LEFTNOTICE = 1004;

		// Token: 0x0400012A RID: 298
		public const ushort S_CHATNOTICE = 1004;

		// Token: 0x0400012B RID: 299
		public const ushort S_SELFROLEINFO = 1006;

		// Token: 0x0400012C RID: 300
		public const ushort S_OPERATEEQUIP = 1009;

		// Token: 0x0400012D RID: 301
		public const ushort S_CLEARITEM = 1009;

		// Token: 0x0400012E RID: 302
		public const ushort S_ITEMINFO = 1008;

		// Token: 0x0400012F RID: 303
		public const ushort S_MAPINFO = 1010;

		// Token: 0x04000130 RID: 304
		public const ushort S_CLEARMONSTER = 1010;

		// Token: 0x04000131 RID: 305
		public const ushort S_SCROOLRANDOM = 1010;

		// Token: 0x04000132 RID: 306
		public const ushort S_LOCK = 1010;

		// Token: 0x04000133 RID: 307
		public const ushort S_EUDEMONTAG = 1010;

		// Token: 0x04000134 RID: 308
		public const ushort S_SPINFO = 1011;

		// Token: 0x04000135 RID: 309
		public const ushort S_ROLEINFO = 1014;

		// Token: 0x04000136 RID: 310
		public const ushort S_COMBO = 1015;

		// Token: 0x04000137 RID: 311
		public const ushort S_HOTKEY = 1015;

		// Token: 0x04000138 RID: 312
		public const ushort S_LEGION_NAME = 1015;

		// Token: 0x04000139 RID: 313
		public const ushort S_UPDATESP = 1017;

		// Token: 0x0400013A RID: 314
		public const ushort S_UPXP = 1017;

		// Token: 0x0400013B RID: 315
		public const ushort S_USERATTRIBUTE = 1017;

		// Token: 0x0400013C RID: 316
		public const ushort S_FRIENDINFO = 1019;

		// Token: 0x0400013D RID: 317
		public const ushort S_TRAD = 1056;

		// Token: 0x0400013E RID: 318
		public const ushort S_ATTACK = 1022;

		// Token: 0x0400013F RID: 319
		public const ushort S_GAMESERVERINFO = 1057;

		// Token: 0x04000140 RID: 320
		public const ushort S_MAGICINFO = 1103;

		// Token: 0x04000141 RID: 321
		public const ushort S_SELFLEGIONINFO = 1106;

		// Token: 0x04000142 RID: 322
		public const ushort S_PTICH_ITEMINFO = 1108;

		// Token: 0x04000143 RID: 323
		public const ushort S_KEY = 1059;

		// Token: 0x04000144 RID: 324
		public const ushort S_DROPITEM = 1101;

		// Token: 0x04000145 RID: 325
		public const ushort S_STRONGINFO = 1102;

		// Token: 0x04000146 RID: 326
		public const ushort S_MAGICATTACK = 1105;

		// Token: 0x04000147 RID: 327
		public const ushort S_EUDEMONBALLTE = 1116;

		// Token: 0x04000148 RID: 328
		public const ushort S_NPCINFO = 2030;

		// Token: 0x04000149 RID: 329
		public const ushort S_NPCREPLY = 2032;

		// Token: 0x0400014A RID: 330
		public const ushort S_EQUIPOPERATION = 2036;

		// Token: 0x0400014B RID: 331
		public const ushort S_EUDEMONINFO = 2037;

		// Token: 0x0400014C RID: 332
		public const ushort S_GUANJUE = 2060;

		// Token: 0x0400014D RID: 333
		public const ushort S_MONSTERINFO = 2069;

		// Token: 0x0400014E RID: 334
		public const ushort C_CREATEROLE = 1001;

		// Token: 0x0400014F RID: 335
		public const ushort C_CHANGEPKMODE = 1010;

		// Token: 0x04000150 RID: 336
		public const ushort C_MSGTALK = 1004;

		// Token: 0x04000151 RID: 337
		public const ushort C_MSGIEM = 1009;

		// Token: 0x04000152 RID: 338
		public const ushort C_HOTKEY = 1015;

		// Token: 0x04000153 RID: 339
		public const ushort C_ADDFRIEND = 1019;

		// Token: 0x04000154 RID: 340
		public const ushort C_ATTACK = 1022;

		// Token: 0x04000155 RID: 341
		public const ushort C_GETFRIENDINFO = 1032;

		// Token: 0x04000156 RID: 342
		public const ushort C_UPDATEKEY = 1052;

		public const ushort C_LOGINCLIENTINFO = 1100;

		// Token: 0x04000157 RID: 343
		public const ushort C_TRAD = 1056;

		// Token: 0x04000158 RID: 344
		public const ushort C_LOGINUSER = 1083;

		// Token: 0x04000159 RID: 345
		public const ushort C_LOGINGAME = 1095;

		public const ushort C_LOGINGAME_DIRECT = 1120;

		// Token: 0x0400015A RID: 346
		public const ushort C_PICKDROPITEM = 1101;

		// Token: 0x0400015B RID: 347
		public const ushort C_STRONGPACK = 1102;

		// Token: 0x0400015C RID: 348
		public const ushort C_QUERYCREATEROLENAME = 1158;

		// Token: 0x0400015D RID: 349
		public const ushort C_OPENNPC = 2031;

		// Token: 0x0400015E RID: 350
		public const ushort C_NPCREPLY = 2032;

		// Token: 0x0400015F RID: 351
		public const ushort C_EQUIPOPERATION = 2036;

		// Token: 0x04000160 RID: 352
		public const ushort C_GUANJUE = 2060;

		// Token: 0x04000161 RID: 353
		public const ushort C_DANCING = 1049;

		// Token: 0x04000162 RID: 354
		public const ushort C_MOVE = 3005;
	}
}
