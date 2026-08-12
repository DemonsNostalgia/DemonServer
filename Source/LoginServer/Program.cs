using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;

namespace LoginServer
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			Log.Instance().Init("./LogicServer", true);
			GlobalException.InitException();
			MemIniFile memIniFile = new MemIniFile();
			if (!memIniFile.LoadFromFile("../GlobalConfig.ini"))
			{
				return;
			}
			Program.m_DicSession = new Dictionary<Socket, GameSession>();
			Program.m_DicSession.Clear();
			Program.m_AuthenticatedSessions = new HashSet<Socket>();
			string text = memIniFile.ReadValue("LogicServer", "IP", "0.0.0.0");
			int num = memIniFile.ReadValue("LogicServer", "Port", 8001);
			Program.m_Key = memIniFile.ReadValue("Global", "EncodeKey", Environment.TickCount);
			Program.m_Key2 = memIniFile.ReadValue("Global", "EncodeKey2", Environment.TickCount);
			Program.m_GameServerIP = memIniFile.ReadValue("GameServer", "IP", "0.0.0.0");
			Program.m_GameServerPort = memIniFile.ReadValue("GameServer", "Port", 8002);
			LoginDatabase.Initialize(
				memIniFile.ReadValue("Mysql", "IP", "127.0.0.1"),
				memIniFile.ReadValue("Mysql", "Port", 3306),
				memIniFile.ReadValue("Mysql", "User", "root"),
				memIniFile.ReadValue("Mysql", "Passwd", ""),
				memIniFile.ReadValue("Mysql", "database", "soul"));
			Program.server = new TcpServer();
			Program.server.onConnect += Program.OnConnect;
			Program.server.onReceive += Program.OnReceive;
			Program.server.onClose += Program.OnClose;
			if (!Program.server.Start(text, num))
			{
				Console.WriteLine("start server error!");
				return;
			}
			Program.mDBPacket = new InternalPacket();
			GenerateKey.Init(Program.m_Key, Program.m_Key2);
			text = memIniFile.ReadValue("DBServer", "IP", "0.0.0.0");
			num = memIniFile.ReadValue("DBServer", "Port", 1500);
			Program.mTcpClient = new GameBase.Network.TcpClient();
			Program.mTcpClient.onConnect += Program.OnDBConnect;
			Program.mTcpClient.onReceive += Program.OnDBReceive;
			Program.mTcpClient.onClose += Program.OnDBClose;
			Program.mTcpClient.Connect(text, num);
			new Thread(new ThreadStart(Program.LogicTimer))
			{
				IsBackground = true
			}.Start();
			string a;
			do
			{
				a = Console.ReadLine();
			}
			while (!(a == "exit"));
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002264 File Offset: 0x00000464
		public static void OnConnect(Socket s)
		{
			GameSession gameSession;
			lock (Program._lock_session)
			{
				gameSession = new GameSession(s, null);
				Program.m_DicSession[s] = gameSession;
			}
			byte[] data = LoginPacketCodec.CreateInitialKey(
				gameSession.GetGamePackKeyEx(), Program.m_Key);
			Program.server.SendData(s, data);
			Log.Instance().WriteLog("client connected: " + s.RemoteEndPoint.ToString());
			#if DEBUG
			Log.Instance().WriteLog("sent initial packet type 1059, key " +
				Program.m_Key.ToString() + ", bytes " + BitConverter.ToString(data));
			#endif
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022F0 File Offset: 0x000004F0
		public static void OnReceive(Socket s, byte[] data, int nSize)
		{
			lock (Program._lock_session)
			{
				if (Program.m_DicSession.ContainsKey(s))
				{
					GameSession gameSession = Program.m_DicSession[s];
					byte[] array = new byte[nSize];
					Buffer.BlockCopy(data, 0, array, 0, nSize);
					#if DEBUG
					Log.Instance().WriteLog("received " + nSize.ToString() +
						" bytes: " + BitConverter.ToString(array));
					#endif
					gameSession.m_GamePack.ProcessNetData(array);
				}
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002360 File Offset: 0x00000560
		public static void OnClose(Socket s)
		{
			lock (Program._lock_session)
			{
				if (Program.m_DicSession.ContainsKey(s))
				{
					GameSession gameSession = Program.m_DicSession[s];
					Program.m_DicSession.Remove(s);
					Program.m_AuthenticatedSessions.Remove(s);
					Log.Instance().WriteLog("client disconnected");
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000023C0 File Offset: 0x000005C0
		public static void LogicTimer()
		{
			int num = 4;
			int millisecondsTimeout = 4;
			int tickCount = Environment.TickCount;
			for (;;)
			{
				if (Environment.TickCount - tickCount > num)
				{
					Program.ProcessDBNetMsg();
					Program.Run();
					tickCount = Environment.TickCount;
				}
				Thread.Sleep(millisecondsTimeout);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023F8 File Offset: 0x000005F8
		private static void OnDBConnect(bool isSucceed)
		{
			if (isSucceed)
			{
				Log.Instance().WriteLog("dbserver connect success!");
				OpenLoginSession openLoginSession = new OpenLoginSession();
				Program.mTcpClient.SendData(openLoginSession.GetBuff());
				return;
			}
			Log.Instance().WriteLog("dbserver connect error!");
			Log.Instance().WriteLog("Reconnect  dbserver ip:" + Program.mTcpClient.GetConnectIP() + " port:" + Program.mTcpClient.GetConnectPort().ToString());
			Thread.Sleep(5000);
			Program.mTcpClient.ReConnect();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002488 File Offset: 0x00000688
		private static void OnDBReceive(byte[] data, int nSize)
		{
			byte[] array = new byte[nSize];
			Buffer.BlockCopy(data, 0, array, 0, nSize);
			Program.mDBPacket.ProcessNetMsg(array);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000024B1 File Offset: 0x000006B1
		private static void OnDBClose(Socket s)
		{
			Program.mDBPacket.ClearPacket();
			Log.Instance().WriteLog("dbserver close!!!reconnect ");
			Program.mTcpClient.ReConnect();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000024D8 File Offset: 0x000006D8
		public static void ProcessDBNetMsg()
		{
			byte[] data = Program.mDBPacket.GetData();
			if (data == null)
			{
				return;
			}
			PackIn packIn = new PackIn(data);
			ushort num = packIn.ReadUInt16();
			ushort num2 = num;
			if (num2 != 12)
			{
				return;
			}
			uint gameid = packIn.ReadUInt32();
			int key = packIn.ReadInt32();
			int key2 = packIn.ReadInt32();
			byte b = packIn.ReadByte();
			if (b == 1)
			{
				GameSession gameSession = Program.FindGameSessionToGameID(gameid);
				if (gameSession != null)
				{
					Program.SendConnectMapServer(gameSession, key, key2);
					return;
				}
			}
			else if (b == 2)
			{
				GameSession gameSession2 = Program.FindGameSessionToGameID(gameid);
				if (gameSession2 != null)
				{
					lock (Program._lock_session)
					{
						Program.m_DicSession.Remove(gameSession2.m_Socket);
					}
					gameSession2.Dispose();
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000025A4 File Offset: 0x000007A4
		public static void Run()
		{
			lock (Program._lock_session)
			{
				foreach (GameSession gameSession in Program.m_DicSession.Values)
				{
					byte[] data = gameSession.m_GamePack.GetData();
					if (data != null)
					{
						PackIn packIn = new PackIn(data);
						ushort num = packIn.ReadUInt16();
						#if DEBUG
						Log.Instance().WriteLog("decoded packet type " +
							num.ToString() + ", payload bytes " +
							data.Length.ToString());
						#endif
						ushort num2 = num;
						if (num2 == 1095)
						{
							LegacyGameLoginPacket login;
							string error;
							if (!LoginPacketCodec.TryReadLegacyGameLogin(
								data, out login, out error))
							{
								Program.RejectGameLogin(
									gameSession, "malformed legacy login: " + error);
								continue;
							}
							Program.TryCompleteGameLogin(
								gameSession,
								login.Account,
								null,
								login.ServerName,
								login.Mode,
								1095);
						}
						else if (num2 == 1083)
						{
							AccountMetadataPacket metadata;
							string error;
							if (LoginPacketCodec.TryReadAccountMetadata(
								data, out metadata, out error))
							{
								Log.Instance().WriteLog(
									"received account metadata on LoginServer: " +
									metadata.Account);
							}
							else
							{
								Log.Instance().WriteLog(
									"rejected malformed LoginServer metadata: " + error);
							}
						}
						else if (num2 == 1120)
						{
							DirectGameLoginPacket login;
							string error;
							if (!LoginPacketCodec.TryReadDirectGameLogin(
								data, out login, out error))
							{
								Program.RejectGameLogin(
									gameSession, "malformed direct login: " + error);
								continue;
							}
							Program.TryCompleteGameLogin(
								gameSession,
								login.Account,
								login.Password,
								login.ServerName,
								login.Mode,
								1120);
						}
						else if (num2 == 1100)
						{
							LoginClientInfoPacket clientInfo;
							string error;
							if (LoginPacketCodec.TryReadClientInfo(
								data, out clientInfo, out error))
							{
								Log.Instance().WriteLog(
									"received post-login client info: value=" +
									clientInfo.Value + ", identifier bytes=" +
									Coding.GetDefauleCoding().GetByteCount(
										clientInfo.DeviceIdentifier));
							}
							else
							{
								Log.Instance().WriteLog(
									"rejected malformed client info: " + error);
							}
						}
						else if (num2 == 1052)
						{
							LoginClientStatusPacket status;
							string error;
							if (LoginPacketCodec.TryReadClientStatus(
								data, out status, out error))
							{
								Log.Instance().WriteLog(
									"received post-login client status: value1=" +
									status.Value1 + ", value2=" + status.Value2 +
									", status=" + status.StatusText);
							}
							else
							{
								Log.Instance().WriteLog(
									"rejected malformed client status: " + error);
							}
						}
						else
						{
							Log.Instance().WriteLog("unhandled packet type " +
								num.ToString());
						}
					}
				}
			}
		}

		private static void TryCompleteGameLogin(
			GameSession gameSession,
			string account,
			string directPassword,
			string serverName,
			int loginMode,
			ushort packetType)
		{
			if (Program.m_AuthenticatedSessions.Contains(gameSession.m_Socket))
			{
				Log.Instance().WriteLog("ignored duplicate game-login packet for account: " +
					account);
				return;
			}

			string validationError;
			bool validRequest =
				packetType == LoginPacketCodec.DirectGameLoginType ?
				LoginRequestValidator.TryValidateDirectLogin(
					account, directPassword, serverName, out validationError) :
				LoginRequestValidator.TryValidateLegacyLogin(
					account, serverName, out validationError);
			if (!validRequest)
			{
				Program.RejectGameLogin(
					gameSession, "invalid login fields: " + validationError);
				return;
			}

			int accountId;
			bool authenticated;
			try
			{
				if (packetType == LoginPacketCodec.DirectGameLoginType)
				{
					authenticated = LoginDatabase.TryAuthenticateCredentials(
						account,
						directPassword,
						out accountId);
				}
				else
				{
					authenticated = LoginDatabase.TryConsumeLoginTicket(
						account,
						serverName,
						out accountId);
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("MySQL authentication error for account " +
					account + ": " + ex.Message);
				Program.SendGameLoginFailure(gameSession);
				return;
			}

			if (!authenticated)
			{
				Log.Instance().WriteLog("game login denied by " +
					(packetType == LoginPacketCodec.DirectGameLoginType ?
						"direct credential validation" :
						"account-stage ticket validation") + ": " +
					"account=" + account + ", server=" + serverName +
					", packet=" + packetType);
				Program.SendGameLoginFailure(gameSession);
				return;
			}

			Program.m_AuthenticatedSessions.Add(gameSession.m_Socket);
			int key = 0;
			int key2 = 0;
			GenerateKey.GenerateKey_(ref key, ref key2);
			byte[] accountField = Program.CreateAccountField(account);
			QueryRole queryRole = new QueryRole(gameSession.gameid,
				key, key2, accountField);
			Program.mTcpClient.SendData(queryRole.GetBuffer());
			Log.Instance().WriteLog("accepted game-login packet " + packetType +
				": account=" + account + ", id=" + accountId +
				", server=" + serverName + ", mode=" + loginMode +
				", auth=" +
				(packetType == LoginPacketCodec.DirectGameLoginType ?
					"direct-mysql" : "account-ticket"));
		}

		private static void RejectGameLogin(GameSession session, string reason)
		{
			Log.Instance().WriteLog("game login rejected: " + reason);
			Program.SendGameLoginFailure(session);
		}

		private static void SendGameLoginFailure(GameSession session)
		{
			byte[] response = LoginPacketCodec.CreateGameServerFailure(
				session.GetGamePackKeyEx(), 0);
			Program.server.SendData(session.m_Socket, response);
			Log.Instance().WriteLog(
				"sent game-login failure packet 1057 with neutral error fields");
		}

		private static byte[] CreateAccountField(string account)
		{
			byte[] value = Coding.GetDefauleCoding().GetBytes(account);
			byte[] field = new byte[16];
			Buffer.BlockCopy(value, 0, field, 0, Math.Min(value.Length, field.Length - 1));
			return field;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000026BC File Offset: 0x000008BC
		public static GameSession FindGameSessionToGameID(uint gameid)
		{
			foreach (GameSession gameSession in Program.m_DicSession.Values)
			{
				if (gameSession.gameid == gameid)
				{
					return gameSession;
				}
			}
			return null;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000274C File Offset: 0x0000094C
		public static void SendConnectMapServer(GameSession session, int key, int key2)
		{
			byte[] response = LoginPacketCodec.CreateGameServerSuccess(
				session.GetGamePackKeyEx(),
				key,
				key2,
				Program.m_GameServerPort,
				0x034310e8,
				Program.m_GameServerIP,
				0,
				Program.m_GameServerPort,
				Program.m_GameServerIP);
			Program.server.SendData(session.m_Socket, response);
			Log.Instance().WriteLog("sent game-server handoff packet 1057: " +
				Program.m_GameServerIP + ":" + Program.m_GameServerPort.ToString());
		}

		// Token: 0x04000001 RID: 1
		public static TcpServer server = null;

		// Token: 0x04000002 RID: 2
		public static GameBase.Network.TcpClient mTcpClient = null;

		// Token: 0x04000003 RID: 3
		public static InternalPacket mDBPacket;

		// Token: 0x04000004 RID: 4
		public static int m_Key;

		// Token: 0x04000005 RID: 5
		public static int m_Key2;

		// Token: 0x04000006 RID: 6
		public static Dictionary<Socket, GameSession> m_DicSession;

		public static HashSet<Socket> m_AuthenticatedSessions;

		// Token: 0x04000007 RID: 7
		private static object _lock_session = new object();

		// Token: 0x04000008 RID: 8
		public static string m_GameServerIP;

		// Token: 0x04000009 RID: 9
		public static int m_GameServerPort;
	}
}
