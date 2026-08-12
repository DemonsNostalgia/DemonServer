using System;
using System.Collections.Generic;

namespace GameStruct
{
	// Token: 0x02000037 RID: 55
	public class NpcShopInfo
	{
		// Token: 0x0600017B RID: 379 RVA: 0x0001033C File Offset: 0x0000E53C
		public NpcShopInfo(uint _npcid)
		{
			this.id = _npcid;
			this.mListItem = new List<uint>();
			this.mListPrice = new List<int>();
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00010364 File Offset: 0x0000E564
		public int GetItemPrice(uint itemid)
		{
			for (int i = 0; i < this.mListItem.Count; i++)
			{
				if (this.mListItem[i] == itemid)
				{
					return this.mListPrice[i];
				}
			}
			return -1;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000103B9 File Offset: 0x0000E5B9
		public void AddItem(uint itemid, int price)
		{
			this.mListItem.Add(itemid);
			this.mListPrice.Add(price);
		}

		// Token: 0x04000291 RID: 657
		public uint id;

		// Token: 0x04000292 RID: 658
		public List<uint> mListItem;

		// Token: 0x04000293 RID: 659
		public List<int> mListPrice;
	}
}
