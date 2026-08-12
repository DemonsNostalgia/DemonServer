using System;

namespace GameStruct
{
	// Token: 0x0200001D RID: 29
	public class BaseAttributeInfo
	{
		// Token: 0x06000134 RID: 308 RVA: 0x0000EE1C File Offset: 0x0000D01C
		public BaseAttributeInfo()
		{
			this.lv = 0;
			this.force = (this.dexterity = (this.health = this.soul));
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000EE58 File Offset: 0x0000D058
		public uint GetLife()
		{
			return (uint)(this.health * 10);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000EE74 File Offset: 0x0000D074
		public uint GetMana()
		{
			return (uint)(this.soul * 20);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000EE90 File Offset: 0x0000D090
		public uint GetMagicAttack()
		{
			return (uint)(this.force / 2);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000EEAC File Offset: 0x0000D0AC
		public uint GetAttack()
		{
			return (uint)this.force;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000EEC4 File Offset: 0x0000D0C4
		public uint GetDoage()
		{
			return (uint)this.dexterity;
		}

		// Token: 0x040000B7 RID: 183
		public byte lv;

		// Token: 0x040000B8 RID: 184
		public int force;

		// Token: 0x040000B9 RID: 185
		public int dexterity;

		// Token: 0x040000BA RID: 186
		public int health;

		// Token: 0x040000BB RID: 187
		public int soul;
	}
}
