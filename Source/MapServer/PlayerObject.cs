using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000093 RID: 147
	public class PlayerObject : BaseObject
	{
		// Token: 0x06000341 RID: 833 RVA: 0x00024F78 File Offset: 0x00023178
		public bool IsGhost()
		{
			return this.m_bGhost;
		}

		// Token: 0x06000342 RID: 834 RVA: 0x00024F90 File Offset: 0x00023190
		public uint GetMountID()
		{
			return this.mnMountID;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x00024FA8 File Offset: 0x000231A8
		public int GetFightSoul()
		{
			return this.mnFightSoul;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x00024FC0 File Offset: 0x000231C0
		public PlayerItem GetItemSystem()
		{
			return this.mItemSystem;
		}

		public PlayerWardrobe GetWardrobeSystem()
		{
			return this.mWardrobeSystem;
		}

		// Token: 0x06000345 RID: 837 RVA: 0x00024FD8 File Offset: 0x000231D8
		public PlayerMagic GetMagicSystem()
		{
			return this.mMagicSystem;
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00024FF0 File Offset: 0x000231F0
		public PlayerEudemon GetEudemonSystem()
		{
			return this.mEudemonSystem;
		}

		public void SetBatchHatchAppraisalAllowance(int count)
		{
			this.mBatchHatchAppraisalAllowance = count < 0 ? 0 : count;
		}

		public bool ConsumeBatchHatchAppraisalAllowance()
		{
			if (this.mBatchHatchAppraisalAllowance <= 0)
			{
				return false;
			}
			this.mBatchHatchAppraisalAllowance--;
			return true;
		}

		// Token: 0x06000347 RID: 839 RVA: 0x00025008 File Offset: 0x00023208
		public PlayerFight GetFightSystem()
		{
			return this.mFightSystem;
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00025020 File Offset: 0x00023220
		public PlayerFriend GetFriendSystem()
		{
			return this.mFriendSystem;
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00025038 File Offset: 0x00023238
		public PlayerTrad GetTradSystem()
		{
			return this.mTradSystem;
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00025050 File Offset: 0x00023250
		public PlayerTimer GetTimerSystem()
		{
			return this.mTimerSystem;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00025068 File Offset: 0x00023268
		public PlayerLegion GetLegionSystem()
		{
			return this.mLegionSystem;
		}

		public PlayerFamily GetFamilySystem()
		{
			return this.mFamilySystem;
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00025080 File Offset: 0x00023280
		public PlayerPK GetPKSystem()
		{
			return this.mPKSystem;
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00025098 File Offset: 0x00023298
		public uint GetFace()
		{
			return this.GetBaseAttr().lookface;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000250B5 File Offset: 0x000232B5
		public void SetFace(uint _face)
		{
			this.face = _face;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000250C0 File Offset: 0x000232C0
		public byte GetSex()
		{
			return this.sex;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x000250D8 File Offset: 0x000232D8
		public void SetTeam(Team _team)
		{
			this.mTeam = _team;
		}

		// Token: 0x06000351 RID: 849 RVA: 0x000250E4 File Offset: 0x000232E4
		public Team GetTeam()
		{
			return this.mTeam;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x000250FC File Offset: 0x000232FC
		public void SetTaskID(uint _id)
		{
			this.mTaskID = _id;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00025108 File Offset: 0x00023308
		public uint GetTaskID()
		{
			return this.mTaskID;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x00025120 File Offset: 0x00023320
		public GUANGJUELEVEL GetGuanJue()
		{
			return this.mGuanJue;
		}

		// Token: 0x06000355 RID: 853 RVA: 0x00025138 File Offset: 0x00023338
		public void SetGuanJue(GUANGJUELEVEL info)
		{
			this.mGuanJue = info;
		}

		// Token: 0x06000356 RID: 854 RVA: 0x00025142 File Offset: 0x00023342
		public void SetCurrentAction(uint action)
		{
			this.mnCurAction = action;
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0002514C File Offset: 0x0002334C
		public uint GetCurrentAction()
		{
			return this.mnCurAction;
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00025164 File Offset: 0x00023364
		public void SetZhaoHuanWuHuanObj(BaseObject obj)
		{
			this.mZhaoHuanWuHuanObj = obj;
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0002516E File Offset: 0x0002336E
		public void SetCurrentRandom(int nValue)
		{
			this.mnCurrentRandom = nValue;
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00025178 File Offset: 0x00023378
		public int GetCurrentRandom()
		{
			return this.mnCurrentRandom;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x00025190 File Offset: 0x00023390
		public void SetCurrentPtichID(int PtichId)
		{
			this.mPtichId = PtichId;
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0002519C File Offset: 0x0002339C
		public int GetCurrentPtichID()
		{
			return this.mPtichId;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000251B4 File Offset: 0x000233B4
		public void SetCurrentRemotePtichId(int RemotePtichId)
		{
			this.mRemotePtichId = RemotePtichId;
		}

		// Token: 0x0600035E RID: 862 RVA: 0x000251C0 File Offset: 0x000233C0
		public int GetCurrentRemotePtichId()
		{
			return this.mRemotePtichId;
		}

		// Token: 0x0600035F RID: 863 RVA: 0x000251D8 File Offset: 0x000233D8
		public void SetUseItemEudemonId(uint eudemon_id)
		{
			this.mUseItemEudemonId = eudemon_id;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000251E4 File Offset: 0x000233E4
		public uint GetUseItemEudemonId()
		{
			return this.mUseItemEudemonId;
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000251FC File Offset: 0x000233FC
		public void SetTransmitIng(bool v)
		{
			this.mbTransmit = v;
			if (this.mTransmitTimeOut == null)
			{
				this.mTransmitTimeOut = new TimeOut();
			}
			this.mTransmitTimeOut.SetInterval(1);
			this.mTransmitTimeOut.Update();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00025248 File Offset: 0x00023448
		public void CalcSex()
		{
			int num = (int)(this.GetBaseAttr().lookface % 2U);
			if (num == 0)
			{
				this.sex = 2;
			}
			else
			{
				this.sex = 1;
			}
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00025280 File Offset: 0x00023480
		public byte GetJob()
		{
			return this.GetBaseAttr().profession;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0002529D File Offset: 0x0002349D
		public void SetJob(byte _job)
		{
			this.GetBaseAttr().profession = _job;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000252AC File Offset: 0x000234AC
		public void SetDancing(short nDancingId)
		{
			this.mnDancingId = nDancingId;
			if (this.mnDancingId == 0)
			{
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteInt16(16);
				packetOut.WriteInt16(1012);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(0);
				base.SendData(packetOut.Flush(), true);
				packetOut = new PacketOut(null);
				packetOut.WriteInt16(12);
				packetOut.WriteInt16(1015);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteInt32(340);
				base.SendData(packetOut.Flush(), true);
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00025360 File Offset: 0x00023560
		public bool IsDancing()
		{
			return this.mnDancingId != 0;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00025380 File Offset: 0x00023580
		public int GetLookFace()
		{
			int num = (int)this.GetBaseAttr().lookface;
			int result;
			if (this.IsGhost())
			{
				if (this.GetSex() == 1)
				{
					num = (int)(this.GetBaseAttr().lookface + 980000000U);
					result = num;
				}
				else
				{
					num = (int)(this.GetBaseAttr().lookface + 990000000U);
					result = num;
				}
			}
			else
			{
				result = num;
			}
			return result;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x000253EC File Offset: 0x000235EC
		public bool IsExit()
		{
			return this.bIsExit;
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00025404 File Offset: 0x00023604
		public void SetExit(bool _isExit)
		{
			this.bIsExit = _isExit;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00025410 File Offset: 0x00023610
		public Dictionary<byte, uint> GetMenuLink()
		{
			return this.mMenuLink;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00025428 File Offset: 0x00023628
		public void ClearScriptMenuLink()
		{
			this.mMenuLink.Clear();
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00025437 File Offset: 0x00023637
		public void SetCurrentNpcInfo(NPCInfo info)
		{
			this.mNpcInfo = info;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00025444 File Offset: 0x00023644
		public NPCInfo GetCurrentNpcInfo()
		{
			return this.mNpcInfo;
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0002545C File Offset: 0x0002365C
		public PlayerAttribute GetBaseAttr()
		{
			return this.mAttribute;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00025474 File Offset: 0x00023674
		public PlayerObject()
		{
			this.mAttribute = new PlayerAttribute();
			this.type = 2;
			this.typeid = IDManager.CreateTypeId(this.type);
			this.Name = "";
			this.lastattacktime = Environment.TickCount;
			this.mMenuLink = new Dictionary<byte, uint>();
			this.mItemSystem = new PlayerItem(this);
			this.mWardrobeSystem = new PlayerWardrobe(this);
			this.mMagicSystem = new PlayerMagic(this);
			this.mTimerSystem = new PlayerTimer(this);
			this.mFightSystem = new PlayerFight(this);
			this.mEudemonSystem = new PlayerEudemon(this);
			this.mFriendSystem = new PlayerFriend(this);
			this.mTradSystem = new PlayerTrad(this);
			this.mLegionSystem = new PlayerLegion(this);
			this.mFamilySystem = new PlayerFamily(this);
			this.mPKSystem = new PlayerPK(this);
			this.mListHotKey = new List<HotkeyInfo>();
			this.mZhaoHuanWuHuanObj = null;
			this.mTeam = null;
			this.m_bGhost = false;
			this.mnGhostTick = Environment.TickCount;
			this.mTarget = null;
			this.mSaveTime = new TimeOut();
			this.mSaveTime.SetInterval(600);
			this.mnCurrentRandom = 0;
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00025604 File Offset: 0x00023804
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.GetGameID() != base.GetGameID())
				{
					int distance = 15;
					if (this.mVisibleList.ContainsKey(baseObject.GetGameID()))
					{
						if (!base.GetPoint().CheckVisualDistance(baseObject.GetCurrentX(), baseObject.GetCurrentY(), distance))
						{
							this.mVisibleList.Remove(baseObject.GetGameID());
							baseObject.GetVisibleList().Remove(base.GetGameID());
							if (baseObject.type == 2)
							{
								this.ClearThis(baseObject as PlayerObject);
								(baseObject as PlayerObject).ClearThis(this);
							}
						}
					}
					else if (base.GetPoint().CheckVisualDistance(baseObject.GetCurrentX(), baseObject.GetCurrentY(), distance))
					{
						if (baseObject.type == 3)
						{
							if ((baseObject as MonsterObject).IsClear())
							{
								continue;
							}
						}
						base.AddVisibleObject(baseObject, false);
					}
				}
			}
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00025774 File Offset: 0x00023974
		public bool Move(MsgMoveInfo move)
		{
			bool result;
			if (!this.GetMagicSystem().CheckMoveSpeed())
			{
				this.ScroolRandom(base.GetCurrentX(), base.GetCurrentY());
				result = false;
			}
			else
			{
				byte b = (byte)(move.dir % 8);
				base.SetDir(b);
				short num = base.GetCurrentX();
				short num2 = base.GetCurrentY();
				num += DIR._DELTA_X[(int)b];
				num2 += DIR._DELTA_Y[(int)b];
				if (!this.mGameMap.CanMove(num, num2))
				{
					num = base.GetCurrentX();
					num2 = base.GetCurrentY();
					if (!this.mGameMap.CanMove(num, num2))
					{
						num = (short)this.mGameMap.GetMapInfo().recallx;
						num2 = (short)this.mGameMap.GetMapInfo().recally;
					}
					this.ScroolRandom(num, num2);
					result = false;
				}
				else
				{
					bool flag = false;
					if (move.ucMode >= 20 && move.ucMode <= 27 && this.GetBaseAttr().sp > 0)
					{
						num += DIR._DELTA_X[(int)(move.ucMode - 20)];
						num2 += DIR._DELTA_Y[(int)(move.ucMode - 20)];
						flag = true;
						if (!this.mGameMap.CanMove(num, num2))
						{
							num = base.GetCurrentX();
							num2 = base.GetCurrentY();
							if (!this.mGameMap.CanMove(num, num2))
							{
								num = (short)this.mGameMap.GetMapInfo().recallx;
								num2 = (short)this.mGameMap.GetMapInfo().recally;
							}
							this.ScroolRandom(num, num2);
							return false;
						}
					}
					uint mapid = 0U;
					short x = 0;
					short y = 0;
					if (ConfigManager.Instance().CheckMapGate(base.GetGameMap().GetMapInfo().id, num, num2, ref mapid, ref x, ref y))
					{
						this.ChangeMap(mapid, x, y);
						result = false;
					}
					else
					{
						if (this.GetBaseAttr().sp <= 0)
						{
							move.ucMode = 0;
						}
						this.SetPoint(num, num2);
						GameStruct.Action action = new GameStruct.Action(2, null);
						if (flag)
						{
							action.AddObject(move.ucMode);
						}
						this.GetFightSystem().SetAutoAttackTarget(null);
						this.PushAction(action);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x000259CC File Offset: 0x00023BCC
		public override bool Run()
		{
			bool result;
			if (base.GetGameSession() == null)
			{
				if (base.GetVisibleList().Count > 0)
				{
					MsgClearObjectInfo msgClearObjectInfo = new MsgClearObjectInfo();
					msgClearObjectInfo.id = base.GetTypeId();
					base.GetGameMap().BroadcastBuffer(this, msgClearObjectInfo.GetBuffer());
				}
				result = false;
			}
			else
			{
				base.Run();
				if (base.IsLock())
				{
					if (!base.CheckLockTime())
					{
						base.UnLock(true);
					}
				}
				this.GetTimerSystem().Run();
				this.GetFightSystem().Run();
				this.GetPKSystem().Run();
				if (this.IsDie() && this.m_bGhost && this.mnGhostTick != -1)
				{
					if (Environment.TickCount - this.mnGhostTick >= 3000)
					{
						this.ChangeAttribute(UserAttribute.STATUS, 6, true);
						this.ChangeAttribute(UserAttribute.LOOKFACE, this.GetLookFace(), true);
						this.mnGhostTick = -1;
					}
				}
				if (this.IsDie() && !base.IsLock() && this.mTarget != null && !this.m_bGhost)
				{
					GameStruct.Action action = new GameStruct.Action(4, null);
					action.AddObject(this.mTarget);
					this.PushAction(action);
				}
				if (this.mSaveTime.ToNextTime())
				{
					UserEngine.Instance().AddSaveRole(this);
				}
				if (this.mbTransmit && this.mTransmitTimeOut.ToNextTime())
				{
					this.GetEudemonSystem().Eudemon_BattleAll();
					base.GetGameMap().SendWeatherInfo(this);
					this.SetTransmitIng(false);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00025B88 File Offset: 0x00023D88
		public void SendNpcInfo(BaseObject obj)
		{
			if (base.GetGameSession() != null)
			{
				MsgNpcInfo msgNpcInfo = new MsgNpcInfo();
				msgNpcInfo.Create(null, this.session.GetGamePackKeyEx());
				int lookface = (obj as NpcObject).mInfo.lookface;
				msgNpcInfo.Init(obj.GetID(), obj.GetCurrentX(), obj.GetCurrentY(), lookface);
				base.SendData(msgNpcInfo.GetBuffer(), false);
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00025BFC File Offset: 0x00023DFC
		public void SendDropItemInfo(BaseObject obj)
		{
			DropItemObject dropItemObject = obj as DropItemObject;
			MsgDropItem msgDropItem = new MsgDropItem();
			msgDropItem.Create(null, base.GetGamePackKeyEx());
			msgDropItem.SetRefreshTag();
			msgDropItem.id = dropItemObject.GetGameID();
			msgDropItem.typeid = dropItemObject.GetTypeId();
			msgDropItem.x = dropItemObject.GetCurrentX();
			msgDropItem.y = dropItemObject.GetCurrentY();
			base.SendData(msgDropItem.GetBuffer(), false);
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00025C6C File Offset: 0x00023E6C
		public void SendRoleInfo(PlayerObject play)
		{
			MsgRoleInfo msgRoleInfo = new MsgRoleInfo();
			msgRoleInfo.Create(null, base.GetGamePackKeyEx());
			msgRoleInfo.role_id = play.GetTypeId();
			msgRoleInfo.x = play.GetCurrentX();
			msgRoleInfo.y = play.GetCurrentY();
			msgRoleInfo.armor_id = play.GetItemSystem().GetArmorLook();
			msgRoleInfo.wepon_id = play.GetItemSystem().GetWeaponLook();
			msgRoleInfo.face_sex = (uint)play.GetLookFace();
			msgRoleInfo.face_sex1 = play.GetBaseAttr().lookface;
			msgRoleInfo.dir = play.GetDir();
			msgRoleInfo.action = play.GetCurrentAction();
			msgRoleInfo.level = play.GetBaseAttr().level;
			msgRoleInfo.job = play.GetBaseAttr().profession;
			msgRoleInfo.guanjue = (byte)play.GetGuanJue();
			msgRoleInfo.hair_id = play.GetBaseAttr().hair;
			msgRoleInfo.str.Add(play.GetName());
			msgRoleInfo.rid_id = play.GetMountID();
			if (play.GetLegionSystem().IsHaveLegion() && play.GetLegionSystem().GetLegion() != null)
			{
				msgRoleInfo.legion_id = play.GetLegionSystem().GetLegion().GetBaseInfo().id;
				msgRoleInfo.legion_title = play.GetLegionSystem().GetLegion().GetBaseInfo().title;
				msgRoleInfo.legion_place = play.GetLegionSystem().GetPlace();
				msgRoleInfo.legion_id1 = msgRoleInfo.legion_id;
			}
			if (play.GetFamilySystem().IsHaveFamily())
			{
				msgRoleInfo.family_id = play.GetFamilySystem().GetFamily().Id;
				msgRoleInfo.family_rank = play.GetFamilySystem().GetRank();
			}
			base.SendData(msgRoleInfo.GetBuffer(), false);
			play.GetTimerSystem().SendState(this);
			if (msgRoleInfo.legion_id > 0U)
			{
				MsgLegionName msgLegionName = new MsgLegionName();
				msgLegionName.Create(null, base.GetGamePackKeyEx());
				msgLegionName.legion_id = msgRoleInfo.legion_id;
				msgLegionName.legion_name = play.GetLegionSystem().GetLegion().GetBaseInfo().name;
				base.SendData(msgLegionName.GetBuffer(), false);
			}
			base.AddVisibleObject(play, true);
			if (play.IsDie() && play.IsGhost())
			{
				play.ChangeAttribute(UserAttribute.LOOKFACE, play.GetLookFace(), true);
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00025E68 File Offset: 0x00024068
		public void SendRoleMoveInfo(BaseObject obj, byte runValue, RefreshObject _refobj)
		{
			if (obj.type == 2)
			{
				PlayerObject playerObject = obj as PlayerObject;
				if (!_refobj.bRefreshTag)
				{
					this.SendRoleInfo(playerObject);
					playerObject.SendRoleInfo(this);
					_refobj.bRefreshTag = true;
				}
				else
				{
					obj.SendData(new MsgMoveInfo
					{
						id = base.GetTypeId(),
						x = base.GetCurrentX(),
						y = base.GetCurrentY(),
						dir = base.GetDir(),
						ucMode = runValue
					}.GetBuffer(), true);
				}
			}
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00025EFC File Offset: 0x000240FC
		public void SendMonsterInfo(BaseObject obj)
		{
			if (base.GetGameSession() != null)
			{
				MsgMonsterInfo msgMonsterInfo = new MsgMonsterInfo();
				msgMonsterInfo.Create(null, base.GetGamePackKeyEx());
				MonsterObject monsterObject = obj as MonsterObject;
				msgMonsterInfo.id = monsterObject.GetTypeId();
				msgMonsterInfo.typeid = monsterObject.GetBasicAttribute().id;
				msgMonsterInfo.lookface = monsterObject.GetBasicAttribute().lookface;
				msgMonsterInfo.x = monsterObject.GetCurrentX();
				msgMonsterInfo.y = monsterObject.GetCurrentY();
				msgMonsterInfo.level = monsterObject.GetBasicAttribute().level;
				msgMonsterInfo.maxhp = monsterObject.GetAttribute().life_max;
				msgMonsterInfo.hp = monsterObject.GetAttribute().life;
				msgMonsterInfo.dir = (int)monsterObject.GetDir();
				if (obj.GetType().FullName == "MapServer.AnShaXieLongObject")
				{
					msgMonsterInfo.param = (int)(obj as AnShaXieLongObject).mPlay.GetTypeId();
				}
				base.SendData(msgMonsterInfo.GetBuffer(), false);
				if (!obj.GetVisibleList().ContainsKey(base.GetGameID()))
				{
					obj.AddVisibleObject(this, false);
				}
				if (!base.GetVisibleList().ContainsKey(obj.GetGameID()))
				{
					base.AddVisibleObject(obj, true);
				}
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00026080 File Offset: 0x00024280
		public void ProcessNetMsg(ushort tag, byte[] netdata)
		{
			ushort num = tag;
			if (num <= 1032)
			{
				if (num <= 1015)
				{
					if (num == 1004)
					{
						MsgTalkInfo msgTalkInfo = new MsgTalkInfo();
						msgTalkInfo.Create(netdata, null);
						string chatError;
						if (!msgTalkInfo.TryValidatePlayerMessage(
							this.GetName(), out chatError))
						{
							Log.Instance().WriteLog(
								"Rejected malformed player talk packet from role " +
								this.GetTypeId().ToString() + ": " + chatError + ".");
							return;
						}
						string talkText = msgTalkInfo.GetTalkText();
						if (talkText.Length > 0)
						{
							char commandPrefix = talkText[0];
							if ((commandPrefix == '\\' || commandPrefix == '/') && this.IsGM())
							{
								GMCommand.ExecuteGMCommand(talkText, this);
								return;
							}
							if (commandPrefix == '\\')
							{
								GMCommand.ExecuteNormalCommand(talkText, this);
								return;
							}
						}
						num = msgTalkInfo.unTxtAttribute;
						if (num == 2130)
						{
							FamilyManager.Instance().UpdateAnnouncement(
								this, talkText, netdata);
						}
						else if (num == 2111)
						{
							LegionManager.Instance().UpdateNotice(
								this, talkText, netdata);
						}
						else if (num == 2006)
						{
							FamilyInfo family = this.GetFamilySystem().GetFamily();
							if (family == null)
							{
								this.ChatNotice(
									"You must belong to a family to use family chat.");
							}
							else
							{
								UserEngine.Instance().BroadcastFamilyPayload(
									this, family, netdata);
							}
						}
						else if (num == MsgTalkInfo._TXTATR_FRIEND)
						{
							this.GetFriendSystem().BroadcastChat(netdata);
						}
						else if (num == 2004)
						{
							Legion legion = this.GetLegionSystem().GetLegion();
							if (legion == null)
							{
								this.ChatNotice(
									"You must belong to a legion to use legion chat.");
							}
							else
							{
								UserEngine.Instance().BroadcastLegionPayload(
									this, legion, netdata);
							}
						}
						else if (num == MsgTalkInfo._TXTATR_TEAM)
						{
							Team team = this.GetTeam();
							if (team == null)
							{
								this.ChatNotice(
									"You must belong to a team to use team chat.");
							}
							else
							{
								team.BroadcastChat(this, netdata);
							}
						}
						else if (num != MsgTalkInfo._TXTATR_PRIVATE)
						{
							if (num != MsgTalkInfo._TXTATR_TALK &&
								num != MsgTalkInfo._TXTATR_GHOST)
							{
								if (num == 2113)
								{
									string talkTargetText = msgTalkInfo.GetTalkTargetText();
									PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToName(talkTargetText);
									if (playerObject != null)
									{
										string talkText2 = msgTalkInfo.GetTalkText();
										if (talkText2 != null)
										{
											if (!(talkText2 == "a"))
											{
												if (!(talkText2 == "b"))
												{
													if (!(talkText2 == "c"))
													{
													}
												}
												else
												{
													this.GetTradSystem().SetTradTarget(0U);
													playerObject.GetTradSystem().SetTradTarget(0U);
													playerObject.LeftNotice("The target has refused to trade with you.");
												}
											}
										}
									}
								}
							}
							else
							{
								PacketOut packetOut = new PacketOut(null);
								packetOut.WriteUInt16((ushort)(netdata.Length + 2));
								packetOut.WriteBuff(netdata);
								this.BroadcastBuffer(packetOut.Flush(), false);
							}
						}
						else
						{
							string talkTargetText = msgTalkInfo.GetTalkTargetText();
							PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToName(talkTargetText);
							if (playerObject != null)
							{
								PacketOut packetOut = new PacketOut(playerObject.GetGamePackKeyEx());
								packetOut.WriteUInt16((ushort)(netdata.Length + 2));
								packetOut.WriteBuff(netdata);
								playerObject.SendData(packetOut.Flush(), false);
							}
							else
							{
								this.ChatNotice("The target is not online.");
							}
						}
						return;
					}
					switch (num)
					{
					case 1015:
					{
						NameQueryPacket nameQuery;
						string error;
						if (!MapPacketCodec.TryReadNameQuery(
							netdata, out nameQuery, out error))
						{
							Log.Instance().WriteLog(
								"Rejected malformed name packet 1015: " + error);
							return;
						}
						this.HandleNameQuery(nameQuery);
						return;
					}
					case 1009:
					{
						#if DEBUG
						Log.Instance().WriteLog(
							"Item operation 1009 received for " + this.GetName() +
							", bytes=" + BitConverter.ToString(netdata) +
							", locked=" + base.IsLock().ToString() + ".");
						#endif
						if (base.IsLock())
						{
							return;
						}
						MsgOperateItem msgOperateItem = new MsgOperateItem();
						msgOperateItem.Create(netdata, null);
						#if DEBUG
						Log.Instance().WriteLog(
							"Item operation 1009 decoded: shop=" +
							msgOperateItem.id.ToString() + ", data=" +
							msgOperateItem.dwData.ToString() + ", action=" +
							msgOperateItem.usAction.ToString() + ".");
						#endif
						num = msgOperateItem.usAction;
						if (num <= 41)
						{
							if (num <= 28)
							{
								switch (num)
								{
								case 1:
								{
									uint num2 = msgOperateItem.id;
									if (num2 != 1207U)
									{
										switch (num2)
										{
										case PlayerWardrobe.HairApplyShop:
											this.GetWardrobeSystem().ApplyHair(
												msgOperateItem.dwData,
												msgOperateItem.amount,
												msgOperateItem.param1);
											break;
										case PlayerWardrobe.HairUnlockShop:
											this.GetWardrobeSystem().UnlockHair(
												msgOperateItem.dwData,
												msgOperateItem.amount,
												msgOperateItem.param1);
											break;
										case 1997U:
											this.GetItemSystem().ChangeHair(msgOperateItem.dwData);
											break;
										case 1998U:
											this.GetItemSystem().ChangeLookFace(msgOperateItem.dwData);
											break;
										default:
											this.GetItemSystem().BuyItem(msgOperateItem.id, msgOperateItem.dwData);
											break;
										}
									}
									else
									{
										this.GetItemSystem().BuyGameShopItem(msgOperateItem.dwData, (int)msgOperateItem.amount);
									}
									break;
								}
								case 2:
									this.GetItemSystem().SellItem(msgOperateItem.id, msgOperateItem.dwData);
									break;
								case 3:
									this.GetItemSystem().DropItemBag(
										msgOperateItem.id,
										(short)(msgOperateItem.dwData >> 16),
										(short)(msgOperateItem.dwData & 65535U));
									break;
								case 4:
									this.GetItemSystem().UseItem(msgOperateItem.id, msgOperateItem.dwData, (short)msgOperateItem.amount, (short)msgOperateItem.param1);
									break;
								case 5:
								case 7:
								case 8:
								case 9:
								case 12:
								case 13:
									break;
								case 6:
									this.GetItemSystem().UnEquip(msgOperateItem.id, msgOperateItem.dwData, true);
									break;
								case 10:
									this.GetItemSystem().SaveStrongMoney((int)msgOperateItem.dwData);
									break;
								case 11:
									this.GetItemSystem().GiveStrongMoney((int)msgOperateItem.dwData);
									break;
								case 14:
									this.GetItemSystem().RepairEquip((uint)msgOperateItem.param1, msgOperateItem.id);
									break;
								default:
									switch (num)
									{
									case 22:
										PtichManager.Instance().SellItem(this, msgOperateItem.id, 22, (int)msgOperateItem.dwData);
										break;
									case 23:
										PtichManager.Instance().GetBackItem(this, msgOperateItem.id);
										break;
									case 24:
										PtichManager.Instance().BuyItem(this, msgOperateItem.dwData, msgOperateItem.id);
										break;
									case 28:
										this.GetEudemonSystem().Eudemon_Evolution(msgOperateItem.id);
										break;
									}
									break;
								}
							}
							else
							{
								switch (num)
								{
								case 32:
									this.GetEudemonSystem().Eudemon_ReCall(msgOperateItem.id);
									break;
								case 33:
								case 34:
									break;
								case 35:
									this.GetEudemonSystem().Eudemon_Fit(msgOperateItem.id);
									break;
								case 36:
									this.GetEudemonSystem().Eudemon_BreakUp(msgOperateItem.id);
									break;
								default:
									if (num == 41)
									{
										this.GetEudemonSystem().Eudemon_DeleteMagic(msgOperateItem.id, msgOperateItem.amount);
									}
									break;
								}
							}
						}
						else if (num <= 59)
						{
							switch (num)
							{
							case 50:
							{
								int num3 = BaseFunc.MakeLong((int)msgOperateItem.amount, (int)msgOperateItem.param1);
								if ((long)num3 == (long)((ulong)base.GetTypeId()))
								{
									this.LeftNotice("Experience Balls are not allowed to be used on characters");
								}
								break;
							}
							case 51:
								break;
							case 52:
								PtichManager.Instance().SellItem(this, msgOperateItem.id, 52, (int)msgOperateItem.dwData);
								break;
							default:
								if (num == 59)
								{
									EquipOperation.Instance().OpenGem(this, msgOperateItem.id, (uint)BaseFunc.MakeLong((int)msgOperateItem.amount, (int)msgOperateItem.param1));
								}
								break;
							}
						}
						else if (num != 63)
						{
							if (num != 101)
							{
								switch (num)
								{
								case 110:
								{
									EudemonObject eudmeonObject = this.GetEudemonSystem().GetEudmeonObject(msgOperateItem.id);
									if (eudmeonObject != null)
									{
										RoleItemInfo roleItemInfo = this.GetItemSystem().FindItem(eudmeonObject.GetEudemonInfo().itemid);
										if (roleItemInfo != null)
										{
											this.TakeMount(eudmeonObject.GetTypeId(), roleItemInfo.itemid);
										}
									}
									break;
								}
								case 111:
									this.TakeOffMount(msgOperateItem.id);
									break;
								case 114:
									PtichManager.Instance().GetRemotePtich(this, (int)(msgOperateItem.dwData - 1U));
									break;
								case 115:
									PtichManager.Instance().GetRemotePtich(this, -1);
									break;
								case 116:
									PtichManager.Instance().BuyRemotePtichItem(this, msgOperateItem.id);
									break;
								}
							}
							else if (this.GetMoneyCount(MONEYTYPE.GAMEGOLD) >= 19)
							{
								uint eudemon_id = (uint)BaseFunc.MakeLong((int)msgOperateItem.amount, (int)msgOperateItem.param1);
								EudemonObject battleEudemon = this.GetEudemonSystem().GetBattleEudemon(eudemon_id);
								if (battleEudemon != null)
								{
									if (battleEudemon.GetAttr().level + 50 > (short)(this.GetBaseAttr().level + 8))
									{
										battleEudemon.ChangeAttribute(EudemonAttribute.Level, (int)((short)(this.GetBaseAttr().level + 8) - battleEudemon.GetAttr().level), true);
									}
									else if (battleEudemon.GetAttr().level + 50 > 255)
									{
										battleEudemon.ChangeAttribute(EudemonAttribute.Level, (int)(255 - battleEudemon.GetAttr().level), true);
									}
									else
									{
										battleEudemon.ChangeAttribute(EudemonAttribute.Level, 50, true);
									}
									battleEudemon.CalcAttribute();
									this.ChangeMoney(MONEYTYPE.GAMEGOLD, -19);
								}
							}
						}
						else
						{
							int num3 = BaseFunc.MakeLong((int)msgOperateItem.amount, (int)msgOperateItem.param1);
							if ((long)num3 == (long)((ulong)base.GetTypeId()))
							{
								this.LeftNotice("Experience Balls are not allowed to be used on characters");
							}
							else
							{
								RoleData_Eudemon roleData_Eudemon = this.GetEudemonSystem().FindEudemon((uint)num3);
								EudemonObject eudmeonObject2 = this.GetEudemonSystem().GetEudmeonObject((uint)num3);
								if (eudmeonObject2 != null && roleData_Eudemon != null)
								{
									roleData_Eudemon.level = 0;
									eudmeonObject2.ChangeAttribute(EudemonAttribute.Level, (int)(this.GetBaseAttr().level + 3), false);
									this.GetItemSystem().DeleteItemByID(msgOperateItem.id);
								}
							}
						}
						return;
					}
					case 1010:
					{
						bool flag = false;
						MsgChangePkMode msgChangePkMode = new MsgChangePkMode();
						msgChangePkMode.Create(netdata, base.GetGamePackKeyEx());
						int num4 = msgChangePkMode.tag;
						if (num4 <= 9622)
						{
							if (num4 <= 9560)
							{
								if (num4 <= 9552)
								{
									switch (num4)
									{
									case 9528:
										break;
									case 9529:
										break;
									case 9530:
										if (!base.IsLock())
										{
											uint num5 = BitConverter.ToUInt32(netdata, 18);
											this.SetCurrentAction(num5);
											msgChangePkMode.SetKey(null);
											this.BroadcastBuffer(msgChangePkMode.GetBuffer(), false);
											if (this.GetTimerSystem().QueryStatus(120) != null)
											{
												foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
												{
													BaseObject baseObject = refreshObject.obj;
													if (baseObject.type == 2)
													{
														(baseObject as PlayerObject).PlayAction(num5);
													}
												}
											}
										}
										break;
									default:
										if (num4 == 9552)
										{
											if (this.IsGhost() && Environment.TickCount - this.mnGhostTick >= 20)
											{
												this.Alive(false);
											}
										}
										break;
									}
								}
								else if (num4 != 9556)
								{
									if (num4 == 9560)
									{
										this.GetFriendSystem().GetFriendInfo(msgChangePkMode.value);
									}
								}
								else if (msgChangePkMode.value >= 0 && msgChangePkMode.value <= 5)
								{
									this.SetPkMode((byte)msgChangePkMode.value);
									flag = true;
								}
							}
							else if (num4 <= 9573)
							{
								if (num4 != 9570)
								{
									if (num4 == 9573)
									{
										PtichManager.Instance().ShutPtich(this, true);
									}
								}
								else if (this.GetCurrentPtichID() != -1)
								{
									PtichManager.Instance().AddPlayPtich(this.GetCurrentPtichID(), this);
								}
							}
							else if (num4 != 9576)
							{
								if (num4 == 9622)
								{
									this.GetTimerSystem().XPFull((short)msgChangePkMode.value);
								}
							}
							else
							{
								PlayerObject playerObject2 = UserEngine.Instance().FindPlayerObjectToTypeID((uint)msgChangePkMode.value);
								if (playerObject2 != null)
								{
									playerObject2.GetItemSystem().SendLookRoleInfo(this);
								}
							}
						}
						else if (num4 <= 9756)
						{
							if (num4 <= 9707)
							{
								switch (num4)
								{
								case 9630:
								{
									PacketOut packetOut = new PacketOut(null);
									packetOut.WriteInt16(28);
									packetOut.WriteInt16(1010);
									packetOut.WriteInt32(Environment.TickCount);
									packetOut.WriteUInt32(base.GetTypeId());
									packetOut.WriteInt32(0);
									packetOut.WriteInt32(0);
									packetOut.WriteInt32(0);
									packetOut.WriteInt32(9630);
									base.SendData(packetOut.Flush(), true);
									break;
								}
								case 9631:
									break;
								case 9632:
									this.GetTimerSystem().DeleteStatus(101);
									this.GetTimerSystem().DeleteStatus(47);
									if (this.GetTimerSystem().QueryStatus(104) != null)
									{
										this.GetTimerSystem().DeleteStatus(104);
									}
									break;
								default:
									if (num4 == 9707)
									{
										uint time = (uint)msgChangePkMode.time;
										PtichManager.Instance().LookPtich(this, time);
									}
									break;
								}
							}
							else
							{
								switch (num4)
								{
								case 9742:
									this.GetEudemonSystem().Eudemon_Soul((uint)msgChangePkMode.value);
									break;
								case 9743:
								{
									PlayerObject playerObject2 = UserEngine.Instance().FindPlayerObjectToTypeID((uint)msgChangePkMode.value);
									if (playerObject2 != null)
									{
										playerObject2.GetEudemonSystem().SendLookEudemonInfo(this);
									}
									break;
								}
								default:
									if (num4 == 9756)
									{
										EudemonObject eudmeonObject = this.GetEudemonSystem().GetEudmeonObject((uint)msgChangePkMode.time);
										if (eudmeonObject != null)
										{
											this.GetEudemonSystem().SetSoulEudemon((uint)msgChangePkMode.time);
											EudemonSoulInfo eudemonSoulInfo = ConfigManager.Instance().GetEudemonSoulInfo(eudmeonObject.GetEudemonInfo().quality / 100);
											if (eudemonSoulInfo == null)
											{
												this.MsgBox("Failed to retrieve transmutation information!!");
												Log.Instance().WriteLog(string.Concat(new string[]
												{
													"Error retrieving transformation information: Name:",
													this.GetName(),
													" Quality:",
													eudmeonObject.GetEudemonInfo().quality.ToString(),
													"Item ID:",
													eudmeonObject.GetEudemonInfo().itemid.ToString()
												}));
											}
											else
											{
												PacketOut packetOut = new PacketOut(null);
												packetOut.WriteInt16(32);
												packetOut.WriteInt16(2036);
												packetOut.WriteInt16(73);
												packetOut.WriteInt16(6);
												packetOut.WriteInt32(eudemonSoulInfo.level);
												packetOut.WriteInt32(eudemonSoulInfo.fu_star);
												packetOut.WriteInt32(0);
												packetOut.WriteInt32(0);
												packetOut.WriteInt32(0);
												packetOut.WriteInt32(eudmeonObject.GetEudemonInfo().quality);
												packetOut.WriteInt32(1);
												this.SendData(packetOut.Flush(), true);
											}
										}
									}
									break;
								}
							}
						}
						else if (num4 <= 9788)
						{
							if (num4 != 9764)
							{
								if (num4 == 9788)
								{
									this.GetEudemonSystem().Eudemon_Battle((uint)msgChangePkMode.value);
								}
							}
							else
							{
								byte[] v = new byte[]
								{
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1,
									1
								};
								PacketOut packetOut = new PacketOut(null);
								packetOut.WriteInt16(36);
								packetOut.WriteInt16(1126);
								packetOut.WriteUInt32((uint)msgChangePkMode.value);
								packetOut.WriteUInt32(base.GetTypeId());
								packetOut.WriteInt16(0);
								packetOut.WriteBuff(v);
								base.SendData(packetOut.Flush(), true);
							}
						}
						else if (num4 != 9855)
						{
							if (num4 == 9894)
							{
								short num6 = BaseFunc.LoWord(msgChangePkMode.type);
								short num7 = BaseFunc.HiWord(msgChangePkMode.type);
								if (base.GetGameMap().CanMove(num6, num7))
								{
									PacketOut packetOut = new PacketOut(null);
									packetOut.WriteInt16(28);
									packetOut.WriteInt16(1010);
									packetOut.WriteInt32(1);
									packetOut.WriteUInt32(base.GetTypeId());
									packetOut.WriteInt16(num6);
									packetOut.WriteInt16(num7);
									packetOut.WriteInt32(0);
									packetOut.WriteUInt32(base.GetGameMap().GetMapInfo().id);
									packetOut.WriteInt32(9894);
									base.SendData(packetOut.Flush(), true);
								}
							}
						}
						if (flag)
						{
							base.SendData(msgChangePkMode.GetBuffer(), false);
						}
						return;
					}
					default:
						break;
					}
				}
				else
				{
					if (num == 1019)
					{
						if (this.IsDie())
						{
							this.LeftNotice(
								"You cannot manage relations while dead.");
							return;
						}
						MsgFriendInfo msgFriendInfo = new MsgFriendInfo();
						msgFriendInfo.Create(netdata, null);
						byte b = msgFriendInfo.type;
						switch (b)
						{
						case 10:
							this.GetFriendSystem().RequestAddFriend(msgFriendInfo);
							break;
						case 11:
							this.GetFriendSystem().AcceptFriend(msgFriendInfo.playerid);
							break;
						case 12:
						case 13:
							break;
						case 14:
							this.GetFriendSystem().DeleteFriend(msgFriendInfo.playerid, true);
							break;
						case 18:
							this.GetFriendSystem().DeleteEnemy(msgFriendInfo.playerid);
							break;
						default:
							if (b == 21)
							{
								this.GetFriendSystem().RefuseFriend(msgFriendInfo.playerid);
							}
							break;
						}
						return;
					}
					if (num == 1023)
					{
						TeamActionPacket teamAction;
						string error;
						if (!MapPacketCodec.TryReadTeamAction(
							netdata, out teamAction, out error))
						{
							Log.Instance().WriteLog(
								"Rejected malformed team packet 1023: " + error);
							return;
						}
						TeamManager.Instance().HandlePacket(this, teamAction);
						return;
					}
					if (num == 1028)
					{
						MonsterFaceStatusPacket statusPacket;
						string error;
						if (!MapPacketCodec.TryReadMonsterFaceStatus(
							netdata, out statusPacket, out error))
						{
							Log.Instance().WriteLog(
								"Rejected malformed monster-face status packet 1028: " +
								error);
							return;
						}
						if (statusPacket.RoleId != base.GetTypeId())
						{
							Log.Instance().WriteLog(
								"Rejected monster-face status packet 1028 for role " +
								statusPacket.RoleId.ToString() + "; active role is " +
								base.GetTypeId().ToString());
							return;
						}
						if (statusPacket.Action != 0 ||
							statusPacket.StatusId != 1000000001U)
						{
							Log.Instance().WriteLog(
								"Unsupported monster-face status packet 1028 variant: " +
								"action=" + statusPacket.Action.ToString() +
								", status=" + statusPacket.StatusId.ToString());
							return;
						}

						byte[] response = MapPacketCodec.CreateMonsterFaceStatusResponse(
							null,
							statusPacket.RoleId,
							0,
							0,
							statusPacket.QueryMode,
							statusPacket.StatusId,
							0);
						base.SendData(response, true);
						Log.Instance().WriteLog(
							"Answered monster-face status packet 1028: role=" +
							statusPacket.RoleId.ToString() + ", status=" +
							statusPacket.StatusId.ToString() + ", value=0");
						return;
					}
					if (num != 1022)
					{
						if (num == 1032)
						{
							Action2Packet actionQuery;
							string error;
							if (!MapPacketCodec.TryReadAction2(
								netdata, out actionQuery, out error))
							{
								Log.Instance().WriteLog(
									"Rejected malformed action packet 1032: " +
									error);
								return;
							}
							if (actionQuery.Action == 157)
							{
								if (actionQuery.Timestamp != 0 ||
									actionQuery.ValueAt8 != 0 ||
									actionQuery.ContextAt12 != 0 ||
									actionQuery.ValueAt16 != 0 ||
									actionQuery.ValueAt18 != 0 ||
									actionQuery.ValueAt20 != 0 ||
									actionQuery.ValueAt24 != 0 ||
									actionQuery.ReservedAt28 != 0)
								{
									Log.Instance().WriteLog(
										"Unsupported action packet 1032/157 startup query " +
										"with nonzero fields");
									return;
								}
								Log.Instance().WriteLog(
									"Recognized action packet 1032/157 startup query; " +
									"no response sent because action 158 selection " +
									"semantics are unresolved");
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.HairUnlockAction)
							{
								this.GetWardrobeSystem().UnlockHairFromAction(
									actionQuery.ValueAt24);
								Log.Instance().WriteLog(
									"Handled wardrobe hair unlock action 1032/202: style=" +
									actionQuery.ValueAt24.ToString());
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.HairApplyAction)
							{
								this.GetWardrobeSystem().ApplyHairFromAction(
									actionQuery);
								Log.Instance().WriteLog(
									"Handled wardrobe hair apply action 1032/203: style=" +
									actionQuery.ValueAt24.ToString());
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.AvatarUnlockAction)
							{
								if (PlayerItem.IsWardrobeWeaponSoulType(
									actionQuery.ValueAt24))
								{
									bool purchased = this.GetItemSystem()
										.PurchaseWardrobeWeaponSoul(
											actionQuery.ValueAt24);
									Log.Instance().WriteLog(
										"Handled wardrobe weapon soul purchase action 1032/204: type=" +
										actionQuery.ValueAt24.ToString() + ", purchased=" +
										purchased.ToString());
								}
								else if (PlayerItem.IsWardrobeMountType(
									actionQuery.ValueAt24))
								{
									bool purchased = this.GetItemSystem()
										.PurchaseWardrobeMount(actionQuery.ValueAt24);
									Log.Instance().WriteLog(
										"Handled wardrobe mount purchase action 1032/204: type=" +
										actionQuery.ValueAt24.ToString() + ", purchased=" +
										purchased.ToString());
								}
								else
								{
									this.GetWardrobeSystem().UnlockAvatarFromAction(
										actionQuery.ValueAt24);
									Log.Instance().WriteLog(
										"Handled wardrobe avatar unlock action 1032/204: style=" +
										actionQuery.ValueAt24.ToString());
								}
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.AvatarApplyAction)
							{
								this.GetWardrobeSystem().ApplyAvatarFromAction(
									actionQuery);
								Log.Instance().WriteLog(
									"Handled wardrobe avatar apply action 1032/205: style=" +
									actionQuery.ValueAt24.ToString());
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.HairListAction &&
								actionQuery.ContextAt12 == 1U)
							{
								bool applied = this.GetWardrobeSystem()
									.ApplyHairFromOwnershipAction(actionQuery);
								Log.Instance().WriteLog(
									"Handled wardrobe hair ownership action 1032/206: style=" +
									actionQuery.ValueAt24.ToString() + ", applied=" +
									applied.ToString());
								return;
							}
							if (actionQuery.Action ==
								PlayerWardrobe.AvatarListAction &&
								actionQuery.ContextAt12 ==
								PlayerWardrobe.AvatarOwnershipContext)
							{
								bool applied = this.GetWardrobeSystem()
									.ApplyAvatarFromOwnershipAction(actionQuery);
								Log.Instance().WriteLog(
									"Handled wardrobe avatar ownership action 1032/206: style=" +
									actionQuery.ValueAt24.ToString() + ", applied=" +
									applied.ToString());
								return;
							}
							if (actionQuery.Action == 159)
							{
								if (actionQuery.Timestamp != 0 ||
									actionQuery.ValueAt8 != 0 ||
									actionQuery.ContextAt12 != 0 ||
									actionQuery.ValueAt16 != 0 ||
									actionQuery.ValueAt18 != 0 ||
									actionQuery.ValueAt20 != 0 ||
									actionQuery.ValueAt24 != 0 ||
									actionQuery.ReservedAt28 != 0)
								{
									Log.Instance().WriteLog(
										"Rejected malformed Goddess action packet 1032/159");
									return;
								}
								Log.Instance().WriteLog(
									"Recognized Goddess upgrade action packet 1032/159; " +
									"no response sent because authoritative cost, state, " +
									"and persistence semantics are unresolved");
								return;
							}
							if (actionQuery.Action == 160 ||
								actionQuery.Action == 161)
							{
								if (actionQuery.Timestamp != 0 ||
									actionQuery.ValueAt8 != 0 ||
									actionQuery.ContextAt12 != 0 ||
									actionQuery.ValueAt16 != 0 ||
									actionQuery.ValueAt18 != 0 ||
									actionQuery.ValueAt20 != 0 ||
									actionQuery.ValueAt24 < 1 ||
									actionQuery.ValueAt24 > 4 ||
									actionQuery.ReservedAt28 != 0)
								{
									Log.Instance().WriteLog(
										"Rejected malformed Goddess action packet 1032/" +
										actionQuery.Action.ToString());
									return;
								}
								Log.Instance().WriteLog(
									"Recognized Goddess selection action packet 1032/" +
									actionQuery.Action.ToString() +
									": selection=" +
									actionQuery.ValueAt24.ToString() +
									"; no response sent because authoritative state " +
									"and persistence semantics are unresolved");
								return;
							}
							Log.Instance().WriteLog(
								"Unsupported action packet 1032 variant: action=" +
								actionQuery.Action.ToString());
							return;
						}
					}
					else
					{
						if (base.GetGameMap().IsSafeArea(base.GetCurrentX(), base.GetCurrentY()))
						{
							this.LeftNotice("PK is prohibited in this area!");
							return;
						}
						MsgAttackInfo msgAttackInfo = new MsgAttackInfo();
						msgAttackInfo.Create(netdata, base.GetGamePackKeyEx());
						if (msgAttackInfo.tag == 21U)
						{
							uint nData = (uint)msgAttackInfo.usPosX ^ msgAttackInfo.roleId ^ 11990U;
							msgAttackInfo.usPosX = (ushort)(65535U & BaseFunc.ExchangeShortBits(nData, 15) + 56594U);
							nData = ((uint)msgAttackInfo.usPosY ^ msgAttackInfo.roleId ^ 47515U);
							msgAttackInfo.usPosY = (ushort)(65535U & BaseFunc.ExchangeShortBits(nData, 11) + 30430U);
							msgAttackInfo.idTarget = (BaseFunc.ExchangeLongBits((ulong)msgAttackInfo.idTarget, 13) ^ msgAttackInfo.roleId ^ 1596793955U) + 2341516570U;
							msgAttackInfo.usType = (65535U & BaseFunc.ExchangeShortBits(msgAttackInfo.usType ^ msgAttackInfo.roleId ^ 37213U, 13) + 5310U);
						}
						if (msgAttackInfo.roleId != base.GetTypeId())
						{
							this.GetEudemonSystem().Eudemon_Attack(msgAttackInfo);
							return;
						}
						this.GetFightSystem().SetFighting();
						if (this.GetTimerSystem().QueryStatus(1010) != null)
						{
							return;
						}
						if (base.IsLock() || this.IsDie())
						{
							return;
						}
						if (this.GetTimerSystem().QueryStatus(100) != null)
						{
							this.GetTimerSystem().DeleteStatus(100);
						}
						uint num2 = msgAttackInfo.tag;
						if (num2 != 2U)
						{
							if (num2 == 21U)
							{
								this.GetFightSystem().MagicAttack(msgAttackInfo);
							}
						}
						else
						{
							this.GetFightSystem().Attack(msgAttackInfo);
						}
						if (this.IsDancing())
						{
							this.SetDancing(0);
						}
						return;
					}
				}
			}
			else if (num <= 1102)
			{
				if (num == 1049)
				{
					PackIn packIn = new PackIn(netdata);
					short num10 = packIn.ReadInt16();
					uint num11 = packIn.ReadUInt32();
					uint num12 = packIn.ReadUInt32();
					int num13 = packIn.ReadInt32();
					short num14 = packIn.ReadInt16();
					short num15 = packIn.ReadInt16();
					PacketOut packetOut = new PacketOut(null);
					int num4 = num13;
					if (num4 != 1280)
					{
						if (num4 == 1285)
						{
							if (this.mnDancingId > 0 && Environment.TickCount - this.mnDancingTick > 2000)
							{
								byte[] array = new byte[16];
								array[0] = 5;
								array[1] = 5;
								byte[] v = array;
								packetOut = new PacketOut(null);
								packetOut.WriteUInt16(28);
								packetOut.WriteUInt16(1049);
								packetOut.WriteUInt32(base.GetTypeId());
								packetOut.WriteUInt32(num12);
								packetOut.WriteBuff(v);
								this.BroadcastBuffer(packetOut.Flush(), true);
								this.mnDancingId = 0;
							}
						}
					}
					else
					{
						BaseObject baseObject2 = this.GetGameMap().FindObjectForID(num12);
						if (baseObject2 != null)
						{
							if (Math.Abs((int)(base.GetCurrentX() - baseObject2.GetCurrentX())) > 2 || Math.Abs((int)(base.GetCurrentY() - baseObject2.GetCurrentY())) > 2)
							{
								this.LeftNotice("Too Far from the Other, Come Closer.");
							}
							else
							{
								byte[] v = new byte[]
								{
									2,
									5,
									0,
									0,
									232,
									0,
									0,
									0,
									131,
									0,
									0,
									0,
									byte.MaxValue,
									byte.MaxValue,
									byte.MaxValue,
									byte.MaxValue
								};
								packetOut.WriteUInt16(28);
								packetOut.WriteUInt16(1049);
								packetOut.WriteUInt32(base.GetTypeId());
								packetOut.WriteUInt32(num12);
								packetOut.WriteBuff(v);
								this.BroadcastBuffer(packetOut.Flush(), true);
								packetOut = new PacketOut(null);
								byte[] v2 = new byte[]
								{
									3,
									5,
									0,
									0,
									25,
									2,
									0,
									0,
									0,
									0,
									0,
									0,
									byte.MaxValue,
									byte.MaxValue,
									byte.MaxValue,
									byte.MaxValue
								};
								packetOut.WriteUInt16(28);
								packetOut.WriteUInt16(1049);
								packetOut.WriteUInt32(base.GetTypeId());
								packetOut.WriteUInt32(num12);
								packetOut.WriteBuff(v2);
								this.BroadcastBuffer(packetOut.Flush(), true);
								this.mnDancingId = num14;
								this.mnDancingTick = Environment.TickCount;
							}
						}
					}
					return;
				}
				if (num == 1056)
				{
					MsgTradInfo msgTradInfo = new MsgTradInfo();
					msgTradInfo.Create(netdata, null);
					short num8 = msgTradInfo.type;
					switch (num8)
					{
					case 1:
						this.GetTradSystem().RequstTrad(msgTradInfo);
						break;
					case 2:
						this.GetTradSystem().QuitTrad(msgTradInfo);
						break;
					case 3:
					case 4:
					case 5:
						break;
					case 6:
						this.GetTradSystem().AddTradItem(msgTradInfo.typeid);
						break;
					case 7:
						this.GetTradSystem().SetTradGold((int)msgTradInfo.typeid);
						break;
					default:
						if (num8 != 10)
						{
							if (num8 == 13)
							{
								this.GetTradSystem().SetTradGameGold((int)msgTradInfo.typeid);
							}
						}
						else
						{
							this.GetTradSystem().SureTrad();
						}
						break;
					}
					return;
				}
				switch (num)
				{
				case 1101:
				{
					MsgDropItem msgDropItem = new MsgDropItem();
					msgDropItem.Create(netdata, null);
					DropItemObject dropItem =
						base.GetGameMap().GetObject(msgDropItem.id) as DropItemObject;
					if (dropItem == null)
					{
						return;
					}
					if (this.IsDie() || base.IsLock())
					{
						return;
					}
					if (base.GetCurrentX() != dropItem.GetCurrentX() ||
						base.GetCurrentY() != dropItem.GetCurrentY())
					{
						this.ScroolRandom(base.GetCurrentX(), base.GetCurrentY());
						return;
					}
					if (dropItem.IsOwner() &&
						dropItem.GetOwnerId() != base.GetTypeId())
					{
						this.LeftNotice("This item is temporarily unable to pick up! ");
						return;
					}
					RoleItemInfo roleItemInfo3 = dropItem.GetRoleItemInfo();
					if (roleItemInfo3 == null)
					{
						if (!this.GetItemSystem().CanAwardItem(
							dropItem.GetTypeId(),
							MsgItemInfo.ITEMPOSITION_BACKPACK))
						{
							this.GetItemSystem().NotifyPackageFull(
								MsgItemInfo.ITEMPOSITION_BACKPACK);
							return;
						}
					}
					else if (!this.GetItemSystem().IsGold(roleItemInfo3.itemid) &&
						!this.GetItemSystem().CanAcceptAtPosition(
							roleItemInfo3.postion))
					{
						this.GetItemSystem().NotifyPackageFull(
							roleItemInfo3.postion);
						return;
					}
					if (!dropItem.TryClaimPickup(this.GetGameID()))
					{
						return;
					}
					if (roleItemInfo3 == null)
					{
						RoleItemInfo awarded = this.GetItemSystem().AwardItem(
							dropItem.GetTypeId(), 50, 1, 0, 0, 0, 0, 0, 0,
							0, 0, 0, true);
						if (awarded != null)
						{
							dropItem.CompletePickup(this.GetGameID());
						}
						else
						{
							dropItem.CancelPickup(this.GetGameID());
						}
					}
					else
					{
						if (this.GetItemSystem().IsGold(roleItemInfo3.itemid))
						{
							this.GetItemSystem().AwardItem(roleItemInfo3);
							dropItem.CompletePickup(this.GetGameID());
						}
						else if (!this.GetItemSystem().AwardDroppedItem(dropItem))
						{
							dropItem.CancelPickup(this.GetGameID());
						}
					}
					return;
				}
				case 1102:
				{
					MsgStrongPack msgStrongPack = new MsgStrongPack();
					msgStrongPack.Create(netdata, null);
					byte packageType = msgStrongPack.param;
					if (packageType != MsgStrongPack.STRONGPACK_TYPE)
					{
						if (packageType == MsgStrongPack.MOUNT_PACKAGE_TYPE)
						{
							if (msgStrongPack.type == MsgStrongPack.PACKAGE_REFRESH)
							{
								this.GetItemSystem().SendWardrobeMountPackage();
							}
							else if (msgStrongPack.type ==
								MsgStrongPack.PACKAGE_CHECK_OUT)
							{
								bool equipped = this.GetItemSystem()
									.EquipWardrobeMount(msgStrongPack.itemid);
								Log.Instance().WriteLog(
									"Handled wardrobe mount equip packet 1102/2: item=" +
									msgStrongPack.itemid.ToString() + ", equipped=" +
									equipped.ToString());
							}
						}
						else if (packageType == MsgStrongPack.FASHION_PACKAGE_TYPE ||
							packageType == MsgStrongPack.WEAPON_SOUL_PACKAGE_TYPE)
						{
							this.GetItemSystem().MoveWardrobeItem(
								msgStrongPack.itemid,
								packageType,
								msgStrongPack.type);
						}
					}
					else
					{
						switch (msgStrongPack.type)
						{
						case MsgStrongPack.STRONGPACK_TYPE_SAVE:
							if (this.GetItemSystem().GetStrongItemCount() < 100)
							{
								this.GetItemSystem().MoveItem(msgStrongPack.itemid, 100);
							}
							break;
						case MsgStrongPack.STRONGPACK_TYPE_GIVE:
							if (!this.GetItemSystem().IsItemFull())
							{
								this.GetItemSystem().MoveItem(msgStrongPack.itemid, 50);
							}
							break;
						}
					}
					return;
				}
				}
			}
			else if (num <= 2036)
			{
				switch (num)
				{
				case 1107:
				{
					SyndicateQueryPacket syndicateQuery;
					string error;
					if (!MapPacketCodec.TryReadSyndicateQuery(
						netdata, out syndicateQuery, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed syndicate packet 1107: " + error);
						return;
					}
					if (syndicateQuery.Reserved != 0 ||
						syndicateQuery.ReservedTail[0] != 0 ||
						syndicateQuery.ReservedTail[1] != 0)
					{
						Log.Instance().WriteLog(
							"Rejected reserved syndicate packet 1107 fields: action=" +
							syndicateQuery.Action.ToString() + ", reserved=" +
							syndicateQuery.Reserved.ToString() + ", target=" +
							syndicateQuery.TargetId.ToString() + ", fealty=" +
							syndicateQuery.FealtyId.ToString() + ", level=" +
							syndicateQuery.Level.ToString());
						return;
					}

					LegionManager.Instance().HandleSyndicatePacket(
						this, syndicateQuery);
					Log.Instance().WriteLog(
						"Processed syndicate packet 1107 action " +
						syndicateQuery.Action.ToString() + " for " +
						this.GetName() + ".");
					return;
				}
				case 1112:
				{
					SyndicateMemberQueryPacket memberQuery;
					string error;
					if (!MapPacketCodec.TryReadSyndicateMemberQuery(
						netdata, out memberQuery, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed syndicate-member packet 1112: " +
							error);
						return;
					}
					LegionManager.Instance().HandleMemberQuery(
						this, memberQuery);
					return;
				}
				case 1117:
				{
					EudemonPackagePacket packageQuery;
					string error;
					if (!MapPacketCodec.TryReadEudemonPackage(
						netdata, out packageQuery, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed eudemon-package packet 1117: " +
							error);
						return;
					}
					if (EudemonHatchManager.Handle(this, packageQuery))
					{
						return;
					}
					if (BatchHatchManager.Handle(this, packageQuery))
					{
						return;
					}
					bool hasRecordData = false;
					for (int index = 0;
						index < packageQuery.EudemonItemRecord.Length;
						index++)
					{
						if (packageQuery.EudemonItemRecord[index] != 0)
						{
							hasRecordData = true;
							break;
						}
					}

					if (packageQuery.Operation == 0 &&
						packageQuery.PackageType == 143 &&
						packageQuery.PackageId == base.GetTypeId() &&
						packageQuery.Context == 0 &&
						packageQuery.OperationValue == 0 &&
						packageQuery.EntryCount == 0 &&
						!hasRecordData)
					{
						byte[] response =
							MapPacketCodec.CreateEmptyEudemonPackageResponse(
								null,
								packageQuery.PackageId,
								packageQuery.Context,
								packageQuery.PackageType);
						base.SendData(response, true);
						Log.Instance().WriteLog(
							"Answered eudemon-package packet 1117 operation 0, " +
							"type 143 with an empty authoritative package for role " +
							packageQuery.PackageId.ToString() + ".");
						return;
					}

					if (packageQuery.Operation == 15 &&
						packageQuery.PackageType == 142 &&
						packageQuery.PackageId == 0 &&
						packageQuery.Context == 0 &&
						packageQuery.OperationValue == 0 &&
						packageQuery.EntryCount == 0 &&
						!hasRecordData &&
						!this.GetLegionSystem().IsHaveLegion())
					{
						Log.Instance().WriteLog(
							"Handled eudemon-package packet 1117 operation 15, " +
							"type 142 as absent: the active role has no syndicate, " +
							"so no battle-pet record response was sent.");
						return;
					}

					Log.Instance().WriteLog(
						"Unsupported eudemon-package packet 1117 variant: " +
						"operation=" + packageQuery.Operation.ToString() +
						", packageType=" + packageQuery.PackageType.ToString() +
						", packageId=" + packageQuery.PackageId.ToString() +
						", context=" + packageQuery.Context.ToString() +
						", value=" + packageQuery.OperationValue.ToString() +
						", entryCount=" + packageQuery.EntryCount.ToString() +
						", recordData=" + hasRecordData.ToString());
					return;
				}
				case 1123:
				{
					SystemTimePacket systemTimePacket;
					string error;
					if (!MapPacketCodec.TryReadSystemTime(
						netdata, out systemTimePacket, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed system-time packet 1123: " + error);
						return;
					}

					DateTime utcNow = DateTime.UtcNow;
					uint serverEpoch = checked((uint)(
						utcNow - new DateTime(
							1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
					int serverZone = checked(
						-(int)TimeZoneInfo.Local.GetUtcOffset(utcNow).TotalSeconds);
					byte[] response = MapPacketCodec.CreateSystemTimeResponse(
						null, 0, serverEpoch, serverZone);
					base.SendData(response, true);
					Log.Instance().WriteLog(
						"Answered system-time packet 1123: epoch=" +
						serverEpoch.ToString() + ", zone=" + serverZone.ToString());
					return;
				}
				case 1142:
				{
					PkItemListPacket pkItemQuery;
					string error;
					if (!MapPacketCodec.TryReadPkItemList(
						netdata, out pkItemQuery, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed PK-item packet 1142: " + error);
						return;
					}
					bool hasRecordData = false;
					for (int index = 0; index < pkItemQuery.PkItemRecord.Length; index++)
					{
						if (pkItemQuery.PkItemRecord[index] != 0)
						{
							hasRecordData = true;
							break;
						}
					}
					if (pkItemQuery.Action < 1 ||
						pkItemQuery.Action > 4 ||
						pkItemQuery.SubjectId != 0 ||
						pkItemQuery.Reserved != 0 ||
						pkItemQuery.Value != 0 ||
						pkItemQuery.EntryCountOrResult != 0 ||
						hasRecordData)
					{
						Log.Instance().WriteLog(
							"Unsupported PK-item packet 1142 variant: action=" +
							pkItemQuery.Action.ToString() + ", subject=" +
							pkItemQuery.SubjectId.ToString() + ", reserved=" +
							pkItemQuery.Reserved.ToString() + ", value=" +
							pkItemQuery.Value.ToString() + ", count/result=" +
							pkItemQuery.EntryCountOrResult.ToString());
						return;
					}

					byte[] response = MapPacketCodec.CreateEmptyPkItemListResponse(
						null, pkItemQuery.Action);
					base.SendData(response, true);
					Log.Instance().WriteLog(
						"Answered PK-item packet 1142 category " +
						pkItemQuery.Action.ToString() +
						" with an empty authoritative list.");
					return;
				}
				case 2031:
				{
					PackIn packIn = new PackIn(netdata);
					packIn.ReadUInt16();
					uint npcid = packIn.ReadUInt32();
					ScripteManager.Instance().ExecuteActionForNpc(npcid, this);
					return;
				}
				case 2032:
				{
					PackIn packIn = new PackIn(netdata);
					packIn.ReadUInt16();
					packIn.ReadUInt32();
					packIn.ReadUInt16();
					byte b2 = packIn.ReadByte();
					packIn.ReadInt16();
					string szStr = packIn.ReadString();
					if (b2 == 255)
					{
						this.SetTaskID(0U);
						return;
					}
					if (b2 > 0)
					{
						ScripteManager.Instance().ExecuteOptionId(b2, this, szStr);
					}
					else if (this.GetTaskID() != 0U)
					{
						ScripteManager.Instance().ExecuteOptionId(this.GetTaskID(), this, szStr);
						this.SetTaskID(0U);
					}
					return;
				}
				default:
					if (num == 2036)
					{
						if (netdata.Length >= 4 &&
							BitConverter.ToUInt16(netdata, 2) ==
								MapPacketCodec.GoddessRandomRequestCommand)
						{
							DataArrayPacket goddessRequest;
							string error;
							if (!MapPacketCodec.TryReadDataArray(
								netdata, out goddessRequest, out error))
							{
								Log.Instance().WriteLog(
									"Rejected malformed Goddess data-array packet " +
									"2036/286: " + error);
								return;
							}
							if (goddessRequest.Reserved != 0 ||
								goddessRequest.TrailingReserved != 0 ||
								goddessRequest.Count < 1 ||
								goddessRequest.Count > 5 ||
								goddessRequest.Values[0] < 1 ||
								goddessRequest.Values[0] > 4)
							{
								Log.Instance().WriteLog(
									"Rejected unsupported Goddess data-array packet " +
									"2036/286 fields");
								return;
							}
							for (int goddessIndex = 1;
								goddessIndex < goddessRequest.Values.Length;
								goddessIndex++)
							{
								if (goddessRequest.Values[goddessIndex] < 1 ||
									goddessRequest.Values[goddessIndex] > 4)
								{
									Log.Instance().WriteLog(
										"Rejected Goddess data-array packet 2036/286 " +
										"choice outside 1..4");
									return;
								}
							}
							Log.Instance().WriteLog(
								"Recognized Goddess random-state request packet " +
								"2036/286: selection=" +
								goddessRequest.Values[0].ToString() +
								", choiceCount=" +
								(goddessRequest.Count - 1).ToString() +
								"; no response sent because command 285 record " +
								"mutation and persistence semantics are unresolved");
							return;
						}
						MsgEquipOperation msgEquipOperation = new MsgEquipOperation();
						msgEquipOperation.Create(netdata, null);
						uint num2 = msgEquipOperation.type;
						if (num2 <= 65753U)
						{
							if (num2 != 65747U)
							{
								if (num2 != 65750U)
								{
									if (num2 == 65753U)
									{
										byte[] array = new byte[24];
										array[14] = 29;
										byte[] v3 = array;
										PacketOut packetOut = new PacketOut(null);
										packetOut.WriteInt16(32);
										packetOut.WriteInt16(1032);
										packetOut.WriteInt32(Environment.TickCount);
										packetOut.WriteBuff(v3);
										base.SendData(packetOut.Flush(), true);
									}
								}
								else
								{
									GuanJueManager.Instance().Donation(this, MONEYTYPE.GAMEGOLD, (int)msgEquipOperation.itemid);
								}
							}
							else
							{
								GuanJueManager.Instance().Donation(this, MONEYTYPE.GOLD, (int)msgEquipOperation.itemid);
							}
						}
						else if (num2 <= 131081U)
						{
							if (num2 != 65826U)
							{
								switch (num2)
								{
								case 131074U:
									EquipOperation.Instance().EquipQuality(this, msgEquipOperation.itemid, msgEquipOperation.materialid);
									break;
								case 131075U:
								case 131078U:
									EquipOperation.Instance().EquipStrong(this, msgEquipOperation.itemid, msgEquipOperation.materialid);
									break;
								case 131076U:
									EquipOperation.Instance().EquipLevel(this, msgEquipOperation.itemid, msgEquipOperation.materialid);
									break;
								case 131079U:
									EquipOperation.Instance().Equip_GodExp(this, msgEquipOperation.itemid, msgEquipOperation.materialid);
									break;
								case 131081U:
									EquipOperation.Instance().Magic_Add_God(this, msgEquipOperation.itemid, msgEquipOperation.materialid);
									break;
								}
							}
							else
							{
								EquipOperation.Instance().GemFusion(this, msgEquipOperation.itemid);
							}
						}
						else if (num2 != 262220U)
						{
							if (num2 == 458838U)
							{
								EquipOperation.Instance().GemReplace(this, netdata);
							}
						}
						else
						{
							byte index = 0;
							uint num16 = 0U;
							if (msgEquipOperation.materialid != 0U)
							{
								num16 = msgEquipOperation.materialid;
								index = 0;
							}
							else if (msgEquipOperation.param != 0U)
							{
								num16 = msgEquipOperation.param;
								index = 1;
							}
							else if (msgEquipOperation.param1 != 0U)
							{
								num16 = msgEquipOperation.param1;
								index = 2;
							}
							if (num16 != 0U)
							{
								EquipOperation.Instance().GemSet(this, num16, msgEquipOperation.itemid, index);
							}
						}
						return;
					}
					break;
				}
			}
			else
			{
				if (num == 2051)
				{
					FamilyQueryPacket familyQuery;
					string error;
					if (!MapPacketCodec.TryReadFamilyQuery(
						netdata, out familyQuery, out error))
					{
						Log.Instance().WriteLog(
							"Rejected malformed family packet 2051: " + error);
						return;
					}
					if (familyQuery.Reserved != 0 ||
						familyQuery.ReservedTail[0] != 0 ||
						familyQuery.ReservedTail[1] != 0 ||
						familyQuery.ReservedTail[2] != 0)
					{
						Log.Instance().WriteLog(
							"Rejected nonzero reserved family packet fields for action " +
							familyQuery.Action.ToString() + ".");
						return;
					}
					FamilyManager.Instance().HandlePacket(this, familyQuery);
					return;
				}
				if (num == 2060)
				{
					PackIn packIn2 = new PackIn(netdata);
					packIn2.ReadUInt16();
					short num17 = packIn2.ReadInt16();
					byte page = packIn2.ReadByte();
					if (num17 == 2)
					{
						GuanJueManager.Instance().RequestData(this, page);
					}
					return;
				}
				if (num == 3005)
				{
					MsgMoveInfo msgMoveInfo = new MsgMoveInfo();
					msgMoveInfo.Create(netdata, this.session.GetGamePackKeyEx());
					if (msgMoveInfo.id != base.GetTypeId())
					{
						this.GetEudemonSystem().Move(msgMoveInfo);
						if (this.IsDancing())
						{
							this.SetDancing(0);
						}
						return;
					}
					if (base.IsLock())
					{
						return;
					}
					if (this.GetTimerSystem().QueryStatus(1010) != null)
					{
						return;
					}
					if (this.Move(msgMoveInfo))
					{
						msgMoveInfo.id = base.GetTypeId();
						msgMoveInfo.x = base.GetCurrentX();
						msgMoveInfo.y = base.GetCurrentY();
						msgMoveInfo.dir = base.GetDir();
						byte[] buffer = msgMoveInfo.GetBuffer();
						base.SendData(buffer, false);
					}
					return;
				}
			}
			Debug.WriteLine("Unknown packet, protocol number:" + tag.ToString());
		}

		private void HandleNameQuery(NameQueryPacket packet)
		{
			switch (packet.Action)
			{
			case MsgHotKey.CHANGE_EUDEMON_NAME:
				this.HandleEudemonRename(packet);
				break;
			case MsgHotKey.WORLD_CHAT:
				this.HandleWorldPigeon(packet);
				break;
			case MsgHotKey.TAG_SAVEHOTKEY:
				this.HandleHotKeySave(packet);
				break;
			case 288:
				this.MsgBox(
					"The next version of the Hall of Fame will be released!");
				break;
			case MsgHotKey.TAG_WANGLING_STATE:
				if (this.GetTimerSystem().QueryStatus(1003) != null)
				{
					this.GetTimerSystem().DeleteStatus(1003);
				}
				else
				{
					this.GetTimerSystem().AddStatus(1003, 0, true);
				}
				break;
			default:
				LegionManager.Instance().HandleNameQuery(this, packet);
				break;
			}
		}

		private void HandleEudemonRename(NameQueryPacket packet)
		{
			if (packet.ReservedTail != 0 || packet.Strings.Length != 1)
			{
				Log.Instance().WriteLog(
					"Rejected malformed Eudemon rename packet for role " +
					base.GetTypeId().ToString() + ".");
				return;
			}
			string name = packet.Strings[0];
			string validationError;
			if (!TryValidateEudemonName(name, out validationError))
			{
				this.ChatNotice(validationError);
				return;
			}

			EudemonObject eudemon = this.GetEudemonSystem()
				.GetEudmeonObject(packet.TargetId);
			if (eudemon == null)
			{
				Log.Instance().WriteLog(
					"Rejected Eudemon rename for unowned ID " +
					packet.TargetId.ToString() + " from role " +
					base.GetTypeId().ToString() + ".");
				return;
			}

			RoleItemInfo item = this.GetItemSystem().FindItem(
				eudemon.GetEudemonInfo().itemid);
			if (item == null)
			{
				Log.Instance().WriteLog(
					"Rejected Eudemon rename because backing item " +
					eudemon.GetEudemonInfo().itemid.ToString() +
					" was not found.");
				return;
			}

			item.forgename = name;
			this.BroadcastBuffer(
				MapPacketCodec.CreateNameQueryResponse(
					null, packet.TargetId, packet.Action, name),
				true);
			this.GetItemSystem().UpdateItemInfo(item.id);
			this.GetItemSystem().DB_Save();
			this.GetEudemonSystem().SendEudemonInfo(
				eudemon.GetEudemonInfo(), true, true);
			if (this.GetEudemonSystem().GetBattleEudemon(packet.TargetId) != null)
			{
				eudemon.SendEudemonInfo(null);
			}
		}

		public static bool TryValidateEudemonName(
			string name,
			out string error)
		{
			const int maximumNameBytes = 31;
			if (string.IsNullOrEmpty(name) ||
				Coding.GetDefauleCoding().GetByteCount(name) > maximumNameBytes)
			{
				error = "Eudemon names must contain 1 to 31 encoded bytes.";
				return false;
			}
			for (int index = 0; index < name.Length; index++)
			{
				if (char.IsControl(name[index]))
				{
					error = "Eudemon names cannot contain control characters.";
					return false;
				}
			}
			error = null;
			return true;
		}

		private void HandleWorldPigeon(NameQueryPacket packet)
		{
			if (packet.ReservedTail != 0 || packet.Strings.Length != 1)
			{
				Log.Instance().WriteLog(
					"Rejected malformed world-pigeon packet from role " +
					base.GetTypeId().ToString() + ".");
				return;
			}

			string validationError;
			if (!WorldPigeon.TryValidateClientMessage(
				base.GetTypeId(), packet.Strings[0], out validationError))
			{
				this.ChatNotice(validationError);
				return;
			}
			if (this.GetLevel() < WorldPigeon.MinimumBroadcastLevel)
			{
				this.ChatNotice(
					"You cannot use Broadcast channel before you reach level 50.");
				return;
			}
			if (this.GetMoneyCount(MONEYTYPE.GAMEGOLD) <
				WorldPigeon.BroadcastPrice)
			{
				this.ChatNotice(
					"You don't have enough eudemon points to broadcast.");
				return;
			}

			int position = WorldPigeon.Instance().AddText(
				base.GetName(), base.GetTypeId(), packet.Strings[0]);
			if (position > 0)
			{
				this.ChangeMoney(
					MONEYTYPE.GAMEGOLD, -WorldPigeon.BroadcastPrice);
				DBServer.Instance().SaveRoleData(this, false);
				this.MsgBox(
					"Message pigeon published successfully, currently ranked: " +
					position.ToString() + ".");
			}
			else if (position == -1)
			{
				this.MsgBox(
					"You have already sent a carrier pigeon. Please wait for it to be sent.");
			}
		}

		private void HandleHotKeySave(NameQueryPacket packet)
		{
			if (packet.ReservedTail != 0 || packet.Strings.Length != 1 ||
				string.IsNullOrEmpty(packet.Strings[0]))
			{
				return;
			}
			byte group = (byte)packet.TargetId;
			string[] entries = packet.Strings[0].Split(new char[] { '-' });
			this.ClearHotKey(group);
			for (int index = 0; index < entries.Length; index++)
			{
				HotkeyInfo hotkey = new HotkeyInfo(group, entries[index]);
				if (hotkey.index != 0 || hotkey.id != 0)
				{
					this.AddHotKeyInfo(hotkey);
				}
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0002813C File Offset: 0x0002633C
		protected override void ProcessAction_Move(GameStruct.Action act)
		{
			byte runValue = 1;
			if (act.GetObjectCount() > 0)
			{
				runValue = (byte)act.GetObject(0);
			}
			this.RefreshVisibleObject();
			if (this.mVisibleList.Count > 0)
			{
				foreach (RefreshObject refreshObject in this.mVisibleList.Values)
				{
					BaseObject obj = refreshObject.obj;
					switch (obj.type)
					{
					case 1:
						if (!refreshObject.bRefreshTag)
						{
							this.SendNpcInfo(obj);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 2:
						this.SendRoleMoveInfo(obj, runValue, refreshObject);
						break;
					case 3:
					case 9:
						if (!refreshObject.bRefreshTag)
						{
							this.SendMonsterInfo(obj);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 4:
						if (!refreshObject.bRefreshTag)
						{
							(obj as EudemonObject).SendEudemonInfo(this);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 5:
						if (!refreshObject.bRefreshTag)
						{
							this.SendDropItemInfo(obj);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 6:
						if (!refreshObject.bRefreshTag)
						{
							(obj as RobotObject).SendRobotInfo(this);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 7:
						if (!refreshObject.bRefreshTag)
						{
							(obj as GuardKnightObject).SendInfo(this);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 8:
						if (!refreshObject.bRefreshTag)
						{
							(obj as EffectObject).SendInfo(this, false);
							refreshObject.bRefreshTag = true;
						}
						break;
					case 10:
						if (!refreshObject.bRefreshTag)
						{
							(obj as PtichObject).SendInfo(this);
							refreshObject.bRefreshTag = true;
						}
						break;
					}
					obj.AddVisibleObject(this, true);
				}
			}
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00028388 File Offset: 0x00026588
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			BaseObject baseObject = act.GetObject(0) as BaseObject;
			this.BeginDeath(baseObject, true);
		}

		public void DieForCommand()
		{
			if (this.IsDie() || this.IsGhost())
			{
				return;
			}

			this.ChangeAttribute(
				UserAttribute.LIFE, -(int)this.GetBaseAttr().life, true);
			this.BeginDeath(this, false);
		}

		private void BeginDeath(BaseObject killer, bool applyPkConsequences)
		{
			uint killerId = killer == null ? base.GetTypeId() : killer.GetTypeId();
			this.BroadcastBuffer(new MsgMonsterDieInfo
			{
				monsterid = base.GetTypeId(),
				roleid = killerId,
				role_x = base.GetCurrentX(),
				role_y = base.GetCurrentY(),
				tag = 14U
			}.GetBuffer(), true);
			this.m_bGhost = true;
			this.mnGhostTick = Environment.TickCount;
			this.GetFightSystem().SetAutoAttackTarget(null);
			PlayerObject enemy = null;
			if (applyPkConsequences && killer != null && killer.type == 2)
			{
				enemy = killer as PlayerObject;
			}
			else if (applyPkConsequences && killer != null && killer.type == 4)
			{
				enemy = (killer as EudemonObject).GetOwnerPlay();
			}
			if (enemy != null && !this.GetPKSystem().IsPKing())
			{
				this.GetFriendSystem().AddEnemy(enemy);
			}
			if (applyPkConsequences && killer != null)
			{
				this.GetPKSystem().Die(killer);
			}
			if (this.GetTimerSystem().QueryStatus(107) != null)
			{
				this.Alive(true);
			}
			this.GetTimerSystem().Die_DeleteState();
			this.GetEudemonSystem().Eudemon_ReCallAll(false);
			if (this.GetMountID() > 0U)
			{
				this.TakeOffMount(0U);
			}
			this.GetTimerSystem().AddStatus(1, 0, true);
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0002847B File Offset: 0x0002667B
		protected override void ProcessAction_Attack(GameStruct.Action act)
		{
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0002848C File Offset: 0x0002668C
		protected override void ProcessAction_Injured(GameStruct.Action act)
		{
			BaseObject baseObject = act.GetObject(0) as BaseObject;
			uint num = (uint)act.GetObject(1);
			if (!this.IsDie())
			{
				if (this.GetTimerSystem().QueryStatus(1008) != null && this.GetTimerSystem().QueryStatus(1003) != null && this.mZhaoHuanWuHuanObj == null)
				{
					int num2 = (int)(baseObject.GetCurrentX() - DIR._DELTA_X[(int)base.GetDir()]);
					int num3 = (int)(baseObject.GetCurrentY() - DIR._DELTA_Y[(int)base.GetDir()]);
					uint[] array = new uint[]
					{
						1433U,
						1432U,
						1434U
					};
					for (int i = 0; i < array.Length; i++)
					{
						MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(array[i]);
						if (monsterInfo != null)
						{
							MonsterObject monsterObject = new DiYuXieFu(this, baseObject, (short)num2, (short)num3, base.GetDir(), monsterInfo.id, monsterInfo.ai);
							base.GetGameMap().AddObject(monsterObject, null);
							monsterObject.Alive(false);
						}
					}
					this.SetZhaoHuanWuHuanObj(baseObject);
				}
			}
			this.mTarget = baseObject;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000285C4 File Offset: 0x000267C4
		public override bool IsDie()
		{
			return this.GetBaseAttr().life == 0U;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000285E8 File Offset: 0x000267E8
		public void EnterGame(GameSession _session, bool isFirst = false)
		{
			if (_session != null)
			{
				base.SetGameSession(_session);
			}
			if (base.GetGameSession() == null)
			{
				Log.Instance().WriteLog("Player entered EnterGame with a null session object.");
			}
			else
			{
				this.CalcAttribute();
				base.GetGameSession().gameid = base.GetGameID();
				UserEngine.Instance().AddPlayerObject(this);
				MsgNotice msgNotice = new MsgNotice();
				msgNotice.Create(null, base.GetGamePackKeyEx());
				MsgSelfRoleInfo msgSelfRoleInfo = new MsgSelfRoleInfo();
				msgSelfRoleInfo.Create(null, base.GetGamePackKeyEx());
				msgSelfRoleInfo.roleid = base.GetTypeId();
				msgSelfRoleInfo.lookface = this.GetBaseAttr().lookface;
				msgSelfRoleInfo.profession = this.GetBaseAttr().profession;
				msgSelfRoleInfo.name = base.GetName();
				this.GetBaseAttr().life = this.GetBaseAttr().life_max;
				msgSelfRoleInfo.life = (ushort)this.GetBaseAttr().life;
				msgSelfRoleInfo.maxlife = (ushort)this.GetBaseAttr().life_max;
				msgSelfRoleInfo.manna = (ushort)this.GetBaseAttr().mana;
				BaseAttributeInfo attributeInfo =
					ConfigManager.Instance().GetAttributeInfo(
						this.GetBaseAttr().profession,
						this.GetBaseAttr().level);
				if (attributeInfo != null)
				{
					msgSelfRoleInfo.attackpower = (ushort)Math.Min(
						(int)ushort.MaxValue,
						Math.Max(0, attributeInfo.force));
					msgSelfRoleInfo.doage = (ushort)Math.Min(
						(int)ushort.MaxValue,
						Math.Max(0, attributeInfo.dexterity));
					msgSelfRoleInfo.health = (ushort)Math.Min(
						(int)ushort.MaxValue,
						Math.Max(0, attributeInfo.health));
					msgSelfRoleInfo.magic_attack = (ushort)Math.Min(
						(int)ushort.MaxValue,
						Math.Max(0, attributeInfo.soul));
				}
				msgSelfRoleInfo.maxpetcall = (ushort)this.GetBaseAttr().maxeudemon;
				msgSelfRoleInfo.level = this.GetBaseAttr().level;
				msgSelfRoleInfo.param6[11] = this.GetBaseAttr().vip;
				msgSelfRoleInfo.param6[16] = this.GetBaseAttr().level;
				msgSelfRoleInfo.exp = (uint)Math.Max(0, this.GetBaseAttr().exp);
				msgSelfRoleInfo.pk =
					(ushort)Math.Max(0, (int)this.GetBaseAttr().pk);
				msgSelfRoleInfo.gold = (uint)this.GetBaseAttr().gold;
				msgSelfRoleInfo.godlevel = (int)this.GetBaseAttr().godlevel;
				msgSelfRoleInfo.gamegold = (uint)this.GetBaseAttr().gamegold;
				msgSelfRoleInfo.hair = this.GetBaseAttr().hair;
				msgSelfRoleInfo.guanjue = (byte)this.GetGuanJue();
				msgSelfRoleInfo.edubroodpacksize =
					EudemonHatchManager.IncubatorCapacity;
				msgSelfRoleInfo.godpetpackagelimit =
					PlayerEudemon.EudemonCapacity;
				msgSelfRoleInfo.param7[3] = PlayerItem.InventoryCapacity;
				if (isFirst)
				{
					base.SendData(msgNotice.GetStartGameBuff(), false);
					msgSelfRoleInfo.godlevel = 0;
					this.session.SendData(msgSelfRoleInfo.GetBuffer());
					this.SendRoleOtherSystemInfo();
					ScripteManager.Instance().ExecuteAction(1000U, this);
				}
				else
				{
					GameMap gameMapToID = MapManager.Instance().GetGameMapToID(this.GetBaseAttr().mapid);
					if (gameMapToID == null)
					{
						Log.Instance().WriteLog(string.Concat(new string[]
						{
							"Illegal player, illegal coordinates.. ",
							base.GetName(),
							"Map ID:",
							this.GetBaseAttr().mapid.ToString(),
							"Has been corrected to return to Canossa City"
						}));
						this.GetBaseAttr().mapid = 1000U;
						this.SetPoint(145, 413);
						gameMapToID = MapManager.Instance().GetGameMapToID(this.GetBaseAttr().mapid);
					}
					MapManager.Instance().GetGameMapToID(this.GetBaseAttr().mapid).AddObject(this, base.GetGameSession());
					this.SendJueweiNotice();
					this.session.SendData(msgNotice.GetStartGameBuff());
					this.session.SendData(msgSelfRoleInfo.GetBuffer());
					this.SendRoleOtherSystemInfo();
					MsgMapInfo msgMapInfo = new MsgMapInfo();
					msgMapInfo.Create(null, this.session.GetGamePackKeyEx());
					msgMapInfo.Init(this.GetBaseAttr().mapid, base.GetCurrentX(), base.GetCurrentY(), MsgMapInfo.ENTERMAP);
					this.session.SendData(msgMapInfo.GetBuffer());
					MsgMapInfo msgLoginComplete = new MsgMapInfo();
					msgLoginComplete.Create(null, this.session.GetGamePackKeyEx());
					msgLoginComplete.InitLoginComplete(base.GetTypeId());
					this.session.SendData(msgLoginComplete.GetBuffer());
					this.ChangeAttribute(UserAttribute.LOOKFACE, this.GetLookFace(), true);
					GameStruct.Action act = new GameStruct.Action(2, null);
					this.PushAction(act);
					ScriptTimerManager.Instance().PlayerEnterGame(this.GetBaseAttr().player_id);
					base.GetGameMap().SendWeatherInfo(this);
				}
			}
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00028958 File Offset: 0x00026B58
		public void ExitGame()
		{
			this.SetExit(true);
			this.GetEudemonSystem().ExitGame();
			this.GetTimerSystem().ExitGame();
			PtichManager.Instance().ShutPtich(this, false);
			DBServer.Instance().SaveRoleData(this, true);
			UserEngine.Instance().RemovePlayObject(this);
			this.GetFriendSystem().BrocatMsg(13);
			this.GetFriendSystem().OnLogout();
			if (this.GetTeam() != null)
			{
				this.GetTeam().ExitTeam(this);
			}
			this.GetFightSystem().RemoveQiShiTuanGuardEffect();
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
			ScriptTimerManager.Instance().PlayerExitGame(this.GetBaseAttr().player_id);
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00028A0C File Offset: 0x00026C0C
		public void Kick()
		{
			if (base.GetGameSession() != null)
			{
				base.GetGameSession().Dispose();
			}
			this.ExitGame();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00028A3C File Offset: 0x00026C3C
		private void SendRoleOtherSystemInfo()
		{
			this.GetMagicSystem().SendAllMagicInfo();
			this.GetItemSystem().SendAllItemInfo();
			this.GetWardrobeSystem().SendAllHairInfo();
			this.GetWardrobeSystem().SendAllAvatarInfo();
			this.ChangeAttribute(UserAttribute.SP, 0, true);
			this.GetEudemonSystem().SendAllEudemonInfo();
			this.SendHotKeyInfo();
			this.GetFriendSystem().SendAllFriendInfo();
			this.GetFriendSystem().BrocatMsg(12);
			this.GetLegionSystem().SendLegionInfo();
			this.GetFamilySystem().Init();
			FamilyManager.Instance().SendSnapshot(this);
			GuanJueManager.Instance().SendGuanJueInfo(this);
		}

		// Token: 0x06000382 RID: 898 RVA: 0x00028AB4 File Offset: 0x00026CB4
		public void FlyMap(uint mapid, short x, short y, byte dir)
		{
			GameMap gameMapToID = MapManager.Instance().GetGameMapToID(mapid);
			if (gameMapToID == null)
			{
				Log.Instance().WriteLog("Game map ID was not found: " + mapid.ToString());
			}
			else
			{
				if (base.GetGameMap() != null)
				{
					base.GetGameMap().RemoveObj(this);
				}
				this.mGameMap = gameMapToID;
				this.GetBaseAttr().mapid = mapid;
				this.SetPoint(x, y);
				base.SetDir(dir);
				gameMapToID.AddObject(this, base.GetGameSession());
				GameStruct.Action act = new GameStruct.Action(2, null);
				this.PushAction(act);
				MsgMapInfo msgMapInfo = new MsgMapInfo();
				msgMapInfo.Create(null, base.GetGamePackKeyEx());
				msgMapInfo.Init(mapid, x, y, 9541);
				base.SendData(msgMapInfo.GetBuffer(), false);
				base.GetGameMap().SendWeatherInfo(this);
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00028B98 File Offset: 0x00026D98
		public override void ClearThis()
		{
			base.ClearThis();
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00028BA4 File Offset: 0x00026DA4
		public void ClearThis(PlayerObject play)
		{
			MsgClearObjectInfo msgClearObjectInfo = new MsgClearObjectInfo();
			msgClearObjectInfo.Create(null, play.GetGamePackKeyEx());
			msgClearObjectInfo.id = base.GetTypeId();
			play.SendData(msgClearObjectInfo.GetBuffer(), false);
			base.GetVisibleList().Remove(play.GetGameID());
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00028BF4 File Offset: 0x00026DF4
		public override void Dispose()
		{
			if (!this.IsExit())
			{
				this.ExitGame();
			}
			base.Dispose();
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00028C1C File Offset: 0x00026E1C
		public bool IsGM()
		{
			bool result;
			if (GameServer.IsTestMode())
			{
				result = true;
			}
			else
			{
				string name = base.GetName();
				result = name.EndsWith("[PM]", StringComparison.OrdinalIgnoreCase);
			}
			return result;
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00028C5C File Offset: 0x00026E5C
		public void ResetLevelExp()
		{
			ulong exp_max = 0UL;
			LevelExp levelExp = ConfigManager.Instance().GetLevelExp(0U, this.GetBaseAttr().level);
			if (levelExp != null)
			{
				exp_max = levelExp.exp;
			}
			this.GetBaseAttr().exp_max = exp_max;
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00028CA4 File Offset: 0x00026EA4
		public void CalcFightSoul()
		{
			int num = (int)this.GetBaseAttr().level;
			num += (int)this.GetBaseAttr().godlevel;
			int num2 = 0;
			bool flag = true;
			for (int i = 1; i < 9; i++)
			{
				RoleItemInfo equipByPostion = this.GetItemSystem().GetEquipByPostion((byte)i);
				if (equipByPostion != null)
				{
					string text = equipByPostion.itemid.ToString();
					int num3 = Convert.ToInt32(text.Substring(text.Length - 1));
					num += num3;
					num += equipByPostion.GetGemCount();
					byte b = 0;
					while ((int)b < equipByPostion.GetGemCount())
					{
						byte gemType = equipByPostion.GetGemType(b);
						if (gemType >= 16 && gemType <= 18)
						{
							switch (gemType)
							{
							case 16:
								num++;
								break;
							case 17:
								num += 3;
								break;
							case 18:
								num += 5;
								break;
							}
						}
						b += 1;
					}
					if (flag)
					{
						if (num2 == 0)
						{
							num2 = (int)equipByPostion.GetStrongLevel();
						}
						else if (num2 > (int)equipByPostion.GetStrongLevel())
						{
							num2 = (int)equipByPostion.GetStrongLevel();
						}
					}
					if (EquipOperation.Instance().IsAccordWithEquip(this.GetBaseAttr().level, this.GetBaseAttr().profession, (byte)i, equipByPostion))
					{
						num++;
					}
				}
				else
				{
					flag = false;
				}
			}
			num += num2;
			switch (this.GetGuanJue())
			{
			case GUANGJUELEVEL.KING:
			case GUANGJUELEVEL.QUEEN:
				num += 6;
				break;
			case GUANGJUELEVEL.DUKE:
				num += 5;
				break;
			case GUANGJUELEVEL.MARQUIS:
				num += 4;
				break;
			case GUANGJUELEVEL.EARL:
				num += 3;
				break;
			case GUANGJUELEVEL.VISCOUNT:
				num += 2;
				break;
			case GUANGJUELEVEL.LORD:
				num++;
				break;
			}
			RoleItemInfo equipByPostion2 = this.GetItemSystem().GetEquipByPostion(15);
			if (equipByPostion2 != null)
			{
				num += (int)(equipByPostion2.GetStrongLevel() * 2);
			}
			equipByPostion2 = this.GetItemSystem().GetEquipByPostion(14);
			if (equipByPostion2 != null)
			{
				num += (int)equipByPostion2.GetStrongLevel();
			}
			equipByPostion2 = this.GetItemSystem().GetEquipByPostion(13);
			if (equipByPostion2 != null)
			{
				num += (int)equipByPostion2.GetStrongLevel();
			}
			num += this.GetEudemonSystem().CalcFightSoul();
			this.mnFightSoul = num;
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00028F18 File Offset: 0x00027118
		public override void CalcAttribute()
		{
			BaseAttributeInfo attributeInfo = ConfigManager.Instance().GetAttributeInfo(this.GetBaseAttr().profession, this.GetBaseAttr().level);
			PlayerAttribute baseAttr = this.GetBaseAttr();
			baseAttr.resetAttr();
			if (attributeInfo != null)
			{
				baseAttr.life_max = attributeInfo.GetLife();
				baseAttr.attack_max = (baseAttr.attack = attributeInfo.GetAttack());
				baseAttr.doage = attributeInfo.GetDoage();
				baseAttr.mana_max = attributeInfo.GetMana();
				baseAttr.magic_attack = (baseAttr.magic_attack_max = attributeInfo.GetMagicAttack());
			}
			for (int i = 1; i < 8; i++)
			{
				RoleItemInfo equipByPostion = this.GetItemSystem().GetEquipByPostion((byte)i);
				if (equipByPostion != null)
				{
					ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(equipByPostion.itemid);
					if (itemTypeInfo != null)
					{
						baseAttr.attack += itemTypeInfo.attack_min;
						baseAttr.attack_max += itemTypeInfo.attack_max;
						baseAttr.magic_attack += itemTypeInfo.magic_attack_min;
						baseAttr.magic_attack_max += itemTypeInfo.magic_attck_max;
						baseAttr.doage += itemTypeInfo.dodge;
						baseAttr.hitrate += itemTypeInfo.hitrate;
						baseAttr.defense += itemTypeInfo.defense;
						baseAttr.magic_defense += itemTypeInfo.magic_defense;
						ItemAdditionInfo itemAdditionInfo = ConfigManager.Instance().GetItemAdditionInfo((byte)i, equipByPostion.GetStrongLevel());
						if (itemAdditionInfo != null)
						{
							baseAttr.attack += itemAdditionInfo.min_attack;
							baseAttr.attack_max += itemAdditionInfo.max_attack;
							baseAttr.life += itemAdditionInfo.life;
							baseAttr.defense += itemAdditionInfo.defense;
							baseAttr.magic_attack += itemAdditionInfo.min_magic_attack;
							baseAttr.magic_attack_max += itemAdditionInfo.max_magic_attack;
							baseAttr.magic_defense += itemAdditionInfo.magic_defense;
							baseAttr.doage += itemAdditionInfo.dodge;
						}
					}
				}
			}
			this.CalcFightSoul();
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00029178 File Offset: 0x00027378
		public override byte GetLevel()
		{
			return this.GetBaseAttr().level;
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00029198 File Offset: 0x00027398
		public override int GetMinAck()
		{
			return (int)(this.GetBaseAttr().attack + (uint)this.GetEudemonSystem().GetFitEudemonMinAtk());
		}

		// Token: 0x0600038C RID: 908 RVA: 0x000291C4 File Offset: 0x000273C4
		public override int GetMaxAck()
		{
			return (int)(this.GetBaseAttr().attack_max + (uint)this.GetEudemonSystem().GetFitEudemonMaxAtk());
		}

		// Token: 0x0600038D RID: 909 RVA: 0x000291F0 File Offset: 0x000273F0
		public override int GetDefense()
		{
			EudemonObject injuredEudemon = this.GetEudemonSystem().GetInjuredEudemon();
			int defense;
			if (injuredEudemon != null)
			{
				defense = injuredEudemon.GetDefense();
			}
			else
			{
				defense = (int)this.GetBaseAttr().defense;
			}
			return defense;
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0002922C File Offset: 0x0002742C
		public override int GetMagicDefense()
		{
			EudemonObject injuredEudemon = this.GetEudemonSystem().GetInjuredEudemon();
			int result;
			if (injuredEudemon != null)
			{
				result = injuredEudemon.GetMagicDefense();
			}
			else
			{
				result = (int)this.GetBaseAttr().magic_defense;
			}
			return result;
		}

		// Token: 0x0600038F RID: 911 RVA: 0x00029268 File Offset: 0x00027468
		public override int GetMagicAck()
		{
			return (int)this.GetBaseAttr().magic_attack;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00029288 File Offset: 0x00027488
		public override int GetMaxMagixAck()
		{
			return (int)this.GetBaseAttr().magic_attack_max;
		}

		// Token: 0x06000391 RID: 913 RVA: 0x000292A5 File Offset: 0x000274A5
		public void SetPkMode(byte pkmode)
		{
			this.GetBaseAttr().pk_mode = pkmode;
		}

		// Token: 0x06000392 RID: 914 RVA: 0x000292B4 File Offset: 0x000274B4
		public void OpenDialog(int dwData)
		{
			if (this.mNpcInfo != null)
			{
				MsgOpenDialog msgOpenDialog = new MsgOpenDialog();
				msgOpenDialog.Create(null, base.GetGamePackKeyEx());
				msgOpenDialog.playid = base.GetTypeId();
				msgOpenDialog.npc_x = this.mNpcInfo.x;
				msgOpenDialog.npc_y = this.mNpcInfo.y;
				msgOpenDialog.npcid = this.mNpcInfo.id;
				msgOpenDialog.dialog_type = dwData;
				base.SendData(msgOpenDialog.GetBuffer(), false);
				if (dwData == 3)
				{
					Thread.Sleep(50);
					List<RoleItemInfo> list = new List<RoleItemInfo>();
					this.GetItemSystem().GetItemStrongInfo(list);
					int num = list.Count / 6;
					if (list.Count % 6 > 0)
					{
						num++;
					}
					for (int i = 0; i < num; i++)
					{
						MsgStrongInfo msgStrongInfo = new MsgStrongInfo();
						if (i > 0)
						{
							msgStrongInfo.param1 = 3;
						}
						msgStrongInfo.playid = base.GetTypeId();
						int num2 = i * 6;
						for (int j = 0; j < 6; j++)
						{
							if (num2 >= list.Count)
							{
								break;
							}
							msgStrongInfo.list_item.Add(list[num2]);
							num2++;
						}
						msgStrongInfo.Create(null, base.GetGamePackKeyEx());
						base.SendData(msgStrongInfo.GetBuffer(), false);
					}
					byte[] strongMoneyBuffer = MsgStrongInfo.GetStrongMoneyBuffer(base.GetTypeId(), this.GetMoneyCount(MONEYTYPE.STRONGGOLD));
					base.SendData(strongMoneyBuffer, true);
				}
			}
		}

		// Token: 0x06000393 RID: 915 RVA: 0x00029458 File Offset: 0x00027658
		public void ScroolRandom(short _x = 0, short _y = 0)
		{
			int num = 0;
			short num2 = _x;
			short num3 = _y;
			if (num2 == 0 && num3 == 0)
			{
				for (;;)
				{
					num2 = (short)IRandom.Random(1, (int)base.GetGameMap().mnWidth);
					num3 = (short)IRandom.Random(1, (int)base.GetGameMap().mnHeight);
					if (base.GetGameMap().CanMove(num2, num3))
					{
						break;
					}
					if (num > 100)
					{
						goto Block_4;
					}
					num++;
				}
				goto IL_7B;
				Block_4:
				return;
			}
			IL_7B:
			this.ClearThis();
			this.SetPoint(num2, num3);
			MsgScroolRandom msgScroolRandom = new MsgScroolRandom();
			msgScroolRandom.Create(null, base.GetGamePackKeyEx());
			msgScroolRandom.time = Environment.TickCount;
			msgScroolRandom.x = (msgScroolRandom._x = base.GetCurrentX());
			msgScroolRandom.y = (msgScroolRandom._y = base.GetCurrentY());
			msgScroolRandom.roleid = base.GetTypeId();
			base.SendData(msgScroolRandom.GetBuffer(), false);
			base.GetVisibleList().Clear();
			GameStruct.Action act = new GameStruct.Action(2, null);
			this.PushAction(act);
			this.GetEudemonSystem().FlyPlay();
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00029584 File Offset: 0x00027784
		public void ChangeFubenMap(GameMap map, short x, short y)
		{
			if (map != null)
			{
				base.GetGameMap().RemoveObj(this);
				this.GetEudemonSystem().Eudemon_ReCallAll(true);
				map.AddObject(this, base.GetGameSession());
				this.ClearThis();
				this.SetPoint(x, y);
				MsgReCall1 msgReCall = new MsgReCall1();
				msgReCall.Create(null, base.GetGamePackKeyEx());
				msgReCall.roleid = base.GetTypeId();
				msgReCall.mapid = (int)base.GetGameMap().GetMapInfo().id;
				msgReCall.x = base.GetCurrentX();
				msgReCall.y = base.GetCurrentY();
				base.SendData(msgReCall.GetBuffer(), false);
				MsgReCall2 msgReCall2 = new MsgReCall2();
				msgReCall2.Create(null, base.GetGamePackKeyEx());
				msgReCall2.roleid = base.GetTypeId();
				msgReCall2.x = base.GetCurrentX();
				msgReCall2.y = base.GetCurrentY();
				base.SendData(msgReCall2.GetBuffer(), false);
				base.GetVisibleList().Clear();
				GameStruct.Action act = new GameStruct.Action(2, null);
				this.PushAction(act);
				this.GetBaseAttr().mapid = map.GetMapInfo().id;
				this.SendJueweiNotice();
				this.SetTransmitIng(true);
			}
		}

		// Token: 0x06000395 RID: 917 RVA: 0x000296C0 File Offset: 0x000278C0
		public void ChangeMap(uint mapid, short x, short y)
		{
			GameMap gameMapToID = MapManager.Instance().GetGameMapToID(mapid);
			if (gameMapToID == null)
			{
				Log.Instance().WriteLog(string.Concat(new string[]
				{
					"Map transmission failed, the map ID does not exist",
					mapid.ToString(),
					" x:",
					x.ToString(),
					" y:",
					y.ToString()
				}));
			}
			else
			{
				base.GetGameMap().RemoveObj(this);
				this.GetEudemonSystem().Eudemon_ReCallAll(true);
				gameMapToID.AddObject(this, base.GetGameSession());
				this.ClearThis();
				this.SetPoint(x, y);
				MsgReCall1 msgReCall = new MsgReCall1();
				msgReCall.Create(null, base.GetGamePackKeyEx());
				msgReCall.roleid = base.GetTypeId();
				msgReCall.mapid = (int)base.GetGameMap().GetMapInfo().id;
				msgReCall.x = base.GetCurrentX();
				msgReCall.y = base.GetCurrentY();
				base.SendData(msgReCall.GetBuffer(), false);
				MsgReCall2 msgReCall2 = new MsgReCall2();
				msgReCall2.Create(null, base.GetGamePackKeyEx());
				msgReCall2.roleid = base.GetTypeId();
				msgReCall2.x = base.GetCurrentX();
				msgReCall2.y = base.GetCurrentY();
				base.SendData(msgReCall2.GetBuffer(), false);
				base.GetVisibleList().Clear();
				GameStruct.Action act = new GameStruct.Action(2, null);
				this.PushAction(act);
				this.GetBaseAttr().mapid = mapid;
				this.SendJueweiNotice();
				this.SetTransmitIng(true);
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00029854 File Offset: 0x00027A54
		public void ReCallMap()
		{
			short x = (short)base.GetGameMap().GetMapInfo().recallx;
			short y = (short)base.GetGameMap().GetMapInfo().recally;
			uint recallid = base.GetGameMap().GetMapInfo().recallid;
			this.ChangeMap(recallid, x, y);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x000298A4 File Offset: 0x00027AA4
		public void BroadcastBuffer(byte[] data, bool isThis = false)
		{
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2 && obj.GetGameSession() != null)
				{
					BaseMsg baseMsg = new BaseMsg();
					baseMsg.Create(data, obj.GetGamePackKeyEx());
					obj.SendData(baseMsg.GetBuffer(), false);
				}
			}
			if (isThis)
			{
				if (base.GetGameSession() != null)
				{
					BaseMsg baseMsg = new BaseMsg();
					baseMsg.Create(data, base.GetGamePackKeyEx());
					base.SendData(baseMsg.GetBuffer(), false);
				}
			}
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00029988 File Offset: 0x00027B88
		public void ChangeAttribute(UserAttribute type, int value, bool isBrocat = true)
		{
			if (base.GetGameSession() != null)
			{
				int num = value;
				if (type <= UserAttribute.HAIR)
				{
					switch (type)
					{
					case UserAttribute.LIFE:
						num = (int)(this.GetBaseAttr().life + (uint)value);
						if (num < 0)
						{
							num = 0;
						}
						this.GetBaseAttr().life = (uint)num;
						if ((long)num > (long)((ulong)this.GetBaseAttr().life_max))
						{
							this.GetBaseAttr().life = this.GetBaseAttr().life_max;
						}
						if (this.GetTimerSystem().QueryStatus(1000) != null)
						{
							this.GetBaseAttr().life = this.GetBaseAttr().life_max;
						}
						break;
					case UserAttribute.LIFE_MAX:
						this.GetBaseAttr().life_max = (uint)value;
						break;
					case UserAttribute.MANA:
						num = (int)(this.GetBaseAttr().mana + (uint)value);
						if (num < 0)
						{
							num = 0;
						}
						this.GetBaseAttr().mana = (uint)num;
						break;
					case UserAttribute.MANA_MAX:
						this.GetBaseAttr().mana_max = (uint)value;
						break;
					case UserAttribute.GOLD:
						if (this.GetBaseAttr().gold + value > 2000000000)
						{
							this.MsgBox("Young man, stop grinding! The maximum is two billion gold coins!");
							this.GetBaseAttr().gold = 2000000000;
						}
						else
						{
							this.GetBaseAttr().gold += value;
						}
						if (this.GetBaseAttr().gold < 0)
						{
							this.GetBaseAttr().gold = 0;
						}
						num = this.GetBaseAttr().gold;
						break;
					case UserAttribute.EXP:
						this.GetBaseAttr().exp += value;
						num = this.GetBaseAttr().exp;
						break;
					case UserAttribute.PK:
						if ((int)this.GetBaseAttr().pk + value >= 30000)
						{
							this.GetBaseAttr().pk = 30000;
						}
						else
						{
							PlayerAttribute baseAttr = this.GetBaseAttr();
							baseAttr.pk += (short)value;
						}
						if (this.GetBaseAttr().pk < 0)
						{
							this.GetBaseAttr().pk = 0;
						}
						this.GetPKSystem().ResetPKNameType();
						break;
					case UserAttribute.PORFESSION:
					case UserAttribute.SIZEADD:
					case UserAttribute.MONEYSAVED:
					case UserAttribute.ADDPOINT:
						break;
					case UserAttribute.SP:
						this.GetBaseAttr().sp += value;
						num = this.GetBaseAttr().sp;
						break;
					case UserAttribute.LOOKFACE:
						if (!this.IsGhost())
						{
							this.GetBaseAttr().lookface = (uint)value;
						}
						num = value;
						break;
					case UserAttribute.LEVEL:
					{
						PlayerAttribute baseAttr2 = this.GetBaseAttr();
						baseAttr2.level += (byte)value;
						num = (int)this.GetBaseAttr().level;
						this.CalcAttribute();
						this.GetBaseAttr().life = this.GetBaseAttr().life_max;
						this.GetBaseAttr().mana = this.GetBaseAttr().mana_max;
						PacketOut packetOut = new PacketOut(null);
						packetOut.WriteInt16(28);
						packetOut.WriteInt16(1010);
						packetOut.WriteInt32(Environment.TickCount);
						packetOut.WriteUInt32(base.GetTypeId());
						packetOut.WriteInt32(0);
						packetOut.WriteInt32(0);
						packetOut.WriteInt32(1);
						packetOut.WriteInt32(9550);
						this.BroadcastBuffer(packetOut.Flush(), true);
						break;
					}
					default:
						switch (type)
						{
						case UserAttribute.STATUS:
							num = value;
							break;
						case UserAttribute.HAIR:
							this.GetBaseAttr().hair = (uint)value;
							num = value;
							break;
						}
						break;
					}
				}
				else if (type != UserAttribute.MAXEUDEMON)
				{
					if (type == UserAttribute.GAMEGOLD)
					{
						if (this.GetBaseAttr().gold + value > 2000000000)
						{
							this.GetBaseAttr().gamegold = 2000000000;
							this.MsgBox("Young man, stop spamming; the maximum is two billion magic stones!");
						}
						else
						{
							this.GetBaseAttr().gamegold += value;
						}
						if (this.GetBaseAttr().gamegold < 0)
						{
							this.GetBaseAttr().gamegold = 0;
						}
						num = this.GetBaseAttr().gamegold;
					}
				}
				else
				{
					PlayerAttribute baseAttr3 = this.GetBaseAttr();
					baseAttr3.maxeudemon += (byte)value;
					num = (int)this.GetBaseAttr().maxeudemon;
				}
				MsgUserAttribute msgUserAttribute = new MsgUserAttribute();
				msgUserAttribute.role_id = base.GetTypeId();
				if (isBrocat)
				{
					msgUserAttribute.Create(null, null);
				}
				msgUserAttribute.AddAttribute(type, (uint)num);
				if (type == UserAttribute.LEVEL)
				{
					msgUserAttribute.AddAttribute(UserAttribute.LIFE_MAX, this.GetBaseAttr().life_max);
					msgUserAttribute.AddAttribute(UserAttribute.LIFE, this.GetBaseAttr().life);
					if (this.GetBaseAttr().mana_max > 0U)
					{
						msgUserAttribute.AddAttribute(UserAttribute.MANA_MAX, this.GetBaseAttr().mana_max);
						msgUserAttribute.AddAttribute(UserAttribute.MANA, this.GetBaseAttr().mana);
					}
				}
				if (isBrocat)
				{
					this.BroadcastBuffer(msgUserAttribute.GetBuffer(), true);
				}
				else
				{
					base.SendData(msgUserAttribute.GetBuffer(), true);
				}
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00029EB0 File Offset: 0x000280B0
		public void AddExp(int nDamage, int nAtkLev, int nDefLev)
		{
			int num = BattleSystem.AdjustExp(nDamage, nAtkLev, nDefLev);
			num = this.AdjustExp(num);
			this.LeftNotice(string.Format("Obtained kill amount experience {0}", num.ToString()));
			this.ChangeAttribute(UserAttribute.EXP, num, false);
			bool flag = false;
			for (;;)
			{
				LevelExp levelExp = ConfigManager.Instance().GetLevelExp(0U, this.GetLevel());
				if (levelExp == null)
				{
					break;
				}
				if ((long)this.GetBaseAttr().exp < (long)levelExp.exp)
				{
					break;
				}
				this.GetBaseAttr().exp -= (int)levelExp.exp;
				PlayerAttribute baseAttr = this.GetBaseAttr();
				baseAttr.level += 1;
				flag = true;
			}
			if (flag)
			{
				this.ChangeAttribute(UserAttribute.LEVEL, 0, true);
				this.ChangeAttribute(UserAttribute.EXP, 0, false);
			}
			this.GetEudemonSystem().AddExp(num);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00029F94 File Offset: 0x00028194
		public override void Injured(BaseObject obj, uint value, MsgAttackInfo info)
		{
			this.GetFightSystem().SetFighting();
			if (!this.GetEudemonSystem().Eudemon_Injured(obj, value, info))
			{
				this.ChangeAttribute(UserAttribute.LIFE, (int)(-(int)value), true);
			}
			GameStruct.Action action = new GameStruct.Action(6, null);
			action.AddObject(obj);
			action.AddObject(value);
			action.AddObject(info);
			this.PushAction(action);
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00029FFC File Offset: 0x000281FC
		public void LeftNotice(string text)
		{
			MsgLeftNotice msgLeftNotice = new MsgLeftNotice();
			msgLeftNotice.Create(null, base.GetGamePackKeyEx());
			msgLeftNotice.SetRoleName(base.GetName());
			msgLeftNotice.SetText(text);
			base.SendData(msgLeftNotice.GetBuffer(), false);
		}

		// Token: 0x0600039C RID: 924 RVA: 0x0002A044 File Offset: 0x00028244
		public void ChatNotice(string text)
		{
			MsgNotice msgNotice = new MsgNotice();
			msgNotice.Create(null, base.GetGamePackKeyEx());
			byte[] chatNoticeBuff = msgNotice.GetChatNoticeBuff(text);
			base.SendData(chatNoticeBuff, false);
		}

		// Token: 0x0600039D RID: 925 RVA: 0x0002A078 File Offset: 0x00028278
		public void MsgBox(string text)
		{
			MsgNotice msgNotice = new MsgNotice();
			msgNotice.Create(null, base.GetGamePackKeyEx());
			byte[] msgBoxBuff = msgNotice.GetMsgBoxBuff(text);
			base.SendData(msgBoxBuff, false);
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0002A0AC File Offset: 0x000282AC
		public void SendHotKeyInfo()
		{
			MsgHotKey msgHotKey = new MsgHotKey();
			msgHotKey.Create(null, base.GetGamePackKeyEx());
			msgHotKey.type = 0;
			msgHotKey.tag = 214;
			msgHotKey.tag2 = 4;
			msgHotKey.str = "";
			for (int i = 0; i < this.mListHotKey.Count; i++)
			{
				MsgHotKey msgHotKey2 = msgHotKey;
				msgHotKey2.str = msgHotKey2.str + this.mListHotKey[i].GetString(false) + "-";
			}
			base.SendData(msgHotKey.GetBuffer(), false);
			PacketOut packetOut = new PacketOut(base.GetGamePackKeyEx());
			packetOut.WriteUInt16(14);
			packetOut.WriteUInt16(1015);
			packetOut.WriteInt32(0);
			packetOut.WriteInt16(656);
			packetOut.WriteInt32(2);
			base.SendData(packetOut.Flush(), false);
		}

		// Token: 0x0600039F RID: 927 RVA: 0x0002A18F File Offset: 0x0002838F
		public void AddHotKeyInfo(HotkeyInfo info)
		{
			this.mListHotKey.Add(info);
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0002A1A0 File Offset: 0x000283A0
		public void ClearHotKey(byte group)
		{
			int num = this.mListHotKey.Count;
			if (num > 0)
			{
				do
				{
					num--;
					if (this.mListHotKey[num].group == group)
					{
						this.mListHotKey.RemoveAt(num);
					}
				}
				while (num > 0);
			}
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x0002A208 File Offset: 0x00028408
		public string GetHotKeyInfo()
		{
			string text = "";
			for (int i = 0; i < this.mListHotKey.Count; i++)
			{
				text = text + this.mListHotKey[i].GetString(true) + ",";
			}
			return text;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x0002A25C File Offset: 0x0002845C
		public void SetHotKeyInfo(string text)
		{
			if (text.Length > 0)
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'|'
					});
					if (array2.Length == 7)
					{
						byte group = Convert.ToByte(array2[0]);
						string text2 = array[i].Substring(array[i].IndexOf('|') + 1);
						HotkeyInfo info = new HotkeyInfo(group, text2);
						this.AddHotKeyInfo(info);
					}
				}
			}
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x0002A311 File Offset: 0x00028511
		public void TransformGhost()
		{
			this.m_bGhost = true;
			this.ChangeAttribute(UserAttribute.LOOKFACE, this.GetLookFace(), true);
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0002A32C File Offset: 0x0002852C
		public int GetMoneyCount(MONEYTYPE type)
		{
			int result;
			switch (type)
			{
			case MONEYTYPE.GOLD:
				result = this.GetBaseAttr().gold;
				break;
			case MONEYTYPE.GAMEGOLD:
				result = this.GetBaseAttr().gamegold;
				break;
			case MONEYTYPE.STRONGGOLD:
				result = (int)this.GetBaseAttr().stronggold;
				break;
			default:
				result = -1;
				break;
			}
			return result;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x0002A388 File Offset: 0x00028588
		public void ChangeMoney(MONEYTYPE type, int value)
		{
			switch (type)
			{
			case MONEYTYPE.GOLD:
				this.ChangeAttribute(UserAttribute.GOLD, value, true);
				break;
			case MONEYTYPE.GAMEGOLD:
				this.ChangeAttribute(UserAttribute.GAMEGOLD, value, true);
				break;
			case MONEYTYPE.STRONGGOLD:
				this.GetBaseAttr().stronggold += (long)value;
				if (this.GetBaseAttr().stronggold > unchecked((long)0xFFFFFFFFB2D05E00UL))
				{
					this.GetBaseAttr().stronggold = unchecked((long)0xFFFFFFFFB2D05E00UL);
				}
				break;
			}
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0002A40B File Offset: 0x0002860B
		public void PlayRobotAction(uint action_id)
		{
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x0002A40E File Offset: 0x0002860E
		public void RefreshRoleInfo()
		{
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x0002A414 File Offset: 0x00028614
		private void SendJueweiNotice()
		{
			string text = "";
			switch (this.GetGuanJue())
			{
			case GUANGJUELEVEL.KING:
				text = string.Format("The Kingdom's Guardian, the Immortal King {0} inheriting the glory of the gods, has arrived at {1}! His arrival brings infinite courage and hope to people.", base.GetName(), base.GetGameMap().GetMapInfo().name);
				break;
			case GUANGJUELEVEL.QUEEN:
				text = string.Format("{0}, Queen's Arrival at {1}. Her radiance is like the most brilliant sun at noon, her smile like a star that never falls in the eternal night, truly admirable.", base.GetName(), base.GetGameMap().GetMapInfo().name);
				break;
			case GUANGJUELEVEL.DUKE:
				text = string.Format("With the arrival of Count {0}, the people of {1} will share in the supreme pride and glory of the Atreian Kingdom at this moment.", base.GetName(), base.GetGameMap().GetMapInfo().name);
				break;
			case GUANGJUELEVEL.MARQUIS:
				text = string.Format("{0}Marquess {1} arrives, his towering figure is like an eagle bringing hope to people.", base.GetName(), base.GetGameMap().GetMapInfo().name);
				break;
			}
			if (text.Length > 0)
			{
				base.GetGameMap().BroadcastMsg(BROADCASTMSGTYPE.LEFT, text);
			}
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x0002A518 File Offset: 0x00028718
		public void TakeMount(uint eudemon_id, uint nMountID)
		{
			if (!base.IsLock())
			{
				this.GetEudemonSystem().TakeMount(eudemon_id);
				byte[] v = new byte[]
				{
					36,
					0,
					244,
					7,
					209,
					0,
					7,
					0
				};
				byte[] v2 = new byte[]
				{
					75,
					0,
					0,
					0,
					1,
					0,
					0,
					0,
					20,
					0,
					0,
					0,
					0,
					0,
					0,
					0
				};
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteBuff(v);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteUInt32(nMountID);
				packetOut.WriteUInt32(eudemon_id);
				packetOut.WriteBuff(v2);
				this.BroadcastBuffer(packetOut.Flush(), true);
				this.mnMountID = nMountID;
				this.GetMagicSystem().SetMoveSpeed(200f);
			}
		}

		public void TakeWardrobeMount(
			uint mountItemId,
			uint mountServerType)
		{
			if (base.IsLock() || mountItemId == 0U || mountServerType == 0U)
			{
				return;
			}

			byte[] packet = MapPacketCodec.CreateWardrobeMountStateResponse(
				null,
				base.GetTypeId(),
				mountServerType,
				mountItemId);
			this.BroadcastBuffer(packet, true);
			this.mnMountID = mountItemId;
			this.GetMagicSystem().SetMoveSpeed(200f);
		}

		// Token: 0x060003AA RID: 938 RVA: 0x0002A5C4 File Offset: 0x000287C4
		public void TakeOffMount(uint eudemon_id)
		{
			if (!base.IsLock())
			{
				this.GetEudemonSystem().TakeOffMount(eudemon_id);
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteUInt16(28);
				packetOut.WriteUInt16(1009);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteUInt32(eudemon_id);
				byte[] array = new byte[16];
				array[0] = 111;
				byte[] v = array;
				packetOut.WriteBuff(v);
				this.BroadcastBuffer(packetOut.Flush(), true);
				this.mnMountID = 0U;
				this.GetMagicSystem().SetMoveSpeed(250f);
			}
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0002A65C File Offset: 0x0002885C
		public bool IsMountState()
		{
			return this.mnMountID != 0U;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0002A67C File Offset: 0x0002887C
		public override bool CanPK(BaseObject obj, bool bGoCrime = true)
		{
			bool bCrime = true;
			PlayerObject playerObject = null;
			if (obj.type == 4)
			{
				playerObject = (obj as EudemonObject).GetOwnerPlay();
			}
			if (obj.type == 2)
			{
				playerObject = (obj as PlayerObject);
			}
			bool result;
			if (playerObject == null)
			{
				result = true;
			}
			else
			{
				byte pk_mode = this.GetBaseAttr().pk_mode;
				bool flag = false;
				if (pk_mode == 0)
				{
					flag = true;
				}
				if (pk_mode == 1)
				{
					result = false;
				}
				else
				{
					if (pk_mode == 3)
					{
						if (playerObject.GetPKSystem().IsPKing() || playerObject.GetPKSystem().GetNameType() == 3)
						{
							flag = true;
							bCrime = false;
						}
					}
					if (playerObject.GetTimerSystem().QueryStatus(14) != null)
					{
						flag = false;
					}
					if (flag && bGoCrime)
					{
						this.GetPKSystem().SetPKIng(true, bCrime);
					}
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x0002A790 File Offset: 0x00028990
		public void PlayAction(uint action_id)
		{
			if (!this.IsDie() && !base.IsLock())
			{
				this.SetCurrentAction(action_id);
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteUInt16(28);
				packetOut.WriteUInt16(1010);
				packetOut.WriteUInt32(0U);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteUInt32(23855267U);
				packetOut.WriteUInt32((uint)base.GetDir());
				packetOut.WriteUInt32(action_id);
				packetOut.WriteUInt32(9530U);
				byte[] data = packetOut.Flush();
				this.BroadcastBuffer(data, true);
			}
		}

		// Token: 0x060003AE RID: 942 RVA: 0x0002A840 File Offset: 0x00028A40
		public void Alive(bool isSitu = false)
		{
			this.GetTimerSystem().DeleteStatus(1);
			this.m_bGhost = false;
			this.ChangeAttribute(UserAttribute.STATUS, 0, true);
			this.ChangeAttribute(UserAttribute.LOOKFACE, this.GetLookFace(), true);
			this.ChangeAttribute(UserAttribute.LIFE, (int)this.GetBaseAttr().life_max, true);
			this.ChangeAttribute(UserAttribute.MANA, (int)this.GetBaseAttr().mana_max, true);
			if (!isSitu)
			{
				this.ReCallMap();
			}
			byte[] array = new byte[]
			{
				16,
				0,
				244,
				3,
				84,
				66,
				15,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			};
			base.GetGamePackKeyEx().EncodePacket(ref array, array.Length);
			base.SendData(array, false);
		}

		// Token: 0x060003AF RID: 943 RVA: 0x0002A910 File Offset: 0x00028B10
		public void Ptich()
		{
			NPCInfo currentNpcInfo = this.GetCurrentNpcInfo();
			if (currentNpcInfo != null)
			{
				short num = (short)(currentNpcInfo.id - 9101U);
				if (!PtichManager.Instance().PtichHasPlay((int)num))
				{
					this.SetCurrentPtichID((int)num);
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
					base.SendData(packetOut.Flush(), true);
				}
			}
		}

		// Token: 0x04000620 RID: 1568
		public PlayerAttribute mAttribute;

		// Token: 0x04000621 RID: 1569
		private int lastattacktime;

		// Token: 0x04000622 RID: 1570
		private uint face = 150001U;

		// Token: 0x04000623 RID: 1571
		private byte sex = 1;

		// Token: 0x04000624 RID: 1572
		public byte job = 10;

		// Token: 0x04000625 RID: 1573
		private Dictionary<byte, uint> mMenuLink;

		// Token: 0x04000626 RID: 1574
		private bool bIsExit;

		// Token: 0x04000627 RID: 1575
		private bool m_bGhost;

		// Token: 0x04000628 RID: 1576
		private int mnGhostTick;

		// Token: 0x04000629 RID: 1577
		private uint mnMountID = 0U;

		// Token: 0x0400062A RID: 1578
		private int mnFightSoul = 0;

		// Token: 0x0400062B RID: 1579
		private PlayerItem mItemSystem;

		private int mBatchHatchAppraisalAllowance;

		private PlayerWardrobe mWardrobeSystem;

		// Token: 0x0400062C RID: 1580
		private PlayerMagic mMagicSystem;

		// Token: 0x0400062D RID: 1581
		private PlayerEudemon mEudemonSystem;

		// Token: 0x0400062E RID: 1582
		private PlayerFight mFightSystem;

		// Token: 0x0400062F RID: 1583
		private PlayerFriend mFriendSystem;

		// Token: 0x04000630 RID: 1584
		private PlayerTrad mTradSystem;

		// Token: 0x04000631 RID: 1585
		private PlayerTimer mTimerSystem;

		// Token: 0x04000632 RID: 1586
		private PlayerLegion mLegionSystem;

		private PlayerFamily mFamilySystem;

		// Token: 0x04000633 RID: 1587
		private PlayerPK mPKSystem;

		// Token: 0x04000634 RID: 1588
		private Team mTeam;

		// Token: 0x04000635 RID: 1589
		private uint mTaskID = 0U;

		// Token: 0x04000636 RID: 1590
		private GUANGJUELEVEL mGuanJue = GUANGJUELEVEL.NORMAL;

		// Token: 0x04000637 RID: 1591
		private uint mnCurAction = 100U;

		// Token: 0x04000638 RID: 1592
		private BaseObject mTarget;

		// Token: 0x04000639 RID: 1593
		public TimeOut mSaveTime;

		// Token: 0x0400063A RID: 1594
		private BaseObject mZhaoHuanWuHuanObj;

		// Token: 0x0400063B RID: 1595
		private int mnCurrentRandom;

		// Token: 0x0400063C RID: 1596
		private int mPtichId = -1;

		// Token: 0x0400063D RID: 1597
		private int mRemotePtichId = 0;

		// Token: 0x0400063E RID: 1598
		private uint mUseItemEudemonId;

		// Token: 0x0400063F RID: 1599
		private TimeOut mTransmitTimeOut = null;

		// Token: 0x04000640 RID: 1600
		private bool mbTransmit = false;

		// Token: 0x04000641 RID: 1601
		private short mnDancingId = 0;

		// Token: 0x04000642 RID: 1602
		private int mnDancingTick = Environment.TickCount;

		// Token: 0x04000643 RID: 1603
		private NPCInfo mNpcInfo = null;

		// Token: 0x04000644 RID: 1604
		private List<HotkeyInfo> mListHotKey;
	}
}
