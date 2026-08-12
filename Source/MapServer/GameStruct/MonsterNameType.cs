using System;

namespace GameStruct
{
	// Token: 0x02000033 RID: 51
	public class MonsterNameType
	{
		// Token: 0x06000174 RID: 372 RVA: 0x000100EC File Offset: 0x0000E2EC
		public static int GetNameType(int nAtkerLev, int nMonsterLev)
		{
			int num = nAtkerLev - nMonsterLev;
			int result;
			if (num >= 3)
			{
				result = 0;
			}
			else if (num >= 0)
			{
				result = 1;
			}
			else if (num >= -5)
			{
				result = 2;
			}
			else
			{
				result = 3;
			}
			return result;
		}

		// Token: 0x04000276 RID: 630
		public const int NAME_GREEN = 0;

		// Token: 0x04000277 RID: 631
		public const int NAME_WHITE = 1;

		// Token: 0x04000278 RID: 632
		public const int NAME_RED = 2;

		// Token: 0x04000279 RID: 633
		public const int NAME_BLACK = 3;
	}
}
