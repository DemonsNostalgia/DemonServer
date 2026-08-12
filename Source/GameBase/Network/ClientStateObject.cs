using System;
using System.Net.Sockets;

namespace GameBase.Network
{
	// Token: 0x02000039 RID: 57
	public class ClientStateObject
	{
		// Token: 0x04000165 RID: 357
		private const int BUFFSIZE = 1024;

		// Token: 0x04000166 RID: 358
		public Socket s = null;

		// Token: 0x04000167 RID: 359
		public byte[] buffer = new byte[1024];

		// Token: 0x04000168 RID: 360
		public TcpClient c;
	}
}
