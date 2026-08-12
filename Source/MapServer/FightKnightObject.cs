using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000014 RID: 20
	public class FightKnightObject : BaseObject
	{
		// Token: 0x0600010D RID: 269 RVA: 0x0000D800 File Offset: 0x0000BA00
		public FightKnightObject(short x, short y, byte dir, uint _id, int nTime)
		{
			this.SetPoint(x, y);
			base.SetDir(dir);
			this.id = _id;
			this.mnTime = nTime * 1000;
			this.mnLastMoveTick = (this.mnTick = Environment.TickCount);
			this.typeid = IDManager.CreateTypeId(7);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000D860 File Offset: 0x0000BA60
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
				if (Environment.TickCount - this.mnLastMoveTick > 700)
				{
					this.Run(base.GetDir(), 3);
					this.mnLastMoveTick = Environment.TickCount;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000D8D6 File Offset: 0x0000BAD6
		public override void ClearThis()
		{
			base.ClearThis();
			base.GetGameMap().RemoveObj(this);
			IDManager.RecoveryTypeID(base.GetTypeId(), this.type);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000D900 File Offset: 0x0000BB00
		public override void Walk(byte dir)
		{
			base.Walk(dir);
			GameStruct.Action act = new GameStruct.Action(2, null);
			this.PushAction(act);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000D928 File Offset: 0x0000BB28
		public override void Run(byte dir, int ucMode)
		{
			base.Run(dir, ucMode);
			GameStruct.Action act = new GameStruct.Action(2, null);
			this.PushAction(act);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000D950 File Offset: 0x0000BB50
		protected override void ProcessAction_Move(GameStruct.Action act)
		{
			this.RefreshVisibleObject();
			List<BaseObject> list = null;
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (!obj.GetVisibleList().ContainsKey(base.GetGameID()))
					{
						if (list == null)
						{
							list = new List<BaseObject>();
						}
						list.Add(obj);
					}
					else
					{
						MsgMoveInfo msgMoveInfo = new MsgMoveInfo();
						msgMoveInfo.Create(null, null);
						msgMoveInfo.id = base.GetTypeId();
						msgMoveInfo.x = base.GetCurrentX();
						msgMoveInfo.y = base.GetCurrentY();
						msgMoveInfo.ucMode = 23;
						msgMoveInfo.dir = base.GetDir();
						byte[] buffer = msgMoveInfo.GetBuffer();
						this.BrocatBuffer(buffer);
					}
				}
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].AddVisibleObject(this, true);
					this.SendInfo(list[i] as PlayerObject);
				}
			}
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000DAC4 File Offset: 0x0000BCC4
		public void SendInfo(PlayerObject play = null)
		{
			MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(this.id);
			if (monsterInfo == null)
			{
				Log.Instance().WriteLog("Failed to get Dark Knight data: " + this.id.ToString());
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
				packetOut.WriteInt32(0);
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

		// Token: 0x06000114 RID: 276 RVA: 0x0000DC34 File Offset: 0x0000BE34
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.type == 2 || baseObject.type == 3)
				{
					if (base.GetPoint().CheckVisualDistance(baseObject.GetCurrentX(), baseObject.GetCurrentY(), 15))
					{
						base.AddVisibleObject(baseObject, false);
					}
					else if (this.mVisibleList.ContainsKey(baseObject.GetGameID()))
					{
						this.mVisibleList.Remove(baseObject.GetGameID());
						if (baseObject.type == 2)
						{
							this.ClearThis(baseObject as PlayerObject);
						}
					}
				}
			}
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000DD38 File Offset: 0x0000BF38
		public void ClearThis(PlayerObject play)
		{
			MsgClearObjectInfo msgClearObjectInfo = new MsgClearObjectInfo();
			msgClearObjectInfo.Create(null, play.GetGamePackKeyEx());
			msgClearObjectInfo.id = base.GetTypeId();
			play.SendData(msgClearObjectInfo.GetBuffer(), false);
		}

		// Token: 0x04000083 RID: 131
		private int mnTime;

		// Token: 0x04000084 RID: 132
		private int mnTick;

		// Token: 0x04000085 RID: 133
		private int mnLastMoveTick;
	}
}
