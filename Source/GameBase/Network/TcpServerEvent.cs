using System;
using System.Net.Sockets;

namespace GameBase.Network
{
	// Token: 0x0200003B RID: 59
	public class TcpServerEvent
	{
		// Token: 0x0200003C RID: 60
		// (Invoke) Token: 0x06000121 RID: 289
		public delegate void OnConnectEventHandler(Socket s);

		// Token: 0x0200003D RID: 61
		// (Invoke) Token: 0x06000125 RID: 293
		public delegate void OnReceiveEventHandler(Socket s, byte[] data, int nSize);

		// Token: 0x0200003E RID: 62
		// (Invoke) Token: 0x06000129 RID: 297
		public delegate void OnCloseEventHandler(Socket s);
	}
}
