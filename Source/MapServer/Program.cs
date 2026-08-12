using System;
using System.Threading;
using GameBase.Config;
using GameBase.Network;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000097 RID: 151
	internal class Program
	{
		// Token: 0x060003D6 RID: 982 RVA: 0x0002CB94 File Offset: 0x0002AD94
		private static void Main(string[] args)
		{
			if (GameServer.Start())
			{
				DBServer.Instance().Init();
				new Thread(new ThreadStart(Program.ServerRun))
				{
					IsBackground = true
				}.Start();
				for (;;)
				{
					string text = Console.ReadLine();
					string[] array = text.Split(new char[]
					{
						' '
					});
					if (array.Length > 0)
					{
						text = array[0];
						try
						{
							if (text == "quit" || text == "exit")
							{
								break;
							}
							if (text == "test")
							{
								PlayerObject playerObject = MapManager.Instance().GetGameMapToID(1000U).GetObject(3988U) as PlayerObject;
								MsgUpdateSP msgUpdateSP = new MsgUpdateSP();
								msgUpdateSP.Create(null, playerObject.GetGamePackKeyEx());
								msgUpdateSP.role_id = playerObject.GetTypeId();
								msgUpdateSP.value = Convert.ToUInt32(array[1]);
								msgUpdateSP.sp = Convert.ToUInt32(array[2]);
								playerObject.SendData(msgUpdateSP.GetBuffer(), false);
							}
						}
						catch (Exception ex)
						{
							Log.Instance().WriteLog(ex.Message);
						}
					}
				}
				GameServer.Stop();
				Log.Instance().WriteLog("exit server!");
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0002CD1C File Offset: 0x0002AF1C
		private static void ServerRun()
		{
			int num = 4;
			int millisecondsTimeout = 4;
			int tickCount = Environment.TickCount;
			for (;;)
			{
				if (Environment.TickCount - tickCount > num)
				{
					GameServer.LogicRun();
					tickCount = Environment.TickCount;
				}
				Thread.Sleep(millisecondsTimeout);
			}
		}

		// Token: 0x04000658 RID: 1624
		public static byte _Head = 0;

		// Token: 0x04000659 RID: 1625
		public static byte _Tail = 0;

		// Token: 0x0400065A RID: 1626
		public static TcpServer server = null;
	}
}
