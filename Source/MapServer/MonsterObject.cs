using System;
using System.Collections.Generic;
using AI;
using GameBase.Core;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000003 RID: 3
	public class MonsterObject : BaseObject
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00002B67 File Offset: 0x00000D67
		public void SetRebirthTime(uint nTime)
		{
			this.mRebirthTime = nTime;
			this.mAliveTime.SetInterval(this.mRebirthTime);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002B88 File Offset: 0x00000D88
		public MonsterObject(uint _id, int nAi_Id, short x, short y, bool isCreateTypeId = true)
		{
			this.mTarget = null;
			this.id = _id;
			this.type = 3;
			this.attr = new MonterAttribute();
			this.mRebirthTime = 1000U;
			if (isCreateTypeId)
			{
				this.typeid = IDManager.CreateTypeId(this.type);
			}
			this.mInitPoint = new Point();
			this.mInitPoint.x = x;
			this.mInitPoint.y = y;
			this.m_Ai = this.CreateAi(nAi_Id);
			this.mnDieMagicTick = Environment.TickCount;
			this.mDieMagicInfo = null;
			this.attr.life = (this.attr.life_max = 0);
			this.mAliveTime = new TimeOut();
			this.mAliveTime.SetInterval(this.mRebirthTime);
			this.SetPoint(x, y);
			this.Alive(true);
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002C74 File Offset: 0x00000E74
		public BaseAI GetAi()
		{
			return this.m_Ai;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002C8C File Offset: 0x00000E8C
		public void SetAi(BaseAI _ai)
		{
			this.m_Ai = _ai;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002C98 File Offset: 0x00000E98
		public void Alive(bool init = true)
		{
			this.mInfo = ConfigManager.Instance().GetMonsterInfo(this.id);
			this.Name = this.mInfo.name;
			this.attr.life = (this.attr.life_max = this.mInfo.life);
			if (!init)
			{
				this.SetPoint(this.mInitPoint.x, this.mInitPoint.y);
			}
			base.SetLastWalkTime(Environment.TickCount);
			this.LastDieTime = Environment.TickCount;
			base.SetWalkTime(IRandom.Random(1000, 60000));
			if (!init)
			{
				this.Walk(8);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002D54 File Offset: 0x00000F54
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.GetGameID() != base.GetGameID())
				{
					if (baseObject.type == 2 || baseObject.type == 5 || baseObject.type == 7 || baseObject.type == 3 || baseObject.type == 9)
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

		// Token: 0x06000040 RID: 64 RVA: 0x00002E70 File Offset: 0x00001070
		public override bool Run()
		{
			bool result;
			if (this.GetAi() == null)
			{
				result = true;
			}
			else
			{
				base.Run();
				if (this.mDieMagicInfo != null && Environment.TickCount - this.mnDieMagicTick >= 500)
				{
					GameStruct.Action action = new GameStruct.Action(4, null);
					action.AddObject(this.mDieMagicInfo.GetObject(0));
					action.AddObject(this.mDieMagicInfo.GetObject(1));
					this.PushAction(action);
					this.mDieMagicInfo = null;
					result = true;
				}
				else if (this.mDieMagicInfo != null)
				{
					result = true;
				}
				else
				{
					this.GetAi().Run();
					if (base.IsLock())
					{
						if (!base.CheckLockTime())
						{
							base.UnLock(false);
							if (this.IsDie())
							{
								GameStruct.Action action = new GameStruct.Action(4, null);
								action.AddObject(this.mTarget);
								action.AddObject((uint)this.mTarget.GetMinAck());
								this.PushAction(action);
								this.LastDieTime = Environment.TickCount;
							}
						}
					}
					if (this.IsDie() && !base.IsLock())
					{
						if (!this.IsClear() && Environment.TickCount - this.LastDieTime > 3000)
						{
							this.ClearThis();
						}
					}
					if (this.IsClear() && this.IsDie() && this.mAliveTime.ToNextTime())
					{
						this.Alive(false);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003014 File Offset: 0x00001214
		public override bool IsDie()
		{
			return this.attr.life == 0;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003038 File Offset: 0x00001238
		public bool IsClear()
		{
			return this.LastDieTime == -1;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003058 File Offset: 0x00001258
		public override void Walk(byte dir, short x, short y)
		{
			base.Walk(dir, x, y);
			GameStruct.Action act = new GameStruct.Action(2, null);
			base.SetLastWalkTime(Environment.TickCount);
			base.SetWalkTime(IRandom.Random(1000, 60000));
			this.PushAction(act);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000030A4 File Offset: 0x000012A4
		public override void Walk(byte dir)
		{
			base.Walk(dir);
			GameStruct.Action act = new GameStruct.Action(2, null);
			base.SetLastWalkTime(Environment.TickCount);
			base.SetWalkTime(IRandom.Random(1000, 60000));
			this.PushAction(act);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000030EC File Offset: 0x000012EC
		protected override void ProcessAction_Move(GameStruct.Action act)
		{
			this.RefreshVisibleObject();
			List<BaseObject> list = null;
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (!obj.GetVisibleList().ContainsKey(base.GetGameID()))
					{
						if (list == null)
						{
							list = new List<BaseObject>();
						}
						list.Add(obj);
					}
					else
					{
						MsgMoveInfo msgMoveInfo = new MsgMoveInfo();
						msgMoveInfo.Create(null, null);
						msgMoveInfo.id = base.GetTypeId();
						msgMoveInfo.x = base.GetCurrentX();
						msgMoveInfo.y = base.GetCurrentY();
						msgMoveInfo.ucMode = 1;
						msgMoveInfo.dir = base.GetDir();
						byte[] buffer = msgMoveInfo.GetBuffer();
						base.GetGameMap().BroadcastBuffer(this, buffer);
					}
				}
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].AddVisibleObject(this, true);
					(list[i] as PlayerObject).SendMonsterInfo(this);
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003258 File Offset: 0x00001458
		protected override void ProcessAction_Injured(GameStruct.Action act)
		{
			BaseObject baseObject = act.GetObject(0) as BaseObject;
			MsgAttackInfo msgAttackInfo = act.GetObject(2) as MsgAttackInfo;
			if (baseObject != null)
			{
				uint num = (uint)act.GetObject(1);
				this.mTarget = baseObject;
				this.GetAi().Injured(baseObject);
				if (this.IsDie() && !base.IsLock() && msgAttackInfo.tag == 2U)
				{
					GameStruct.Action action = new GameStruct.Action(4, null);
					action.AddObject(baseObject);
					action.AddObject(num);
					this.PushAction(action);
				}
				if (msgAttackInfo.tag == 21U && this.IsDie() && !base.IsLock())
				{
					this.mnDieMagicTick = Environment.TickCount;
					this.mDieMagicInfo = act;
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003338 File Offset: 0x00001538
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			PlayerObject playerObject = act.GetObject(0) as PlayerObject;
			BaseObject baseObject = act.GetObject(0) as BaseObject;
			if (playerObject == null && baseObject.type == 4)
			{
				playerObject = (baseObject as EudemonObject).GetOwnerPlay();
			}
			uint nDamage = (uint)act.GetObject(1);
			byte[] buffer = new MsgMonsterDieInfo
			{
				roleid = baseObject.GetTypeId(),
				role_x = baseObject.GetCurrentX(),
				role_y = baseObject.GetCurrentY(),
				injuredvalue = 0U,
				monsterid = base.GetTypeId()
			}.GetBuffer();
			this.DropItem(baseObject);
			this.BrocatBuffer(buffer);
			this.LastDieTime = Environment.TickCount;
			if (playerObject != null || baseObject.type == 4)
			{
				playerObject.AddExp((int)nDamage, (int)playerObject.GetLevel(), (int)this.GetLevel());
				playerObject.GetEudemonSystem().Eudemon_Alive(this);
				this.GetAi().Die();
				this.GetAi().SetAttackTarget(null);
				this.mAliveTime.Update();
				if (this.mInfo.die_scripte_id > 0U && playerObject != null)
				{
					ScripteManager.Instance().ExecuteAction(this.mInfo.die_scripte_id, playerObject);
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003488 File Offset: 0x00001688
		public MonterAttribute GetAttribute()
		{
			return this.attr;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000034A0 File Offset: 0x000016A0
		public MonsterInfo GetBasicAttribute()
		{
			return this.mInfo;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000034B8 File Offset: 0x000016B8
		public override void Injured(BaseObject obj, uint value, MsgAttackInfo info)
		{
			if ((ulong)value > (ulong)((long)this.attr.life))
			{
				this.attr.life = 0;
			}
			else
			{
				this.attr.life -= (int)value;
			}
			GameStruct.Action action = new GameStruct.Action(6, null);
			action.AddObject(obj);
			action.AddObject(value);
			action.AddObject(info);
			this.PushAction(action);
		}

		// Token: 0x0600004B RID: 75 RVA: 0x0000352B File Offset: 0x0000172B
		public override void ClearThis()
		{
			base.ClearThis();
			this.LastDieTime = -1;
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000353C File Offset: 0x0000173C
		public void DropItem(BaseObject attack)
		{
			uint typeId = attack.GetTypeId();
			if (attack.type == 4)
			{
				if ((attack as EudemonObject).GetOwnerPlay() != null)
				{
					typeId = (attack as EudemonObject).GetOwnerPlay().GetTypeId();
				}
			}
			byte b = BattleSystem.AdjustDrop(attack, this);
			if (b != 0)
			{
				DropItemInfo dropItemInfo = ConfigManager.Instance().GetDropItemInfo(this.mInfo.drop_group);
				if (dropItemInfo != null)
				{
					int num = 0;
					switch (b)
					{
					case 1:
						num = IRandom.Random(2, 5);
						break;
					case 2:
						num = IRandom.Random(6, 9);
						break;
					case 3:
						num = IRandom.Random(10, 15);
						break;
					}
					int num2 = 0;
					short x = 0;
					short y = 0;
					for (int i = 0; i < num; i++)
					{
						int num3 = IRandom.Random(0, dropItemInfo.listamount.Count);
						int j = 0;
						while (j < num3)
						{
							int index = IRandom.Random(0, dropItemInfo.listamount.Count);
							if ((long)IRandom.Random(1, 100) < (long)((ulong)dropItemInfo.listrate[index]))
							{
								this.GetDropItemPoint(ref x, ref y);
								DropItemClass dropItemClass = dropItemInfo.listitem[index];
								if (dropItemClass.list_itemid.Count == 1)
								{
									base.GetGameMap().AddDropItemObj(dropItemClass.list_itemid[0], x, y, typeId, 120000, null, null);
									num2++;
									if (num2 == num)
									{
										break;
									}
								}
							}
							IL_19E:
							j++;
							continue;
							goto IL_19E;
						}
					}
					for (int i = 0; i < dropItemInfo.listitem.Count; i++)
					{
						DropItemClass dropItemClass = dropItemInfo.listitem[i];
						if (dropItemClass.list_itemid.Count > 1)
						{
							int j = 0;
							while ((long)j < (long)((ulong)dropItemInfo.listamount[i]))
							{
								if ((long)IRandom.Random(1, 100) < (long)((ulong)dropItemInfo.listrate[i]))
								{
									this.GetDropItemPoint(ref x, ref y);
									base.GetGameMap().AddDropItemObj(dropItemClass.list_itemid[IRandom.Random(0, dropItemClass.list_itemid.Count - 1)], x, y, typeId, 120000, null, null);
								}
								j++;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003824 File Offset: 0x00001A24
		private bool GetDropItemPoint(ref short x, ref short y)
		{
			x = base.GetCurrentX();
			y = base.GetCurrentY();
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
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (int)x + ((int)array[j] * i + 1);
					int num2 = (int)y + ((int)array2[j] * i + 1);
					if (!base.GetGameMap().GetPointOfObj(this, (short)num, (short)num2))
					{
						x = (short)num;
						y = (short)num2;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000038D8 File Offset: 0x00001AD8
		public override byte GetLevel()
		{
			return (byte)this.mInfo.level;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000038F8 File Offset: 0x00001AF8
		public override int GetMinAck()
		{
			return (int)this.mInfo.attack_min;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003918 File Offset: 0x00001B18
		public override int GetMaxAck()
		{
			return (int)this.mInfo.attack_max;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003938 File Offset: 0x00001B38
		public override int GetDefense()
		{
			return (int)this.mInfo.defense;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003958 File Offset: 0x00001B58
		public override int GetMagicAck()
		{
			return (int)this.mInfo.attack_max;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003978 File Offset: 0x00001B78
		public override int GetMagicDefense()
		{
			return (int)this.mInfo.defense;
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003998 File Offset: 0x00001B98
		public BaseAI CreateAi(int nAi_Id)
		{
			BaseAI baseAI = null;
			switch (nAi_Id)
			{
			case 1:
			case 2:
				baseAI = new BaseAI();
				break;
			}
			if (baseAI == null)
			{
				baseAI = new BaseAI();
			}
			baseAI.Init(this, nAi_Id);
			return baseAI;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000039E8 File Offset: 0x00001BE8
		public override bool CanPK(BaseObject obj, bool bGoCrime = true)
		{
			if (obj.type == 2)
			{
				PlayerObject playerObject = obj as PlayerObject;
				if (playerObject.GetTimerSystem().QueryStatus(14) != null || playerObject.GetTimerSystem().QueryStatus(101) != null || playerObject.GetTimerSystem().QueryStatus(100) != null)
				{
					return false;
				}
			}
			return base.CanPK(obj, true);
		}

		// Token: 0x04000011 RID: 17
		private MonsterInfo mInfo;

		// Token: 0x04000012 RID: 18
		protected MonterAttribute attr;

		// Token: 0x04000013 RID: 19
		private Point mInitPoint;

		// Token: 0x04000014 RID: 20
		protected uint mRebirthTime;

		// Token: 0x04000015 RID: 21
		private int LastDieTime;

		// Token: 0x04000016 RID: 22
		private BaseAI m_Ai;

		// Token: 0x04000017 RID: 23
		private BaseObject mTarget;

		// Token: 0x04000018 RID: 24
		private GameStruct.Action mDieMagicInfo;

		// Token: 0x04000019 RID: 25
		private int mnDieMagicTick;

		// Token: 0x0400001A RID: 26
		private TimeOut mAliveTime;
	}
}
