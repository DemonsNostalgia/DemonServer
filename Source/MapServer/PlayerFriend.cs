using System;
using System.Collections.Generic;
using GameBase.Network;
using GameBase.Network.Internal;
using NetMsg;

namespace MapServer
{
	public class PlayerFriend
	{
		public PlayerFriend(PlayerObject player)
		{
			this.play = player;
			this.mList = new List<RoleData_Friend>();
		}

		public void SendFriendInfo(RoleData_Friend info, byte type = 0)
		{
			if (info == null)
			{
				return;
			}
			byte action = type == 0 ? info.friendtype : type;
			MsgFriendInfo message = CreateRelationMessage(
				info.friendid, info.friendname, action,
				this.play.GetGamePackKeyEx());
			this.play.SendData(message.GetBuffer(), false);
		}

		public void SendAllFriendInfo()
		{
			List<RoleData_Friend> relations = this.GetActiveRelations();
			for (int i = 0; i < relations.Count; i++)
			{
				if (IsSupportedRelationType(relations[i].friendtype))
				{
					this.SendFriendInfo(relations[i]);
				}
			}
		}

		public void DB_Load(ROLEDATA_FRIEND info)
		{
			lock (this.syncRoot)
			{
				this.mList.Clear();
				HashSet<ulong> loaded = new HashSet<ulong>();
				for (int i = 0; i < info.list_item.Count; i++)
				{
					RoleData_Friend relation = info.list_item[i];
					if (relation == null || relation.id == -1 ||
						relation.friendid == 0U ||
						relation.friendid == (uint)this.play.GetBaseAttr().player_id ||
						!IsSupportedRelationType(relation.friendtype))
					{
						continue;
					}
					ulong key = MakeRelationKey(
						relation.friendid, relation.friendtype);
					if (loaded.Add(key))
					{
						this.mList.Add(relation);
					}
				}
			}
		}

