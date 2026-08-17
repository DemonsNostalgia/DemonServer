using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Network;
using GameBase.Network.Internal;
using NetMsg;

namespace MapServer
{
	// Token: 0x020000A1 RID: 161
	public class SessionManager
	{
		// Token: 0x06000436 RID: 1078 RVA: 0x00032898 File Offset: 0x00030A98
		public static SessionManager Instance()
		{
			if (SessionManager.m_Instance == null)
			{
				SessionManager.m_Instance = new SessionManager();
			}
			return SessionManager.m_Instance;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x000328CA File Offset: 0x00030ACA
		public SessionManager()
		{
			this.m_DicSession = new Dictionary<Socket, GameSession>();
			this.m_DicSession.Clear();
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x000328F4 File Offset: 0x00030AF4
		public void Dispose()
		{
			foreach (GameSession gameSession in this.m_DicSession.Values)
			{
				gameSession.Dispose();
			}
			this.m_DicSession.Clear();
			this.m_DicSession = null;
			SessionManager.m_Instance = null;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00032970 File Offset: 0x00030B70
		public void AddSession(Socket s, TcpServer server)
		{
			string remoteEndpoint = SessionManager.GetRemoteEndpoint(s);
			if (remoteEndpoint == null)
			{
				Log.Instance().WriteLog("Ignored disconnected game client during session setup.");
				return;
			}
			if (this.IsSession(s))
			{
				Log.Instance().WriteLog("Reusing socket.");
			}
			GameSession value = new GameSession(s, server);
			this.m_DicSession[s] = value;
			Log.Instance().WriteLog("Game client connected: " + remoteEndpoint);
		}

		private static string GetRemoteEndpoint(Socket s)
		{
			try
			{
				return s.RemoteEndPoint == null ? null : s.RemoteEndPoint.ToString();
			}
			catch (ObjectDisposedException)
			{
				return null;
			}
			catch (SocketException)
			{
				return null;
			}
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x000329B4 File Offset: 0x00030BB4
		public void RemoveSession(Socket s)
		{
			if (this.IsSession(s))
			{
				GameSession gameSession = this.m_DicSession[s];
				this.m_DicSession.Remove(s);
				Log.Instance().WriteLog("Game client disconnected.");
			}
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x000329EC File Offset: 0x00030BEC
		public bool IsSession(Socket s)
		{
			return this.m_DicSession.ContainsKey(s);
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00032A0C File Offset: 0x00030C0C
		public void AddNetData(Socket s, byte[] data, int nLen)
		{
			if (this.IsSession(s))
			{
				GameSession gameSession = this.m_DicSession[s];
				if (gameSession != null && gameSession.m_GamePack != null)
				{
					byte[] array = new byte[nLen];
					Buffer.BlockCopy(data, 0, array, 0, nLen);
					gameSession.m_GamePack.ProcessNetData(array);
					gameSession.m_nLastTime = Environment.TickCount;
				}
			}
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00032A7C File Offset: 0x00030C7C
		public void ProcessNetMsg()
		{
			foreach (GameSession gameSession in this.m_DicSession.Values)
			{
				if (gameSession != null && gameSession.m_GamePack != null)
				{
					byte[] data = gameSession.m_GamePack.GetData();
					if (data != null)
					{
					PackIn packIn = new PackIn(data);
					PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToID(gameSession.gameid);
					ushort num = packIn.ReadUInt16();
					#if DEBUG
					Log.Instance().WriteLog("Decoded client packet type " + num.ToString() +
						", payload bytes " + data.Length.ToString() +
						", active role=" + (playerObject != null).ToString());
					#endif
					if (playerObject != null)
					{
						if (!SessionManager.IsRecognizedClientPacket(num))
						{
							#if DEBUG
							Log.Instance().WriteLog("Unhandled client packet type " +
								num.ToString() + ", decrypted payload=" +
								BitConverter.ToString(data));
							#else
							Log.Instance().WriteLog("Unhandled client packet type " +
								num.ToString() + ".");
							#endif
						}
						playerObject.ProcessNetMsg(num, data);
						}
						else if (playerObject != null || num == 1052 || num == 1158 || num == 1001)
						{
							ushort num2 = num;
							if (num2 != 1001)
							{
								if (num2 != 1052)
								{
									if (num2 == 1158)
									{
										MsgQueryCreateRoleName msgQueryCreateRoleName = new MsgQueryCreateRoleName();
										msgQueryCreateRoleName.Create(data, null);
										Log.Instance().WriteLog("Character-name query received: " +
											msgQueryCreateRoleName.GetName());
										int key = 0;
										int key2 = 0;
										gameSession.GetGamePackKeyEx().GetKey(ref key, ref key2);
										TempPlayObject tempPlayObj = UserEngine.Instance().GetTempPlayObj(key, key2);
										if (tempPlayObj == null)
										{
											Log.Instance().WriteLog(
												"Character-name query rejected: temporary login state was not found.");
										}
										else
										{
											QueryRoleName queryRoleName = new QueryRoleName();
											queryRoleName.gameid = tempPlayObj.play.GetGameID();
											queryRoleName.name = msgQueryCreateRoleName.GetName();
											DBServer.Instance().GetDBClient().SendData(queryRoleName.GetBuffer());
										}
									}
								}
								else
								{
									MapConnectPacket mapConnect;
									string mapConnectError;
									if (!MapPacketCodec.TryReadMapConnect(
										data, out mapConnect, out mapConnectError))
									{
										Log.Instance().WriteLog(
											"MapServer key update rejected: " +
											mapConnectError + "; payload length=" +
											(data == null ? "<null>" : data.Length.ToString()) +
											", bytes=" +
											(data == null ? "<null>" : BitConverter.ToString(data)) + ".");
										break;
									}
									int key = unchecked((int)mapConnect.Key1);
									int key2 = unchecked((int)mapConnect.Key2);
									TempPlayObject tempPlayObj2 = UserEngine.Instance().GetTempPlayObj(key, key2);
									if (tempPlayObj2 == null)
									{
										Log.Instance().WriteLog(
											"MapServer key update rejected: temporary login state was not found.");
										break;
									}
									Log.Instance().WriteLog("MapServer key update matched account " +
										tempPlayObj2.accountid.ToString() + ", has role=" +
										tempPlayObj2.isRole.ToString());
									tempPlayObj2.play.SetGameSession(gameSession);
									gameSession.GetGamePackKeyEx().SunUpdateKey(key, key2);
									if (!tempPlayObj2.isRole)
									{
										MsgNotice msgNotice = new MsgNotice();
										msgNotice.Create(null, gameSession.GetGamePackKeyEx());
										gameSession.SendData(msgNotice.GetCreateRoleBuff());
										Log.Instance().WriteLog("Sent character-creation prompt.");
										break;
									}
									UserEngine.Instance().RemoveTempPlayObject(tempPlayObj2.play.GetGameID());
									tempPlayObj2.play.EnterGame(gameSession, false);
								}
							}
							else
							{
								int key = 0;
								int key2 = 0;
								gameSession.GetGamePackKeyEx().GetKey(ref key, ref key2);
								TempPlayObject tempPlayObj = UserEngine.Instance().GetTempPlayObj(key, key2);
								if (tempPlayObj == null)
								{
									Log.Instance().WriteLog(
										"Character creation rejected: temporary login state was not found.");
								}
								else
								{
									MsgCreateRoleInfo msgCreateRoleInfo = new MsgCreateRoleInfo();
									msgCreateRoleInfo.Create(data, null);
									Log.Instance().WriteLog("Character creation packet received: name=" +
										msgCreateRoleInfo.GetName() + ", profession=" +
										msgCreateRoleInfo.profession.ToString() + ", lookface=" +
										msgCreateRoleInfo.lookface.ToString());
									if (msgCreateRoleInfo.GetName().Length <= 0)
									{
										Log.Instance().WriteLog("Role name is empty.");
									}
									else
									{
										PlayerObject play = tempPlayObj.play;
										play.SetGameSession(gameSession);
										play.SetName(msgCreateRoleInfo.GetName());
										play.GetBaseAttr().profession = (byte)msgCreateRoleInfo.profession;
										play.GetBaseAttr().lookface = msgCreateRoleInfo.lookface;
										CreateRole createRole = new CreateRole();
										createRole.accountid = tempPlayObj.accountid;
										createRole.lookface = msgCreateRoleInfo.lookface;
										createRole.name = msgCreateRoleInfo.GetName();
										createRole.profession = (byte)msgCreateRoleInfo.profession;
										createRole.gameid = tempPlayObj.play.GetGameID();
										DBServer.Instance().GetDBClient().SendData(createRole.GetBuffer());
										Log.Instance().WriteLog(
											"Sent character creation request to DBServer.");
									}
								}
							}
						}
						else
						{
							Log.Instance().WriteLog("Unhandled pre-role client packet type " +
								num.ToString());
						}
					}
				}
			}
		}

		private static bool IsRecognizedClientPacket(ushort packetType)
		{
			switch (packetType)
			{
			case 1004:
			case 1009:
			case 1010:
			case 1015:
			case 1019:
			case 1022:
			case 1023:
			case 1028:
			case 1032:
			case 1049:
			case 1056:
			case 1101:
			case 1102:
			case 1107:
			case 1112:
			case 1117:
			case 1123:
			case 1142:
			case 2031:
			case 2032:
			case 2036:
			case 2060:
			case 3005:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x04000681 RID: 1665
		private Dictionary<Socket, GameSession> m_DicSession = null;

		// Token: 0x04000682 RID: 1666
		private static SessionManager m_Instance = null;
	}
}
