using System;

namespace GameStruct
{
	// Token: 0x02000023 RID: 35
	public class MagicTypeInfo
	{
		// Token: 0x0600013F RID: 319 RVA: 0x0000F100 File Offset: 0x0000D300
		public MagicTypeInfo()
		{
			this.id = 0U;
			this.typeid = 0U;
			this.sort = 0;
			this.name = "";
			this.crime = 0U;
			this.ground = 0U;
			this.multi = 0U;
			this.target = 0U;
			this.level = 0;
			this.use_mp = 0U;
			this.use_potential = 0U;
			this.power = 0U;
			this.intone_speed = 0U;
			this.percent = 0U;
			this.step_secs = 0U;
			this.range = 0U;
			this.distance = 0U;
			this.status_chance = 0U;
			this.status = 0U;
			this.need_prof = 0U;
			this.need_exp = 0U;
			this.need_level = 0U;
			this.need_gemtype = 0U;
			this.use_xp = 0U;
			this.weapon_subtype = 0U;
			this.active_times = 0U;
			this.auto_active = 0U;
			this.floor_attr = 0U;
			this.auto_learn = 0U;
			this.learn_level = 0U;
			this.drop_weapon = 0U;
			this.use_ep = 0U;
			this.weapon_hit = 0U;
			this.use_item = 0U;
			this.next_magic = 0U;
			this.delay_ms = 0U;
			this.use_item_num = 0U;
			this.width = 0U;
			this.durability = 0U;
			this.apply_ms = 0U;
			this.track_id = 0U;
			this.track_id2 = 0U;
			this.auto_learn_prob = 0U;
			this.group_type = 0U;
			this.group_member1_pos = 0U;
			this.group_member2_pos = 0U;
			this.group_member3_pos = 0U;
			this.magic1 = 0U;
			this.magic2 = 0U;
			this.magic3 = 0U;
			this.magic4 = 0U;
			this.attack_combine = 0U;
			this.flag = 0U;
		}

		// Token: 0x040000F3 RID: 243
		public const byte MAGICSORT_ATTACK = 1;

		// Token: 0x040000F4 RID: 244
		public const byte MAGICSORT_RECRUIT = 2;

		// Token: 0x040000F5 RID: 245
		public const byte MAGICSORT_CROSS = 3;

		// Token: 0x040000F6 RID: 246
		public const byte MAGICSORT_FAN = 4;

		// Token: 0x040000F7 RID: 247
		public const byte MAGICSORT_BOMB = 5;

		// Token: 0x040000F8 RID: 248
		public const byte MAGICSORT_ATTACHSTATUS = 6;

		// Token: 0x040000F9 RID: 249
		public const byte MAGICSORT_DETACHSTATUS = 7;

		// Token: 0x040000FA RID: 250
		public const byte MAGICSORT_SQUARE = 8;

		// Token: 0x040000FB RID: 251
		public const byte MAGICSORT_JUMPATTACK = 9;

		// Token: 0x040000FC RID: 252
		public const byte MAGICSORT_RANDOMTRANS = 10;

		// Token: 0x040000FD RID: 253
		public const byte MAGICSORT_DISPATCHXP = 11;

		// Token: 0x040000FE RID: 254
		public const byte MAGICSORT_COLLIDE = 12;

		// Token: 0x040000FF RID: 255
		public const byte MAGICSORT_SERIALCUT = 13;

		// Token: 0x04000100 RID: 256
		public const byte MAGICSORT_LINE = 14;

		// Token: 0x04000101 RID: 257
		public const byte MAGICSORT_ATKRANGE = 15;

		// Token: 0x04000102 RID: 258
		public const byte MAGICSORT_ATKSTATUS = 16;

		// Token: 0x04000103 RID: 259
		public const byte MAGICSORT_CALLTEAMMEMBER = 17;

		// Token: 0x04000104 RID: 260
		public const byte MAGICSORT_RECORDTRANSSPELL = 18;

		// Token: 0x04000105 RID: 261
		public const byte MAGICSORT_TRANSFORM = 19;

		// Token: 0x04000106 RID: 262
		public const byte MAGICSORT_ADDMANA = 20;

		// Token: 0x04000107 RID: 263
		public const byte MAGICSORT_LAYTRAP = 21;

		// Token: 0x04000108 RID: 264
		public const byte MAGICSORT_DANCE = 22;

		// Token: 0x04000109 RID: 265
		public const byte MAGICSORT_CALLPET = 23;

		// Token: 0x0400010A RID: 266
		public const byte MAGICSORT_VAMPIRE = 24;

		// Token: 0x0400010B RID: 267
		public const byte MAGICSORT_INSTEAD = 25;

		// Token: 0x0400010C RID: 268
		public const byte MAGICSORT_DECLIFE = 26;

