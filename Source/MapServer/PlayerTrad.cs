using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000096 RID: 150
	public class PlayerTrad
	{
		// Token: 0x060003C4 RID: 964 RVA: 0x0002C0DC File Offset: 0x0002A2DC
		public PlayerTrad(PlayerObject _play)
		{
			this.mnGameGold = (this.mnGold = 0);
			this.play = _play;
			this.mbSureTrad = false;
			this.mListItem = new List<RoleItemInfo>();
			this.mListEudemon = new List<RoleData_Eudemon>();
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0002C148 File Offset: 0x0002A348
		public void SetTrading(bool v)
		{
			this.mIsTrad = v;
			if (this.mIsTrad)
			{
				PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
				if (playerObject == null)
				{
					this.SetTrading(false);
				}
				else
				{
					PacketOut packetOut = new PacketOut(playerObject.GetGamePackKeyEx());
					packetOut.WriteUInt16(16);
					packetOut.WriteUInt16(1056);
					packetOut.WriteUInt32(this.play.GetTypeId());
					packetOut.WriteUInt32(3U);
					packetOut.WriteUInt32(1U);
					playerObject.SendData(packetOut.Flush(), false);
				}
			}
			else
			{
				byte[] v2 = new byte[]
				{
					16,
					0,
					32,
					4,
					0,
					0,
					0,
					0,
					5,
					0,
					0,
					0,
					0,
					0,
					0,
					0
				};
				PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
				packetOut.WriteBuff(v2);
				this.play.SendData(packetOut.Flush(), false);
				this.play.LeftNotice("Transaction Failed!");
				this.play.ChangeAttribute(UserAttribute.GOLD, this.mnGold, true);
				if (this.mnGameGold > 0)
				{
					this.play.ChangeAttribute(UserAttribute.GAMEGOLD, this.mnGameGold, true);
				}
				if (this.mnGold > 0)
				{
					this.play.ChangeAttribute(UserAttribute.GOLD, this.mnGameGold, true);
				}
				for (int i = 0; i < this.mListItem.Count; i++)
				{
					this.play.GetItemSystem().AddTradItem(this.mListItem[i]);
				}
				for (int i = 0; i < this.mListEudemon.Count; i++)
				{
					this.play.GetEudemonSystem().AddEudemon(this.mListEudemon[i]);
				}
				this.mListItem.Clear();
				this.mListEudemon.Clear();
			}
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0002C33C File Offset: 0x0002A53C
		public bool IsTrading()
		{
			return this.mIsTrad;
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0002C354 File Offset: 0x0002A554
		public void RequstTrad(MsgTradInfo info)
		{
			PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(info.typeid);
			if (playerObject == null)
			{
				this.play.LeftNotice("The other party has gone offline, unable to trade!");
			}
			else if (this.play.GetTimerSystem().QueryStatus(1010) != null)
			{
				this.play.MsgBox("Cannot trade during booth setup!");
			}
			else if (playerObject.GetTimerSystem().QueryStatus(1010) != null)
			{
				this.play.MsgBox("The other party is currently vending, unable to trade!");
			}
			else if (this.GetTradTarget() == info.typeid)
			{
				playerObject.GetTradSystem().SetTrading(true);
				this.play.GetTradSystem().SetTrading(true);
			}
			else if (this.GetTradTarget() != 0U)
			{
				this.play.LeftNotice("Currently in transaction, cannot trade again.");
			}
			else
			{
				int num = Math.Abs((int)(this.play.GetCurrentX() - playerObject.GetCurrentX()));
				int num2 = Math.Abs((int)(this.play.GetCurrentY() - playerObject.GetCurrentY()));
				if (num > 18 || num2 > 18)
				{
					this.play.LeftNotice("Distance too far, unable to trade");
				}
				else
				{
					MsgTradInfo msgTradInfo = new MsgTradInfo();
					msgTradInfo.Create(null, playerObject.GetGamePackKeyEx());
					msgTradInfo.typeid = this.play.GetTypeId();
					msgTradInfo.type = 1;
					msgTradInfo.level = (short)this.play.GetBaseAttr().level;
					msgTradInfo.fightpower = (short)this.play.GetFightSoul();
					playerObject.SendData(msgTradInfo.GetBuffer(), false);
					playerObject.GetTradSystem().SetTradTarget(this.play.GetTypeId());
					this.play.GetTradSystem().SetTradTarget(playerObject.GetTypeId());
					this.play.LeftNotice("[Transaction]Transaction request has been sent.");
				}
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0002C560 File Offset: 0x0002A760
		public void QuitTrad(MsgTradInfo info)
		{
			if (this.GetTradTarget() != 0U)
			{
				this.SetTrading(false);
				this.SetSureTradTag(false);
				PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
				if (playerObject != null)
				{
					this.SetTradTarget(0U);
					playerObject.GetTradSystem().SetTradTarget(0U);
					playerObject.GetTradSystem().SetTrading(false);
					playerObject.GetTradSystem().SetSureTradTag(false);
				}
			}
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0002C5DE File Offset: 0x0002A7DE
		public void SetTradTarget(uint typeid)
		{
			this.mTargetId = typeid;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0002C5E8 File Offset: 0x0002A7E8
		public uint GetTradTarget()
		{
			return this.mTargetId;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0002C600 File Offset: 0x0002A800
		public void SetTradGold(int gold)
		{
			if (gold > 0)
			{
				PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
				if (playerObject != null)
				{
					this.mnGold = gold;
					PacketOut packetOut = new PacketOut(playerObject.GetGamePackKeyEx());
					packetOut.WriteUInt16(16);
					packetOut.WriteUInt16(1056);
					packetOut.WriteInt32(this.mnGold);
					packetOut.WriteInt32(8);
					packetOut.WriteInt32(0);
					playerObject.SendData(packetOut.Flush(), false);
				}
			}
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0002C68C File Offset: 0x0002A88C
		public int GetTradGold()
		{
			return this.mnGold;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0002C6A4 File Offset: 0x0002A8A4
		public void SetTradGameGold(int gamegold)
		{
			if (gamegold > 0)
			{
				PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
				if (playerObject != null)
				{
					this.mnGameGold = gamegold;
					PacketOut packetOut = new PacketOut(playerObject.GetGamePackKeyEx());
					packetOut.WriteUInt16(16);
					packetOut.WriteUInt16(1056);
					packetOut.WriteInt32(this.mnGameGold);
					packetOut.WriteInt32(12);
					packetOut.WriteInt32(0);
					playerObject.SendData(packetOut.Flush(), false);
				}
			}
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0002C730 File Offset: 0x0002A930
		public int GetTradGameGold()
		{
			return this.mnGameGold;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0002C748 File Offset: 0x0002A948
		public void SetSureTradTag(bool v)
		{
			this.mbSureTrad = v;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0002C754 File Offset: 0x0002A954
		public bool GetSureTradTag()
		{
			return this.mbSureTrad;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0002C76C File Offset: 0x0002A96C
		public void AddTradItem(uint itemid)
		{
			PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
			if (playerObject != null)
			{
				RoleItemInfo roleItemInfo;
				if (itemid >= IDManager.eudemon_start_id)
				{
					RoleData_Eudemon roleData_Eudemon = this.play.GetEudemonSystem().FindEudemon(itemid);
					if (roleData_Eudemon == null)
					{
						return;
					}
					roleItemInfo = this.play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
				}
				else
				{
					roleItemInfo = this.play.GetItemSystem().FindItem(itemid);
				}
				if (roleItemInfo != null)
				{
					if (this.mListItem.Count < 20)
					{
						for (int index = 0;
							index < this.mListItem.Count;
							index++)
						{
							if (this.mListItem[index].id == roleItemInfo.id)
							{
								return;
							}
						}

						if (roleItemInfo.postion == 50)
						{
							if (!playerObject.GetItemSystem().CanAcceptAtPosition(
								MsgItemInfo.ITEMPOSITION_BACKPACK,
								this.GetOfferedPositionCount(
									MsgItemInfo.ITEMPOSITION_BACKPACK) + 1,
								playerObject.GetTradSystem().
									GetOfferedPositionCount(
										MsgItemInfo.ITEMPOSITION_BACKPACK)))
							{
								this.RestoreRejectedTradeItem(itemid);
								this.play.LeftNotice("The other party's item bar is full, cannot place more items");
								return;
							}
						}
						else if (roleItemInfo.postion == 53)
						{
							if (!playerObject.GetItemSystem().CanAcceptAtPosition(
								MsgItemInfo.ITEMPOSITION_EUDEMON_PACK,
								this.GetOfferedPositionCount(
									MsgItemInfo.ITEMPOSITION_EUDEMON_PACK) + 1,
								playerObject.GetTradSystem().
									GetOfferedPositionCount(
										MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)))
							{
								this.RestoreRejectedTradeItem(itemid);
								this.play.LeftNotice("The other party's beast inventory is full, unable to place more items");
								return;
							}
						}
						this.mListItem.Add(roleItemInfo);
						playerObject.GetItemSystem().SendItemInfo(roleItemInfo, 2);
						if (roleItemInfo.postion == 53)
						{
							RoleData_Eudemon roleData_Eudemon = this.play.GetEudemonSystem().FindEudemon(itemid);
							if (roleData_Eudemon != null)
							{
								this.mListEudemon.Add(roleData_Eudemon);
								this.play.GetEudemonSystem().SendLookTradEudemonInfo(playerObject, roleData_Eudemon);
							}
						}
					}
				}
			}
		}

		private void RestoreRejectedTradeItem(uint itemid)
		{
			MsgTradInfo failure = new MsgTradInfo();
			failure.Create(null, this.play.GetGamePackKeyEx());
			failure.typeid = itemid;
			failure.type = MsgTradInfo.ADD_ITEM_FAILED;
			this.play.SendData(failure.GetBuffer(), false);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0002C906 File Offset: 0x0002AB06
		public void ClearTradItem()
		{
			this.mListItem.Clear();
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0002C918 File Offset: 0x0002AB18
		public List<RoleItemInfo> GetTradItem()
		{
			return this.mListItem;
		}

		private int GetOfferedPositionCount(ushort position)
		{
			int count = 0;
			for (int index = 0; index < this.mListItem.Count; index++)
			{
				if (this.mListItem[index].postion == position)
				{
					count++;
				}
			}
			return count;
		}

		private bool CanReceiveItemsFrom(PlayerTrad source)
		{
			return this.play.GetItemSystem().CanAcceptAtPosition(
				MsgItemInfo.ITEMPOSITION_BACKPACK,
				source.GetOfferedPositionCount(
					MsgItemInfo.ITEMPOSITION_BACKPACK),
				this.GetOfferedPositionCount(
					MsgItemInfo.ITEMPOSITION_BACKPACK)) &&
				this.play.GetItemSystem().CanAcceptAtPosition(
					MsgItemInfo.ITEMPOSITION_EUDEMON_PACK,
					source.GetOfferedPositionCount(
						MsgItemInfo.ITEMPOSITION_EUDEMON_PACK),
					this.GetOfferedPositionCount(
						MsgItemInfo.ITEMPOSITION_EUDEMON_PACK));
		}

		private bool OfferedItemsAreTransferable()
		{
			for (int index = 0; index < this.mListItem.Count; index++)
			{
				RoleItemInfo item = this.mListItem[index];
				if (this.play.GetItemSystem().FindItem(item.id) == null ||
					ConfigManager.Instance().GetItemTypeInfo(item.itemid) == null)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0002C940 File Offset: 0x0002AB40
		public void SureTrad()
		{
			PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToTypeID(this.GetTradTarget());
			if (playerObject != null)
			{
				this.play.GetTradSystem().SetSureTradTag(true);
				byte[] array = new byte[]
				{
					16,
					0,
					32,
					4,
					0,
					0,
					0,
					0,
					10,
					0,
					0,
					0,
					0,
					0,
					0,
					0
				};
				playerObject.GetGamePackKeyEx().EncodePacket(ref array, array.Length);
				playerObject.SendData(array, false);
				if (playerObject.GetTradSystem().GetSureTradTag())
				{
					PlayerTrad other = playerObject.GetTradSystem();
					if (!this.OfferedItemsAreTransferable() ||
						!other.OfferedItemsAreTransferable() ||
						!this.CanReceiveItemsFrom(other) ||
						!other.CanReceiveItemsFrom(this))
					{
						this.SetSureTradTag(false);
						other.SetSureTradTag(false);
						this.play.LeftNotice(
							"Transaction cannot complete because a package is full or an offered item is unavailable.");
						playerObject.LeftNotice(
							"Transaction cannot complete because a package is full or an offered item is unavailable.");
						return;
					}
					playerObject.GetTradSystem().Trad(this.play);
					this.play.GetTradSystem().Trad(playerObject);
				}
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0002C9F8 File Offset: 0x0002ABF8
		public void Trad(PlayerObject obj)
		{
			int tradGold = obj.GetTradSystem().GetTradGold();
			if (tradGold > 0)
			{
				this.play.ChangeAttribute(UserAttribute.GOLD, tradGold, true);
			}
			int tradGameGold = obj.GetTradSystem().GetTradGameGold();
			if (tradGameGold > 0)
			{
				this.play.ChangeAttribute(UserAttribute.GAMEGOLD, tradGameGold, true);
			}
			obj.GetTradSystem().SetTradGameGold(0);
			obj.GetTradSystem().SetTradGold(0);
			List<RoleItemInfo> tradItem = obj.GetTradSystem().GetTradItem();
			for (int i = 0; i < tradItem.Count; i++)
			{
				if (tradItem[i].postion == 53)
				{
					RoleData_Eudemon roleData_Eudemon = obj.GetEudemonSystem().FindEudemon(tradItem[i].typeid);
					if (roleData_Eudemon != null)
					{
						this.play.GetEudemonSystem().AddTempEudemon(roleData_Eudemon);
					}
				}
				this.play.GetItemSystem().AwardItem(tradItem[i], false);
				obj.GetItemSystem().DeleteItemByID(tradItem[i].id);
			}
			obj.GetTradSystem().ClearTradItem();
			this.play.LeftNotice("Transaction Successful");
			this.SetSureTradTag(false);
			this.SetTradTarget(0U);
			this.mIsTrad = false;
			byte[] array = new byte[]
			{
				16,
				0,
				32,
				4,
				0,
				0,
				0,
				0,
				4,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			this.play.GetGamePackKeyEx().EncodePacket(ref array, array.Length);
			this.play.SendData(array, false);
		}

		// Token: 0x04000650 RID: 1616
		public PlayerObject play;

		// Token: 0x04000651 RID: 1617
		private uint mTargetId = 0U;

		// Token: 0x04000652 RID: 1618
		private bool mIsTrad = false;

		// Token: 0x04000653 RID: 1619
		private int mnGold;

		// Token: 0x04000654 RID: 1620
		private int mnGameGold;

		// Token: 0x04000655 RID: 1621
		private bool mbSureTrad;

		// Token: 0x04000656 RID: 1622
		private List<RoleItemInfo> mListItem;

		// Token: 0x04000657 RID: 1623
		private List<RoleData_Eudemon> mListEudemon;
	}
}
