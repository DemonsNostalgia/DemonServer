using System;
using System.Collections.Generic;
using GameStruct;
using MapServer;
using NetMsg;

namespace AI
{
	// Token: 0x02000005 RID: 5
	public class BaseAI
	{
		// Token: 0x0600005C RID: 92 RVA: 0x00003C14 File Offset: 0x00001E14
		public BaseObject GetTargetObject()
		{
			return this.TargetObj;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003C38 File Offset: 0x00001E38
		public virtual void Init(BaseObject obj = null, int nAi_Id = 1)
		{
			this.TargetObj = null;
			this.SelObj = obj;
			this.nState = BaseAI.STATE_IDLE;
			this.mnLastAttackTick = Environment.TickCount;
			this.mnLastMoveTick = this.mnLastAttackTick;
			this.findlist = null;
			this.mAiInfo = ConfigManager.Instance().GetAIInfo(nAi_Id);
			this.mnActiveAttackTick = Environment.TickCount;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003C9C File Offset: 0x00001E9C
		public virtual void Run()
		{
			if (!this.SelObj.IsDie())
			{
				if (this.SelObj.IsLock())
				{
					if (this.findlist != null)
					{
						this.findlist.Clear();
					}
				}
				else if (this.TargetObj == null || !this.TargetObj.IsLock())
				{
					if (this.TargetObj != null && this.TargetObj.IsDie())
					{
						this.SetAttackTarget(null);
					}
					if (this.TargetObj != null && this.TargetObj.type == 4 && (this.TargetObj as EudemonObject).GetState() != EUDEMONSTATE.BATTLE)
					{
						this.SetAttackTarget(null);
					}
					if (this.nState == BaseAI.STATE_IDLE && this.mAiInfo.bIdle_Move)
					{
						if (Environment.TickCount - this.SelObj.GetLastWalkTime() > this.SelObj.GetWalkTime() && !this.SelObj.IsDie())
						{
							if (IRandom.Random(1, 4) > 2)
							{
								bool flag = false;
								foreach (RefreshObject refreshObject in this.SelObj.GetVisibleList().Values)
								{
									if (refreshObject.obj.type == 2)
									{
										flag = true;
										break;
									}
								}
								if (flag)
								{
									byte dir = 0;
									short x = 0;
									short y = 0;
									if (DIR.Random_Walk(this.SelObj, ref dir, ref x, ref y))
									{
										this.SelObj.Walk(dir, x, y);
									}
								}
							}
						}
					}
					if (this.TargetObj != null && this.TargetObj.type == 2 && this.TargetObj.GetGameSession() == null)
					{
						this.SetAttackTarget(null);
					}
					this.ActiveAttackPlay();
					if (this.TargetObj != null && (this.nState == BaseAI.INJURED || this.nState == BaseAI.ATTACK || this.nState == BaseAI.FOLLOW))
					{
						if (!this.SelObj.CanPK(this.TargetObj, true))
						{
							this.SetAttackTarget(null);
						}
						else
						{
							if (this.TargetObj.IsDie() || !this.SelObj.GetPoint().CheckVisualDistance(this.TargetObj.GetCurrentX(), this.TargetObj.GetCurrentY(), this.mAiInfo.nRange))
							{
								this.TargetObj = null;
								this.nState = BaseAI.STATE_IDLE;
								if (this.findlist != null)
								{
									this.findlist.Clear();
								}
							}
							if (this.TargetObj != null && Environment.TickCount - this.mnLastMoveTick > this.mAiInfo.nMove_Speed && this.mAiInfo.bMove)
							{
								if (Math.Abs((int)(this.TargetObj.GetCurrentX() - this.SelObj.GetCurrentX())) > this.mAiInfo.nAttack_Range || Math.Abs((int)(this.TargetObj.GetCurrentY() - this.SelObj.GetCurrentY())) > this.mAiInfo.nAttack_Range)
								{
									if (this.findlist != null && this.findlist.Count > 0)
									{
										FindPoint findPoint = this.findlist[this.findlist.Count - 1];
										this.findlist.RemoveAt(this.findlist.Count - 1);
										if (findPoint.x == this.SelObj.GetCurrentX() && findPoint.y == this.SelObj.GetCurrentY())
										{
											return;
										}
										byte dir = DIR.GetDirByPos(this.SelObj.GetCurrentX(), this.SelObj.GetCurrentY(), findPoint.x, findPoint.y);
										if (this.findlist.Count == 0)
										{
											this.nState = BaseAI.ATTACK;
										}
										this.SelObj.Walk(dir, findPoint.x, findPoint.y);
									}
									else
									{
										this.FollowTarget();
									}
									this.mnLastMoveTick = Environment.TickCount;
									return;
								}
							}
							if (this.TargetObj != null && Environment.TickCount - this.mnLastAttackTick > this.mAiInfo.nAttack_Speed)
							{
								if (Math.Abs((int)(this.TargetObj.GetCurrentX() - this.SelObj.GetCurrentX())) <= this.mAiInfo.nAttack_Range && Math.Abs((int)(this.TargetObj.GetCurrentY() - this.SelObj.GetCurrentY())) <= this.mAiInfo.nAttack_Range)
								{
									MsgMonsterAttackInfo msgMonsterAttackInfo = new MsgMonsterAttackInfo();
									msgMonsterAttackInfo.Create(null, null);
									msgMonsterAttackInfo.monsterid = this.SelObj.GetTypeId();
									msgMonsterAttackInfo.roleid = this.TargetObj.GetTypeId();
									msgMonsterAttackInfo.role_x = this.TargetObj.GetCurrentX();
									msgMonsterAttackInfo.role_y = this.TargetObj.GetCurrentY();
									msgMonsterAttackInfo.injuredvalue = BattleSystem.AdjustDamage(this.SelObj, this.TargetObj, false);
									byte[] buffer = msgMonsterAttackInfo.GetBuffer();
									(this.SelObj as MonsterObject).BrocatBuffer(buffer);
									MsgAttackInfo msgAttackInfo = new MsgAttackInfo();
									msgAttackInfo.tag = 21U;
									this.TargetObj.Injured(this.SelObj, msgMonsterAttackInfo.injuredvalue, msgAttackInfo);
									this.mnLastAttackTick = Environment.TickCount;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000042E4 File Offset: 0x000024E4
		public virtual void Injured(BaseObject attackobj)
		{
			this.SetAttackTarget(attackobj);
			this.nState = BaseAI.INJURED;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000042FC File Offset: 0x000024FC
		public virtual void Die()
		{
			this.TargetObj = null;
			if (this.findlist != null)
			{
				this.findlist.Clear();
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000432C File Offset: 0x0000252C
		public virtual void SetAttackTarget(BaseObject obj)
		{
			if (obj != null && !this.SelObj.CanPK(obj, true))
			{
				this.TargetObj = null;
				this.nState = BaseAI.STATE_IDLE;
			}
			else
			{
				this.TargetObj = obj;
				if (this.TargetObj == null)
				{
					this.nState = BaseAI.STATE_IDLE;
				}
				else
				{
					this.nState = BaseAI.ATTACK;
				}
				if (this.findlist != null)
				{
					this.findlist.Clear();
				}
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000043B4 File Offset: 0x000025B4
		public virtual void FollowTarget()
		{
			if (this.SelObj.GetGameMap().CanMove(this.TargetObj.GetCurrentX(), this.TargetObj.GetCurrentY()))
			{
				this.findlist = this.SelObj.GetGameMap().GetMapPath().FindPath(this.SelObj.GetCurrentX(), this.SelObj.GetCurrentY(), this.TargetObj.GetCurrentX(), this.TargetObj.GetCurrentY());
				if (this.findlist != null && this.findlist.Count > 0)
				{
					this.findlist.RemoveAt(0);
				}
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004468 File Offset: 0x00002668
		protected virtual void ActiveAttackPlay()
		{
			if (this.mAiInfo.nType == 1)
			{
				if ((long)(Environment.TickCount - this.mnActiveAttackTick) >= 5000L)
				{
					this.mnActiveAttackTick = Environment.TickCount;
					Point point = null;
					BaseObject baseObject = null;
					if (this.TargetObj == null && Environment.TickCount - this.mnLastMoveTick > this.mAiInfo.nMove_Speed && this.SelObj.GetVisibleList().Count > 0)
					{
						foreach (RefreshObject refreshObject in this.SelObj.GetVisibleList().Values)
						{
							if (refreshObject.obj.type == 2 || refreshObject.obj.type == 4 || refreshObject.obj.type == 7 || refreshObject.obj.type == 3 || refreshObject.obj.type == 9)
							{
								if (this.SelObj.CanPK(refreshObject.obj, true))
								{
									Point point2 = new Point();
									point2.x = (short)Math.Abs((int)(refreshObject.obj.GetCurrentX() - this.SelObj.GetCurrentX()));
									point2.y = (short)Math.Abs((int)(refreshObject.obj.GetCurrentY() - this.SelObj.GetCurrentY()));
									if (point == null)
									{
										point = point2;
										baseObject = refreshObject.obj;
									}
									if (point2.x < point.x && point2.y < point.y)
									{
										point = point2;
										baseObject = refreshObject.obj;
									}
								}
							}
						}
						if (baseObject != null)
						{
							this.TargetObj = baseObject;
							this.nState = BaseAI.ATTACK;
							this.mnLastMoveTick = Environment.TickCount;
						}
					}
				}
			}
		}

		// Token: 0x0400001F RID: 31
		protected static byte STATE_IDLE = 1;

		// Token: 0x04000020 RID: 32
		protected static byte INJURED = 2;

		// Token: 0x04000021 RID: 33
		protected static byte ATTACK = 3;

		// Token: 0x04000022 RID: 34
		public static byte FOLLOW = 4;

		// Token: 0x04000023 RID: 35
		protected BaseObject TargetObj;

		// Token: 0x04000024 RID: 36
		protected BaseObject SelObj;

		// Token: 0x04000025 RID: 37
		private byte nState;

		// Token: 0x04000026 RID: 38
		private int mnLastAttackTick;

		// Token: 0x04000027 RID: 39
		private int mnLastMoveTick;

		// Token: 0x04000028 RID: 40
		protected AiInfo mAiInfo;

		// Token: 0x04000029 RID: 41
		private List<FindPoint> findlist;

		// Token: 0x0400002A RID: 42
		private int mnActiveAttackTick;
	}
}
