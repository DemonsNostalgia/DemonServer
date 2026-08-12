using System;
using System.Collections.Generic;
using GameBase.Config;

namespace MapServer
{
	// Token: 0x02000049 RID: 73
	public class MapManager
	{
		// Token: 0x060001AD RID: 429 RVA: 0x00012DA8 File Offset: 0x00010FA8
		public static MapManager Instance()
		{
			if (MapManager.m_Instance == null)
			{
				MapManager.m_Instance = new MapManager();
			}
			return MapManager.m_Instance;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00012DDA File Offset: 0x00010FDA
		public MapManager()
		{
			this.m_DicMap = new Dictionary<uint, GameMap>();
			this.m_DicFubenMap = new Dictionary<uint, FubenGameMap>();
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00012DFC File Offset: 0x00010FFC
		public bool AddMap(GameMap map)
		{
			bool result;
			if (this.m_DicMap.ContainsKey(map.GetID()))
			{
				Log.Instance().WriteLog("Failed to add map because it already exists. Map ID: " + map.GetID().ToString());
				result = false;
			}
			else
			{
				this.m_DicMap[map.GetID()] = map;
				result = true;
			}
			return result;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00012E64 File Offset: 0x00011064
		public GameMap AddFubenMap(uint mapid)
		{
			GameMap gameMapToID = this.GetGameMapToID(mapid);
			GameMap result;
			if (gameMapToID == null)
			{
				Log.Instance().WriteLog("Failed to create instance map. Map ID: " + mapid.ToString());
				result = null;
			}
			else
			{
				FubenGameMap fubenGameMap = null;
				if (this.m_DicFubenMap.ContainsKey(gameMapToID.GetMapInfo().id))
				{
					fubenGameMap = this.m_DicFubenMap[gameMapToID.GetMapInfo().id];
				}
				if (fubenGameMap == null)
				{
					fubenGameMap = new FubenGameMap(gameMapToID.GetMapInfo().id);
					this.m_DicFubenMap[gameMapToID.GetMapInfo().id] = fubenGameMap;
				}
				GameMap gameMap = fubenGameMap.GetFubenMap();
				if (gameMap == null)
				{
					gameMap = gameMapToID.Clone();
				}
				result = gameMap;
			}
			return result;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00012F48 File Offset: 0x00011148
		public void Process()
		{
			foreach (GameMap gameMap in this.m_DicMap.Values)
			{
				gameMap.Process();
			}
			foreach (FubenGameMap fubenGameMap in this.m_DicFubenMap.Values)
			{
				fubenGameMap.Process();
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x00012FF8 File Offset: 0x000111F8
		public GameMap GetGameMapToID(uint id)
		{
			GameMap result;
			if (this.m_DicMap.ContainsKey(id))
			{
				result = this.m_DicMap[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0400035E RID: 862
		private Dictionary<uint, GameMap> m_DicMap;

		// Token: 0x0400035F RID: 863
		private Dictionary<uint, FubenGameMap> m_DicFubenMap;

		// Token: 0x04000360 RID: 864
		private static MapManager m_Instance = null;
	}
}
