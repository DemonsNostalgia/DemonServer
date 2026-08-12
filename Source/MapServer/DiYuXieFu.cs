using System;

namespace MapServer
{
	// Token: 0x0200000F RID: 15
	public class DiYuXieFu : MonsterObject
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x000093CC File Offset: 0x000075CC
		public DiYuXieFu(PlayerObject _play, BaseObject _AttackTarget, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mPlay = _play;
			base.SetDir(dir);
			base.GetAi().SetAttackTarget(_AttackTarget);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000942C File Offset: 0x0000762C
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

		// Token: 0x060000BB RID: 187 RVA: 0x000094A7 File Offset: 0x000076A7
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x04000069 RID: 105
		public PlayerObject mPlay;
	}
}
