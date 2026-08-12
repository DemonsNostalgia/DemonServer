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
	// Token: 0x0200008D RID: 141
	public class PlayerEudemon
	{
		public const int EudemonCapacity = 12;

		// Token: 0x060002A5 RID: 677 RVA: 0x0001ACD2 File Offset: 0x00018ED2
		public void SetSoulEudemon(uint id)
		{
			this.mCurEudemonSoulId = id;
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0001ACDC File Offset: 0x00018EDC
		public PlayerEudemon(PlayerObject _play)
		{
			this.play = _play;
			this.mDicEudemon = new Dictionary<uint, RoleData_Eudemon>();
			this.mTempDicEudemon = new List<RoleData_Eudemon>();
			this.mBattleObj = new List<EudemonObject>();
			this.mListObj = new List<EudemonObject>();
			this.mListRecordEudemon = new List<uint>();
		}

		// Token: 0x060002A7 RID: 679 RVA: 0x0001AD30 File Offset: 0x00018F30
		public bool IsEudemonFull()
		{
			return !this.play.GetItemSystem().CanAcceptAtPosition(
				MsgItemInfo.ITEMPOSITION_EUDEMON_PACK);
		}

		public int GetEudemonCount()
		{
			return this.play.GetItemSystem().GetEudemonCount();
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0001AD54 File Offset: 0x00018F54
		public RoleData_Eudemon FindEudemon(uint eudemon_id)
		{
			RoleData_Eudemon result;
			if (this.mDicEudemon.ContainsKey(eudemon_id))
			{
				result = this.mDicEudemon[eudemon_id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x0001AD8C File Offset: 0x00018F8C
		public void DeleteEudemon(uint eudemon_id)
		{
			if (this.mDicEudemon.ContainsKey(eudemon_id))
			{
				RoleData_Eudemon roleData_Eudemon = this.mDicEudemon[eudemon_id];
				this.mDicEudemon.Remove(eudemon_id);
				for (int i = 0; i < this.mListObj.Count; i++)
				{
					if (this.mListObj[i].GetTypeId() == eudemon_id)
					{
						this.mListObj.RemoveAt(i);
						break;
					}
				}
			}
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16(12);
			packetOut.WriteUInt16(1015);
			packetOut.WriteUInt32(eudemon_id);
			packetOut.WriteInt32(601);
			this.play.SendData(packetOut.Flush(), true);
			byte[] array = new byte[60];
			byte[] v = array;
			packetOut = new PacketOut(null);
			packetOut.WriteInt16(76);
			packetOut.WriteInt16(1040);
			packetOut.WriteInt32(0);
			packetOut.WriteInt16(5);
			packetOut.WriteInt16(1);
			packetOut.WriteUInt32(eudemon_id);
			packetOut.WriteBuff(v);
			this.play.SendData(packetOut.Flush(), true);
			MsgClearItem msgClearItem = new MsgClearItem();
			msgClearItem.id = eudemon_id;
			msgClearItem.roleid = this.play.GetTypeId();
			this.play.SendData(msgClearItem.GetBuffer(), true);
		}

		// Token: 0x060002AA RID: 682 RVA: 0x0001AEF3 File Offset: 0x000190F3
		public void AddTempEudemon(RoleData_Eudemon eudemon)
		{
			this.mTempDicEudemon.Add(eudemon);
		}

		// Token: 0x060002AB RID: 683 RVA: 0x0001AF04 File Offset: 0x00019104
		public RoleData_Eudemon FindTempEudemon(uint eudemon_typeid)
		{
			for (int i = 0; i < this.mTempDicEudemon.Count; i++)
			{
				if (this.mTempDicEudemon[i].typeid == eudemon_typeid)
				{
					return this.mTempDicEudemon[i];
				}
			}
			return null;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x0001AF60 File Offset: 0x00019160
		public void DeleteTempEudemon(uint eudemon_typeid)
		{
			for (int i = 0; i < this.mTempDicEudemon.Count; i++)
			{
				if (this.mTempDicEudemon[i].typeid == eudemon_typeid)
				{
					this.mTempDicEudemon.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x0001AFB8 File Offset: 0x000191B8
		public void AddEudemon(RoleData_Eudemon eudemon)
		{
			this.mDicEudemon[eudemon.GetTypeID()] = eudemon;
			EudemonObject eudemonObject = new EudemonObject(eudemon, this.play);
			eudemonObject.CalcAttribute();
			this.mListObj.Add(eudemonObject);
			this.SendEudemonInfo(eudemon, true, true);
		}

		// Token: 0x060002AE RID: 686 RVA: 0x0001B004 File Offset: 0x00019204
		public void AddEudemon(
			RoleItemInfo item,
			byte level = 1,
			int quality = 0,
			byte wuxing = 0,
			bool sendClientInfo = true)
		{
			uint id = GetEudemonConfigId(item.itemid);
			EudemonInfo eudemonInfo = ConfigManager.Instance().GetEudemonInfo(id);
			if (eudemonInfo == null)
			{
				Log.Instance().WriteLog("Failed to create Eudemon; Eudemon ID does not exist: " + item.id.ToString());
			}
			else if (this.mDicEudemon.ContainsKey(item.typeid))
			{
				Log.Instance().WriteLog("Failed to create Eudemon; duplicate Eudemon ID: " + item.id.ToString());
			}
			else
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(item.itemid);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog("Failed to create Eudemon; base item ID was not found: " + item.itemid.ToString());
				}
				else
				{
					RoleData_Eudemon roleData_Eudemon = new RoleData_Eudemon();
					roleData_Eudemon.id = 0U;
					roleData_Eudemon.itemid = item.id;
					ApplyInitialHatchRoll(
						roleData_Eudemon,
						eudemonInfo,
						itemTypeInfo,
						level,
						quality,
						wuxing);
					roleData_Eudemon.typeid = item.typeid;
					this.mDicEudemon[roleData_Eudemon.GetTypeID()] = roleData_Eudemon;
					EudemonObject eudemonObject = new EudemonObject(roleData_Eudemon, this.play);
					eudemonObject.CalcAttribute();
					this.mListObj.Add(eudemonObject);
					if (sendClientInfo)
					{
						this.SendEudemonInfo(roleData_Eudemon, true, true);
					}
				}
			}
		}

		public bool RerollBatchHatchEudemon(
			RoleItemInfo item,
			RoleData_Eudemon eudemon)
		{
			if (item == null || eudemon == null ||
				eudemon.itemid != item.id ||
				eudemon.GetTypeID() != item.typeid)
			{
				return false;
			}

			EudemonInfo eudemonInfo = ConfigManager.Instance().GetEudemonInfo(
				GetEudemonConfigId(item.itemid));
			ItemTypeInfo itemTypeInfo =
				ConfigManager.Instance().GetItemTypeInfo(item.itemid);
			if (eudemonInfo == null || itemTypeInfo == null)
			{
				return false;
			}
			EudemonObject eudemonObject =
				this.GetEudmeonObject(eudemon.GetTypeID());
			if (eudemonObject == null)
			{
				return false;
			}

			ApplyInitialHatchRoll(
				eudemon,
				eudemonInfo,
				itemTypeInfo,
				1,
				0,
				0);
			eudemonObject.SetEudemonInfo(eudemon);
			eudemonObject.CalcAttribute();
			return true;
		}

		private static uint GetEudemonConfigId(uint itemTypeId)
		{
			return itemTypeId - itemTypeId % 10U;
		}

		private static void ApplyInitialHatchRoll(
			RoleData_Eudemon eudemon,
			EudemonInfo config,
			ItemTypeInfo itemType,
			byte level,
			int quality,
			byte wuxing)
		{
			eudemon.phyatk_grow_rate =
				IRandom.Random(0.5f, config.atk_grow_min, 1);
			eudemon.phyatk_grow_rate_max =
				IRandom.Random(config.atk_grow_min, config.atk_grow_max, 1);
			eudemon.magicatk_grow_rate =
				IRandom.Random(0.5f, config.magicatk_grow_min, 1);
			eudemon.magicatk_grow_rate_max = IRandom.Random(
				config.magicatk_grow_min, config.magicatk_grow_max, 1);
			eudemon.life_grow_rate =
				IRandom.Random(config.life_grow_min, config.life_grow_max, 1);
			eudemon.defense_grow_rate = IRandom.Random(
				config.defense_grow_min, config.defense_grow_max, 1);
			eudemon.magicdef_grow_rate = IRandom.Random(
				config.magicdef_grow_min, config.magicdef_grow_max, 1);
			eudemon.init_life = IRandom.Random(config.life_min, config.life_max);
			eudemon.init_atk_min =
				IRandom.Random(config.atk_min_min, config.atk_min_max);
			eudemon.init_atk_max =
				IRandom.Random(config.atk_max_min, config.atk_max_max);
			eudemon.init_defense =
				IRandom.Random(config.defense_min, config.defense_max);
			eudemon.init_magicdef =
				IRandom.Random(config.magicdef_min, config.magicdef_max);
			eudemon.init_magicatk_min = IRandom.Random(
				config.magicatk_min_min, config.magicatk_min_max);
			eudemon.init_magicatk_max = IRandom.Random(
				config.magicatk_max_min, config.magicatk_max_max);
			eudemon.luck = IRandom.Random(1, 100);
			eudemon.intimacy = 150;
			eudemon.level = (short)level;
			eudemon.card = 0;
			eudemon.exp = 0;
			eudemon.quality = quality;
			eudemon.wuxing = wuxing == 0
				? IDManager.GetEudemonWuxing()
				: (int)wuxing;
			eudemon.name = itemType.name;
			eudemon.recall_count = 0;
			eudemon.bDie = false;
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0001B364 File Offset: 0x00019564
		public void DB_Load(ROLEDATE_EUDEMON data)
		{
			for (int i = 0; i < data.list_item.Count; i++)
			{
				RoleData_Eudemon roleData_Eudemon = data.list_item[i];
				RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(roleData_Eudemon.itemid);
				if (roleItemInfo != null)
				{
					roleData_Eudemon.typeid = roleItemInfo.typeid;
					this.mDicEudemon[roleData_Eudemon.GetTypeID()] = roleData_Eudemon;
					EudemonObject eudemonObject = new EudemonObject(roleData_Eudemon, this.play);
					eudemonObject.CalcAttribute();
					this.mListObj.Add(eudemonObject);
				}
			}
			this.play.GetItemSystem().Process_DieEudemon();
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0001B414 File Offset: 0x00019614
		public void DB_Save()
		{
			ROLEDATE_EUDEMON roledate_EUDEMON = new ROLEDATE_EUDEMON();
			roledate_EUDEMON.SetSaveTag();
			roledate_EUDEMON.playerid = this.play.GetBaseAttr().player_id;
			foreach (RoleData_Eudemon item in this.mDicEudemon.Values)
			{
				roledate_EUDEMON.list_item.Add(item);
			}
			DBServer.Instance().GetDBClient().SendData(roledate_EUDEMON.GetBuffer());
		}

		public void DB_Save(RoleData_Eudemon eudemon)
		{
			if (eudemon == null)
			{
				return;
			}
			ROLEDATE_EUDEMON data = new ROLEDATE_EUDEMON();
			data.SetSaveTag();
			data.playerid = this.play.GetBaseAttr().player_id;
			data.list_item.Add(eudemon);
			DBServer.Instance().GetDBClient().SendData(data.GetBuffer());
		}

		public static void PrepareDroppedEudemonForDatabaseRecreation(
			RoleData_Eudemon eudemon)
		{
			if (eudemon == null)
			{
				return;
			}

			eudemon.id = 0U;
			for (int index = eudemon.mListMagicInfo.Count - 1;
				index >= 0; index--)
			{
				MagicInfo magic = eudemon.mListMagicInfo[index];
				if (magic.id < 0)
				{
					eudemon.mListMagicInfo.RemoveAt(index);
					continue;
				}
				magic.id = 0;
				magic.ownerid = 0;
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0001B4B4 File Offset: 0x000196B4
		public static int ConvertGrowRate(float fValue)
		{
			int result = (int)(fValue * 1000f);
			string text = result.ToString();
			if (text.Length > 4)
			{
				text.Substring(0, 3);
				result = Convert.ToInt32(text);
			}
			return result;
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0001B4FC File Offset: 0x000196FC
		public void SendLookTradEudemonInfo(PlayerObject _play, RoleData_Eudemon info)
		{
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = info.GetTypeID();
			msgEudemonInfo.tag = 4;
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max, info.atk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min, info.atk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max, info.magicatk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min, info.magicatk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense, info.defense);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Magic_Defense, info.magicdef);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life, info.life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Max, info.life_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Intimacy, info.intimacy);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Level, (int)info.level);
			msgEudemonInfo.AddAttribute(EudemonAttribute.WuXing, info.wuxing);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Luck, info.luck);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Recall_Count, info.recall_count);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Card, info.card);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Exp, info.exp);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Quality, info.quality);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Atk, info.GetInitAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Magic_Atk, info.GetInitMagicAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Defense, info.GetInitDefense());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Life, info.init_life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.life_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.defense_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicDefense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicdef_grow_rate));
			MonsterInfo monsterInfo = EudemonObject.GetMonsterInfo(this.play, info.itemid);
			if (monsterInfo != null)
			{
				msgEudemonInfo.AddAttribute(EudemonAttribute.Riding, monsterInfo.eudemon_type);
			}
			_play.SendData(msgEudemonInfo.GetBuffer(), true);
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0001B70C File Offset: 0x0001990C
		public void SendLookPtichEudemonInfo(PlayerObject _play, RoleData_Eudemon info)
		{
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = info.GetTypeID();
			msgEudemonInfo.tag = 3;
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max, info.atk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min, info.atk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max, info.magicatk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min, info.magicatk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense, info.defense);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Magic_Defense, info.magicdef);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life, info.life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Max, info.life_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Intimacy, info.intimacy);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Level, (int)info.level);
			msgEudemonInfo.AddAttribute(EudemonAttribute.WuXing, info.wuxing);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Luck, info.luck);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Recall_Count, info.recall_count);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Card, info.card);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Exp, info.exp);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Quality, info.quality);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Atk, info.GetInitAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Magic_Atk, info.GetInitMagicAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Defense, info.GetInitDefense());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Life, info.init_life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.life_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.defense_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicDefense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicdef_grow_rate));
			MonsterInfo monsterInfo = EudemonObject.GetMonsterInfo(this.play, info.itemid);
			if (monsterInfo != null)
			{
				msgEudemonInfo.AddAttribute(EudemonAttribute.Riding, monsterInfo.eudemon_type);
			}
			_play.SendData(msgEudemonInfo.GetBuffer(), true);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0001B930 File Offset: 0x00019B30
		public void SendEudemonInfo(RoleData_Eudemon info, bool tag = true, bool bRank = true)
		{
			if (tag)
			{
				MsgEudemonTag msgEudemonTag = new MsgEudemonTag();
				msgEudemonTag.playerid = this.play.GetTypeId();
				msgEudemonTag.eudemonid = info.GetTypeID();
				msgEudemonTag.SetBreakTag();
				this.play.SendData(msgEudemonTag.GetBuffer(), true);
			}
			if (bRank && info.quality > 0)
			{
				byte[] v = new byte[]
				{
					12,
					0,
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
					37,
					38,
					0,
					0
				};
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteInt16(28);
				packetOut.WriteInt16(1010);
				packetOut.WriteUInt32(info.typeid);
				packetOut.WriteUInt32(this.play.GetTypeId());
				packetOut.WriteBuff(v);
				this.play.SendData(packetOut.Flush(), true);
			}
			MsgEudemonInfo msgEudemonInfo = CreateEudemonInfoMessage(info, 1);
			this.play.SendData(msgEudemonInfo.GetBuffer(), true);
		}

		// Definitive 6685 packet-2037 processing uses tags 12 and 13 for
		// Batch Hatcher appraisal replies. Unlike the ordinary eudemon update,
		// this response must not send the break-tag or ranking side packets.
		public void SendBatchHatchAppraisalInfo(
			RoleData_Eudemon info,
			int informationTag)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (informationTag != 12 && informationTag != 13)
			{
				throw new ArgumentOutOfRangeException("informationTag");
			}

			MsgEudemonInfo message =
				CreateEudemonInfoMessage(info, informationTag);
			this.play.SendData(message.GetBuffer(), true);
		}

		private MsgEudemonInfo CreateEudemonInfoMessage(
			RoleData_Eudemon info,
			int informationTag)
		{
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = info.GetTypeID();
			msgEudemonInfo.tag = informationTag;
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max, info.atk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min, info.atk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max, info.magicatk_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min, info.magicatk_min);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense, info.defense);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Magic_Defense, info.magicdef);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life, info.life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Max, info.life_max);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Intimacy, info.intimacy);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Level, (int)info.level);
			msgEudemonInfo.AddAttribute(EudemonAttribute.WuXing, info.wuxing);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Luck, info.luck);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Recall_Count, info.recall_count);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Card, info.card);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Exp, info.exp);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Quality, info.quality);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Atk, info.GetInitAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Magic_Atk, info.GetInitMagicAtk());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Defense, info.GetInitDefense());
			msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Life, info.init_life);
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.life_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.phyatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicatk_grow_rate_max));
			msgEudemonInfo.AddAttribute(EudemonAttribute.Defense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.defense_grow_rate));
			msgEudemonInfo.AddAttribute(EudemonAttribute.MagicDefense_Grow_Rate, PlayerEudemon.ConvertGrowRate(info.magicdef_grow_rate));
			MonsterInfo monsterInfo = EudemonObject.GetMonsterInfo(this.play, info.itemid);
			if (monsterInfo != null)
			{
				msgEudemonInfo.AddAttribute(EudemonAttribute.Riding, monsterInfo.eudemon_type);
			}
			return msgEudemonInfo;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0001BC10 File Offset: 0x00019E10
		public void SendAllEudemonInfo()
		{
			foreach (RoleData_Eudemon roleData_Eudemon in this.mDicEudemon.Values)
			{
				if (!this.play.GetItemSystem().IsEudemonInBag(
					roleData_Eudemon.GetTypeID()))
				{
					continue;
				}
				this.SendEudemonInfo(roleData_Eudemon, true, true);
				EudemonObject eudmeonObject = this.GetEudmeonObject(roleData_Eudemon.GetTypeID());
				if (eudmeonObject != null)
				{
					eudmeonObject.SendMagicInfo();
				}
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0001BC94 File Offset: 0x00019E94
		public EudemonObject GetEudmeonObject(uint eudemon_id)
		{
			for (int i = 0; i < this.mListObj.Count; i++)
			{
				if (this.mListObj[i].GetTypeId() == eudemon_id)
				{
					return this.mListObj[i];
				}
			}
			return null;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0001BCF0 File Offset: 0x00019EF0
		public EudemonObject GetBattleEudemonSystem(byte nIndex)
		{
			EudemonObject result;
			if ((int)nIndex >= this.mBattleObj.Count)
			{
				result = null;
			}
			else
			{
				result = this.mBattleObj[(int)nIndex];
			}
			return result;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0001BD24 File Offset: 0x00019F24
		public EudemonObject GetBattleEudemon(uint eudemon_id)
		{
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				if (this.mBattleObj[i].GetEudemonInfo().GetTypeID() == eudemon_id)
				{
					return this.mBattleObj[i];
				}
			}
			return null;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0001BD84 File Offset: 0x00019F84
		public uint GetEudemonTypeID(uint itemid)
		{
			foreach (RoleData_Eudemon roleData_Eudemon in this.mDicEudemon.Values)
			{
				if (roleData_Eudemon.itemid == itemid)
				{
					return roleData_Eudemon.GetTypeID();
				}
			}
			return 0U;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0001BE00 File Offset: 0x0001A000
		public void Move(MsgMoveInfo moveinfo)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(moveinfo.id);
			if (battleEudemon != null)
			{
				battleEudemon.Move(moveinfo);
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0001BE30 File Offset: 0x0001A030
		public void ExitGame()
		{
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				if (this.mBattleObj[i].GetState() == EUDEMONSTATE.BATTLE)
				{
					this.play.GetGameMap().RemoveObj(this.mBattleObj[i]);
				}
			}
			this.mBattleObj.Clear();
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0001BEA0 File Offset: 0x0001A0A0
		public void Eudemon_BreakUpAll()
		{
			int i = this.mBattleObj.Count;
			while (i > 0)
			{
				i--;
				if (this.mBattleObj[i].GetState() == EUDEMONSTATE.FIT)
				{
					this.Eudemon_BreakUp(this.mBattleObj[i].GetTypeId());
				}
			}
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0001BF00 File Offset: 0x0001A100
		public void Eudemon_ReCallAll(bool isRecord = false)
		{
			if (isRecord)
			{
				this.mListRecordEudemon.Clear();
			}
			int i = this.mBattleObj.Count;
			while (i > 0)
			{
				i--;
				if (this.mBattleObj[i].GetState() == EUDEMONSTATE.BATTLE)
				{
					if (isRecord)
					{
						this.mListRecordEudemon.Add(this.mBattleObj[i].GetTypeId());
					}
					this.Eudemon_ReCall(this.mBattleObj[i].GetTypeId());
				}
			}
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0001BF9C File Offset: 0x0001A19C
		public void Eudemon_BattleAll()
		{
			for (int i = 0; i < this.mListRecordEudemon.Count; i++)
			{
				this.play.GetEudemonSystem().Eudemon_Battle(this.mListRecordEudemon[i]);
			}
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0001BFE4 File Offset: 0x0001A1E4
		public void Eudemon_ReCall(uint eudemon_id)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(eudemon_id);
			if (battleEudemon != null)
			{
				this.mBattleObj.Remove(battleEudemon);
				battleEudemon.GetGameMap().RemoveObj(battleEudemon);
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x0001C024 File Offset: 0x0001A224
		public void Eudemon_Fit(uint eudemon_id)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(eudemon_id);
			if (battleEudemon != null)
			{
				battleEudemon.SetState(EUDEMONSTATE.FIT);
				byte[] array = new byte[16];
				array[0] = 32;
				byte[] v = array;
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteUInt16(28);
				packetOut.WriteUInt16(1009);
				packetOut.WriteUInt32(battleEudemon.GetTypeId());
				packetOut.WriteUInt32(battleEudemon.GetTypeId());
				packetOut.WriteBuff(v);
				this.play.BroadcastBuffer(packetOut.Flush(), true);
				array = new byte[16];
				array[12] = 73;
				array[13] = 37;
				byte[] v2 = array;
				packetOut = new PacketOut(null);
				packetOut.WriteUInt16(28);
				packetOut.WriteUInt16(1010);
				packetOut.WriteInt32(Environment.TickCount);
				packetOut.WriteUInt32(battleEudemon.GetTypeId());
				packetOut.WriteBuff(v2);
				this.play.BroadcastBuffer(packetOut.Flush(), true);
				packetOut = new PacketOut(null);
				packetOut.WriteUInt16(32);
				packetOut.WriteUInt16(2037);
				packetOut.WriteUInt32(1U);
				packetOut.WriteUInt32(battleEudemon.GetTypeId());
				packetOut.WriteInt32(2);
				packetOut.WriteInt32(6);
				packetOut.WriteInt32(battleEudemon.GetAttr().life);
				packetOut.WriteInt32(7);
				packetOut.WriteInt32(battleEudemon.GetAttr().life_max);
				this.play.SendData(packetOut.Flush(), true);
				array = new byte[20];
				array[0] = byte.MaxValue;
				array[4] = 35;
				byte[] v3 = array;
				packetOut = new PacketOut(null);
				packetOut.WriteUInt16(28);
				packetOut.WriteUInt16(1009);
				packetOut.WriteUInt32(battleEudemon.GetTypeId());
				packetOut.WriteBuff(v3);
				this.play.BroadcastBuffer(packetOut.Flush(), true);
			}
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x0001C21C File Offset: 0x0001A41C
		public void Eudemon_BreakUp(uint eudemon_id)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(eudemon_id);
			if (battleEudemon != null)
			{
				if (battleEudemon.GetState() == EUDEMONSTATE.FIT)
				{
					this.mBattleObj.Remove(battleEudemon);
					byte[] array = new byte[20];
					array[0] = byte.MaxValue;
					array[4] = 36;
					byte[] v = array;
					PacketOut packetOut = new PacketOut(null);
					packetOut.WriteUInt16(28);
					packetOut.WriteUInt16(1009);
					packetOut.WriteUInt32(battleEudemon.GetTypeId());
					packetOut.WriteBuff(v);
					this.play.SendData(packetOut.Flush(), true);
					byte[] v2 = new byte[]
					{
						2,
						0,
						0,
						0,
						6,
						0,
						0,
						0,
						228,
						0,
						0,
						0,
						7,
						0,
						0,
						0,
						228,
						0,
						0,
						0
					};
					packetOut = new PacketOut(null);
					packetOut.WriteUInt16(32);
					packetOut.WriteUInt16(2037);
					packetOut.WriteUInt32(1U);
					packetOut.WriteUInt32(battleEudemon.GetTypeId());
					packetOut.WriteBuff(v2);
					this.play.SendData(packetOut.Flush(), true);
					packetOut = new PacketOut(null);
					packetOut.WriteUInt16(16);
					packetOut.WriteUInt16(1012);
					packetOut.WriteUInt32(this.play.GetTypeId());
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					this.play.SendData(packetOut.Flush(), true);
					battleEudemon.SetState(EUDEMONSTATE.NROMAL);
				}
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0001C390 File Offset: 0x0001A590
		public void Eudemon_Evolution(uint eudemon_id)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(eudemon_id);
			if (battleEudemon != null)
			{
				if (battleEudemon.GetState() == EUDEMONSTATE.BATTLE)
				{
					RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(battleEudemon.GetEudemonInfo().itemid);
					RoleData_Eudemon roleData_Eudemon = this.FindEudemon(eudemon_id);
					if (roleItemInfo != null && roleData_Eudemon != null)
					{
						string text = roleItemInfo.itemid.ToString();
						int num = Convert.ToInt32(text.Substring(text.Length - 1));
						if (num < 2)
						{
							ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid + 1U);
							if (itemTypeInfo == null)
							{
								this.play.MsgBox("Evolution Failed.1");
								uint num2 = roleItemInfo.itemid + 1U;
								Log.Instance().WriteLog("Evolution failed; item ID was not found: " + num2.ToString());
							}
							else
							{
								MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(itemTypeInfo.monster_type);
								if (monsterInfo == null)
								{
									this.play.MsgBox("Evolution Failed 2.");
									Log.Instance().WriteLog("Evolution failed; monster type was not found: " + itemTypeInfo.monster_type.ToString());
								}
								else
								{
									if (num == 0)
									{
										if (battleEudemon.GetEudemonInfo().level < 20)
										{
											return;
										}
										if (battleEudemon.GetEudemonInfo().quality == 0)
										{
											EudemonInfo eudemonInfo = ConfigManager.Instance().GetEudemonInfo(roleItemInfo.itemid);
											int quality = 1000;
											if (eudemonInfo != null)
											{
												quality = IRandom.Random(eudemonInfo.quality_min, eudemonInfo.qulity_max);
											}
											roleData_Eudemon.quality = quality;
										}
										roleItemInfo.itemid += 1U;
									}
									else
									{
										if (battleEudemon.GetEudemonInfo().level < 40)
										{
											return;
										}
										roleItemInfo.itemid += 1U;
										roleData_Eudemon.card = IDManager.CreateEudemonCard();
									}
									battleEudemon.SetEudemonInfo(roleData_Eudemon);
									this.play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
									battleEudemon.SetMosterInfo(monsterInfo);
									PacketOut packetOut = new PacketOut(null);
									packetOut.WriteInt16(24);
									packetOut.WriteInt16(2035);
									packetOut.WriteUInt32(battleEudemon.GetTypeId());
									packetOut.WriteUInt32(battleEudemon.GetMonsterInfo().lookface);
									packetOut.WriteInt32(2);
									packetOut.WriteInt16(battleEudemon.GetCurrentX());
									packetOut.WriteInt16(battleEudemon.GetCurrentY());
									packetOut.WriteUInt32(battleEudemon.GetTypeId());
									battleEudemon.BrocatBuffer(packetOut.Flush());
									this.SendEudemonInfo(battleEudemon.GetEudemonInfo(), false, true);
									byte[] v = new byte[]
									{
										12,
										0,
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
										37,
										38,
										0,
										0
									};
									packetOut = new PacketOut(null);
									packetOut.WriteInt16(28);
									packetOut.WriteInt16(1010);
									packetOut.WriteUInt32(battleEudemon.GetTypeId());
									packetOut.WriteUInt32(this.play.GetTypeId());
									packetOut.WriteBuff(v);
									battleEudemon.BrocatBuffer(packetOut.Flush());
									byte[] array = new byte[20];
									array[0] = 1;
									array[4] = 28;
									byte[] v2 = array;
									packetOut = new PacketOut(null);
									packetOut.WriteInt16(28);
									packetOut.WriteInt16(1009);
									packetOut.WriteUInt32(battleEudemon.GetTypeId());
									packetOut.WriteBuff(v2);
									battleEudemon.BrocatBuffer(packetOut.Flush());
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0001C74C File Offset: 0x0001A94C
		public void Eudemon_Attack(MsgAttackInfo info)
		{
			EudemonObject battleEudemon = this.GetBattleEudemon(info.roleId);
			if (battleEudemon != null)
			{
				uint tag = info.tag;
				if (tag != 2U)
				{
					if (tag == 21U)
					{
						battleEudemon.MagicAttack(info);
					}
				}
				else
				{
					battleEudemon.Attack(info);
				}
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0001C79C File Offset: 0x0001A99C
		public void Eudemon_Battle(uint eudemon_id)
		{
			if (!this.play.GetItemSystem().IsEudemonInBag(eudemon_id))
			{
				return;
			}
			if (this.mBattleObj.Count < 3)
			{
				if (this.GetBattleEudemon(eudemon_id) == null)
				{
					if (this.mDicEudemon.ContainsKey(eudemon_id))
					{
						EudemonObject eudemonObject = null;
						for (int i = 0; i < this.mListObj.Count; i++)
						{
							if (this.mListObj[i].GetTypeId() == eudemon_id)
							{
								eudemonObject = this.mListObj[i];
								break;
							}
						}
						if (eudemonObject != null)
						{
							if (eudemonObject.GetEudemonInfo().level <= (short)this.play.GetBaseAttr().level || eudemonObject.GetEudemonInfo().level - (short)this.play.GetBaseAttr().level <= 9)
							{
								if (eudemonObject.IsRiding())
								{
									this.play.TakeOffMount(eudemonObject.GetTypeId());
								}
								eudemonObject.Battle();
								this.mBattleObj.Add(eudemonObject);
							}
						}
					}
				}
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x0001C8C8 File Offset: 0x0001AAC8
		public void FlyPlay()
		{
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.BATTLE)
				{
					eudemonObject.FlyPlay();
				}
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0001C918 File Offset: 0x0001AB18
		public void Eudemon_Alive(MonsterObject taget)
		{
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.BATTLE && eudemonObject.GetAttr().bDie)
				{
					eudemonObject.GetAttr().life += (int)((double)eudemonObject.GetAttr().life_max * 0.1);
					new MsgEudemonInfo
					{
						id = eudemonObject.GetTypeId()
					}.AddAttribute(EudemonAttribute.Life, eudemonObject.GetAttr().life);
					PacketOut packetOut = new PacketOut(null);
					if (eudemonObject.GetAttr().life >= eudemonObject.GetAttr().life_max)
					{
						eudemonObject.GetAttr().life = eudemonObject.GetAttr().life_max;
						packetOut.WriteInt16(24);
						packetOut.WriteInt16(2037);
						packetOut.WriteInt32(1);
						packetOut.WriteUInt32(eudemonObject.GetTypeId());
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(6);
						packetOut.WriteInt32(eudemonObject.GetAttr().life);
						eudemonObject.BrocatBuffer(packetOut.Flush());
						packetOut = new PacketOut(null);
						packetOut.WriteInt16(20);
						packetOut.WriteInt16(1017);
						packetOut.WriteUInt32(eudemonObject.GetTypeId());
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(0);
						packetOut.WriteInt32(eudemonObject.GetAttr().life);
						eudemonObject.BrocatBuffer(packetOut.Flush());
						packetOut = new PacketOut(null);
						packetOut.WriteInt16(20);
						packetOut.WriteInt16(1017);
						packetOut.WriteUInt32(eudemonObject.GetTypeId());
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(26);
						packetOut.WriteInt32(4);
						eudemonObject.BrocatBuffer(packetOut.Flush());
						packetOut = new PacketOut(null);
						packetOut.WriteInt16(20);
						packetOut.WriteInt16(1017);
						packetOut.WriteUInt32(eudemonObject.GetTypeId());
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(26);
						packetOut.WriteInt32(0);
						eudemonObject.BrocatBuffer(packetOut.Flush());
						eudemonObject.GetAttr().bDie = false;
						eudemonObject.SendEudemonInfo(null);
					}
					else
					{
						packetOut.WriteInt16(40);
						packetOut.WriteInt16(1022);
						packetOut.WriteInt32(Environment.TickCount);
						packetOut.WriteUInt32(eudemonObject.GetTypeId());
						packetOut.WriteUInt32(taget.GetTypeId());
						packetOut.WriteInt16(eudemonObject.GetCurrentX());
						packetOut.WriteInt16(eudemonObject.GetCurrentY());
						packetOut.WriteInt32(32);
						packetOut.WriteInt16(4);
						packetOut.WriteInt32(eudemonObject.GetAttr().life);
						packetOut.WriteInt32(0);
						packetOut.WriteInt32(0);
						packetOut.WriteInt16(0);
						eudemonObject.BrocatBuffer(packetOut.Flush());
					}
					break;
				}
			}
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0001CC14 File Offset: 0x0001AE14
		public bool Eudemon_Injured(BaseObject obj, uint value, MsgAttackInfo info)
		{
			int i = this.mBattleObj.Count;
			bool result = false;
			while (i > 0)
			{
				i--;
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.FIT)
				{
					eudemonObject.Injured(obj, value, info);
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0001CC78 File Offset: 0x0001AE78
		public EudemonObject GetInjuredEudemon()
		{
			int i = this.mBattleObj.Count;
			while (i > 0)
			{
				i--;
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.FIT)
				{
					return eudemonObject;
				}
			}
			return null;
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0001CCCC File Offset: 0x0001AECC
		public void Eudemon_DeleteMagic(uint eudemon_id, ushort magicid)
		{
			EudemonObject eudmeonObject = this.GetEudmeonObject(eudemon_id);
			if (eudmeonObject != null)
			{
				eudmeonObject.DeleteMagicInfo(magicid);
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0001CCF8 File Offset: 0x0001AEF8
		public void AddExp(int nExp)
		{
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (!eudemonObject.IsDie())
				{
					eudemonObject.AddExp(nExp);
				}
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0001CD48 File Offset: 0x0001AF48
		public void Eudemon_Soul(uint _id)
		{
			EudemonObject eudmeonObject = this.GetEudmeonObject(_id);
			EudemonObject eudmeonObject2 = this.GetEudmeonObject(this.mCurEudemonSoulId);
			if (eudmeonObject2 == null)
			{
				this.play.LeftNotice("Fantasy transformation failed, main fantasy beast not found!");
			}
			else if (eudmeonObject == null)
			{
				this.play.LeftNotice("Fantasy transformation failed, no secondary illusion found!");
			}
			else if (eudmeonObject2.GetState() != EUDEMONSTATE.BATTLE)
			{
				this.play.LeftNotice("Fantasy Beast Transformation Failed, Main Beast Must Embark!");
			}
			else if (eudmeonObject.GetState() != EUDEMONSTATE.BATTLE)
			{
				this.play.LeftNotice("Transmutation failed, the sub-phantom must go to war!");
			}
			else
			{
				RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(eudmeonObject2.GetEudemonInfo().itemid);
				if (roleItemInfo != null)
				{
					EudemonSoulInfo eudemonSoulInfo = ConfigManager.Instance().GetEudemonSoulInfo(eudmeonObject2.GetEudemonInfo().quality / 100);
					if (eudemonSoulInfo == null)
					{
						this.play.LeftNotice("Transformation failed, error!001");
					}
					else if ((int)eudmeonObject2.GetEudemonInfo().level < eudemonSoulInfo.level)
					{
						this.play.MsgBox("Main Fantasy Beast Level Requirement:" + eudemonSoulInfo.level.ToString() + "Lvl:");
					}
					else if ((int)eudmeonObject.GetEudemonInfo().level < eudemonSoulInfo.level)
					{
						this.play.MsgBox("Sub Fantasy Beast Level Requirement:" + eudemonSoulInfo.fu_level.ToString() + "Lvl:");
					}
					else if (eudmeonObject.GetEudemonInfo().quality < eudemonSoulInfo.fu_star)
					{
						this.play.MsgBox("Sub-Demon Star Level Requirement: Ultimate" + eudemonSoulInfo.fu_star.ToString());
					}
					else
					{
						RoleData_Eudemon roleData_Eudemon = this.play.GetEudemonSystem().FindEudemon(eudmeonObject2.GetEudemonInfo().GetTypeID());
						if (roleData_Eudemon == null)
						{
							this.play.MsgBox("Transformation failed! aaa");
						}
						else
						{
							string text = roleItemInfo.itemid.ToString();
							text = text.Substring(0, text.Length - 1) + "0";
							roleItemInfo.itemid = Convert.ToUInt32(text);
							if (EudemonObject.GetMonsterInfo(this.play, roleItemInfo.id) == null)
							{
								this.play.MsgBox("Transmutation Failed");
							}
							else
							{
								this.play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
								eudmeonObject2.SetMosterInfo(EudemonObject.GetMonsterInfo(this.play, roleItemInfo.id));
								eudmeonObject2.SendEudemonInfo(null);
								this.Eudemon_ReCall(eudmeonObject.GetTypeId());
								this.play.GetItemSystem().DeleteItemByID(eudmeonObject.GetTypeId());
								roleData_Eudemon.level = 1;
								roleData_Eudemon.recall_count++;
								MonsterInfo monsterInfo = eudmeonObject2.GetMonsterInfo();
								roleData_Eudemon.quality += IRandom.Random(eudemonSoulInfo.add_min, eudemonSoulInfo.add_max);
								if (eudemonSoulInfo.add_main > 0f)
								{
									switch (monsterInfo.eudemon_type)
									{
									case 1:
									case 5:
										roleData_Eudemon.phyatk_grow_rate += eudemonSoulInfo.add_main;
										roleData_Eudemon.phyatk_grow_rate_max += eudemonSoulInfo.add_main;
										roleData_Eudemon.defense_grow_rate += eudemonSoulInfo.add_main;
										break;
									case 2:
									case 4:
										roleData_Eudemon.magicatk_grow_rate += eudemonSoulInfo.add_main;
										roleData_Eudemon.magicatk_grow_rate_max += eudemonSoulInfo.add_main;
										roleData_Eudemon.magicdef_grow_rate += eudemonSoulInfo.add_main;
										break;
									}
								}
								if (eudemonSoulInfo.add_fu > 0f)
								{
									switch (monsterInfo.eudemon_type)
									{
									case 1:
									case 5:
										roleData_Eudemon.magicatk_grow_rate += eudemonSoulInfo.add_fu;
										roleData_Eudemon.magicatk_grow_rate_max += eudemonSoulInfo.add_fu;
										roleData_Eudemon.magicdef_grow_rate += eudemonSoulInfo.add_fu;
										break;
									case 2:
									case 4:
										roleData_Eudemon.phyatk_grow_rate += eudemonSoulInfo.add_fu;
										roleData_Eudemon.phyatk_grow_rate_max += eudemonSoulInfo.add_fu;
										roleData_Eudemon.defense_grow_rate += eudemonSoulInfo.add_fu;
										roleData_Eudemon.phyatk_grow_rate += eudemonSoulInfo.add_fu;
										break;
									}
									roleData_Eudemon.life_grow_rate += eudemonSoulInfo.add_fu;
								}
								if (eudemonSoulInfo.add_init > 0)
								{
									roleData_Eudemon.init_life += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_atk_min += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_atk_max += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_magicatk_min += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_magicatk_max += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_defense += eudemonSoulInfo.add_init;
									roleData_Eudemon.init_magicdef += eudemonSoulInfo.add_init;
								}
								eudmeonObject2.SetEudemonInfo(roleData_Eudemon);
								MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
								PacketOut packetOut = new PacketOut(null);
								packetOut.WriteInt16(28);
								packetOut.WriteInt16(1010);
								packetOut.WriteUInt32(eudmeonObject2.GetTypeId());
								packetOut.WriteUInt32(this.play.GetTypeId());
								packetOut.WriteInt16(21);
								packetOut.WriteInt16(69);
								packetOut.WriteInt32(0);
								packetOut.WriteInt32(0);
								packetOut.WriteInt32(9765);
								this.play.SendData(packetOut.Flush(), true);
								this.SendEudemonInfo(eudmeonObject2.GetEudemonInfo(), true, true);
								packetOut = new PacketOut(null);
								packetOut.WriteInt16(28);
								packetOut.WriteInt16(1010);
								packetOut.WriteInt32(Environment.TickCount);
								packetOut.WriteUInt32(eudmeonObject2.GetTypeId());
								packetOut.WriteInt32(50);
								packetOut.WriteInt32(0);
								packetOut.WriteInt32(1);
								packetOut.WriteInt32(9742);
								this.play.SendData(packetOut.Flush(), true);
							}
						}
					}
				}
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0001D3F0 File Offset: 0x0001B5F0
		public int CalcFightSoul()
		{
			int num = 0;
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetEudemonInfo().quality > 0)
				{
					num += 20;
					num += eudemonObject.GetEudemonInfo().quality / 100;
					if (eudemonObject.IsDie())
					{
						num -= 2;
					}
				}
			}
			return num;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0001D474 File Offset: 0x0001B674
		public int GetFitEudemonMinAtk()
		{
			int num = 0;
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.FIT)
				{
					num += eudemonObject.GetMinAck();
				}
			}
			return num;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0001D4D4 File Offset: 0x0001B6D4
		public int GetFitEudemonMaxAtk()
		{
			int num = 0;
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetState() == EUDEMONSTATE.FIT)
				{
					num += eudemonObject.GetMaxAck();
				}
			}
			return num;
		}

		// Token: 0x060002CF RID: 719 RVA: 0x0001D534 File Offset: 0x0001B734
		public void TakeMount(uint eudemon_id)
		{
			EudemonObject eudemonObject;
			for (int i = 0; i < this.mBattleObj.Count; i++)
			{
				eudemonObject = this.mBattleObj[i];
				if (eudemonObject.GetTypeId() == eudemon_id)
				{
					if (eudemonObject.GetState() == EUDEMONSTATE.BATTLE)
					{
						this.Eudemon_ReCall(eudemon_id);
					}
					else if (eudemonObject.GetState() == EUDEMONSTATE.FIT)
					{
						this.Eudemon_BreakUp(eudemon_id);
					}
					break;
				}
			}
			eudemonObject = this.GetEudmeonObject(eudemon_id);
			if (eudemonObject != null)
			{
				eudemonObject.SetRiding(true);
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001D5D4 File Offset: 0x0001B7D4
		public void TakeOffMount(uint eudemon_id)
		{
			EudemonObject eudmeonObject = this.GetEudmeonObject(eudemon_id);
			if (eudmeonObject != null)
			{
				eudmeonObject.SetRiding(false);
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001D600 File Offset: 0x0001B800
		public void Process_DieEudemon()
		{
			for (int i = 0; i < this.mListObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mListObj[i];
				RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(eudemonObject.GetAttr().itemid);
				if (roleItemInfo == null)
				{
					this.DeleteEudemon(eudemonObject.GetTypeId());
				}
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001D670 File Offset: 0x0001B870
		public void SendLookEudemonInfo(PlayerObject target)
		{
			uint typeId = this.play.GetTypeId();
			PacketOut packetOut = new PacketOut(null);
			for (int i = 0; i < this.mListObj.Count; i++)
			{
				EudemonObject eudemonObject = this.mListObj[i];
				uint itemid = eudemonObject.GetEudemonInfo().itemid;
				RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(itemid);
				RoleData_Eudemon eudemonInfo = eudemonObject.GetEudemonInfo();
				if (roleItemInfo != null)
				{
					packetOut = new PacketOut(null);
					int num = 84 + Coding.GetDefauleCoding().GetBytes(roleItemInfo.forgename).Length;
					packetOut.WriteInt16((short)num);
					packetOut.WriteInt16(1008);
					packetOut.WriteUInt32(typeId);
					packetOut.WriteUInt32(eudemonInfo.GetTypeID());
					packetOut.WriteUInt32(roleItemInfo.itemid);
					packetOut.WriteInt32(0);
					packetOut.WriteByte(7);
					packetOut.WriteByte(0);
					packetOut.WriteByte(53);
					byte[] v = new byte[]
					{
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
						0,
						0,
						1
					};
					packetOut.WriteBuff(v);
					packetOut.WriteString(roleItemInfo.forgename);
					packetOut.WriteByte(0);
					packetOut.WriteByte(0);
					packetOut.WriteByte(0);
					target.SendData(packetOut.Flush(), true);
					MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
					msgEudemonInfo.id = eudemonInfo.GetTypeID();
					msgEudemonInfo.tag = 2;
					msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max, eudemonInfo.atk_max);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min, eudemonInfo.atk_min);
					msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max, eudemonInfo.magicatk_max);
					msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min, eudemonInfo.magicatk_min);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Defense, eudemonInfo.defense);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Magic_Defense, eudemonInfo.magicdef);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Life, eudemonInfo.life);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Max, eudemonInfo.life_max);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Intimacy, eudemonInfo.intimacy);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Level, (int)eudemonInfo.level);
					msgEudemonInfo.AddAttribute(EudemonAttribute.WuXing, eudemonInfo.wuxing);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Luck, eudemonInfo.luck);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Recall_Count, eudemonInfo.recall_count);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Card, eudemonInfo.card);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Exp, eudemonInfo.exp);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Quality, eudemonInfo.quality);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Atk, eudemonInfo.GetInitAtk());
					msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Magic_Atk, eudemonInfo.GetInitMagicAtk());
					msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Defense, eudemonInfo.GetInitDefense());
					msgEudemonInfo.AddAttribute(EudemonAttribute.Init_Life, eudemonInfo.init_life);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.life_grow_rate));
					msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.phyatk_grow_rate));
					msgEudemonInfo.AddAttribute(EudemonAttribute.Atk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.phyatk_grow_rate_max));
					msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Min_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.magicatk_grow_rate));
					msgEudemonInfo.AddAttribute(EudemonAttribute.MagicAtk_Max_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.magicatk_grow_rate_max));
					msgEudemonInfo.AddAttribute(EudemonAttribute.Defense_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.defense_grow_rate));
					msgEudemonInfo.AddAttribute(EudemonAttribute.MagicDefense_Grow_Rate, PlayerEudemon.ConvertGrowRate(eudemonInfo.magicdef_grow_rate));
					MonsterInfo monsterInfo = EudemonObject.GetMonsterInfo(this.play, eudemonInfo.itemid);
					if (monsterInfo != null)
					{
						msgEudemonInfo.AddAttribute(EudemonAttribute.Riding, monsterInfo.eudemon_type);
					}
					target.SendData(msgEudemonInfo.GetBuffer(), true);
				}
			}
		}

		// Token: 0x040005F8 RID: 1528
		public const int MAX_EUDEMON_COUNT = 12;

		// Token: 0x040005F9 RID: 1529
		private PlayerObject play;

		// Token: 0x040005FA RID: 1530
		private Dictionary<uint, RoleData_Eudemon> mDicEudemon;

		// Token: 0x040005FB RID: 1531
		private List<RoleData_Eudemon> mTempDicEudemon;

		// Token: 0x040005FC RID: 1532
		private List<EudemonObject> mBattleObj;

		// Token: 0x040005FD RID: 1533
		private List<uint> mListRecordEudemon;

		// Token: 0x040005FE RID: 1534
		private List<EudemonObject> mListObj;

		// Token: 0x040005FF RID: 1535
		private uint mCurEudemonSoulId;
	}
}
