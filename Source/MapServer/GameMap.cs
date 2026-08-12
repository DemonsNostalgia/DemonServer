using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameBase.Config;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000015 RID: 21
	public class GameMap
	{
		// Token: 0x06000116 RID: 278 RVA: 0x0000DD74 File Offset: 0x0000BF74
		public MapGridInfo[,] GetMapGridInfo()
		{
			if (this.mMapGridInfo == null)
			{
				this.mMapGridInfo = new MapGridInfo[(int)((UIntPtr)this.mnWidth), (int)((UIntPtr)this.mnHeight)];
			}
			return this.mMapGridInfo;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000DDB8 File Offset: 0x0000BFB8
		public Dictionary<uint, BaseObject> GetAllObject()
		{
			return this.mDicObject;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public uint GetID()
		{
			return this.info.id;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		public GameMap(MapInfo mapinfo)
		{
			this.mListRegionInfo = null;
			this.info = mapinfo;
			this.mDicObject = new Dictionary<uint, BaseObject>();
			this.mListDeleteObj = new List<BaseObject>();
			this.mListAddObj = new List<BaseObject>();
			this.last_null_tick = Environment.TickCount;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000DE40 File Offset: 0x0000C040
		public GameMap Clone()
		{
			GameMap gameMap = new GameMap(this.info);
			gameMap.mnWidth = this.mnWidth;
			gameMap.mnHeight = this.mnHeight;
			gameMap.mPath = new MapPath(this.mnHeight, this.mnWidth);
			for (uint num = 0U; num < this.mnHeight; num += 1U)
			{
				for (uint num2 = 0U; num2 < this.mnWidth; num2 += 1U)
				{
					gameMap.GetMapGridInfo()[(int)((UIntPtr)num2), (int)((UIntPtr)num)] = this.mMapGridInfo[(int)((UIntPtr)num2), (int)((UIntPtr)num)];
					if (gameMap.GetMapGridInfo()[(int)((UIntPtr)num2), (int)((UIntPtr)num)].Mask > 0)
					{
						gameMap.mPath.SetPointMask((short)num2, (short)num, 0);
					}
				}
			}
			foreach (BaseObject baseObject in this.GetAllObject().Values)
			{
				gameMap.AddObject(baseObject, baseObject.GetGameSession());
			}
			return gameMap;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000DF7C File Offset: 0x0000C17C
		public bool Create()
		{
			bool result;
			if (this.info == null)
			{
				result = false;
			}
			else if (!File.Exists(this.info.dmappath))
			{
				Log.Instance().WriteLog("Map file does not exist: " + this.info.dmappath);
				result = false;
			}
			else
			{
				FileStream fileStream = new FileStream(this.info.dmappath, FileMode.Open);
				BinaryReader binaryReader = new BinaryReader(fileStream);
				this.mnVersion = binaryReader.ReadUInt32();
				uint num = binaryReader.ReadUInt32();
				byte[] bytes = binaryReader.ReadBytes(260);
				string @string = Encoding.Default.GetString(bytes);
				uint num2 = binaryReader.ReadUInt32();
				uint num3 = binaryReader.ReadUInt32();
				this.mnWidth = num2;
				this.mnHeight = num3;
				this.mPath = new MapPath(num2, num3);
				this.mMapGridInfo = new MapGridInfo[(int)((UIntPtr)num2), (int)((UIntPtr)num3)];
				for (uint num4 = 0U; num4 < num3; num4 += 1U)
				{
					uint num5 = 0U;
					for (uint num6 = 0U; num6 < num2; num6 += 1U)
					{
						ushort num7 = binaryReader.ReadUInt16();
						ushort num8 = binaryReader.ReadUInt16();
						short num9 = binaryReader.ReadInt16();
						num5 += (uint)num7 * ((uint)num8 + num4 + 1U) + (uint)(num9 + 2) * (num6 + 1U + (uint)num8);
						MapGridInfo mapGridInfo;
						mapGridInfo.Mask = (byte)num7;
						this.mMapGridInfo[(int)((UIntPtr)num6), (int)((UIntPtr)num4)] = mapGridInfo;
						if (num7 > 0)
						{
							this.mPath.SetPointMask((short)num6, (short)num4, 0);
						}
					}
					uint num10 = binaryReader.ReadUInt32();
					if (num10 != num5)
					{
						Log.Instance().WriteLog("Failed to load map file. Path: " + this.info.dmappath);
						return false;
					}
				}
				fileStream.Dispose();
				result = true;
			}
			return result;
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000E16C File Offset: 0x0000C36C
		public void Process()
		{
			if (this.mDicObject.Count != 0 || this.mListDeleteObj.Count != 0 || this.mListAddObj.Count != 0)
			{
				if (this.mListDeleteObj.Count > 0)
				{
					for (int i = 0; i < this.mListDeleteObj.Count; i++)
					{
						BaseObject baseObject = this.mListDeleteObj[i];
						uint key;
						if (baseObject.type == 3 || baseObject.type == 7)
						{
							key = baseObject.GetTypeId();
						}
						else
						{
							key = baseObject.GetGameID();
						}
						if (this.mDicObject.ContainsKey(key))
						{
							this.mDicObject.Remove(key);
						}
					}
					this.mListDeleteObj.Clear();
				}
				if (this.mListAddObj.Count > 0)
				{
					for (int i = 0; i < this.mListAddObj.Count; i++)
					{
						BaseObject baseObject = this.mListAddObj[i];
						if (baseObject.type == 3)
						{
							this.mDicObject[baseObject.GetTypeId()] = baseObject;
						}
						else
						{
							this.mDicObject[baseObject.GetGameID()] = baseObject;
						}
					}
					this.mListAddObj.Clear();
				}
				foreach (BaseObject baseObject in this.mDicObject.Values)
				{
					if (!baseObject.Run())
					{
						this.mListDeleteObj.Add(baseObject);
					}
				}
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000E33C File Offset: 0x0000C53C
		public BaseObject GetObject(uint id)
		{
			BaseObject result;
			if (this.mDicObject.ContainsKey(id))
			{
				result = this.mDicObject[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000E374 File Offset: 0x0000C574
		public MapInfo GetMapInfo()
		{
			return this.info;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000E38C File Offset: 0x0000C58C
		public bool CanMove(short x, short y)
		{
			return (long)x < (long)((ulong)this.mnWidth) && (long)y < (long)((ulong)this.mnHeight) && x >= 0 && y >= 0 && this.mMapGridInfo[(int)x, (int)y].Mask == 0;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000E3E8 File Offset: 0x0000C5E8
		public void SendWeatherInfo(PlayerObject play)
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteInt16(20);
			packetOut.WriteInt16(1110);
			packetOut.WriteUInt32(this.GetMapInfo().id);
			packetOut.WriteUInt32(this.GetMapInfo().id);
			if (this.GetMapInfo().issnows)
			{
				byte[] v = new byte[]
				{
					0,
					0,
					32,
					0,
					128,
					0,
					18,
					0
				};
				packetOut.WriteBuff(v);
			}
			else
			{
				packetOut.WriteInt32(0);
				packetOut.WriteInt32(0);
			}
			play.SendData(packetOut.Flush(), true);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000E488 File Offset: 0x0000C688
		public void AddObject(BaseObject obj, GameSession session = null)
		{
			this.mListAddObj.Add(obj);
			obj.mGameMap = this;
			obj.session = session;
			if (obj.type == 2)
			{
				this.last_null_tick = Environment.TickCount;
			}
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000E4D0 File Offset: 0x0000C6D0
		public void RemoveObj(BaseObject obj)
		{
			uint key;
			if (obj.type == 3)
			{
				key = obj.GetTypeId();
			}
			else
			{
				key = obj.GetGameID();
			}
			if (this.mDicObject.ContainsKey(key))
			{
				if (obj.type == 2)
				{
					PlayerObject playerObject = obj as PlayerObject;
					playerObject.ClearThis();
				}
				if (obj.type == 4)
				{
					EudemonObject eudemonObject = obj as EudemonObject;
					eudemonObject.ReCall();
				}
				if (obj.type == 10)
				{
					PtichObject ptichObject = obj as PtichObject;
					ptichObject.ClearThis();
				}
				this.mListDeleteObj.Add(obj);
			}
			if (this.GetObjectCount(2) == 0)
			{
				this.last_null_tick = Environment.TickCount;
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000E5AC File Offset: 0x0000C7AC
		public void CreateMonster(GeneratorInfo info)
		{
			MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(info.monsterid);
			if (monsterInfo == null)
			{
				Log.Instance().WriteLog("Monster ID was not found: " + info.monsterid.ToString());
			}
			else
			{
				Random random = new Random();
				int num = 0;
				while ((long)num < (long)((ulong)info.amount))
				{
					byte b = 0;
					short num2;
					short num3;
					for (;;)
					{
						num2 = (short)random.Next((int)info.bound_x, (int)(info.bound_x + info.bound_cx));
						num3 = (short)random.Next((int)info.bound_y, (int)(info.bound_y + info.bound_cy));
						if (this.CanMove(num2, num3))
						{
							break;
						}
						b += 1;
						if (b >= 100)
						{
							goto Block_3;
						}
					}
					IL_CF:
					if (num2 == 0 && num3 == 0)
					{
						Log.Instance().WriteLog(string.Concat(new string[]
						{
							"Monster creation failed, unable to find a foothold",
							this.GetMapInfo().name,
							"Monster Name:",
							monsterInfo.name,
							"Map ID:",
							info.mapid.ToString(),
							" x:",
							info.bound_x.ToString(),
							" y:",
							info.bound_y.ToString()
						}));
						break;
					}
					MonsterObject monsterObject = new MonsterObject(monsterInfo.id, monsterInfo.ai, num2, num3, true);
					if (info.dir == 8)
					{
						monsterObject.SetDir(DIR.Random_Dir());
					}
					else
					{
						monsterObject.SetDir(info.dir);
					}
					monsterObject.SetRebirthTime(info.time);
					this.AddObject(monsterObject, null);
					num++;
					continue;
					goto IL_CF;
					Block_3:
					num3 = (num2 = 0);
					goto IL_CF;
				}
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000E7AC File Offset: 0x0000C9AC
		public BaseObject FindObjectForID(uint id)
		{
			BaseObject baseObject = this.FindMonsterObject(id);
			BaseObject result;
			if (baseObject != null)
			{
				result = baseObject;
			}
			else
			{
				foreach (BaseObject baseObject2 in this.mDicObject.Values)
				{
					if (baseObject2.GetTypeId() == id)
					{
						return baseObject2;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000E838 File Offset: 0x0000CA38
		public MonsterObject FindMonsterObject(uint id)
		{
			MonsterObject result;
			if (this.mDicObject.ContainsKey(id))
			{
				result = (this.mDicObject[id] as MonsterObject);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000E874 File Offset: 0x0000CA74
		public void AddDropItemObj(uint itemid, short x, short y, uint ownerid = 0U, int time = 120000, RoleItemInfo info = null, RoleData_Eudemon eudemon = null)
		{
			DropItemObject dropItemObject = new DropItemObject(itemid, x, y, ownerid, time);
			dropItemObject.SetRoleItemInfo(info);
			dropItemObject.SetRoleEudemonInfo(eudemon);
			this.AddObject(dropItemObject, null);
			Log.Instance().WriteLog(string.Format(
				"Ground object created: map={0}, groundId={1}, typeId={2}, x={3}, y={4}, sourceItemId={5}, eudemon={6}",
				this.GetID(),
				dropItemObject.GetGameID(),
				itemid,
				x,
				y,
				info == null ? 0U : info.id,
				eudemon != null));
			dropItemObject.RefreshVisibleObject();
			dropItemObject.BroadcastInfo(1U);
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000E8BC File Offset: 0x0000CABC
		public MapPath GetMapPath()
		{
			return this.mPath;
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000E8D4 File Offset: 0x0000CAD4
		public void BroadcastBuffer(BaseObject obj, byte[] buff)
		{
			foreach (RefreshObject refreshObject in obj.GetVisibleList().Values)
			{
				BaseObject obj2 = refreshObject.obj;
				if (obj2.type == 2 && obj2.GetGameSession() != null)
				{
					BaseMsg baseMsg = new BaseMsg();
					baseMsg.Create(buff, obj2.GetGamePackKeyEx());
					obj2.SendData(baseMsg.GetBuffer(), false);
				}
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000E978 File Offset: 0x0000CB78
		public bool GetPointOfObj(BaseObject obj, short x, short y)
		{
			bool result;
			if (!this.CanMove(x, y))
			{
				result = true;
			}
			else
			{
				obj.RefreshVisibleObject();
				foreach (RefreshObject refreshObject in obj.GetVisibleList().Values)
				{
					BaseObject baseObject = refreshObject.obj;
					if (baseObject.GetCurrentX() == x && baseObject.GetCurrentY() == y)
					{
						return true;
					}
				}
				if (this.mListAddObj.Count > 0)
				{
					for (int i = 0; i < this.mListAddObj.Count; i++)
					{
						BaseObject baseObject = this.mListAddObj[i];
						if (baseObject.GetCurrentX() == x && baseObject.GetCurrentY() == y)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000EA8C File Offset: 0x0000CC8C
		public void BroadcastMsg(BROADCASTMSGTYPE type, string msg)
		{
			foreach (BaseObject baseObject in this.mDicObject.Values)
			{
				if (baseObject.type == 2)
				{
					PlayerObject playerObject = baseObject as PlayerObject;
					if (baseObject.GetGameSession() != null)
					{
						switch (type)
						{
						case BROADCASTMSGTYPE.LEFT:
							playerObject.LeftNotice(msg);
							break;
						case BROADCASTMSGTYPE.CHAT:
							playerObject.ChatNotice(msg);
							break;
						case BROADCASTMSGTYPE.SCREEN:
							UserEngine.Instance().SceneNotice(msg);
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000EB4C File Offset: 0x0000CD4C
		public void AddRegionInfo(MapRegionInfo info)
		{
			if (this.mListRegionInfo == null)
			{
				this.mListRegionInfo = new List<MapRegionInfo>();
			}
			this.mListRegionInfo.Add(info);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000EB84 File Offset: 0x0000CD84
		public bool IsSafeArea(short x, short y)
		{
			bool result;
			if (this.mListRegionInfo == null)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.mListRegionInfo.Count; i++)
				{
					if (this.mListRegionInfo[i].type == 595849)
					{
						int num = Math.Abs((int)(x - this.mListRegionInfo[i].bound_x));
						int num2 = Math.Abs((int)(y - this.mListRegionInfo[i].bound_y));
						if (num <= (int)this.mListRegionInfo[i].bound_cx && num2 <= (int)this.mListRegionInfo[i].bound_cy)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000EC60 File Offset: 0x0000CE60
		public int GetObjectCount(byte type)
		{
			int num = 0;
			foreach (BaseObject baseObject in this.mDicObject.Values)
			{
				if (baseObject.type == type)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04000086 RID: 134
		private MapInfo info;

		// Token: 0x04000087 RID: 135
		public uint mnVersion;

		// Token: 0x04000088 RID: 136
		public uint mnWidth;

		// Token: 0x04000089 RID: 137
		public uint mnHeight;

		// Token: 0x0400008A RID: 138
		private MapGridInfo[,] mMapGridInfo;

		// Token: 0x0400008B RID: 139
		private Dictionary<uint, BaseObject> mDicObject;

		// Token: 0x0400008C RID: 140
		private List<BaseObject> mListDeleteObj;

		// Token: 0x0400008D RID: 141
		private List<BaseObject> mListAddObj;

		// Token: 0x0400008E RID: 142
		private List<MapRegionInfo> mListRegionInfo;

		// Token: 0x0400008F RID: 143
		public int last_null_tick;

		// Token: 0x04000090 RID: 144
		public MapPath mPath;
	}
}
