using System;
using System.Net;
using System.Net.Sockets;
using GameBase.Config;

namespace GameBase.Network
{
	// Token: 0x0200003A RID: 58
	public class TcpClient
	{
		// Token: 0x0600010E RID: 270 RVA: 0x00007614 File Offset: 0x00005814
		public Socket GetSocket()
		{
			return this.mSocket;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000762C File Offset: 0x0000582C
		public void SetSocket(Socket s)
		{
			this.mSocket = s;
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000110 RID: 272 RVA: 0x00007638 File Offset: 0x00005838
		// (remove) Token: 0x06000111 RID: 273 RVA: 0x00007674 File Offset: 0x00005874
		public event TcpClientEvent.OnConnectEventHandler onConnect;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000112 RID: 274 RVA: 0x000076B0 File Offset: 0x000058B0
		// (remove) Token: 0x06000113 RID: 275 RVA: 0x000076EC File Offset: 0x000058EC
		public event TcpClientEvent.OnReceiveEventHandler onReceive;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000114 RID: 276 RVA: 0x00007728 File Offset: 0x00005928
		// (remove) Token: 0x06000115 RID: 277 RVA: 0x00007764 File Offset: 0x00005964
		public event TcpClientEvent.OnCloseEventHandler onClose;

		// Token: 0x06000116 RID: 278 RVA: 0x000077A0 File Offset: 0x000059A0
		public string GetConnectIP()
		{
			return this.msIP;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x000077B8 File Offset: 0x000059B8
		public int GetConnectPort()
		{
			return this.mnPort;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x000077D0 File Offset: 0x000059D0
		public void Connect(string ip, int port)
		{
			IPAddress address = IPAddress.Parse(ip);
			IPEndPoint remoteEP = new IPEndPoint(address, port);
			this.msIP = ip;
			this.mnPort = port;
			this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.mSocket.BeginConnect(remoteEP, new AsyncCallback(TcpClient.ConnectCallback), this);
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00007823 File Offset: 0x00005A23
		public void ReConnect()
		{
			this.Connect(this.msIP, this.mnPort);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000783C File Offset: 0x00005A3C
		private static void ConnectCallback(IAsyncResult ar)
		{
			TcpClient tcpClient = (TcpClient)ar.AsyncState;
			try
			{
				Socket socket = tcpClient.GetSocket();
				socket.EndConnect(ar);
				tcpClient.onConnect(true);
				ClientStateObject clientStateObject = new ClientStateObject();
				clientStateObject.s = socket;
				clientStateObject.c = tcpClient;
				socket.BeginReceive(clientStateObject.buffer, 0, clientStateObject.buffer.Length, SocketFlags.None, new AsyncCallback(TcpClient.ReceiveCallback), clientStateObject);
			}
			catch (Exception ex)
			{
				tcpClient.onConnect(false);
			}
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000078D4 File Offset: 0x00005AD4
		private static void ReceiveCallback(IAsyncResult ar)
		{
			ClientStateObject clientStateObject = (ClientStateObject)ar.AsyncState;
			TcpClient c = clientStateObject.c;
			Socket socket = c.GetSocket();
			if (socket != null)
			{
				try
				{
					int num = socket.EndReceive(ar);
					if (num > 0)
					{
						c.onReceive(clientStateObject.buffer, num);
						socket.BeginReceive(clientStateObject.buffer, 0, clientStateObject.buffer.Length, SocketFlags.None, new AsyncCallback(TcpClient.ReceiveCallback), clientStateObject);
					}
					else
					{
						c.onClose(socket);
					}
				}
				catch (Exception ex)
				{
					c.onClose(socket);
					Log.Instance().WriteLog(ex.Message);
					Log.Instance().WriteLog(ex.StackTrace);
				}
			}
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000079BC File Offset: 0x00005BBC
		public void SendData(byte[] data)
		{
			if (this.mSocket.Connected)
			{
				ClientStateObject clientStateObject = new ClientStateObject();
				clientStateObject.c = this;
				clientStateObject.s = this.mSocket;
				this.mSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(TcpClient.SendCallback), clientStateObject);
			}
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00007A14 File Offset: 0x00005C14
		public static void SendCallback(IAsyncResult ar)
		{
			ClientStateObject clientStateObject = (ClientStateObject)ar.AsyncState;
			try
			{
				int num = clientStateObject.s.EndSend(ar);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				clientStateObject.c.onClose(clientStateObject.s);
			}
		}

		// Token: 0x04000169 RID: 361
		private Socket mSocket = null;

		// Token: 0x0400016A RID: 362
		private string msIP;

		// Token: 0x0400016B RID: 363
		private int mnPort;
	}
}
