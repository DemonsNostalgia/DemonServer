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
	// Token: 0x02000013 RID: 19
	public class EudemonObject : BaseObject
	{
		// Token: 0x060000DB RID: 219 RVA: 0x0000B12C File Offset: 0x0000932C
		public RoleData_Eudemon GetAttr()
		{
			return this.mInfo;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000B144 File Offset: 0x00009344
		public RoleData_Eudemon GetEudemonInfo()
		{
			return this.mInfo;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000B15C File Offset: 0x0000935C
		public void SetEudemonInfo(RoleData_Eudemon info)
		{
			this.mInfo = info;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000B168 File Offset: 0x00009368
		public Dictionary<uint, PlayerObject> GetPlayObjectList()
		{
			return this.mPlayObject;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000B180 File Offset: 0x00009380
		public void SetMosterInfo(MonsterInfo info)
		{
			this.mMonsterInfo = info;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000B18C File Offset: 0x0000938C
		public MonsterInfo GetMonsterInfo()
		{
			return this.mMonsterInfo;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000B1A4 File Offset: 0x000093A4
		public EUDEMONSTATE GetState()
		{
			return this.mState;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000B1BC File Offset: 0x000093BC
		public void SetState(EUDEMONSTATE _state)
		{
			this.mState = _state;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000B1C6 File Offset: 0x000093C6
		public void SetRiding(bool v)
		{
			this.mbRiding = v;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000B1D0 File Offset: 0x000093D0
		public bool IsRiding()
		{
			return this.mbRiding;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000B1E8 File Offset: 0x000093E8
		public uint GetEudemonId()
		{
			uint result;
			if (this.mInfo == null)
			{
				result = 0U;
			}
			else
			{
				result = this.mInfo.itemid;
			}
			return result;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000B21C File Offset: 0x0000941C
		public EudemonObject(RoleData_Eudemon info, PlayerObject _play)
		{
			this.type = 4;
			this.mInfo = info;
			this.play = _play;
			this.mPlayObject = new Dictionary<uint, PlayerObject>();
			this.typeid = info.GetTypeID();
			this.mMonsterInfo = EudemonObject.GetMonsterInfo(this.play, info.itemid);
			this.mbIsCombo = false;
			this.mAttackSpeed = new TimeOut();
			this.mAttackSpeed.SetInterval(1000f);
			this.mAttackSpeed.Update();
			this.mMagicAttackSpeed = new List<TimeOut>();
			this.SetRiding(false);
			this.SetState(EUDEMONSTATE.NROMAL);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000B2D4 File Offset: 0x000094D4
		public override bool Run()
		{
			base.Run();
			bool result;
			if (Math.Abs((int)(this.play.GetCurrentX() - base.GetCurrentX())) > 10 || Math.Abs((int)(this.play.GetCurrentY() - base.GetCurrentY())) > 10)
			{
				this.FlyPlay();
				result = true;
			}
			else
			{
				if (base.IsLock())
				{
					if (!base.CheckLockTime())
					{
						base.UnLock(true);
					}
				}
				if (this.GetState() == EUDEMONSTATE.BATTLE)
				{
					if (this.IsDie() && !base.IsLock() && !this.GetAttr().bDie)
					{
						GameStruct.Action act = new GameStruct.Action(4, null);
						this.PushAction(act);
					}
				}
				if (this.GetState() == EUDEMONSTATE.FIT)
				{
					if (this.mbIsCombo && this.IsDie() && !this.play.IsLock())
					{
						this.mbIsCombo = false;
					}
					if (this.IsDie() && !this.mbIsCombo && !this.GetAttr().bDie)
					{
						GameStruct.Action act = new GameStruct.Action(4, null);
						this.PushAction(act);
					}
				}
				if (this.GetState() == EUDEMONSTATE.FIT)
				{
					this.SetPoint(this.play.GetCurrentX(), this.play.GetCurrentY());
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000B44C File Offset: 0x0000964C
		public bool CheckMagicAttackSpeed(ushort magicid, byte magiclv)
		{
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo((uint)magicid, magiclv);
			bool result;
			if (magicTypeInfo.delay_ms == 0U)
			{
				result = true;
			}
			else if (magicTypeInfo == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < this.mMagicAttackSpeed.Count; i++)
				{
					TimeOut timeOut = this.mMagicAttackSpeed[i];
					if ((ushort)timeOut.GetObject() == magicid)
					{
						if (timeOut.ToNextTime())
						{
							flag = true;
							break;
						}
						flag2 = true;
					}
				}
				for (int i = 0; i < this.mMagicAttackSpeed.Count; i++)
				{
					this.mMagicAttackSpeed[i].Update();
				}
				if (!flag && !flag2)
				{
					TimeOut timeOut = new TimeOut();
					timeOut.SetInterval(magicTypeInfo.delay_ms);
					timeOut.SetObject(magicid);
					timeOut.Update();
					this.mMagicAttackSpeed.Add(timeOut);
					result = true;
				}
				else
				{
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000B584 File Offset: 0x00009784
		public override bool IsDie()
		{
			return this.GetAttr().life == 0;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000B5A4 File Offset: 0x000097A4
		public override void ClearThis()
		{
			byte[] buffer = new MsgClearObjectInfo
			{
				id = base.GetTypeId()
			}.GetBuffer();
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2 && obj.GetGameSession() != null)
				{
					if (obj.GetGameID() != this.play.GetGameID())
					{
						BaseMsg baseMsg = new BaseMsg();
						baseMsg.Create(buffer, obj.GetGamePackKeyEx());
						obj.SendData(baseMsg.GetBuffer(), false);
					}
				}
			}
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (obj.GetVisibleList().ContainsKey(base.GetGameID()))
					{
						obj.GetVisibleList().Remove(base.GetGameID());
					}
				}
			}
			base.GetVisibleList().Clear();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000B720 File Offset: 0x00009920
		public static MonsterInfo GetMonsterInfo(PlayerObject _play, uint _item_id)
		{
			RoleItemInfo roleItemInfo = _play.GetItemSystem().FindItem(_item_id);
			MonsterInfo result;
			if (roleItemInfo == null)
			{
				Log.Instance().WriteLog("Failed to deploy Eudemon; item ID was not found: " + _item_id.ToString());
				result = null;
			}
			else
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog("Failed to deploy Eudemon; item ID was not found (code 1): " + roleItemInfo.itemid.ToString());
					result = null;
				}
				else
				{
					MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(itemTypeInfo.monster_type);
					if (monsterInfo == null)
					{
						Log.Instance().WriteLog("Failed to deploy Eudemon; monster ID was not found (code 1): " + itemTypeInfo.monster_type.ToString());
						result = null;
					}
					else
					{
						result = monsterInfo;
					}
				}
			}
			return result;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000B7FC File Offset: 0x000099FC
		public void Battle()
		{
			if (this.mMonsterInfo == null)
			{
				Log.Instance().WriteLog("Failed to deploy Eudemon; monster ID was not found (code 1): ");
			}
			else
			{
				this.mGameMap = this.play.GetGameMap();
				base.GetGameMap().AddObject(this, null);
				short currentX = this.play.GetCurrentX();
				short currentY = this.play.GetCurrentY();
				this.SetPoint(currentX, currentY);
				this.RefreshVisibleObject();
				PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
				packetOut.WriteUInt16(24);
				packetOut.WriteUInt16(2035);
				packetOut.WriteUInt32(this.mInfo.GetTypeID());
				packetOut.WriteUInt32(this.mMonsterInfo.id);
				packetOut.WriteInt32(1);
				packetOut.WriteInt16(base.GetCurrentX());
				packetOut.WriteInt16(base.GetCurrentY());
				packetOut.WriteUInt32(this.mInfo.GetTypeID());
				this.play.SendData(packetOut.Flush(), false);
				this.play.AddVisibleObject(this, true);
				this.SetState(EUDEMONSTATE.BATTLE);
				this.SendEudemonInfo(null);
				this.SendMagicInfo();
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000B930 File Offset: 0x00009B30
		public void DeleteMagicInfo(ushort magicid)
		{
			int i = 0;
			while (i < this.mInfo.mListMagicInfo.Count)
			{
				if (this.mInfo.mListMagicInfo[i].magicid == (uint)magicid)
				{
					if (this.mInfo.mListMagicInfo[i].id == 0)
					{
						this.mInfo.mListMagicInfo.RemoveAt(i);
						break;
					}
					this.mInfo.mListMagicInfo[i].id = -1;
					break;
				}
				else
				{
					i++;
				}
			}
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteInt16(28);
			packetOut.WriteInt16(1010);
			packetOut.WriteInt32(Environment.TickCount);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt16(base.GetCurrentX());
			packetOut.WriteInt16(base.GetCurrentY());
			packetOut.WriteInt32(0);
			packetOut.WriteUInt16(magicid);
			packetOut.WriteUInt16(0);
			packetOut.WriteInt32(9585);
			this.play.SendData(packetOut.Flush(), true);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000BA58 File Offset: 0x00009C58
		public bool AddMagicInfo(ushort magicid, byte magiclv = 0, uint exp = 0U)
		{
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo((uint)magicid, magiclv);
			bool result;
			if (magicTypeInfo == null)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				for (int i = 0; i < this.mInfo.mListMagicInfo.Count; i++)
				{
					if (this.mInfo.mListMagicInfo[i].magicid == (uint)magicid)
					{
						this.mInfo.mListMagicInfo[i].level = magiclv;
						this.mInfo.mListMagicInfo[i].exp = exp;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					MagicInfo magicInfo = new MagicInfo();
					magicInfo.id = 0;
					magicInfo.level = magiclv;
					magicInfo.magicid = (uint)magicid;
					magicInfo.exp = exp;
					magicInfo.ownerid = (int)this.mInfo.id;
					this.mInfo.mListMagicInfo.Add(magicInfo);
				}
				MsgMagicInfo msgMagicInfo = new MsgMagicInfo();
				msgMagicInfo.id = base.GetTypeId();
				msgMagicInfo.magicid = magicid;
				msgMagicInfo.level = (ushort)magiclv;
				msgMagicInfo.exp = exp;
				this.play.SendData(msgMagicInfo.GetBuffer(), true);
				result = true;
			}
			return result;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000BBC0 File Offset: 0x00009DC0
		public void SendEudemonInfo(PlayerObject _play = null)
		{
			if (this.mMonsterInfo != null)
			{
				if (this.GetState() != EUDEMONSTATE.FIT && this.GetState() != EUDEMONSTATE.NROMAL)
				{
					MsgEudemonBattleInfo msgEudemonBattleInfo = new MsgEudemonBattleInfo();
					msgEudemonBattleInfo.id = base.GetTypeId();
					RoleItemInfo roleItemInfo = this.play.GetItemSystem().FindItem(this.GetEudemonInfo().itemid);
					if (roleItemInfo != null)
					{
						msgEudemonBattleInfo.lookface = this.mMonsterInfo.lookface;
						msgEudemonBattleInfo.name = roleItemInfo.forgename;
						msgEudemonBattleInfo.monsterid = this.mMonsterInfo.id;
						msgEudemonBattleInfo.play_id = this.play.GetTypeId();
						msgEudemonBattleInfo.life = this.mInfo.life;
						msgEudemonBattleInfo.life_max = this.mInfo.life;
						msgEudemonBattleInfo.x = base.GetCurrentX();
						msgEudemonBattleInfo.y = base.GetCurrentY();
						msgEudemonBattleInfo.dir = (short)this.play.GetDir();
						msgEudemonBattleInfo.wuxing = (byte)this.mInfo.wuxing;
						msgEudemonBattleInfo.wuxing = 5;
						if (this.mInfo.quality == 0)
						{
							msgEudemonBattleInfo.param4 = 0;
						}
						else
						{
							msgEudemonBattleInfo.param4 = 69888;
						}
						int star = this.mInfo.quality / 100;
						msgEudemonBattleInfo.star = star;
						byte[] array = new byte[]
						{
							44,
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
							60,
							89,
							16,
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
							0
						};
						Buffer.BlockCopy(array, 0, msgEudemonBattleInfo.param2, 0, array.Length);
						if (_play != null)
						{
							_play.SendData(msgEudemonBattleInfo.GetBuffer(), true);
						}
						else
						{
							this.BrocatBuffer(msgEudemonBattleInfo.GetBuffer());
						}
						PacketOut packetOut;
						if (this.GetAttr().bDie)
						{
							packetOut = new PacketOut(null);
							packetOut.WriteInt16(20);
							packetOut.WriteInt16(1017);
							packetOut.WriteUInt32(base.GetTypeId());
							packetOut.WriteInt32(1);
							packetOut.WriteInt32(26);
							packetOut.WriteInt32(6);
							this.BrocatBuffer(packetOut.Flush());
						}
						byte[] array2 = new byte[16];
						array2[0] = 132;
						byte[] v = array2;
						packetOut = new PacketOut(null);
						packetOut.WriteInt16(28);
						packetOut.WriteInt16(1009);
						packetOut.WriteUInt32(this.play.GetTypeId());
						packetOut.WriteUInt32(base.GetTypeId());
						packetOut.WriteBuff(v);
						this.BrocatBuffer(packetOut.Flush());
					}
				}
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000BE60 File Offset: 0x0000A060
		public void SendPlayRefreshInfo(PlayerObject play)
		{
			if (play.GetGameSession() != null)
			{
				if (this.mMonsterInfo != null)
				{
					MsgEudemonBattleInfo msgEudemonBattleInfo = new MsgEudemonBattleInfo();
					msgEudemonBattleInfo.Create(null, play.GetGamePackKeyEx());
					msgEudemonBattleInfo.id = this.mInfo.GetTypeID();
					msgEudemonBattleInfo.lookface = this.mMonsterInfo.lookface;
					msgEudemonBattleInfo.name = this.mInfo.name;
					msgEudemonBattleInfo.monsterid = this.mMonsterInfo.id;
					msgEudemonBattleInfo.x = play.GetCurrentX();
					msgEudemonBattleInfo.y = play.GetCurrentY();
					msgEudemonBattleInfo.dir = (short)play.GetDir();
					play.SendData(msgEudemonBattleInfo.GetBuffer(), false);
					MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
					msgEudemonInfo.Create(null, play.GetGamePackKeyEx());
					msgEudemonInfo.id = this.mInfo.GetTypeID();
					msgEudemonInfo.AddAttribute(EudemonAttribute.Life, this.mInfo.life);
					msgEudemonInfo.AddAttribute(EudemonAttribute.Life_Max, this.mInfo.life);
					play.SendData(msgEudemonInfo.GetBuffer(), false);
				}
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000BF7C File Offset: 0x0000A17C
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in base.GetGameMap().GetAllObject().Values)
			{
				if (baseObject.GetGameID() != base.GetGameID())
				{
					if (baseObject.type != 1)
					{
						if (base.GetPoint().CheckVisualDistance(baseObject.GetCurrentX(), baseObject.GetCurrentY(), 15))
						{
							base.AddVisibleObject(baseObject, false);
						}
						else if (this.mVisibleList.ContainsKey(baseObject.GetGameID()))
						{
							this.mVisibleList.Remove(baseObject.GetGameID());
						}
					}
				}
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000C06C File Offset: 0x0000A26C
		public void FlyPlay()
		{
			this.ClearThis();
			short x = (short)(this.play.GetCurrentX() - 2);
			short y = (short)(this.play.GetCurrentY() - 2);
			this.SetPoint(x, y);
			this.SendEudemonInfo(null);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000C0B0 File Offset: 0x0000A2B0
		public bool Move(MsgMoveInfo move)
		{
			byte b = (byte)(move.dir % 8);
			base.SetDir(b);
			short num = base.GetCurrentX();
			short num2 = base.GetCurrentY();
			bool result;
			if (Math.Abs((int)(this.play.GetCurrentX() - base.GetCurrentX())) > 10 || Math.Abs((int)(this.play.GetCurrentY() - base.GetCurrentY())) > 10)
			{
				num = this.play.GetCurrentX();
				num2 = this.play.GetCurrentY();
				this.SetPoint(num, num2);
				this.SendEudemonInfo(null);
				result = false;
			}
			else
			{
				num += DIR._DELTA_X[(int)b];
				num2 += DIR._DELTA_Y[(int)b];
				if (!this.mGameMap.CanMove(num, num2))
				{
					result = false;
				}
				else
				{
					bool flag = false;
					if (move.ucMode >= 20 && move.ucMode <= 27)
					{
						num += DIR._DELTA_X[(int)(move.ucMode - 20)];
						num2 += DIR._DELTA_Y[(int)(move.ucMode - 20)];
						flag = true;
					}
					GameStruct.Action action = new GameStruct.Action(2, null);
					if (flag)
					{
						action.AddObject(move.ucMode);
					}
					this.SetPoint(num, num2);
					this.PushAction(action);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000C210 File Offset: 0x0000A410
		protected override void ProcessAction_Move(GameStruct.Action act)
		{
			byte ucMode = 1;
			if (act.GetObjectCount() > 0)
			{
				ucMode = (byte)act.GetObject(0);
			}
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (!obj.GetPoint().CheckVisualDistance(base.GetCurrentX(), base.GetCurrentY(), 14))
					{
						MsgClearObjectInfo msgClearObjectInfo = new MsgClearObjectInfo();
						msgClearObjectInfo.id = base.GetTypeId();
						(obj as PlayerObject).SendData(msgClearObjectInfo.GetBuffer(), true);
						obj.GetVisibleList().Remove(base.GetGameID());
					}
				}
			}
			this.RefreshVisibleObject();
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (!obj.GetVisibleList().ContainsKey(base.GetGameID()))
					{
						obj.AddVisibleObject(this, true);
						this.SendEudemonInfo(null);
					}
				}
			}
			this.BrocatBuffer(new MsgMoveInfo
			{
				id = base.GetTypeId(),
				x = base.GetCurrentX(),
				y = base.GetCurrentY(),
				dir = base.GetDir(),
				ucMode = ucMode
			}.GetBuffer());
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000C3EC File Offset: 0x0000A5EC
		public void SendMoveInfo(BaseObject obj, byte runValue)
		{
			MsgMoveInfo msgMoveInfo = new MsgMoveInfo();
			msgMoveInfo.Create(null, obj.GetGamePackKeyEx());
			msgMoveInfo.id = base.GetTypeId();
			msgMoveInfo.x = base.GetCurrentX();
			msgMoveInfo.y = base.GetCurrentY();
			msgMoveInfo.dir = base.GetDir();
			msgMoveInfo.ucMode = runValue;
			obj.SendData(msgMoveInfo.GetBuffer(), false);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000C454 File Offset: 0x0000A654
		public void ReCall()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16(28);
			packetOut.WriteUInt16(1009);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt32(32);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(0);
			this.BrocatBuffer(packetOut.Flush());
			MsgEudemonTag msgEudemonTag = new MsgEudemonTag();
			msgEudemonTag.playerid = this.play.GetTypeId();
			msgEudemonTag.eudemonid = base.GetTypeId();
			msgEudemonTag.SetReCallTag();
			this.BrocatBuffer(msgEudemonTag.GetBuffer());
			this.SetState(EUDEMONSTATE.NROMAL);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000C508 File Offset: 0x0000A708
		public bool IsHaveMagic(ushort magicid)
		{
			for (int i = 0; i < this.GetEudemonInfo().mListMagicInfo.Count; i++)
			{
				MagicInfo magicInfo = this.GetEudemonInfo().mListMagicInfo[i];
				if (magicInfo.magicid == (uint)magicid)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000C564 File Offset: 0x0000A764
		public ushort GetMagicLevel(ushort magicid)
		{
			for (int i = 0; i < this.GetEudemonInfo().mListMagicInfo.Count; i++)
			{
				MagicInfo magicInfo = this.GetEudemonInfo().mListMagicInfo[i];
				if (magicInfo.magicid == (uint)magicid)
				{
					return (ushort)magicInfo.level;
				}
			}
			return 0;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000C5C4 File Offset: 0x0000A7C4
		public void MagicAttack(MsgAttackInfo info)
		{
			if (this.IsHaveMagic((ushort)info.usType))
			{
				MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(info.usType, 0);
				if (magicTypeInfo != null)
				{
					ushort magicLevel = this.GetMagicLevel((ushort)info.usType);
					if (this.CheckMagicAttackSpeed((ushort)info.usType, (byte)magicLevel))
					{
						switch (magicTypeInfo.sort)
						{
						case 1:
						{
							BaseObject baseObject = base.GetGameMap().FindObjectForID(info.idTarget);
							if (baseObject != null)
							{
								if (!baseObject.IsDie())
								{
									if (!baseObject.IsLock())
									{
										byte dirByPos = DIR.GetDirByPos(base.GetCurrentX(), base.GetCurrentY(), baseObject.GetCurrentX(), baseObject.GetCurrentY());
										base.SetDir(dirByPos);
										if ((long)Math.Abs((int)(base.GetCurrentX() - baseObject.GetCurrentY())) <= (long)((ulong)magicTypeInfo.distance) || (long)Math.Abs((int)(base.GetCurrentY() - baseObject.GetCurrentY())) <= (long)((ulong)magicTypeInfo.distance))
										{
											if (this.play.CanPK(baseObject, true))
											{
												uint num = BattleSystem.AdjustDamage(this, baseObject, true);
												if (num <= 0U)
												{
													num = 1U;
												}
												this.BrocatBuffer(new MsgMonsterMagicInjuredInfo
												{
													time = Environment.TickCount,
													roleid = base.GetTypeId(),
													role_x = base.GetCurrentX(),
													role_y = base.GetCurrentY(),
													monsterid = baseObject.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer());
												MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
												msgGroupMagicAttackInfo.SetSigleAttack(baseObject.GetTypeId());
												msgGroupMagicAttackInfo.nID = base.GetTypeId();
												msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
												msgGroupMagicAttackInfo.nMagicLv = magicLevel;
												msgGroupMagicAttackInfo.bDir = base.GetDir();
												msgGroupMagicAttackInfo.AddObject(baseObject.GetTypeId(), (int)num);
												this.BrocatBuffer(msgGroupMagicAttackInfo.GetBuffer());
												baseObject.Injured(this, num, info);
											}
										}
									}
								}
							}
							break;
						}
						case 4:
						{
							byte dirByPos = DIR.GetDirByPos(base.GetCurrentX(), base.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
							base.SetDir(dirByPos);
							MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
							msgGroupMagicAttackInfo.nID = base.GetTypeId();
							msgGroupMagicAttackInfo.nX = base.GetCurrentX();
							msgGroupMagicAttackInfo.nY = base.GetCurrentY();
							msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
							msgGroupMagicAttackInfo.nMagicLv = magicLevel;
							msgGroupMagicAttackInfo.bDir = dirByPos;
							List<BaseObject> list = this.GetFanVisibleObj(info);
							if (list != null)
							{
								for (int i = 0; i < list.Count; i++)
								{
									uint num = BattleSystem.AdjustDamage(this, list[i], true);
									if (num <= 0U)
									{
										num = 1U;
									}
									if (list[i].type == 3 && magicTypeInfo.use_xp > 0U)
									{
										num *= 10U;
									}
									list[i].Injured(this, num, info);
									msgGroupMagicAttackInfo.AddObject(list[i].GetTypeId(), (int)num);
								}
							}
							byte[] buffer = msgGroupMagicAttackInfo.GetBuffer();
							this.BrocatBuffer(buffer);
							break;
						}
						case 5:
						{
							byte dirByPos = DIR.GetDirByPos(base.GetCurrentX(), base.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
							this.play.SetDir(dirByPos);
							MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
							msgGroupMagicAttackInfo.nID = base.GetTypeId();
							msgGroupMagicAttackInfo.nX = base.GetCurrentX();
							msgGroupMagicAttackInfo.nY = base.GetCurrentY();
							msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
							msgGroupMagicAttackInfo.nMagicLv = magicLevel;
							msgGroupMagicAttackInfo.bDir = base.GetDir();
							List<BaseObject> list = this.GetBombVisibleObj(info);
							if (list != null)
							{
								for (int i = 0; i < list.Count; i++)
								{
									uint num = BattleSystem.AdjustDamage(this, list[i], true);
									list[i].Injured(this, num, info);
									msgGroupMagicAttackInfo.AddObject(list[i].GetTypeId(), (int)num);
								}
							}
							byte[] buffer = msgGroupMagicAttackInfo.GetBuffer();
							this.BrocatBuffer(buffer);
							break;
						}
						}
					}
				}
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000CA78 File Offset: 0x0000AC78
		private List<BaseObject> GetBombVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int range = (int)magicTypeInfo.range;
				foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.play.GetFightSystem().IsAddMagicVisibleObj(obj))
					{
						if (base.GetPoint().CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), range))
						{
							list.Add(obj);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000CB60 File Offset: 0x0000AD60
		private List<BaseObject> GetFanVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int num = (int)(magicTypeInfo.range + 2U);
				int num2 = num * 2 + 1;
				int width = (int)magicTypeInfo.width;
				foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.play.GetFightSystem().IsAddMagicVisibleObj(obj))
					{
						Point point = base.GetPoint();
						Point point2 = new Point();
						point2.x = (short)magicinfo.usPosX;
						point2.y = (short)magicinfo.usPosY;
						if (base.GetPoint().CheckFanDistance(obj.GetPoint(), point2, num))
						{
							list.Add(obj);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000CC88 File Offset: 0x0000AE88
		public void Attack(MsgAttackInfo info)
		{
			if (this.mAttackSpeed.ToNextTime())
			{
				BaseObject baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
				if (baseObject != null)
				{
					if (this.mMonsterInfo != null)
					{
						if (!baseObject.IsDie())
						{
							if (!baseObject.IsLock())
							{
								if (!this.mInfo.bDie)
								{
									if (Math.Abs((int)(base.GetCurrentX() - baseObject.GetCurrentX())) <= (int)this.mMonsterInfo.range || Math.Abs((int)(base.GetCurrentY() - baseObject.GetCurrentY())) <= (int)this.mMonsterInfo.range)
									{
										if (baseObject.type == 2)
										{
											if (!this.play.CanPK(baseObject, true))
											{
												return;
											}
										}
										if (baseObject.type == 4)
										{
											if (!this.play.CanPK((baseObject as EudemonObject).GetOwnerPlay(), true))
											{
												return;
											}
										}
										uint num = 0U;
										switch (this.mMonsterInfo.eudemon_type)
										{
										case 1:
										case 5:
										{
											num = BattleSystem.AdjustDamage(this, baseObject, false);
											byte[] buffer = new MsgMonsterInjuredInfo
											{
												roleid = base.GetTypeId(),
												role_x = base.GetCurrentX(),
												role_y = base.GetCurrentY(),
												injuredvalue = num,
												monsterid = baseObject.GetTypeId(),
												tag = 2U
											}.GetBuffer();
											this.BrocatBuffer(buffer);
											break;
										}
										case 2:
										case 4:
										{
											num = BattleSystem.AdjustDamage(this, baseObject, true);
											if (num == 0U)
											{
												num = 1U;
											}
											MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
											msgGroupMagicAttackInfo.SetSigleAttack(baseObject.GetTypeId());
											msgGroupMagicAttackInfo.nID = base.GetTypeId();
											msgGroupMagicAttackInfo.nMagicID = 5000;
											msgGroupMagicAttackInfo.nMagicLv = 0;
											msgGroupMagicAttackInfo.bDir = base.GetDir();
											msgGroupMagicAttackInfo.AddObject(baseObject.GetTypeId(), (int)num);
											this.BrocatBuffer(msgGroupMagicAttackInfo.GetBuffer());
											break;
										}
										}
										baseObject.Injured(this, num, info);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000CEF8 File Offset: 0x0000B0F8
		public PlayerObject GetOwnerPlay()
		{
			return this.play;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000CF10 File Offset: 0x0000B110
		public override void CalcAttribute()
		{
			this.GetAttr().life = (int)((float)(this.GetAttr().init_life + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().life_grow_rate);
			this.GetAttr().life_max = this.GetAttr().life;
			this.GetAttr().atk_min = (int)((float)(this.GetAttr().init_atk_min + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().phyatk_grow_rate);
			this.GetAttr().atk_max = (int)((float)(this.GetAttr().init_atk_max + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().phyatk_grow_rate_max);
			this.GetAttr().magicatk_min = (int)((float)(this.GetAttr().init_magicatk_min + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().magicatk_grow_rate);
			this.GetAttr().magicatk_max = (int)((float)(this.GetAttr().init_magicatk_max + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().magicatk_grow_rate_max);
			this.GetAttr().defense = (int)((float)(this.GetAttr().init_defense + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().defense_grow_rate);
			this.GetAttr().magicdef = (int)((float)(this.GetAttr().init_magicdef + this.GetAttr().quality / 1000 + (int)this.GetAttr().level) * this.GetAttr().magicdef_grow_rate);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000D104 File Offset: 0x0000B304
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			this.GetAttr().life = 0;
			this.mInfo.bDie = true;
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = base.GetTypeId();
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life, 0);
			this.play.SendData(msgEudemonInfo.GetBuffer(), true);
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16(24);
			packetOut.WriteUInt16(2037);
			packetOut.WriteUInt32(1U);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt32(1);
			packetOut.WriteInt32(83);
			packetOut.WriteInt32(45);
			this.play.SendData(packetOut.Flush(), true);
			packetOut = new PacketOut(null);
			packetOut.WriteInt16(20);
			packetOut.WriteInt16(1017);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt32(1);
			packetOut.WriteInt32(35);
			packetOut.WriteInt32(45);
			this.play.SendData(packetOut.Flush(), true);
			packetOut = new PacketOut(null);
			packetOut.WriteUInt16(24);
			packetOut.WriteUInt16(2037);
			packetOut.WriteUInt32(1U);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt32(1);
			packetOut.WriteInt32(8);
			packetOut.WriteInt32(149);
			this.play.SendData(packetOut.Flush(), true);
			if (this.GetState() == EUDEMONSTATE.FIT)
			{
				this.play.GetEudemonSystem().Eudemon_BreakUp(base.GetTypeId());
				this.play.GetEudemonSystem().Eudemon_Battle(base.GetTypeId());
			}
			if (this.GetState() == EUDEMONSTATE.BATTLE)
			{
				this.SendEudemonInfo(null);
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000D2C8 File Offset: 0x0000B4C8
		public override void Injured(BaseObject obj, uint value, MsgAttackInfo info)
		{
			this.mbIsCombo = this.play.GetFightSystem().IsComboMagic(info.usType);
			this.GetAttr().life -= (int)value;
			if (this.GetAttr().life < 0)
			{
				this.GetAttr().life = 0;
			}
			if (!this.mbIsCombo && this.GetAttr().life <= 0)
			{
				GameStruct.Action act = new GameStruct.Action(4, null);
				this.PushAction(act);
			}
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = base.GetTypeId();
			msgEudemonInfo.AddAttribute(EudemonAttribute.Life, this.GetAttr().life);
			this.BrocatBuffer(msgEudemonInfo.GetBuffer());
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000D38A File Offset: 0x0000B58A
		protected override void ProcessAction_Injured(GameStruct.Action act)
		{
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000D390 File Offset: 0x0000B590
		public void SendMagicInfo()
		{
			for (int i = 0; i < this.mInfo.mListMagicInfo.Count; i++)
			{
				if (this.mInfo.mListMagicInfo[i].id == -1)
				{
					break;
				}
				MsgMagicInfo msgMagicInfo = new MsgMagicInfo();
				msgMagicInfo.id = base.GetTypeId();
				msgMagicInfo.magicid = (ushort)this.mInfo.mListMagicInfo[i].magicid;
				msgMagicInfo.exp = this.mInfo.mListMagicInfo[i].exp;
				this.play.SendData(msgMagicInfo.GetBuffer(), true);
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000D448 File Offset: 0x0000B648
		public void ChangeAttribute(EudemonAttribute type, int value, bool isBrocat = true)
		{
			RoleData_Eudemon roleData_Eudemon = this.play.GetEudemonSystem().FindEudemon(base.GetTypeId());
			if (roleData_Eudemon != null)
			{
				int value2 = value;
				if (type == EudemonAttribute.Level)
				{
					RoleData_Eudemon roleData_Eudemon2 = roleData_Eudemon;
					roleData_Eudemon2.level += (short)value;
					this.SetEudemonInfo(roleData_Eudemon);
					this.CalcAttribute();
					PacketOut packetOut = new PacketOut(null);
					packetOut.WriteInt16(28);
					packetOut.WriteInt16(1010);
					packetOut.WriteInt32(Environment.TickCount);
					packetOut.WriteUInt32(base.GetTypeId());
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(1);
					packetOut.WriteInt32(9550);
					this.BrocatBuffer(packetOut.Flush());
					value2 = (int)this.GetAttr().level;
				}
				MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
				msgEudemonInfo.id = base.GetTypeId();
				msgEudemonInfo.AddAttribute(type, value2);
				if (isBrocat)
				{
					this.BrocatBuffer(msgEudemonInfo.GetBuffer());
				}
				else
				{
					this.play.SendData(msgEudemonInfo.GetBuffer(), true);
				}
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000D570 File Offset: 0x0000B770
		public void AddExp(int nExp)
		{
			this.GetAttr().exp += nExp;
			bool flag = false;
			for (;;)
			{
				LevelExp levelExp = ConfigManager.Instance().GetLevelExp(1U, (byte)this.GetAttr().level);
				if (levelExp == null)
				{
					break;
				}
				if (this.GetAttr().exp < (int)levelExp.exp)
				{
					break;
				}
				this.GetAttr().exp -= (int)levelExp.exp;
				RoleData_Eudemon attr = this.GetAttr();
				attr.level += 1;
				flag = true;
			}
			if (flag)
			{
				PacketOut packetOut = new PacketOut(null);
				packetOut.WriteInt16(28);
				packetOut.WriteInt16(1010);
				packetOut.WriteInt32(Environment.TickCount);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(1);
				packetOut.WriteInt32(9550);
				this.BrocatBuffer(packetOut.Flush());
			}
			MsgEudemonInfo msgEudemonInfo = new MsgEudemonInfo();
			msgEudemonInfo.id = base.GetTypeId();
			msgEudemonInfo.AddAttribute(EudemonAttribute.Exp, this.GetAttr().exp);
			if (flag)
			{
				msgEudemonInfo.AddAttribute(EudemonAttribute.Level, (int)this.GetAttr().level);
				this.CalcAttribute();
				this.play.GetEudemonSystem().SendEudemonInfo(this.GetEudemonInfo(), true, true);
			}
			this.play.SendData(msgEudemonInfo.GetBuffer(), true);
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000D700 File Offset: 0x0000B900
		public override int GetDefense()
		{
			return this.GetAttr().defense;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000D720 File Offset: 0x0000B920
		public override byte GetLevel()
		{
			return (byte)this.GetAttr().level;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000D740 File Offset: 0x0000B940
		public override int GetLuck()
		{
			return this.GetAttr().luck;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000D760 File Offset: 0x0000B960
		public override int GetMagicAck()
		{
			return this.GetAttr().magicatk_min;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000D780 File Offset: 0x0000B980
		public override int GetMaxMagixAck()
		{
			return this.GetAttr().magicatk_max;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000D7A0 File Offset: 0x0000B9A0
		public override int GetMagicDefense()
		{
			return this.GetAttr().magicdef;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000D7C0 File Offset: 0x0000B9C0
		public override int GetMaxAck()
		{
			return this.GetAttr().atk_max;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000D7E0 File Offset: 0x0000B9E0
		public override int GetMinAck()
		{
			return this.GetAttr().atk_min;
		}

		// Token: 0x0400007A RID: 122
		private RoleData_Eudemon mInfo = null;

		// Token: 0x0400007B RID: 123
		private PlayerObject play;

		// Token: 0x0400007C RID: 124
		private Dictionary<uint, PlayerObject> mPlayObject;

		// Token: 0x0400007D RID: 125
		private MonsterInfo mMonsterInfo = null;

		// Token: 0x0400007E RID: 126
		private EUDEMONSTATE mState = EUDEMONSTATE.NROMAL;

		// Token: 0x0400007F RID: 127
		private bool mbIsCombo;

		// Token: 0x04000080 RID: 128
		private TimeOut mAttackSpeed;

		// Token: 0x04000081 RID: 129
		private List<TimeOut> mMagicAttackSpeed;

		// Token: 0x04000082 RID: 130
		private bool mbRiding;
	}
}
