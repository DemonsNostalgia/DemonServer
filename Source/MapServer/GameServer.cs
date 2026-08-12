using System;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;

namespace MapServer
{
	// Token: 0x0200008A RID: 138
	public class GameServer
	{
		// Token: 0x06000294 RID: 660 RVA: 0x0001A874 File Offset: 0x00018A74
		public static TcpServer GetTcpServer()
		{
			return GameServer.mTcpServer;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0001A88C File Offset: 0x00018A8C
		public static bool IsTestMode()
		{
			return GameServer.mbTestMode;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0001A8A4 File Offset: 0x00018AA4
		public static bool Start()
		{
			bool result = true;
			Log.Instance().Init("./MapServer", true);
			GlobalException.InitException();
			try
			{
				ConfigManager.Instance().LoadConfig();
				MemIniFile memIniFile = new MemIniFile();
				if (!memIniFile.LoadFromFile("../GlobalConfig.ini"))
				{
					Log.Instance().WriteLog("load golbalconfig error!");
					return false;
				}
				string sBindIP = memIniFile.ReadValue("GameServer", "IP", "0.0.0.0");
				int nPort = memIniFile.ReadValue("GameServer", "Port", 8002);
				GameServer.mTcpServer = new TcpServer();
				GameServer.mTcpServer.SendAudit = delegate(string message)
				{
					Log.Instance().WriteLog(message);
				};
				GameServer.mTcpServer.onConnect += GameServer.OnConnect;
				GameServer.mTcpServer.onClose += GameServer.OnClose;
				GameServer.mTcpServer.onReceive += GameServer.OnReceive;
				if (!GameServer.mTcpServer.Start(sBindIP, nPort))
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("Failed to start the server.");
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				return false;
			}
			return result;
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0001A9F4 File Offset: 0x00018BF4
		public static void Stop()
		{
			UserEngine.Instance().Stop();
			GameServer.mTcpServer.Dispose();
			SessionManager.Instance().Dispose();
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0001AA18 File Offset: 0x00018C18
		private static void OnConnect(Socket s)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 0;
			socketInfo.s = s;
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0001AA48 File Offset: 0x00018C48
		private static void OnClose(Socket s)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 3;
			socketInfo.s = s;
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0001AA78 File Offset: 0x00018C78
		private static void OnReceive(Socket s, byte[] data, int nSize)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 2;
			socketInfo.s = s;
			socketInfo.data = new byte[nSize];
			Buffer.BlockCopy(data, 0, socketInfo.data, 0, nSize);
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0001AAC4 File Offset: 0x00018CC4
		public static void LogicRun()
		{
			SocketCallBack.Instance().Run();
			DBServer.Instance().ProcessDBNetMsg();
			SessionManager.Instance().ProcessNetMsg();
			MapManager.Instance().Process();
			UserEngine.Instance().Run();
			ScriptTimerManager.Instance().Run();
			WorldPigeon.Instance().Run();
		}

		// Token: 0x040005F0 RID: 1520
		private static TcpServer mTcpServer;

		// Token: 0x040005F1 RID: 1521
		private static bool mbTestMode = false;
	}
}