		// Token: 0x0400010D RID: 269
		public const byte MAGICSORT_GROUNDSTING = 27;

		// Token: 0x0400010E RID: 270
		public const byte MAGICSORT_REBORN = 28;

		// Token: 0x0400010F RID: 271
		public const byte MAGICSORT_TEAM_MAGIC = 29;

		// Token: 0x04000110 RID: 272
		public const byte MAGICSORT_BOMB_LOCKALL = 30;

		// Token: 0x04000111 RID: 273
		public const byte MAGICSORT_SORB_SOUL = 31;

		// Token: 0x04000112 RID: 274
		public const byte MAGICSORT_STEAL = 32;

		// Token: 0x04000113 RID: 275
		public const byte MAGICSORT_LINE_PENETRABLE = 33;

		// Token: 0x04000114 RID: 276
		public const byte MAGICSORT_DRAGON_MOLONGSHOUHU = 40;

		// Token: 0x04000115 RID: 277
		public const byte MAGICSORT_POINTBOMB = 41;

		// Token: 0x04000116 RID: 278
		public const byte MAGICSORT_DRAGON_QISHITUANSHOUHU = 42;

		// Token: 0x04000117 RID: 279
		public const byte MAGICSORT_DRAGON_QISHITUANCHONGFENG = 43;

		// Token: 0x04000118 RID: 280
		public const byte MAGICSORT_JUMP_ATTACK = 81;

		// Token: 0x04000119 RID: 281
		public const byte MAGICSORT_JUMPBOMB = 82;

		// Token: 0x0400011A RID: 282
		public const byte MAGICSORT_STEALTH = 83;

		// Token: 0x0400011B RID: 283
		public const byte MAGICSORT_HIDEDEN = 84;

		// Token: 0x0400011C RID: 284
		public const byte MAGICSORT_YUANSUZHANGKONG = 85;

		// Token: 0x0400011D RID: 285
		public const byte MAGICSORT_LIUXINGYUNHUO = 86;

		// Token: 0x0400011E RID: 286
		public const byte MAGICSORT_JUYANSHENGDUN = 87;

		// Token: 0x0400011F RID: 287
		public const byte MAGICSORT_YUANSUZHAOHUAN = 88;

		// Token: 0x04000120 RID: 288
		public const byte MAGICSORT_ZHAOHUANWUHUAN = 90;

		// Token: 0x04000121 RID: 289
		public const byte MAGICSORT_JIANGLINGZHOUYU = 92;

		// Token: 0x04000122 RID: 290
		public const byte MAGICSORT_ANSHAXIELONG = 93;

		// Token: 0x04000123 RID: 291
		public const byte MAGICSORT_MINGGUOSHENGNV = 94;

		// Token: 0x04000124 RID: 292
		public const byte MAGICSORT_WANGNIANWULING = 95;

		// Token: 0x04000125 RID: 293
		public const byte MAGICSORT_SHENYUANELING = 96;

		// Token: 0x04000126 RID: 294
		public const byte MAGICSORT_DIYUXIEFU = 97;

		// Token: 0x04000127 RID: 295
		public const byte MAGICSORT_SHIHUNWULING = 98;

		// Token: 0x04000128 RID: 296
		public const byte MAGICSORT_GULINGQIYUE = 99;

		// Token: 0x04000129 RID: 297
		public const byte MAGICSORT_MIXINSHU = 100;

		// Token: 0x0400012A RID: 298
		public const byte MAGICSORT_SINGLE_DANCING = 101;

		// Token: 0x0400012B RID: 299
		public const byte MAGICSORT_DOUBLE_DANCING = 102;

		// Token: 0x0400012C RID: 300
		public const uint SILIANZHAN = 1005U;

		// Token: 0x0400012D RID: 301
		public const uint FEITIANZHAN = 1007U;

		// Token: 0x0400012E RID: 302
		public const uint LIULIANZHAN = 1009U;

		// Token: 0x0400012F RID: 303
		public const uint FEITIANLIANZHAN = 1010U;

		// Token: 0x04000130 RID: 304
		public const uint LONGHUNFENGBAO = 1021U;

		// Token: 0x04000131 RID: 305
		public const uint LEITINGWANJUN = 3021U;

		// Token: 0x04000132 RID: 306
		public const uint LONGQIANGLIEHUN = 5212U;

		// Token: 0x04000133 RID: 307
		public const uint LONGQIANGZANGHUN = 5213U;

		// Token: 0x04000134 RID: 308
		public const uint MOLONGSHOUHU = 5225U;

		// Token: 0x04000135 RID: 309
		public const uint LONGQIANGSUIHUN = 5242U;

		// Token: 0x04000136 RID: 310
		public const uint YANHUNQIANG_LIEDI = 5214U;

		// Token: 0x04000137 RID: 311
		public const uint YANHUNQIANG_LIUYAN = 5217U;

