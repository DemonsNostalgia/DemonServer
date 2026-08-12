using System;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000010 RID: 16
	public class DropItemObject : BaseObject
	{
		// Token: 0x060000BC RID: 188 RVA: 0x000094DC File Offset: 0x000076DC
		public RoleItemInfo GetRoleItemInfo()
		{
			return this.mItemInfo;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x000094F4 File Offset: 0x000076F4
		public RoleData_Eudemon GetRoleEudemonInfo()
		{
			return this.mEudemonInfo;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000950C File Offset: 0x0000770C
		public DropItemObject(uint itemid, short x, short y, uint ownerid, int time)
		{
			this.mTime = time;
			this.type = 5;
			this.typeid = itemid;
			this.mCurTime = Environment.TickCount;
			this.nOwnerid = ownerid;
			this.SetPoint(x, y);
			this.mItemInfo = null;
			this.mPickupClaimant = 0U;
			this.mPickupCompleted = false;
		}

		public bool TryClaimPickup(uint claimant)
		{
			if (claimant == 0U)
			{
				return false;
			}
			lock (this.mPickupSync)
			{
				if (this.mPickupCompleted || this.mPickupClaimant != 0U)
				{
					return false;
				}
				this.mPickupClaimant = claimant;
				return true;
			}
		}

		public void CancelPickup(uint claimant)
		{
			lock (this.mPickupSync)
			{
				if (!this.mPickupCompleted &&
					this.mPickupClaimant == claimant)
				{
					this.mPickupClaimant = 0U;
				}
			}
		}

		public bool CompletePickup(uint claimant)
		{
			lock (this.mPickupSync)
			{
				if (this.mPickupCompleted ||
					this.mPickupClaimant != claimant)
				{
					return false;
				}
				this.mPickupCompleted = true;
				this.mPickupClaimant = 0U;
			}

			this.RefreshVisibleObject();
			this.BroadcastInfo(2U);
			base.GetGameMap().RemoveObj(this);
			return true;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000955B File Offset: 0x0000775B
		public void SetRoleItemInfo(RoleItemInfo info)
		{
			this.mItemInfo = info;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00009565 File Offset: 0x00007765
		public void SetRoleEudemonInfo(RoleData_Eudemon eudemon)
		{
			this.mEudemonInfo = eudemon;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00009570 File Offset: 0x00007770
		public override bool Run()
		{
			lock (this.mPickupSync)
			{
				if (this.mPickupCompleted)
				{
					return false;
				}
				if (this.mPickupClaimant != 0U)
				{
					// Keep the object reserved until the asynchronous database
					// result either completes or cancels the pickup.
					return true;
				}
			}
			bool result;
			if (Environment.TickCount - this.mCurTime > this.mTime)
			{
				this.RefreshVisibleObject();
				this.BroadcastInfo(2U);
				result = false;
			}
			else
			{
				if (this.nOwnerid != 0U)
				{
					if (Environment.TickCount - this.mCurTime > 60000)
					{
						this.nOwnerid = 0U;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000095E4 File Offset: 0x000077E4
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.type == 2)
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

		// Token: 0x060000C3 RID: 195 RVA: 0x000096B4 File Offset: 0x000078B4
		public void BroadcastInfo(uint tag = 1U)
		{
			byte[] buffer = new MsgDropItem
			{
				tag = tag,
				id = base.GetGameID(),
				typeid = base.GetTypeId(),
				x = base.GetCurrentX(),
				y = base.GetCurrentY()
			}.GetBuffer();
			base.GetGameMap().BroadcastBuffer(this, buffer);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00009714 File Offset: 0x00007914
		public bool IsOwner()
		{
			return this.nOwnerid != 0U;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00009734 File Offset: 0x00007934
		public uint GetOwnerId()
		{
			return this.nOwnerid;
		}

		// Token: 0x0400006A RID: 106
		private const int nOwnerTime = 60000;

		// Token: 0x0400006B RID: 107
		private int mTime;

		// Token: 0x0400006C RID: 108
		private int mCurTime;

		// Token: 0x0400006D RID: 109
		private uint nOwnerid;

		// Token: 0x0400006E RID: 110
		private RoleItemInfo mItemInfo;

		// Token: 0x0400006F RID: 111
		private RoleData_Eudemon mEudemonInfo;

		private readonly object mPickupSync = new object();

		private uint mPickupClaimant;

		private bool mPickupCompleted;
	}
}
