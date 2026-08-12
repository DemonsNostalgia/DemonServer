using System;
using GameBase.Core;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200004F RID: 79
	public class MingGuoShengNv : MonsterObject
	{
		// Token: 0x060001C1 RID: 449 RVA: 0x00013688 File Offset: 0x00011888
		public MingGuoShengNv(PlayerObject _play, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mPlay = _play;
			base.SetDir(dir);
			this.mnRefreshTick = Environment.TickCount;
			this.mMagicAttackTime = new TimeOut();
			this.mMagicAttackTime.SetInterval(5);
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x000136FC File Offset: 0x000118FC
		public override bool Run()
		{
			bool flag = base.Run();
			bool result;
			if (!base.GetPoint().CheckVisualDistance(this.mPlay.GetCurrentX(), this.mPlay.GetCurrentY(), MingGuoShengNv.DIS))
			{
				this.mPlay.GetTimerSystem().DeleteStatus(106);
				result = false;
			}
			else
			{
				if (base.GetAi().GetTargetObject() == null)
				{
					if (Environment.TickCount - this.mnRefreshTick > MingGuoShengNv.REFRESHTIME)
					{
						this.RefreshVisibleObject();
						this.mnRefreshTick = Environment.TickCount;
					}
				}
				if (this.mMagicAttackTime.ToNextTime())
				{
					this.RefreshVisibleObject();
					this.BrocatBuffer(new MsgMonsterMagicInjuredInfo
					{
						roleid = base.GetTypeId(),
						role_x = base.GetCurrentX(),
						role_y = base.GetCurrentY(),
						tag = 21U,
						magicid = 6051,
						magiclv = 0
					}.GetBuffer());
					MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
					msgGroupMagicAttackInfo.nID = base.GetTypeId();
					msgGroupMagicAttackInfo.nX = base.GetCurrentX();
					msgGroupMagicAttackInfo.nY = base.GetCurrentY();
					msgGroupMagicAttackInfo.nMagicID = 6051;
					msgGroupMagicAttackInfo.nMagicLv = 0;
					msgGroupMagicAttackInfo.bDir = base.GetDir();
					foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
					{
						if (refreshObject.obj.type == 3)
						{
							BaseObject obj = refreshObject.obj;
							if (base.GetPoint().CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), 10))
							{
								uint num = BattleSystem.AdjustDamage(this.mPlay, obj, true);
								obj.Injured(this, num, new MsgAttackInfo
								{
									tag = 21U
								});
								msgGroupMagicAttackInfo.AddObject(obj.GetTypeId(), (int)num);
							}
						}
					}
					this.BrocatBuffer(msgGroupMagicAttackInfo.GetBuffer());
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00013948 File Offset: 0x00011B48
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			base.ProcessAction_Die(act);
			this.mPlay.GetTimerSystem().DeleteStatus(106);
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00013966 File Offset: 0x00011B66
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0001399C File Offset: 0x00011B9C
		public override bool CanPK(BaseObject obj, bool bGoCrime = true)
		{
			bool flag = base.CanPK(obj, true);
			if (flag)
			{
				if (obj.type == 2)
				{
					if (obj.GetTypeId() == this.mPlay.GetTypeId())
					{
						return false;
					}
				}
			}
			return flag;
		}

		// Token: 0x04000372 RID: 882
		private static int DIS = 20;

		// Token: 0x04000373 RID: 883
		private static int REFRESHTIME = 5000;

		// Token: 0x04000374 RID: 884
		public PlayerObject mPlay;

		// Token: 0x04000375 RID: 885
		private int mnRefreshTick;

		// Token: 0x04000376 RID: 886
		private TimeOut mMagicAttackTime;
	}
}
