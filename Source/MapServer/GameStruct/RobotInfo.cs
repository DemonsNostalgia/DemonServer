using System;

namespace GameStruct
{
	// Token: 0x02000040 RID: 64
	public class RobotInfo
	{
		// Token: 0x06000183 RID: 387 RVA: 0x000104A4 File Offset: 0x0000E6A4
		public RobotInfo()
		{
			this.name = "";
			this.legion_name = "";
		}

		// Token: 0x040002DD RID: 733
		public string name;

		// Token: 0x040002DE RID: 734
		public uint lookface;

		// Token: 0x040002DF RID: 735
		public uint hair;

		// Token: 0x040002E0 RID: 736
		public uint armor_id;

		// Token: 0x040002E1 RID: 737
		public uint wepon_id;

		// Token: 0x040002E2 RID: 738
		public byte guanjue;

		// Token: 0x040002E3 RID: 739
		public uint rid_id;

		// Token: 0x040002E4 RID: 740
		public string legion_name;

		// Token: 0x040002E5 RID: 741
		public short legion_place;

		// Token: 0x040002E6 RID: 742
		public byte legion_title;

		// Token: 0x040002E7 RID: 743
		public uint map_id;

		// Token: 0x040002E8 RID: 744
		public short x;

		// Token: 0x040002E9 RID: 745
		public short y;

		// Token: 0x040002EA RID: 746
		public byte dir;
	}
}
