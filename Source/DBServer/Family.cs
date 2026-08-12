using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network.Internal;
using MySql.Data.MySqlClient;

namespace DBServer
{
	public sealed class Family
	{
		private static Family instance;

		public static Family GetInstance()
		{
			if (instance == null)
			{
				instance = new Family();
			}
			return instance;
		}

		private Family()
		{
			families = new Dictionary<uint, FamilyInfo>();
		}

		public void DB_Load()
		{
			EnsureSchema();
			families.Clear();
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"SELECT id,family_name,`rank`,leader_name,leader_id," +
					"announce,money,repute,create_date,create_name,del_flag," +
					"star_tower,challenge_map,family_map,truce," +
					"ally_family0_id,ally_family1_id,ally_family2_id," +
					"ally_family3_id,ally_family4_id,enemy_family0_id," +
					"enemy_family1_id,enemy_family2_id,enemy_family3_id," +
					"enemy_family4_id FROM cq_family WHERE del_flag=0",
					connection))
				using (MySqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						FamilyInfo info = new FamilyInfo();
						info.Id = reader.GetUInt32("id");
						info.Name = Decode(reader, "family_name");
						info.Rank = reader.GetByte("rank");
						info.LeaderName = Decode(reader, "leader_name");
						info.LeaderId = reader.GetInt32("leader_id");
						info.Announcement = Decode(reader, "announce");
						info.Money = reader.GetUInt64("money");
						info.Reputation = reader.GetUInt32("repute");
						info.CreateDate = reader.GetUInt32("create_date");
						info.CreateName = Decode(reader, "create_name");
						info.Deleted = reader.GetByte("del_flag");
						info.StarTower = reader.GetByte("star_tower");
						info.ChallengeMap = reader.GetUInt32("challenge_map");
						info.FamilyMap = reader.GetUInt32("family_map");
						info.Truce = reader.GetUInt32("truce");
						for (int index = 0; index < 5; index++)
						{
							info.AllyIds[index] = reader.GetUInt32(
								"ally_family" + index.ToString() + "_id");
							info.EnemyIds[index] = reader.GetUInt32(
								"enemy_family" + index.ToString() + "_id");
						}
						families[info.Id] = info;
					}
				}
			}
			finally
			{
				MysqlConn.Conn_Close();
			}

			foreach (FamilyInfo family in families.Values)
			{
				LoadMembers(family);
			}
			Log.Instance().WriteLog(
				"Loaded " + families.Count.ToString() + " family record(s).");
		}

		public void CreateFamily(FamilyInfo info, int playerId)
		{
			CreateFamilyResult result = new CreateFamilyResult
			{
				PlayerId = playerId,
				FamilyId = -1,
				Success = 0
			};
			try
			{
				SaveNewFamily(info);
				families[info.Id] = info;
				result.FamilyId = checked((int)info.Id);
				result.Success = 1;
			}
			catch (Exception exception)
			{
				Log.Instance().WriteLog(
					"Family creation failed: " + exception.Message);
			}
			SessionManager.Instance().SendMapServer(0, result.GetBuffer());
		}

		public void UpdateFamily(FamilyInfo info)
		{
			if (info == null || !families.ContainsKey(info.Id))
			{
				return;
			}
			SaveExistingFamily(info);
			families[info.Id] = info;
		}

		public void DeleteFamily(uint familyId)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				using (MySqlCommand members = new MySqlCommand(
					"DELETE FROM cq_family_attr WHERE family_id=@family_id",
					connection, transaction))
				{
					members.Parameters.AddWithValue("@family_id", familyId);
					members.ExecuteNonQuery();
				}
				using (MySqlCommand family = new MySqlCommand(
					"DELETE FROM cq_family WHERE id=@family_id",
					connection, transaction))
				{
					family.Parameters.AddWithValue("@family_id", familyId);
					family.ExecuteNonQuery();
				}
				transaction.Commit();
				families.Remove(familyId);
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

		public void SendData(int mapServerId)
		{
			FamilyCollection collection = new FamilyCollection();
			foreach (FamilyInfo family in families.Values)
			{
				collection.Families.Add(family);
			}
			SessionManager.Instance().SendMapServer(
				mapServerId, collection.GetBuffer());
		}

		private static void EnsureSchema()
		{
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"CREATE TABLE IF NOT EXISTS cq_family (" +
					"id int unsigned NOT NULL AUTO_INCREMENT," +
					"family_name varchar(15) NOT NULL," +
					"`rank` tinyint unsigned NOT NULL DEFAULT 0," +
					"leader_name varchar(32) NOT NULL," +
					"leader_id int NOT NULL," +
					"announce varchar(127) NOT NULL DEFAULT ''," +
					"money bigint unsigned NOT NULL DEFAULT 0," +
					"repute int unsigned NOT NULL DEFAULT 0," +
					"amount int unsigned NOT NULL DEFAULT 0," +
					"enemy_family0_id int unsigned NOT NULL DEFAULT 0," +
					"enemy_family1_id int unsigned NOT NULL DEFAULT 0," +
					"enemy_family2_id int unsigned NOT NULL DEFAULT 0," +
					"enemy_family3_id int unsigned NOT NULL DEFAULT 0," +
					"enemy_family4_id int unsigned NOT NULL DEFAULT 0," +
					"ally_family0_id int unsigned NOT NULL DEFAULT 0," +
					"ally_family1_id int unsigned NOT NULL DEFAULT 0," +
					"ally_family2_id int unsigned NOT NULL DEFAULT 0," +
					"ally_family3_id int unsigned NOT NULL DEFAULT 0," +
					"ally_family4_id int unsigned NOT NULL DEFAULT 0," +
					"create_date int unsigned NOT NULL DEFAULT 0," +
					"create_name varchar(32) NOT NULL DEFAULT ''," +
					"del_flag tinyint unsigned NOT NULL DEFAULT 0," +
					"star_tower tinyint unsigned NOT NULL DEFAULT 0," +
					"challenge_map int unsigned NOT NULL DEFAULT 0," +
					"family_map int unsigned NOT NULL DEFAULT 0," +
					"truce int unsigned NOT NULL DEFAULT 0," +
					"PRIMARY KEY (id),UNIQUE KEY uq_cq_family_name (family_name)," +
					"UNIQUE KEY uq_cq_family_leader (leader_id)" +
					") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",
					connection))
				{
					command.ExecuteNonQuery();
				}
				using (MySqlCommand command = new MySqlCommand(
					"CREATE TABLE IF NOT EXISTS cq_family_attr (" +
					"id int NOT NULL,family_id int unsigned NOT NULL," +
					"`rank` smallint unsigned NOT NULL DEFAULT 10," +
					"proffer int unsigned NOT NULL DEFAULT 0," +
					"join_date int unsigned NOT NULL DEFAULT 0," +
					"auto_exercise tinyint unsigned NOT NULL DEFAULT 0," +
					"exp_date int unsigned NOT NULL DEFAULT 0," +
					"PRIMARY KEY (id),KEY ix_cq_family_attr_family (family_id)," +
					"CONSTRAINT fk_cq_family_attr_family FOREIGN KEY (family_id) " +
					"REFERENCES cq_family(id) ON DELETE CASCADE" +
					") ENGINE=InnoDB DEFAULT CHARSET=utf8mb4",
					connection))
				{
					command.ExecuteNonQuery();
				}
			}
			finally
			{
				MysqlConn.Conn_Close();
			}
		}

		private static void LoadMembers(FamilyInfo family)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			try
			{
				MysqlConn.Conn_Open();
				using (MySqlCommand command = new MySqlCommand(
					"SELECT attr.id,attr.`rank`,attr.proffer,attr.join_date," +
					"attr.auto_exercise,attr.exp_date,user.name " +
					"FROM cq_family_attr AS attr " +
					"LEFT JOIN cq_user AS user ON user.id=attr.id " +
					"WHERE attr.family_id=@family_id " +
					"ORDER BY attr.`rank` DESC,attr.join_date,attr.id",
					connection))
				{
					command.Parameters.AddWithValue("@family_id", family.Id);
					using (MySqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							family.Members.Add(new FamilyMember
							{
								PlayerId = reader.GetInt32("id"),
								Name = Decode(reader, "name"),
								Rank = reader.GetUInt16("rank"),
								Proffer = reader.GetUInt32("proffer"),
								JoinDate = reader.GetUInt32("join_date"),
								AutoExercise = reader.GetByte("auto_exercise"),
								ExpDate = reader.GetUInt32("exp_date")
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

		private static void SaveNewFamily(FamilyInfo info)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				using (MySqlCommand command = BuildInsertCommand(
					connection, transaction, info))
				{
					command.ExecuteNonQuery();
					info.Id = checked((uint)command.LastInsertedId);
				}
				InsertMembers(connection, transaction, info);
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

		private static void SaveExistingFamily(FamilyInfo info)
		{
			MySqlConnection connection = MysqlConn.GetConn();
			MySqlTransaction transaction = null;
			try
			{
				MysqlConn.Conn_Open();
				transaction = connection.BeginTransaction();
				using (MySqlCommand command = BuildUpdateCommand(
					connection, transaction, info))
				{
					command.ExecuteNonQuery();
				}
				using (MySqlCommand command = new MySqlCommand(
					"DELETE FROM cq_family_attr WHERE family_id=@family_id",
					connection, transaction))
				{
					command.Parameters.AddWithValue("@family_id", info.Id);
					command.ExecuteNonQuery();
				}
				InsertMembers(connection, transaction, info);
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

		private static MySqlCommand BuildInsertCommand(
			MySqlConnection connection,
			MySqlTransaction transaction,
			FamilyInfo info)
		{
			MySqlCommand command = new MySqlCommand(
				"INSERT INTO cq_family (family_name,`rank`,leader_name," +
				"leader_id,announce,money,repute,amount,create_date," +
				"create_name,del_flag,star_tower,challenge_map,family_map," +
				"truce,ally_family0_id,ally_family1_id,ally_family2_id," +
				"ally_family3_id,ally_family4_id,enemy_family0_id," +
				"enemy_family1_id,enemy_family2_id,enemy_family3_id," +
				"enemy_family4_id) VALUES (@name,@rank,@leader_name," +
				"@leader_id,@announce,@money,@repute,@amount,@create_date," +
				"@create_name,@deleted,@tower,@challenge,@family_map,@truce," +
				"@ally0,@ally1,@ally2,@ally3,@ally4,@enemy0,@enemy1," +
				"@enemy2,@enemy3,@enemy4)", connection, transaction);
			AddFamilyParameters(command, info, false);
			return command;
		}

		private static MySqlCommand BuildUpdateCommand(
			MySqlConnection connection,
			MySqlTransaction transaction,
			FamilyInfo info)
		{
			MySqlCommand command = new MySqlCommand(
				"UPDATE cq_family SET family_name=@name,`rank`=@rank," +
				"leader_name=@leader_name,leader_id=@leader_id," +
				"announce=@announce,money=@money,repute=@repute," +
				"amount=@amount,create_date=@create_date," +
				"create_name=@create_name,del_flag=@deleted," +
				"star_tower=@tower,challenge_map=@challenge," +
				"family_map=@family_map,truce=@truce," +
				"ally_family0_id=@ally0,ally_family1_id=@ally1," +
				"ally_family2_id=@ally2,ally_family3_id=@ally3," +
				"ally_family4_id=@ally4,enemy_family0_id=@enemy0," +
				"enemy_family1_id=@enemy1,enemy_family2_id=@enemy2," +
				"enemy_family3_id=@enemy3,enemy_family4_id=@enemy4 " +
				"WHERE id=@id", connection, transaction);
			AddFamilyParameters(command, info, true);
			return command;
		}

		private static void AddFamilyParameters(
			MySqlCommand command,
			FamilyInfo info,
			bool includeId)
		{
			command.Parameters.AddWithValue(
				"@name", Coding.GB2312ToLatin1(info.Name ?? ""));
			command.Parameters.AddWithValue("@rank", info.Rank);
			command.Parameters.AddWithValue(
				"@leader_name", Coding.GB2312ToLatin1(info.LeaderName ?? ""));
			command.Parameters.AddWithValue("@leader_id", info.LeaderId);
			command.Parameters.AddWithValue(
				"@announce", Coding.GB2312ToLatin1(info.Announcement ?? ""));
			command.Parameters.AddWithValue("@money", info.Money);
			command.Parameters.AddWithValue("@repute", info.Reputation);
			command.Parameters.AddWithValue("@amount", info.Members.Count);
			command.Parameters.AddWithValue("@create_date", info.CreateDate);
			command.Parameters.AddWithValue(
				"@create_name", Coding.GB2312ToLatin1(info.CreateName ?? ""));
			command.Parameters.AddWithValue("@deleted", info.Deleted);
			command.Parameters.AddWithValue("@tower", info.StarTower);
			command.Parameters.AddWithValue("@challenge", info.ChallengeMap);
			command.Parameters.AddWithValue("@family_map", info.FamilyMap);
			command.Parameters.AddWithValue("@truce", info.Truce);
			for (int index = 0; index < 5; index++)
			{
				command.Parameters.AddWithValue(
					"@ally" + index.ToString(), info.AllyIds[index]);
				command.Parameters.AddWithValue(
					"@enemy" + index.ToString(), info.EnemyIds[index]);
			}
			if (includeId)
			{
				command.Parameters.AddWithValue("@id", info.Id);
			}
		}

		private static void InsertMembers(
			MySqlConnection connection,
			MySqlTransaction transaction,
			FamilyInfo info)
		{
			for (int index = 0; index < info.Members.Count; index++)
			{
				FamilyMember member = info.Members[index];
				using (MySqlCommand command = new MySqlCommand(
					"INSERT INTO cq_family_attr " +
					"(id,family_id,`rank`,proffer,join_date,auto_exercise," +
					"exp_date) VALUES (@id,@family_id,@rank,@proffer," +
					"@join_date,@auto_exercise,@exp_date)",
					connection, transaction))
				{
					command.Parameters.AddWithValue("@id", member.PlayerId);
					command.Parameters.AddWithValue("@family_id", info.Id);
					command.Parameters.AddWithValue("@rank", member.Rank);
					command.Parameters.AddWithValue("@proffer", member.Proffer);
					command.Parameters.AddWithValue("@join_date", member.JoinDate);
					command.Parameters.AddWithValue(
						"@auto_exercise", member.AutoExercise);
					command.Parameters.AddWithValue("@exp_date", member.ExpDate);
					command.ExecuteNonQuery();
				}
			}
		}

		private static string Decode(MySqlDataReader reader, string column)
		{
			int ordinal = reader.GetOrdinal(column);
			return reader.IsDBNull(ordinal) ? "" :
				Coding.Latin1ToGB2312(reader.GetString(ordinal));
		}

		private readonly Dictionary<uint, FamilyInfo> families;
	}
}
