using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network.Internal;
using MySql.Data.MySqlClient;

namespace DBServer
{
	// Token: 0x02000008 RID: 8
	public class Data
	{
		// Token: 0x06000036 RID: 54 RVA: 0x000034A8 File Offset: 0x000016A8
		public static int GetAccountId(string sAcc)
		{
			MySqlCommand mySqlCommand = new MySqlCommand("select * from account where account = '" + sAcc + "'", MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			int result = -1;
			if (mySqlDataReader.Read())
			{
				if (mySqlDataReader.HasRows)
				{
					result = mySqlDataReader.GetInt32("id");
				}
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			return result;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003520 File Offset: 0x00001720
		public static bool IsOnline(string sAcc, ref int mapserverindex)
		{
			MySqlCommand mySqlCommand = new MySqlCommand("select * from account where account = '" + sAcc + "'", MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			bool flag = true;
			if (mySqlDataReader.Read())
			{
				if (mySqlDataReader.HasRows)
				{
					flag = (mySqlDataReader.GetInt32("serverindex") != -1);
					if (flag)
					{
						mapserverindex = mySqlDataReader.GetInt32("serverindex");
					}
				}
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			return flag;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000035B8 File Offset: 0x000017B8
		public static void SetOnlineState(int accountid, int mapserverindex)
		{
			string cmdText = string.Format("update account set serverindex ={0} where id={1}", mapserverindex, accountid);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			mySqlCommand.ExecuteNonQuery();
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		public static void ClearStaleOnlineStates()
		{
			MySqlCommand mySqlCommand = new MySqlCommand(
				"update account set serverindex = -1 where serverindex <> -1",
				MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			int affectedRows = mySqlCommand.ExecuteNonQuery();
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			Log.Instance().WriteLog("Cleared stale online state for " +
				affectedRows.ToString() + " account(s).");
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003604 File Offset: 0x00001804
		public static int QueryAccount(string sAcc)
		{
			int accountId = Data.GetAccountId(sAcc);
			if (accountId == -1 && Global.mbTestMode && sAcc.Length > 0)
			{
				string cmdText = string.Format("insert into account(account,password,vip) values('{0}','{1}',{2})", sAcc, "123456", 1);
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
				accountId = Data.GetAccountId(sAcc);
			}
			return accountId;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003688 File Offset: 0x00001888
		private static string DBStringToNormal(string dbStr)
		{
			byte[] array = new byte[dbStr.Length];
			for (int i = 0; i < dbStr.Length; i++)
			{
				array[i] = (byte)dbStr[i];
			}
			return Coding.GetDefauleCoding().GetString(array, 0, array.Length);
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000036D8 File Offset: 0x000018D8
		public static RoleInfo QueryRoleInfo(int accountid)
		{
			RoleInfo roleInfo = new RoleInfo(null);
			roleInfo.isRole = false;
			roleInfo.accountid = accountid;
			MySqlCommand mySqlCommand = new MySqlCommand(
				"select u.*, a.vip as account_vip from cq_user u " +
				"inner join account a on a.id = u.accountid " +
				"where u.accountid = @accountid",
				MysqlConn.GetConn());
			mySqlCommand.Parameters.AddWithValue("@accountid", accountid);
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				roleInfo.isRole = true;
				roleInfo.playerid = mySqlDataReader.GetInt32("id");
				roleInfo.name = mySqlDataReader.GetString("name");
				roleInfo.name = Coding.Latin1ToGB2312(roleInfo.name);
				roleInfo.lookface = mySqlDataReader.GetUInt32("lookface");
				roleInfo.hair = mySqlDataReader.GetUInt32("hair");
				roleInfo.lv = mySqlDataReader.GetByte("level");
				roleInfo.exp = mySqlDataReader.GetUInt32("exp");
				roleInfo.life = mySqlDataReader.GetUInt32("life");
				roleInfo.mana = mySqlDataReader.GetUInt32("mana");
				roleInfo.profession = mySqlDataReader.GetByte("profession");
				roleInfo.pk = mySqlDataReader.GetInt16("pk");
				roleInfo.gold = mySqlDataReader.GetInt32("gold");
				roleInfo.gamegold = mySqlDataReader.GetInt32("gamegold");
				roleInfo.stronggold = mySqlDataReader.GetInt32("stronggold");
				roleInfo.mapid = mySqlDataReader.GetInt32("mapid");
				roleInfo.x = mySqlDataReader.GetInt16("record_x");
				roleInfo.y = mySqlDataReader.GetInt16("record_y");
				roleInfo.hotkey = mySqlDataReader.GetString("hotkey");
				roleInfo.guanjue = mySqlDataReader.GetUInt64("guanjue");
				roleInfo.godlevel = (int)mySqlDataReader.GetByte("godlevel");
				roleInfo.maxeudemon = mySqlDataReader.GetByte("maxeudemon");
				roleInfo.vip = (byte)Math.Max(0, Math.Min(6,
					mySqlDataReader.GetInt32("account_vip")));
				roleInfo.wardrobeHairs = ParseWardrobeHairs(
					mySqlDataReader.GetString("wardrobe_hairs"));
				roleInfo.wardrobeAvatars = ParseWardrobeAvatars(
					mySqlDataReader.GetString("wardrobe_avatars"));
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			return roleInfo;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x000038BC File Offset: 0x00001ABC
		public static QueryRoleName_Ret QueryRoleName(string name)
		{
			QueryRoleName_Ret queryRoleName_Ret = new QueryRoleName_Ret();
			bool flag = false;
			if (Filter.Instance().CheckFileterName(name))
			{
				flag = true;
			}
			if (!flag)
			{
				MySqlCommand mySqlCommand = new MySqlCommand("select * from cq_user where name = '" + name + "'", MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				mySqlDataReader.Read();
				if (mySqlDataReader.HasRows)
				{
					flag = true;
				}
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
			queryRoleName_Ret.tag = flag;
			return queryRoleName_Ret;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003950 File Offset: 0x00001B50
		public static bool CreateRole(int accountid, string name, uint lookface, byte professin, ref int playerid)
		{
			try
			{
				string text = string.Format("insert into cq_user(accountid,name,lookface,profession,level) values({0},'{1}',{2},{3},1)", new object[]
				{
					accountid.ToString(),
					name,
					lookface.ToString(),
					professin.ToString()
				});
				string cmdText = text;
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
				string cmdText2 = "select max(id) from cq_user";
				mySqlCommand = new MySqlCommand(cmdText2, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				mySqlDataReader.Read();
				if (mySqlDataReader.HasRows)
				{
					playerid = mySqlDataReader.GetInt32(0);
				}
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("createrole error!");
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				return false;
			}
			return true;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003A6C File Offset: 0x00001C6C
		public static bool SaveRoleData_Attr(SaveRoleData_Attr info)
		{
			if (info.gamegold < 0L)
			{
				info.gamegold = 0L;
			}
			if (info.gold < 0L)
			{
				info.gold = 0L;
			}
			MySqlCommand mySqlCommand = null;
			string text = Coding.GB2312ToLatin1(info.name);
			string text2 = "";
			try
			{
				text2 = string.Format("update cq_user set name='{0}',lookface={1},hair={2},level={3},exp={4},life={5},mana={6},profession={7},pk={8},gold={9},gamegold={10},stronggold={11},mapid={12},record_x={13},record_y={14},hotkey='{15}',guanjue={16},godlevel={17},maxeudemon={18},wardrobe_hairs=@wardrobe_hairs,wardrobe_avatars=@wardrobe_avatars where accountid={19} ", new object[]
				{
					text,
					info.lookface,
					info.hair,
					info.level,
					info.exp,
					info.life,
					info.mana,
					info.profession,
					info.pk,
					info.gold,
					info.gamegold,
					info.stronggold,
					info.mapid,
					info.x,
					info.y,
					info.hotkey,
					info.guanjue,
					info.godlevel,
					info.maxeudemon,
					info.accountid
				});
				string cmdText = text2;
				mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				mySqlCommand.Parameters.AddWithValue(
					"@wardrobe_hairs",
					SerializeWardrobeHairs(info.wardrobeHairs));
				mySqlCommand.Parameters.AddWithValue(
					"@wardrobe_avatars",
					SerializeWardrobeAvatars(info.wardrobeAvatars));
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("SaveRoleData_Attr error!");
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				Log.Instance().WriteLog("SQL statement: " + text2);
				if (mySqlCommand != null)
				{
					mySqlCommand.Dispose();
				}
				return false;
			}
			return true;
		}

		private static List<uint> ParseWardrobeHairs(string value)
		{
			List<uint> styles = new List<uint>();
			if (string.IsNullOrWhiteSpace(value))
			{
				return styles;
			}

			HashSet<uint> seen = new HashSet<uint>();
			foreach (string part in value.Split(','))
			{
				uint styleId;
				if (uint.TryParse(part, out styleId) && styleId != 0U &&
					seen.Add(styleId))
				{
					styles.Add(styleId);
				}
			}
			styles.Sort();
			return styles;
		}

		private static string SerializeWardrobeHairs(IEnumerable<uint> styles)
		{
			if (styles == null)
			{
				return string.Empty;
			}

			List<uint> sorted = new List<uint>(styles);
			sorted.Sort();
			return string.Join(",", sorted);
		}

		private static List<uint> ParseWardrobeAvatars(string value)
		{
			List<uint> styles = new List<uint>();
			if (string.IsNullOrWhiteSpace(value))
			{
				return styles;
			}

			HashSet<uint> seen = new HashSet<uint>();
			foreach (string part in value.Split(','))
			{
				uint styleId;
				if (uint.TryParse(part, out styleId) && seen.Add(styleId))
				{
					styles.Add(styleId);
				}
			}
			styles.Sort();
			return styles;
		}

		private static string SerializeWardrobeAvatars(
			IEnumerable<uint> styles)
		{
			if (styles == null)
			{
				return string.Empty;
			}

			List<uint> sorted = new List<uint>(styles);
			sorted.Sort();
			return string.Join(",", sorted);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003CB8 File Offset: 0x00001EB8
		public static bool AddRoleData_Item(AddRoleData_Item info, ref uint nkey)
		{
			try
			{
				string forgeName = Coding.GB2312ToLatin1(
					info.item.forgename ?? string.Empty);
				string text = string.Format("insert into cq_item(playerid,itemid,postion,stronglv,gem1,gem2,forgename,amount,war_ghost_exp,di_attack,shui_attack,huo_attack,feng_attack,property,gem3,god_exp,god_strong) values({0},{1},{2},{3},{4},{5},@forgename,{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})", new object[]
				{
					info.item.playerid,
					info.item.itemid,
					info.item.postion,
					info.item.stronglv,
					info.item.gem1,
					info.item.gem2,
					info.item.forgename,
					info.item.amount,
					info.item.war_ghost_exp,
					info.item.di_attack,
					info.item.shui_attack,
					info.item.huo_attack,
					info.item.feng_attack,
					info.item.property,
					info.item.gem3,
					info.item.god_exp,
					info.item.god_strong
				});
				string cmdText = text;
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				mySqlCommand.Parameters.AddWithValue("@forgename", forgeName);
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
				string cmdText2 = "select max(id) from cq_item";
				mySqlCommand = new MySqlCommand(cmdText2, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				nkey = 0U;
				mySqlDataReader.Read();
				if (mySqlDataReader.HasRows)
				{
					nkey = mySqlDataReader.GetUInt32(0);
				}
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("createrole error!");
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				return false;
			}
			return true;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00003F30 File Offset: 0x00002130
		public static void LoadRoleData_Item(ROLEDATA_ITEM info)
		{
			string cmdText = string.Format("select * from cq_item where playerid={0}", info.playerid);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				if (!mySqlDataReader.HasRows)
				{
					break;
				}
				RoleData_Item roleData_Item = new RoleData_Item();
				roleData_Item.id = mySqlDataReader.GetUInt32("id");
				roleData_Item.playerid = mySqlDataReader.GetInt32("playerid");
				roleData_Item.itemid = mySqlDataReader.GetUInt32("itemid");
				roleData_Item.postion = mySqlDataReader.GetUInt16("postion");
				roleData_Item.stronglv = mySqlDataReader.GetByte("stronglv");
				roleData_Item.gem1 = (uint)mySqlDataReader.GetByte("gem1");
				roleData_Item.gem2 = (uint)mySqlDataReader.GetByte("gem2");
				roleData_Item.forgename = mySqlDataReader.GetString("forgename");
				if (roleData_Item.forgename.Length > 0)
				{
					roleData_Item.forgename = Coding.Latin1ToGB2312(roleData_Item.forgename);
				}
				roleData_Item.amount = mySqlDataReader.GetUInt16("amount");
				roleData_Item.war_ghost_exp = mySqlDataReader.GetInt32("war_ghost_exp");
				roleData_Item.di_attack = mySqlDataReader.GetByte("di_attack");
				roleData_Item.shui_attack = mySqlDataReader.GetByte("shui_attack");
				roleData_Item.huo_attack = mySqlDataReader.GetByte("huo_attack");
				roleData_Item.feng_attack = mySqlDataReader.GetByte("feng_attack");
				roleData_Item.property = mySqlDataReader.GetInt32("property");
				roleData_Item.gem3 = (uint)mySqlDataReader.GetByte("gem3");
				roleData_Item.god_exp = mySqlDataReader.GetInt32("god_exp");
				roleData_Item.god_strong = mySqlDataReader.GetInt32("god_strong");
				info.mListItem.Add(roleData_Item);
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004110 File Offset: 0x00002310
		public static void SaveRoleData_Item(ROLEDATA_ITEM info)
		{
			try
			{
				for (int i = 0; i < info.mListItem.Count; i++)
				{
					RoleData_Item roleData_Item = info.mListItem[i];
					string forgeName = Coding.GB2312ToLatin1(
						roleData_Item.forgename ?? string.Empty);
					string cmdText;
					if (roleData_Item.id == 0U)
					{
						cmdText = string.Format("insert into cq_item(playerid,itemid,postion,stronglv,gem1,gem2,forgename,amount,war_ghost_exp,di_attack,shui_attack,huo_attack,feng_attack,property,gem3,god_exp,god_strong) values({0},{1},{2},{3},{4},{5},@forgename,{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})", new object[]
						{
							info.playerid,
							roleData_Item.itemid,
							roleData_Item.postion,
							roleData_Item.stronglv,
							roleData_Item.gem1,
							roleData_Item.gem2,
							roleData_Item.forgename,
							roleData_Item.amount,
							roleData_Item.war_ghost_exp,
							roleData_Item.di_attack,
							roleData_Item.shui_attack,
							roleData_Item.huo_attack,
							roleData_Item.feng_attack,
							roleData_Item.property,
							roleData_Item.gem3,
							roleData_Item.god_exp,
							roleData_Item.god_strong
						});
					}
					else
					{
						cmdText = string.Format("update cq_item set itemid={0},postion={1},stronglv={2},gem1={3},gem2={4},forgename=@forgename,amount={6},war_ghost_exp={7},di_attack={8},shui_attack={9},huo_attack={10},feng_attack={11},property={12},gem3={13},god_exp={14},god_strong={15} where playerid={16} and id={17}", new object[]
						{
							roleData_Item.itemid,
							roleData_Item.postion,
							roleData_Item.stronglv,
							roleData_Item.gem1,
							roleData_Item.gem2,
							roleData_Item.forgename,
							roleData_Item.amount,
							roleData_Item.war_ghost_exp,
							roleData_Item.di_attack,
							roleData_Item.shui_attack,
							roleData_Item.huo_attack,
							roleData_Item.feng_attack,
							roleData_Item.property,
							roleData_Item.gem3,
							roleData_Item.god_exp,
							roleData_Item.god_strong,
							info.playerid,
							roleData_Item.id
						});
					}
					MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
					mySqlCommand.Parameters.AddWithValue("@forgename", forgeName);
					MysqlConn.Conn_Open();
					mySqlCommand.ExecuteNonQuery();
					MysqlConn.Conn_Close();
					mySqlCommand.Dispose();
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog("---------------------------------------------------------------------------");
				Log.Instance().WriteLog("Failed to save role item data. Role ID: " + info.playerid.ToString());
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				Log.Instance().WriteLog("---------------------------------------------------------------------------");
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004488 File Offset: 0x00002688
		public static void SaveRoleData_Magic(RoleData_Magic info)
		{
			for (int i = 0; i < info.mListMagic.Count; i++)
			{
				MagicInfo magicInfo = info.mListMagic[i];
				string cmdText;
				if (magicInfo.id == 0)
				{
					cmdText = string.Format("insert into cq_magic(ownerid,magicid,level,exp) values({0},{1},{2},{3})", new object[]
					{
						info.ownerid,
						magicInfo.magicid,
						magicInfo.level,
						magicInfo.exp
					});
				}
				else
				{
					cmdText = string.Format("update cq_magic set magicid={0},level={1},exp={2} where ownerid={3} and id={4}", new object[]
					{
						magicInfo.magicid,
						magicInfo.level,
						magicInfo.exp,
						info.ownerid,
						magicInfo.id
					});
				}
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000045B8 File Offset: 0x000027B8
		public static bool DeleteRoleData_Item(int playerid, uint id)
		{
			string cmdText = string.Format("delete from cq_item where playerid={0} and id ={1}", playerid, id);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			mySqlCommand.ExecuteNonQuery();
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			return true;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004608 File Offset: 0x00002808
		public static void LoadRoleData_Magic(RoleData_Magic info)
		{
			string cmdText = string.Format("select * from cq_magic where ownerid={0}", info.ownerid);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				if (!mySqlDataReader.HasRows)
				{
					break;
				}
				MagicInfo magicInfo = new MagicInfo();
				magicInfo.id = mySqlDataReader.GetInt32("id");
				magicInfo.magicid = mySqlDataReader.GetUInt32("magicid");
				magicInfo.level = mySqlDataReader.GetByte("level");
				magicInfo.exp = mySqlDataReader.GetUInt32("exp");
				info.mListMagic.Add(magicInfo);
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000046C8 File Offset: 0x000028C8
		public static void LoadRoleData_Eudemon_MagicInfo(ROLEDATE_EUDEMON info)
		{
			for (int i = 0; i < info.list_item.Count; i++)
			{
				RoleData_Eudemon roleData_Eudemon = info.list_item[i];
				string cmdText = string.Format("select * from cq_eudemon_magic where ownerid={0}", roleData_Eudemon.id);
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
				while (mySqlDataReader.Read())
				{
					if (!mySqlDataReader.HasRows)
					{
						break;
					}
					MagicInfo magicInfo = new MagicInfo();
					magicInfo.id = mySqlDataReader.GetInt32("id");
					magicInfo.ownerid = mySqlDataReader.GetInt32("ownerid");
					magicInfo.magicid = mySqlDataReader.GetUInt32("magicid");
					magicInfo.exp = mySqlDataReader.GetUInt32("level");
					roleData_Eudemon.mListMagicInfo.Add(magicInfo);
				}
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000047C8 File Offset: 0x000029C8
		public static void LoadRoleData_Eudemon(ROLEDATE_EUDEMON info)
		{
			string cmdText = string.Format("select * from cq_eudemon where ownerid ={0}", info.playerid);
			MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
			MysqlConn.Conn_Open();
			MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			while (mySqlDataReader.Read())
			{
				if (!mySqlDataReader.HasRows)
				{
					break;
				}
				RoleData_Eudemon roleData_Eudemon = new RoleData_Eudemon();
				roleData_Eudemon.id = mySqlDataReader.GetUInt32("id");
				roleData_Eudemon.itemid = mySqlDataReader.GetUInt32("itemid");
				roleData_Eudemon.name = mySqlDataReader.GetString("name");
				roleData_Eudemon.name = Coding.Latin1ToGB2312(roleData_Eudemon.name);
				roleData_Eudemon.phyatk_grow_rate = mySqlDataReader.GetFloat("phyatk_grow_rate");
				roleData_Eudemon.phyatk_grow_rate_max = mySqlDataReader.GetFloat("phyatk_grow_rate_max");
				roleData_Eudemon.magicatk_grow_rate = mySqlDataReader.GetFloat("magicatk_grow_rate");
				roleData_Eudemon.magicatk_grow_rate_max = mySqlDataReader.GetFloat("magicatk_grow_rate_max");
				roleData_Eudemon.life_grow_rate = mySqlDataReader.GetFloat("life_grow_rate");
				roleData_Eudemon.defense_grow_rate = mySqlDataReader.GetFloat("defense_grow_rate");
				roleData_Eudemon.magicdef_grow_rate = mySqlDataReader.GetFloat("magicdef_grow_rate");
				roleData_Eudemon.init_life = mySqlDataReader.GetInt32("life");
				roleData_Eudemon.init_atk_min = mySqlDataReader.GetInt32("atk_min");
				roleData_Eudemon.init_atk_max = mySqlDataReader.GetInt32("atk_max");
				roleData_Eudemon.init_magicatk_min = mySqlDataReader.GetInt32("magicatk_min");
				roleData_Eudemon.init_magicatk_max = mySqlDataReader.GetInt32("magicatk_max");
				roleData_Eudemon.init_defense = mySqlDataReader.GetInt32("defense");
				roleData_Eudemon.init_magicdef = mySqlDataReader.GetInt32("magicdef");
				roleData_Eudemon.luck = mySqlDataReader.GetInt32("luck");
				roleData_Eudemon.intimacy = mySqlDataReader.GetInt32("intimacy");
				roleData_Eudemon.level = mySqlDataReader.GetInt16("level");
				roleData_Eudemon.card = mySqlDataReader.GetInt32("card");
				roleData_Eudemon.exp = mySqlDataReader.GetInt32("exp");
				roleData_Eudemon.quality = mySqlDataReader.GetInt32("quality");
				roleData_Eudemon.wuxing = mySqlDataReader.GetInt32("wuxing");
				roleData_Eudemon.recall_count = mySqlDataReader.GetInt32("recall_count");
				info.list_item.Add(roleData_Eudemon);
			}
			MysqlConn.Conn_Close();
			mySqlCommand.Dispose();
			Data.LoadRoleData_Eudemon_MagicInfo(info);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004A10 File Offset: 0x00002C10
		public static void SaveRoleData_Eudemon_MagicInfo(ROLEDATE_EUDEMON info)
		{
			for (int i = 0; i < info.list_item.Count; i++)
			{
				RoleData_Eudemon roleData_Eudemon = info.list_item[i];
				for (int j = 0; j < roleData_Eudemon.mListMagicInfo.Count; j++)
				{
					MagicInfo magicInfo = roleData_Eudemon.mListMagicInfo[j];
					string cmdText;
					if (magicInfo.id == 0)
					{
						MySqlCommand findExisting = new MySqlCommand(
							"select id from cq_eudemon_magic " +
							"where ownerid=@ownerid and magicid=@magicid " +
							"order by id limit 1",
							MysqlConn.GetConn());
						findExisting.Parameters.AddWithValue(
							"@ownerid", info.list_item[i].id);
						findExisting.Parameters.AddWithValue(
							"@magicid", magicInfo.magicid);
						MysqlConn.Conn_Open();
						object existingId = findExisting.ExecuteScalar();
						MysqlConn.Conn_Close();
						findExisting.Dispose();
						if (existingId != null && existingId != DBNull.Value)
						{
							MySqlCommand updateExisting = new MySqlCommand(
								"update cq_eudemon_magic set level=@level,exp=@exp " +
								"where id=@id and ownerid=@ownerid",
								MysqlConn.GetConn());
							updateExisting.Parameters.AddWithValue(
								"@level", magicInfo.level);
							updateExisting.Parameters.AddWithValue(
								"@exp", magicInfo.exp);
							updateExisting.Parameters.AddWithValue(
								"@id", Convert.ToUInt32(existingId));
							updateExisting.Parameters.AddWithValue(
								"@ownerid", info.list_item[i].id);
							MysqlConn.Conn_Open();
							updateExisting.ExecuteNonQuery();
							MysqlConn.Conn_Close();
							updateExisting.Dispose();
							continue;
						}
						cmdText = string.Format(
							"insert into cq_eudemon_magic" +
							"(ownerid,magicid,level,exp) values({0},{1},{2},{3})",
							new object[]
							{
								info.list_item[i].id,
								magicInfo.magicid,
								magicInfo.level,
								magicInfo.exp
							});
					}
					else if (magicInfo.id == -1)
					{
						cmdText = string.Format(
							"delete from cq_eudemon_magic " +
							"where ownerid={0} and magicid={1}",
							info.list_item[i].id,
							magicInfo.magicid);
					}
					else
					{
						cmdText = string.Format("update cq_eudemon_magic set magicid={0},level={1},exp={2} where ownerid={3} and id={4}", new object[]
						{
							magicInfo.magicid,
							magicInfo.level,
							magicInfo.exp,
							magicInfo.ownerid,
							magicInfo.id
						});
					}
					MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
					MysqlConn.Conn_Open();
					mySqlCommand.ExecuteNonQuery();
					MysqlConn.Conn_Close();
					mySqlCommand.Dispose();
				}
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004BBC File Offset: 0x00002DBC
		public static void SaveRoleData_Eudemon(ROLEDATE_EUDEMON info)
		{
			for (int i = 0; i < info.list_item.Count; i++)
			{
				RoleData_Eudemon roleData_Eudemon = info.list_item[i];
				string databaseName = Coding.GB2312ToLatin1(
					roleData_Eudemon.name ?? string.Empty);
				if (roleData_Eudemon.id == 0U)
				{
					roleData_Eudemon.id = Data.FindEudemonDatabaseId(
						info.playerid,
						roleData_Eudemon.itemid);
				}
				bool isNewEudemon = roleData_Eudemon.id == 0U;
				string cmdText;
				if (isNewEudemon)
				{
					cmdText = string.Format("insert into cq_eudemon(itemid,ownerid,name,phyatk_grow_rate,phyatk_grow_rate_max,magicatk_grow_rate,magicatk_grow_rate_max,life_grow_rate,defense_grow_rate,magicdef_grow_rate,life,atk_min,atk_max,magicatk_min,magicatk_max,defense,magicdef,luck,intimacy,level,card,exp,quality,wuxing,recall_count) values({0},{1},'{2}',{3},{4},{5},{6},'{7}',{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24})", new object[]
					{
						roleData_Eudemon.itemid,
						info.playerid,
						databaseName,
						roleData_Eudemon.phyatk_grow_rate,
						roleData_Eudemon.phyatk_grow_rate_max,
						roleData_Eudemon.magicatk_grow_rate,
						roleData_Eudemon.magicatk_grow_rate_max,
						roleData_Eudemon.life_grow_rate,
						roleData_Eudemon.defense_grow_rate,
						roleData_Eudemon.magicdef_grow_rate,
						roleData_Eudemon.init_life,
						roleData_Eudemon.init_atk_min,
						roleData_Eudemon.init_atk_max,
						roleData_Eudemon.init_magicatk_min,
						roleData_Eudemon.init_magicatk_max,
						roleData_Eudemon.init_defense,
						roleData_Eudemon.init_magicdef,
						roleData_Eudemon.luck,
						roleData_Eudemon.intimacy,
						roleData_Eudemon.level,
						roleData_Eudemon.card,
						roleData_Eudemon.exp,
						roleData_Eudemon.quality,
						roleData_Eudemon.wuxing,
						roleData_Eudemon.recall_count
					});
				}
				else
				{
					cmdText = string.Format("update cq_eudemon set itemid={0},ownerid={1},name='{2}',phyatk_grow_rate={3},phyatk_grow_rate_max={4},magicatk_grow_rate={5},magicatk_grow_rate_max={6},life_grow_rate={7},defense_grow_rate={8},magicdef_grow_rate={9},life={10},atk_min={11},atk_max={12},magicatk_min={13},magicatk_max={14},defense={15},magicdef={16},luck={17},intimacy={18},level={19},card={20},exp={21},quality={22},wuxing={23},recall_count={24} where id={25}", new object[]
					{
						roleData_Eudemon.itemid,
						info.playerid,
						databaseName,
						roleData_Eudemon.phyatk_grow_rate,
						roleData_Eudemon.phyatk_grow_rate_max,
						roleData_Eudemon.magicatk_grow_rate,
						roleData_Eudemon.magicatk_grow_rate_max,
						roleData_Eudemon.life_grow_rate,
						roleData_Eudemon.defense_grow_rate,
						roleData_Eudemon.magicdef_grow_rate,
						roleData_Eudemon.init_life,
						roleData_Eudemon.init_atk_min,
						roleData_Eudemon.init_atk_max,
						roleData_Eudemon.init_magicatk_min,
						roleData_Eudemon.init_magicatk_max,
						roleData_Eudemon.init_defense,
						roleData_Eudemon.init_magicdef,
						roleData_Eudemon.luck,
						roleData_Eudemon.intimacy,
						roleData_Eudemon.level,
						roleData_Eudemon.card,
						roleData_Eudemon.exp,
						roleData_Eudemon.quality,
						roleData_Eudemon.wuxing,
						roleData_Eudemon.recall_count,
						roleData_Eudemon.id
					});
				}
				MySqlCommand mySqlCommand = new MySqlCommand(cmdText, MysqlConn.GetConn());
				MysqlConn.Conn_Open();
				mySqlCommand.ExecuteNonQuery();
				if (isNewEudemon)
				{
					roleData_Eudemon.id = checked((uint)mySqlCommand.LastInsertedId);
				}
				MysqlConn.Conn_Close();
				mySqlCommand.Dispose();
			}
			Data.SaveRoleData_Eudemon_MagicInfo(info);
		}

		private static uint FindEudemonDatabaseId(int ownerId, uint itemId)
		{
			const string commandText =
				"select id from cq_eudemon " +
				"where ownerid=@ownerid and itemid=@itemid " +
				"order by id limit 1";
			MySqlCommand command = new MySqlCommand(
				commandText,
				MysqlConn.GetConn());
			command.Parameters.AddWithValue("@ownerid", ownerId);
			command.Parameters.AddWithValue("@itemid", itemId);
			MysqlConn.Conn_Open();
			object value = command.ExecuteScalar();
			MysqlConn.Conn_Close();
			command.Dispose();
			if (value == null || value == DBNull.Value)
			{
				return 0U;
			}
			return Convert.ToUInt32(value);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000501C File Offset: 0x0000321C
		public static bool DeleteDroppedEudemonItem(
			int playerid,
			uint itemId)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				uint eudemonId = 0U;
				using (MySqlCommand find = new MySqlCommand(
					"select id from cq_eudemon " +
					"where ownerid=@ownerid and itemid=@itemid " +
					"order by id limit 1 for update",
					connection,
					transaction))
				{
					find.Parameters.AddWithValue("@ownerid", playerid);
					find.Parameters.AddWithValue("@itemid", itemId);
					object value = find.ExecuteScalar();
					if (value != null && value != DBNull.Value)
					{
						eudemonId = Convert.ToUInt32(value);
					}
				}

				if (eudemonId != 0U)
				{
					using (MySqlCommand deleteMagic = new MySqlCommand(
						"delete from cq_eudemon_magic where ownerid=@ownerid",
						connection,
						transaction))
					{
						deleteMagic.Parameters.AddWithValue(
							"@ownerid", eudemonId);
						deleteMagic.ExecuteNonQuery();
					}
					using (MySqlCommand deleteEudemon = new MySqlCommand(
						"delete from cq_eudemon " +
						"where id=@id and ownerid=@ownerid and itemid=@itemid",
						connection,
						transaction))
					{
						deleteEudemon.Parameters.AddWithValue("@id", eudemonId);
						deleteEudemon.Parameters.AddWithValue(
							"@ownerid", playerid);
						deleteEudemon.Parameters.AddWithValue("@itemid", itemId);
						deleteEudemon.ExecuteNonQuery();
					}
				}

				int deletedItems;
				using (MySqlCommand deleteItem = new MySqlCommand(
					"delete from cq_item where playerid=@playerid and id=@id",
					connection,
					transaction))
				{
					deleteItem.Parameters.AddWithValue("@playerid", playerid);
					deleteItem.Parameters.AddWithValue("@id", itemId);
					deletedItems = deleteItem.ExecuteNonQuery();
				}
				transaction.Commit();
				return deletedItems == 1;
			}
			catch
			{
				if (transaction != null)
				{
					try
					{
						transaction.Rollback();
					}
					catch
					{
					}
				}
				throw;
			}
			finally
			{
				if (transaction != null)
				{
					transaction.Dispose();
				}
				MysqlConn.Conn_Close();
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000506C File Offset: 0x0000326C
		public static void LoadRoleData_Friend(ROLEDATA_FRIEND info)
		{
			const string sql =
				"SELECT id,friendtype,friendid,friendname " +
				"FROM cq_friend WHERE userid=@userid ORDER BY id DESC";
			MySqlCommand command = new MySqlCommand(sql, MysqlConn.GetConn());
			command.Parameters.AddWithValue("@userid", info.playerid);
			HashSet<ulong> loaded = new HashSet<ulong>();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						byte relationType = reader.GetByte("friendtype");
						uint friendId = reader.GetUInt32("friendid");
						ulong key = ((ulong)relationType << 32) | friendId;
						if (!loaded.Add(key))
						{
							continue;
						}
						RoleData_Friend relation = new RoleData_Friend();
						relation.id = reader.GetInt32("id");
						relation.friendid = friendId;
						relation.friendname = Coding.Latin1ToGB2312(
							reader.GetString("friendname"));
						relation.friendtype = relationType;
						info.list_item.Add(relation);
					}
				}
				info.list_item.Reverse();
			}
			finally
			{
				MysqlConn.Conn_Close();
				command.Dispose();
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005140 File Offset: 0x00003340
		public static void SaveRoleData_Friend(ROLEDATA_FRIEND info)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				for (int i = 0; i < info.list_item.Count; i++)
				{
					RoleData_Friend relation = info.list_item[i];
					if (relation == null || relation.friendid == 0U)
					{
						continue;
					}
					if (relation.id == -1)
					{
						string deleteSql = relation.friendtype == 15 ?
							"DELETE FROM cq_friend WHERE friendtype=@type AND " +
							"((userid=@userid AND friendid=@friendid) OR " +
							"(userid=@friendid AND friendid=@userid))" :
							"DELETE FROM cq_friend WHERE userid=@userid AND " +
							"friendtype=@type AND friendid=@friendid";
						using (MySqlCommand delete = new MySqlCommand(
							deleteSql, connection, transaction))
						{
							delete.Parameters.AddWithValue("@userid", info.playerid);
							delete.Parameters.AddWithValue("@type", relation.friendtype);
							delete.Parameters.AddWithValue("@friendid", relation.friendid);
							delete.ExecuteNonQuery();
						}
						continue;
					}

					using (MySqlCommand delete = new MySqlCommand(
						"DELETE FROM cq_friend WHERE userid=@userid AND " +
						"friendtype=@type AND friendid=@friendid",
						connection, transaction))
					{
						delete.Parameters.AddWithValue("@userid", info.playerid);
						delete.Parameters.AddWithValue("@type", relation.friendtype);
						delete.Parameters.AddWithValue("@friendid", relation.friendid);
						delete.ExecuteNonQuery();
					}
					using (MySqlCommand insert = new MySqlCommand(
						"INSERT INTO cq_friend " +
						"(userid,friendtype,friendid,friendname) " +
						"VALUES (@userid,@type,@friendid,@friendname)",
						connection, transaction))
					{
						insert.Parameters.AddWithValue("@userid", info.playerid);
						insert.Parameters.AddWithValue("@type", relation.friendtype);
						insert.Parameters.AddWithValue("@friendid", relation.friendid);
						insert.Parameters.AddWithValue(
							"@friendname",
							Coding.GB2312ToLatin1(relation.friendname ?? ""));
						insert.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
			catch
			{
				if (transaction != null)
				{
					try
					{
						transaction.Rollback();
					}
					catch
					{
					}
				}
				throw;
			}
			finally
			{
				if (transaction != null)
				{
					transaction.Dispose();
				}
				MysqlConn.Conn_Close();
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000052B4 File Offset: 0x000034B4
		public static int CreateLegion(LegionInfo info)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"INSERT INTO cq_legion " +
					"(name,member_title,leader_id,leader_name,money,notice) " +
					"VALUES (@name,@title,@leader_id,@leader_name,@money,@notice)",
					connection))
				{
					command.Parameters.AddWithValue(
						"@name", Coding.GB2312ToLatin1(info.name ?? ""));
					command.Parameters.AddWithValue("@title", info.title);
					command.Parameters.AddWithValue(
						"@leader_id", info.leader_id);
					command.Parameters.AddWithValue(
						"@leader_name",
						Coding.GB2312ToLatin1(info.leader_name ?? ""));
					command.Parameters.AddWithValue("@money", info.money);
					command.Parameters.AddWithValue(
						"@notice",
						Coding.GB2312ToLatin1(info.notice ?? ""));
					command.ExecuteNonQuery();
					return checked((int)command.LastInsertedId);
				}
			}
			finally
			{
				MysqlConn.Conn_Close();
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000053DC File Offset: 0x000035DC
		public static void UpdateLegion(LegionInfo info)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"UPDATE cq_legion SET name=@name,member_title=@title," +
					"leader_id=@leader_id,leader_name=@leader_name," +
					"money=@money,notice=@notice WHERE id=@id",
					connection))
				{
					command.Parameters.AddWithValue(
						"@name", Coding.GB2312ToLatin1(info.name ?? ""));
					command.Parameters.AddWithValue("@title", info.title);
					command.Parameters.AddWithValue(
						"@leader_id", info.leader_id);
					command.Parameters.AddWithValue(
						"@leader_name",
						Coding.GB2312ToLatin1(info.leader_name ?? ""));
					command.Parameters.AddWithValue("@money", info.money);
					command.Parameters.AddWithValue(
						"@notice",
						Coding.GB2312ToLatin1(info.notice ?? ""));
					command.Parameters.AddWithValue("@id", info.id);
					command.ExecuteNonQuery();
				}
			}
			finally
			{
				MysqlConn.Conn_Close();
			}
		}

		public static void SyncLegionMembers(
			uint legionId,
			System.Collections.Generic.IList<LegionMember> members)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				System.Collections.Generic.List<uint> retainedIds =
					new System.Collections.Generic.List<uint>();
				for (int index = 0; index < members.Count; index++)
				{
					LegionMember member = members[index];
					if (member.player_id <= 0)
					{
						throw new InvalidOperationException(
							"Legion member " + member.members_name +
							" has no player ID.");
					}
					using (MySqlCommand command = new MySqlCommand(
						"INSERT INTO cq_legion_members " +
						"(legion_id,player_id,members_name,money,emoney,`rank`) " +
						"VALUES (@legion_id,@player_id,@name,@money,@emoney,@rank) " +
						"AS incoming " +
						"ON DUPLICATE KEY UPDATE " +
						"id=LAST_INSERT_ID(id),legion_id=incoming.legion_id," +
						"members_name=incoming.members_name,money=incoming.money," +
						"emoney=incoming.emoney,`rank`=incoming.`rank`",
						connection,
						transaction))
					{
						command.Parameters.AddWithValue(
							"@legion_id", legionId);
						command.Parameters.AddWithValue(
							"@player_id", member.player_id);
						command.Parameters.AddWithValue(
							"@name",
							Coding.GB2312ToLatin1(
								member.members_name ?? ""));
						command.Parameters.AddWithValue(
							"@money", member.money);
						command.Parameters.AddWithValue(
							"@emoney", member.emoney);
						command.Parameters.AddWithValue(
							"@rank", member.rank);
						command.ExecuteNonQuery();
						member.id = checked((uint)command.LastInsertedId);
						retainedIds.Add(member.id);
					}
				}

				string deleteSql =
					"DELETE FROM cq_legion_members WHERE legion_id=@legion_id";
				if (retainedIds.Count > 0)
				{
					deleteSql += " AND id NOT IN (" +
						string.Join(",", retainedIds) + ")";
				}
				using (MySqlCommand deleteCommand = new MySqlCommand(
					deleteSql, connection, transaction))
				{
					deleteCommand.Parameters.AddWithValue(
						"@legion_id", legionId);
					deleteCommand.ExecuteNonQuery();
				}
				transaction.Commit();
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}
				throw;
			}
			finally
			{
				if (transaction != null)
				{
					transaction.Dispose();
				}
				MysqlConn.Conn_Close();
			}
		}

		public static void DeleteLegion(uint legionId)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				using (MySqlCommand members = new MySqlCommand(
					"DELETE FROM cq_legion_members WHERE legion_id=@id",
					connection,
					transaction))
				{
					members.Parameters.AddWithValue("@id", legionId);
					members.ExecuteNonQuery();
				}
				using (MySqlCommand legion = new MySqlCommand(
					"DELETE FROM cq_legion WHERE id=@id",
					connection,
					transaction))
				{
					legion.Parameters.AddWithValue("@id", legionId);
					legion.ExecuteNonQuery();
				}
				transaction.Commit();
			}
			catch
			{
				if (transaction != null)
				{
					transaction.Rollback();
				}
				throw;
			}
			finally
			{
				if (transaction != null)
				{
					transaction.Dispose();
				}
				MysqlConn.Conn_Close();
			}
		}
	}
}
