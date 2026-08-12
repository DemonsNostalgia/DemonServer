using System;

namespace MapServer
{
	// Token: 0x020000A2 RID: 162
	public class ShenYuanELing : MonsterObject
	{
		// Token: 0x0600043F RID: 1087 RVA: 0x00032E4C File Offset: 0x0003104C
		public ShenYuanELing(PlayerObject _play, BaseObject _AttackTarget, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.mPlay = _play;
			this.attr.life = (this.attr.life_max = (int)(this.mPlay.GetBaseAttr().life * 1.5f));
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			base.SetDir(dir);
			base.GetAi().SetAttackTarget(_AttackTarget);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00032EE0 File Offset: 0x000310E0
		public override bool Run()
		{
			bool result = base.Run();
			if (base.GetAi().GetTargetObject() == null)
			{
				this.ClearThis();
				if (this.mPlay.GetGameSession() != null)
				{
					this.mPlay.SetZhaoHuanWuHuanObj(null);
				}
				result = false;
			}
			if (this.mPlay.GetGameSession() == null)
			{
				this.ClearThis();
				result = false;
			}
			return result;
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x00032F5B File Offset: 0x0003115B
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x04000683 RID: 1667
		public PlayerObject mPlay;
	}
}
