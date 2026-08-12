using System;
using GameBase.Core;

namespace MapServer
{
	// Token: 0x0200009F RID: 159
	internal class PlayTimeOut
	{
		// Token: 0x0600042A RID: 1066 RVA: 0x0003207B File Offset: 0x0003027B
		public PlayTimeOut()
		{
			this.time_id = 0;
			this.callback_scripte_id = 0U;
			this.id = 0;
			this.TimeOut = new TimeOut();
			this.IsOnline = true;
		}

		// Token: 0x04000676 RID: 1654
		public int time_id;

		// Token: 0x04000677 RID: 1655
		public int id;

		// Token: 0x04000678 RID: 1656
		public uint callback_scripte_id;

		// Token: 0x04000679 RID: 1657
		public TimeOut TimeOut;

		// Token: 0x0400067A RID: 1658
		public bool IsOnline;
	}
}