		public void DB_Save()
		{
			if (!DBServer.Instance().IsConnect() ||
				DBServer.Instance().GetDBClient() == null)
			{
				return;
			}
			ROLEDATA_FRIEND data = new ROLEDATA_FRIEND();
			data.playerid = this.play.GetBaseAttr().player_id;
			data.SetSaveTag();
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					data.list_item.Add(this.mList[i]);
				}
			}
			DBServer.Instance().GetDBClient().SendData(data.GetBuffer());
		}

		public void BrocatMsg(byte type)
		{
			if (type != MsgFriendInfo.TYPE_ONLINE &&
				type != MsgFriendInfo.TYPE_OFFLIE)
			{
				return;
			}
			List<RoleData_Friend> friends =
				this.GetActiveRelations(MsgFriendInfo.TYPE_FRIEND);
			for (int i = 0; i < friends.Count; i++)
			{
				PlayerObject recipient = UserEngine.Instance()
					.FindPlayerObjectToPlayerId((int)friends[i].friendid);
				if (recipient != null && recipient.GetGameSession() != null)
				{
					this.SendPresence(recipient, type);
				}
			}
			byte enemyAction = type == MsgFriendInfo.TYPE_ONLINE ?
				MsgFriendInfo.TYPE_ENEMY_ONLINE :
				MsgFriendInfo.TYPE_ENEMY_OFFLINE;
			UserEngine.Instance().BroadcastEnemyStatus(this.play, enemyAction);
		}

		public void RequestAddFriend(MsgFriendInfo info)
		{
			if (this.play.IsDie())
			{
				this.play.LeftNotice(
					"You cannot send friend requests while dead.");
				return;
			}
			PlayerObject target = UserEngine.Instance()
				.FindPlayerObjectToTypeID(info.playerid);
			if (target == null || target.GetGameSession() == null)
			{
				this.play.LeftNotice(
					"The other player is offline and cannot receive a friend request.");
				return;
			}
			if (!this.play.GetVisibleList().ContainsKey(target.GetGameID()))
			{
				this.play.LeftNotice(
					"That player is no longer close enough to receive the request.");
				return;
			}
			uint requesterId = (uint)this.play.GetBaseAttr().player_id;
			uint targetId = (uint)target.GetBaseAttr().player_id;
			if (requesterId == targetId)
			{
				this.play.LeftNotice("You cannot add yourself as a friend.");
				return;
			}
			if (this.HasFriend(targetId))
			{
				this.play.LeftNotice(target.GetName() + " is already your friend.");
				return;
			}
			if (this.GetActiveCount(MsgFriendInfo.TYPE_FRIEND) >=
				MAX_FRIEND_COUNT)
			{
				this.play.LeftNotice(
					"Your friend list is full and cannot accept more friends.");
				return;
			}

			if (Requests.Consume(targetId, requesterId))
			{
				this.CompleteFriendship(target);
				return;
			}

			Requests.Register(requesterId, targetId);
			MsgFriendInfo request = CreateRelationMessage(
				requesterId, this.play.GetName(),
				MsgFriendInfo.TYPE_ADDFRIEND,
				target.GetGamePackKeyEx());
			target.SendData(request.GetBuffer(), false);
			this.play.LeftNotice("Friend request sent to " + target.GetName() + ".");
			target.LeftNotice(
				this.play.GetName() + " wants to add you as a friend.");
		}

		public void AcceptFriend(uint requesterId)
		{
			uint recipientId = (uint)this.play.GetBaseAttr().player_id;
			if (!Requests.Consume(requesterId, recipientId))
			{
				this.play.LeftNotice(
					"That friend request is no longer available.");
				return;
			}
			PlayerObject requester = UserEngine.Instance()
				.FindPlayerObjectToPlayerId((int)requesterId);
			if (requester == null || requester.GetGameSession() == null)
			{
				this.play.LeftNotice(
					"The player who sent that request is now offline.");
				return;
			}
			this.CompleteFriendship(requester);
		}

		public void AddFriend(uint playerId, byte type, bool party = true)
		{
			if (type == MsgFriendInfo.TYPE_FRIEND && party)
			{
				this.AcceptFriend(playerId);
				return;
			}
			PlayerObject target = UserEngine.Instance()
				.FindPlayerObjectToPlayerId((int)playerId);
			if (target != null)
			{
				this.AddRelation(target, type, true);
			}
		}

		public void DeleteFriend(uint playerId, bool deleteParty = true)
		{
			RoleData_Friend relation = this.FindActiveRelation(
				playerId, MsgFriendInfo.TYPE_FRIEND);
			if (relation == null)
			{
				this.play.LeftNotice("That player is not on your friend list.");
				return;
			}
			string friendName = relation.friendname;
			this.MarkDeleted(relation);
			this.SendFriendInfo(relation, MsgFriendInfo.TYPE_KILL);
			this.DB_Save();
			this.play.LeftNotice(
				"You are no longer friends with " + friendName + ".");

			if (!deleteParty)
			{
				return;
			}
			PlayerObject target = UserEngine.Instance()
				.FindPlayerObjectToPlayerId((int)playerId);
			if (target != null)
			{
				target.GetFriendSystem().DeleteFriend(
					(uint)this.play.GetBaseAttr().player_id, false);
			}
		}

		public void AddEnemy(PlayerObject enemy)
		{
			if (enemy == null || enemy == this.play)
			{
				return;
			}
			uint enemyId = (uint)enemy.GetBaseAttr().player_id;
			RoleData_Friend existing = this.FindActiveRelation(
				enemyId, MsgFriendInfo.TYPE_ENEMY);
			if (existing != null)
			{
				lock (this.syncRoot)
				{
					existing.friendname = enemy.GetName();
					this.mList.Remove(existing);
					this.mList.Add(existing);
				}
				this.SendFriendInfo(existing, MsgFriendInfo.TYPE_ENEMY);
				this.DB_Save();
				return;
			}

			if (this.GetActiveCount(MsgFriendInfo.TYPE_ENEMY) >=
				MAX_ENEMY_COUNT)
			{
				RoleData_Friend oldest = this.GetFirstActiveRelation(
					MsgFriendInfo.TYPE_ENEMY);
				if (oldest != null)
				{
					this.MarkDeleted(oldest);
					this.SendFriendInfo(
						oldest, MsgFriendInfo.TYPE_ENEMY_KILL);
				}
			}
			this.AddRelation(enemy, MsgFriendInfo.TYPE_ENEMY, true);
		}

		public void DeleteEnemy(uint playerId)
		{
			RoleData_Friend relation = this.FindActiveRelation(
				playerId, MsgFriendInfo.TYPE_ENEMY);
			if (relation == null)
			{
				return;
			}
			this.MarkDeleted(relation);
			this.SendFriendInfo(relation, MsgFriendInfo.TYPE_ENEMY_KILL);
			this.DB_Save();
		}

		public void RefuseFriend(uint requesterId)
		{
			uint recipientId = (uint)this.play.GetBaseAttr().player_id;
			if (!Requests.Consume(requesterId, recipientId))
			{
				return;
			}
			PlayerObject requester = UserEngine.Instance()
				.FindPlayerObjectToPlayerId((int)requesterId);
			if (requester != null)
			{
				MsgFriendInfo refusal = CreateRelationMessage(
					recipientId, this.play.GetName(),
					MsgFriendInfo.TYPE_REFUSE,
					requester.GetGamePackKeyEx());
				requester.SendData(refusal.GetBuffer(), false);
				requester.LeftNotice(
					this.play.GetName() + " declined your friend request.");
			}
		}

		public void GetFriendInfo(int playerId)
		{
			RoleData_Friend relation = this.FindActiveRelation(
				(uint)playerId, MsgFriendInfo.TYPE_FRIEND);
			if (relation == null)
			{
				relation = this.FindActiveRelation(
					(uint)playerId, MsgFriendInfo.TYPE_ENEMY);
			}
			if (relation == null)
			{
				return;
			}
			PlayerObject target = UserEngine.Instance()
				.FindPlayerObjectToPlayerId(playerId);
			if (target == null || target.GetGameSession() == null)
			{
				this.play.LeftNotice(relation.friendname + " is offline.");
				return;
			}

			uint legionIdAndRank = 0U;
			Legion legion = target.GetLegionSystem().GetLegion();
			if (legion != null)
			{
				int rank = target.GetLegionSystem().GetPlace();
				rank = Math.Max(0, Math.Min(byte.MaxValue, rank));
				legionIdAndRank =
					(legion.GetBaseInfo().id & 0x00ffffffU) |
					((uint)rank << 24);
			}
			MsgFriendDetail detail = new MsgFriendDetail();
			detail.Create(null, this.play.GetGamePackKeyEx());
			detail.playerId = (uint)playerId;
			detail.lookface = target.GetBaseAttr().lookface;
			detail.level = target.GetBaseAttr().level;
			detail.profession = target.GetBaseAttr().profession;
			detail.pkPoints = target.GetBaseAttr().pk;
			detail.legionIdAndRank = legionIdAndRank;
			detail.nobilityRank = (byte)target.GetGuanJue();
			detail.relationType =
				relation.friendtype == MsgFriendInfo.TYPE_FRIEND ?
					MsgFriendDetail.RELATION_FRIEND :
					MsgFriendDetail.RELATION_ENEMY;
			this.play.SendData(detail.GetBuffer(), false);
		}

		public void BroadcastChat(byte[] payload)
		{
			if (payload == null || this.play.GetGameSession() == null)
			{
				return;
			}
			PacketOut output = new PacketOut(null);
			output.WriteUInt16((ushort)(payload.Length + 2));
			output.WriteBuff(payload);
			byte[] packet = output.Flush();
			HashSet<int> recipients = new HashSet<int>();
			List<RoleData_Friend> friends =
				this.GetActiveRelations(MsgFriendInfo.TYPE_FRIEND);
			for (int i = 0; i < friends.Count; i++)
			{
				PlayerObject target = UserEngine.Instance()
					.FindPlayerObjectToPlayerId((int)friends[i].friendid);
				this.SendChatPacket(target, packet, recipients);
			}
		}

		public bool HasFriend(uint playerId)
		{
			return this.FindActiveRelation(
				playerId, MsgFriendInfo.TYPE_FRIEND) != null;
		}

		public bool HasEnemy(uint playerId)
		{
			return this.FindActiveRelation(
				playerId, MsgFriendInfo.TYPE_ENEMY) != null;
		}

		public void OnLogout()
		{
			Requests.RemoveForPlayer(
				(uint)this.play.GetBaseAttr().player_id);
		}

		private void CompleteFriendship(PlayerObject target)
		{
			if (target == null || target == this.play)
			{
				return;
			}
			uint targetId = (uint)target.GetBaseAttr().player_id;
			uint playerId = (uint)this.play.GetBaseAttr().player_id;
			if (this.HasFriend(targetId))
			{
				this.play.LeftNotice(target.GetName() + " is already your friend.");
				return;
			}
			if (this.GetActiveCount(MsgFriendInfo.TYPE_FRIEND) >=
				MAX_FRIEND_COUNT)
			{
				this.play.LeftNotice("Your friend list is full.");
				return;
			}
			if (target.GetFriendSystem().GetActiveCount(
				MsgFriendInfo.TYPE_FRIEND) >= MAX_FRIEND_COUNT)
			{
				this.play.LeftNotice(target.GetName() + " has a full friend list.");
				target.LeftNotice("Your friend list is full.");
				return;
			}

			this.AddRelation(target, MsgFriendInfo.TYPE_FRIEND, true);
			target.GetFriendSystem().AddRelation(
				this.play, MsgFriendInfo.TYPE_FRIEND, true);
			Requests.Consume(playerId, targetId);
			Requests.Consume(targetId, playerId);
			this.play.LeftNotice(
				"You and " + target.GetName() + " are now friends.");
			target.LeftNotice(
				"You and " + this.play.GetName() + " are now friends.");
		}

		private bool AddRelation(
			PlayerObject target,
			byte relationType,
			bool sendToClient)
		{
			if (target == null || !IsSupportedRelationType(relationType))
			{
				return false;
			}
			uint targetId = (uint)target.GetBaseAttr().player_id;
			if (targetId == 0U ||
				targetId == (uint)this.play.GetBaseAttr().player_id ||
				this.FindActiveRelation(targetId, relationType) != null)
			{
				return false;
			}

			RoleData_Friend relation = null;
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					if (this.mList[i].id == -1 &&
						this.mList[i].friendid == targetId &&
						this.mList[i].friendtype == relationType)
					{
						relation = this.mList[i];
						this.mList.RemoveAt(i);
						break;
					}
				}
				relation = new RoleData_Friend
				{
					id = 0,
					friendid = targetId,
					friendtype = relationType,
					friendname = target.GetName()
				};
				this.mList.Add(relation);
			}
			if (sendToClient)
			{
				this.SendFriendInfo(relation, relationType);
			}
			this.DB_Save();
			return true;
		}

		private static MsgFriendInfo CreateRelationMessage(
			uint playerId,
			string playerName,
			byte action,
			GamePacketKeyEx key)
		{
			MsgFriendInfo message = new MsgFriendInfo();
			message.Create(null, key);
			message.playerid = playerId;
			message.name = playerName ?? "";
			message.type = action;
			PlayerObject online = UserEngine.Instance()
				.FindPlayerObjectToPlayerId((int)playerId);
			if (online != null && online.GetGameSession() != null)
			{
				message.Online = 1;
				message.level = online.GetBaseAttr().level;
				message.fightpower = (uint)Math.Max(0, online.GetFightSoul());
			}
			return message;
		}

		private void SendPresence(PlayerObject recipient, byte action)
		{
			MsgFriendInfo message = CreateRelationMessage(
				(uint)this.play.GetBaseAttr().player_id,
				this.play.GetName(), action,
				recipient.GetGamePackKeyEx());
			if (action == MsgFriendInfo.TYPE_OFFLIE ||
				action == MsgFriendInfo.TYPE_ENEMY_OFFLINE)
			{
				message.Online = 0;
			}
			recipient.SendData(message.GetBuffer(), false);
		}

		public void SendEnemyPresence(PlayerObject subject, byte action)
		{
			if (subject == null ||
				!this.HasEnemy((uint)subject.GetBaseAttr().player_id))
			{
				return;
			}
			MsgFriendInfo message = CreateRelationMessage(
				(uint)subject.GetBaseAttr().player_id,
				subject.GetName(), action,
				this.play.GetGamePackKeyEx());
			if (action == MsgFriendInfo.TYPE_ENEMY_OFFLINE)
			{
				message.Online = 0;
			}
			this.play.SendData(message.GetBuffer(), false);
		}

		private void SendChatPacket(
			PlayerObject target,
			byte[] packet,
			HashSet<int> recipients)
		{
			if (target == null || target.GetGameSession() == null ||
				!recipients.Add(target.GetBaseAttr().player_id))
			{
				return;
			}
			BaseMsg encrypted = new BaseMsg();
			encrypted.Create(packet, target.GetGamePackKeyEx());
			target.SendData(encrypted.GetBuffer(), false);
		}

		private void MarkDeleted(RoleData_Friend relation)
		{
			lock (this.syncRoot)
			{
				relation.id = -1;
			}
		}

		private RoleData_Friend FindActiveRelation(
			uint playerId,
			byte relationType)
		{
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					RoleData_Friend relation = this.mList[i];
					if (relation.id != -1 && relation.friendid == playerId &&
						relation.friendtype == relationType)
					{
						return relation;
					}
				}
			}
			return null;
		}

		private RoleData_Friend GetFirstActiveRelation(byte relationType)
		{
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					if (this.mList[i].id != -1 &&
						this.mList[i].friendtype == relationType)
					{
						return this.mList[i];
					}
				}
			}
			return null;
		}

		private int GetActiveCount(byte relationType)
		{
			int count = 0;
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					if (this.mList[i].id != -1 &&
						this.mList[i].friendtype == relationType)
					{
						count++;
					}
				}
			}
			return count;
		}

		private List<RoleData_Friend> GetActiveRelations(byte relationType = 0)
		{
			List<RoleData_Friend> result = new List<RoleData_Friend>();
			lock (this.syncRoot)
			{
				for (int i = 0; i < this.mList.Count; i++)
				{
					RoleData_Friend relation = this.mList[i];
					if (relation.id != -1 &&
						(relationType == 0 ||
						 relation.friendtype == relationType))
					{
						result.Add(relation);
					}
				}
			}
			return result;
		}

		private static bool IsSupportedRelationType(byte relationType)
		{
			return relationType == MsgFriendInfo.TYPE_FRIEND ||
				relationType == MsgFriendInfo.TYPE_ENEMY;
		}

		private static ulong MakeRelationKey(uint playerId, byte relationType)
		{
			return ((ulong)relationType << 32) | playerId;
		}

		public const int MAX_FRIEND_COUNT = 50;

		public const int MAX_ENEMY_COUNT = 10;

		private static readonly FriendRequestRegistry Requests =
			new FriendRequestRegistry();

		private readonly object syncRoot = new object();

		private readonly PlayerObject play;

		private readonly List<RoleData_Friend> mList;
	}
}
