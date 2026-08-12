using System;
using System.Net.Sockets;

namespace GameBase.Network
{
	// Token: 0x02000009 RID: 9
	public class GameSession
	{
		// Token: 0x06000032 RID: 50 RVA: 0x00003267 File Offset: 0x00001467
		public GameSession(Socket s, TcpServer tcpserver = null)
		{
			this.server = tcpserver;
			this.m_Socket = s;
			this.m_GamePack = new GamePack();
			this.m_nLastTime = Environment.TickCount;
			this.gameid = IDManager.CreateGameId();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x000032A4 File Offset: 0x000014A4
		~GameSession()
		{
		}

		// Token: 0x06000034 RID: 52 RVA: 0x000032D0 File Offset: 0x000014D0
		public GamePacketKeyEx GetGamePackKeyEx()
		{
			GamePacketKeyEx result;
			if (this.m_GamePack == null)
			{
				result = null;
			}
			else
			{
				result = this.m_GamePack.m_Key;
			}
			return result;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003304 File Offset: 0x00001504
		public TcpServer GetTcpServer()
		{
			return this.server;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x0000331C File Offset: 0x0000151C
		public void SendData(byte[] data, ushort packetType = 0)
		{
			if (this.server != null && this.m_Socket != null)
			{
				if (!this.server.SendData(this.m_Socket, data, packetType))
				{
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003364 File Offset: 0x00001564
		public void Dispose()
		{
			this.server = null;
			if (this.m_Socket != null && this.m_Socket.Connected)
			{
				this.m_Socket.Close();
				this.m_Socket.Dispose();
			}
			this.m_Socket = null;
			this.m_GamePack = null;
		}

		// Token: 0x04000015 RID: 21
		public TcpServer server;

		// Token: 0x04000016 RID: 22
		public Socket m_Socket;

		// Token: 0x04000017 RID: 23
		public GamePack m_GamePack;

		// Token: 0x04000018 RID: 24
		public int m_nLastTime;

		// Token: 0x04000019 RID: 25
		public uint gameid;
	}
}
