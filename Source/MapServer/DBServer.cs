using System;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200000E RID: 14
	internal class DBServer
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00008814 File Offset: 0x00006A14
		public bool IsConnect()
		{
			return this.mbConnect;
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000882C File Offset: 0x00006A2C
		public static DBServer Instance()
		{
			if (DBServer.m_Intsance == null)
			{
				DBServer.m_Intsance = new DBServer();
			}
			return DBServer.m_Intsance;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000885E File Offset: 0x00006A5E
		public DBServer()
		{
			this.mbConnect = false;
			this.mnReconnectTick = Environment.TickCount;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000887C File Offset: 0x00006A7C
		public void Init()
		{
			MemIniFile memIniFile = new MemIniFile();
			if (!memIniFile.LoadFromFile("../GlobalConfig.ini"))
			{
				Log.Instance().WriteLog("load golbalconfig error!");
			}
			else
			{
				this.mDBPacket = new InternalPacket();
				this.mTcpDBClient = new GameBase.Network.TcpClient();
				this.mTcpDBClient.onConnect += this.OnDBConnectEventHandler;
				this.mTcpDBClient.onReceive += this.OnDBReceiveEventHandler;
				this.mTcpDBClient.onClose += this.OnDBClose;
				string ip = memIniFile.ReadValue("DBServer", "IP", "0.0.0.0");
				int port = memIniFile.ReadValue("DBServer", "Port", 1500);
				this.mTcpDBClient.Connect(ip, port);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00008950 File Offset: 0x00006B50
		private void OnDBConnectEventHandler(bool isSucceed)
		{
			if (isSucceed)
			{
				Log.Instance().WriteLog("dbserver connect success!");
				OpenMapSession openMapSession = new OpenMapSession();
				this.mTcpDBClient.SendData(openMapSession.GetBuff());
				this.mbConnect = true;
			}
			else
			{
				Log.Instance().WriteLog("dbserver connect error!");
				Log.Instance().WriteLog("Reconnect  dbserver ip:" + this.mTcpDBClient.GetConnectIP() + " port:" + this.mTcpDBClient.GetConnectPort().ToString());
				this.mbConnect = false;
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x000089EC File Offset: 0x00006BEC
		private void OnDBReceiveEventHandler(byte[] data, int nSize)
		{
			lock (DBServer._lock)
			{
				byte[] array = new byte[nSize];
				Buffer.BlockCopy(data, 0, array, 0, nSize);
				this.mDBPacket.ProcessNetMsg(array);
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00008A50 File Offset: 0x00006C50
		private void OnDBClose(Socket s)
		{
			lock (DBServer._lock)
			{
				this.mbConnect = false;
				this.mDBPacket.ClearPacket();
				Log.Instance().WriteLog("dbserver close!!!reconnect ");
				this.mTcpDBClient.SetSocket(null);
				this.mnReconnectTick = Environment.TickCount;
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00008AD0 File Offset: 0x00006CD0
		public void ProcessDBNetMsg()
		{
			if (!this.mbConnect && Environment.TickCount - this.mnReconnectTick > 5000)
			{
				this.mTcpDBClient.ReConnect();
				this.mnReconnectTick = Environment.TickCount;
			}
			byte[] array = null;
			lock (DBServer._lock)
			{
				array = this.mDBPacket.GetData();
			}
			if (array != null)
			{
				PackIn packIn = new PackIn(array);
				ushort num = packIn.ReadUInt16();
				ushort num2 = num;
				switch (num2)
				{
				case 112:
				{
					RoleInfo roleInfo = new RoleInfo(array);
					PlayerObject cachePlay = UserEngine.Instance().GetCachePlay(roleInfo.sAccount);
					if (cachePlay != null)
					{
						Log.Instance().WriteLog("Cached role data detected; saving: " + cachePlay.GetName());
						UserEngine.Instance().RemoveCachePlay(cachePlay);
						cachePlay.ExitGame();
					}
					else
					{
						UserEngine.Instance().AddTempPlayObject(roleInfo);
						RoleInfo_Ret roleInfo_Ret = new RoleInfo_Ret();
						roleInfo_Ret.gameid = roleInfo.gameid;
						roleInfo_Ret.key = roleInfo.mKey;
						roleInfo_Ret.key2 = roleInfo.mKey1;
						roleInfo_Ret.accountid = roleInfo.accountid;
						this.mTcpDBClient.SendData(roleInfo_Ret.GetBuffer());
						Log.Instance().WriteLog("Received temporary role data: " + roleInfo.sAccount + " id:" + roleInfo.accountid.ToString());
					}
					break;
				}
				case 113:
				case 114:
					break;
				case 115:
				{
					QueryRoleName_Ret queryRoleName_Ret = new QueryRoleName_Ret();
					queryRoleName_Ret.Create(array);
					TempPlayObject tempPlayObj = UserEngine.Instance().GetTempPlayObj(queryRoleName_Ret.gameid);
					if (tempPlayObj == null)
					{
						Log.Instance().WriteLog(
							"Character-name result discarded: temporary login state was not found.");
					}
					else
					{
						Log.Instance().WriteLog("Character-name query result: available=" +
							(!queryRoleName_Ret.tag).ToString());
						MsgNotice msgNotice = new MsgNotice();
						msgNotice.Create(null, tempPlayObj.play.GetGamePackKeyEx());
						tempPlayObj.play.SendData(msgNotice.GetQueryNameBuff(!queryRoleName_Ret.tag), false);
					}
					break;
				}
				case 116:
				{
					CreateRole_Ret createRole_Ret = new CreateRole_Ret();
					createRole_Ret.Create(array);
					TempPlayObject tempPlayObj = UserEngine.Instance().GetTempPlayObj(createRole_Ret.gameid);
					if (tempPlayObj == null)
					{
						Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 3.");
					}
					else
					{
						Log.Instance().WriteLog("Character creation result: success=" +
							createRole_Ret.tag.ToString() + ", player id=" +
							createRole_Ret.playerid.ToString());
						UserEngine.Instance().RemoveTempPlayObject(createRole_Ret.gameid);
						tempPlayObj.play.GetBaseAttr().account_id = tempPlayObj.accountid;
						tempPlayObj.play.GetBaseAttr().player_id = createRole_Ret.playerid;
						tempPlayObj.play.EnterGame(null, true);
					}
					break;
				}
				default:
					switch (num2)
					{
					case 121:
					{
						AddRoleData_Item_Ret addRoleData_Item_Ret = new AddRoleData_Item_Ret();
						addRoleData_Item_Ret.Create(array);
						PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToID(addRoleData_Item_Ret.gameid);
						if (playerObject == null)
						{
							Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 4.");
						}
						else
						{
							playerObject.GetItemSystem().AwardItem_Ret(addRoleData_Item_Ret.sordid, addRoleData_Item_Ret.id);
						}
						break;
					}
					case 123:
					{
						ROLEDATA_ITEM roledata_ITEM = new ROLEDATA_ITEM();
						roledata_ITEM.Create(array);
						TempPlayObject tempPlayObj2 = UserEngine.Instance().GetTempPlayObj(roledata_ITEM.key, roledata_ITEM.key2);
						if (tempPlayObj2 == null)
						{
							Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 5.");
						}
						else
						{
							for (int i = 0; i < roledata_ITEM.mListItem.Count; i++)
							{
								tempPlayObj2.play.GetItemSystem().AddItemInfo(roledata_ITEM.mListItem[i]);
							}
						}
						break;
					}
					case 125:
					{
						RoleData_Magic roleData_Magic = new RoleData_Magic();
						roleData_Magic.Create(array);
						TempPlayObject tempPlayObj2 = UserEngine.Instance().GetTempPlayObj(roleData_Magic.key, roleData_Magic.key2);
						if (tempPlayObj2 == null)
						{
							Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 6.");
						}
						else
						{
							for (int i = 0; i < roleData_Magic.mListMagic.Count; i++)
							{
								MagicInfo info = roleData_Magic.mListMagic[i];
								tempPlayObj2.play.GetMagicSystem().AddMagicInfo(info);
							}
						}
						break;
					}
					case 127:
					{
						KickGamePlay kickGamePlay = new KickGamePlay();
						kickGamePlay.Create(array);
						PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToAccountId(kickGamePlay.accountid);
						if (playerObject != null)
						{
							SessionManager.Instance().RemoveSession(playerObject.GetGameSession().m_Socket);
							playerObject.Kick();
						}
						break;
					}
					case 128:
					{
						ROLEDATE_EUDEMON roledate_EUDEMON = new ROLEDATE_EUDEMON();
						roledate_EUDEMON.Create(array);
						TempPlayObject tempPlayObj2 = UserEngine.Instance().GetTempPlayObj(roledate_EUDEMON.key, roledate_EUDEMON.key2);
						if (tempPlayObj2 == null)
						{
							PlayerObject playerObject2 = UserEngine.Instance().FindPlayerObjectToPlayerId(roledate_EUDEMON.playerid);
							if (playerObject2 == null)
							{
								Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 6.");
							}
							else
							{
								playerObject2.GetEudemonSystem().DB_Load(roledate_EUDEMON);
								playerObject2.GetEudemonSystem().SendAllEudemonInfo();
							}
						}
						else
						{
							tempPlayObj2.play.GetEudemonSystem().DB_Load(roledate_EUDEMON);
						}
						break;
					}
					case 130:
					{
						ROLEDATA_FRIEND roledata_FRIEND = new ROLEDATA_FRIEND();
						roledata_FRIEND.Create(array);
						TempPlayObject tempPlayObj2 = UserEngine.Instance().GetTempPlayObj(roledata_FRIEND.key, roledata_FRIEND.key2);
						if (tempPlayObj2 == null)
						{
							PlayerObject playerObject2 = UserEngine.Instance().FindPlayerObjectToPlayerId(roledata_FRIEND.playerid);
							if (playerObject2 == null)
							{
								Log.Instance().WriteLog("Player object not found in ProcessDBNetMsg code 7.");
							}
							else
							{
								playerObject2.GetFriendSystem().DB_Load(roledata_FRIEND);
								playerObject2.GetFriendSystem().SendAllFriendInfo();
							}
						}
						else
						{
							tempPlayObj2.play.GetFriendSystem().DB_Load(roledata_FRIEND);
						}
						break;
					}
					case 132:
					{
						GUANJUEINFO guanjueinfo = new GUANJUEINFO();
						guanjueinfo.Create(array);
						GuanJueManager.Instance().DB_Load(guanjueinfo);
						break;
					}
					case 134:
					{
						LEGIONINFO legioninfo = new LEGIONINFO();
						legioninfo.Create(array);
						LegionManager.Instance().DB_Load(legioninfo);
						break;
					}
					case 138:
					{
						CreateLegion_Ret createLegion_Ret = new CreateLegion_Ret();
						createLegion_Ret.Create(array);
						LegionManager.Instance().CreateLegion_Ret(createLegion_Ret);
						break;
					}
					case 139:
					{
						PackPayRecInfo packPayRecInfo = new PackPayRecInfo();
						packPayRecInfo.Creaet(array);
						PayManager.Instance().DB_Load(packPayRecInfo);
						break;
					}
					case FamilyCollection.Parameter:
					{
						FamilyCollection collection = new FamilyCollection();
						collection.Create(array);
						FamilyManager.Instance().DB_Load(collection);
						break;
					}
					case CreateFamilyResult.Parameter:
					{
						CreateFamilyResult result = new CreateFamilyResult();
						result.Create(array);
						FamilyManager.Instance().HandleCreateResult(result);
						break;
					}
					}
					break;
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000091DC File Offset: 0x000073DC
		public GameBase.Network.TcpClient GetDBClient()
		{
			return this.mTcpDBClient;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000091F4 File Offset: 0x000073F4
		public void SaveRoleData(PlayerObject play, bool isExit = false)
		{
			if (!this.IsConnect())
			{
				UserEngine.Instance().AddCachePlay(play);
				Log.Instance().WriteLog("Failed to save player data because DBServer is disconnected; queued in the database buffer.");
			}
			else
			{
				SaveRoleData_Attr saveRoleData_Attr = new SaveRoleData_Attr();
				PlayerAttribute baseAttr = play.GetBaseAttr();
				saveRoleData_Attr.accountid = baseAttr.account_id;
				saveRoleData_Attr.IsExit = isExit;
				saveRoleData_Attr.name = play.GetName();
				saveRoleData_Attr.lookface = baseAttr.lookface;
				saveRoleData_Attr.hair = baseAttr.hair;
				saveRoleData_Attr.level = baseAttr.level;
				saveRoleData_Attr.exp = baseAttr.exp;
				saveRoleData_Attr.life = baseAttr.life;
				saveRoleData_Attr.mana = baseAttr.mana;
				saveRoleData_Attr.profession = baseAttr.profession;
				saveRoleData_Attr.pk = baseAttr.pk;
				saveRoleData_Attr.gold = (long)baseAttr.gold;
				saveRoleData_Attr.gamegold = (long)baseAttr.gamegold;
				saveRoleData_Attr.stronggold = baseAttr.stronggold;
				saveRoleData_Attr.godlevel = baseAttr.godlevel;
				saveRoleData_Attr.maxeudemon = baseAttr.maxeudemon;
				saveRoleData_Attr.wardrobeHairs =
					play.GetWardrobeSystem().GetOwnedHairStyles();
				saveRoleData_Attr.wardrobeAvatars =
					play.GetWardrobeSystem().GetOwnedAvatarStyles();
				if (play.GetGameMap() == null)
				{
					saveRoleData_Attr.mapid = 1000U;
					saveRoleData_Attr.x = 145;
					saveRoleData_Attr.y = 413;
				}
				else
				{
					saveRoleData_Attr.mapid = play.GetGameMap().GetMapInfo().id;
					saveRoleData_Attr.x = play.GetCurrentX();
					saveRoleData_Attr.y = play.GetCurrentY();
				}
				saveRoleData_Attr.hotkey = play.GetHotKeyInfo();
				saveRoleData_Attr.guanjue = baseAttr.guanjue;
				this.GetDBClient().SendData(saveRoleData_Attr.GetBuffer());
				play.GetItemSystem().DB_Save();
				play.GetMagicSystem().DB_Save();
				play.GetEudemonSystem().DB_Save();
				play.GetFriendSystem().DB_Save();
			}
		}

		// Token: 0x04000063 RID: 99
		private static DBServer m_Intsance = null;

		// Token: 0x04000064 RID: 100
		private GameBase.Network.TcpClient mTcpDBClient;

		// Token: 0x04000065 RID: 101
		private bool mbConnect;

		// Token: 0x04000066 RID: 102
		private InternalPacket mDBPacket;

		// Token: 0x04000067 RID: 103
		private static object _lock = new object();

		// Token: 0x04000068 RID: 104
		private int mnReconnectTick;
	}
}
