using System;
using System.Net.Sockets;

namespace GameBase.Network
{
	// Token: 0x02000035 RID: 53
	public class TcpClientEvent
	{
		// Token: 0x02000036 RID: 54
		// (Invoke) Token: 0x06000102 RID: 258
		public delegate void OnConnectEventHandler(bool isSucceed);

		// Token: 0x02000037 RID: 55
		// (Invoke) Token: 0x06000106 RID: 262
		public delegate void OnReceiveEventHandler(byte[] data, int nSize);

		// Token: 0x02000038 RID: 56
		// (Invoke) Token: 0x0600010A RID: 266
		public delegate void OnCloseEventHandler(Socket s);
	}
}
