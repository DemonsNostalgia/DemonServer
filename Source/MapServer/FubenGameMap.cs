using System;
using System.Collections.Generic;

namespace MapServer
{
	// Token: 0x02000048 RID: 72
	public class FubenGameMap
	{
		// Token: 0x060001A8 RID: 424 RVA: 0x00012BC0 File Offset: 0x00010DC0
		public uint GetMapID()
		{
			return this.mMapID;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00012BD8 File Offset: 0x00010DD8
		public FubenGameMap(uint id)
		{
			this.mMapID = id;
			this.mGameMap = new List<GameMap>();
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00012BF8 File Offset: 0x00010DF8
		public bool AddFubenMap(GameMap map)
		{
			bool result;
			if (this.mGameMap.Count >= 100)
			{
				result = false;
			}
			else
			{
				this.mGameMap.Add(map);
				result = true;
			}
			return result;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00012C30 File Offset: 0x00010E30
		public GameMap GetFubenMap()
		{
			for (int i = 0; i < this.mGameMap.Count; i++)
			{
				if (this.mGameMap[i].GetObjectCount(2) == 0)
				{
					this.mGameMap[i].last_null_tick = Environment.TickCount;
					return this.mGameMap[i];
				}
			}
			return null;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00012CA4 File Offset: 0x00010EA4
		public void Process()
		{
			List<GameMap> list = null;
			for (int i = 0; i < this.mGameMap.Count; i++)
			{
				if (this.mGameMap[i].GetObjectCount(2) == 0)
				{
					if (Environment.TickCount - this.mGameMap[i].last_null_tick > 60000)
					{
						if (list == null)
						{
							list = new List<GameMap>();
						}
						list.Add(this.mGameMap[i]);
					}
				}
				this.mGameMap[i].Process();
			}
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.mGameMap.Remove(list[i]);
					list[i] = null;
				}
				list.Clear();
			}
		}

		// Token: 0x0400035C RID: 860
		public uint mMapID;

		// Token: 0x0400035D RID: 861
		public List<GameMap> mGameMap;
	}
}
