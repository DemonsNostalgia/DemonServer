using System;

namespace MapServer
{
	// Token: 0x020000A3 RID: 163
	public class ShiHunWuLing : MonsterObject
	{
		// Token: 0x06000442 RID: 1090 RVA: 0x00032F90 File Offset: 0x00031190
		public ShiHunWuLing(PlayerObject _play, BaseObject _AttackTarget, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mPlay = _play;
			base.SetDir(dir);
			base.GetAi().SetAttackTarget(_AttackTarget);
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00032FF0 File Offset: 0x000311F0
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

		// Token: 0x06000444 RID: 1092 RVA: 0x0003306B File Offset: 0x0003126B
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x04000684 RID: 1668
		public PlayerObject mPlay;
	}
}
