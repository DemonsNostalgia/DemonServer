using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000052 RID: 82
	public class MsgLogin : BaseMsg
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x0001408F File Offset: 0x0001228F
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001409B File Offset: 0x0001229B
		public override void Process()
		{
		}
	}
}
