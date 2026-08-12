using System;
using GameBase.Core;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x020000AB RID: 171
	internal class WangNianWuLing : MonsterObject
	{
		// Token: 0x06000478 RID: 1144 RVA: 0x000342B8 File Offset: 0x000324B8
		public WangNianWuLing(PlayerObject _play, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 9;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mPlay = _play;
			base.SetDir(dir);
			this.mnRefreshTick = Environment.TickCount;
			this.mAddHP_Time = new TimeOut();
			this.mAddHP_Time.SetInterval(2);
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x0003432C File Offset: 0x0003252C
		public override bool Run()
		{
			bool flag = base.Run();
			bool result;
			if (!base.GetPoint().CheckVisualDistance(this.mPlay.GetCurrentX(), this.mPlay.GetCurrentY(), WangNianWuLing.DIS))
			{
				this.mPlay.GetTimerSystem().DeleteStatus(107);
				result = false;
			}
			else
			{
				if (base.GetAi().GetTargetObject() == null)
				{
					if (Environment.TickCount - this.mnRefreshTick > WangNianWuLing.REFRESHTIME)
					{
						this.RefreshVisibleObject();
						this.mnRefreshTick = Environment.TickCount;
					}
				}
				if (this.mAddHP_Time.ToNextTime())
				{
					if (this.mPlay.GetBaseAttr().life < this.mPlay.GetBaseAttr().life_max)
					{
						int num = (int)(this.mPlay.GetBaseAttr().life_max * 0.05);
						this.mPlay.ChangeAttribute(UserAttribute.LIFE, num, true);
						this.BrocatBuffer(new MsgMonsterMagicInjuredInfo
						{
							roleid = base.GetTypeId(),
							role_x = base.GetCurrentX(),
							role_y = base.GetCurrentY(),
							tag = 21U,
							magicid = 6055,
							magiclv = 0,
							monsterid = this.mPlay.GetTypeId(),
							injuredvalue = (uint)num
						}.GetBuffer());
						byte[] array = new byte[48];
						byte[] v = array;
						PacketOut packetOut = new PacketOut(null);
						packetOut.WriteUInt16(88);
						packetOut.WriteUInt16(1105);
						packetOut.WriteUInt32(base.GetTypeId());
						packetOut.WriteUInt32(this.mPlay.GetTypeId());
						packetOut.WriteUInt16(6055);
						packetOut.WriteUInt16(0);
						packetOut.WriteByte(base.GetDir());
						packetOut.WriteByte(1);
						packetOut.WriteUInt32(0U);
						packetOut.WriteUInt32(0U);
						packetOut.WriteUInt32(0U);
						packetOut.WriteUInt16(0);
						packetOut.WriteUInt32(this.mPlay.GetTypeId());
						packetOut.WriteInt32(num);
						packetOut.WriteBuff(v);
						this.BrocatBuffer(packetOut.Flush());
					}
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00034588 File Offset: 0x00032788
		protected override void ProcessAction_Die(GameStruct.Action act)
		{
			base.ProcessAction_Die(act);
			this.mPlay.GetTimerSystem().DeleteStatus(107);
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x000345A6 File Offset: 0x000327A6
		public override void ClearThis()
		{
			this.attr.life = 0;
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x000345DC File Offset: 0x000327DC
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

		// Token: 0x040006A3 RID: 1699
		private static int DIS = 20;

		// Token: 0x040006A4 RID: 1700
		private static int REFRESHTIME = 5000;

		// Token: 0x040006A5 RID: 1701
		public PlayerObject mPlay;

		// Token: 0x040006A6 RID: 1702
		private int mnRefreshTick;

		// Token: 0x040006A7 RID: 1703
		private TimeOut mAddHP_Time;
	}
}
