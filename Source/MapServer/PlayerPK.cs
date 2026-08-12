using System;
using GameBase.Core;
using GameStruct;

namespace MapServer
{
	// Token: 0x02000094 RID: 148
	public class PlayerPK
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x0002A9BB File Offset: 0x00028BBB
		public PlayerPK(PlayerObject _play)
		{
			this.play = _play;
			this.mDecTime = new TimeOut();
			this.mDecTime.SetInterval(60);
			this.mnNameType = this.GetNameType();
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0002A9F4 File Offset: 0x00028BF4
		public void Run()
		{
			if (this.play.GetTimerSystem().QueryStatus(2) == null)
			{
				if (this.mDecTime.ToNextTime() && this.play.GetBaseAttr().pk > 0)
				{
					int value = -1;
					if (this.play.GetGameMap().GetMapInfo().id == 300U)
					{
						value = -3;
					}
					this.mnNameType = this.GetNameType();
					this.play.ChangeAttribute(UserAttribute.PK, value, true);
				}
			}
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0002AA94 File Offset: 0x00028C94
		public void Die(BaseObject target)
		{
			ushort num = 0;
			int min = 0;
			int max = 0;
			int min2 = 0;
			int max2 = 0;
			int num2 = 0;
			switch (this.mnNameType)
			{
			case 0:
				num = 20;
				min = 10;
				max = 50;
				min2 = 10;
				max2 = 50;
				num2 = 1;
				break;
			case 2:
				num = 10;
				min = 50;
				max = 100;
				min2 = 50;
				max2 = 100;
				num2 = 20;
				break;
			case 3:
				min = 100;
				max = 100;
				min2 = 100;
				max2 = 100;
				num2 = 30;
				num = 0;
				break;
			}
			if (this.IsPKing())
			{
				num = 0;
			}
			int num3 = IRandom.Random(min, max);
			int num4 = IRandom.Random(min2, max2);
			int i = (int)((float)this.play.GetItemSystem().GetBagCount() * ((float)num3 / 100f));
			if (i > 0)
			{
				while (i > 0)
				{
					int num5 = IRandom.Random(0, i);
					int num6 = 0;
					foreach (RoleItemInfo roleItemInfo in this.play.GetItemSystem().GetDicItem().Values)
					{
						if (roleItemInfo.postion == 50)
						{
							if (num6 == num5)
							{
								this.play.GetItemSystem().DropItemBag(roleItemInfo.id);
								break;
							}
							num6++;
						}
					}
					i--;
				}
			}
			int num7 = (int)((float)this.play.GetBaseAttr().gold * ((float)num4 / 100f));
			if (num7 > 0)
			{
				this.play.GetItemSystem().DropGold(num7);
			}
			long num8 = (long)(this.play.GetBaseAttr().exp * (num2 / 100));
			if (num8 > 0L)
			{
				this.play.ChangeAttribute(UserAttribute.EXP, (int)(-(int)num8), true);
			}
			if (num > 0 && target.type == 2)
			{
				(target as PlayerObject).ChangeAttribute(UserAttribute.PK, (int)num, true);
			}
			this.SetPKIng(false, true);
			if (this.mnNameType == 3)
			{
				int num5 = 0;
				RoleItemInfo equipByPostion;
				for (;;)
				{
					byte b = (byte)IRandom.Random(3, 8);
					if (b != 5)
					{
						equipByPostion = this.play.GetItemSystem().GetEquipByPostion(b);
						if (equipByPostion != null)
						{
							break;
						}
						num5++;
						if (num5 >= 8)
						{
							goto Block_12;
						}
					}
				}
				this.play.GetItemSystem().DropItemEquip(equipByPostion.id);
				Block_12:
				this.play.ChangeMap(300U, 76, 86);
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x0002AD84 File Offset: 0x00028F84
		public void SetPKIng(bool v, bool bCrime = true)
		{
			if (v && bCrime)
			{
				this.play.GetTimerSystem().AddStatus(2, 30, true);
			}
			else
			{
				this.mnNameType = this.GetNameType();
			}
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0002ADC8 File Offset: 0x00028FC8
		public bool IsPKing()
		{
			return this.play.GetTimerSystem().QueryStatus(2) != null;
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0002ADF4 File Offset: 0x00028FF4
		public int GetNameType()
		{
			short pk = this.play.GetBaseAttr().pk;
			int result;
			if (pk < 20)
			{
				result = 0;
			}
			else if (pk < 100 && pk >= 20)
			{
				result = 2;
			}
			else
			{
				result = 3;
			}
			return result;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0002AE40 File Offset: 0x00029040
		public void ResetPKNameType()
		{
			if (this.play.GetTimerSystem().QueryStatus(2) == null)
			{
				short pk = this.play.GetBaseAttr().pk;
				if (pk >= 20 && pk < 100)
				{
					this.play.GetTimerSystem().AddStatus(1001, 0, true);
				}
				else if (pk >= 100)
				{
					this.play.GetTimerSystem().AddStatus(1002, 0, true);
				}
			}
		}

		// Token: 0x04000645 RID: 1605
		private PlayerObject play;

		// Token: 0x04000646 RID: 1606
		private TimeOut mDecTime;

		// Token: 0x04000647 RID: 1607
		private int mnNameType;
	}
}
