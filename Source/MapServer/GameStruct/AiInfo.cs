using System;

namespace GameStruct
{
	// Token: 0x02000041 RID: 65
	public class AiInfo
	{
		// Token: 0x06000184 RID: 388 RVA: 0x000104C8 File Offset: 0x0000E6C8
		public AiInfo()
		{
			this.nId = 0;
			this.nType = 0;
			this.nRange = 0;
			this.nAttack_Range = 0;
			this.nMove_Speed = 0;
			this.nAttack_Speed = 0;
			this.bIdle_Move = false;
			this.bMove = false;
		}

		// Token: 0x040002EB RID: 747
		public int nId;

		// Token: 0x040002EC RID: 748
		public int nType;

		// Token: 0x040002ED RID: 749
		public int nRange;

		// Token: 0x040002EE RID: 750
		public int nAttack_Range;

		// Token: 0x040002EF RID: 751
		public int nMove_Speed;

		// Token: 0x040002F0 RID: 752
		public int nAttack_Speed;

		// Token: 0x040002F1 RID: 753
		public bool bIdle_Move;

		// Token: 0x040002F2 RID: 754
		public bool bMove;
	}
}
