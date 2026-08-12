using System;
using System.Collections.Generic;
using GameBase.Config;
using GameStruct;

namespace MapServer
{
	// Token: 0x0200000C RID: 12
	public class ConfigManager
	{
		// Token: 0x06000079 RID: 121 RVA: 0x00005330 File Offset: 0x00003530
		public static ConfigManager Instance()
		{
			if (ConfigManager.m_Instance == null)
			{
				ConfigManager.m_Instance = new ConfigManager();
			}
			return ConfigManager.m_Instance;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005364 File Offset: 0x00003564
		public ConfigManager()
		{
			this.mPacket = new VerPacket("");
			this.mDicNpc = new Dictionary<uint, NPCInfo>();
			this.mDicMonster = new Dictionary<uint, MonsterInfo>();
			this.mDicItemType = new Dictionary<uint, ItemTypeInfo>();
			this.mDicMagicType = new Dictionary<uint, MagicTypeInfo>();
			this.mDicAttribute = new Dictionary<byte, Dictionary<byte, BaseAttributeInfo>>();
			this.mDicLevelExp = new Dictionary<uint, Dictionary<byte, LevelExp>>();
			this.mDicDropItem = new Dictionary<uint, DropItemInfo>();
			this.mDicMapGate = new Dictionary<uint, List<MapGateInfo>>();
			this.mDicTrack = new Dictionary<uint, TrackInfo>();
			this.mDicGem = new Dictionary<uint, GemInfo>();
			this.mDicNpcShop = new Dictionary<uint, NpcShopInfo>();
			this.mDicItemAddition = new Dictionary<byte, List<ItemAdditionInfo>>();
			this.mDicEudemonInfo = new Dictionary<uint, EudemonInfo>();
			this.mDicChangeEggPrice = new Dictionary<uint, int>();
			this.mDicLookFace = new Dictionary<uint, LookFaceInfo>();
			this.mDicHair = new Dictionary<uint, HairInfo>();
			this.mDicWardrobeHair = new Dictionary<ulong, WardrobeHairInfo>();
			this.mDicWardrobeAvatar = new Dictionary<ulong, WardrobeAvatarInfo>();
			this.mListRobotInfo = new List<RobotInfo>();
			this.mDicAiInfo = new Dictionary<int, AiInfo>();
			this.mDicEudemonSoul = new Dictionary<int, EudemonSoulInfo>();
			this.mDicNpc.Clear();
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000545C File Offset: 0x0000365C
		public bool LoadConfig()
		{
			bool result;
			if (!this.LoadGameMapInfo())
			{
				Log.Instance().WriteLog("Failed to load the map file.");
				result = false;
			}
			else if (!this.LoadAiInfo())
			{
				Log.Instance().WriteLog("Failed to load the AI configuration file.");
				result = false;
			}
			else if (!this.LoadNpcInfo())
			{
				Log.Instance().WriteLog("Failed to load the NPC file.");
				result = false;
			}
			else if (!this.LoadMonsterInfo())
			{
				Log.Instance().WriteLog("Failed to load the monster file.");
				result = false;
			}
			else if (!this.LoadMagicTypeInfo())
			{
				Log.Instance().WriteLog("Failed to load the skill file.");
				result = false;
			}
			else if (!this.LoadGeneratorInfo())
			{
				Log.Instance().WriteLog("Failed to load the monster spawn file.");
				result = false;
			}
			else if (!this.LoadItemTypeInfo())
			{
				Log.Instance().WriteLog("Failed to load the item file.");
				result = false;
			}
			else if (!this.LoadGolbalScript())
			{
				Log.Instance().WriteLog("Failed to load the global script.");
				result = false;
			}
			else if (!this.LoadAttributeInfo())
			{
				Log.Instance().WriteLog("Failed to load the level attribute file.");
				result = false;
			}
			else if (!this.LoadLevelExpInfo())
			{
				Log.Instance().WriteLog("Failed to load the level experience file.");
				result = false;
			}
			else if (!this.LoadDropItemInfo())
			{
				Log.Instance().WriteLog("Failed to load the monster drop file.");
				result = false;
			}
			else if (!this.LoadMapGateInfo())
			{
				Log.Instance().WriteLog("Failed to load map portals.");
				result = false;
			}
			else if (!this.LoadRegionInfo())
			{
				Log.Instance().WriteLog("Failed to load the map parameter file.");
				result = false;
			}
			else if (!this.LoadMagicTrackInfo())
			{
				Log.Instance().WriteLog("Failed to load combo actions.");
				result = false;
			}
			else if (!EquipOperation.Instance().Load())
			{
				Log.Instance().WriteLog("Failed to load equipment operation data.");
				result = false;
			}
			else if (!this.LoadGemInfo())
			{
				Log.Instance().WriteLog("Failed to load the gem configuration file.");
				result = false;
			}
			else if (!this.LoadNpcShopInfo())
			{
				Log.Instance().WriteLog("Failed to load the NPC shop file.");
				result = false;
			}
			else if (!this.LoadItemAdditionInfo())
			{
				Log.Instance().WriteLog("Failed to load equipment enhancement data.");
				result = false;
			}
			else if (!this.LoadEudemonInfo())
			{
				Log.Instance().WriteLog("Failed to load Eudemon attributes.");
				result = false;
			}
			else if (!this.LoadChangeEggInfo())
			{
				Log.Instance().WriteLog(
					"Failed to load Batch Hatcher egg-exchange prices.");
				result = false;
			}
			else if (!this.LoadLookFaceInfo())
			{
				Log.Instance().WriteLog("Failed to load the avatar file.");
				result = false;
			}
			else if (!this.LoadHairInfo())
			{
				Log.Instance().WriteLog("Failed to load the hairstyle file.");
				result = false;
			}
			else if (!this.LoadWardrobeHairInfo())
			{
				Log.Instance().WriteLog("Failed to load the wardrobe hairstyle file.");
				result = false;
			}
			else if (!this.LoadWardrobeAvatarInfo())
			{
				Log.Instance().WriteLog("Failed to load the wardrobe avatar file.");
				result = false;
			}
			else if (!this.LoadRobotInfo())
			{
				Log.Instance().WriteLog("Failed to load robot data.");
				result = false;
			}
			else if (!this.LoadEudemonSoulInfo())
			{
				Log.Instance().WriteLog("Failed to load Eudemon composition data.");
				result = false;
			}
			else if (!ScriptTimerManager.Instance().Load())
			{
				Log.Instance().WriteLog("Failed to load the scheduled script manager file.");
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000579C File Offset: 0x0000399C
		private bool LoadGameMapInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/GameMap.csv");
			CsvFile csvFile = new CsvFile(text);
			bool result;
			if (text == "")
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < csvFile.GetLine(); i++)
				{
					MapInfo mapInfo = new MapInfo();
					string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
					mapInfo.id = Convert.ToUInt32(fieldInfoToValue);
					mapInfo.name = csvFile.GetFieldInfoToValue(i, "name");
					mapInfo.dmappath = csvFile.GetFieldInfoToValue(i, "dmap");
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "recallid");
					mapInfo.recallid = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "recallx");
					mapInfo.recallx = Convert.ToUInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "recally");
					mapInfo.recally = Convert.ToUInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "snows");
					mapInfo.issnows = Convert.ToBoolean(fieldInfoToValue);
					GameMap gameMap = new GameMap(mapInfo);
					if (!gameMap.Create())
					{
						Log.Instance().WriteLog("Failed to load map: " + mapInfo.name);
					}
					MapManager.Instance().AddMap(gameMap);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000058F4 File Offset: 0x00003AF4
		private bool LoadNpcInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Npc.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				NPCInfo npcinfo = new NPCInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
				npcinfo.id = Convert.ToUInt32(fieldInfoToValue);
				npcinfo.name = csvFile.GetFieldInfoToValue(i, "name");
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "mapid");
				npcinfo.mapid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "x");
				npcinfo.x = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "y");
				npcinfo.y = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "lookface");
				npcinfo.lookface = Convert.ToInt32(fieldInfoToValue);
				npcinfo.ScriptPath = csvFile.GetFieldInfoToValue(i, "script");
				if (npcinfo.ScriptPath != "null")
				{
					npcinfo.ScriptID = ScripteManager.Instance().LoadScripteFile(npcinfo.ScriptPath, false);
				}
				else
				{
					npcinfo.ScriptID = 0U;
				}
				NpcObject npcObject = new NpcObject(npcinfo);
				npcObject.SetID(npcinfo.id);
				npcObject.Name = npcinfo.name;
				npcObject.ScriptId = npcinfo.ScriptID;
				if (this.mDicNpc.ContainsKey(npcinfo.id))
				{
					Log.Instance().WriteLog("Duplicate NPC ID detected: " + npcinfo.name + " Duplicate: " + this.mDicNpc[npcinfo.id].name);
				}
				else
				{
					GameMap gameMapToID = MapManager.Instance().GetGameMapToID(npcinfo.mapid);
					if (gameMapToID != null)
					{
						gameMapToID.AddObject(npcObject, null);
						npcObject.SetPoint(npcinfo.x, npcinfo.y);
						this.mDicNpc[npcinfo.id] = npcinfo;
					}
				}
			}
			return true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00005B08 File Offset: 0x00003D08
		private bool LoadMonsterInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Monster.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				MonsterInfo monsterInfo = new MonsterInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
				monsterInfo.id = Convert.ToUInt32(fieldInfoToValue);
				monsterInfo.name = csvFile.GetFieldInfoToValue(i, "name");
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "ai");
				monsterInfo.ai = (int)Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "lookface");
				monsterInfo.lookface = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				monsterInfo.level = Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life");
				monsterInfo.life = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "mana");
				monsterInfo.mana = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_min");
				monsterInfo.attack_min = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_max");
				monsterInfo.attack_max = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense");
				monsterInfo.defense = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dodge");
				monsterInfo.dodge = Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "range");
				monsterInfo.range = Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_speed");
				monsterInfo.attack_speed = Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "move_speed");
				monsterInfo.move_speed = Convert.ToUInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "drop_group");
				monsterInfo.drop_group = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "eudemon_type");
				monsterInfo.eudemon_type = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "die_scripte_id");
				monsterInfo.die_scripte_id = Convert.ToUInt32(fieldInfoToValue);
				if (this.mDicMonster.ContainsKey(monsterInfo.id))
				{
					Log.Instance().WriteLog("Duplicate monster ID detected: " + monsterInfo.name + " Duplicate: " + this.mDicNpc[monsterInfo.id].name);
				}
				else
				{
					this.mDicMonster[monsterInfo.id] = monsterInfo;
				}
			}
			return true;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005D78 File Offset: 0x00003F78
		private bool LoadGeneratorInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Generator.csv");
			CsvFile csvFile = new CsvFile(text);
			GeneratorInfo generatorInfo = new GeneratorInfo();
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "mapid");
				generatorInfo.mapid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_x");
				generatorInfo.bound_x = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_y");
				generatorInfo.bound_y = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_cx");
				generatorInfo.bound_cx = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_cy");
				generatorInfo.bound_cy = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "amount");
				generatorInfo.amount = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "time");
				generatorInfo.time = Convert.ToUInt32(fieldInfoToValue) * 1000U;
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "monsterid");
				generatorInfo.monsterid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dir");
				generatorInfo.dir = Convert.ToByte(fieldInfoToValue);
				GameMap gameMapToID = MapManager.Instance().GetGameMapToID(generatorInfo.mapid);
				if (gameMapToID != null)
				{
					gameMapToID.CreateMonster(generatorInfo);
				}
			}
			return true;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005EF0 File Offset: 0x000040F0
		private bool LoadItemTypeInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Itemtype.csv");
			CsvFile csvFile = new CsvFile(text);
			int i = 0;
			try
			{
				while (i < csvFile.GetLine())
				{
					ItemTypeInfo itemTypeInfo = new ItemTypeInfo();
					string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
					itemTypeInfo.id = Convert.ToUInt32(fieldInfoToValue);
					itemTypeInfo.name = csvFile.GetFieldInfoToValue(i, "name");
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "req_profession");
					itemTypeInfo.req_profession = Convert.ToByte(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "req_level");
					itemTypeInfo.req_level = Convert.ToByte(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "req_sex");
					itemTypeInfo.req_sex = Convert.ToByte(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_min");
					itemTypeInfo.attack_min = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_max");
					itemTypeInfo.attack_max = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense");
					itemTypeInfo.defense = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic_defense");
					itemTypeInfo.magic_defense = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic_attack_min");
					itemTypeInfo.magic_attack_min = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic_attck_max");
					itemTypeInfo.magic_attck_max = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dodge");
					itemTypeInfo.dodge = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "hitrate");
					itemTypeInfo.hitrate = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "amount");
					itemTypeInfo.amount = Convert.ToUInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "amount_limit");
					itemTypeInfo.amount_limit = Convert.ToUInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "actionid");
					itemTypeInfo.actionid = Convert.ToUInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "price");
					itemTypeInfo.price = Convert.ToInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "monster_type");
					itemTypeInfo.monster_type = Convert.ToUInt32(fieldInfoToValue);
					itemTypeInfo.info = csvFile.GetFieldInfoToValue(i, "info");
					fieldInfoToValue = csvFile.GetFieldInfoToValue(
						i, "client_monopoly");
					if (!string.IsNullOrWhiteSpace(fieldInfoToValue))
					{
						itemTypeInfo.client_monopoly =
							Convert.ToUInt16(fieldInfoToValue);
						itemTypeInfo.client_monopoly_known = true;
					}
					this.mDicItemType[itemTypeInfo.id] = itemTypeInfo;
					i++;
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				Log.Instance().WriteLog("Failed to load the item database at line: " + i.ToString());
				return false;
			}
			return true;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000061A4 File Offset: 0x000043A4
		private bool LoadMagicTypeInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/MagicType.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				MagicTypeInfo magicTypeInfo = new MagicTypeInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
				magicTypeInfo.id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "typeid");
				magicTypeInfo.typeid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "sort");
				magicTypeInfo.sort = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "name");
				magicTypeInfo.name = fieldInfoToValue;
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "crime");
				magicTypeInfo.crime = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "ground");
				magicTypeInfo.ground = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "multi");
				magicTypeInfo.multi = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "target");
				magicTypeInfo.target = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				magicTypeInfo.level = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_mp");
				magicTypeInfo.use_mp = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_potential");
				magicTypeInfo.use_potential = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "power");
				magicTypeInfo.power = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "intone_speed");
				magicTypeInfo.intone_speed = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "percent");
				magicTypeInfo.percent = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "step_secs");
				magicTypeInfo.step_secs = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "range");
				magicTypeInfo.range = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "distance");
				magicTypeInfo.distance = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "status_chance");
				magicTypeInfo.status_chance = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "status");
				magicTypeInfo.status = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "need_prof");
				magicTypeInfo.need_prof = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "need_exp");
				magicTypeInfo.need_exp = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "need_level");
				magicTypeInfo.need_level = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "need_gemtype");
				magicTypeInfo.need_gemtype = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_xp");
				magicTypeInfo.use_xp = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "weapon_subtype");
				magicTypeInfo.weapon_subtype = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "active_times");
				magicTypeInfo.active_times = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "auto_active");
				magicTypeInfo.auto_active = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "floor_attr");
				magicTypeInfo.floor_attr = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "auto_learn");
				magicTypeInfo.auto_learn = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "learn_level");
				magicTypeInfo.learn_level = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "drop_weapon");
				magicTypeInfo.drop_weapon = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_ep");
				magicTypeInfo.use_ep = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "weapon_hit");
				magicTypeInfo.weapon_hit = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_item");
				magicTypeInfo.use_item = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "next_magic");
				magicTypeInfo.next_magic = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "delay_ms");
				magicTypeInfo.delay_ms = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "use_item_num");
				magicTypeInfo.use_item_num = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "width");
				magicTypeInfo.width = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "durability");
				magicTypeInfo.durability = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "apply_ms");
				magicTypeInfo.apply_ms = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "track_id");
				magicTypeInfo.track_id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "track_id2");
				magicTypeInfo.track_id2 = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "auto_learn_prob");
				magicTypeInfo.auto_learn_prob = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "group_type");
				magicTypeInfo.group_type = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "group_member1_pos");
				magicTypeInfo.group_member1_pos = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "group_member2_pos");
				magicTypeInfo.group_member2_pos = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "group_member3_pos");
				magicTypeInfo.group_member3_pos = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic1");
				magicTypeInfo.magic1 = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic2");
				magicTypeInfo.magic2 = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic3");
				magicTypeInfo.magic3 = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic4");
				magicTypeInfo.magic4 = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_combine");
				magicTypeInfo.attack_combine = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "flag");
				magicTypeInfo.flag = 0U;
				this.mDicMagicType[magicTypeInfo.id] = magicTypeInfo;
			}
			return true;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00006764 File Offset: 0x00004964
		private bool LoadGolbalScript()
		{
			string text = this.mPacket.LoadFileToText("data/config/Script.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "script");
				ScripteManager.Instance().LoadScripteFile(fieldInfoToValue, false);
			}
			return true;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000067C4 File Offset: 0x000049C4
		private bool LoadAttributeInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Attribute.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				BaseAttributeInfo baseAttributeInfo = new BaseAttributeInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "profession");
				byte key = Convert.ToByte(fieldInfoToValue);
				Dictionary<byte, BaseAttributeInfo> dictionary;
				if (!this.mDicAttribute.ContainsKey(key))
				{
					dictionary = new Dictionary<byte, BaseAttributeInfo>();
					this.mDicAttribute[key] = dictionary;
				}
				else
				{
					dictionary = this.mDicAttribute[key];
				}
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				baseAttributeInfo.lv = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "force");
				baseAttributeInfo.force = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dexterity");
				baseAttributeInfo.dexterity = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "health");
				baseAttributeInfo.health = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "soul");
				baseAttributeInfo.soul = Convert.ToInt32(fieldInfoToValue);
				dictionary[baseAttributeInfo.lv] = baseAttributeInfo;
			}
			return true;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00006904 File Offset: 0x00004B04
		private bool LoadLevelExpInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/LevelExp.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				LevelExp levelExp = new LevelExp();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "type");
				uint key = Convert.ToUInt32(fieldInfoToValue);
				Dictionary<byte, LevelExp> dictionary;
				if (!this.mDicLevelExp.ContainsKey(key))
				{
					dictionary = new Dictionary<byte, LevelExp>();
					this.mDicLevelExp[key] = dictionary;
				}
				else
				{
					dictionary = this.mDicLevelExp[key];
				}
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				levelExp.level = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "exp");
				levelExp.exp = Convert.ToUInt64(fieldInfoToValue);
				dictionary[levelExp.level] = levelExp;
			}
			return true;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000069F4 File Offset: 0x00004BF4
		private bool LoadDropItemInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/DropItem.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "group");
				uint num = Convert.ToUInt32(fieldInfoToValue);
				DropItemInfo dropItemInfo;
				if (this.mDicDropItem.ContainsKey(num))
				{
					dropItemInfo = this.mDicDropItem[num];
				}
				else
				{
					dropItemInfo = new DropItemInfo();
					dropItemInfo.groupid = num;
					this.mDicDropItem[num] = dropItemInfo;
				}
				DropItemClass dropItemClass = new DropItemClass();
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				if (fieldInfoToValue.IndexOf('|') != -1)
				{
					string[] array = fieldInfoToValue.Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < array.Length; j++)
					{
						uint num2 = Convert.ToUInt32(array[j]);
						if (ConfigManager.Instance().GetItemTypeInfo(num2) == null)
						{
							Log.Instance().WriteLog("Drop item ID was not found: " + num2.ToString());
						}
						dropItemClass.list_itemid.Add(num2);
					}
				}
				else
				{
					uint num2 = Convert.ToUInt32(fieldInfoToValue);
					if (ConfigManager.Instance().GetItemTypeInfo(num2) == null)
					{
						Log.Instance().WriteLog("Drop item ID was not found: " + num2.ToString());
					}
					dropItemClass.list_itemid.Add(num2);
				}
				dropItemInfo.listitem.Add(dropItemClass);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "amount");
				uint item = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "rate");
				uint item2 = Convert.ToUInt32(fieldInfoToValue);
				dropItemInfo.listamount.Add(item);
				dropItemInfo.listrate.Add(item2);
			}
			return true;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00006BF8 File Offset: 0x00004DF8
		private bool LoadGemInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/GemInfo.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				uint num = Convert.ToUInt32(fieldInfoToValue);
				if (ConfigManager.Instance().GetItemTypeInfo(num) == null)
				{
					Log.Instance().WriteLog("Failed to load gem data; item does not exist: " + num.ToString());
				}
				else
				{
					GemInfo gemInfo = new GemInfo();
					gemInfo.itemid = num;
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "type");
					gemInfo.type = Convert.ToByte(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "value");
					gemInfo.value = Convert.ToInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "amount");
					gemInfo.amount = Convert.ToInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "gemtype");
					gemInfo.gemtype = Convert.ToByte(fieldInfoToValue);
					this.mDicGem[gemInfo.itemid] = gemInfo;
				}
			}
			return true;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00006D24 File Offset: 0x00004F24
		private bool LoadNpcShopInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/NpcShop.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "npcid");
				uint num = Convert.ToUInt32(fieldInfoToValue);
				NpcShopInfo npcShopInfo;
				if (this.mDicNpcShop.ContainsKey(num))
				{
					npcShopInfo = this.mDicNpcShop[num];
				}
				else
				{
					npcShopInfo = new NpcShopInfo(num);
					this.mDicNpcShop[num] = npcShopInfo;
				}
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				uint itemid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "price");
				int price = Convert.ToInt32(fieldInfoToValue);
				npcShopInfo.AddItem(itemid, price);
			}
			return true;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00006E04 File Offset: 0x00005004
		private bool LoadItemAdditionInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/ItemAddition.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "type");
				byte b = Convert.ToByte(fieldInfoToValue);
				List<ItemAdditionInfo> list;
				if (this.mDicItemAddition.ContainsKey(b))
				{
					list = this.mDicItemAddition[b];
				}
				else
				{
					list = new List<ItemAdditionInfo>();
					this.mDicItemAddition[b] = list;
				}
				ItemAdditionInfo itemAdditionInfo = new ItemAdditionInfo();
				itemAdditionInfo.type = b;
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				itemAdditionInfo.level = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life");
				itemAdditionInfo.life = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "max_attack");
				itemAdditionInfo.max_attack = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "min_attack");
				itemAdditionInfo.min_attack = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense");
				itemAdditionInfo.defense = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "max_magicack");
				itemAdditionInfo.max_magic_attack = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "min_magicack");
				itemAdditionInfo.min_attack = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magic_defense");
				itemAdditionInfo.magic_defense = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dodge");
				itemAdditionInfo.dodge = Convert.ToUInt32(fieldInfoToValue);
				list.Add(itemAdditionInfo);
			}
			return true;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006FB0 File Offset: 0x000051B0
		public ItemAdditionInfo GetItemAdditionInfo(byte type, byte level)
		{
			if (this.mDicItemAddition.ContainsKey(type))
			{
				List<ItemAdditionInfo> list = this.mDicItemAddition[type];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].level == level)
					{
						return list[i];
					}
				}
			}
			return null;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00007020 File Offset: 0x00005220
		public NpcShopInfo GetNpcShopInfo(uint npcid)
		{
			NpcShopInfo result;
			if (this.mDicNpcShop.ContainsKey(npcid))
			{
				result = this.mDicNpcShop[npcid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00007058 File Offset: 0x00005258
		private bool LoadMagicTrackInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Track.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				TrackInfo trackInfo = new TrackInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
				trackInfo.id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id_next");
				trackInfo.id_next = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "direction");
				trackInfo.direction = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "step");
				trackInfo.step = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "alt");
				trackInfo.alt = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "action");
				trackInfo.action = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "power");
				trackInfo.power = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "apply_ms");
				trackInfo.apply_ms = Convert.ToInt32(fieldInfoToValue);
				this.mDicTrack[trackInfo.id] = trackInfo;
			}
			return true;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00007194 File Offset: 0x00005394
		private bool LoadRegionInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Region.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "mapid");
				MapRegionInfo info;
				info.mapid = Convert.ToUInt32(fieldInfoToValue);
				GameMap gameMapToID = MapManager.Instance().GetGameMapToID(info.mapid);
				if (gameMapToID == null)
				{
					Log.Instance().WriteLog("Failed to load map registration parameters; map ID was not found: " + info.mapid.ToString());
				}
				else
				{
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "type");
					info.type = Convert.ToInt32(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_x");
					info.bound_x = Convert.ToInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_y");
					info.bound_y = Convert.ToInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_cx");
					info.bound_cx = Convert.ToInt16(fieldInfoToValue);
					fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "bound_cy");
					info.bound_cy = Convert.ToInt16(fieldInfoToValue);
					gameMapToID.AddRegionInfo(info);
				}
			}
			return true;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000072D8 File Offset: 0x000054D8
		private bool LoadMapGateInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/MapGate.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "src_mapid");
				uint num = Convert.ToUInt32(fieldInfoToValue);
				List<MapGateInfo> list;
				if (this.mDicMapGate.ContainsKey(num))
				{
					list = this.mDicMapGate[num];
				}
				else
				{
					list = new List<MapGateInfo>();
					this.mDicMapGate[num] = list;
				}
				MapGateInfo mapGateInfo = new MapGateInfo();
				mapGateInfo.src_mapid = num;
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "src_x");
				mapGateInfo.src_x = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "src_y");
				mapGateInfo.src_y = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "target_mapid");
				mapGateInfo.target_mapid = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "target_x");
				mapGateInfo.target_x = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "target_y");
				mapGateInfo.target_y = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dis");
				mapGateInfo.dis = Convert.ToInt32(fieldInfoToValue);
				list.Add(mapGateInfo);
			}
			return true;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000743C File Offset: 0x0000563C
		private bool LoadEudemonInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Eudemon.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				EudemonInfo eudemonInfo = new EudemonInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				eudemonInfo.id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life_min");
				eudemonInfo.life_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life_max");
				eudemonInfo.life_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense_min");
				eudemonInfo.defense_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense_max");
				eudemonInfo.defense_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicdef_min");
				eudemonInfo.magicdef_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicdef_max");
				eudemonInfo.magicdef_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_min_min");
				eudemonInfo.atk_min_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_min_max");
				eudemonInfo.atk_min_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_max_min");
				eudemonInfo.atk_max_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_max_max");
				eudemonInfo.atk_max_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_min_min");
				eudemonInfo.magicatk_min_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_min_max");
				eudemonInfo.magicatk_min_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_max_min");
				eudemonInfo.magicatk_max_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_max_max");
				eudemonInfo.magicatk_max_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life_grow_min");
				eudemonInfo.life_grow_min = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "life_grow_max");
				eudemonInfo.life_grow_max = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense_grow_min");
				eudemonInfo.defense_grow_min = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "defense_grow_max");
				eudemonInfo.defense_grow_max = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicdef_grow_min");
				eudemonInfo.magicdef_grow_min = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicdef_grow_max");
				eudemonInfo.magicdef_grow_max = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_grow_min");
				eudemonInfo.atk_grow_min = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "atk_grow_max");
				eudemonInfo.atk_grow_max = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_grow_min");
				eudemonInfo.magicatk_grow_min = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "magicatk_grow_max");
				eudemonInfo.magicatk_grow_max = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "quality_min");
				eudemonInfo.quality_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "quality_max");
				eudemonInfo.qulity_max = Convert.ToInt32(fieldInfoToValue);
				this.mDicEudemonInfo[eudemonInfo.id] = eudemonInfo;
			}
			return true;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00007764 File Offset: 0x00005964
		public EudemonInfo GetEudemonInfo(uint id)
		{
			EudemonInfo result;
			if (this.mDicEudemonInfo.ContainsKey(id))
			{
				result = this.mDicEudemonInfo[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000779C File Offset: 0x0000599C
		private bool LoadHairInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Hair.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				HairInfo hairInfo = new HairInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				hairInfo.itemid = Convert.ToUInt32(fieldInfoToValue);
				hairInfo.name = csvFile.GetFieldInfoToValue(i, "name");
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "hairid");
				hairInfo.hairid = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "price");
				hairInfo.price = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "sex");
				hairInfo.sex = Convert.ToByte(fieldInfoToValue);
				this.mDicHair[hairInfo.itemid] = hairInfo;
			}
			return true;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00007880 File Offset: 0x00005A80
		public HairInfo GetHairInfo(uint itemid)
		{
			HairInfo result;
			if (this.mDicHair.ContainsKey(itemid))
			{
				result = this.mDicHair[itemid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		private bool LoadChangeEggInfo()
		{
			string text = this.mPacket.LoadFileToText(
				TextDefine.CONFIG_FILE_CHANGE_EGG);
			CsvFile csvFile = new CsvFile(text);
			this.mDicChangeEggPrice.Clear();
			for (int index = 0; index < csvFile.GetLine(); index++)
			{
				uint itemTypeId = Convert.ToUInt32(
					csvFile.GetFieldInfoToValue(index, "itemid"));
				int price = Convert.ToInt32(
					csvFile.GetFieldInfoToValue(index, "price"));
				if (price <= 0)
				{
					Log.Instance().WriteLog(
						"Invalid Batch Hatcher exchange price for item type " +
						itemTypeId.ToString() + ": " + price.ToString() + ".");
					return false;
				}
				if (this.mDicChangeEggPrice.ContainsKey(itemTypeId))
				{
					Log.Instance().WriteLog(
						"Duplicate Batch Hatcher exchange-price item type: " +
						itemTypeId.ToString() + ".");
					return false;
				}
				this.mDicChangeEggPrice.Add(itemTypeId, price);
			}
			return this.mDicChangeEggPrice.Count > 0;
		}

		public bool TryGetChangeEggPrice(uint itemTypeId, out int price)
		{
			if (this.mDicChangeEggPrice.TryGetValue(itemTypeId, out price))
			{
				return true;
			}
			// Every quality 0/1/2 triplet in the definitive ChangeEgg.ini has
			// the same price. The server Eudemon configuration uses the quality-0
			// base ID, so resolve those two client-visible variants through it.
			uint quality = itemTypeId % 10U;
			return (quality == 1U || quality == 2U) &&
				this.mDicChangeEggPrice.TryGetValue(
					itemTypeId - quality, out price);
		}

		private bool LoadWardrobeHairInfo()
		{
			string text = this.mPacket.LoadFileToText(
				"data/config/WardrobeHair.csv");
			if (text == "")
			{
				return false;
			}

			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				WardrobeHairInfo info = new WardrobeHairInfo();
				info.styleid = Convert.ToUInt32(
					csvFile.GetFieldInfoToValue(i, "styleid"));
				info.sex = Convert.ToByte(
					csvFile.GetFieldInfoToValue(i, "sex"));
				info.purchasecurrency = Convert.ToByte(
					csvFile.GetFieldInfoToValue(i, "purchasecurrency"));
				info.unlockprice = Convert.ToInt32(
					csvFile.GetFieldInfoToValue(i, "unlockprice"));
				info.changeprice = Convert.ToInt32(
					csvFile.GetFieldInfoToValue(i, "changeprice"));

				if (info.styleid == 0U ||
					(info.sex != 1 && info.sex != 2) ||
					(info.purchasecurrency != 0 &&
					 info.purchasecurrency != 1) ||
					info.unlockprice < 0 || info.changeprice < 0)
				{
					Log.Instance().WriteLog(
						"Invalid wardrobe hairstyle configuration at row " +
						(i + 1).ToString() + ".");
					return false;
				}

				ulong key = MakeWardrobeHairKey(info.styleid, info.sex);
				if (this.mDicWardrobeHair.ContainsKey(key))
				{
					Log.Instance().WriteLog(
						"Duplicate wardrobe hairstyle configuration: style " +
						info.styleid.ToString() + ", sex " +
						info.sex.ToString() + ".");
					return false;
				}
				this.mDicWardrobeHair.Add(key, info);
			}
			return this.mDicWardrobeHair.Count > 0;
		}

		public WardrobeHairInfo GetWardrobeHairInfo(uint styleId, byte sex)
		{
			WardrobeHairInfo info;
			this.mDicWardrobeHair.TryGetValue(
				MakeWardrobeHairKey(styleId, sex), out info);
			return info;
		}

		private static ulong MakeWardrobeHairKey(uint styleId, byte sex)
		{
			return ((ulong)sex << 32) | styleId;
		}

		private bool LoadWardrobeAvatarInfo()
		{
			string text = this.mPacket.LoadFileToText(
				"data/config/WardrobeAvatar.csv");
			if (text == "")
			{
				return false;
			}

			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				WardrobeAvatarInfo info = new WardrobeAvatarInfo();
				info.styleid = Convert.ToUInt32(
					csvFile.GetFieldInfoToValue(i, "styleid"));
				info.sex = Convert.ToByte(
					csvFile.GetFieldInfoToValue(i, "sex"));
				info.purchasecurrency = Convert.ToByte(
					csvFile.GetFieldInfoToValue(i, "purchasecurrency"));
				info.unlockprice = Convert.ToInt32(
					csvFile.GetFieldInfoToValue(i, "unlockprice"));
				info.job = Convert.ToByte(
					csvFile.GetFieldInfoToValue(i, "job"));
				info.changeprice = Convert.ToInt32(
					csvFile.GetFieldInfoToValue(i, "changeprice"));

				if ((info.sex != 1 && info.sex != 2) ||
					(info.purchasecurrency != 0 &&
					 info.purchasecurrency != 1) ||
					info.unlockprice < 0 || info.changeprice < 0)
				{
					Log.Instance().WriteLog(
						"Invalid wardrobe avatar configuration at row " +
						(i + 1).ToString() + ".");
					return false;
				}

				ulong key = MakeWardrobeAvatarKey(info.styleid, info.sex);
				if (this.mDicWardrobeAvatar.ContainsKey(key))
				{
					Log.Instance().WriteLog(
						"Duplicate wardrobe avatar configuration: style " +
						info.styleid.ToString() + ", sex " +
						info.sex.ToString() + ".");
					return false;
				}
				this.mDicWardrobeAvatar.Add(key, info);
			}
			return this.mDicWardrobeAvatar.Count > 0;
		}

		public WardrobeAvatarInfo GetWardrobeAvatarInfo(
			uint styleId,
			byte sex)
		{
			WardrobeAvatarInfo info;
			this.mDicWardrobeAvatar.TryGetValue(
				MakeWardrobeAvatarKey(styleId, sex), out info);
			return info;
		}

		private static ulong MakeWardrobeAvatarKey(uint styleId, byte sex)
		{
			return ((ulong)sex << 32) | styleId;
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000078B8 File Offset: 0x00005AB8
		private bool LoadLookFaceInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/LookFace.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				LookFaceInfo lookFaceInfo = new LookFaceInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "itemid");
				lookFaceInfo.itemid = Convert.ToUInt32(fieldInfoToValue);
				lookFaceInfo.name = csvFile.GetFieldInfoToValue(i, "name");
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "lookfaceid");
				lookFaceInfo.lookfaceid = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "price");
				lookFaceInfo.price = Convert.ToInt32(fieldInfoToValue);
				this.mDicLookFace[lookFaceInfo.itemid] = lookFaceInfo;
			}
			return true;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00007984 File Offset: 0x00005B84
		private bool LoadEudemonSoulInfo()
		{
			this.mDicEudemonSoul.Clear();
			string text = this.mPacket.LoadFileToText("data/config/EudemonSoul.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				EudemonSoulInfo eudemonSoulInfo = new EudemonSoulInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "star");
				eudemonSoulInfo.star = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "level");
				eudemonSoulInfo.level = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "fu_level");
				eudemonSoulInfo.fu_level = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "fu_star");
				eudemonSoulInfo.fu_star = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "add_min");
				eudemonSoulInfo.add_min = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "add_max");
				eudemonSoulInfo.add_max = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "add_main");
				eudemonSoulInfo.add_main = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "add_fu");
				eudemonSoulInfo.add_fu = Convert.ToSingle(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "add_init");
				eudemonSoulInfo.add_init = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "notice");
				eudemonSoulInfo.bNotice = Convert.ToBoolean(fieldInfoToValue);
				this.mDicEudemonSoul[eudemonSoulInfo.star] = eudemonSoulInfo;
			}
			return true;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00007B00 File Offset: 0x00005D00
		private bool LoadRobotInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Robot.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				RobotInfo robotInfo = new RobotInfo();
				robotInfo.name = csvFile.GetFieldInfoToValue(i, "name");
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "lookface");
				robotInfo.lookface = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "hair");
				robotInfo.hair = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "armor_id");
				robotInfo.armor_id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "wepon_id");
				robotInfo.wepon_id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "guanjue");
				robotInfo.guanjue = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "rid_id");
				robotInfo.rid_id = Convert.ToUInt32(fieldInfoToValue);
				robotInfo.legion_name = csvFile.GetFieldInfoToValue(i, "legion_name");
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "legion_place");
				robotInfo.legion_place = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "legion_title");
				robotInfo.legion_title = Convert.ToByte(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "map_id");
				robotInfo.map_id = Convert.ToUInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "x");
				robotInfo.x = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "y");
				robotInfo.y = Convert.ToInt16(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "dir");
				robotInfo.dir = Convert.ToByte(fieldInfoToValue);
				this.mListRobotInfo.Add(robotInfo);
				if (robotInfo.legion_name.Length > 0)
				{
					RobotLegionManager.GetInstance().CreateLegion(robotInfo.legion_name);
				}
			}
			for (int i = 0; i < this.mListRobotInfo.Count; i++)
			{
				RobotInfo robotInfo = this.mListRobotInfo[i];
				GameMap gameMapToID = MapManager.Instance().GetGameMapToID(robotInfo.map_id);
				if (gameMapToID != null)
				{
					RobotObject robotObject = new RobotObject();
					robotObject.SetRobotInfo(robotInfo);
					gameMapToID.AddObject(robotObject, null);
				}
			}
			return true;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00007D58 File Offset: 0x00005F58
		public bool LoadAiInfo()
		{
			string text = this.mPacket.LoadFileToText("data/config/Ai.csv");
			CsvFile csvFile = new CsvFile(text);
			for (int i = 0; i < csvFile.GetLine(); i++)
			{
				AiInfo aiInfo = new AiInfo();
				string fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "id");
				aiInfo.nId = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "type");
				aiInfo.nType = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "range");
				aiInfo.nRange = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_range");
				aiInfo.nAttack_Range = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "move_speed");
				aiInfo.nMove_Speed = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "attack_speed");
				aiInfo.nAttack_Speed = Convert.ToInt32(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "idle_move");
				aiInfo.bIdle_Move = Convert.ToBoolean(fieldInfoToValue);
				fieldInfoToValue = csvFile.GetFieldInfoToValue(i, "move");
				aiInfo.bMove = Convert.ToBoolean(fieldInfoToValue);
				if (this.mDicAiInfo.ContainsKey(aiInfo.nId))
				{
					Log.Instance().WriteLog("Duplicate AI ID detected: " + aiInfo.nId.ToString());
				}
				if (aiInfo.nType != 0 && aiInfo.nType != 1)
				{
					Log.Instance().WriteLog("Invalid AI type; reset to passive. AI ID: " + aiInfo.nId.ToString());
					aiInfo.nType = 0;
				}
				this.mDicAiInfo[aiInfo.nId] = aiInfo;
			}
			return true;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00007F14 File Offset: 0x00006114
		public EudemonSoulInfo GetEudemonSoulInfo(int nStar)
		{
			EudemonSoulInfo result;
			if (this.mDicEudemonSoul.ContainsKey(nStar))
			{
				result = this.mDicEudemonSoul[nStar];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00007F4C File Offset: 0x0000614C
		public AiInfo GetAIInfo(int nAi_id)
		{
			AiInfo result;
			if (this.mDicAiInfo.ContainsKey(nAi_id))
			{
				result = this.mDicAiInfo[nAi_id];
			}
			else
			{
				Log.Instance().WriteLog("AI data was not found; reset to the first AI ID.");
				result = this.mDicAiInfo[1];
			}
			return result;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00007FA0 File Offset: 0x000061A0
		public LookFaceInfo GetLookFaceInfo(uint itemid)
		{
			LookFaceInfo result;
			if (this.mDicLookFace.ContainsKey(itemid))
			{
				result = this.mDicLookFace[itemid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00007FD8 File Offset: 0x000061D8
		public bool CheckMapGate(uint mapid, short x, short y, ref uint target_mapid, ref short target_x, ref short target_y)
		{
			if (this.mDicMapGate.ContainsKey(mapid))
			{
				List<MapGateInfo> list = this.mDicMapGate[mapid];
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].src_mapid == mapid)
					{
						if (Math.Abs((int)(list[i].src_x - x)) <= list[i].dis && Math.Abs((int)(list[i].src_y - y)) <= list[i].dis)
						{
							target_x = list[i].target_x;
							target_y = list[i].target_y;
							target_mapid = list[i].target_mapid;
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000080C4 File Offset: 0x000062C4
		public TrackInfo GetTrackInfo(uint id)
		{
			TrackInfo result;
			if (this.mDicTrack.ContainsKey(id))
			{
				result = this.mDicTrack[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000080FC File Offset: 0x000062FC
		public int GetTrackNumber(uint id)
		{
			int num = 0;
			uint id2 = id;
			for (;;)
			{
				TrackInfo trackInfo = this.GetTrackInfo(id2);
				if (trackInfo == null)
				{
					break;
				}
				num++;
				if (trackInfo.id_next == 0U)
				{
					break;
				}
				id2 = trackInfo.id_next;
			}
			return num;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00008154 File Offset: 0x00006354
		public int GetTrackTime(uint id)
		{
			int num = 0;
			uint id2 = id;
			for (;;)
			{
				TrackInfo trackInfo = this.GetTrackInfo(id2);
				if (trackInfo == null)
				{
					break;
				}
				num += trackInfo.apply_ms;
				if (trackInfo.id_next == 0U)
				{
					break;
				}
				id2 = trackInfo.id_next;
			}
			return num;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000081B0 File Offset: 0x000063B0
		public LevelExp GetLevelExp(uint id, byte level)
		{
			if (this.mDicLevelExp.ContainsKey(id))
			{
				Dictionary<byte, LevelExp> dictionary = this.mDicLevelExp[id];
				if (dictionary.ContainsKey(level))
				{
					return dictionary[level];
				}
			}
			return null;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00008200 File Offset: 0x00006400
		public GemInfo GetGemInfo(uint itemid)
		{
			GemInfo result;
			if (this.mDicGem.ContainsKey(itemid))
			{
				result = this.mDicGem[itemid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00008238 File Offset: 0x00006438
		public BaseAttributeInfo GetAttributeInfo(byte profession, byte level)
		{
			if (this.mDicAttribute.ContainsKey(profession))
			{
				Dictionary<byte, BaseAttributeInfo> dictionary = this.mDicAttribute[profession];
				if (dictionary.ContainsKey(level))
				{
					return dictionary[level];
				}
			}
			return null;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00008288 File Offset: 0x00006488
		public VerPacket GetVerPacket()
		{
			return this.mPacket;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000082A0 File Offset: 0x000064A0
		public NPCInfo GetNpcInfoToID(uint id)
		{
			NPCInfo result;
			if (this.mDicNpc.ContainsKey(id))
			{
				result = this.mDicNpc[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000082D8 File Offset: 0x000064D8
		public MonsterInfo GetMonsterInfo(uint id)
		{
			MonsterInfo result;
			if (this.mDicMonster.ContainsKey(id))
			{
				result = this.mDicMonster[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00008310 File Offset: 0x00006510
		public ItemTypeInfo GetItemTypeInfo(uint id)
		{
			ItemTypeInfo result;
			if (this.mDicItemType.ContainsKey(id))
			{
				result = this.mDicItemType[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00008348 File Offset: 0x00006548
		public MagicTypeInfo GetMagicTypeInfo(uint id, byte level = 0)
		{
			uint key = id * 10U + (uint)level;
			MagicTypeInfo result;
			if (this.mDicMagicType.ContainsKey(key))
			{
				result = this.mDicMagicType[key];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00008388 File Offset: 0x00006588
		public ItemTypeInfo GetItemTypeInfo(string name)
		{
			foreach (ItemTypeInfo itemTypeInfo in this.mDicItemType.Values)
			{
				if (itemTypeInfo.name == name)
				{
					return itemTypeInfo;
				}
			}
			return null;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00008400 File Offset: 0x00006600
		public DropItemInfo GetDropItemInfo(uint groupid)
		{
			DropItemInfo result;
			if (this.mDicDropItem.ContainsKey(groupid))
			{
				result = this.mDicDropItem[groupid];
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00008438 File Offset: 0x00006638
		public void ReloadAllScripte()
		{
			ScripteManager.Instance().ClearAllScripte();
			this.LoadGolbalScript();
			foreach (NPCInfo npcinfo in this.mDicNpc.Values)
			{
				ScripteManager.Instance().LoadScripteFile(npcinfo.ScriptPath, true);
			}
		}

		// Token: 0x04000049 RID: 73
		public static ConfigManager m_Instance = null;

		// Token: 0x0400004A RID: 74
		private VerPacket mPacket;

		// Token: 0x0400004B RID: 75
		private Dictionary<uint, NPCInfo> mDicNpc;

		// Token: 0x0400004C RID: 76
		private Dictionary<uint, MonsterInfo> mDicMonster;

		// Token: 0x0400004D RID: 77
		private Dictionary<uint, ItemTypeInfo> mDicItemType;

		// Token: 0x0400004E RID: 78
		private Dictionary<uint, MagicTypeInfo> mDicMagicType;

		// Token: 0x0400004F RID: 79
		private Dictionary<uint, Dictionary<byte, LevelExp>> mDicLevelExp;

		// Token: 0x04000050 RID: 80
		private Dictionary<byte, Dictionary<byte, BaseAttributeInfo>> mDicAttribute;

		// Token: 0x04000051 RID: 81
		private Dictionary<uint, DropItemInfo> mDicDropItem;

		// Token: 0x04000052 RID: 82
		private Dictionary<uint, List<MapGateInfo>> mDicMapGate;

		// Token: 0x04000053 RID: 83
		private Dictionary<uint, TrackInfo> mDicTrack;

		// Token: 0x04000054 RID: 84
		private Dictionary<uint, GemInfo> mDicGem;

		// Token: 0x04000055 RID: 85
		private Dictionary<uint, NpcShopInfo> mDicNpcShop;

		// Token: 0x04000056 RID: 86
		private Dictionary<byte, List<ItemAdditionInfo>> mDicItemAddition;

		// Token: 0x04000057 RID: 87
		private Dictionary<uint, EudemonInfo> mDicEudemonInfo;

		private Dictionary<uint, int> mDicChangeEggPrice;

		// Token: 0x04000058 RID: 88
		private Dictionary<uint, LookFaceInfo> mDicLookFace;

		// Token: 0x04000059 RID: 89
		private Dictionary<uint, HairInfo> mDicHair;

		private Dictionary<ulong, WardrobeHairInfo> mDicWardrobeHair;

		private Dictionary<ulong, WardrobeAvatarInfo> mDicWardrobeAvatar;

		// Token: 0x0400005A RID: 90
		private List<RobotInfo> mListRobotInfo;

		// Token: 0x0400005B RID: 91
		private Dictionary<int, EudemonSoulInfo> mDicEudemonSoul;

		// Token: 0x0400005C RID: 92
		private Dictionary<int, AiInfo> mDicAiInfo;
	}
}
