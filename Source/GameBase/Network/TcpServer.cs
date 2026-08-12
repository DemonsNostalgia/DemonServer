using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameBase.Network
{
	// Token: 0x02000040 RID: 64
	public class TcpServer
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600012D RID: 301 RVA: 0x00007AB0 File Offset: 0x00005CB0
		// (remove) Token: 0x0600012E RID: 302 RVA: 0x00007AEC File Offset: 0x00005CEC
		public event TcpServerEvent.OnConnectEventHandler onConnect;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600012F RID: 303 RVA: 0x00007B28 File Offset: 0x00005D28
		// (remove) Token: 0x06000130 RID: 304 RVA: 0x00007B64 File Offset: 0x00005D64
		public event TcpServerEvent.OnReceiveEventHandler onReceive;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000131 RID: 305 RVA: 0x00007BA0 File Offset: 0x00005DA0
		// (remove) Token: 0x06000132 RID: 306 RVA: 0x00007BDC File Offset: 0x00005DDC
		public event TcpServerEvent.OnCloseEventHandler onClose;

		// Token: 0x06000134 RID: 308 RVA: 0x00007C34 File Offset: 0x00005E34
		public bool Start(string sBindIP, int nPort)
		{
			this.m_sIP = sBindIP;
			this.m_nPort = nPort;
			IPAddress address = IPAddress.Parse(this.m_sIP);
			IPEndPoint localEP = new IPEndPoint(address, this.m_nPort);
			try
			{
				this.m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				this.m_Socket.Bind(localEP);
				this.m_Socket.Listen(this.BACKLOG);
				this.m_Socket.BeginAccept(new AsyncCallback(TcpServer.AcceptCallback), this);
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Concat(new string[]
				{
					"Server startup failed: Binding IP:",
					sBindIP.ToString(),
					"Bind Port:",
					nPort.ToString(),
					ex.Message
				}));
				return false;
			}
			return true;
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00007D18 File Offset: 0x00005F18
		public static void AcceptCallback(IAsyncResult ar)
		{
			StateObject stateObject = null;
			TcpServer tcpServer = (TcpServer)ar.AsyncState;
			try
			{
				if (tcpServer != null)
				{
					Socket socket = tcpServer.m_Socket;
					Socket socket2 = socket.EndAccept(ar);
					tcpServer.onConnect(socket2);
					stateObject = new StateObject();
					stateObject.s = socket2;
					stateObject.c = tcpServer;
					socket2.BeginReceive(stateObject.buffer, 0, stateObject.buffer.Length, SocketFlags.None, new AsyncCallback(TcpServer.ReadCallBack), stateObject);
					socket.BeginAccept(new AsyncCallback(TcpServer.AcceptCallback), tcpServer);
				}
			}
			catch (Exception ex)
			{
				if (stateObject != null)
				{
					stateObject.c.onClose(stateObject.s);
					stateObject.s.Dispose();
				}
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007DFC File Offset: 0x00005FFC
		public static void ReadCallBack(IAsyncResult ar)
		{
			StateObject stateObject = (StateObject)ar.AsyncState;
			Socket s = stateObject.s;
			if (s.Connected)
			{
				try
				{
					SocketError socketError = SocketError.Disconnecting;
					int num = s.EndReceive(ar, out socketError);
					if (socketError == SocketError.Success && num > 0)
					{
						stateObject.c.onReceive(stateObject.s, stateObject.buffer, num);
						s.BeginReceive(stateObject.buffer, 0, stateObject.buffer.Length, SocketFlags.None, new AsyncCallback(TcpServer.ReadCallBack), stateObject);
					}
					else
					{
						stateObject.c.onClose(s);
						s.Dispose();
					}
				}
				catch (Exception ex)
				{
					stateObject.c.onClose(s);
					s.Dispose();
					Console.WriteLine(ex.Message);
				}
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00007EF4 File Offset: 0x000060F4
		public static void SendCallback(IAsyncResult ar)
		{
			StateObject stateObject = (StateObject)ar.AsyncState;
			try
			{
				int num = stateObject.s.EndSend(ar);
				int pending = stateObject.sendTracked ?
					Interlocked.Decrement(ref stateObject.c.m_PendingSends) :
					stateObject.c.m_PendingSends;
				stateObject.sendTracked = false;
				stateObject.c.WriteSendAudit(
					"complete id=" + stateObject.sendId.ToString() +
					" packet=" + stateObject.sendPacketType.ToString() +
					" requested=" + stateObject.sendLength.ToString() +
					" sent=" + num.ToString() +
					" partial=" + (num != stateObject.sendLength).ToString() +
					" pending=" + pending.ToString() +
					" remote=" + stateObject.sendRemoteEndpoint);
			}
			catch (Exception ex)
			{
				int pending = stateObject.sendTracked ?
					Interlocked.Decrement(ref stateObject.c.m_PendingSends) :
					stateObject.c.m_PendingSends;
				stateObject.sendTracked = false;
				stateObject.c.WriteSendAudit(
					"callback-error id=" + stateObject.sendId.ToString() +
					" packet=" + stateObject.sendPacketType.ToString() +
					" requested=" + stateObject.sendLength.ToString() +
					" pending=" + pending.ToString() +
					" remote=" + stateObject.sendRemoteEndpoint +
					" exception=" + ex.GetType().Name +
					" message=" + ex.Message);
				Console.WriteLine(ex.Message);
				stateObject.c.onClose(stateObject.s);
				stateObject.s.Close();
			}
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007F64 File Offset: 0x00006164
		public bool SendData(Socket s, byte[] data, ushort packetType = 0)
		{
			bool result;
			if (s == null)
			{
				this.WriteSendAudit(
					"rejected packet=" + packetType.ToString() +
					" reason=null-socket");
				result = false;
			}
			else if (data == null)
			{
				this.WriteSendAudit(
					"rejected packet=" + packetType.ToString() +
					" reason=null-data remote=" + GetRemoteEndpoint(s));
				result = false;
			}
			else
			{
				StateObject stateObject = new StateObject();
				stateObject.c = this;
				stateObject.s = s;
				stateObject.sendId = Interlocked.Increment(ref this.m_NextSendId);
				stateObject.sendLength = data.Length;
				stateObject.sendPacketType = packetType;
				stateObject.sendRemoteEndpoint = GetRemoteEndpoint(s);
				int pending = Interlocked.Increment(ref this.m_PendingSends);
				stateObject.sendTracked = true;
				this.WriteSendAudit(
					"begin id=" + stateObject.sendId.ToString() +
					" packet=" + packetType.ToString() +
					" requested=" + data.Length.ToString() +
					" pending=" + pending.ToString() +
					" remote=" + stateObject.sendRemoteEndpoint);
				try
				{
					s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(TcpServer.SendCallback), stateObject);
				}
				catch (Exception ex)
				{
					pending = stateObject.sendTracked ?
						Interlocked.Decrement(ref this.m_PendingSends) :
						this.m_PendingSends;
					stateObject.sendTracked = false;
					this.WriteSendAudit(
						"begin-error id=" + stateObject.sendId.ToString() +
						" packet=" + packetType.ToString() +
						" requested=" + data.Length.ToString() +
						" pending=" + pending.ToString() +
						" remote=" + stateObject.sendRemoteEndpoint +
						" exception=" + ex.GetType().Name +
						" message=" + ex.Message);
					this.onClose(s);
					return false;
				}
				result = true;
			}
			return result;
		}

		private static string GetRemoteEndpoint(Socket socket)
		{
			try
			{
				return socket.RemoteEndPoint == null ?
					"<unknown>" : socket.RemoteEndPoint.ToString();
			}
			catch (Exception)
			{
				return "<unavailable>";
			}
		}

		private void WriteSendAudit(string message)
		{
			Action<string> audit = this.SendAudit;
			if (audit == null)
			{
				return;
			}
			try
			{
				audit("[SEND-AUDIT] " + message);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00007FE0 File Offset: 0x000061E0
		public void Stop()
		{
			if (this.m_Socket != null)
			{
				this.m_Socket.Close();
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008009 File Offset: 0x00006209
		public void Dispose()
		{
			this.Stop();
		}

		// Token: 0x04000176 RID: 374
		private string m_sIP;

		// Token: 0x04000177 RID: 375
		private int m_nPort;

		// Token: 0x04000178 RID: 376
		public Socket m_Socket = null;

		// Token: 0x04000179 RID: 377
		private int BACKLOG = 100;

		private int m_NextSendId;

		private int m_PendingSends;

		public Action<string> SendAudit;
	}
}
