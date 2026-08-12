using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;

namespace AccServer
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			Log.Instance().Init("./Accserver", true);
			GlobalException.InitException();
			MemIniFile memIniFile = new MemIniFile();
			if (memIniFile.LoadFromFile("../GlobalConfig.ini"))
			{
				string text = memIniFile.ReadValue("AccServer", "IP", "0.0.0.0");
				int nPort = memIniFile.ReadValue("AccServer", "Port", 8000);
				Program.m_Key = memIniFile.ReadValue("Global", "EncodeKey", Environment.TickCount);
				LoginDatabase.Initialize(
					memIniFile.ReadValue("Mysql", "IP", "127.0.0.1"),
					memIniFile.ReadValue("Mysql", "Port", 3306),
					memIniFile.ReadValue("Mysql", "User", "root"),
					memIniFile.ReadValue("Mysql", "Passwd", ""),
					memIniFile.ReadValue("Mysql", "database", "soul"));
				Console.Title = "AccServer";
				Program.server.onConnect += Program.OnConnect;
				Program.server.onReceive += Program.OnRecv;
				Program.server.onClose += Program.OnClose;
				Log.Instance().WriteLog("bind ip:" + text + "bindport:" + nPort.ToString());
				if (!Program.server.Start(text, nPort))
				{
					Log.Instance().WriteLog("start tcpserver error!");
				}
				Log.Instance().WriteLog("start server success!!");
				Program.m_LogicTimer = new Timer(1.0);
				Program.m_LogicTimer.Elapsed += Program.LogicTimer;
				Program.m_LogicTimer.Enabled = true;
				string a;
				do
				{
					a = Console.ReadLine();
				}
				while (!(a == "quit") && !(a == "exit"));
				Program.server.Stop();
				Log.Instance().Dispose();
			}
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000021E8 File Offset: 0x000003E8
		public static void OnConnect(Socket s)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 0;
			socketInfo.s = s;
			lock (Program.SessionLock)
			{
				socketInfo.session = new GameSession(s, Program.server);
				Program.Sessions[s] = socketInfo.session;
			}
			Log.Instance().WriteLog("client connected: " + s.RemoteEndPoint);
			byte[] handshake = LoginPacketCodec.CreateInitialKey(
				socketInfo.session.GetGamePackKeyEx(), Program.m_Key);
			Program.server.SendData(s, handshake);
			#if DEBUG
			Log.Instance().WriteLog("sent initial packet type 1059, key " +
				Program.m_Key.ToString() + ", bytes " + BitConverter.ToString(handshake));
			#endif
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002218 File Offset: 0x00000418
		public static void OnClose(Socket s)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 3;
			socketInfo.s = s;
			lock (Program.SessionLock)
			{
				if (Program.Sessions.TryGetValue(s, out GameSession session))
				{
					socketInfo.session = session;
					Program.Sessions.Remove(s);
				}
			}
			Log.Instance().WriteLog("client disconnected");
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002248 File Offset: 0x00000448
		public static void OnRecv(Socket s, byte[] data, int nSize)
		{
			SocketInfo socketInfo = new SocketInfo();
			socketInfo.type = 2;
			socketInfo.s = s;
			socketInfo.data = new byte[nSize];
			Buffer.BlockCopy(data, 0, socketInfo.data, 0, nSize);
			lock (Program.SessionLock)
			{
				Program.Sessions.TryGetValue(s, out socketInfo.session);
			}
			#if DEBUG
			Log.Instance().WriteLog("received " + nSize.ToString() + " bytes: " +
				BitConverter.ToString(socketInfo.data));
			#endif
			SocketCallBack.Instance().AddData(socketInfo);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002292 File Offset: 0x00000492
		public static void LogicTimer(object source, ElapsedEventArgs e)
		{
			SocketCallBack.Instance().Run();
		}

		// Token: 0x04000001 RID: 1
		public static TcpServer server = new TcpServer();

		// Token: 0x04000002 RID: 2
		public static Timer m_LogicTimer;

		public static int m_Key;

		public static readonly Dictionary<Socket, GameSession> Sessions =
			new Dictionary<Socket, GameSession>();

		public static readonly object SessionLock = new object();
	}
}
