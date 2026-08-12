using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000002 RID: 2
	public class BaseObject
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Dictionary<uint, RefreshObject> GetVisibleList()
		{
			return this.mVisibleList;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		public Point GetPoint()
		{
			return this.mPoint;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
		public int GetLastWalkTime()
		{
			return this.lastwalktime;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002098 File Offset: 0x00000298
		public void SetLastWalkTime(int _lasttime)
		{
			this.lastwalktime = _lasttime;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020A4 File Offset: 0x000002A4
		public int GetWalkTime()
		{
			return this.walkTime;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BC File Offset: 0x000002BC
		public void SetWalkTime(int _time)
		{
			this.walkTime = _time;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020C8 File Offset: 0x000002C8
		public void Lock(int time, bool isSendData = true)
		{
			this.locktime = time;
			this.lastlocktime = Environment.TickCount;
			if (isSendData)
			{
				MsgLock msgLock = new MsgLock();
				msgLock.Lock();
				msgLock.id = this.GetTypeId();
				msgLock.x = this.GetCurrentX();
				msgLock.y = this.GetCurrentY();
				this.GetGameMap().BroadcastBuffer(this, msgLock.GetBuffer());
			}
			this.mIsLock = true;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002140 File Offset: 0x00000340
		public bool IsLock()
		{
			return this.mIsLock;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002158 File Offset: 0x00000358
		public bool CheckLockTime()
		{
			bool result;
			if (this.locktime == 0)
			{
				result = false;
			}
			else if (Environment.TickCount - this.lastlocktime > this.locktime)
			{
				this.locktime = 0;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000021A8 File Offset: 0x000003A8
		public void UnLock(bool isSendData = true)
		{
			this.mIsLock = false;
			this.locktime = 0;
			this.lastlocktime = Environment.TickCount;
			if (isSendData)
			{
				MsgLock msgLock = new MsgLock();
				msgLock.UnLock();
				msgLock.id = this.GetTypeId();
				msgLock.x = this.GetCurrentX();
				msgLock.y = this.GetCurrentY();
				this.RefreshVisibleObject();
				this.GetGameMap().BroadcastBuffer(this, msgLock.GetBuffer());
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002224 File Offset: 0x00000424
		public BaseObject()
		{
			this.mVisibleList = new Dictionary<uint, RefreshObject>();
			this.mActionList = new List<GameStruct.Action>();
			this.mPoint = new Point();
			this.type = 0;
			this.gameid = IDManager.CreateId();
			this.session = null;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002288 File Offset: 0x00000488
		public uint GetID()
		{
			return this.id;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022A0 File Offset: 0x000004A0
		public void SetID(uint __id)
		{
			this.id = __id;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022AC File Offset: 0x000004AC
		public uint GetGameID()
		{
			return this.gameid;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022C4 File Offset: 0x000004C4
		public uint GetTypeId()
		{
			return this.typeid;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022DC File Offset: 0x000004DC
		public GameSession GetGameSession()
		{
			return this.session;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022F4 File Offset: 0x000004F4
		public void SetGameSession(GameSession _session)
		{
			this.session = _session;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002300 File Offset: 0x00000500
		public string GetName()
		{
			return this.Name;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002318 File Offset: 0x00000518
		public void SetName(string _name)
		{
			this.Name = _name;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002324 File Offset: 0x00000524
		public virtual bool Run()
		{
			for (;;)
			{
				GameStruct.Action action = this.PopAction();
				if (action == null)
				{
					break;
				}
				this.ProcessAction(action);
			}
			return true;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000235C File Offset: 0x0000055C
		public virtual void RefreshVisibleObject()
		{
			List<uint> list = null;
			foreach (RefreshObject refreshObject in this.mVisibleList.Values)
			{
				BaseObject obj = refreshObject.obj;
				uint item;
				if (obj.type == 3 || obj.type == 7)
				{
					item = obj.GetTypeId();
				}
				else
				{
					item = obj.GetGameID();
				}
				if (this.GetGameMap().GetObject(item) == null)
				{
					if (list == null)
					{
						list = new List<uint>();
					}
					list.Add(item);
				}
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.mVisibleList.Remove(list[i]);
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002468 File Offset: 0x00000668
		public void SetDir(byte dir)
		{
			this.bDir = dir;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002474 File Offset: 0x00000674
		public byte GetDir()
		{
			return this.bDir;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000248C File Offset: 0x0000068C
		public virtual void SetPoint(short x, short y)
		{
			this.mPoint.x = x;
			this.mPoint.y = y;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000024A8 File Offset: 0x000006A8
		public short GetCurrentX()
		{
			return this.mPoint.x;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000024C8 File Offset: 0x000006C8
		public short GetCurrentY()
		{
			return this.mPoint.y;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000024E8 File Offset: 0x000006E8
		public GameMap GetGameMap()
		{
			return this.mGameMap;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002500 File Offset: 0x00000700
		public virtual void Walk(byte dir, short x, short y)
		{
			this.bDir = dir;
			this.SetPoint(x, y);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002514 File Offset: 0x00000714
		public virtual void Run(byte dir, int ucMode)
		{
			if (dir != 8)
			{
				this.Walk(dir);
				for (int i = 0; i < ucMode; i++)
				{
					this.Walk(dir);
				}
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002554 File Offset: 0x00000754
		public virtual void Walk(byte dir)
		{
			if (dir != 8)
			{
				this.bDir = dir;
				switch (dir)
				{
				case 0:
				{
					Point point = this.mPoint;
					point.x -= 1;
					Point point2 = this.mPoint;
					point2.y += 1;
					break;
				}
				case 1:
				{
					Point point3 = this.mPoint;
					point3.x -= 1;
					break;
				}
				case 2:
				{
					Point point4 = this.mPoint;
					point4.x -= 1;
					Point point5 = this.mPoint;
					point5.y -= 1;
					break;
				}
				case 3:
				{
					Point point6 = this.mPoint;
					point6.y -= 1;
					break;
				}
				case 4:
				{
					Point point7 = this.mPoint;
					point7.x += 1;
					Point point8 = this.mPoint;
					point8.y -= 1;
					break;
				}
				case 5:
				{
					Point point9 = this.mPoint;
					point9.x += 1;
					break;
				}
				case 6:
				{
					Point point10 = this.mPoint;
					point10.x += 1;
					Point point11 = this.mPoint;
					point11.y += 1;
					break;
				}
				case 7:
				{
					Point point12 = this.mPoint;
					point12.y += 1;
					break;
				}
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000026BA File Offset: 0x000008BA
		public virtual void PushAction(GameStruct.Action act)
		{
			this.mActionList.Add(act);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026CC File Offset: 0x000008CC
		public virtual GameStruct.Action PopAction()
		{
			GameStruct.Action result;
			if (this.mActionList.Count > 0)
			{
				GameStruct.Action action = this.mActionList[0];
				this.mActionList.Remove(action);
				result = action;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002714 File Offset: 0x00000914
		public virtual void ProcessAction(GameStruct.Action act)
		{
			if (act != null)
			{
				switch (act.GetAction())
				{
				case 2:
					this.ProcessAction_Move(act);
					break;
				case 3:
					this.ProcessAction_Attack(act);
					break;
				case 4:
					this.ProcessAction_Die(act);
					break;
				case 5:
					this.ProcessAction_Alive(act);
					break;
				case 6:
					this.ProcessAction_Injured(act);
					break;
				}
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000278B File Offset: 0x0000098B
		protected virtual void ProcessAction_Move(GameStruct.Action act)
		{
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000278E File Offset: 0x0000098E
		protected virtual void ProcessAction_Attack(GameStruct.Action act)
		{
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002791 File Offset: 0x00000991
		protected virtual void ProcessAction_Die(GameStruct.Action act)
		{
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002794 File Offset: 0x00000994
		protected virtual void ProcessAction_Alive(GameStruct.Action act)
		{
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002797 File Offset: 0x00000997
		protected virtual void ProcessAction_Injured(GameStruct.Action act)
		{
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000279A File Offset: 0x0000099A
		public virtual void Injured(BaseObject obj, uint value, MsgAttackInfo info)
		{
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000027A0 File Offset: 0x000009A0
		public virtual void Dispose()
		{
			this.mVisibleList.Clear();
			this.mActionList.Clear();
			if (this.session != null)
			{
				this.session.Dispose();
				this.session = null;
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000027E8 File Offset: 0x000009E8
		public virtual bool IsDie()
		{
			return false;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000027FB File Offset: 0x000009FB
		public virtual void CalcAttribute()
		{
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002800 File Offset: 0x00000A00
		public virtual int GetMinAck()
		{
			return 0;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002814 File Offset: 0x00000A14
		public virtual int GetMaxAck()
		{
			return 0;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002828 File Offset: 0x00000A28
		public virtual int GetDefense()
		{
			return 0;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000283C File Offset: 0x00000A3C
		public virtual int GetMagicDefense()
		{
			return 0;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002850 File Offset: 0x00000A50
		public virtual byte GetLevel()
		{
			return 0;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002864 File Offset: 0x00000A64
		public virtual int GetMagicAck()
		{
			return 0;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002878 File Offset: 0x00000A78
		public virtual int GetMaxMagixAck()
		{
			return 0;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000288C File Offset: 0x00000A8C
		public virtual int GetLuck()
		{
			return 0;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000028A0 File Offset: 0x00000AA0
		public virtual int AdjustExp(int exp)
		{
			return exp;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000028B4 File Offset: 0x00000AB4
		public virtual void BrocatBuffer(byte[] msg)
		{
			foreach (RefreshObject refreshObject in this.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2 && obj.GetGameSession() != null)
				{
					BaseMsg baseMsg = new BaseMsg();
					baseMsg.Create(msg, obj.GetGamePackKeyEx());
					obj.SendData(baseMsg.GetBuffer(), false);
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00002958 File Offset: 0x00000B58
		public virtual void ClearThis()
		{
			byte[] buffer = new MsgClearObjectInfo
			{
				id = this.GetTypeId()
			}.GetBuffer();
			this.BrocatBuffer(buffer);
			foreach (RefreshObject refreshObject in this.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				if (obj.type == 2)
				{
					if (obj.GetVisibleList().ContainsKey(this.GetGameID()))
					{
						obj.GetVisibleList().Remove(this.GetGameID());
					}
				}
			}
			this.GetVisibleList().Clear();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002A28 File Offset: 0x00000C28
		public void AddVisibleObject(BaseObject obj, bool bRefreshTag = true)
		{
			if (this.GetVisibleList().ContainsKey(obj.GetGameID()))
			{
				RefreshObject refreshObject = this.mVisibleList[obj.GetGameID()];
				refreshObject.bRefreshTag = bRefreshTag;
			}
			else
			{
				RefreshObject refreshObject = new RefreshObject();
				refreshObject.bRefreshTag = bRefreshTag;
				refreshObject.obj = obj;
				this.mVisibleList[obj.GetGameID()] = refreshObject;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002A94 File Offset: 0x00000C94
		public virtual bool CanPK(BaseObject obj, bool bGoCrime = true)
		{
			return true;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002AA8 File Offset: 0x00000CA8
		public void SendData(byte[] data, bool isEncode = false)
		{
			if (this.GetGameSession() != null)
			{
				ushort packetType = 0;
				if (data != null && data.Length >= 4)
				{
					packetType = BitConverter.ToUInt16(data, 2);
				}
				if (isEncode)
				{
					if (this.GetGamePackKeyEx() == null)
					{
						Log.Instance().WriteLog("Failed to send data. The player may be disconnected.");
					}
					else
					{
						byte[] array = new byte[data.Length];
						Buffer.BlockCopy(data, 0, array, 0, data.Length);
						this.GetGamePackKeyEx().EncodePacket(ref array, array.Length);
						this.GetGameSession().SendData(array, packetType);
					}
				}
				else
				{
					this.GetGameSession().SendData(data, packetType);
				}
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002B38 File Offset: 0x00000D38
		public GamePacketKeyEx GetGamePackKeyEx()
		{
			GamePacketKeyEx result;
			if (this.GetGameSession() != null)
			{
				result = this.GetGameSession().GetGamePackKeyEx();
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		protected uint gameid;

		// Token: 0x04000002 RID: 2
		protected uint typeid;

		// Token: 0x04000003 RID: 3
		protected uint id;

		// Token: 0x04000004 RID: 4
		private Point mPoint;

		// Token: 0x04000005 RID: 5
		private byte bDir;

		// Token: 0x04000006 RID: 6
		public string Name;

		// Token: 0x04000007 RID: 7
		public byte type;

		// Token: 0x04000008 RID: 8
		public GameMap mGameMap;

		// Token: 0x04000009 RID: 9
		public Dictionary<uint, RefreshObject> mVisibleList;

		// Token: 0x0400000A RID: 10
		private List<GameStruct.Action> mActionList;

		// Token: 0x0400000B RID: 11
		public GameSession session;

		// Token: 0x0400000C RID: 12
		private int walkTime;

		// Token: 0x0400000D RID: 13
		private int lastwalktime;

		// Token: 0x0400000E RID: 14
		private bool mIsLock;

		// Token: 0x0400000F RID: 15
		private int locktime = 0;

		// Token: 0x04000010 RID: 16
		private int lastlocktime = Environment.TickCount;
	}
}
