using System;
using GameStruct;

namespace MapServer
{
	// Token: 0x0200000B RID: 11
	public class BattleSystem
	{
		// Token: 0x06000074 RID: 116 RVA: 0x00004D64 File Offset: 0x00002F64
		public static int AdjustExp(int nDamage, int nAtkLev, int nDefLev)
		{
			if (nAtkLev > 135)
			{
				nAtkLev = 125;
			}
			int num = nDamage;
			int nameType = MonsterNameType.GetNameType(nAtkLev, nDefLev);
			int num2 = nAtkLev - nDefLev;
			if (nameType == 0)
			{
				if (num2 >= 3 && num2 <= 5)
				{
					num = num * 70 / 100;
				}
				else if (num2 > 5 && num2 <= 10)
				{
					num = num * 20 / 100;
				}
				else if (num2 > 10 && num2 <= 20)
				{
					num = num * 10 / 100;
				}
				else if (num2 > 20)
				{
					num = num * 5 / 100;
				}
			}
			else if (nameType == 2)
			{
				num = (int)((double)num * 1.3);
			}
			else if (nameType == 3)
			{
				if (num2 >= -10 && num2 < -5)
				{
					num *= 5;
				}
				else if (num2 >= -20 && num2 < -10)
				{
					num = (int)((double)num * 1.8);
				}
				else if (num2 < -20)
				{
					num = (int)((double)num * 2.3);
				}
			}
			return (num < 0) ? 0 : num;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004EB0 File Offset: 0x000030B0
		public static byte AdjustDrop(BaseObject attack, BaseObject Def)
		{
			byte level = attack.GetLevel();
			byte level2 = Def.GetLevel();
			int num = (int)(level - level2);
			byte result;
			if (num >= 0)
			{
				if (num <= 5 && num > 3)
				{
					result = (byte)((IRandom.Random(0, 100) < 50) ? 2 : 0);
				}
				else if (num <= 9 && num > 6)
				{
					result = (byte)((IRandom.Random(0, 100) < 50) ? 1 : 0);
				}
				else
				{
					result = (byte)((IRandom.Random(0, 100) < 50) ? 3 : 1);
				}
			}
			else if (num <= -5 && num > -3)
			{
				result = (byte)((IRandom.Random(0, 100) < 50) ? 2 : 1);
			}
			else if (num <= -9 && num > -6)
			{
				result = (byte)((IRandom.Random(0, 100) < 50) ? 1 : 0);
			}
			else
			{
				result = (byte)((IRandom.Random(0, 100) < 50) ? 3 : 1);
			}
			return result;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004FBC File Offset: 0x000031BC
		private static uint AdjustDamage(PlayerObject attack, PlayerObject def, bool isMagicAck = false)
		{
			int fightSoul = attack.GetFightSoul();
			int fightSoul2 = def.GetFightSoul();
			int num = fightSoul - fightSoul2;
			int num2 = 0;
			int num3 = 0;
			if (num > 0)
			{
				num2 = num * 100;
			}
			else
			{
				num3 = Math.Abs(num) * 100;
			}
			int num4 = 50 + attack.GetLuck();
			int num5;
			if (IRandom.Random(0, 100) < num4)
			{
				if (isMagicAck && attack.GetBaseAttr().profession == 10)
				{
					num5 = attack.GetMinAck() + IRandom.Random(0, attack.GetMaxMagixAck() - attack.GetMagicAck());
				}
				else
				{
					num5 = attack.GetMaxAck() + IRandom.Random(0, attack.GetMaxAck() - attack.GetMinAck());
				}
			}
			else if (isMagicAck && attack.GetBaseAttr().profession == 10)
			{
				num5 = attack.GetMinAck() + IRandom.Random(0, attack.GetMagicAck() - attack.GetMagicAck());
			}
			else
			{
				num5 = attack.GetMinAck() + IRandom.Random(0, attack.GetMaxAck() - attack.GetMinAck());
			}
			num5 += num2;
			int num6;
			if (isMagicAck)
			{
				num6 = def.GetMagicDefense();
			}
			else
			{
				num6 = def.GetDefense();
			}
			num6 += num3;
			int num7 = num5 - num6;
			if (attack.type == 2)
			{
				num7 += (int)(attack.GetLevel() / 10);
			}
			if (num7 <= 0)
			{
				num7 = ((IRandom.Random(1, 100) >= 50) ? 1 : 0);
				if (!isMagicAck)
				{
					num7 = 1;
				}
			}
			return (uint)num7;
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005174 File Offset: 0x00003374
		public static uint AdjustDamage(BaseObject attack, BaseObject def, bool isMagicAck = false)
		{
			uint result;
			if (attack.type == 2 && def.type == 2)
			{
				result = BattleSystem.AdjustDamage(attack as PlayerObject, def as PlayerObject, isMagicAck);
			}
			else
			{
				int num = 50 + attack.GetLuck();
				int num2;
				if (IRandom.Random(0, 100) < num)
				{
					if (isMagicAck && attack.type == 2 && (attack as PlayerObject).GetBaseAttr().profession == 10)
					{
						num2 = attack.GetMagicAck() + IRandom.Random(0, attack.GetMaxMagixAck() - attack.GetMagicAck());
					}
					else
					{
						num2 = attack.GetMaxAck() + IRandom.Random(0, attack.GetMaxAck() - attack.GetMinAck());
					}
				}
				else if (isMagicAck && attack.type == 2 && (attack as PlayerObject).GetBaseAttr().profession == 10)
				{
					num2 = attack.GetMagicAck() + IRandom.Random(0, attack.GetMaxMagixAck() - attack.GetMagicAck());
				}
				else
				{
					num2 = attack.GetMinAck() + IRandom.Random(0, attack.GetMaxAck() - attack.GetMinAck());
				}
				int num3;
				if (isMagicAck)
				{
					num3 = def.GetMagicDefense();
				}
				else
				{
					num3 = def.GetDefense();
				}
				int num4 = num2 - num3;
				if (attack.type == 2)
				{
					num4 += (int)(attack.GetLevel() / 10);
				}
				if (num4 <= 0)
				{
					num4 = ((IRandom.Random(1, 100) >= 50) ? 1 : 0);
					if (!isMagicAck)
					{
						num4 = 1;
					}
				}
				result = (uint)num4;
			}
			return result;
		}

		// Token: 0x04000045 RID: 69
		public const byte EXPLODE_ITEM_CHANCE1 = 0;

		// Token: 0x04000046 RID: 70
		public const byte EXPLODE_ITEM_CHANCE2 = 1;

		// Token: 0x04000047 RID: 71
		public const byte EXPLODE_ITEM_CHANCE3 = 2;

		// Token: 0x04000048 RID: 72
		public const byte EXPLODE_ITEM_CHANCE4 = 3;
	}
}
