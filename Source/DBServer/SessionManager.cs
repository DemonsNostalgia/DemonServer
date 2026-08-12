using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameBase.Network;

namespace DBServer
{
	// Token: 0x0200000D RID: 13
	public class SessionManager
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00005C8C File Offset: 0x00003E8C
		public static SessionManager Instance()
		{
			if (SessionManager.mInstance == null)
			{
				SessionManager.mInstance = new SessionManager();
			}
			return SessionManager.mInstance;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005CBE File Offset: 0x00003EBE
		public SessionManager()
		{
			this.mDicSession = new Dictionary<Socket, InternalSession>();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00005CD4 File Offset: 0x00003ED4
		public void AddSession(Socket s, TcpServer server)
		{
			lock (SessionManager._lock)
			{
				InternalSession value = new InternalSession(server, s);
				this.mDicSession[s] = value;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00005D30 File Offset: 0x00003F30
		public void RemoveSession(Socket s)
		{
			lock (SessionManager._lock)
			{
				if (this.mDicSession.ContainsKey(s))
				{
					InternalSession internalSession = this.mDicSession[s];
					this.mDicSession.Remove(s);
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00005DA4 File Offset: 0x00003FA4
		public void ReceiveData(Socket s, byte[] data, int nSize)
		{
			lock (SessionManager._lock)
			{
				if (this.mDicSession.ContainsKey(s))
				{
					InternalSession internalSession = this.mDicSession[s];
					byte[] array = new byte[nSize];
					Buffer.BlockCopy(data, 0, array, 0, nSize);
					internalSession.GetPacket().ProcessNetMsg(array);
					internalSession.SetLastTime(Environment.TickCount);
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00005E3C File Offset: 0x0000403C
		public void Run()
		{
			lock (SessionManager._lock)
			{
				foreach (InternalSession internalSession in this.mDicSession.Values)
				{
					internalSession.Run();
				}
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00005ED4 File Offset: 0x000040D4
		public InternalSession FindSessionToSocket(Socket s)
		{
			InternalSession result = null;
			if (this.mDicSession.ContainsKey(s))
			{
				result = this.mDicSession[s];
			}
			return result;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00005F0C File Offset: 0x0000410C
		public void SendMapServer(int mapid, byte[] data)
		{
			lock (SessionManager._lock)
			{
				foreach (InternalSession internalSession in this.mDicSession.Values)
				{
					if (internalSession.GetSessionType() == 5)
					{
						internalSession.GetTcpServer().SendData(internalSession.GetSocket(), data);
						break;
					}
				}
			}
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00005FC4 File Offset: 0x000041C4
		public void SendLoginServer(byte[] data)
		{
			lock (SessionManager._lock)
			{
				foreach (InternalSession internalSession in this.mDicSession.Values)
				{
					if (internalSession.GetSessionType() == 2)
					{
						internalSession.GetTcpServer().SendData(internalSession.GetSocket(), data);
						break;
					}
				}
			}
		}

		// Token: 0x04000036 RID: 54
		private static SessionManager mInstance = null;

		// Token: 0x04000037 RID: 55
		private Dictionary<Socket, InternalSession> mDicSession;

		// Token: 0x04000038 RID: 56
		private static object _lock = new object();
	}
}
