using System;
using GameBase.Config;
using GameBase.Network;
using GameStruct;

namespace MapServer
{
	// Token: 0x02000045 RID: 69
	public class GuardKnightObject : MonsterObject
	{
		// Token: 0x06000194 RID: 404 RVA: 0x00012298 File Offset: 0x00010498
		public GuardKnightObject(PlayerObject _play, short x, short y, byte dir, uint _id, int nAi_Id) : base(_id, nAi_Id, x, y, false)
		{
			this.type = 7;
			this.typeid = IDManager.CreateTypeId(7);
			this.SetPoint(x, y);
			this.mRebirthTime = 0U;
			this.mnSurvivalTick = Environment.TickCount;
			this.mPlay = _play;
			base.SetDir(dir);
		}

		// Token: 0x06000195 RID: 405 RVA: 0x000122F4 File Offset: 0x000104F4
		public override bool Run()
		{
			base.Run();
			bool result;
			if (Environment.TickCount - this.mnSurvivalTick >= 120000 && this.mPlay != null)
			{
				this.mPlay.GetFightSystem().RemoveQiShiTuanGuardEffect();
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00012348 File Offset: 0x00010548
		public override void ClearThis()
		{
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
			this.mPlay = null;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00012384 File Offset: 0x00010584
		public void SendInfo(PlayerObject play = null)
		{
			MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(this.id);
			if (monsterInfo == null)
			{
				Log.Instance().WriteLog("Failed to get Guard Knight data: " + this.id.ToString());
			}
			else
			{
				PacketOut packetOut;
				if (play == null)
				{
					packetOut = new PacketOut(null);
				}
				else
				{
					packetOut = new PacketOut(play.GetGamePackKeyEx());
				}
				packetOut.WriteUInt16(81);
				packetOut.WriteUInt16(2069);
				packetOut.WriteUInt32(base.GetTypeId());
				packetOut.WriteUInt32(play.GetTypeId());
				byte[] array = new byte[32];
				byte[] array2 = array;
				packetOut.WriteBuff(array2);
				packetOut.WriteUInt32(monsterInfo.lookface);
				packetOut.WriteInt16(base.GetCurrentX());
				packetOut.WriteInt16(base.GetCurrentY());
				packetOut.WriteInt16(0);
				packetOut.WriteUInt16(monsterInfo.level);
				packetOut.WriteUInt32(monsterInfo.id);
				packetOut.WriteInt32(this.attr.life);
				packetOut.WriteInt32(monsterInfo.life);
				packetOut.WriteInt16((short)base.GetDir());
				byte[] v = new byte[]
				{
					100,
					0,
					1,
					4,
					210,
					193,
					183,
					227,
					0,
					0,
					0
				};
				packetOut.WriteBuff(v);
				if (play != null)
				{
					play.SendData(packetOut.Flush(), false);
				}
				else
				{
					array2 = packetOut.Flush();
					this.BrocatBuffer(array2);
				}
			}
		}

		// Token: 0x04000356 RID: 854
		private int mnSurvivalTick;

		// Token: 0x04000357 RID: 855
		private PlayerObject mPlay;
	}
}
