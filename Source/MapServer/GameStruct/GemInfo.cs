using System;

namespace GameStruct
{
	// Token: 0x02000036 RID: 54
	public class GemInfo
	{
		// Token: 0x0600017A RID: 378 RVA: 0x0001031C File Offset: 0x0000E51C
		public GemInfo()
		{
			this.itemid = 0U;
			this.type = 0;
			this.value = 0;
		}

		// Token: 0x04000287 RID: 647
		public const byte GEMTYPE_ADDATTACK = 50;

		// Token: 0x04000288 RID: 648
		public const byte GEMTYPE_FIGHTPOWER = 54;

		// Token: 0x04000289 RID: 649
		public const byte GEMTYPE_DECLIFE = 9;

		// Token: 0x0400028A RID: 650
		public const byte GEMTYPE_DURA = 63;

		// Token: 0x0400028B RID: 651
		public const byte GEMTYPE_ADDEXP = 52;

		// Token: 0x0400028C RID: 652
		public uint itemid;

		// Token: 0x0400028D RID: 653
		public byte type;

		// Token: 0x0400028E RID: 654
		public int value;

		// Token: 0x0400028F RID: 655
		public int amount;

		// Token: 0x04000290 RID: 656
		public byte gemtype;
	}
}
