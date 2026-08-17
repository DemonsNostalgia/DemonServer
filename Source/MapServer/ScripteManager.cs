using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameBase.Config;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200009D RID: 157
	public class ScripteManager
	{
		// Token: 0x060003F6 RID: 1014 RVA: 0x0002E66C File Offset: 0x0002C86C
		public static ScripteManager Instance()
		{
			if (ScripteManager.m_Instance == null)
			{
				ScripteManager.m_Instance = new ScripteManager();
			}
			return ScripteManager.m_Instance;
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0002E69E File Offset: 0x0002C89E
		public ScripteManager()
		{
			this.mszStr = "";
			this.mDicScripte = new Dictionary<uint, ActionInfo>();
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0002E6BF File Offset: 0x0002C8BF
		public void reset()
		{
			this.mnSelectIndex = 1;
			this.mbEndTag = false;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0002E6D0 File Offset: 0x0002C8D0
		public void ClearAllScripte()
		{
			this.mDicScripte.Clear();
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0002E6E0 File Offset: 0x0002C8E0
		public uint LoadScripteFile(string path, bool reload = false)
		{
			string text = "";
			uint num = 0U;
			try
			{
				if (path == "null")
				{
					return 0U;
				}
				if (!File.Exists(path))
				{
					Log.Instance().WriteLog("Failed to load script file: " + path);
					return 0U;
				}
				FileStream fileStream = new FileStream(path, FileMode.Open);
				StreamReader streamReader = new StreamReader(fileStream, Encoding.Default);
				string[] array;
				for (;;)
				{
					text = streamReader.ReadLine();
					if (text == null)
					{
						break;
					}
					if (text.Length > 0)
					{
						if (text[0] != '/' || text[1] != '/')
						{
							array = text.Split(new char[]
							{
								'\t'
							});
							if (array.Length != 6)
							{
								goto Block_9;
							}
							ActionInfo actionInfo = new ActionInfo();
							actionInfo.id = Convert.ToUInt32(array[0]);
							actionInfo.id_next = Convert.ToUInt32(array[1]);
							actionInfo.id_nextfail = Convert.ToUInt32(array[2]);
							actionInfo.type = Convert.ToUInt32(array[3]);
							actionInfo.data = Convert.ToUInt32(array[4]);
							actionInfo.param = array[5];
							if (!reload)
							{
								if (this.mDicScripte.ContainsKey(actionInfo.id))
								{
									Log.Instance().WriteLog("Duplicate ID while reading file: " + path);
									Log.Instance().WriteLog(string.Concat(new string[]
									{
										actionInfo.id.ToString(),
										" ",
										actionInfo.id_next.ToString(),
										" ",
										actionInfo.id_nextfail.ToString(),
										" ",
										actionInfo.type.ToString(),
										" ",
										actionInfo.data.ToString(),
										" ",
										actionInfo.param.ToString()
									}));
									Log.Instance().WriteLog(string.Concat(new string[]
									{
										this.mDicScripte[actionInfo.id].id.ToString(),
										" ",
										this.mDicScripte[actionInfo.id].id_next.ToString(),
										" ",
										this.mDicScripte[actionInfo.id].id_nextfail.ToString(),
										" ",
										this.mDicScripte[actionInfo.id].type.ToString(),
										" ",
										this.mDicScripte[actionInfo.id].data.ToString(),
										" ",
										this.mDicScripte[actionInfo.id].param.ToString()
									}));
								}
							}
							if (num == 0U)
							{
								num = actionInfo.id;
							}
							this.mDicScripte[actionInfo.id] = actionInfo;
						}
					}
				}
				fileStream.Dispose();
				goto IL_403;
				Block_9:
				if (array.Length == 2 && array[0] == "call")
				{
					num = Convert.ToUInt32(array[1]);
					fileStream.Dispose();
					return num;
				}
				Log.Instance().WriteLog("Error loading script file: " + path + " Data: " + text);
				fileStream.Dispose();
				return 1U;
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog(path);
				Log.Instance().WriteLog(text);
			}
			IL_403:
			return num;
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0002EB18 File Offset: 0x0002CD18
		public void ExecuteActionForNpc(uint npcid, PlayerObject play)
		{
			NPCInfo npcInfoToID = ConfigManager.Instance().GetNpcInfoToID(npcid);
			play.SetCurrentNpcInfo(npcInfoToID);
			if (npcInfoToID == null)
			{
				Log.Instance().WriteLog("Script execution failed; NPC ID script was not found: " + npcid.ToString());
			}
			else
			{
				this.ExecuteAction(npcInfoToID.ScriptID, play);
			}
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x0002EB74 File Offset: 0x0002CD74
		public void ExecuteAction(uint id, PlayerObject play)
		{
			this.reset();
			if (play != null)
			{
				play.ClearScriptMenuLink();
			}
			uint num = id;
			while (this.mDicScripte.ContainsKey(num))
			{
				ActionInfo actionInfo = this.mDicScripte[num];
				bool flag = false;
				try
				{
					flag = this.SWITCH(actionInfo, play);
				}
				catch (Exception ex)
				{
					Log.Instance().WriteLog(ex.Message);
					Log.Instance().WriteLog(ex.StackTrace);
					Log.Instance().WriteLog("Script execution failed. Script ID: " + actionInfo.id.ToString() + " Player name: " + play.GetName());
					flag = false;
				}
				if (flag)
				{
					num = actionInfo.id_next;
				}
				else
				{
					num = actionInfo.id_nextfail;
				}
				if (actionInfo.id_next == 0U && this.mbEndTag && play != null)
				{
					MsgNpcReply msgNpcReply = new MsgNpcReply();
					msgNpcReply.Create(null, play.GetGamePackKeyEx());
					play.SendData(msgNpcReply.Flush(), false);
				}
				else if (num != 0U)
				{
					continue;
				}
				return;
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0002ECB4 File Offset: 0x0002CEB4
		public void ExecuteOptionId(byte index, PlayerObject play, string szStr = "")
		{
			this.mszStr = szStr;
			if (play.GetMenuLink().ContainsKey(index))
			{
				this.ExecuteAction(play.GetMenuLink()[index], play);
			}
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0002ECF2 File Offset: 0x0002CEF2
		public void ExecuteOptionId(uint scriptid, PlayerObject play, string szStr = "")
		{
			this.mszStr = szStr;
			this.ExecuteAction(scriptid, play);
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0002ED08 File Offset: 0x0002CF08
		private bool SWITCH(ActionInfo info, PlayerObject play)
		{
			bool result = true;
			uint type = info.type;
			if (type <= 1004U)
			{
				if (type <= 126U)
				{
					switch (type)
					{
					case 101U:
						this.Action_MenuText(info, play);
						this.mbEndTag = true;
						break;
					case 102U:
						this.Action_MenuLink(info, play);
						this.mnSelectIndex += 1;
						break;
					case 103U:
						this.Action_MenuEdit(info, play);
						break;
					case 104U:
						this.Action_MenuImage(info, play);
						break;
					default:
						if (type == 126U)
						{
							this.Action_MessageBox(info, play);
						}
						break;
					}
				}
				else
				{
					switch (type)
					{
					case 501U:
						this.Action_Item_Add(info, play);
						break;
					case 502U:
						break;
					case 503U:
						this.Action_Item_Delete(info, play);
						break;
					case 504U:
						result = this.Action_Item_Level(info, play);
						break;
					case 505U:
						result = this.Action_Item_Delete_Name(info, play);
						break;
					case 506U:
						result = this.Action_Item_Delete_ItemID(info, play);
						break;
					case 507U:
						result = this.Action_Equip_Operation(info, play);
						break;
					case 508U:
						result = this.Action_Check_Bag_Size(info, play);
						break;
					default:
						switch (type)
						{
						case 1001U:
							this.Action_Map_EnterMap(info, play);
							break;
						case 1002U:
							this.Action_Map_Change(info, play);
							break;
						case 1003U:
							this.Action_Map_ReCall(info, play);
							break;
						case 1004U:
							this.Action_Map_Random(info, play);
							break;
						}
						break;
					}
				}
			}
			else if (type <= 2015U)
			{
				switch (type)
				{
				case 1501U:
					result = this.Action_CheckProfession(info, play);
					break;
				case 1502U:
					result = this.Action_CheckLevel(info, play);
					break;
				case 1503U:
					this.Action_Set_Role_Pro(info, play);
					break;
				case 1504U:
					this.Action_AddMagic(info, play);
					break;
				case 1505U:
					result = this.Action_Get_Role_Pro(info, play);
					break;
				case 1506U:
					result = this.Action_TimeOut_Create(info, play);
					break;
				case 1507U:
					result = this.Action_TimeOut_Check(info, play);
					break;
				case 1508U:
					this.Action_TimeOut_Delete(info, play);
					break;
				case 1509U:
					break;
				case 1510U:
					result = this.Action_Magic_Operation(info, play);
					break;
				default:
					switch (type)
					{
					case 2001U:
						this.Action_OpenDialog(info, play);
						break;
					case 2002U:
						this.Action_LearnMagic(info, play);
						break;
					case 2003U:
						result = this.Action_CheckMagic(info, play);
						break;
					case 2004U:
					{
						string text = this.Sprintf_string(info.param, play);
						play.LeftNotice(text);
						break;
					}
					case 2005U:
					{
						string text = this.Sprintf_string(info.param, play);
						play.ChatNotice(text);
						break;
					}
					case 2006U:
					{
						string text = this.Sprintf_string(info.param, play);
						UserEngine.Instance().SceneNotice(text);
						break;
					}
					case 2007U:
						this.Action_Random_Init(info, play);
						break;
					case 2008U:
						result = this.Action_Random_Compare(info, play);
						break;
					case 2009U:
					{
						string text = this.Sprintf_string(info.param, play);
						play.MsgBox(text);
						break;
					}
					case 2010U:
						play.Ptich();
						break;
					case 2011U:
						PayManager.Instance().GetMoney(play);
						break;
					case 2012U:
						this.Action_Fuck_Nian(info, play);
						break;
					case 2014U:
						result = this.Action_Get_Eudemon_Pro(info, play);
						break;
					case 2015U:
						this.Action_Set_Eudemon_Pro(info, play);
						break;
					}
					break;
				}
			}
			else
			{
				switch (type)
				{
				case 2501U:
					result = this.Action_Eudemon_Create(info, play);
					break;
				case 2502U:
					this.Action_Recall_Eudemon(info, play);
					break;
				case 2503U:
					this.Action_Eudemon_CreateEx(info, play);
					break;
				default:
					switch (type)
					{
					case 2601U:
						result = this.Action_Legion_Create(info, play);
						break;
					case 2602U:
						this.Action_Legion_ChangeTitle(info, play);
						break;
					case 2603U:
						result = this.Action_Family_Create(play);
						break;
					default:
						if (type == 3001U)
						{
							result = this.Action_Fuben_Create(info, play);
						}
						break;
					}
					break;
				}
			}
			return result;
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0002F140 File Offset: 0x0002D340
		private void Action_MenuText(ActionInfo info, PlayerObject play)
		{
			MsgNpcReply msgNpcReply = new MsgNpcReply();
			msgNpcReply.Create(null, play.GetGamePackKeyEx());
			msgNpcReply.interactType = 257;
			msgNpcReply.optionid = byte.MaxValue;
			msgNpcReply.text = this.Sprintf_string(info.param, play);
			play.SendData(msgNpcReply.GetBuffer(), false);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0002F19C File Offset: 0x0002D39C
		private void Action_MenuLink(ActionInfo info, PlayerObject play)
		{
			string text;
			uint targetActionId;
			if (!TryParseMenuLinkParameter(info.param, out text, out targetActionId))
			{
				Log.Instance().WriteLog(
					"Action_MenuLink has invalid parameters. ID: " +
					info.id.ToString() + " param: " + info.param);
				return;
			}

			MsgNpcReply msgNpcReply = new MsgNpcReply();
			msgNpcReply.Create(null, play.GetGamePackKeyEx());
			msgNpcReply.interactType = 258;
			msgNpcReply.param = 111;
			msgNpcReply.param2 = 112;
			msgNpcReply.param3[1] = 113;
			msgNpcReply.param3[2] = 114;
			msgNpcReply.param3[0] = 115;
			if (info.id_next == 0U)
			{
				msgNpcReply.optionid = byte.MaxValue;
			}
			else
			{
				msgNpcReply.optionid = this.mnSelectIndex;
			}
			play.GetMenuLink()[this.mnSelectIndex] = targetActionId;
			msgNpcReply.text = text;
			play.SendData(msgNpcReply.GetBuffer(), false);
		}

		internal static bool TryParseMenuLinkParameter(
			string parameter,
			out string text,
			out uint targetActionId)
		{
			text = string.Empty;
			targetActionId = 0U;
			if (string.IsNullOrWhiteSpace(parameter))
			{
				return false;
			}

			string trimmed = parameter.Trim();
			int separator = trimmed.LastIndexOf(' ');
			if (separator <= 0 ||
				!uint.TryParse(
					trimmed.Substring(separator + 1),
					out targetActionId))
			{
				return false;
			}

			text = trimmed.Substring(0, separator).TrimEnd();
			return text.Length > 0;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0002F26C File Offset: 0x0002D46C
		private void Action_MenuEdit(ActionInfo info, PlayerObject play)
		{
			ushort inputLength;
			uint taskID;
			string text;
			if (!TryParseMenuEditParameter(
					info.param,
					out inputLength,
					out taskID,
					out text))
			{
				Log.Instance().WriteLog(
					"Action_MenuEdit has invalid parameters. ID: " +
					info.id.ToString() + " param: " + info.param);
				return;
			}

			MsgNpcReply msgNpcReply = new MsgNpcReply();
			msgNpcReply.Create(null, play.GetGamePackKeyEx());
			msgNpcReply.interactType = 259;
			play.SetTaskID(taskID);
			msgNpcReply.param2 = inputLength;
			msgNpcReply.text = text;
			play.SendData(msgNpcReply.GetBuffer(), false);
		}

		internal static bool TryParseMenuEditParameter(
			string parameter,
			out ushort inputLength,
			out uint taskID,
			out string text)
		{
			inputLength = 0;
			taskID = 0U;
			text = string.Empty;
			if (string.IsNullOrWhiteSpace(parameter))
			{
				return false;
			}

			string[] parts = parameter.Trim().Split(
				new char[] { ' ' },
				3,
				StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length != 3 ||
				!ushort.TryParse(parts[0], out inputLength) ||
				!uint.TryParse(parts[1], out taskID))
			{
				return false;
			}

			text = parts[2].Trim();
			return text.Length > 0;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x0002F31C File Offset: 0x0002D51C
		private void Action_MessageBox(ActionInfo info, PlayerObject play)
		{
			string text = this.Sprintf_string(info.param, play);
			play.MsgBox(text);
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x0002F340 File Offset: 0x0002D540
		private void Action_MenuImage(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			MsgNpcReply msgNpcReply = new MsgNpcReply();
			msgNpcReply.Create(null, play.GetGamePackKeyEx());
			ushort imageid = Convert.ToUInt16(array[2]);
			play.SendData(msgNpcReply.NpcImage(imageid), false);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0002F394 File Offset: 0x0002D594
		private void Action_Map_EnterMap(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length < 2)
			{
				Log.Instance().WriteLog("Invalid script parameters. ID: " + info.id.ToString() + " param:" + info.param);
			}
			else
			{
				uint mapid = Convert.ToUInt32(array[0]);
				short x = Convert.ToInt16(array[1]);
				short y = Convert.ToInt16(array[2]);
				byte dir = Convert.ToByte(array[3]);
				play.FlyMap(mapid, x, y, dir);
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0002F42C File Offset: 0x0002D62C
		private void Action_Item_Add(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length < 2)
			{
				Log.Instance().WriteLog("Invalid script parameters. ID: " + info.id.ToString() + " param:" + info.param);
			}
			else
			{
				uint itemid = Convert.ToUInt32(array[0]);
				byte postion = Convert.ToByte(array[1]);
				byte stronglv = 0;
				byte b = 1;
				if (array.Length >= 3)
				{
					b = Convert.ToByte(array[2]);
				}
				if (array.Length >= 4)
				{
					stronglv = Convert.ToByte(array[3]);
				}
				byte gem = 0;
				byte gem2 = 0;
				byte gem3 = 0;
				if (array.Length >= 5)
				{
					gem = Convert.ToByte(array[4]);
				}
				if (array.Length >= 6)
				{
					gem2 = Convert.ToByte(array[5]);
				}
				if (array.Length >= 7)
				{
					gem3 = Convert.ToByte(array[6]);
				}
				byte warghost_exp = 0;
				if (array.Length >= 8)
				{
					warghost_exp = Convert.ToByte(array[7]);
				}
				byte di_attack = 0;
				if (array.Length >= 9)
				{
					di_attack = Convert.ToByte(array[8]);
				}
				byte shui_attack = 0;
				byte huo_attack = 0;
				byte feng_attack = 0;
				if (array.Length >= 10)
				{
					shui_attack = Convert.ToByte(array[9]);
				}
				if (array.Length >= 11)
				{
					huo_attack = Convert.ToByte(array[10]);
				}
				if (array.Length >= 12)
				{
					feng_attack = Convert.ToByte(array[11]);
				}
				for (int i = 0; i < (int)b; i++)
				{
					play.GetItemSystem().AwardItem(itemid, postion, b, stronglv, gem, gem2, gem3, warghost_exp, di_attack, shui_attack, huo_attack, feng_attack, true);
				}
			}
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0002F5E4 File Offset: 0x0002D7E4
		private bool Action_CheckProfession(ActionInfo info, PlayerObject play)
		{
			byte b = Convert.ToByte(info.param);
			return play.GetBaseAttr().profession == b;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0002F620 File Offset: 0x0002D820
		private bool Action_CheckLevel(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length == 2)
			{
				byte b = Convert.ToByte(array[1]);
				string text = array[0];
				if (text != null)
				{
					if (text == "<")
					{
						return play.GetBaseAttr().level < b;
					}
					if (text == "=")
					{
						return play.GetBaseAttr().level == b;
					}
					if (text == ">")
					{
						return play.GetBaseAttr().level > b;
					}
				}
			}
			return false;
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0002F6D4 File Offset: 0x0002D8D4
		private bool Action_Get_Role_Pro(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			bool result;
			if (array.Length != 3)
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Get_Role_Pro: " + info.param);
				result = false;
			}
			else
			{
				string text = array[0];
				int num = Convert.ToInt32(array[2]);
				if (text == "godship")
				{
					return this.CompareIntegerProperty((int)play.GetBaseAttr().godship, array[1], num);
				}
				if (text == "godtype")
				{
					return this.CompareIntegerProperty((int)play.GetBaseAttr().godtype, array[1], num);
				}
				string text2 = text;
				if (text2 != null)
				{
					if (!(text2 == "gold"))
					{
						if (!(text2 == "gamegold"))
						{
							if (!(text2 == "level"))
							{
								if (!(text2 == "godlevel"))
								{
									if (!(text2 == "pk"))
									{
										if (text2 == "maxeudemon")
										{
											text2 = array[1];
											if (text2 != null)
											{
												if (text2 == ">")
												{
													return (int)play.GetBaseAttr().maxeudemon > num;
												}
												if (text2 == "<")
												{
													return (int)play.GetBaseAttr().maxeudemon < num;
												}
												if (text2 == "=")
												{
													return (int)play.GetBaseAttr().maxeudemon == num;
												}
												if (text2 == ">=")
												{
													return (int)play.GetBaseAttr().maxeudemon >= num;
												}
												if (text2 == "<=")
												{
													return (int)play.GetBaseAttr().maxeudemon <= num;
												}
											}
										}
									}
									else
									{
										text2 = array[1];
										if (text2 != null)
										{
											if (text2 == ">")
											{
												return (int)play.GetBaseAttr().pk > num;
											}
											if (text2 == "<")
											{
												return (int)play.GetBaseAttr().pk < num;
											}
											if (text2 == "=")
											{
												return (int)play.GetBaseAttr().pk > num;
											}
											if (text2 == ">=")
											{
												return (int)play.GetBaseAttr().pk >= num;
											}
											if (text2 == "<=")
											{
												return (int)play.GetBaseAttr().pk <= num;
											}
										}
									}
								}
								else
								{
									text2 = array[1];
									if (text2 != null)
									{
										if (text2 == ">")
										{
											return (int)play.GetBaseAttr().godlevel > num;
										}
										if (text2 == "<")
										{
											return (int)play.GetBaseAttr().godlevel < num;
										}
										if (text2 == "=")
										{
											return (int)play.GetBaseAttr().godlevel == num;
										}
										if (text2 == ">=")
										{
											return (int)play.GetBaseAttr().godlevel >= num;
										}
										if (text2 == "<=")
										{
											return (int)play.GetBaseAttr().godlevel <= num;
										}
									}
								}
							}
							else
							{
								text2 = array[1];
								if (text2 != null)
								{
									if (text2 == ">")
									{
										return (int)play.GetBaseAttr().level > num;
									}
									if (text2 == "<")
									{
										return (int)play.GetBaseAttr().level < num;
									}
									if (text2 == "=")
									{
										return (int)play.GetBaseAttr().level == num;
									}
									if (text2 == ">=")
									{
										return (int)play.GetBaseAttr().level >= num;
									}
									if (text2 == "<=")
									{
										return (int)play.GetBaseAttr().level <= num;
									}
								}
							}
						}
						else
						{
							text2 = array[1];
							if (text2 != null)
							{
								if (text2 == ">")
								{
									return play.GetBaseAttr().gamegold > num;
								}
								if (text2 == "<")
								{
									return play.GetBaseAttr().gamegold < num;
								}
								if (text2 == "=")
								{
									return play.GetBaseAttr().gamegold == num;
								}
								if (text2 == ">=")
								{
									return play.GetBaseAttr().gamegold >= num;
								}
								if (text2 == "<=")
								{
									return play.GetBaseAttr().gamegold <= num;
								}
							}
						}
					}
					else
					{
						text2 = array[1];
						if (text2 != null)
						{
							if (text2 == ">")
							{
								return play.GetBaseAttr().gold > num;
							}
							if (text2 == "<")
							{
								return play.GetBaseAttr().gold < num;
							}
							if (text2 == "=")
							{
								return play.GetBaseAttr().gold == num;
							}
							if (text2 == ">=")
							{
								return play.GetBaseAttr().gold >= num;
							}
							if (text2 == "<=")
							{
								return play.GetBaseAttr().gold <= num;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private bool CompareIntegerProperty(int currentValue, string operation, int expectedValue)
		{
			switch (operation)
			{
			case ">":
				return currentValue > expectedValue;
			case "<":
				return currentValue < expectedValue;
			case "=":
				return currentValue == expectedValue;
			case ">=":
				return currentValue >= expectedValue;
			case "<=":
				return currentValue <= expectedValue;
			default:
				return false;
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0002FC58 File Offset: 0x0002DE58
		private void Action_Set_Role_Pro(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			string text = array[0];
			string text2 = text;
			switch (text2)
			{
			case "level":
			{
				byte b = Convert.ToByte(array[2]);
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "+"))
					{
						if (!(text2 == "-"))
						{
							if (text2 == "=")
							{
								play.GetBaseAttr().level = b;
								play.ChangeAttribute(UserAttribute.LEVEL, 0, true);
							}
						}
						else
						{
							play.ChangeAttribute(UserAttribute.LEVEL, (int)(-(int)b), true);
						}
					}
					else
					{
						play.ChangeAttribute(UserAttribute.LEVEL, (int)b, true);
					}
				}
				break;
			}
			case "godlevel":
			{
				byte b2 = Convert.ToByte(array[2]);
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "+"))
					{
						if (!(text2 == "-"))
						{
							if (text2 == "=")
							{
								play.GetBaseAttr().godlevel = b2;
							}
						}
						else
						{
							PlayerAttribute baseAttr = play.GetBaseAttr();
							baseAttr.godlevel -= b2;
						}
					}
					else
					{
						PlayerAttribute baseAttr2 = play.GetBaseAttr();
						baseAttr2.godlevel += b2;
					}
				}
				break;
			}
			case "godship":
			{
				byte godship;
				if (array.Length != 3 || array[1] != "=" || !byte.TryParse(array[2], out godship))
				{
					Log.Instance().WriteLog("Invalid parameters for Godship assignment: " + info.param);
					break;
				}
				if (godship < 1 || godship > 4)
				{
					Log.Instance().WriteLog("Rejected invalid Godship value: " + godship.ToString());
					break;
				}
				PlayerAttribute baseAttr3 = play.GetBaseAttr();
				if (baseAttr3.godlevel < 1)
				{
					Log.Instance().WriteLog("Rejected Godship assignment before Apotheosis for role " + play.GetName());
					break;
				}
				if (baseAttr3.godship != 0)
				{
					Log.Instance().WriteLog("Rejected Godship reassignment for role " + play.GetName());
					break;
				}
				baseAttr3.godship = godship;
				DBServer.Instance().SaveRoleData(play, false);
				Log.Instance().WriteLog("Godship " + godship.ToString() + " assigned to role " + play.GetName());
				break;
			}
			case "godtype":
			{
				byte godtype;
				if (array.Length != 3 || array[1] != "=" || !byte.TryParse(array[2], out godtype))
				{
					Log.Instance().WriteLog("Invalid parameters for deity assignment: " + info.param);
					break;
				}
				if (godtype < 1 || godtype > 12)
				{
					Log.Instance().WriteLog("Rejected invalid deity value: " + godtype.ToString());
					break;
				}
				PlayerAttribute deityAttr = play.GetBaseAttr();
				byte requiredGodship = (byte)(((int)godtype - 1) / 3 + 1);
				if (deityAttr.godlevel < 1 || deityAttr.godship != requiredGodship)
				{
					Log.Instance().WriteLog("Rejected deity outside the stored Godship for role " + play.GetName());
					break;
				}
				if (deityAttr.godtype != 0)
				{
					Log.Instance().WriteLog("Rejected deity reassignment for role " + play.GetName());
					break;
				}
				deityAttr.godtype = godtype;
				play.ReconcileGodshipSkills();
				DBServer.Instance().SaveRoleData(play, false);
				play.SendGodshipInfo();
				Log.Instance().WriteLog("Deity " + godtype.ToString() + " assigned to role " + play.GetName());
				break;
			}
			case "hair":
				play.ChangeAttribute(UserAttribute.HAIR, Convert.ToInt32(array[1]), true);
				break;
			case "gold":
			{
				int num2 = Convert.ToInt32(array[2]);
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "+"))
					{
						if (!(text2 == "-"))
						{
							if (text2 == "=")
							{
								play.GetBaseAttr().gold = num2;
								play.ChangeAttribute(UserAttribute.GOLD, 0, true);
							}
						}
						else
						{
							play.ChangeAttribute(UserAttribute.GOLD, -num2, true);
						}
					}
					else
					{
						play.ChangeAttribute(UserAttribute.GOLD, num2, true);
					}
				}
				break;
			}
			case "gamegold":
			{
				int num3 = Convert.ToInt32(array[2]);
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "+"))
					{
						if (!(text2 == "-"))
						{
							if (text2 == "=")
							{
								play.GetBaseAttr().gamegold = num3;
								play.ChangeAttribute(UserAttribute.GAMEGOLD, 0, true);
							}
						}
						else
						{
							play.ChangeAttribute(UserAttribute.GAMEGOLD, -num3, true);
						}
					}
					else
					{
						play.ChangeAttribute(UserAttribute.GAMEGOLD, num3, true);
					}
				}
				break;
			}
			case "job":
			{
				byte b3 = Convert.ToByte(array[2]);
				break;
			}
			case "pk":
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "="))
					{
						if (!(text2 == "+"))
						{
							if (text2 == "-")
							{
								PlayerAttribute baseAttr3 = play.GetBaseAttr();
								baseAttr3.pk -= Convert.ToInt16(array[2]);
							}
						}
						else
						{
							PlayerAttribute baseAttr4 = play.GetBaseAttr();
							baseAttr4.pk += Convert.ToInt16(array[2]);
						}
					}
					else
					{
						play.GetBaseAttr().pk = Convert.ToInt16(array[2]);
					}
				}
				play.ChangeAttribute(UserAttribute.PK, 0, false);
				break;
			case "maxeudemon":
				text2 = array[1];
				if (text2 != null)
				{
					if (!(text2 == "="))
					{
						if (!(text2 == "+"))
						{
							if (text2 == "-")
							{
								PlayerAttribute baseAttr5 = play.GetBaseAttr();
								baseAttr5.maxeudemon -= Convert.ToByte(array[2]);
							}
						}
						else
						{
							PlayerAttribute baseAttr6 = play.GetBaseAttr();
							baseAttr6.maxeudemon += Convert.ToByte(array[2]);
						}
					}
					else
					{
						play.GetBaseAttr().maxeudemon = Convert.ToByte(array[2]);
					}
				}
				play.ChangeAttribute(UserAttribute.MAXEUDEMON, 0, false);
				break;
			}
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00030094 File Offset: 0x0002E294
		private void Action_AddMagic(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			uint magidid = Convert.ToUInt32(array[0]);
			byte level = 0;
			if (array.Length >= 2)
			{
				level = Convert.ToByte(array[1]);
			}
			uint exp = 0U;
			if (array.Length >= 3)
			{
				exp = Convert.ToUInt32(array[2]);
			}
			play.GetMagicSystem().AddMagicInfo(magidid, level, exp);
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00030104 File Offset: 0x0002E304
		private void Action_OpenDialog(ActionInfo info, PlayerObject play)
		{
			int dwData = Convert.ToInt32(info.data);
			play.OpenDialog(dwData);
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00030128 File Offset: 0x0002E328
		private void Action_LearnMagic(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			uint magidid = Convert.ToUInt32(array[0]);
			byte level = 0;
			uint exp = 0U;
			if (array.Length >= 2)
			{
				level = Convert.ToByte(array[1]);
			}
			if (array.Length >= 3)
			{
				exp = Convert.ToUInt32(array[2]);
			}
			play.GetMagicSystem().AddMagicInfo(magidid, level, exp);
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x0003019C File Offset: 0x0002E39C
		private bool Action_Map_Random(ActionInfo info, PlayerObject play)
		{
			play.ScroolRandom(0, 0);
			return true;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x000301B8 File Offset: 0x0002E3B8
		private bool Action_Map_ReCall(ActionInfo info, PlayerObject play)
		{
			play.ReCallMap();
			return true;
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x000301D4 File Offset: 0x0002E3D4
		private void Action_Map_Change(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length == 3)
			{
				uint num = Convert.ToUInt32(array[0]);
				short x = Convert.ToInt16(array[1]);
				short y = Convert.ToInt16(array[2]);
				if (play.GetGameMap().GetMapInfo().id == num)
				{
					play.ScroolRandom(x, y);
				}
				else
				{
					play.ChangeMap(num, x, y);
				}
			}
			else
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Map_Change: " + info.param);
			}
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00030280 File Offset: 0x0002E480
		private bool Action_Item_Delete_ItemID(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			uint itemid = Convert.ToUInt32(array[0]);
			int num = Convert.ToInt32(array[1]);
			bool result;
			if (num <= 0)
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Item_Delete_Name.");
				result = false;
			}
			else
			{
				int num2 = 0;
				RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(itemid, ref num2);
				if (num2 < num)
				{
					result = false;
				}
				else
				{
					play.GetItemSystem().DeleteItemByItemID(itemid, num);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00030318 File Offset: 0x0002E518
		private bool Action_Item_Delete_Name(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			string name = array[0];
			int num = Convert.ToInt32(array[1]);
			bool result;
			if (num <= 0)
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Item_Delete_Name.");
				result = false;
			}
			else
			{
				int num2 = 0;
				RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(name, ref num2);
				if (num2 < num)
				{
					result = false;
				}
				else
				{
					play.GetItemSystem().DeleteItemByItemName(name, num);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000303A8 File Offset: 0x0002E5A8
		private void Action_Item_Delete(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			uint num = Convert.ToUInt32(array[0]);
			ushort num2 = 1;
			if (num == 0U)
			{
				num = play.GetItemSystem().GetScriptItemId();
			}
			if (array.Length == 2)
			{
				num2 = Convert.ToUInt16(array[1]);
			}
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(num);
			if (roleItemInfo != null)
			{
				RoleItemInfo roleItemInfo2 = roleItemInfo;
				roleItemInfo2.amount -= num2;
				if (roleItemInfo.amount == 0)
				{
					play.GetItemSystem().DeleteItemByID(roleItemInfo.id);
				}
				else
				{
					play.GetItemSystem().UpdateItemInfo(roleItemInfo.id);
				}
			}
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0003047C File Offset: 0x0002E67C
		private bool Action_Check_Bag_Size(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			string text = array[0];
			int num = Convert.ToInt32(array[1]);
			string text2 = text.ToLower();
			if (text2 != null)
			{
				if (text2 == "backpack")
				{
					return !play.GetItemSystem().CanAcceptAtPosition(
						MsgItemInfo.ITEMPOSITION_BACKPACK, num);
				}
			}
			return false;
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x000304EC File Offset: 0x0002E6EC
		private bool Action_Equip_Operation(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			string text = array[0].ToLower();
			string text2 = text;
			if (text2 != null)
			{
				if (text2 == "checkequip")
				{
					byte postion = Convert.ToByte(array[1]);
					return play.GetItemSystem().GetEquipByPostion(postion) != null;
				}
				if (!(text2 == "setequippro"))
				{
					if (text2 == "checkequippro")
					{
						byte postion = Convert.ToByte(array[1]);
						RoleItemInfo equipByPostion = play.GetItemSystem().GetEquipByPostion(postion);
						if (equipByPostion == null)
						{
							return false;
						}
						string text3 = array[2];
						string text4 = array[3];
						int num = Convert.ToInt32(array[4]);
						text2 = text3;
						if (text2 != null)
						{
							if (!(text2 == "shui_attack"))
							{
								if (!(text2 == "di_attack"))
								{
									if (!(text2 == "huo_attack"))
									{
										if (!(text2 == "feng_attack"))
										{
											if (text2 == "hole")
											{
												text2 = text4;
												if (text2 != null)
												{
													if (text2 == "=")
													{
														return equipByPostion.GetGemCount() == num;
													}
												}
											}
										}
										else
										{
											text2 = text4;
											if (text2 != null)
											{
												if (text2 == "=")
												{
													return (int)equipByPostion.feng_attack == num;
												}
												if (text2 == ">")
												{
													return (int)equipByPostion.feng_attack > num;
												}
												if (text2 == ">=")
												{
													return (int)equipByPostion.feng_attack >= num;
												}
												if (text2 == "<")
												{
													return (int)equipByPostion.feng_attack < num;
												}
												if (text2 == "<=")
												{
													return (int)equipByPostion.feng_attack <= num;
												}
											}
										}
									}
									else
									{
										text2 = text4;
										if (text2 != null)
										{
											if (text2 == "=")
											{
												return (int)equipByPostion.huo_attack == num;
											}
											if (text2 == ">")
											{
												return (int)equipByPostion.huo_attack > num;
											}
											if (text2 == ">=")
											{
												return (int)equipByPostion.huo_attack >= num;
											}
											if (text2 == "<")
											{
												return (int)equipByPostion.huo_attack < num;
											}
											if (text2 == "<=")
											{
												return (int)equipByPostion.huo_attack <= num;
											}
										}
									}
								}
								else
								{
									text2 = text4;
									if (text2 != null)
									{
										if (text2 == "=")
										{
											return (int)equipByPostion.di_attack == num;
										}
										if (text2 == ">")
										{
											return (int)equipByPostion.di_attack > num;
										}
										if (text2 == ">=")
										{
											return (int)equipByPostion.di_attack >= num;
										}
										if (text2 == "<")
										{
											return (int)equipByPostion.di_attack < num;
										}
										if (text2 == "<=")
										{
											return (int)equipByPostion.di_attack <= num;
										}
									}
								}
							}
							else
							{
								text2 = text4;
								if (text2 != null)
								{
									if (text2 == "=")
									{
										return (int)equipByPostion.shui_attack == num;
									}
									if (text2 == ">")
									{
										return (int)equipByPostion.shui_attack > num;
									}
									if (text2 == ">=")
									{
										return (int)equipByPostion.shui_attack >= num;
									}
									if (text2 == "<")
									{
										return (int)equipByPostion.shui_attack < num;
									}
									if (text2 == "<=")
									{
										return (int)equipByPostion.shui_attack <= num;
									}
								}
							}
						}
					}
				}
				else
				{
					byte postion = Convert.ToByte(array[1]);
					RoleItemInfo equipByPostion = play.GetItemSystem().GetEquipByPostion(postion);
					if (equipByPostion == null)
					{
						return false;
					}
					string text3 = array[2];
					bool result = true;
					text2 = text3;
					if (text2 != null)
					{
						if (text2 == "shui_attack")
						{
							string text4 = array[3];
							byte b = Convert.ToByte(array[4]);
							text2 = text4;
							if (text2 != null)
							{
								if (!(text2 == "="))
								{
									if (!(text2 == "-"))
									{
										if (text2 == "+")
										{
											RoleItemInfo roleItemInfo = equipByPostion;
											roleItemInfo.shui_attack += b;
										}
									}
									else
									{
										RoleItemInfo roleItemInfo2 = equipByPostion;
										roleItemInfo2.shui_attack -= b;
									}
								}
								else
								{
									equipByPostion.shui_attack = b;
								}
							}
							goto IL_341;
						}
						if (text2 == "di_attack")
						{
							string text4 = array[3];
							byte b = Convert.ToByte(array[4]);
							text2 = text4;
							if (text2 != null)
							{
								if (!(text2 == "="))
								{
									if (!(text2 == "-"))
									{
										if (text2 == "+")
										{
											RoleItemInfo roleItemInfo3 = equipByPostion;
											roleItemInfo3.di_attack += b;
										}
									}
									else
									{
										RoleItemInfo roleItemInfo4 = equipByPostion;
										roleItemInfo4.di_attack -= b;
									}
								}
								else
								{
									equipByPostion.di_attack = b;
								}
							}
							goto IL_341;
						}
						if (text2 == "huo_attack")
						{
							string text4 = array[3];
							byte b = Convert.ToByte(array[4]);
							text2 = text4;
							if (text2 != null)
							{
								if (!(text2 == "="))
								{
									if (!(text2 == "-"))
									{
										if (text2 == "+")
										{
											RoleItemInfo roleItemInfo5 = equipByPostion;
											roleItemInfo5.huo_attack += b;
										}
									}
									else
									{
										RoleItemInfo roleItemInfo6 = equipByPostion;
										roleItemInfo6.huo_attack -= b;
									}
								}
								else
								{
									equipByPostion.huo_attack = b;
								}
							}
							goto IL_341;
						}
						if (text2 == "feng_attack")
						{
							string text4 = array[3];
							byte b = Convert.ToByte(array[4]);
							text2 = text4;
							if (text2 != null)
							{
								if (!(text2 == "="))
								{
									if (!(text2 == "-"))
									{
										if (text2 == "+")
										{
											RoleItemInfo roleItemInfo7 = equipByPostion;
											roleItemInfo7.feng_attack += b;
										}
									}
									else
									{
										RoleItemInfo roleItemInfo8 = equipByPostion;
										roleItemInfo8.feng_attack -= b;
									}
								}
								else
								{
									equipByPostion.feng_attack = b;
								}
							}
							goto IL_341;
						}
						if (text2 == "hole")
						{
							string text4 = array[3];
							byte b = Convert.ToByte(array[4]);
							text2 = text4;
							if (text2 != null)
							{
								if (text2 == "=")
								{
									equipByPostion.OpenGem(b);
								}
							}
							goto IL_341;
						}
					}
					result = false;
					IL_341:
					play.GetItemSystem().UpdateItemInfo(equipByPostion.id);
					play.CalcAttribute();
					return result;
				}
			}
			return false;
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00030C14 File Offset: 0x0002EE14
		private bool Action_Item_Level(ActionInfo info, PlayerObject play)
		{
			uint scriptItemId = play.GetItemSystem().GetScriptItemId();
			RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(scriptItemId);
			bool result;
			if (roleItemInfo == null)
			{
				result = false;
			}
			else
			{
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
				if (itemTypeInfo == null)
				{
					result = false;
				}
				else
				{
					string[] array = info.param.Split(new char[]
					{
						' '
					});
					byte b = Convert.ToByte(array[1]);
					string text = array[0];
					if (text != null)
					{
						if (text == ">")
						{
							return b > itemTypeInfo.req_level;
						}
						if (text == "<")
						{
							return b < itemTypeInfo.req_level;
						}
						if (text == "=")
						{
							return b == itemTypeInfo.req_level;
						}
						if (text == ">=")
						{
							return b >= itemTypeInfo.req_level;
						}
						if (text == "<=")
						{
							return b <= itemTypeInfo.req_level;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00030D48 File Offset: 0x0002EF48
		public bool Action_CheckMagic(ActionInfo info, PlayerObject play)
		{
			uint typeid = Convert.ToUInt32(info.param);
			return play.GetMagicSystem().isMagic(typeid);
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00030D74 File Offset: 0x0002EF74
		private void Action_Eudemon_CreateEx(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length < 1)
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Eudemon_CreateEx: " + info.param + "id" + info.id.ToString());
			}
			else
			{
				uint num = Convert.ToUInt32(array[0]);
				if (ConfigManager.Instance().GetItemTypeInfo(num) == null)
				{
					Log.Instance().WriteLog("Action_Eudemon_CreateEx item ID does not exist: " + num.ToString());
				}
				else
				{
					byte b = 0;
					if (array.Length >= 2)
					{
						b = Convert.ToByte(array[1]);
					}
					int num2 = 0;
					if (array.Length >= 3)
					{
						num2 = Convert.ToInt32(array[2]);
					}
					byte b2 = 0;
					if (array.Length >= 4)
					{
						b2 = Convert.ToByte(array[3]);
					}
					RoleItemInfo roleItemInfo = play.GetItemSystem().AwardItem(num, 53, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
					if (roleItemInfo != null &&
						(b != 0 || num2 != 0 || b2 != 0))
					{
						roleItemInfo.typeid = IDManager.CreateTypeId(4);
						RoleData_Eudemon roleData_Eudemon = new RoleData_Eudemon();
						roleData_Eudemon.typeid = roleItemInfo.typeid;
						roleData_Eudemon.level = (short)b;
						roleData_Eudemon.quality = num2;
						roleData_Eudemon.wuxing = (int)b2;
						play.GetEudemonSystem().AddTempEudemon(roleData_Eudemon);
					}
				}
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00030EE4 File Offset: 0x0002F0E4
		private bool Action_Eudemon_Create(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			if (array.Length < 1)
			{
				Log.Instance().WriteLog("Invalid parameters for Action_Eudemon_Create: " + info.param + "id" + info.id.ToString());
				return false;
			}
			else
			{
				uint num = Convert.ToUInt32(array[0]);
				int num2 = 1;
				if (array.Length >= 2)
				{
					num2 = Convert.ToInt32(array[1]);
				}
				if (num2 <= 0)
				{
					Log.Instance().WriteLog(
						"Invalid Eudemon egg count for Action_Eudemon_Create: " +
						num2.ToString() + " id " + info.id.ToString() + ".");
					return false;
				}

				ItemTypeInfo itemTypeInfo =
					ConfigManager.Instance().GetItemTypeInfo(num);
				if (itemTypeInfo == null)
				{
					Log.Instance().WriteLog(
						"Failed to create Eudemon egg; item ID was not found: " +
						num.ToString());
					return false;
				}

				RoleItemInfo sourceItem = play.GetItemSystem().FindItem(
					play.GetItemSystem().GetScriptItemId());
				ItemTypeInfo sourceItemType = sourceItem == null ? null :
					ConfigManager.Instance().GetItemTypeInfo(sourceItem.itemid);
				if (EudemonHatchManager.IsItemTriggeredEggPackage(
					info.id, sourceItem, sourceItemType) &&
					EudemonHatchManager.CanCreateQueuedEggFromEudemonType(num))
				{
					return EudemonHatchManager.TryAwardQueuedEggs(
						play, num, num2);
				}

				if (!play.GetItemSystem().CanAcceptAtPosition(
					MsgItemInfo.ITEMPOSITION_EUDEMON_PACK, num2))
				{
					play.GetItemSystem().NotifyPackageFull(
						MsgItemInfo.ITEMPOSITION_EUDEMON_PACK);
					return false;
				}

				for (int index = 0; index < num2; index++)
				{
					if (play.GetItemSystem().AwardItem(
						num, MsgItemInfo.ITEMPOSITION_EUDEMON_PACK,
						1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true) == null)
					{
						Log.Instance().WriteLog(
							"Failed to create direct Eudemon item " +
							num.ToString() + " for role " +
							play.GetTypeId().ToString() + ".");
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00031004 File Offset: 0x0002F204
		public bool Action_Legion_Create(ActionInfo info, PlayerObject play)
		{
			string text = this.mszStr;
			this.mszStr = "";
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else if (play.GetLegionSystem().IsHaveLegion())
			{
				result = false;
			}
			else if (LegionManager.Instance().IsExist(text))
			{
				result = false;
			}
			else
			{
				string[] array = info.param.Split(new char[]
				{
					' '
				});
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				if ((int)play.GetBaseAttr().level < num)
				{
					result = false;
				}
				else if (play.GetMoneyCount(MONEYTYPE.GOLD) < num2)
				{
					result = false;
				}
				else
				{
					result = LegionManager.Instance().CreateLegion(
						play.GetBaseAttr().player_id,
						text,
						play.GetName(),
						1,
						(long)num3,
						"Announcement",
						num2);
				}
			}
			return result;
		}

		public bool Action_Family_Create(PlayerObject play)
		{
			string familyName = this.mszStr;
			this.mszStr = "";
			return FamilyManager.Instance().CreateFamily(play, familyName);
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0003111C File Offset: 0x0002F31C
		private void Action_Legion_ChangeTitle(ActionInfo info, PlayerObject play)
		{
			if (play.GetLegionSystem().IsHaveLegion())
			{
				byte title = Convert.ToByte(info.param);
				play.GetLegionSystem().ChangeLegionTitle(title);
			}
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x00031154 File Offset: 0x0002F354
		private bool Action_TimeOut_Create(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			int time_id = Convert.ToInt32(array[0]);
			int time = Convert.ToInt32(array[1]);
			uint callback_scripte_id = Convert.ToUInt32(array[2]);
			return ScriptTimerManager.Instance().AddPlayerTimeOut(time_id, play.GetBaseAttr().player_id, time, callback_scripte_id);
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000311B8 File Offset: 0x0002F3B8
		private bool Action_TimeOut_Check(ActionInfo info, PlayerObject play)
		{
			int time_id = Convert.ToInt32(info.param);
			return ScriptTimerManager.Instance().CheckPlayerTimeOut(time_id, play.GetBaseAttr().player_id);
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000311EC File Offset: 0x0002F3EC
		private void Action_TimeOut_Delete(ActionInfo info, PlayerObject play)
		{
			int time_id = Convert.ToInt32(info.param);
			ScriptTimerManager.Instance().DeletePlayerTimeOut(time_id, play.GetBaseAttr().player_id);
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00031220 File Offset: 0x0002F420
		private bool Action_Magic_Operation(ActionInfo info, PlayerObject play)
		{
			bool result = true;
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			string text = array[0];
			string text2 = text.ToLower();
			if (text2 != null)
			{
				if (text2 == "learnmagic")
				{
					byte b = Convert.ToByte(array[1]);
					uint num = Convert.ToUInt32(array[2]);
					byte level = Convert.ToByte(array[3]);
					uint exp = Convert.ToUInt32(array[4]);
					MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(num, 0);
					if (magicTypeInfo == null)
					{
						return false;
					}
					if (b != 0 && play.GetBaseAttr().profession != b)
					{
						result = false;
						play.LeftNotice("Profession mismatch, unable to learn skill");
					}
					else if (play.GetMagicSystem().isMagic(num))
					{
						result = false;
						play.LeftNotice("You Have Already Learned" + magicTypeInfo.name + ",Please Do Not Repeat Learning!");
					}
					else
					{
						play.GetMagicSystem().AddMagicInfo(num, level, exp);
						play.LeftNotice("Congratulations, You Have Learned" + magicTypeInfo.name);
					}
				}
			}
			return result;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00031350 File Offset: 0x0002F550
		private void Action_Random_Init(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			int min = Convert.ToInt32(array[0]);
			int max = Convert.ToInt32(array[1]);
			play.SetCurrentRandom(IRandom.Random(min, max));
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00031398 File Offset: 0x0002F598
		private bool Action_Random_Compare(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			int num = Convert.ToInt32(array[1]);
			int currentRandom = play.GetCurrentRandom();
			string text = array[0];
			if (text != null)
			{
				if (text == ">")
				{
					return num > currentRandom;
				}
				if (text == "=")
				{
					return num == currentRandom;
				}
				if (text == "<")
				{
					return num < currentRandom;
				}
				if (text == ">=")
				{
					return num >= currentRandom;
				}
				if (text == "<=")
				{
					return num <= currentRandom;
				}
			}
			return false;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00031458 File Offset: 0x0002F658
		public void Action_Set_Eudemon_Pro(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			int num = Convert.ToInt32(array[0]);
			uint eudemon_id = 0U;
			if (num == 0)
			{
				eudemon_id = play.GetUseItemEudemonId();
			}
			RoleData_Eudemon roleData_Eudemon = play.GetEudemonSystem().FindEudemon(eudemon_id);
			EudemonObject eudmeonObject = play.GetEudemonSystem().GetEudmeonObject(eudemon_id);
			if (roleData_Eudemon != null && eudmeonObject != null)
			{
				string text = array[1];
				string text2 = array[2];
				int num2 = Convert.ToInt32(array[3]);
				string text3 = text;
				if (text3 != null)
				{
					if (!(text3 == "quality"))
					{
						if (text3 == "wuxing")
						{
							text3 = text2;
							if (text3 != null)
							{
								if (text3 == "=")
								{
									roleData_Eudemon.wuxing = num2;
								}
							}
						}
					}
					else
					{
						text3 = text2;
						if (text3 != null)
						{
							if (!(text3 == "+"))
							{
								if (!(text3 == "-"))
								{
									if (text3 == "=")
									{
										roleData_Eudemon.quality = num2;
									}
								}
								else
								{
									roleData_Eudemon.quality -= num2;
								}
							}
							else
							{
								roleData_Eudemon.quality += num2;
							}
						}
					}
				}
				if (roleData_Eudemon != null)
				{
					eudmeonObject.SetEudemonInfo(roleData_Eudemon);
					play.GetEudemonSystem().SendEudemonInfo(roleData_Eudemon, true, true);
				}
			}
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x000315C8 File Offset: 0x0002F7C8
		public bool Action_Get_Eudemon_Pro(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			int num = Convert.ToInt32(array[0]);
			uint eudemon_id = 0U;
			if (num == 0)
			{
				eudemon_id = play.GetUseItemEudemonId();
			}
			RoleData_Eudemon roleData_Eudemon = play.GetEudemonSystem().FindEudemon(eudemon_id);
			bool result;
			if (roleData_Eudemon == null)
			{
				result = false;
			}
			else
			{
				string text = array[1];
				string text2 = array[2];
				int num2 = Convert.ToInt32(array[3]);
				string text3 = text;
				if (text3 != null)
				{
					if (text3 == "quality")
					{
						text3 = text2;
						if (text3 != null)
						{
							if (text3 == ">")
							{
								return roleData_Eudemon.quality > num2;
							}
							if (text3 == ">=")
							{
								return roleData_Eudemon.quality >= num2;
							}
							if (text3 == "=")
							{
								return roleData_Eudemon.quality == num2;
							}
							if (text3 == "<")
							{
								return roleData_Eudemon.quality < num2;
							}
							if (text3 == "<=")
							{
								return roleData_Eudemon.quality <= num2;
							}
						}
						return false;
					}
					if (text3 == "wuxing")
					{
						text3 = text2;
						if (text3 != null)
						{
							if (text3 == "=")
							{
								return roleData_Eudemon.wuxing == num2;
							}
							if (text3 == "!=")
							{
								return roleData_Eudemon.wuxing != num2;
							}
						}
						return false;
					}
					if (text3 == "level")
					{
						text3 = text2;
						if (text3 != null)
						{
							if (text3 == ">")
							{
								return (int)roleData_Eudemon.level > num2;
							}
							if (text3 == ">=")
							{
								return (int)roleData_Eudemon.level >= num2;
							}
							if (text3 == "=")
							{
								return (int)roleData_Eudemon.level == num2;
							}
							if (text3 == "<")
							{
								return (int)roleData_Eudemon.level < num2;
							}
							if (text3 == "<=")
							{
								return (int)roleData_Eudemon.level <= num2;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0003184C File Offset: 0x0002FA4C
		private void Action_Recall_Eudemon(ActionInfo info, PlayerObject play)
		{
			switch (Convert.ToInt32(info.param))
			{
			case 0:
				play.GetEudemonSystem().Eudemon_ReCallAll(false);
				play.GetEudemonSystem().Eudemon_BreakUpAll();
				break;
			case 1:
				play.GetEudemonSystem().Eudemon_ReCallAll(false);
				break;
			case 2:
				play.GetEudemonSystem().Eudemon_BreakUpAll();
				break;
			}
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000318B8 File Offset: 0x0002FAB8
		private void Action_Fuck_Nian(ActionInfo info, PlayerObject play)
		{
			int num = Convert.ToInt32(info.param);
			int[,] array = null;
			switch (num)
			{
			case 11:
				array = new int[5, 5];
				array[0, 0] = 180000;
				array[0, 1] = 1;
				array[1, 0] = 743388;
				array[1, 1] = 9;
				array[2, 0] = 743382;
				array[2, 1] = 90;
				array[3, 0] = 743381;
				array[3, 1] = 250;
				array[4, 0] = 743380;
				array[4, 1] = 650;
				break;
			case 12:
				array = new int[5, 5];
				array[0, 0] = 180020;
				array[0, 1] = 1;
				array[1, 0] = 743492;
				array[1, 1] = 9;
				array[2, 0] = 743385;
				array[2, 1] = 90;
				array[3, 0] = 743384;
				array[3, 1] = 250;
				array[4, 0] = 743383;
				array[4, 1] = 650;
				break;
			case 13:
				array = new int[6, 6];
				array[0, 0] = 180040;
				array[0, 1] = 1;
				array[1, 0] = 743495;
				array[1, 1] = 9;
				array[2, 0] = 743389;
				array[2, 1] = 90;
				array[3, 0] = 743386;
				array[3, 1] = 100;
				array[4, 0] = 743385;
				array[4, 1] = 150;
				array[5, 0] = 743384;
				array[5, 1] = 650;
				break;
			case 14:
				array = new int[5, 5];
				array[0, 0] = 180060;
				array[0, 1] = 1;
				array[1, 0] = 743497;
				array[1, 1] = 9;
				array[2, 0] = 743389;
				array[2, 1] = 90;
				array[3, 0] = 743386;
				array[3, 1] = 250;
				array[4, 0] = 743385;
				array[4, 1] = 650;
				break;
			case 15:
				array = new int[5, 5];
				array[0, 0] = 180080;
				array[0, 1] = 1;
				array[1, 0] = 743500;
				array[1, 1] = 9;
				array[2, 0] = 743491;
				array[2, 1] = 90;
				array[3, 0] = 743390;
				array[3, 1] = 250;
				array[4, 0] = 743387;
				array[4, 1] = 650;
				break;
			case 16:
				array = new int[5, 5];
				array[0, 0] = 180100;
				array[0, 1] = 1;
				array[1, 0] = 743501;
				array[1, 1] = 9;
				array[2, 0] = 743493;
				array[2, 1] = 90;
				array[3, 0] = 743491;
				array[3, 1] = 250;
				array[4, 0] = 743390;
				array[4, 1] = 650;
				break;
			case 17:
				array = new int[5, 5];
				array[0, 0] = 180120;
				array[0, 1] = 1;
				array[1, 0] = 743502;
				array[1, 1] = 9;
				array[2, 0] = 743496;
				array[2, 1] = 90;
				array[3, 0] = 743493;
				array[3, 1] = 250;
				array[4, 0] = 743491;
				array[4, 1] = 650;
				break;
			case 18:
				array = new int[5, 5];
				array[0, 0] = 180140;
				array[0, 1] = 1;
				array[1, 0] = 743503;
				array[1, 1] = 9;
				array[2, 0] = 743499;
				array[2, 1] = 90;
				array[3, 0] = 743497;
				array[3, 1] = 250;
				array[4, 0] = 743494;
				array[4, 1] = 650;
				break;
			}
			play.ChangeMap(1000U, 296, 520);
			if (array != null)
			{
				int num2 = IRandom.Random(1, 1000);
				for (int i = 0; i < 10; i++)
				{
					num2 = IRandom.Random(1, 1000);
				}
				int num3 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					num3 += array[i, 1];
					if (num2 <= num3)
					{
						ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo((uint)array[i, 0]);
						if (itemTypeInfo != null)
						{
							play.GetItemSystem().AwardItem((uint)array[i, 0], 50, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, true);
							play.MsgBox("Little bitch, you got beaten out! ");
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000426 RID: 1062 RVA: 0x00031E28 File Offset: 0x00030028
		public bool Action_Fuben_Create(ActionInfo info, PlayerObject play)
		{
			string[] array = info.param.Split(new char[]
			{
				' '
			});
			uint mapid = Convert.ToUInt32(array[0]);
			byte b = Convert.ToByte(array[1]);
			short x = Convert.ToInt16(array[2]);
			short y = Convert.ToInt16(array[3]);
			GameMap gameMap = MapManager.Instance().AddFubenMap(mapid);
			bool result;
			if (gameMap == null)
			{
				result = false;
			}
			else
			{
				if (b == 1)
				{
					play.ChangeFubenMap(gameMap, x, y);
				}
				else if (b == 2)
				{
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00031ECC File Offset: 0x000300CC
		public string Sprintf_string(string text, PlayerObject play)
		{
			string text2 = text;
			bool flag = false;
			for (;;)
			{
				int num = text2.IndexOf('[');
				if (num == -1)
				{
					break;
				}
				int num2 = text2.IndexOf(']');
				if (num2 == -1)
				{
					break;
				}
				string text3 = text2.Substring(num + 1, num2 - num - 1);
				string[] array = text3.Split(new char[]
				{
					','
				});
				string oldValue = text2.Substring(num, num2 - num + 1);
				string newValue = "";
				string text4 = array[0];
				if (text4 == null)
				{
					goto IL_171;
				}
				if (!(text4 == "username"))
				{
					if (!(text4 == "itemname"))
					{
						if (!(text4 == "timeout"))
						{
							goto IL_171;
						}
						int time_id = Convert.ToInt32(array[1]);
						newValue = ScriptTimerManager.Instance().GetPlayerTimeOutS(time_id, play.GetBaseAttr().player_id).ToString() + "Second";
						text2 = text2.Replace(oldValue, newValue);
					}
					else
					{
						RoleItemInfo roleItemInfo = play.GetItemSystem().FindItem(play.GetItemSystem().GetScriptItemId());
						if (roleItemInfo != null)
						{
							ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(roleItemInfo.itemid);
							if (itemTypeInfo != null)
							{
								newValue = itemTypeInfo.name;
							}
						}
						text2 = text2.Replace(oldValue, newValue);
					}
				}
				else
				{
					text2 = text2.Replace(text3, play.GetName());
				}
				IL_176:
				if (flag)
				{
					break;
				}
				continue;
				IL_171:
				flag = true;
				goto IL_176;
			}
			return text2;
		}

		// Token: 0x04000668 RID: 1640
		private static ScripteManager m_Instance = null;

		// Token: 0x04000669 RID: 1641
		private Dictionary<uint, ActionInfo> mDicScripte;

		// Token: 0x0400066A RID: 1642
		private byte mnSelectIndex;

		// Token: 0x0400066B RID: 1643
		private bool mbEndTag;

		// Token: 0x0400066C RID: 1644
		private string mszStr;
	}
}
