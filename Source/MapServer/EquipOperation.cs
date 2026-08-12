using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000012 RID: 18
	public class EquipOperation
	{
		// Token: 0x060000CB RID: 203 RVA: 0x00009BE8 File Offset: 0x00007DE8
		public EquipOperation()
		{
			this.mListStrong = new List<EquipStrongInfo>();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00009C00 File Offset: 0x00007E00
		public static EquipOperation Instance()
		{
			if (EquipOperation.mInstance == null)
			{
				EquipOperation.mInstance = new EquipOperation();
			}
			return EquipOperation.mInstance;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00009C34 File Offset: 0x00007E34
		public bool Load()
		{
			VerPacket verPacket = ConfigManager.Instance().GetVerPacket();
			string text = verPacket.LoadFileToText("data/config/EquipStrong.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				EquipStrongInfo equipStrongInfo = new EquipStrongInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				equipStrongInfo.level = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "chance");
				equipStrongInfo.chance = Convert.ToInt32(fieldInfoToValue);
				this.mListStrong.Add(equipStrongInfo);
			}
			return true;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009CD0 File Offset: 0x00007ED0
		public void EquipQuality(PlayerObject play, uint srcid, uint materialid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(srcid);
			RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(materialid);
			if (roleItemInfo != null && roleItemInfo2 != null)
			{
				if (roleItemInfo.GetQuality() != 7)
				{
					int num = IRandom.Random(1, 100);
					bool flag = false;
					if (play.GetItemSystem().IsEquip(roleItemInfo.itemid))
					{
						MsgEquipOperationRet msgEquipOperationRet = new MsgEquipOperationRet();
						msgEquipOperationRet.Create(null, play.GetGamePackKeyEx());
						msgEquipOperationRet.srcid = srcid;
						msgEquipOperationRet.destid = materialid;
						msgEquipOperationRet.type = 196610U;
						if (roleItemInfo2.itemid == 1037160U || roleItemInfo2.itemid == 1037200U)
						{
							if (!play.GetItemSystem().DeleteItemByID(materialid))
							{
								return;
							}
							if (num < this.RateSuccForQuality(roleItemInfo))
							{
								roleItemInfo.UpQuality();
								flag = true;
								msgEquipOperationRet.ret = 1U;
							}
						}
						else if (roleItemInfo2.itemid == 1037169U && roleItemInfo.GetQuality() <= 4)
						{
							if (!play.GetItemSystem().DeleteItemByID(materialid))
							{
								return;
							}
							roleItemInfo.UpQuality();
							flag = true;
							msgEquipOperationRet.ret = 1U;
						}
						if (flag)
						{
							if (roleItemInfo.forgename.Length == 0)
							{
								roleItemInfo.forgename = play.GetName();
							}
							play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
						}
						play.SendData(msgEquipOperationRet.GetBuffer(), false);
					}
				}
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00009E84 File Offset: 0x00008084
		public void EquipStrong(PlayerObject play, uint srcid, uint materialid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(srcid);
			RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(materialid);
			if (roleItemInfo != null && roleItemInfo2 != null)
			{
				if (roleItemInfo.GetStrongLevel() < 12)
				{
					if ((int)roleItemInfo.GetStrongLevel() < this.mListStrong.Count)
					{
						int num = IRandom.Random(1, 100);
						bool flag = false;
						MsgEquipOperationRet msgEquipOperationRet = new MsgEquipOperationRet();
						msgEquipOperationRet.Create(null, play.GetGamePackKeyEx());
						msgEquipOperationRet.srcid = srcid;
						msgEquipOperationRet.destid = materialid;
						msgEquipOperationRet.type = 196611U;
						if (roleItemInfo2.itemid == 1037150U || roleItemInfo.GetStrongLevel() <= 9)
						{
							if (roleItemInfo2.itemid == 1037150U)
							{
								if (!play.GetItemSystem().DeleteItemByID(materialid))
								{
									return;
								}
								if (num < this.mListStrong[(int)roleItemInfo.GetStrongLevel()].chance)
								{
									roleItemInfo.UpStrongLevel(1);
									flag = true;
									msgEquipOperationRet.ret = 1U;
								}
								else
								{
									msgEquipOperationRet.ret = 0U;
									if (roleItemInfo.GetStrongLevel() > 9 && roleItemInfo.DecStrongLevel())
									{
										flag = true;
									}
								}
							}
							else if (roleItemInfo2.itemid == 1037159U)
							{
								if (!play.GetItemSystem().DeleteItemByID(materialid))
								{
									return;
								}
								roleItemInfo.UpStrongLevel(1);
								flag = true;
								msgEquipOperationRet.ret = 1U;
							}
							if (flag)
							{
								play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
							}
							play.SendData(msgEquipOperationRet.GetBuffer(), false);
						}
					}
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000A05C File Offset: 0x0000825C
		public void EquipLevel(PlayerObject play, uint srcid, uint materialid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(srcid);
			RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(materialid);
			if (roleItemInfo != null && roleItemInfo2 != null)
			{
				bool flag = false;
				MsgEquipOperationRet msgEquipOperationRet = new MsgEquipOperationRet();
				msgEquipOperationRet.Create(null, play.GetGamePackKeyEx());
				msgEquipOperationRet.srcid = srcid;
				msgEquipOperationRet.destid = materialid;
				msgEquipOperationRet.type = 196612U;
				int num = this.RateSuccForEquipLevel(roleItemInfo);
				if (roleItemInfo.IsShield() || roleItemInfo.IsArmor() || roleItemInfo.IsHelmet())
				{
					if (roleItemInfo.GetLevel() > 9)
					{
						return;
					}
					if (roleItemInfo2.itemid == 1037179U)
					{
						if (!play.GetItemSystem().DeleteItemByID(materialid))
						{
							return;
						}
						roleItemInfo.UpLevel();
						flag = true;
						msgEquipOperationRet.ret = 1U;
					}
					else if (roleItemInfo2.itemid == 1037170U)
					{
						if (!play.GetItemSystem().DeleteItemByID(materialid))
						{
							return;
						}
						if (IRandom.Random(1, 100) < num)
						{
							roleItemInfo.UpLevel();
							flag = true;
							msgEquipOperationRet.ret = 1U;
						}
					}
				}
				else
				{
					if (roleItemInfo.GetLevel() > 25)
					{
						return;
					}
					if (roleItemInfo2.itemid == 1037179U)
					{
						if (!play.GetItemSystem().DeleteItemByID(materialid))
						{
							return;
						}
						roleItemInfo.UpLevel();
						msgEquipOperationRet.ret = 1U;
						flag = true;
					}
					else if (roleItemInfo2.itemid == 1037170U)
					{
						if (!play.GetItemSystem().DeleteItemByID(materialid))
						{
							return;
						}
						if (IRandom.Random(1, 100) < num)
						{
							roleItemInfo.UpLevel();
							msgEquipOperationRet.ret = 1U;
							flag = true;
						}
					}
				}
				if (flag)
				{
					play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
				}
				play.SendData(msgEquipOperationRet.GetBuffer(), false);
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000A290 File Offset: 0x00008490
		private int RateSuccForQuality(RoleItemInfo item)
		{
			int quality = item.GetQuality();
			int result;
			if (quality == 0)
			{
				result = 30;
			}
			else if (quality == 1)
			{
				result = 12;
			}
			else if (quality == 2)
			{
				result = 6;
			}
			else if (quality == 3)
			{
				result = 4;
			}
			else if (quality == 4)
			{
				result = 12;
			}
			else if (quality == 5)
			{
				result = 6;
			}
			else if (quality == 6)
			{
				result = 4;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000A318 File Offset: 0x00008518
		private int RateSuccForEquipLevel(RoleItemInfo pEquipItem)
		{
			int level = pEquipItem.GetLevel();
			if (pEquipItem.IsShield() || pEquipItem.IsArmor() || pEquipItem.IsHelmet())
			{
				if (level >= 0 && level < 2)
				{
					return 100;
				}
				if (level >= 2 && level < 4)
				{
					return 35;
				}
				if (level >= 4 && level < 6)
				{
					return 20;
				}
				if (level >= 6 && level < 7)
				{
					return 10;
				}
				if (level >= 7 && level < 8)
				{
					return 7;
				}
				if (level >= 8 && level < 9)
				{
					return 4;
				}
			}
			else
			{
				if (level >= 0 && level < 4)
				{
					return 100;
				}
				if (level >= 4 && level < 7)
				{
					return 35;
				}
				if (level >= 7 && level < 10)
				{
					return 20;
				}
				if (level >= 10 && level < 13)
				{
					return 10;
				}
				if (level >= 13 && level < 16)
				{
					return 7;
				}
				if (level >= 16 && level < 19)
				{
					return 4;
				}
				if (level >= 19 && level < 22)
				{
					return 2;
				}
			}
			return 0;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000A4B4 File Offset: 0x000086B4
		public void OpenGem(PlayerObject play, uint srcid, uint destid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(destid);
			RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(srcid);
			if (roleItemInfo != null && roleItemInfo2 != null)
			{
				uint itemid = roleItemInfo2.itemid;
				byte index;
				if (itemid != 723002U)
				{
					if (itemid != 742178U)
					{
						if (itemid != 820300U)
						{
							return;
						}
						if (roleItemInfo.GetGemCount() != 1)
						{
							return;
						}
						index = 1;
					}
					else
					{
						if (roleItemInfo.GetGemCount() != 2)
						{
							return;
						}
						index = 2;
					}
				}
				else
				{
					if (roleItemInfo.GetGemCount() != 0)
					{
						return;
					}
					index = 0;
				}
				play.GetItemSystem().DeleteItemByID(srcid);
				roleItemInfo.OpenGem(index);
				play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000A580 File Offset: 0x00008780
		public void GemSet(PlayerObject play, uint srcid, uint destid, byte index)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(destid);
			RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(srcid);
			if (roleItemInfo != null && roleItemInfo2 != null)
			{
				if (roleItemInfo2.IsGem())
				{
					if (roleItemInfo.GetGemCount() >= (int)index)
					{
						if (roleItemInfo.GetGemType(index) == 255)
						{
							play.GetItemSystem().DeleteItemByID(srcid);
							roleItemInfo.SetGemType(index, roleItemInfo2.GetGemType());
							play.GetItemSystem().UpdateItemInfo(destid);
						}
					}
				}
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x0000A61C File Offset: 0x0000881C
		public void GemFusion(PlayerObject play, uint destid)
		{
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(destid);
			ItemTypeInfo itemTypeInfo2 = ConfigManager.Instance().GetItemTypeInfo(destid - 10U);
			if (itemTypeInfo != null && itemTypeInfo2 != null)
			{
				GemInfo gemInfo = ConfigManager.Instance().GetGemInfo(itemTypeInfo.id);
				if (gemInfo != null)
				{
					if (!play.GetItemSystem().DeleteItemByItemID(itemTypeInfo2.id, gemInfo.amount))
					{
						play.MsgBox("Synthesis failed, insufficient quantity");
					}
					else
					{
						play.GetItemSystem().AwardItem(itemTypeInfo.id, 50, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
						play.MsgBox("Gem Synthesis Success!");
					}
				}
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x0000A6D0 File Offset: 0x000088D0
		public bool IsAccordWithEquip(byte level, byte profession, byte equip_pos, RoleItemInfo item_info)
		{
			uint num = 0U;
			int num2 = 0;
			switch (equip_pos)
			{
			case 1:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 113000U;
							}
						}
						else
						{
							num = 111000U;
						}
					}
					else
					{
						num = 115000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 112000U;
						}
					}
					else
					{
						num = 119000U;
					}
				}
				else
				{
					num = 117000U;
				}
				num2 = 14;
				break;
			case 2:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 123000U;
							}
						}
						else
						{
							num = 121000U;
						}
					}
					else
					{
						num = 125000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 122000U;
						}
					}
					else
					{
						num = 129000U;
					}
				}
				else
				{
					num = 127000U;
				}
				num2 = 14;
				break;
			case 3:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 133000U;
							}
						}
						else
						{
							num = 131000U;
						}
					}
					else
					{
						num = 135000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 132000U;
						}
					}
					else
					{
						num = 139000U;
					}
				}
				else
				{
					num = 137000U;
				}
				num2 = 14;
				break;
			case 4:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 490000U;
							}
						}
						else if (item_info.itemid >= 420000U)
						{
							num = 420000U;
						}
						else
						{
							num = 410000U;
						}
					}
					else
					{
						num = 440000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 480000U;
						}
					}
					else
					{
						num = 430000U;
					}
				}
				else
				{
					num = 450000U;
				}
				num2 = 27;
				break;
			case 7:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 143000U;
							}
						}
						else
						{
							num = 141000U;
						}
					}
					else
					{
						num = 145000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 142000U;
						}
					}
					else
					{
						num = 149010U;
					}
				}
				else
				{
					num = 147000U;
				}
				num2 = 14;
				break;
			case 8:
				if (profession <= 30)
				{
					if (profession != 10)
					{
						if (profession != 20)
						{
							if (profession == 30)
							{
								num = 163000U;
							}
						}
						else
						{
							num = 161000U;
						}
					}
					else
					{
						num = 165000U;
					}
				}
				else if (profession != 50)
				{
					if (profession != 60)
					{
						if (profession == 70)
						{
							num = 162000U;
						}
					}
					else
					{
						num = 169000U;
					}
				}
				else
				{
					num = 167000U;
				}
				num2 = 10;
				break;
			}
			ItemTypeInfo itemTypeInfo = null;
			int i = 0;
			while (i < num2)
			{
				itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(num);
				if (itemTypeInfo != null && itemTypeInfo.req_level >= level)
				{
					if (itemTypeInfo.req_level == level)
					{
						break;
					}
					num -= 10U;
					itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(num);
					break;
				}
				else
				{
					num += 10U;
					i++;
				}
			}
			bool result;
			if (itemTypeInfo == null)
			{
				result = false;
			}
			else
			{
				ItemTypeInfo itemTypeInfo2 = ConfigManager.Instance().GetItemTypeInfo(item_info.itemid);
				result = (itemTypeInfo2 != null && string.Compare(itemTypeInfo.name, itemTypeInfo2.name) == 0);
			}
			return result;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000AA94 File Offset: 0x00008C94
		public void GemReplace(PlayerObject play, byte[] data)
		{
			PackIn packIn = new PackIn(data);
			packIn.ReadInt16();
			packIn.ReadUInt32();
			uint id = packIn.ReadUInt32();
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(id);
			if (roleItemInfo == null)
			{
				play.MsgBox("Replacement failed, equipment does not exist.");
			}
			else
			{
				int num = packIn.ReadInt32();
				int num2 = packIn.ReadInt32();
				int num3 = packIn.ReadInt32();
				uint id2 = packIn.ReadUInt32();
				uint id3 = packIn.ReadUInt32();
				uint id4 = packIn.ReadUInt32();
				RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(id2);
				RoleItemInfo roleItemInfo3 = play.GetItemSystem().FindItem(id3);
				RoleItemInfo roleItemInfo4 = play.GetItemSystem().FindItem(id4);
				if (roleItemInfo2 != null && roleItemInfo2.IsGem())
				{
					if (roleItemInfo.GetGemCount() > 0)
					{
						roleItemInfo.SetGemType(0, roleItemInfo2.GetGemType());
						play.GetItemSystem().DeleteItemByID(roleItemInfo2.id);
					}
				}
				if (roleItemInfo3 != null && roleItemInfo3.IsGem())
				{
					if (roleItemInfo.GetGemCount() > 1)
					{
						roleItemInfo.SetGemType(1, roleItemInfo3.GetGemType());
						play.GetItemSystem().DeleteItemByID(roleItemInfo3.id);
					}
				}
				if (roleItemInfo4 != null && roleItemInfo4.IsGem())
				{
					if (roleItemInfo.GetGemCount() > 2)
					{
						roleItemInfo.SetGemType(2, roleItemInfo4.GetGemType());
						play.GetItemSystem().DeleteItemByID(roleItemInfo4.id);
					}
				}
				play.GetItemSystem().SendItemInfo(roleItemInfo, 1);
				play.MsgBox("Gem Replacement Successful");
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000AC50 File Offset: 0x00008E50
		public void Magic_Add_God(PlayerObject play, uint srcid, uint destid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(srcid);
			if (roleItemInfo != null)
			{
				RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(destid);
				if (roleItemInfo2 != null)
				{
					if (roleItemInfo.stronglv >= 12)
					{
						play.MsgBox("Reached the highest level of divine artifacts");
					}
					else
					{
						int num = roleItemInfo.god_exp / 10000;
						int num2 = 0;
						switch (roleItemInfo2.itemid)
						{
						case 1037231U:
							num2 = 20;
							break;
						case 1037232U:
							num2 = 60;
							break;
						case 1037233U:
							num2 = 180;
							break;
						}
						if (roleItemInfo.itemid == 1110110U && roleItemInfo.god_strong >= 22500)
						{
							roleItemInfo.god_strong = 0;
							RoleItemInfo roleItemInfo3 = roleItemInfo;
							roleItemInfo3.stronglv += 1;
						}
						if (num2 != 0 || roleItemInfo.itemid == roleItemInfo2.itemid)
						{
							if (num2 == 0)
							{
							}
							num2 = 1000;
							if (roleItemInfo.itemid != 1110010U || roleItemInfo.god_strong < 7500)
							{
								if (roleItemInfo.itemid != 1110110U || roleItemInfo.god_strong < 22500)
								{
									if (roleItemInfo.itemid != 1110210U || roleItemInfo.god_strong < 30000)
									{
										roleItemInfo.god_strong += num2;
										if (roleItemInfo.itemid == 1110010U && roleItemInfo.god_strong >= 7500)
										{
											roleItemInfo.god_strong = 0;
											RoleItemInfo roleItemInfo4 = roleItemInfo;
											roleItemInfo4.stronglv += 1;
										}
										if (roleItemInfo.itemid == 1110110U && roleItemInfo.god_strong >= 22500)
										{
											roleItemInfo.god_strong = 0;
											RoleItemInfo roleItemInfo5 = roleItemInfo;
											roleItemInfo5.stronglv += 1;
										}
										if (roleItemInfo.itemid == 1110210U && roleItemInfo.god_strong >= 30000)
										{
											roleItemInfo.god_strong = 0;
											RoleItemInfo roleItemInfo6 = roleItemInfo;
											roleItemInfo6.stronglv += 1;
										}
										play.GetItemSystem().DeleteItemByID(roleItemInfo2.id);
										play.GetItemSystem().SendItemInfo(roleItemInfo, 1);
										MsgEquipOperationRet msgEquipOperationRet = new MsgEquipOperationRet();
										msgEquipOperationRet.srcid = srcid;
										msgEquipOperationRet.destid = destid;
										byte[] array = new byte[4];
										array[0] = 9;
										array[2] = 3;
										byte[] value = array;
										msgEquipOperationRet.type = BitConverter.ToUInt32(value, 0);
										msgEquipOperationRet.ret = 1U;
										play.SendData(msgEquipOperationRet.GetBuffer(), true);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000AF2C File Offset: 0x0000912C
		public void Equip_GodExp(PlayerObject play, uint srcid, uint destid)
		{
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(srcid);
			if (roleItemInfo != null)
			{
				RoleItemInfo roleItemInfo2 = play.GetItemSystem().FindItem(destid);
				if (roleItemInfo2 != null)
				{
					if (roleItemInfo.god_exp >= 90000)
					{
						play.MsgBox("Reached the highest divine protection level");
					}
					else
					{
						int num = roleItemInfo.god_exp / 10000;
						int num2;
						switch (num)
						{
						case 0:
							num2 = 1000;
							break;
						case 1:
							num2 = 500;
							break;
						case 2:
							num2 = 200;
							break;
						case 3:
							num2 = 125;
							break;
						case 4:
							num2 = 83;
							break;
						case 5:
							num2 = 55;
							break;
						case 6:
							num2 = 40;
							break;
						case 7:
							num2 = 28;
							break;
						case 8:
							num2 = 20;
							break;
						default:
							return;
						}
						int num3 = 1;
						uint itemid = roleItemInfo2.itemid;
						if (itemid != 1037210U)
						{
							switch (itemid)
							{
							case 1037260U:
								if (num < 3)
								{
									return;
								}
								num3 = 5;
								break;
							case 1037261U:
								if (num < 5)
								{
									return;
								}
								num3 = 10;
								break;
							case 1037262U:
								if (num < 8)
								{
									return;
								}
								num3 = 25;
								break;
							}
						}
						else
						{
							num3 = 1;
						}
						roleItemInfo.god_exp += num2 * num3;
						play.GetItemSystem().DeleteItemByID(roleItemInfo2.id);
						play.GetItemSystem().SendItemInfo(roleItemInfo, 1);
						MsgEquipOperationRet msgEquipOperationRet = new MsgEquipOperationRet();
						msgEquipOperationRet.srcid = srcid;
						msgEquipOperationRet.destid = destid;
						byte[] array = new byte[4];
						array[0] = 7;
						array[2] = 3;
						byte[] value = array;
						msgEquipOperationRet.type = BitConverter.ToUInt32(value, 0);
						msgEquipOperationRet.ret = 1U;
						play.SendData(msgEquipOperationRet.GetBuffer(), true);
					}
				}
			}
		}

		// Token: 0x04000077 RID: 119
		private const byte MAX_STRONGLEVEL = 12;

		// Token: 0x04000078 RID: 120
		private static EquipOperation mInstance = null;

		// Token: 0x04000079 RID: 121
		private List<EquipStrongInfo> mListStrong;
	}
}
