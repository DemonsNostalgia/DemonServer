using System;
using GameBase.Network;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000011 RID: 17
	public class EffectObject : BaseObject
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x0000974C File Offset: 0x0000794C
		public EffectObject(PlayerObject _play, int nEffId, int nParam, int nParam1, int nTime, short nX, short nY)
		{
			this.mnEffID = nEffId;
			this.mPlay = _play;
			this.mnTime = nTime * 1000;
			this.mnTick = Environment.TickCount;
			this.mnParam = nParam;
			this.mnParam1 = nParam1;
			this.type = 8;
			this.typeid = IDManager.CreateTypeId(8);
			this.mnAttackTick = Environment.TickCount;
			this.SetPoint(nX, nY);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000097C4 File Offset: 0x000079C4
		public override bool Run()
		{
			base.Run();
			bool result;
			if (Environment.TickCount - this.mnTick > this.mnTime)
			{
				this.ClearThis();
				result = false;
			}
			else
			{
				if (this.mnEffID == 5678)
				{
					if (Environment.TickCount - this.mnAttackTick > 1000)
					{
						this.mnAttackTick = Environment.TickCount;
						this.BrocatBuffer(new MsgMonsterMagicInjuredInfo
						{
							roleid = (uint)this.mnEffID,
							role_x = base.GetCurrentX(),
							role_y = base.GetCurrentY(),
							tag = 21U,
							magicid = 6030,
							magiclv = 0
						}.GetBuffer());
						MsgGroupMagicAttackInfo msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
						msgGroupMagicAttackInfo.nID = this.mPlay.GetTypeId();
						msgGroupMagicAttackInfo.nX = base.GetCurrentX();
						msgGroupMagicAttackInfo.nY = base.GetCurrentY();
						msgGroupMagicAttackInfo.nMagicID = 6030;
						msgGroupMagicAttackInfo.nMagicLv = 0;
						msgGroupMagicAttackInfo.bDir = base.GetDir();
						MsgAttackInfo msgAttackInfo = new MsgAttackInfo();
						msgAttackInfo.tag = 21U;
						foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
						{
							BaseObject obj = refreshObject.obj;
							if (obj.type == 3)
							{
								if (base.GetPoint().CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), 7))
								{
									uint num = BattleSystem.AdjustDamage(this.mPlay, obj, true);
									msgGroupMagicAttackInfo.AddObject(obj.GetTypeId(), (int)num);
									obj.Injured(this.mPlay, num, msgAttackInfo);
								}
							}
						}
						this.BrocatBuffer(msgGroupMagicAttackInfo.GetBuffer());
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000099D4 File Offset: 0x00007BD4
		public override void ClearThis()
		{
			this.SendInfo(null, true);
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), 8);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00009A04 File Offset: 0x00007C04
		public void SendInfo(PlayerObject play = null, bool bClear = false)
		{
			PacketOut packetOut;
			if (play != null)
			{
				packetOut = new PacketOut(play.GetGamePackKeyEx());
			}
			else
			{
				packetOut = new PacketOut(null);
			}
			packetOut.WriteUInt16(32);
			packetOut.WriteUInt16(1101);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt32(this.mnEffID);
			packetOut.WriteInt16(base.GetCurrentX());
			packetOut.WriteInt16(base.GetCurrentY());
			packetOut.WriteInt32(0);
			if (!bClear)
			{
				packetOut.WriteInt32(this.mnParam);
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(this.mnParam1);
			}
			else
			{
				packetOut.WriteInt32(12);
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(this.mnParam1);
			}
			if (play != null)
			{
				play.SendData(packetOut.Flush(), false);
			}
			else
			{
				byte[] msg = packetOut.Flush();
				this.BrocatBuffer(msg);
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00009AFC File Offset: 0x00007CFC
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.type == 2 || baseObject.type == 3 || baseObject.type == 4)
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

		// Token: 0x04000070 RID: 112
		private PlayerObject mPlay;

		// Token: 0x04000071 RID: 113
		private int mnEffID;

		// Token: 0x04000072 RID: 114
		private int mnParam;

		// Token: 0x04000073 RID: 115
		private int mnParam1;

		// Token: 0x04000074 RID: 116
		private int mnTime;

		// Token: 0x04000075 RID: 117
		private int mnTick;

		// Token: 0x04000076 RID: 118
		private int mnAttackTick;
	}
}
