using System;
using GameStruct;

namespace MapServer
{
	// Token: 0x02000008 RID: 8
	public class NpcObject : BaseObject
	{
		// Token: 0x0600006D RID: 109 RVA: 0x00004A83 File Offset: 0x00002C83
		public NpcObject(NPCInfo info)
		{
			this.type = 1;
			this.mInfo = info;
		}

		// Token: 0x04000040 RID: 64
		public uint ScriptId;

		// Token: 0x04000041 RID: 65
		public NPCInfo mInfo;
	}
}
