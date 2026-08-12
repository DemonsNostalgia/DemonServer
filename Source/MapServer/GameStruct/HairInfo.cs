using System;

namespace GameStruct
{
	// Token: 0x0200003D RID: 61
	public class HairInfo
	{
		// Token: 0x06000181 RID: 385 RVA: 0x0001046A File Offset: 0x0000E66A
		public HairInfo()
		{
			this.name = "";
			this.itemid = 0U;
			this.hairid = 0;
			this.sex = 0;
			this.price = 0;
		}

		// Token: 0x040002C5 RID: 709
		public uint itemid;

		// Token: 0x040002C6 RID: 710
		public int hairid;

		// Token: 0x040002C7 RID: 711
		public string name;

		// Token: 0x040002C8 RID: 712
		public byte sex;

		// Token: 0x040002C9 RID: 713
		public int price;
	}
}
