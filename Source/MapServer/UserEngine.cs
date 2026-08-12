using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x020000AA RID: 170
	public class UserEngine
	{
		// Token: 0x0600045C RID: 1116 RVA: 0x00033654 File Offset: 0x00031854
		public UserEngine()
		{
			this.m_DicPlayerObject = new Dictionary<uint, PlayerObject>();
			this.m_DicPlayerObject.Clear();
			this.m_DicTempPlayObject = new Dictionary<uint, TempPlayObject>();
			this.m_DicTempPlayObject.Clear();
			this.mListCacheRole = new List<PlayerObject>();
			this.mListCacheRole.Clear();
			this.mListSaveRole = new List<PlayerObject>();
			this.mnCachePlaySaveTick = Environment.TickCount;
		}

		// Token: 0x0600045D RID: 1117 RVA: 0x000336D4 File Offset: 0x000318D4
		public static UserEngine Instance()
		{
			if (UserEngine.m_Instance == null)
			{
				UserEngine.m_Instance = new UserEngine();
			}
			return UserEngine.m_Instance;
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x00033708 File Offset: 0x00031908
		public PlayerObject CreatePlayObject()
		{
			PlayerObject playerObject = new PlayerObject();
			this.m_DicPlayerObject[playerObject.GetGameID()] = playerObject;
			return playerObject;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00033734 File Offset: 0x00031934
		public PlayerObject FindPlayerObjectToSocket(Socket s)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				GameSession gameSession = playerObject.GetGameSession();
				if (gameSession != null && gameSession.m_Socket == s)
				{
					return playerObject;
				}
			}
			return null;
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x000337BC File Offset: 0x000319BC
		public PlayerObject FindPlayerObjectToID(uint id)
		{
			PlayerObject result;
			if (this.m_DicPlayerObject.ContainsKey(id))
			{
				result = this.m_DicPlayerObject[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000337F4 File Offset: 0x000319F4
		public PlayerObject FindPlayerObjectToTypeID(uint id)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				if (playerObject.GetTypeId() == id)
				{
					return playerObject;
				}
			}
			return null;
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00033868 File Offset: 0x00031A68
		public PlayerObject FindPlayerObjectToPlayerId(int play_id)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				if (playerObject.GetBaseAttr().player_id == play_id)
				{
					return playerObject;
				}
			}
			return null;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x000338E4 File Offset: 0x00031AE4
		public PlayerObject FindPlayerObjectToAccountId(int Accountid)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				if (playerObject.GetBaseAttr().account_id == Accountid)
				{
					return playerObject;
				}
			}
			return null;
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00033960 File Offset: 0x00031B60
		public PlayerObject FindPlayerObjectToName(string name)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				if (playerObject.GetName() == name)
				{
					return playerObject;
				}
			}
			return null;
		}

		public void BroadcastLegionPayload(
			PlayerObject sender,
			Legion legion,
			byte[] payload)
		{
			if (legion == null || payload == null)
			{
				return;
			}
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16((ushort)(payload.Length + 2));
			packetOut.WriteBuff(payload);
			byte[] packet = packetOut.Flush();
			foreach (PlayerObject player in this.m_DicPlayerObject.Values)
			{
				if (player == sender || player.GetGameSession() == null ||
					player.GetLegionSystem().GetLegion() != legion)
				{
					continue;
				}
				BaseMsg encrypted = new BaseMsg();
				encrypted.Create(packet, player.GetGamePackKeyEx());
				player.SendData(encrypted.GetBuffer(), false);
			}
		}

		public void BroadcastFamilyPayload(
			PlayerObject sender,
			FamilyInfo family,
			byte[] payload)
		{
			if (family == null || payload == null)
			{
				return;
			}
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16((ushort)(payload.Length + 2));
			packetOut.WriteBuff(payload);
			byte[] packet = packetOut.Flush();
			foreach (PlayerObject player in this.m_DicPlayerObject.Values)
			{
				if (player == sender || player.GetGameSession() == null ||
					player.GetFamilySystem().GetFamily() != family)
				{
					continue;
				}
				BaseMsg encrypted = new BaseMsg();
				encrypted.Create(packet, player.GetGamePackKeyEx());
				player.SendData(encrypted.GetBuffer(), false);
			}
		}

		public void BroadcastEnemyStatus(PlayerObject subject, byte action)
		{
			if (subject == null ||
				(action != MsgFriendInfo.TYPE_ENEMY_ONLINE &&
				 action != MsgFriendInfo.TYPE_ENEMY_OFFLINE))
			{
				return;
			}
			foreach (PlayerObject player in this.m_DicPlayerObject.Values)
			{
				if (player == subject || player.GetGameSession() == null)
				{
					continue;
				}
				player.GetFriendSystem().SendEnemyPresence(subject, action);
			}
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x000339D8 File Offset: 0x00031BD8
		public void RemovePlayObjectToSocket(Socket s)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				GameSession gameSession = playerObject.GetGameSession();
				if (gameSession != null && gameSession.m_Socket == s)
				{
					playerObject.Dispose();
					this.m_DicPlayerObject.Remove(playerObject.GetID());
					break;
				}
			}
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00033A6C File Offset: 0x00031C6C
		public void RemovePlayObject(PlayerObject obj)
		{
			if (obj != null)
			{
				if (obj.GetGameMap() != null)
				{
					obj.GetGameMap().RemoveObj(obj);
				}
				obj.Dispose();
				if (this.m_DicPlayerObject.ContainsKey(obj.GetGameID()))
				{
					this.m_DicPlayerObject.Remove(obj.GetGameID());
				}
			}
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00033AD6 File Offset: 0x00031CD6
		public void AddPlayerObject(PlayerObject obj)
		{
			this.m_DicPlayerObject[obj.GetGameID()] = obj;
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00033AEC File Offset: 0x00031CEC
		public void Run()
		{
			if (this.mListSaveRole.Count > 0)
			{
				DBServer.Instance().SaveRoleData(this.mListSaveRole[0], false);
				this.mListSaveRole.RemoveAt(0);
			}
			if (DBServer.Instance().IsConnect() && this.mListCacheRole.Count > 0)
			{
				if (Environment.TickCount - this.mnCachePlaySaveTick > 5000)
				{
					Log.Instance().WriteLog("Saving queued player data. Player name: " + this.mListCacheRole[0].GetName());
					this.mnCachePlaySaveTick = Environment.TickCount;
					DBServer.Instance().SaveRoleData(this.mListCacheRole[0], false);
					this.mListCacheRole.RemoveAt(0);
				}
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00033BD0 File Offset: 0x00031DD0
		public void SceneNotice(string text)
		{
			MsgNotice msgNotice = new MsgNotice();
			msgNotice.Create(null, null);
			byte[] sceneNoticeBuff = msgNotice.GetSceneNoticeBuff(text);
			UserEngine.Instance().BrocatBuffer(sceneNoticeBuff);
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00033C04 File Offset: 0x00031E04
		public void AddTempPlayObject(RoleInfo info)
		{
			TempPlayObject tempPlayObject = new TempPlayObject();
			PlayerObject playerObject = new PlayerObject();
			tempPlayObject.play = playerObject;
			tempPlayObject.key = info.mKey;
			tempPlayObject.key2 = info.mKey1;
			tempPlayObject.isRole = info.isRole;
			tempPlayObject.accountid = info.accountid;
			this.m_DicTempPlayObject[playerObject.GetGameID()] = tempPlayObject;
			if (tempPlayObject.isRole)
			{
				playerObject.SetName(info.name);
				PlayerAttribute baseAttr = playerObject.GetBaseAttr();
				baseAttr.account_id = info.accountid;
				baseAttr.player_id = info.playerid;
				baseAttr.mana = info.mana;
				baseAttr.lookface = info.lookface;
				baseAttr.hair = info.hair;
				baseAttr.profession = info.profession;
				baseAttr.level = info.lv;
				baseAttr.exp = (int)info.exp;
				baseAttr.life = info.life;
				baseAttr.pk = info.pk;
				baseAttr.gold = info.gold;
				baseAttr.gamegold = info.gamegold;
				baseAttr.stronggold = (long)info.stronggold;
				baseAttr.mapid = (uint)info.mapid;
				baseAttr.guanjue = info.guanjue;
				baseAttr.sAccount = info.sAccount;
				baseAttr.godlevel = (byte)info.godlevel;
				baseAttr.maxeudemon = info.maxeudemon;
				baseAttr.vip = info.vip;
				playerObject.SetHotKeyInfo(info.hotkey);
				playerObject.CalcSex();
				playerObject.GetWardrobeSystem().LoadOwned(
					info.wardrobeHairs);
				playerObject.GetWardrobeSystem().LoadOwnedAvatars(
					info.wardrobeAvatars);
				playerObject.SetPoint(info.x, info.y);
				GUANGJUELEVEL level = GuanJueManager.Instance().GetLevel(playerObject);
				playerObject.SetGuanJue(level);
				playerObject.GetLegionSystem().Init(null);
				playerObject.GetFamilySystem().Init();
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00033DB4 File Offset: 0x00031FB4
		public void RemoveTempPlayObject(int key, int key2)
		{
			foreach (TempPlayObject tempPlayObject in this.m_DicTempPlayObject.Values)
			{
				if (tempPlayObject.key == key && tempPlayObject.key2 == key2)
				{
					this.m_DicTempPlayObject.Remove(tempPlayObject.play.GetGameID());
					break;
				}
			}
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00033E44 File Offset: 0x00032044
		public void RemoveTempPlayObject(uint gameid)
		{
			if (this.m_DicTempPlayObject.ContainsKey(gameid))
			{
				this.m_DicTempPlayObject.Remove(gameid);
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x00033E74 File Offset: 0x00032074
		public TempPlayObject GetTempPlayObj(int key, int key2)
		{
			foreach (TempPlayObject tempPlayObject in this.m_DicTempPlayObject.Values)
			{
				if (tempPlayObject.key == key && tempPlayObject.key2 == key2)
				{
					return tempPlayObject;
				}
			}
			return null;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x00033EF8 File Offset: 0x000320F8
		public TempPlayObject GetTempPlayObj(uint gameid)
		{
			TempPlayObject result;
			if (this.m_DicTempPlayObject.ContainsKey(gameid))
			{
				result = this.m_DicTempPlayObject[gameid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00033F30 File Offset: 0x00032130
		public void BrocatBuffer(byte[] data)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				BaseMsg baseMsg = new BaseMsg();
				baseMsg.Create(data, playerObject.GetGamePackKeyEx());
				playerObject.SendData(baseMsg.GetBuffer(), false);
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x00033FAC File Offset: 0x000321AC
		public void BroadcastMsg(BROADCASTMSGTYPE type, string msg)
		{
			foreach (PlayerObject playerObject in this.m_DicPlayerObject.Values)
			{
				if (playerObject.GetGameSession() != null)
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

		// Token: 0x06000471 RID: 1137 RVA: 0x00034054 File Offset: 0x00032254
		public void AddSaveRole(PlayerObject play)
		{
			for (int i = 0; i < this.mListSaveRole.Count; i++)
			{
				if (this.mListSaveRole[i].GetGameID() == play.GetGameID())
				{
					return;
				}
			}
			this.mListSaveRole.Add(play);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x000340B0 File Offset: 0x000322B0
		public int GetOnlineCount()
		{
			return this.m_DicPlayerObject.Count;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x000340D0 File Offset: 0x000322D0
		public void RemoveCachePlay(PlayerObject play)
		{
			for (int i = 0; i < this.mListCacheRole.Count; i++)
			{
				if (this.mListCacheRole[i].GetBaseAttr().sAccount == play.GetBaseAttr().sAccount)
				{
					this.mListCacheRole.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00034138 File Offset: 0x00032338
		public void AddCachePlay(PlayerObject play)
		{
			for (int i = 0; i < this.mListCacheRole.Count; i++)
			{
				if (this.mListCacheRole[i].GetBaseAttr().sAccount == play.GetBaseAttr().sAccount)
				{
					this.mListCacheRole.RemoveAt(i);
					break;
				}
			}
			this.mListCacheRole.Add(play);
		}

		// Token: 0x06000475 RID: 1141 RVA: 0x000341AC File Offset: 0x000323AC
		public PlayerObject GetCachePlay(string sAccount)
		{
			for (int i = 0; i < this.mListCacheRole.Count; i++)
			{
				if (this.mListCacheRole[i].GetBaseAttr().sAccount == sAccount)
				{
					return this.mListCacheRole[i];
				}
			}
			return null;
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00034210 File Offset: 0x00032410
		public void Stop()
		{
			List<PlayerObject> list = new List<PlayerObject>();
			foreach (PlayerObject item in this.m_DicPlayerObject.Values)
			{
				list.Add(item);
			}
			for (int i = 0; i < list.Count; i++)
			{
				list[i].ExitGame();
			}
			Log.Instance().WriteLog("Server is shutting down.");
		}

		// Token: 0x0400069D RID: 1693
		private static UserEngine m_Instance = null;

		// Token: 0x0400069E RID: 1694
		private Dictionary<uint, PlayerObject> m_DicPlayerObject = null;

		// Token: 0x0400069F RID: 1695
		private Dictionary<uint, TempPlayObject> m_DicTempPlayObject = null;

		// Token: 0x040006A0 RID: 1696
		private List<PlayerObject> mListSaveRole;

		// Token: 0x040006A1 RID: 1697
		private List<PlayerObject> mListCacheRole;

		// Token: 0x040006A2 RID: 1698
		private int mnCachePlaySaveTick;
	}
}
