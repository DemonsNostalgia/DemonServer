using System;
using GameBase.Network.Internal;

namespace MapServer
{
	// Token: 0x02000046 RID: 70
	public class Legion
	{
		// Token: 0x06000198 RID: 408 RVA: 0x00012500 File Offset: 0x00010700
		public LegionInfo GetBaseInfo()
		{
			return this.mInfo;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00012518 File Offset: 0x00010718
		public void SetBaseInfo(LegionInfo info)
		{
			this.mInfo = info;
		}

		// Token: 0x04000358 RID: 856
		private LegionInfo mInfo;
	}
}
