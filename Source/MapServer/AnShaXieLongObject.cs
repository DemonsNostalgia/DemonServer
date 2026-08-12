using System;
using GameStruct;

namespace MapServer
{
	// Token: 0x02000004 RID: 4
	public class AnShaXieLongObject : MonsterObject
	{
		// Token: 0x06000056 RID: 86 RVA: 0x00003A58 File Offset: 0x00001C58
		public AnShaXieLongObject(PlayerObject _play, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mPlay = _play;
			base.SetDir(dir);
			this.mnRefreshTick = Environment.TickCount;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003AB4 File Offset: 0x00001CB4
		public override bool Run()
		{
			bool flag = base.Run();
			bool result;
			if (!base.GetPoint().CheckVisualDistance(this.mPlay.GetCurrentX(), this.mPlay.GetCurrentY(), AnShaXieLongObject.DIS))
			{
				this.mPlay.GetTimerSystem().DeleteStatus(105);
				result = false;
			}
			else
			{
				if (base.GetAi().GetTargetObject() == null)
				{
					if (Environment.TickCount - this.mnRefreshTick > AnShaXieLongObject.REFRESHTIME)
					{
						this.RefreshVisibleObject();
						this.mnRefreshTick = Environment.TickCount;
					}
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003B55 File Offset: 0x00001D55
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			base.ProcessAction_Die(act);
			this.mPlay.GetTimerSystem().DeleteStatus(105);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003B73 File Offset: 0x00001D73
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003BA8 File Offset: 0x00001DA8
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

		// Token: 0x0400001B RID: 27
		private static int DIS = 20;

		// Token: 0x0400001C RID: 28
		private static int REFRESHTIME = 5000;

		// Token: 0x0400001D RID: 29
		public PlayerObject mPlay;

		// Token: 0x0400001E RID: 30
		private int mnRefreshTick;
	}
}
