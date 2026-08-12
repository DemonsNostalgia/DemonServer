using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network.Internal;
using MySql.Data.MySqlClient;

namespace DBServer
{
	public class Legion
	{
		private static Legion mInstance;

		public readonly Dictionary<uint, LegionInfo> mDicInfo;

		public static Legion GetInstance()
		{
			if (mInstance == null)
			{
				mInstance = new Legion();
			}
			return mInstance;
		}

		public Legion()
		{
			mDicInfo = new Dictionary<uint, LegionInfo>();
		}

		public void DB_Load()
		{
			mDicInfo.Clear();
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"SELECT id,name,member_title,leader_id,leader_name," +
					"money,notice FROM cq_legion",
					connection))
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						LegionInfo info = new LegionInfo
						{
							id = reader.GetUInt32("id"),
							name = Decode(reader, "name"),
							title = reader.IsDBNull(
								reader.GetOrdinal("member_title")) ?
								(byte)0 :
								reader.GetByte("member_title"),
							leader_id = reader.GetInt32("leader_id"),
							leader_name = Decode(reader, "leader_name"),
							money = reader.IsDBNull(
								reader.GetOrdinal("money")) ?
								0L :
								reader.GetInt64("money"),
							notice = Decode(reader, "notice")
						};
						mDicInfo[info.id] = info;
					}
				}
			}
			finally
			{
				MysqlConn.Conn_Close();
			}

			foreach (LegionInfo info in mDicInfo.Values)
			{
				try
				{
					MysqlConn.Conn_Open();
					using (MySqlCommand command = new MySqlCommand(
						"SELECT id,player_id,members_name,money,emoney,`rank` " +
						"FROM cq_legion_members WHERE legion_id=@legion_id " +
						"ORDER BY `rank` DESC,id",
						connection))
					{
						command.Parameters.AddWithValue(
							"@legion_id", info.id);
						using (MySqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								info.list_member.Add(new LegionMember
								{
									id = reader.GetUInt32("id"),
									player_id = reader.GetInt32("player_id"),
									members_name = Decode(
										reader, "members_name"),
									money = reader.IsDBNull(
										reader.GetOrdinal("money")) ?
										0L :
										reader.GetInt64("money"),
									emoney = reader.IsDBNull(
										reader.GetOrdinal("emoney")) ?
										0L :
										reader.GetInt64("emoney"),
									rank = reader.GetInt16("rank")
								});
							}
						}
					}
				}
				finally
				{
					MysqlConn.Conn_Close();
				}
			}
			Log.Instance().WriteLog(
				"Loaded " + mDicInfo.Count.ToString() +
				" legion record(s).");
		}

		public void CreateLegion(LegionInfo info, int playerId)
		{
			CreateLegion_Ret response = new CreateLegion_Ret
			{
				play_id = playerId,
				legion_id = -1
			};
			try
			{
				int legionId = Data.CreateLegion(info);
				if (legionId <= 0)
				{
					throw new InvalidOperationException(
						"MySQL did not return a legion ID.");
				}
				info.id = (uint)legionId;
				LegionMember leader = new LegionMember
				{
					player_id = playerId,
					members_name = info.leader_name,
					money = info.money,
					rank = 1000
				};
				info.list_member.Add(leader);
				Data.SyncLegionMembers(info.id, info.list_member);
				mDicInfo[info.id] = info;

				response.ret = 1;
				response.legion_id = legionId;
				response.money = info.money;
				response.boss_id = leader.id;
			}
			catch (Exception exception)
			{
				if (info.id != 0)
				{
					try
					{
						Data.DeleteLegion(info.id);
					}
					catch (Exception cleanupException)
					{
						Log.Instance().WriteLog(
							"Failed to clean up legion " +
							info.id.ToString() + ": " +
							cleanupException.Message);
					}
				}
				Log.Instance().WriteLog(
					"Legion creation failed: " + exception.Message);
			}
			SessionManager.Instance().SendMapServer(
				0, response.GetBuffer());
		}

		public void UpdateLegion(LegionInfo info)
		{
			if (info == null || !mDicInfo.ContainsKey(info.id))
			{
				return;
			}
			Data.UpdateLegion(info);
			Data.SyncLegionMembers(info.id, info.list_member);
			for (int index = 0; index < info.list_member.Count; index++)
			{
				info.list_member[index].boChange = false;
			}
			mDicInfo[info.id] = info;
		}

		public void DeleteLegion(uint legionId)
		{
			Data.DeleteLegion(legionId);
			mDicInfo.Remove(legionId);
		}

		public void SendData(int mapserverid = 0)
		{
			LEGIONINFO legionInfo = new LEGIONINFO();
			foreach (LegionInfo item in mDicInfo.Values)
			{
				legionInfo.list_item.Add(item);
			}
			SessionManager.Instance().SendMapServer(
				mapserverid, legionInfo.GetBuffer());
		}

		private static string Decode(
			MySqlDataReader reader,
			string column)
		{
			int ordinal = reader.GetOrdinal(column);
			return reader.IsDBNull(ordinal) ?
				"" :
				Coding.Latin1ToGB2312(reader.GetString(ordinal));
		}
	}
}
