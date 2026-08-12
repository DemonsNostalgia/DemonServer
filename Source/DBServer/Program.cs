using System;
using System.Net.Sockets;
using System.Threading;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;

namespace DBServer
{
	// Token: 0x0200000C RID: 12
	internal class Program
	{
		// Token: 0x0600005B RID: 91 RVA: 0x000059C0 File Offset: 0x00003BC0
		private static void Main(string[] args)
		{
			Log.Instance().Init("./DBServer", true);
			GlobalException.InitException();
			MemIniFile memIniFile = new MemIniFile();
			if (memIniFile.LoadFromFile("../GlobalConfig.ini"))
			{
				if (!Filter.Instance().LoadFilterNameFile("data/FilterName.txt"))
				{
					Log.Instance().WriteLog("Failed to load the name filter file.");
				}
				string text = memIniFile.ReadValue("Mysql", "IP", "127.0.0.1");
				int num = memIniFile.ReadValue("Mysql", "Port", 3306);
				string user = memIniFile.ReadValue("Mysql", "User", "root");
				string paswd = memIniFile.ReadValue("Mysql", "Passwd", "test");
				string database = memIniFile.ReadValue("Mysql", "database", "soul");
				if (!MysqlConn.Connect(text, num, user, paswd, database))
				{
					Log.Instance().WriteLog("connect mysql error!");
				}
				else
				{
					Data.ClearStaleOnlineStates();
					Program.LoadGameKernel();
					text = memIniFile.ReadValue("DBServer", "IP", "0.0.0.0");
					num = memIniFile.ReadValue("DBServer", "Port", 1500);
					Program.mTcpServer = new TcpServer();
					Program.mTcpServer.onConnect += Program.OnConnect;
					Program.mTcpServer.onReceive += Program.OnReceive;
					Program.mTcpServer.onClose += Program.OnClose;
					if (!Program.mTcpServer.Start(text, num))
					{
						Console.WriteLine("start server error!");
						MysqlConn.Dispose();
					}
					else
					{
						new Thread(new ThreadStart(Program.LogicRun))
						{
							IsBackground = true
						}.Start();
						string a;
						do
						{
							a = Console.ReadLine();
						}
						while (!(a == "quit") && !(a == "exit"));
						MysqlConn.Dispose();
					}
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005BCC File Offset: 0x00003DCC
		public static void OnConnect(Socket s)
		{
			SessionManager.Instance().AddSession(s, Program.mTcpServer);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005BE0 File Offset: 0x00003DE0
		public static void OnReceive(Socket s, byte[] data, int nSize)
		{
			SessionManager.Instance().ReceiveData(s, data, nSize);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005BF1 File Offset: 0x00003DF1
		public static void OnClose(Socket s)
		{
			SessionManager.Instance().RemoveSession(s);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005C00 File Offset: 0x00003E00
		private static void LogicRun()
		{
			int num = 4;
			int millisecondsTimeout = 4;
			int tickCount = Environment.TickCount;
			for (;;)
			{
				if (Environment.TickCount - tickCount > num)
				{
					SessionManager.Instance().Run();
					PayManager.Instance().Run();
					tickCount = Environment.TickCount;
				}
				Thread.Sleep(millisecondsTimeout);
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00005C55 File Offset: 0x00003E55
		private static void LoadGameKernel()
		{
			GuanJue.GetInstance().DB_Load();
			Legion.GetInstance().DB_Load();
			Family.GetInstance().DB_Load();
			PayManager.Instance().DB_Load();
		}

		// Token: 0x04000035 RID: 53
		private static TcpServer mTcpServer = null;
	}
}