		// Token: 0x04000138 RID: 312
		public const uint LIUXINGYUNHUO = 5302U;

		// Token: 0x04000139 RID: 313
		public const uint HEILONGWU = 6008U;

		// Token: 0x0400013A RID: 314
		public const uint LIEHUNSHAN = 6009U;

		// Token: 0x0400013B RID: 315
		public const uint WUNUSHIHUN = 6017U;

		// Token: 0x0400013C RID: 316
		public const uint ZHENSHIDAJI = 7003U;

		// Token: 0x0400013D RID: 317
		public const uint XUEXI = 7007U;

		// Token: 0x0400013E RID: 318
		public const uint SHUNYINGJI = 7009U;

		// Token: 0x0400013F RID: 319
		public const uint XUEYINGLUNHUI = 7011U;

		// Token: 0x04000140 RID: 320
		public const uint XUEYINGQIANHUAN = 7010U;

		// Token: 0x04000141 RID: 321
		public const uint XUEYINGXINGMANG = 7016U;

		// Token: 0x04000142 RID: 322
		public const uint XUEYUXUANWO = 7014U;

		// Token: 0x04000143 RID: 323
		public uint id;

		// Token: 0x04000144 RID: 324
		public uint typeid;

		// Token: 0x04000145 RID: 325
		public byte sort;

		// Token: 0x04000146 RID: 326
		public string name;

		// Token: 0x04000147 RID: 327
		public uint crime;

		// Token: 0x04000148 RID: 328
		public uint ground;

		// Token: 0x04000149 RID: 329
		public uint multi;

		// Token: 0x0400014A RID: 330
		public uint target;

		// Token: 0x0400014B RID: 331
		public byte level;

		// Token: 0x0400014C RID: 332
		public uint use_mp;

		// Token: 0x0400014D RID: 333
		public uint use_potential;

		// Token: 0x0400014E RID: 334
		public uint power;

		// Token: 0x0400014F RID: 335
		public uint intone_speed;

		// Token: 0x04000150 RID: 336
		public uint percent;

		// Token: 0x04000151 RID: 337
		public uint step_secs;

		// Token: 0x04000152 RID: 338
		public uint range;

		// Token: 0x04000153 RID: 339
		public uint distance;

		// Token: 0x04000154 RID: 340
		public uint status_chance;

		// Token: 0x04000155 RID: 341
		public uint status;

		// Token: 0x04000156 RID: 342
		public uint need_prof;

		// Token: 0x04000157 RID: 343
		public uint need_exp;

		// Token: 0x04000158 RID: 344
		public uint need_level;

		// Token: 0x04000159 RID: 345
		public uint need_gemtype;

		// Token: 0x0400015A RID: 346
		public uint use_xp;

		// Token: 0x0400015B RID: 347
		public uint weapon_subtype;

		// Token: 0x0400015C RID: 348
		public uint active_times;

		// Token: 0x0400015D RID: 349
		public uint auto_active;

		// Token: 0x0400015E RID: 350
		public uint floor_attr;

		// Token: 0x0400015F RID: 351
		public uint auto_learn;

		// Token: 0x04000160 RID: 352
		public uint learn_level;

		// Token: 0x04000161 RID: 353
		public uint drop_weapon;

		// Token: 0x04000162 RID: 354
		public uint use_ep;

		// Token: 0x04000163 RID: 355
		public uint weapon_hit;

		// Token: 0x04000164 RID: 356
		public uint use_item;

		// Token: 0x04000165 RID: 357
		public uint next_magic;

		// Token: 0x04000166 RID: 358
		public uint delay_ms;

		// Token: 0x04000167 RID: 359
		public uint use_item_num;

		// Token: 0x04000168 RID: 360
		public uint width;

		// Token: 0x04000169 RID: 361
		public uint durability;

		// Token: 0x0400016A RID: 362
		public uint apply_ms;

		// Token: 0x0400016B RID: 363
		public uint track_id;

		// Token: 0x0400016C RID: 364
		public uint track_id2;

		// Token: 0x0400016D RID: 365
		public uint auto_learn_prob;

		// Token: 0x0400016E RID: 366
		public uint group_type;

		// Token: 0x0400016F RID: 367
		public uint group_member1_pos;

		// Token: 0x04000170 RID: 368
		public uint group_member2_pos;

		// Token: 0x04000171 RID: 369
		public uint group_member3_pos;

		// Token: 0x04000172 RID: 370
		public uint magic1;

		// Token: 0x04000173 RID: 371
		public uint magic2;

		// Token: 0x04000174 RID: 372
		public uint magic3;

		// Token: 0x04000175 RID: 373
		public uint magic4;

		// Token: 0x04000176 RID: 374
		public uint attack_combine;

		// Token: 0x04000177 RID: 375
		public uint flag;
	}
}
