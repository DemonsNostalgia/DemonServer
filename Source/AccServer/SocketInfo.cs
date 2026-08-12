using System;
using System.Net.Sockets;
using GameBase.Network;

namespace AccServer
{
	// Token: 0x02000003 RID: 3
	public class SocketInfo
	{
		// Token: 0x04000003 RID: 3
		public byte type = 0;

		// Token: 0x04000004 RID: 4
		public Socket s = null;

		// Token: 0x04000005 RID: 5
		public byte[] data = null;

		// Token: 0x04000006 RID: 6
		public GameSession session = null;
	}
}
