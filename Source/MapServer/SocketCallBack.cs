using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace MapServer
{
	// Token: 0x020000A5 RID: 165
	public class SocketCallBack
	{
		// Token: 0x06000446 RID: 1094 RVA: 0x000330C0 File Offset: 0x000312C0
		public static SocketCallBack Instance()
		{
			if (SocketCallBack.mInstance == null)
			{
				SocketCallBack.mInstance = new SocketCallBack();
			}
			return SocketCallBack.mInstance;
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x000330F2 File Offset: 0x000312F2
		public SocketCallBack()
		{
			this.mList = new List<SocketInfo>();
		}

		// Token: 0x06000448 RID: 1096 RVA: 0x00033108 File Offset: 0x00031308
		public void AddData(SocketInfo info)
		{
			lock (this.mList)
			{
				this.mList.Add(info);
			}
		}

		// Token: 0x06000449 RID: 1097 RVA: 0x0003315C File Offset: 0x0003135C
		public SocketInfo GetInfo()
		{
			SocketInfo result = null;
			lock (this.mList)
			{
				if (this.mList.Count > 0)
				{
					result = this.mList[0];
					this.mList.RemoveAt(0);
				}
			}
			return result;
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x000331E0 File Offset: 0x000313E0
		public void Run()
		{
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= 300)
			{
				SocketInfo info = this.GetInfo();
				if (info != null)
				{
					if (info.s != null)
					{
						Socket s = info.s;
						switch (info.type)
						{
						case 0:
							SessionManager.Instance().AddSession(s, GameServer.GetTcpServer());
							break;
						case 2:
							SessionManager.Instance().AddNetData(s, info.data, info.data.Length);
							break;
						case 3:
						{
							PlayerObject obj = UserEngine.Instance().FindPlayerObjectToSocket(s);
							SessionManager.Instance().RemoveSession(s);
							UserEngine.Instance().RemovePlayObject(obj);
							break;
						}
						}
						continue;
					}
				}
				return;
			}
		}

		// Token: 0x04000688 RID: 1672
		public const byte TYPE_ONCONNECT = 0;

		// Token: 0x04000689 RID: 1673
		public const byte TYPE_RECEIVE = 2;

		// Token: 0x0400068A RID: 1674
		public const byte TYPE_CLOSE = 3;

		// Token: 0x0400068B RID: 1675
		private static SocketCallBack mInstance = null;

		// Token: 0x0400068C RID: 1676
		private List<SocketInfo> mList;
	}
}
