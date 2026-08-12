using System;

namespace GameStruct
{
	// Token: 0x0200001C RID: 28
	public class LevelExp
	{
		// Token: 0x06000133 RID: 307 RVA: 0x0000EE02 File Offset: 0x0000D002
		public LevelExp()
		{
			this.level = 0;
			this.exp = 0UL;
		}

		// Token: 0x040000B3 RID: 179
		public const uint LEVELEXP_ROLE = 0U;

		// Token: 0x040000B4 RID: 180
		public const uint LEVELEXP_EUDEMON = 1U;

		// Token: 0x040000B5 RID: 181
		public byte level;

		// Token: 0x040000B6 RID: 182
		public ulong exp;
	}
}
