using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000090 RID: 144
	public class PlayerItem
	{
		public const int InventoryCapacity = 40;

		public const ushort WardrobeMountPosition =
			MsgStrongPack.MOUNT_PACKAGE_TYPE;

		public const ushort WardrobeMountCapacity = 45;

		public const uint RoseFlowerCartBridleType = 1500000U;

		public const uint RoseFlyCarSaddleType = 1500001U;

		public const uint WardrobeMountServerType = 1073210U;

		public const ushort WardrobeWeaponSoulPosition =
			MsgItemInfo.ITEMPOSITION_CHEST_SOUL;

		private const uint WarriorBladeSoulFamily = 415U;

		private const uint WarriorSwordSoulFamily = 425U;

		private const uint MageStaffSoulFamily = 435U;

		private const uint MageScepterSoulFamily = 445U;

		private const uint VampireTalonsSoulFamily = 455U;

		private const uint NecromancerSoulFamily = 485U;

		private const uint PaladinWandSoulFamily = 495U;

		public static bool FitsCapacity(
			int currentCount,
			int pendingCount,
			int outgoingCount,
			int incomingCount,
			int capacity)
		{
			if (currentCount < 0 || pendingCount < 0 ||
				outgoingCount < 0 || incomingCount < 0 || capacity < 0)
			{
				return false;
			}

			long finalCount = (long)currentCount + pendingCount -
				outgoingCount + incomingCount;
			return finalCount >= 0 && finalCount <= capacity;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00021B50 File Offset: 0x0001FD50
		public uint GetScriptItemId()
		{
			return this.mScriptItemId;
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00021B68 File Offset: 0x0001FD68
		public Dictionary<uint, RoleItemInfo> GetDicItem()
		{
			return this.mDicItem;
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00021B80 File Offset: 0x0001FD80
		public int GetBagCount()
		{
			int num = 0;
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == 50)
				{
					num++;
				}
			}
			return num;
		}

		public int GetPendingPositionCount(ushort position)
		{
			int count = 0;
			foreach (RoleItemInfo item in this.mDicAddItem.Values)
			{
				if (item.postion == position)
				{
					count++;
				}
			}
			return count;
		}

		public int GetPositionCount(ushort position)
		{
			int count = 0;
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == position)
				{
					count++;
				}
			}
			return count;
		}

		public bool CanAcceptAtPosition(
			ushort position,
			int incomingCount = 1,
			int outgoingCount = 0)
		{
			if (position == MsgItemInfo.ITEMPOSITION_BACKPACK)
			{
				return FitsCapacity(
					this.GetBagCount(),
					this.GetPendingPositionCount(position),
					outgoingCount,
					incomingCount,
					InventoryCapacity);
			}
			if (position == MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
			{
				return FitsCapacity(
					this.GetEudemonCount(),
					this.GetPendingPositionCount(position),
					outgoingCount,
					incomingCount,
					this.play.GetEudemonCapacity());
			}
			return true;
		}

		public bool CanAwardItem(
			uint itemId,
			ushort position,
			bool useStackLimit = true)
		{
			if (this.IsGold(itemId))
			{
				return true;
			}

			ItemTypeInfo itemType =
				ConfigManager.Instance().GetItemTypeInfo(itemId);
			if (itemType == null)
			{
				return GameServer.IsTestMode();
			}
			if (!this.IsEquip(itemType.id) && useStackLimit &&
				itemType.amount_limit > 1)
			{
				foreach (RoleItemInfo item in this.mDicItem.Values)
				{
					if (item.itemid == itemId &&
						item.amount + itemType.amount <=
						itemType.amount_limit)
					{
						return true;
					}
				}
			}
			return this.CanAcceptAtPosition(position);
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x00021BFC File Offset: 0x0001FDFC
		public uint GetWeaponLook()
		{
			return this.mWeaponId;
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x00021C14 File Offset: 0x0001FE14
		public uint GetArmorLook()
		{
			uint result;
			if (this.mFashionId != 0U)
			{
				result = this.mFashionId;
			}
			else
			{
				result = this.mArmorId;
			}
			return result;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x00021C44 File Offset: 0x0001FE44
		public PlayerItem(PlayerObject _play)
		{
			this.mDicItem = new Dictionary<uint, RoleItemInfo>();
			this.mDicItem.Clear();
			this.play = _play;
			this.mDicAddItem = new Dictionary<uint, RoleItemInfo>();
			this.mDicAddItem.Clear();
			this.mWardrobeMountPendingPrices =
				new Dictionary<RoleItemInfo, int>();
			this.mWardrobeWeaponSoulPendingPrices =
				new Dictionary<RoleItemInfo, int>();
			this.mPendingDroppedItemPickups =
				new Dictionary<RoleItemInfo, DropItemObject>();
			this.mScriptItemId = 0U;
			this.mWeaponId = 0U;
			this.mFashionId = 0U;
			this.mArmorId = 0U;
		}

		private uint GetNextAddItemSortId()
		{
			do
			{
				this.mNextAddItemSortId++;
				if (this.mNextAddItemSortId == 0U)
				{
					this.mNextAddItemSortId = 1U;
				}
			}
			while (this.mDicAddItem.ContainsKey(this.mNextAddItemSortId));
			return this.mNextAddItemSortId;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00021CB4 File Offset: 0x0001FEB4
		public RoleItemInfo AwardItem(
			RoleItemInfo info,
			bool enforceCapacity = true,
			DropItemObject pickupSource = null)
		{
			RoleItemInfo result;
			if (this.IsGold(info.itemid))
			{
				this.play.ChangeAttribute(UserAttribute.GOLD, info.property, true);
				this.play.LeftNotice(string.Format("Obtained {0} gold coins!", info.property));
				result = null;
			}
			else
			{
				if (enforceCapacity &&
					!this.CanAcceptAtPosition(info.postion))
				{
					this.NotifyPackageFull(info.postion);
					return null;
				}

				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(info.itemid);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog("Failed to create item; item does not exist. ID: " + info.itemid.ToString());
					if (!GameServer.IsTestMode())
					{
						return null;
					}
					itemTypeInfo = new ItemTypeInfo();
					itemTypeInfo.id = info.itemid;
				}
				uint num = this.GetNextAddItemSortId();
				RoleItemInfo roleItemInfo = new RoleItemInfo();
				roleItemInfo.itemid = itemTypeInfo.id;
				roleItemInfo.postion = info.postion;
				roleItemInfo.stronglv = info.stronglv;
				roleItemInfo.gemcount = info.gemcount;
				roleItemInfo.amount = info.amount;
				roleItemInfo.gem1 = info.gem1;
				roleItemInfo.gem2 = info.gem2;
				roleItemInfo.forgename = info.forgename;
				roleItemInfo.war_ghost_exp = info.war_ghost_exp;
				roleItemInfo.di_attack = info.di_attack;
				roleItemInfo.huo_attack = info.huo_attack;
				roleItemInfo.shui_attack = info.shui_attack;
				roleItemInfo.feng_attack = info.feng_attack;
				roleItemInfo.property = info.property;
				roleItemInfo.gem3 = info.gem3;
				roleItemInfo.god_strong = info.god_strong;
				roleItemInfo.god_exp = info.god_exp;
				roleItemInfo.typeid = info.typeid;
				this.mDicAddItem[num] = roleItemInfo;
				if (pickupSource != null)
				{
					this.mPendingDroppedItemPickups[roleItemInfo] =
						pickupSource;
					if (roleItemInfo.postion ==
						MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
					{
						RoleData_Eudemon eudemon =
							pickupSource.GetRoleEudemonInfo();
						if (eudemon == null)
						{
							this.mPendingDroppedItemPickups.Remove(
								roleItemInfo);
							this.mDicAddItem.Remove(num);
							return null;
						}
						this.play.GetEudemonSystem().AddTempEudemon(
							eudemon);
					}
				}
				AddRoleData_Item addRoleData_Item = new AddRoleData_Item();
				addRoleData_Item.item.playerid = this.play.GetBaseAttr().player_id;
				addRoleData_Item.gameid = this.play.GetGameID();
				addRoleData_Item.item.postion = roleItemInfo.postion;
				addRoleData_Item.item.itemid = roleItemInfo.itemid;
				addRoleData_Item.item.stronglv = roleItemInfo.stronglv;
				addRoleData_Item.item.amount = roleItemInfo.amount;
				addRoleData_Item.item.gem1 = roleItemInfo.gem1;
				addRoleData_Item.item.gem2 = roleItemInfo.gem2;
				addRoleData_Item.item.forgename = roleItemInfo.forgename;
				addRoleData_Item.item.war_ghost_exp = roleItemInfo.war_ghost_exp;
				addRoleData_Item.item.di_attack = roleItemInfo.di_attack;
				addRoleData_Item.item.huo_attack = roleItemInfo.huo_attack;
				addRoleData_Item.item.shui_attack = roleItemInfo.shui_attack;
				addRoleData_Item.item.feng_attack = roleItemInfo.feng_attack;
				addRoleData_Item.item.property = roleItemInfo.property;
				addRoleData_Item.item.gem3 = roleItemInfo.gem3;
				addRoleData_Item.item.god_strong = roleItemInfo.god_strong;
				addRoleData_Item.item.god_exp = roleItemInfo.god_exp;
				addRoleData_Item.sortid = num;
				DBServer.Instance().GetDBClient().SendData(addRoleData_Item.GetBuffer());
				result = roleItemInfo;
			}
			return result;
		}

		public bool AwardDroppedItem(DropItemObject pickupSource)
		{
			if (pickupSource == null)
			{
				return false;
			}
			RoleItemInfo info = pickupSource.GetRoleItemInfo();
			if (info == null || this.IsGold(info.itemid))
			{
				return false;
			}
			if (!CanDropFromPosition(info.postion) ||
				!this.CanAcceptAtPosition(info.postion))
			{
				this.NotifyPackageFull(info.postion);
				return false;
			}
			if (info.postion == MsgItemInfo.ITEMPOSITION_EUDEMON_PACK &&
				pickupSource.GetRoleEudemonInfo() == null)
			{
				Log.Instance().WriteLog(
					"Rejected a dropped Eudemon without its persisted state. Item ID: " +
					info.id.ToString());
				return false;
			}
			return this.AwardItem(info, false, pickupSource) != null;
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x00021FDC File Offset: 0x000201DC
		public RoleItemInfo ItemLimit(uint itemid, byte amount)
		{
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(itemid);
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.itemid == itemid)
				{
					if (roleItemInfo.amount + (ushort)amount <= itemTypeInfo.amount_limit)
					{
						RoleItemInfo roleItemInfo2 = roleItemInfo;
						roleItemInfo2.amount += (ushort)amount;
						this.UpdateItemInfo(roleItemInfo.id);
						return roleItemInfo;
					}
				}
			}
			return null;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00022094 File Offset: 0x00020294
		public bool IsGold(uint itemid)
		{
			return itemid == 1090000U || itemid == 1090010U || itemid == 1090020U || itemid == 1090030U || itemid == 1090040U;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x000220E0 File Offset: 0x000202E0
		public RoleItemInfo AwardItem(uint itemid, byte postion, byte amount = 1, byte stronglv = 0, byte gem1 = 0, byte gem2 = 0, byte gem3 = 0, byte warghost_exp = 0, byte di_attack = 0, byte shui_attack = 0, byte huo_attack = 0, byte feng_attack = 0, bool limit = true, int pendingWardrobeMountPrice = 0, int pendingWardrobeWeaponSoulPrice = 0)
		{
			RoleItemInfo result;
			if (this.IsGold(itemid))
			{
				result = null;
			}
			else
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(itemid);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog("Failed to create item; item does not exist. ID: " + itemid.ToString());
					if (!GameServer.IsTestMode())
					{
						return null;
					}
					itemTypeInfo = new ItemTypeInfo();
					itemTypeInfo.id = itemid;
				}
				RoleItemInfo roleItemInfo;
				if (!this.IsEquip(itemTypeInfo.id) && limit && itemTypeInfo.amount_limit > 1)
				{
					roleItemInfo = this.ItemLimit(itemid, (byte)itemTypeInfo.amount);
					if (roleItemInfo != null)
					{
						return roleItemInfo;
					}
				}
				if (!this.CanAcceptAtPosition(postion))
				{
					this.NotifyPackageFull(postion);
					return null;
				}
				uint num = this.GetNextAddItemSortId();
				roleItemInfo = new RoleItemInfo();
				roleItemInfo.itemid = itemTypeInfo.id;
				roleItemInfo.postion = (ushort)postion;
				roleItemInfo.stronglv = stronglv;
				roleItemInfo.gem1 = (uint)gem1;
				roleItemInfo.gem2 = (uint)gem2;
				roleItemInfo.gem3 = (uint)gem3;
				roleItemInfo.war_ghost_exp = (int)(warghost_exp * 100);
				roleItemInfo.di_attack = di_attack;
				roleItemInfo.shui_attack = shui_attack;
				roleItemInfo.huo_attack = huo_attack;
				roleItemInfo.feng_attack = feng_attack;
				roleItemInfo.amount = itemTypeInfo.amount;
				GemInfo gemInfo = ConfigManager.Instance().GetGemInfo(roleItemInfo.itemid);
				if (gemInfo != null)
				{
					roleItemInfo.gem1 = (uint)gemInfo.type;
				}
				this.mDicAddItem[num] = roleItemInfo;
				if (roleItemInfo.postion == WardrobeMountPosition &&
					pendingWardrobeMountPrice > 0)
				{
					this.mWardrobeMountPendingPrices[roleItemInfo] =
						pendingWardrobeMountPrice;
				}
				if (roleItemInfo.postion == WardrobeWeaponSoulPosition &&
					pendingWardrobeWeaponSoulPrice > 0)
				{
					this.mWardrobeWeaponSoulPendingPrices[roleItemInfo] =
						pendingWardrobeWeaponSoulPrice;
				}
				AddRoleData_Item addRoleData_Item = new AddRoleData_Item();
				addRoleData_Item.item.playerid = this.play.GetBaseAttr().player_id;
				addRoleData_Item.gameid = this.play.GetGameID();
				addRoleData_Item.item.postion = (ushort)postion;
				addRoleData_Item.item.itemid = roleItemInfo.itemid;
				addRoleData_Item.item.stronglv = stronglv;
				addRoleData_Item.item.amount = (ushort)amount;
				addRoleData_Item.sortid = num;
				DBServer.Instance().GetDBClient().SendData(addRoleData_Item.GetBuffer());
				result = roleItemInfo;
			}
			return result;
		}

		// Token: 0x060002FC RID: 764 RVA: 0x000222F8 File Offset: 0x000204F8
		public void AwardItem_Ret(uint sortid, uint id)
		{
			if (this.mDicAddItem.ContainsKey(sortid))
			{
				RoleItemInfo roleItemInfo = this.mDicAddItem[sortid];
				DropItemObject pendingDroppedPickup;
				bool isPendingDroppedPickup =
					this.mPendingDroppedItemPickups.TryGetValue(
						roleItemInfo,
						out pendingDroppedPickup);
				int pendingMountPrice;
				bool isPendingMountPurchase =
					this.mWardrobeMountPendingPrices.TryGetValue(
						roleItemInfo,
						out pendingMountPrice);
				int pendingWeaponSoulPrice;
				bool isPendingWeaponSoulPurchase =
					this.mWardrobeWeaponSoulPendingPrices.TryGetValue(
						roleItemInfo,
						out pendingWeaponSoulPrice);
				this.mDicAddItem.Remove(sortid);
				if (id == 0U)
				{
					if (isPendingDroppedPickup)
					{
						this.mPendingDroppedItemPickups.Remove(roleItemInfo);
						if (roleItemInfo.postion ==
							MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
						{
							this.play.GetEudemonSystem().DeleteTempEudemon(
								roleItemInfo.typeid);
						}
						pendingDroppedPickup.CancelPickup(
							this.play.GetGameID());
						this.play.LeftNotice(
							"The dropped item could not be restored. It remains on the ground.");
					}
					if (isPendingMountPurchase)
					{
						this.mWardrobeMountPendingPrices.Remove(roleItemInfo);
						this.play.ChangeMoney(
							MONEYTYPE.GAMEGOLD,
							pendingMountPrice);
						this.play.LeftNotice(
							"The mount purchase could not be saved. Your EP was refunded.");
					}
					if (isPendingWeaponSoulPurchase)
					{
						this.mWardrobeWeaponSoulPendingPrices.Remove(roleItemInfo);
						this.play.ChangeMoney(
							MONEYTYPE.GAMEGOLD,
							pendingWeaponSoulPrice);
						this.play.LeftNotice(
							"The weapon soul purchase could not be saved. Your EP was refunded.");
					}
					Log.Instance().WriteLog(
						"Rejected item creation result with ID 0. Role ID: " +
						this.play.GetBaseAttr().player_id.ToString() +
						" Item type: " + roleItemInfo.itemid.ToString());
					return;
				}
				if (isPendingMountPurchase)
				{
					this.mWardrobeMountPendingPrices.Remove(roleItemInfo);
				}
				if (isPendingWeaponSoulPurchase)
				{
					this.mWardrobeWeaponSoulPendingPrices.Remove(roleItemInfo);
				}
				roleItemInfo.id = id;
				uint num = 0U;
				if (roleItemInfo.postion == 53 && roleItemInfo.typeid > 0U)
				{
					num = roleItemInfo.typeid;
				}
				this.mDicItem[roleItemInfo.id] = roleItemInfo;
				if (roleItemInfo.postion != 100)
				{
					if (roleItemInfo.postion == WardrobeMountPosition)
					{
						this.SendWardrobeMountPackage();
					}
					else if (roleItemInfo.postion == 53)
					{
						ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
						if (itemTypeInfo != null)
						{
							roleItemInfo.forgename = itemTypeInfo.name;
						}
						roleItemInfo.typeid = IDManager.CreateTypeId(4);
					}
					else if (roleItemInfo.postion != 50)
					{
						this.CalcEquipLook(roleItemInfo);
					}
					if (roleItemInfo.postion != WardrobeMountPosition)
					{
						this.SendItemInfo(roleItemInfo, 1);
					}
					if (roleItemInfo.postion == 53)
					{
						RoleData_Eudemon roleData_Eudemon = this.play.GetEudemonSystem().FindTempEudemon(num);
						if (roleData_Eudemon != null && roleData_Eudemon.itemid != 0U)
						{
							roleData_Eudemon.itemid = roleItemInfo.id;
							roleData_Eudemon.typeid = roleItemInfo.typeid;
							this.play.GetEudemonSystem().AddEudemon(roleData_Eudemon);
							this.play.GetEudemonSystem().DeleteTempEudemon(num);
							IDManager.RecoveryTypeID(num, 4);
							if (isPendingDroppedPickup)
							{
								this.play.GetEudemonSystem().DB_Save(
									roleData_Eudemon);
							}
						}
						else if (roleData_Eudemon != null && roleData_Eudemon.itemid == 0U)
						{
							this.play.GetEudemonSystem().AddEudemon(roleItemInfo, (byte)roleData_Eudemon.level, roleData_Eudemon.quality, (byte)roleData_Eudemon.wuxing);
						}
						else
						{
							this.play.GetEudemonSystem().AddEudemon(roleItemInfo, 1, 0, 0);
						}
					}
				}
				if (isPendingDroppedPickup)
				{
					this.mPendingDroppedItemPickups.Remove(roleItemInfo);
					pendingDroppedPickup.CompletePickup(
						this.play.GetGameID());
				}
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x000224EC File Offset: 0x000206EC
		private void CalcEquipLook(RoleItemInfo _item = null)
		{
			bool flag = false;
			bool flag2 = false;
			this.mFashionId = (this.mArmorId = (this.mWeaponId = 0U));
			if (_item != null)
			{
				if (_item.postion == 12 || _item.postion == 4 || _item.postion == 3)
				{
					flag = true;
				}
			}
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion != 50)
				{
					if (this.IsEquip(roleItemInfo.itemid))
					{
						flag2 = true;
						ushort postion = roleItemInfo.postion;
						switch (postion)
						{
						case 3:
							if (roleItemInfo.itemid != this.mArmorId)
							{
								flag = true;
							}
							this.mArmorId = roleItemInfo.itemid;
							break;
						case 4:
							if (roleItemInfo.itemid != this.mWeaponId)
							{
								flag = true;
							}
							this.mWeaponId = roleItemInfo.itemid;
							break;
						default:
							if (postion == 12)
							{
								if (roleItemInfo.itemid != this.mFashionId)
								{
									flag = true;
								}
								this.mFashionId = roleItemInfo.itemid;
							}
							break;
						}
						if (roleItemInfo.postion == 26)
						{
							this.mWeaponId = roleItemInfo.itemid;
						}
					}
				}
			}
			if (flag2)
			{
				this.play.CalcAttribute();
			}
			if (flag && this.play.GetGameMap() != null)
			{
				foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (obj.type == 2)
					{
						(obj as PlayerObject).SendRoleInfo(this.play);
					}
				}
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x00022758 File Offset: 0x00020958
		public void DeleteScripteItem()
		{
			if (this.mScriptItemId != 0U)
			{
				if (this.mDicItem.ContainsKey(this.mScriptItemId))
				{
					this.DeleteItemByID(this.mScriptItemId);
				}
				this.mScriptItemId = 0U;
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x000227A8 File Offset: 0x000209A8
		public void SendItemInfo(RoleItemInfo info, byte tag = 1)
		{
			MsgItemInfo msgItemInfo = new MsgItemInfo();
			msgItemInfo.Create(null, this.play.GetGamePackKeyEx());
			msgItemInfo.postion = (byte)info.postion;
			if (msgItemInfo.postion == 53)
			{
				msgItemInfo.id = info.typeid;
				if (tag == 1)
				{
					msgItemInfo.tag = 3;
				}
			}
			else
			{
				msgItemInfo.id = info.id;
			}
			msgItemInfo.item_id = info.itemid;
			msgItemInfo.amount = info.amount;
			msgItemInfo.amount_limit = info.amount;
			msgItemInfo.magic3 = info.stronglv;
			msgItemInfo.gem = (byte)info.gem1;
			msgItemInfo.gem2 = (byte)info.gem2;
			msgItemInfo.warghost_exp = info.war_ghost_exp;
			msgItemInfo.di_attack = info.di_attack;
			msgItemInfo.shui_attack = info.shui_attack;
			msgItemInfo.huo_attack = info.huo_attack;
			msgItemInfo.feng_attack = info.feng_attack;
			msgItemInfo.properties = info.property;
			msgItemInfo.gem3 = (byte)info.gem3;
			msgItemInfo.god_exp = info.god_exp;
			msgItemInfo.god_strong = info.god_strong;
			msgItemInfo.tag = tag;
			msgItemInfo.name = info.forgename;
			if (msgItemInfo.item_id == 500001U || msgItemInfo.item_id == 830000U)
			{
				msgItemInfo.param3 = 1000000;
			}
			if (msgItemInfo.item_id == 1110010U || msgItemInfo.item_id == 1110110U || msgItemInfo.item_id == 1110210U)
			{
				msgItemInfo.param3 = info.god_strong;
			}
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(msgItemInfo.item_id);
			if (itemTypeInfo != null)
			{
				msgItemInfo.amount_limit = itemTypeInfo.amount_limit;
			}
			this.play.SendData(msgItemInfo.GetBuffer(), false);
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00022998 File Offset: 0x00020B98
		public void UpdateItemInfo(uint id)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				this.SendItemInfo(roleItemInfo, 1);
				if (roleItemInfo.postion >= 1 && roleItemInfo.postion < 12)
				{
					this.play.CalcAttribute();
				}
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x00022A00 File Offset: 0x00020C00
		public void SendAllItemInfo()
		{
			foreach (RoleItemInfo info in this.mDicItem.Values)
			{
				if (info.postion != WardrobeMountPosition &&
					info.postion != MsgItemInfo.ITEMPOSITION_BATCH_HATCHER &&
					EudemonHatchManager.ShouldSendAsRegularItem(info))
				{
					this.SendItemInfo(info, 1);
				}
			}
			this.SendWardrobeMountPackage();
		}

		// Token: 0x06000302 RID: 770 RVA: 0x00022A64 File Offset: 0x00020C64
		public void AddItemInfo(RoleData_Item item)
		{
			RoleItemInfo roleItemInfo = new RoleItemInfo();
			roleItemInfo.id = item.id;
			roleItemInfo.itemid = item.itemid;
			roleItemInfo.postion = item.postion;
			roleItemInfo.stronglv = item.stronglv;
			roleItemInfo.gem1 = item.gem1;
			roleItemInfo.gem2 = item.gem2;
			roleItemInfo.forgename = item.forgename;
			roleItemInfo.amount = item.amount;
			roleItemInfo.war_ghost_exp = item.war_ghost_exp;
			roleItemInfo.di_attack = item.di_attack;
			roleItemInfo.shui_attack = item.shui_attack;
			roleItemInfo.huo_attack = item.huo_attack;
			roleItemInfo.feng_attack = item.feng_attack;
			roleItemInfo.property = item.property;
			roleItemInfo.gem3 = item.gem3;
			roleItemInfo.god_exp = item.god_exp;
			roleItemInfo.god_strong = item.god_strong;
			this.mDicItem[roleItemInfo.id] = roleItemInfo;
			if (roleItemInfo.postion == 53 ||
				(roleItemInfo.postion ==
					MsgItemInfo.ITEMPOSITION_BATCH_HATCHER &&
				 roleItemInfo.itemid /
					EudemonHatchManager.ItemTypeFamilyDivisor ==
					EudemonHatchManager.EudemonItemTypeFamily))
			{
				roleItemInfo.typeid = IDManager.CreateTypeId(4);
			}
			this.CalcEquipLook(null);
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00022B80 File Offset: 0x00020D80
		public bool DeleteItemByID(uint id)
		{
			uint num = id;
			if (id >= IDManager.eudemon_start_id)
			{
				num = this.GetEudemonItemId(id);
			}
			bool result;
			if (this.mDicItem.ContainsKey(num))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[num];
				this.mDicItem.Remove(num);
				if (roleItemInfo.postion == 53)
				{
					this.play.GetEudemonSystem().DeleteEudemon(id);
				}
				else
				{
					this.ClearItem(id);
				}
				if (roleItemInfo.postion >= 3 && roleItemInfo.postion <= 15)
				{
					if (roleItemInfo.postion == 3 || roleItemInfo.postion == 4)
					{
						this.CalcEquipLook(roleItemInfo);
					}
					this.play.CalcAttribute();
				}
				DeleteItemByID deleteItemByID = new DeleteItemByID();
				deleteItemByID.id = num;
				deleteItemByID.playerid = this.play.GetBaseAttr().player_id;
				deleteItemByID.postion = roleItemInfo.postion;
				DBServer.Instance().GetDBClient().SendData(deleteItemByID.GetBuffer());
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00022CB0 File Offset: 0x00020EB0
		public int GetItemCount(uint itemid)
		{
			int num = 0;
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.itemid == itemid)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00022D2C File Offset: 0x00020F2C
		public bool DeleteItemByItemID(uint itemid, int count = 1)
		{
			bool result;
			if (this.GetItemCount(itemid) < count)
			{
				result = false;
			}
			else
			{
				List<uint> list = null;
				int num = 0;
				foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
				{
					if (roleItemInfo.itemid == itemid)
					{
						if (list == null)
						{
							list = new List<uint>();
						}
						list.Add(roleItemInfo.id);
						num++;
						if (num == count)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						this.DeleteItemByID(list[i]);
					}
				}
				result = true;
			}
			return result;
		}

		public int GetBackpackItemAmount(uint itemTypeId)
		{
			int total = 0;
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == MsgItemInfo.ITEMPOSITION_BACKPACK &&
					item.itemid == itemTypeId)
				{
					total = checked(total + (item.amount == 0 ? 1 : item.amount));
				}
			}
			return total;
		}

		public bool ConsumeBackpackItemAmount(uint itemTypeId, int count)
		{
			if (count < 0 || this.GetBackpackItemAmount(itemTypeId) < count)
			{
				return false;
			}
			if (count == 0)
			{
				return true;
			}

			List<RoleItemInfo> matchingItems = new List<RoleItemInfo>();
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == MsgItemInfo.ITEMPOSITION_BACKPACK &&
					item.itemid == itemTypeId)
				{
					matchingItems.Add(item);
				}
			}
			matchingItems.Sort(delegate(RoleItemInfo left, RoleItemInfo right)
			{
				return left.id.CompareTo(right.id);
			});

			int remaining = count;
			for (int index = 0;
				index < matchingItems.Count && remaining > 0;
				index++)
			{
				RoleItemInfo item = matchingItems[index];
				int amount = item.amount == 0 ? 1 : item.amount;
				if (amount <= remaining)
				{
					remaining -= amount;
					if (!this.DeleteItemByID(item.id))
					{
						return false;
					}
				}
				else
				{
					item.amount = checked((ushort)(amount - remaining));
					remaining = 0;
					this.SendItemInfo(item, 1);
				}
			}
			if (remaining != 0)
			{
				return false;
			}
			this.DB_Save();
			return true;
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00022E30 File Offset: 0x00021030
		public void DB_Save()
		{
			if (this.mDicItem.Count > 0)
			{
				ROLEDATA_ITEM roledata_ITEM = new ROLEDATA_ITEM();
				roledata_ITEM.SetSaveTag();
				roledata_ITEM.playerid = this.play.GetBaseAttr().player_id;
				foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
				{
					RoleData_Item roleData_Item = new RoleData_Item();
					roleData_Item.id = roleItemInfo.id;
					roleData_Item.itemid = roleItemInfo.itemid;
					roleData_Item.postion = roleItemInfo.postion;
					roleData_Item.stronglv = roleItemInfo.stronglv;
					roleData_Item.gem1 = roleItemInfo.gem1;
					roleData_Item.gem2 = roleItemInfo.gem2;
					roleData_Item.forgename = roleItemInfo.forgename;
					roleData_Item.amount = roleItemInfo.amount;
					roleData_Item.war_ghost_exp = roleItemInfo.war_ghost_exp;
					roleData_Item.di_attack = roleItemInfo.di_attack;
					roleData_Item.shui_attack = roleItemInfo.shui_attack;
					roleData_Item.huo_attack = roleItemInfo.huo_attack;
					roleData_Item.feng_attack = roleItemInfo.feng_attack;
					roleData_Item.property = roleItemInfo.property;
					roleData_Item.gem3 = roleItemInfo.gem3;
					roleData_Item.god_exp = roleItemInfo.god_exp;
					roleData_Item.god_strong = roleItemInfo.god_strong;
					roledata_ITEM.mListItem.Add(roleData_Item);
				}
				DBServer.Instance().GetDBClient().SendData(roledata_ITEM.GetBuffer());
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00022FC0 File Offset: 0x000211C0
		public void Equip(uint id, uint postion)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				RoleItemInfo equipByPostion = this.GetEquipByPostion((byte)postion);
				if (equipByPostion != null)
				{
					bool reservedBackpackSlot =
						roleItemInfo.postion ==
						MsgItemInfo.ITEMPOSITION_BACKPACK &&
						GetUnequipDestination(equipByPostion.postion) ==
						MsgItemInfo.ITEMPOSITION_BACKPACK;
					if (!this.UnEquip(
						equipByPostion.id,
						0U,
						false,
						reservedBackpackSlot))
					{
						return;
					}
				}
				roleItemInfo.postion = (ushort)postion;
				MsgOperateEquip msgOperateEquip = new MsgOperateEquip();
				msgOperateEquip.SetTagEquip();
				msgOperateEquip.Create(null, this.play.GetGamePackKeyEx());
				msgOperateEquip.equipid = roleItemInfo.id;
				msgOperateEquip.postion = (int)postion;
				this.play.SendData(msgOperateEquip.GetBuffer(), false);
				roleItemInfo.postion = (ushort)postion;
				this.CalcEquipLook(roleItemInfo);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00023070 File Offset: 0x00021270
		public static ushort GetUnequipDestination(ushort position)
		{
			if (position == MsgItemInfo.ITEMPOSITION_FASHION)
			{
				return MsgItemInfo.ITEMPOSITION_CHEST;
			}
			if (position == MsgItemInfo.ITEMPOSTION_WEPON_SOUL)
			{
				return MsgItemInfo.ITEMPOSITION_CHEST_SOUL;
			}
			return MsgItemInfo.ITEMPOSITION_BACKPACK;
		}

		public bool UnEquip(
			uint id,
			uint oldpostion,
			bool isChangeLook = true,
			bool reservedBackpackSlot = false)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				ushort destination =
					GetUnequipDestination(roleItemInfo.postion);
				if (destination == MsgItemInfo.ITEMPOSITION_BACKPACK &&
					!reservedBackpackSlot && this.IsItemFull())
				{
					this.NotifyPackageFull(destination);
					return false;
				}

				roleItemInfo.postion = destination;
				MsgOperateEquip msgOperateEquip = new MsgOperateEquip();
				msgOperateEquip.SetTagUnEquip();
				msgOperateEquip.Create(null, this.play.GetGamePackKeyEx());
				msgOperateEquip.equipid = roleItemInfo.id;
				msgOperateEquip.postion = (int)oldpostion;
				this.play.SendData(msgOperateEquip.GetBuffer(), false);
				if (isChangeLook)
				{
					this.CalcEquipLook(roleItemInfo);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0002314C File Offset: 0x0002134C
		public void UseItem(uint id, uint dwdata, short param, short param1)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog("Player used an item that does not exist. ID: " + id.ToString() + " Base ID: " + roleItemInfo.itemid.ToString());
					if (!GameServer.IsTestMode())
					{
						return;
					}
					itemTypeInfo = new ItemTypeInfo();
					itemTypeInfo.id = roleItemInfo.itemid;
				}
				if (this.play.GetBaseAttr().level < itemTypeInfo.req_level)
				{
					this.play.ChatNotice("Insufficient level, cannot use.");
				}
				else if (itemTypeInfo.req_profession != 0 && itemTypeInfo.req_profession != this.play.GetBaseAttr().profession)
				{
					this.play.ChatNotice("Profession does not match, cannot use. ");
				}
				else if (EudemonHatchManager.TryMoveEggToEggBagFromInventoryUse(
					this.play, roleItemInfo))
				{
					return;
				}
				else if (this.IsEquip(itemTypeInfo.id))
				{
					if ((uint)this.GetEquipPostion(itemTypeInfo) == dwdata)
					{
						this.Equip(id, dwdata);
					}
				}
				else
				{
					this.play.SetUseItemEudemonId(0U);
					if (itemTypeInfo.id == 831002U)
					{
						uint useItemEudemonId = (uint)BaseFunc.MakeLong((int)param, (int)param1);
						this.play.SetUseItemEudemonId(useItemEudemonId);
					}
					if (itemTypeInfo.actionid > 0U)
					{
						this.mScriptItemId = id;
						ScripteManager.Instance().ExecuteAction(itemTypeInfo.actionid, this.play);
					}
				}
			}
		}

		// Token: 0x0600030A RID: 778 RVA: 0x000232F4 File Offset: 0x000214F4
		public void DropItemEquip(uint id)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				short x = 0;
				short y = 0;
				if (this.GetDropItemPoint(ref x, ref y))
				{
					this.DeleteItemByID(id);
					this.play.GetGameMap().AddDropItemObj(roleItemInfo.itemid, x, y, 0U, 120000, roleItemInfo, null);
				}
			}
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00023364 File Offset: 0x00021564
		public static bool CanDropFromPosition(ushort position)
		{
			return position == MsgItemInfo.ITEMPOSITION_BACKPACK ||
				position == MsgItemInfo.ITEMPOSITION_EUDEMON_PACK;
		}

		public static bool IsAdjacentDropPoint(
			short playerX,
			short playerY,
			short dropX,
			short dropY)
		{
			int deltaX = Math.Abs((int)dropX - playerX);
			int deltaY = Math.Abs((int)dropY - playerY);
			return deltaX <= 1 && deltaY <= 1 &&
				(deltaX != 0 || deltaY != 0);
		}

		public void DropItemBag(uint id)
		{
			short x = 0;
			short y = 0;
			if (this.GetDropItemPoint(ref x, ref y))
			{
				this.DropItemBagAtPoint(id, x, y, false);
			}
		}

		public void DropItemBag(uint id, short dropX, short dropY)
		{
			this.DropItemBagAtPoint(id, dropX, dropY, true);
		}

		private void DropItemBagAtPoint(
			uint id,
			short dropX,
			short dropY,
			bool validateClientPoint)
		{
			if (this.play.GetTimerSystem().QueryStatus(1010) != null)
			{
				this.play.MsgBox("In Stall Mode, Items Cannot Be Discarded!");
			}
			else
			{
				uint key = id;
				if (id >= IDManager.eudemon_start_id)
				{
					key = this.GetEudemonItemId(id);
				}
				if (this.mDicItem.ContainsKey(key))
				{
					RoleItemInfo roleItemInfo = this.mDicItem[key];
					if (!CanDropFromPosition(roleItemInfo.postion))
					{
						return;
					}
					if (validateClientPoint &&
						(!IsAdjacentDropPoint(
							this.play.GetCurrentX(),
							this.play.GetCurrentY(),
							dropX,
							dropY) ||
						 this.play.GetGameMap().GetPointOfObj(
							this.play, dropX, dropY)))
					{
						return;
					}
					RoleData_Eudemon roleData_Eudemon = null;
					if (roleItemInfo.postion ==
						MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
					{
						roleData_Eudemon =
							this.play.GetEudemonSystem().FindEudemon(id);
						EudemonObject eudemonObject =
							this.play.GetEudemonSystem().GetEudmeonObject(id);
						if (roleData_Eudemon == null ||
							this.play.GetEudemonSystem().GetBattleEudemon(id) != null ||
							(eudemonObject != null && eudemonObject.IsRiding()))
						{
							return;
						}
					}
					if (!this.DeleteItemByID(id))
					{
						return;
					}
					if (roleData_Eudemon != null)
					{
						PlayerEudemon.PrepareDroppedEudemonForDatabaseRecreation(
							roleData_Eudemon);
					}
					this.play.GetGameMap().AddDropItemObj(
						roleItemInfo.itemid,
						dropX,
						dropY,
						0U,
						120000,
						roleItemInfo,
						roleData_Eudemon);
				}
			}
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00023488 File Offset: 0x00021688
		public void DropGold(int gold)
		{
			this.play.ChangeAttribute(UserAttribute.GOLD, -gold, true);
			this.play.LeftNotice(string.Format("You lost {0} gold coins!", gold));
			RoleItemInfo roleItemInfo = new RoleItemInfo();
			roleItemInfo.property = gold;
			short x = 0;
			short y = 0;
			if (this.GetDropItemPoint(ref x, ref y))
			{
				if (gold < 10)
				{
					roleItemInfo.itemid = 1090000U;
				}
				else if (gold > 10 && gold < 100)
				{
					roleItemInfo.itemid = 1090010U;
				}
				else if (gold > 100 && gold < 500)
				{
					roleItemInfo.itemid = 1090020U;
				}
				else if (gold > 500 && gold < 1500)
				{
					roleItemInfo.itemid = 1090030U;
				}
				else
				{
					roleItemInfo.itemid = 1090040U;
				}
				this.play.GetGameMap().AddDropItemObj(roleItemInfo.itemid, x, y, 0U, 120000, roleItemInfo, null);
			}
		}

		// Token: 0x0600030D RID: 781 RVA: 0x000235A8 File Offset: 0x000217A8
		public bool IsItemFull()
		{
			return !this.CanAcceptAtPosition(
				MsgItemInfo.ITEMPOSITION_BACKPACK);
		}

		public void NotifyPackageFull(ushort position)
		{
			if (position == MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
			{
				this.play.LeftNotice("Your eudemon bag is full.");
			}
			else if (position == MsgItemInfo.ITEMPOSITION_BACKPACK)
			{
				this.play.LeftNotice("Your item bar is full.");
			}
			else if (position == MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK)
			{
				this.play.LeftNotice(
					"All three incubator slots are occupied.");
			}
		}

		// Token: 0x0600030E RID: 782 RVA: 0x000235D8 File Offset: 0x000217D8
		public RoleItemInfo GetEquipByPostion(byte postion)
		{
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == (ushort)postion)
				{
					return roleItemInfo;
				}
			}
			return null;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0002364C File Offset: 0x0002184C
		public bool IsEquip(uint itemid)
		{
			ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(itemid);
			bool result;
			if (itemTypeInfo == null)
			{
				result = GameServer.IsTestMode();
			}
			else
			{
				result = (this.GetEquipPostion(itemTypeInfo) != 0);
			}
			return result;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0002369C File Offset: 0x0002189C
		public RoleItemInfo FindItem(uint id)
		{
			RoleItemInfo result;
			if (this.mDicItem.ContainsKey(id))
			{
				result = this.mDicItem[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000311 RID: 785 RVA: 0x000236D4 File Offset: 0x000218D4
		public RoleItemInfo FindItem(uint itemid, ref int nCount)
		{
			RoleItemInfo roleItemInfo = null;
			foreach (RoleItemInfo roleItemInfo2 in this.mDicItem.Values)
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo2.itemid);
				if (itemTypeInfo != null && itemTypeInfo.id == itemid)
				{
					if (roleItemInfo == null)
					{
						roleItemInfo = roleItemInfo2;
					}
					nCount++;
				}
			}
			return roleItemInfo;
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00023778 File Offset: 0x00021978
		public RoleItemInfo FindItem(string name, ref int nCount)
		{
			RoleItemInfo roleItemInfo = null;
			foreach (RoleItemInfo roleItemInfo2 in this.mDicItem.Values)
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo2.itemid);
				if (itemTypeInfo != null && itemTypeInfo.name == name)
				{
					if (roleItemInfo == null)
					{
						roleItemInfo = roleItemInfo2;
					}
					nCount++;
				}
			}
			return roleItemInfo;
		}

		// Token: 0x06000313 RID: 787 RVA: 0x00023820 File Offset: 0x00021A20
		public void DeleteItemByItemName(string name, int count = 1)
		{
			List<RoleItemInfo> list = new List<RoleItemInfo>();
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
				if (itemTypeInfo != null && itemTypeInfo.name == name)
				{
					this.DeleteItemByID(roleItemInfo.id);
					list.Add(roleItemInfo);
					if (list.Count == count)
					{
						break;
					}
				}
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.DeleteItemByID(list[i].id);
				}
			}
		}

		// Token: 0x06000314 RID: 788 RVA: 0x00023944 File Offset: 0x00021B44
		private bool GetDropItemPoint(ref short x, ref short y)
		{
			x = this.play.GetCurrentX();
			y = this.play.GetCurrentY();
			short[] array = new short[]
			{
				0,
				-1,
				-1,
				-1,
				0,
				1,
				1,
				1,
				0
			};
			short[] array2 = new short[]
			{
				1,
				1,
				0,
				-1,
				-1,
				-1,
				0,
				1,
				0
			};
			for (int distance = 1; distance <= 4; distance++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (int)x + (int)array[j] * distance;
					int num2 = (int)y + (int)array2[j] * distance;
					if (!this.play.GetGameMap().GetPointOfObj(this.play, (short)num, (short)num2))
					{
						x = (short)num;
						y = (short)num2;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000315 RID: 789 RVA: 0x00023A0C File Offset: 0x00021C0C
		public void BuyGameShopItem(uint itemid, int nAmount)
		{
			if (nAmount > 0 && FitsCapacity(
				this.GetBagCount(),
				this.GetPendingPositionCount(
					MsgItemInfo.ITEMPOSITION_BACKPACK),
				0,
				nAmount,
				InventoryCapacity))
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(itemid);
				if (itemTypeInfo != null)
				{
					NpcShopInfo npcShopInfo = ConfigManager.Instance().GetNpcShopInfo(1207U);
					if (npcShopInfo != null)
					{
						int num = npcShopInfo.GetItemPrice(itemid) * nAmount;
						if (num > 0)
						{
							if (this.play.GetBaseAttr().gamegold >= num)
							{
								this.play.ChangeAttribute(UserAttribute.GAMEGOLD, -num, true);
								for (int i = 0; i < nAmount; i++)
								{
									this.AwardItem(itemid, 50, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000316 RID: 790 RVA: 0x00023AE0 File Offset: 0x00021CE0
		public void BuyItem(uint npcid, uint itemid)
		{
			if (this.play.GetCurrentNpcInfo() != null)
			{
				if (this.play.GetCurrentNpcInfo().id == npcid)
				{
					ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(itemid);
					if (itemTypeInfo != null)
					{
						NpcShopInfo npcShopInfo = ConfigManager.Instance().GetNpcShopInfo(npcid);
						if (npcShopInfo != null)
						{
							int itemPrice = npcShopInfo.GetItemPrice(itemid);
							if (itemPrice != -1)
							{
								if (this.play.GetMoneyCount(MONEYTYPE.GOLD) >= itemPrice &&
									this.CanAwardItem(
										itemid,
										MsgItemInfo.ITEMPOSITION_BACKPACK))
								{
									this.play.ChangeMoney(MONEYTYPE.GOLD, -itemPrice);
									this.AwardItem(itemid, 50, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x00023BA8 File Offset: 0x00021DA8
		public void SellItem(uint npcid, uint itemid)
		{
			if (this.play.GetCurrentNpcInfo() != null)
			{
				if (this.play.GetCurrentNpcInfo().id == npcid)
				{
					RoleItemInfo roleItemInfo = this.FindItem(itemid);
					if (roleItemInfo != null)
					{
						ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
						if (itemTypeInfo != null)
						{
							int value = (int)((double)itemTypeInfo.price * 0.8);
							this.play.ChangeAttribute(UserAttribute.GOLD, value, true);
							this.DeleteItemByID(itemid);
						}
					}
				}
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x00023C44 File Offset: 0x00021E44
		public void RepairEquip(uint npcid, uint itemid)
		{
			if (this.play.GetCurrentNpcInfo() != null)
			{
				if (this.play.GetCurrentNpcInfo().id == npcid)
				{
					RoleItemInfo roleItemInfo = this.FindItem(itemid);
					if (roleItemInfo == null)
					{
					}
				}
			}
		}

		// Token: 0x06000319 RID: 793 RVA: 0x00023C98 File Offset: 0x00021E98
		public void ClearItem(uint id)
		{
			uint id2 = id;
			if (id >= IDManager.eudemon_start_id)
			{
				id2 = this.GetEudemonItemId(id);
			}
			MsgClearItem msgClearItem = new MsgClearItem();
			msgClearItem.id = id2;
			msgClearItem.roleid = this.play.GetTypeId();
			this.play.SendData(msgClearItem.GetBuffer(), true);
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00023CF0 File Offset: 0x00021EF0
		public void GetItemStrongInfo(List<RoleItemInfo> list)
		{
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == 100)
				{
					list.Add(roleItemInfo);
				}
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00023D64 File Offset: 0x00021F64
		public void MoveItem(uint id, ushort dest_postion)
		{
			if (this.mDicItem.ContainsKey(id))
			{
				RoleItemInfo roleItemInfo = this.mDicItem[id];
				ushort postion = roleItemInfo.postion;
				if (postion != dest_postion &&
					!this.CanAcceptAtPosition(dest_postion))
				{
					this.NotifyPackageFull(dest_postion);
					return;
				}
				ushort num = postion;
				if (num == 50)
				{
					this.ClearItem(id);
				}
				roleItemInfo.postion = dest_postion;
				if (dest_postion == 50)
				{
					this.UpdateItemInfo(id);
				}
				if (postion == 100 || dest_postion == 100)
				{
					this.play.OpenDialog(3);
				}
			}
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00023DEC File Offset: 0x00021FEC
		public void SaveStrongMoney(int gold)
		{
			if (this.play.GetMoneyCount(MONEYTYPE.GOLD) >= gold)
			{
				this.play.ChangeAttribute(UserAttribute.GOLD, -gold, true);
				MsgStrongInfo msgStrongInfo = new MsgStrongInfo();
				msgStrongInfo.Create(null, this.play.GetGamePackKeyEx());
				this.play.ChangeMoney(MONEYTYPE.STRONGGOLD, gold);
				byte[] strongMoneyBuffer = MsgStrongInfo.GetStrongMoneyBuffer(this.play.GetTypeId(), this.play.GetMoneyCount(MONEYTYPE.STRONGGOLD));
				this.play.SendData(strongMoneyBuffer, true);
			}
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00023E74 File Offset: 0x00022074
		public void GiveStrongMoney(int gold)
		{
			if (this.play.GetMoneyCount(MONEYTYPE.STRONGGOLD) >= gold)
			{
				this.play.ChangeAttribute(UserAttribute.GOLD, gold, true);
				this.play.ChangeMoney(MONEYTYPE.STRONGGOLD, -gold);
				MsgStrongInfo msgStrongInfo = new MsgStrongInfo();
				msgStrongInfo.Create(null, this.play.GetGamePackKeyEx());
				byte[] strongMoneyBuffer = MsgStrongInfo.GetStrongMoneyBuffer(this.play.GetTypeId(), this.play.GetMoneyCount(MONEYTYPE.STRONGGOLD));
				this.play.SendData(strongMoneyBuffer, true);
			}
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00023EFC File Offset: 0x000220FC
		public int GetStrongItemCount()
		{
			int num = 0;
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == 100)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00023F78 File Offset: 0x00022178
		public int GetEudemonCount()
		{
			int num = 0;
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == 53)
				{
					num++;
				}
			}
			return num;
		}

		public bool IsEudemonInBag(uint eudemonId)
		{
			uint itemId = this.GetEudemonItemId(eudemonId);
			RoleItemInfo item = this.FindItem(itemId);
			return item != null &&
				item.postion == MsgItemInfo.ITEMPOSITION_EUDEMON_PACK;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00023FF4 File Offset: 0x000221F4
		public uint GetEudemonItemId(uint eudemon_id)
		{
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.typeid == eudemon_id)
				{
					return roleItemInfo.id;
				}
			}
			return 0U;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00024070 File Offset: 0x00022270
		public void ChangeLookFace(uint itemid)
		{
			LookFaceInfo lookFaceInfo = ConfigManager.Instance().GetLookFaceInfo(itemid);
			if (lookFaceInfo != null)
			{
				if (this.play.GetMoneyCount(MONEYTYPE.GOLD) < lookFaceInfo.price)
				{
					this.play.LeftNotice("Not enough gold coins, cannot purchase!");
				}
				else if ((int)(this.play.GetSex() % 2) != lookFaceInfo.lookfaceid % 2)
				{
					this.play.LeftNotice("Gender does not match, cannot purchase! ");
				}
				else
				{
					this.play.ChangeMoney(MONEYTYPE.GOLD, -lookFaceInfo.price);
					this.play.ChangeAttribute(UserAttribute.LOOKFACE, lookFaceInfo.lookfaceid, true);
				}
			}
		}

		public static bool IsWardrobeMountType(uint itemTypeId)
		{
			return itemTypeId == RoseFlowerCartBridleType ||
				itemTypeId == RoseFlyCarSaddleType;
		}

		public static bool IsWardrobeWeaponSoulType(uint itemTypeId)
		{
			uint weaponFamily = itemTypeId / 1000U;
			uint soulStyle = itemTypeId % 1000U;
			bool supportedWeaponFamily =
				weaponFamily == WarriorBladeSoulFamily ||
				weaponFamily == WarriorSwordSoulFamily ||
				weaponFamily == MageStaffSoulFamily ||
				weaponFamily == MageScepterSoulFamily ||
				weaponFamily == VampireTalonsSoulFamily ||
				weaponFamily == NecromancerSoulFamily ||
				weaponFamily == PaladinWandSoulFamily;
			bool supportedSoulStyle = soulStyle == 0U ||
				soulStyle == 10U || soulStyle == 20U ||
				soulStyle == 30U || soulStyle == 50U ||
				soulStyle == 60U || soulStyle == 70U;
			return supportedWeaponFamily && supportedSoulStyle;
		}

		public bool PurchaseWardrobeWeaponSoul(uint itemTypeId)
		{
			ItemTypeInfo itemType =
				ConfigManager.Instance().GetItemTypeInfo(itemTypeId);
			if (!IsWardrobeWeaponSoulType(itemTypeId) || itemType == null ||
				this.GetEquipPostion(itemType) !=
					MsgItemInfo.ITEMPOSTION_WEPON_SOUL)
			{
				this.play.LeftNotice("This wardrobe weapon soul is not available.");
				return false;
			}

			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.itemid == itemTypeId)
				{
					if (item.postion == WardrobeWeaponSoulPosition)
					{
						this.SendItemInfo(item, 1);
					}
					this.play.LeftNotice("You already own this weapon soul.");
					return true;
				}
			}
			foreach (RoleItemInfo pendingItem in this.mDicAddItem.Values)
			{
				if (pendingItem.itemid == itemTypeId)
				{
					return true;
				}
			}

			NpcShopInfo collectionShop =
				ConfigManager.Instance().GetNpcShopInfo(1207U);
			int price = collectionShop == null
				? -1
				: collectionShop.GetItemPrice(itemTypeId);
			if (price <= 0)
			{
				this.play.LeftNotice(
					"This wardrobe weapon soul has no server purchase price.");
				return false;
			}
			if (this.play.GetMoneyCount(MONEYTYPE.GAMEGOLD) < price)
			{
				this.play.LeftNotice("Not enough EP to unlock this weapon soul.");
				return false;
			}

			RoleItemInfo pending = this.AwardItem(
				itemTypeId,
				(byte)WardrobeWeaponSoulPosition,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				false,
				0,
				price);
			if (pending == null)
			{
				return false;
			}

			this.play.ChangeMoney(MONEYTYPE.GAMEGOLD, -price);
			Log.Instance().WriteLog(
				"Wardrobe weapon soul purchase queued for " +
				this.play.GetName() + ": type=" + itemTypeId.ToString() +
				", price=" + price.ToString() + " EP.");
			return true;
		}

		public static uint GetWardrobeMountServerType(uint itemTypeId)
		{
			return IsWardrobeMountType(itemTypeId)
				? WardrobeMountServerType
				: 0U;
		}

		public bool PurchaseWardrobeMount(uint itemTypeId)
		{
			if (!IsWardrobeMountType(itemTypeId) ||
				ConfigManager.Instance().GetItemTypeInfo(itemTypeId) == null)
			{
				this.play.LeftNotice("This wardrobe mount is not available.");
				return false;
			}

			if (this.FindWardrobeMountByType(itemTypeId) != null)
			{
				this.SendWardrobeMountPackage();
				return true;
			}
			foreach (RoleItemInfo pendingItem in this.mDicAddItem.Values)
			{
				if (pendingItem.postion == WardrobeMountPosition &&
					pendingItem.itemid == itemTypeId)
				{
					return true;
				}
			}

			int ownedCount = 0;
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == WardrobeMountPosition)
				{
					ownedCount++;
				}
			}
			if (!FitsCapacity(
				ownedCount,
				this.GetPendingPositionCount(WardrobeMountPosition),
				0,
				1,
				WardrobeMountCapacity))
			{
				this.play.LeftNotice("Your mount wardrobe is full.");
				return false;
			}

			NpcShopInfo collectionShop =
				ConfigManager.Instance().GetNpcShopInfo(1207U);
			int price = collectionShop == null
				? -1
				: collectionShop.GetItemPrice(itemTypeId);
			if (price <= 0)
			{
				this.play.LeftNotice(
					"This wardrobe mount has no server purchase price.");
				return false;
			}
			if (this.play.GetMoneyCount(MONEYTYPE.GAMEGOLD) < price)
			{
				this.play.LeftNotice("Not enough EP to unlock this mount.");
				return false;
			}

			RoleItemInfo pending = this.AwardItem(
				itemTypeId,
				(byte)WardrobeMountPosition,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				false,
				price);
			if (pending == null)
			{
				return false;
			}

			this.play.ChangeMoney(MONEYTYPE.GAMEGOLD, -price);
			Log.Instance().WriteLog(
				"Wardrobe mount purchase queued for " + this.play.GetName() +
				": type=" + itemTypeId.ToString() +
				", price=" + price.ToString() + " EP.");
			return true;
		}

		public bool EquipWardrobeMount(uint itemId)
		{
			RoleItemInfo item = this.FindWardrobeMountById(itemId);
			if (item == null)
			{
				return false;
			}

			uint mountServerType = GetWardrobeMountServerType(item.itemid);
			if (mountServerType == 0U)
			{
				return false;
			}
			this.play.TakeWardrobeMount(item.id, mountServerType);
			return this.play.GetMountID() == item.id;
		}

		public void SendWardrobeMountPackage()
		{
			List<RoleItemInfo> ownedItems = new List<RoleItemInfo>();
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == WardrobeMountPosition &&
					IsWardrobeMountType(item.itemid))
				{
					ownedItems.Add(item);
				}
			}
			ownedItems.Sort(delegate(RoleItemInfo left, RoleItemInfo right)
			{
				return left.id.CompareTo(right.id);
			});

			List<WardrobePackageItem> packageItems =
				new List<WardrobePackageItem>();
			foreach (RoleItemInfo item in ownedItems)
			{
				ItemTypeInfo itemType =
					ConfigManager.Instance().GetItemTypeInfo(item.itemid);
				packageItems.Add(new WardrobePackageItem
				{
					ItemId = item.id,
					ItemTypeId = item.itemid,
					Amount = item.amount == 0 ? (ushort)1 : item.amount,
					AmountLimit = itemType == null || itemType.amount_limit == 0
						? (ushort)1
						: itemType.amount_limit
				});
			}

			byte[] packet = MapPacketCodec.CreateWardrobePackageListResponse(
				null,
				MsgStrongPack.MOUNT_PACKAGE_TYPE,
				WardrobeMountCapacity,
				packageItems);
			this.play.SendData(packet, true);
		}

		private RoleItemInfo FindWardrobeMountByType(uint itemTypeId)
		{
			foreach (RoleItemInfo item in this.mDicItem.Values)
			{
				if (item.postion == WardrobeMountPosition &&
					item.itemid == itemTypeId)
				{
					return item;
				}
			}
			return null;
		}

		private RoleItemInfo FindWardrobeMountById(uint itemId)
		{
			RoleItemInfo item;
			if (!this.mDicItem.TryGetValue(itemId, out item) ||
				item.postion != WardrobeMountPosition ||
				!IsWardrobeMountType(item.itemid))
			{
				return null;
			}
			return item;
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00024124 File Offset: 0x00022324
		public void ChangeHair(uint itemid)
		{
			HairInfo hairInfo = ConfigManager.Instance().GetHairInfo(itemid);
			#if DEBUG
			Log.Instance().WriteLog(
				"Hair purchase lookup for " + this.play.GetName() +
				": item=" + itemid.ToString() + ", found=" +
				(hairInfo != null).ToString() + ", sex=" +
				this.play.GetSex().ToString() + ", gold=" +
				this.play.GetMoneyCount(MONEYTYPE.GOLD).ToString() + ".");
			#endif
			if (hairInfo != null)
			{
				this.play.GetWardrobeSystem().PurchaseHairWithGold(hairInfo);
			}
		}

		// Token: 0x06000323 RID: 803 RVA: 0x000241D0 File Offset: 0x000223D0
		public static bool TryGetWardrobeRoute(
			byte packageType,
			byte operation,
			out ushort sourcePosition,
			out ushort destinationPosition,
			out byte requiredEquipmentPosition)
		{
			sourcePosition = 0;
			destinationPosition = 0;
			requiredEquipmentPosition = 0;
			ushort chestPosition;
			if (packageType == MsgStrongPack.FASHION_PACKAGE_TYPE)
			{
				chestPosition = MsgItemInfo.ITEMPOSITION_CHEST;
				requiredEquipmentPosition = MsgItemInfo.ITEMPOSITION_FASHION;
			}
			else if (packageType == MsgStrongPack.WEAPON_SOUL_PACKAGE_TYPE)
			{
				chestPosition = MsgItemInfo.ITEMPOSITION_CHEST_SOUL;
				requiredEquipmentPosition = MsgItemInfo.ITEMPOSTION_WEPON_SOUL;
			}
			else
			{
				return false;
			}

			if (operation == MsgStrongPack.PACKAGE_CHECK_IN)
			{
				sourcePosition = MsgItemInfo.ITEMPOSITION_BACKPACK;
				destinationPosition = chestPosition;
				return true;
			}
			if (operation == MsgStrongPack.PACKAGE_CHECK_OUT)
			{
				sourcePosition = chestPosition;
				destinationPosition = MsgItemInfo.ITEMPOSITION_BACKPACK;
				return true;
			}
			return false;
		}

		public bool MoveWardrobeItem(
			uint itemId,
			byte packageType,
			byte operation)
		{
			ushort sourcePosition;
			ushort destinationPosition;
			byte requiredEquipmentPosition;
			if (!TryGetWardrobeRoute(
				packageType,
				operation,
				out sourcePosition,
				out destinationPosition,
				out requiredEquipmentPosition))
			{
				return false;
			}

			RoleItemInfo item = this.FindItem(itemId);
			if (item == null || item.postion != sourcePosition)
			{
				return false;
			}
			ItemTypeInfo itemType =
				ConfigManager.Instance().GetItemTypeInfo(item.itemid);
			if (itemType == null ||
				this.GetEquipPostion(itemType) != requiredEquipmentPosition)
			{
				return false;
			}

			if (!this.CanAcceptAtPosition(destinationPosition))
			{
				this.NotifyPackageFull(destinationPosition);
				return false;
			}
			this.MoveItem(itemId, destinationPosition);
			if (item.postion != destinationPosition)
			{
				return false;
			}
			if (destinationPosition != MsgItemInfo.ITEMPOSITION_BACKPACK)
			{
				this.UpdateItemInfo(itemId);
			}
			return true;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00024221 File Offset: 0x00022421
		public void AddTradItem(RoleItemInfo info)
		{
			this.mDicItem[info.id] = info;
			this.SendItemInfo(info, 1);
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00024240 File Offset: 0x00022440
		public byte GetEquipPostion(ItemTypeInfo info)
		{
			string text = info.id.ToString();
			byte result;
			if (info.id == 1110010U)
			{
				result = 13;
			}
			else if (info.id == 1110110U)
			{
				result = 14;
			}
			else if (info.id == 1110210U)
			{
				result = 15;
			}
			else if (text[0] == '4' && text[2] == '5')
			{
				result = 26;
			}
			else if (text[0] == '4')
			{
				result = 4;
			}
			else if (text[0] == '1' && text[1] == '1')
			{
				result = 1;
			}
			else if (text[0] == '1' && text[1] == '2')
			{
				result = 2;
			}
			else if (text[0] == '1' && text[1] == '3')
			{
				result = 3;
			}
			else if (text[0] == '1' && text[1] == '4')
			{
				result = 7;
			}
			else if (text[0] == '1' && text[1] == '6')
			{
				result = 8;
			}
			else if (text[0] == '1' && text[1] == '9')
			{
				result = 12;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00024400 File Offset: 0x00022600
		public void SendLookRoleInfo(PlayerObject target)
		{
			for (int i = 1; i < 16; i++)
			{
				RoleItemInfo equipByPostion = this.GetEquipByPostion((byte)i);
				if (equipByPostion != null)
				{
					target.GetItemSystem().SendItemInfo(equipByPostion, 4);
				}
			}
			byte[] v = new byte[]
			{
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				69,
				0,
				0,
				0
			};
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteInt16(40);
			packetOut.WriteInt16(2036);
			packetOut.WriteInt32(524359);
			packetOut.WriteUInt32(this.play.GetTypeId());
			packetOut.WriteInt32(this.play.GetFightSoul());
			packetOut.WriteBuff(v);
			target.SendData(packetOut.Flush(), true);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x000244BC File Offset: 0x000226BC
		public void Process_DieEudemon()
		{
			List<uint> list = new List<uint>();
			foreach (RoleItemInfo roleItemInfo in this.mDicItem.Values)
			{
				if (roleItemInfo.postion == 53)
				{
					EudemonObject eudmeonObject = this.play.GetEudemonSystem().GetEudmeonObject(roleItemInfo.typeid);
					if (eudmeonObject == null)
					{
						list.Add(roleItemInfo.id);
					}
				}
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.DeleteItemByID(list[i]);
				}
			}
		}

		// Token: 0x0400060C RID: 1548
		private const int IETMSORT_FINERY = 1;

		// Token: 0x0400060D RID: 1549
		private const int ITEMSORT_MOUNT = 6;

		// Token: 0x0400060E RID: 1550
		public const int MAX_STRONGITEM = 100;

		// Token: 0x0400060F RID: 1551
		public const long MAX_GOLD = 3000000000L;

		// Token: 0x04000610 RID: 1552
		public const int MAXBAG_COUNT = InventoryCapacity;

		// Token: 0x04000611 RID: 1553
		private uint mScriptItemId = 0U;

		// Token: 0x04000612 RID: 1554
		private PlayerObject play;

		// Token: 0x04000613 RID: 1555
		private Dictionary<uint, RoleItemInfo> mDicItem;

		// Token: 0x04000614 RID: 1556
		private Dictionary<uint, RoleItemInfo> mDicAddItem;

		private Dictionary<RoleItemInfo, int> mWardrobeMountPendingPrices;

		private Dictionary<RoleItemInfo, int>
			mWardrobeWeaponSoulPendingPrices;

		private Dictionary<RoleItemInfo, DropItemObject>
			mPendingDroppedItemPickups;

		private uint mNextAddItemSortId;

		// Token: 0x04000615 RID: 1557
		private uint mWeaponId;

		// Token: 0x04000616 RID: 1558
		private uint mArmorId;

		// Token: 0x04000617 RID: 1559
		private uint mFashionId;
	}
}
