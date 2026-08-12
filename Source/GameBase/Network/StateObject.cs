using System;
using System.Net.Sockets;

namespace GameBase.Network
{
	// Token: 0x0200003F RID: 63
	public class StateObject
	{
		// Token: 0x0400016F RID: 367
		private const int BUFFSIZE = 1024;

		// Token: 0x04000170 RID: 368
		public Socket s = null;

		// Token: 0x04000171 RID: 369
		public byte[] buffer = new byte[1024];

		// Token: 0x04000172 RID: 370
		public TcpServer c;

		public int sendId;

		public int sendLength;

		public ushort sendPacketType;

		public string sendRemoteEndpoint;

		public bool sendTracked;
	}
}
