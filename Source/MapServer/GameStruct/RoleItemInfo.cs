using System;
using MapServer;

namespace GameStruct
{
	// Token: 0x0200002F RID: 47
	public class RoleItemInfo
	{
		// Token: 0x0600015E RID: 350 RVA: 0x0000FC00 File Offset: 0x0000DE00
		public RoleItemInfo()
		{
			this.id = 0U;
			this.itemid = 0U;
			this.postion = 0;
			this.stronglv = 0;
			this.gemcount = 0;
			this.gem1 = 0U;
			this.gem2 = 0U;
			this.forgename = "";
			this.amount = 0;
			this.war_ghost_exp = 0;
			this.di_attack = 0;
			this.shui_attack = 0;
			this.huo_attack = 0;
			this.feng_attack = 0;
			this.property = 0;
			this.gem3 = 0U;
			this.god_strong = 0;
			this.god_exp = 0;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000FC98 File Offset: 0x0000DE98
		public byte GetStrongLevel()
		{
			return this.stronglv;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000FCB0 File Offset: 0x0000DEB0
		public void UpStrongLevel(byte lv)
		{
			this.stronglv += lv;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000FCC4 File Offset: 0x0000DEC4
		public bool DecStrongLevel()
		{
			bool result;
			if (this.stronglv == 0)
			{
				result = false;
			}
			else
			{
				this.stronglv -= 1;
				result = true;
			}
			return result;
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000FCFA File Offset: 0x0000DEFA
		public void UpQuality()
		{
			this.itemid += 1U;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000FD0C File Offset: 0x0000DF0C
		public int GetQuality()
		{
			string text = this.itemid.ToString();
			return Convert.ToInt32(text.Substring(text.Length - 1));
		}

		// Token: 0x06000164 RID: 356 RVA: 0x0000FD40 File Offset: 0x0000DF40
		public int GetLevel()
		{
			int result;
			if (this.IsShield() || this.IsArmor() || this.IsHelmet())
			{
				result = (int)(this.itemid % 100U / 10U);
			}
			else
			{
				result = (int)(this.itemid % 1000U / 10U);
			}
			return result;
		}

		// Token: 0x06000165 RID: 357 RVA: 0x0000FD94 File Offset: 0x0000DF94
		public void UpLevel()
		{
			uint num = this.itemid + 10U;
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(num);
			if (itemTypeInfo != null)
			{
				this.itemid = num;
			}
		}

		// Token: 0x06000166 RID: 358 RVA: 0x0000FDCC File Offset: 0x0000DFCC
		public bool IsEquip()
		{
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(this.itemid);
			uint type = itemTypeInfo.id;
			return !this.IsArrowSort(type) && ((this.GetItemSort() >= 1 && this.GetItemSort() <= 6) || this.IsShield());
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000FE20 File Offset: 0x0000E020
		public bool IsArrowSort(uint type)
		{
			return type == 170001U || type == 1710001U;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000FE54 File Offset: 0x0000E054
		public int GetItemSort()
		{
			return (int)(this.itemid % 10000000U / 100000U);
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000FE78 File Offset: 0x0000E078
		public bool IsShield()
		{
			return this.GetItemSort() == -1;
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000FE94 File Offset: 0x0000E094
		public int GetItemType()
		{
			int result;
			if (this.GetItemSort() == 4)
			{
				result = (int)(this.itemid % 100000U / 1000U * 1000U);
			}
			else
			{
				result = (int)(this.itemid % 100000U / 10000U * 10000U);
			}
			return result;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000FEEC File Offset: 0x0000E0EC
		public bool IsArmor()
		{
			return this.IsFinery() && this.GetItemType() == 30000;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000FF18 File Offset: 0x0000E118
		public bool IsFinery()
		{
			return this.GetItemSort() == 1;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000FF34 File Offset: 0x0000E134
		public bool IsHelmet()
		{
			return this.IsFinery() && this.GetItemType() == 10000;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000FF60 File Offset: 0x0000E160
		public int GetGemCount()
		{
			int num = 0;
			if (this.gem1 != 0U)
			{
				num++;
			}
			if (this.gem2 != 0U)
			{
				num++;
			}
			if (this.gem3 != 0U)
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000FFA8 File Offset: 0x0000E1A8
		public byte GetGemType(byte index)
		{
			byte result;
			switch (index)
			{
			case 0:
				result = (byte)this.gem1;
				break;
			case 1:
				result = (byte)this.gem2;
				break;
			case 2:
				result = (byte)this.gem3;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000FFF4 File Offset: 0x0000E1F4
		public void SetGemType(byte index, byte value)
		{
			switch (index)
			{
			case 0:
				this.gem1 = (uint)value;
				break;
			case 1:
				this.gem2 = (uint)value;
				break;
			case 2:
				this.gem3 = (uint)value;
				break;
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00010038 File Offset: 0x0000E238
		public void OpenGem(byte index)
		{
			switch (index)
			{
			case 0:
				this.gem1 = 255U;
				break;
			case 1:
				this.gem2 = 255U;
				break;
			case 2:
				this.gem3 = 255U;
				break;
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00010088 File Offset: 0x0000E288
		public bool IsGem()
		{
			return ConfigManager.Instance().GetGemInfo(this.itemid) != null;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000100B4 File Offset: 0x0000E2B4
		public byte GetGemType()
		{
			GemInfo gemInfo = ConfigManager.Instance().GetGemInfo(this.itemid);
			byte result;
			if (gemInfo != null)
			{
				result = gemInfo.gemtype;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0400020D RID: 525
		private const int ITEMSORT_INVALID = -1;

		// Token: 0x0400020E RID: 526
		private const int ITEMSORT_EXPEND = 10;

		// Token: 0x0400020F RID: 527
		private const int IETMSORT_FINERY = 1;

		// Token: 0x04000210 RID: 528
		private const int ITEMSORT_WEAPON1 = 4;

		// Token: 0x04000211 RID: 529
		private const int ITEMSORT_MOUNT = 6;

		// Token: 0x04000212 RID: 530
		private const int ITEMSORT_OTHER = 7;

		// Token: 0x04000213 RID: 531
		public const int ITEMTYPE_HELMET = 10000;

		// Token: 0x04000214 RID: 532
		public const int ITEMTYPE_NECKLACE = 20000;

		// Token: 0x04000215 RID: 533
		public const int ITEMTYPE_ARMOR = 30000;

		// Token: 0x04000216 RID: 534
		public const int ITEMTYPE_BANGLE = 40000;

		// Token: 0x04000217 RID: 535
		public const int ITEMTYPE_MANTLE = 50000;

		// Token: 0x04000218 RID: 536
		public const int ITEMTYPE_SHOES = 60000;

		// Token: 0x04000219 RID: 537
		public uint id;

		// Token: 0x0400021A RID: 538
		public uint itemid;

		// Token: 0x0400021B RID: 539
		public ushort postion;

		// Token: 0x0400021C RID: 540
		public byte stronglv;

		// Token: 0x0400021D RID: 541
		public byte gemcount;

		// Token: 0x0400021E RID: 542
		public uint gem1;

		// Token: 0x0400021F RID: 543
		public uint gem2;

		// Token: 0x04000220 RID: 544
		public string forgename;

		// Token: 0x04000221 RID: 545
		public ushort amount;

		// Token: 0x04000222 RID: 546
		public int war_ghost_exp;

		// Token: 0x04000223 RID: 547
		public byte di_attack;

		// Token: 0x04000224 RID: 548
		public byte shui_attack;

		// Token: 0x04000225 RID: 549
		public byte huo_attack;

		// Token: 0x04000226 RID: 550
		public byte feng_attack;

		// Token: 0x04000227 RID: 551
		public int property;

		// Token: 0x04000228 RID: 552
		public uint gem3;

		// Token: 0x04000229 RID: 553
		public int god_strong;

		// Token: 0x0400022A RID: 554
		public int god_exp;

		// Token: 0x0400022B RID: 555
		public uint typeid;
	}
}
