using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200009A RID: 154
	public class PtichManager
	{
		// Token: 0x060003DC RID: 988 RVA: 0x0002CD90 File Offset: 0x0002AF90
		public static PtichManager Instance()
		{
			if (PtichManager.mInstance == null)
			{
				PtichManager.mInstance = new PtichManager();
			}
			return PtichManager.mInstance;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0002CDC4 File Offset: 0x0002AFC4
		public PtichManager()
		{
			this.mListPtichInfo = new List<PtichInfo>();
			for (int i = 0; i < 100; i++)
			{
				PtichInfo ptichInfo = new PtichInfo();
				ptichInfo.Id = i;
				ptichInfo.play = null;
				ptichInfo.PtichObj = null;
				ptichInfo.mSellItemList = new List<PtichSellItemInfo>();
				this.mListPtichInfo.Add(ptichInfo);
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0002CE2C File Offset: 0x0002B02C
		public bool AddPlayPtich(int nPtichId, PlayerObject play)
		{
			bool result;
			if (nPtichId < 0 || nPtichId >= 100)
			{
				result = false;
			}
			else if (this.PtichHasPlay(nPtichId))
			{
				result = false;
			}
			else if (play.GetTimerSystem().QueryStatus(1010) != null)
			{
				result = false;
			}
			else
			{
				play.GetTimerSystem().AddStatus(1010, 0, true);
				this.mListPtichInfo[nPtichId].play = play;
				play.SetDir(0);
				PacketOut packetOut = new PacketOut(null);
				PtichObject ptichObject = new PtichObject(play);
				ptichObject.SetPoint(
					(short)(play.GetCurrentX() + 1),
					(short)(play.GetCurrentY() + 1));
				play.GetGameMap().AddObject(ptichObject, null);
				ptichObject.Refresh();
				this.mListPtichInfo[nPtichId].PtichObj = ptichObject;
				packetOut = new PacketOut(null);
				packetOut.WriteInt16(28);
				packetOut.WriteInt16(1010);
				packetOut.WriteInt32(101088);
				packetOut.WriteUInt32(play.GetTypeId());
				packetOut.WriteInt16(ptichObject.GetCurrentX());
				packetOut.WriteInt16(ptichObject.GetCurrentY());
				packetOut.WriteInt32(0);
				packetOut.WriteUInt32(ptichObject.GetTypeId());
				packetOut.WriteInt32(9570);
				play.SendData(packetOut.Flush(), true);
				result = true;
			}
			return result;
		}

		// Token: 0x060003DF RID: 991 RVA: 0x0002CF84 File Offset: 0x0002B184
		public bool PtichHasPlay(int nPtichId)
		{
			bool result;
			if (this.mListPtichInfo[nPtichId].play == null)
			{
				result = false;
			}
			else
			{
				PlayerObject play = this.mListPtichInfo[nPtichId].play;
				result = (play.GetTimerSystem().QueryStatus(1010) != null);
			}
			return result;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x0002CFE8 File Offset: 0x0002B1E8
		public void DeletePlayPtich(PlayerObject play)
		{
			if (play.GetTimerSystem().QueryStatus(1010) == null)
			{
			}
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x0002D014 File Offset: 0x0002B214
		public uint GetPtichObjectTypeID(int nPtichId)
		{
			uint result;
			if (nPtichId < 0 || nPtichId >= this.mListPtichInfo.Count)
			{
				result = 0U;
			}
			else if (this.mListPtichInfo[nPtichId].PtichObj == null)
			{
				result = 0U;
			}
			else
			{
				result = this.mListPtichInfo[nPtichId].PtichObj.GetTypeId();
			}
			return result;
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x0002D07C File Offset: 0x0002B27C
		public void SellItem(PlayerObject play, uint item_id, byte type, int price)
		{
			if (play.GetTimerSystem().QueryStatus(1010) != null)
			{
				uint ptichObjectTypeID = this.GetPtichObjectTypeID(play.GetCurrentPtichID());
				if (ptichObjectTypeID != 0U)
				{
					RoleItemInfo roleItemInfo;
					if (item_id >= IDManager.eudemon_start_id)
					{
						RoleData_Eudemon roleData_Eudemon = play.GetEudemonSystem().FindEudemon(item_id);
						if (roleData_Eudemon == null)
						{
							return;
						}
						roleItemInfo = play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
						if (roleItemInfo == null)
						{
							return;
						}
					}
					else
					{
						roleItemInfo = play.GetItemSystem().FindItem(item_id);
						if (roleItemInfo == null)
						{
							return;
						}
					}
					if (ptichObjectTypeID != 0U)
					{
						int currentPtichID = play.GetCurrentPtichID();
						for (int i = 0; i < this.mListPtichInfo[currentPtichID].mSellItemList.Count; i++)
						{
							if (this.mListPtichInfo[currentPtichID].mSellItemList[i].item_id == item_id)
							{
								return;
							}
						}
						if (this.mListPtichInfo[currentPtichID].mSellItemList.Count < 18)
						{
							PacketOut packetOut = new PacketOut(null);
							packetOut.WriteInt16(28);
							packetOut.WriteInt16(1009);
							packetOut.WriteUInt32(item_id);
							packetOut.WriteInt32(price);
							packetOut.WriteInt32((int)type);
							packetOut.WriteInt32(0);
							packetOut.WriteInt32(0);
							packetOut.WriteInt32(0);
							play.SendData(packetOut.Flush(), true);
							roleItemInfo.postion = 111;
							PtichSellItemInfo ptichSellItemInfo = new PtichSellItemInfo();
							ptichSellItemInfo.item_id = item_id;
							ptichSellItemInfo.price = price;
							ptichSellItemInfo.sell_type = type;
							this.mListPtichInfo[currentPtichID].mSellItemList.Add(ptichSellItemInfo);
						}
					}
				}
			}
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x0002D284 File Offset: 0x0002B484
		public void GetBackItem(PlayerObject play, uint item_id)
		{
			if (play.GetTimerSystem().QueryStatus(1010) != null)
			{
				uint ptichObjectTypeID = this.GetPtichObjectTypeID(play.GetCurrentPtichID());
				if (ptichObjectTypeID != 0U)
				{
					RoleItemInfo roleItemInfo;
					if (item_id >= IDManager.eudemon_start_id)
					{
						RoleData_Eudemon roleData_Eudemon = play.GetEudemonSystem().FindEudemon(item_id);
						if (roleData_Eudemon == null)
						{
							return;
						}
						roleItemInfo = play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
						if (roleItemInfo == null)
						{
							return;
						}
					}
					else
					{
						roleItemInfo = play.GetItemSystem().FindItem(item_id);
						if (roleItemInfo == null)
						{
							return;
						}
					}
					int currentPtichID = play.GetCurrentPtichID();
					for (int i = 0; i < this.mListPtichInfo[currentPtichID].mSellItemList.Count; i++)
					{
						if (this.mListPtichInfo[currentPtichID].mSellItemList[i].item_id == item_id)
						{
							this.mListPtichInfo[currentPtichID].mSellItemList.RemoveAt(i);
							break;
						}
					}
					if (item_id >= IDManager.eudemon_start_id)
					{
						roleItemInfo.postion = 53;
					}
					else
					{
						roleItemInfo.postion = 50;
					}
					PacketOut packetOut = new PacketOut(null);
					packetOut.WriteInt16(28);
					packetOut.WriteInt16(1009);
					packetOut.WriteUInt32(item_id);
					packetOut.WriteUInt32(ptichObjectTypeID);
					packetOut.WriteInt32(23);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					play.SendData(packetOut.Flush(), true);
				}
			}
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0002D44C File Offset: 0x0002B64C
		public void ShutPtich(PlayerObject play, bool bSendData = true)
		{
			if (play.GetTimerSystem().QueryStatus(1010) != null)
			{
				uint ptichObjectTypeID = this.GetPtichObjectTypeID(play.GetCurrentPtichID());
				if (ptichObjectTypeID != 0U)
				{
					int i = 0;
					while (i < this.mListPtichInfo[play.GetCurrentPtichID()].mSellItemList.Count)
					{
						RoleItemInfo roleItemInfo;
						if (this.mListPtichInfo[play.GetCurrentPtichID()].mSellItemList[i].item_id < IDManager.eudemon_start_id)
						{
							roleItemInfo = play.GetItemSystem().FindItem(this.mListPtichInfo[play.GetCurrentPtichID()].mSellItemList[i].item_id);
							goto IL_FD;
						}
						RoleData_Eudemon roleData_Eudemon = play.GetEudemonSystem().FindEudemon(this.mListPtichInfo[play.GetCurrentPtichID()].mSellItemList[i].item_id);
						if (roleData_Eudemon != null)
						{
							roleItemInfo = play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
							goto IL_FD;
						}
						IL_1AC:
						i++;
						continue;
						IL_FD:
						if (roleItemInfo != null)
						{
							if (roleItemInfo.typeid >= IDManager.eudemon_start_id)
							{
								roleItemInfo.postion = 53;
							}
							else
							{
								roleItemInfo.postion = 50;
							}
							if (bSendData)
							{
								PacketOut packetOut = new PacketOut(null);
								packetOut.WriteInt16(28);
								packetOut.WriteInt16(1009);
								packetOut.WriteUInt32(roleItemInfo.id);
								packetOut.WriteUInt32(ptichObjectTypeID);
								packetOut.WriteInt32(23);
								packetOut.WriteInt32(0);
								packetOut.WriteInt32(0);
								packetOut.WriteInt32(0);
								play.SendData(packetOut.Flush(), true);
							}
						}
						goto IL_1AC;
					}
					this.mListPtichInfo[play.GetCurrentPtichID()].play = null;
					play.GetGameMap().RemoveObj(this.mListPtichInfo[play.GetCurrentPtichID()].PtichObj);
					this.mListPtichInfo[play.GetCurrentPtichID()].PtichObj = null;
					this.mListPtichInfo[play.GetCurrentPtichID()].mSellItemList.Clear();
					if (bSendData)
					{
						PacketOut packetOut = new PacketOut(null);
						packetOut.WriteInt16(16);
						packetOut.WriteInt16(2031);
						packetOut.WriteUInt32(ptichObjectTypeID);
						packetOut.WriteUInt32(play.GetTypeId());
						packetOut.WriteInt32(2);
						play.SendData(packetOut.Flush(), true);
					}
					play.GetTimerSystem().DeleteStatus(1010);
				}
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0002D738 File Offset: 0x0002B938
		public void LookPtich(PlayerObject play, uint ptich_obj_id)
		{
			int num = -1;
			for (int i = 0; i < this.mListPtichInfo.Count; i++)
			{
				if (this.mListPtichInfo[i].PtichObj != null)
				{
					if (this.mListPtichInfo[i].PtichObj.GetTypeId() == ptich_obj_id)
					{
						num = i;
						break;
					}
				}
			}
			if (num != -1)
			{
				byte[] v = new byte[]
				{
					42,
					0,
					105,
					4,
					244,
					1,
					0,
					0,
					64,
					66,
					15,
					0,
					36,
					52,
					156,
					8,
					3,
					0,
					0,
					0,
					30,
					214,
					44,
					135,
					2,
					0,
					0,
					0,
					164,
					3,
					178,
					5,
					1,
					0
				};
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteBuff(v);
				packetOut.WriteInt16((short)(num + 1));
				byte[] v2 = new byte[]
				{
					1,
					4,
					202,
					165,
					213,
					189
				};
				packetOut.WriteBuff(v2);
				play.SendData(packetOut.Flush(), true);
				int i = 0;
				while (i < this.mListPtichInfo[num].mSellItemList.Count)
				{
					RoleData_Eudemon roleData_Eudemon = null;
					RoleItemInfo roleItemInfo;
					if (this.mListPtichInfo[num].mSellItemList[i].item_id < IDManager.eudemon_start_id)
					{
						roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(this.mListPtichInfo[num].mSellItemList[i].item_id);
						goto IL_1B6;
					}
					roleData_Eudemon = this.mListPtichInfo[num].play.GetEudemonSystem().FindEudemon(this.mListPtichInfo[num].mSellItemList[i].item_id);
					if (roleData_Eudemon != null)
					{
						roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
						goto IL_1B6;
					}
					IL_24E:
					i++;
					continue;
					IL_1B6:
					if (roleItemInfo != null)
					{
						MsgPtichItemInfo msgPtichItemInfo = new MsgPtichItemInfo(roleItemInfo, ptich_obj_id, this.mListPtichInfo[num].mSellItemList[i].price, this.mListPtichInfo[num].mSellItemList[i].sell_type, false);
						play.SendData(msgPtichItemInfo.GetBuffer(), true);
						if (roleItemInfo.typeid >= IDManager.eudemon_start_id)
						{
							this.mListPtichInfo[num].play.GetEudemonSystem().SendLookPtichEudemonInfo(play, roleData_Eudemon);
						}
					}
					goto IL_24E;
				}
			}
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x0002D9BC File Offset: 0x0002BBBC
		public void BuyItem(PlayerObject play, uint ptich_obj_id, uint item_id)
		{
			if (play.GetTimerSystem().QueryStatus(1010) != null)
			{
				play.MsgBox("No purchases allowed during booth setup!");
			}
			else
			{
				int num = -1;
				for (int i = 0; i < this.mListPtichInfo.Count; i++)
				{
					if (this.mListPtichInfo[i].PtichObj != null)
					{
						if (this.mListPtichInfo[i].PtichObj.GetTypeId() == ptich_obj_id)
						{
							num = i;
							break;
						}
					}
				}
				if (num != -1)
				{
					RoleItemInfo roleItemInfo = null;
					RoleData_Eudemon roleData_Eudemon = null;
					bool flag = false;
					int i = 0;
					while (i < this.mListPtichInfo[num].mSellItemList.Count)
					{
						if (this.mListPtichInfo[num].mSellItemList[i].item_id == item_id)
						{
							flag = true;
							if (item_id >= IDManager.eudemon_start_id)
							{
								roleData_Eudemon = this.mListPtichInfo[num].play.GetEudemonSystem().FindEudemon(item_id);
								if (roleData_Eudemon == null)
								{
									return;
								}
								roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
							}
							else
							{
								roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(item_id);
							}
							if (roleItemInfo == null)
							{
								play.MsgBox("Purchase Failed!");
								return;
							}
							ushort destination = item_id >=
								IDManager.eudemon_start_id
								? (ushort)MsgItemInfo.ITEMPOSITION_EUDEMON_PACK
								: (ushort)MsgItemInfo.ITEMPOSITION_BACKPACK;
							if (!play.GetItemSystem().CanAcceptAtPosition(
								destination))
							{
								play.GetItemSystem().NotifyPackageFull(destination);
								return;
							}
							int price = this.mListPtichInfo[num].mSellItemList[i].price;
							byte sell_type = this.mListPtichInfo[num].mSellItemList[i].sell_type;
							if (sell_type == 52)
							{
								if (price > play.GetMoneyCount(MONEYTYPE.GOLD))
								{
									play.MsgBox("Purchase failed, magic stone insufficient! ");
									return;
								}
								play.ChangeMoney(MONEYTYPE.GAMEGOLD, -price);
								this.mListPtichInfo[num].play.ChangeMoney(MONEYTYPE.GAMEGOLD, price);
							}
							else
							{
								if (sell_type != 22)
								{
									return;
								}
								if (price > play.GetMoneyCount(MONEYTYPE.GOLD))
								{
									play.MsgBox("Purchase failed, not enough gold coins!");
									return;
								}
								play.ChangeMoney(MONEYTYPE.GOLD, -price);
								this.mListPtichInfo[num].play.ChangeMoney(MONEYTYPE.GOLD, price);
							}
							this.mListPtichInfo[num].mSellItemList.RemoveAt(i);
							break;
						}
						else
						{
							i++;
						}
					}
					if (!flag)
					{
						play.MsgBox("Purchase failed, the item has been taken down!");
					}
					else
					{
						if (item_id >= IDManager.eudemon_start_id)
						{
							roleItemInfo.postion = 53;
							play.GetEudemonSystem().AddTempEudemon(roleData_Eudemon);
						}
						else
						{
							roleItemInfo.postion = 50;
						}
						play.GetItemSystem().AwardItem(roleItemInfo);
						this.GetBackItem(this.mListPtichInfo[num].play, item_id);
						this.mListPtichInfo[num].play.GetItemSystem().DeleteItemByID(item_id);
						PacketOut packetOut = new PacketOut(null);
						packetOut.WriteInt16(28);
						packetOut.WriteInt16(1009);
						packetOut.WriteUInt32(item_id);
						packetOut.WriteUInt32(ptich_obj_id);
						packetOut.WriteInt32(23);
						packetOut.WriteInt16(0);
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(0);
						packetOut.WriteInt16(0);
						play.SendData(packetOut.Flush(), true);
					}
				}
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0002DD8C File Offset: 0x0002BF8C
		public void GetRemotePtich(PlayerObject play, int id = -1)
		{
			int num;
			if (id != -1 && id >= 0 && id < this.mListPtichInfo.Count)
			{
				if (this.mListPtichInfo[id].play == null)
				{
					play.MsgBox("This Stall is Closed");
					return;
				}
				num = id;
			}
			else
			{
				num = this.GetRemotePtichId(play.GetCurrentRemotePtichId());
			}
			if (num != -1)
			{
				play.SetCurrentRemotePtichId(num);
				string name = this.mListPtichInfo[num].play.GetName();
				int num2 = 13 + Coding.GetDefauleCoding().GetBytes(name).Length;
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteInt16((short)num2);
				packetOut.WriteInt16(1015);
				packetOut.WriteInt32(num + 1);
				packetOut.WriteInt16(125);
				packetOut.WriteByte(1);
				packetOut.WriteString(name);
				packetOut.WriteByte(0);
				play.SendData(packetOut.Flush(), true);
				int i = 0;
				while (i < this.mListPtichInfo[num].mSellItemList.Count)
				{
					RoleData_Eudemon roleData_Eudemon = null;
					RoleItemInfo roleItemInfo;
					if (this.mListPtichInfo[num].mSellItemList[i].item_id < IDManager.eudemon_start_id)
					{
						roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(this.mListPtichInfo[num].mSellItemList[i].item_id);
						goto IL_1EB;
					}
					roleData_Eudemon = this.mListPtichInfo[num].play.GetEudemonSystem().FindEudemon(this.mListPtichInfo[num].mSellItemList[i].item_id);
					if (roleData_Eudemon != null)
					{
						roleItemInfo = this.mListPtichInfo[num].play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
						goto IL_1EB;
					}
					IL_287:
					i++;
					continue;
					IL_1EB:
					if (roleItemInfo != null)
					{
						MsgPtichItemInfo msgPtichItemInfo = new MsgPtichItemInfo(roleItemInfo, (uint)(num + 1), this.mListPtichInfo[num].mSellItemList[i].price, this.mListPtichInfo[num].mSellItemList[i].sell_type, true);
						play.SendData(msgPtichItemInfo.GetBuffer(), true);
						if (roleItemInfo.typeid >= IDManager.eudemon_start_id)
						{
							this.mListPtichInfo[num].play.GetEudemonSystem().SendLookPtichEudemonInfo(play, roleData_Eudemon);
						}
					}
					goto IL_287;
				}
			}
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x0002E04C File Offset: 0x0002C24C
		private int GetRemotePtichId(int ptich_id)
		{
			int num = -1;
			for (int i = ptich_id; i < this.mListPtichInfo.Count; i++)
			{
				if (this.mListPtichInfo[i].play != null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				for (int i = 0; i < this.mListPtichInfo.Count; i++)
				{
					if (this.mListPtichInfo[i].play != null)
					{
						num = i;
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0002E0E0 File Offset: 0x0002C2E0
		public void BuyRemotePtichItem(PlayerObject play, uint item_id)
		{
			int currentRemotePtichId = play.GetCurrentRemotePtichId();
			if (currentRemotePtichId >= 0 && currentRemotePtichId < this.mListPtichInfo.Count)
			{
				if (this.mListPtichInfo[currentRemotePtichId].PtichObj != null)
				{
					this.BuyItem(play, this.mListPtichInfo[currentRemotePtichId].PtichObj.GetTypeId(), item_id);
				}
			}
		}

		// Token: 0x04000662 RID: 1634
		private static PtichManager mInstance = null;

		// Token: 0x04000663 RID: 1635
		private List<PtichInfo> mListPtichInfo;
	}
}
