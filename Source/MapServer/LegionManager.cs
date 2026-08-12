using System;
using System.Collections.Generic;
using System.Globalization;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	public class LegionManager
	{
		public const short LeaderRank = 1000;
		public const short DeputyLeaderRank = 990;
		public const short MemberRank = 200;

		private const int RequestLifetimeMilliseconds = 60000;

		private sealed class PendingCreation
		{
			public LegionInfo Info;
			public int CreationCost;
		}

		private sealed class PendingRequest
		{
			public uint LeaderTypeId;
			public uint MemberTypeId;
			public short Rank;
			public int CreatedAt;
		}

		public static LegionManager Instance()
		{
			if (mInstance == null)
			{
				mInstance = new LegionManager();
			}
			return mInstance;
		}

		public LegionManager()
		{
			mDicLegion = new Dictionary<uint, Legion>();
			pendingCreations = new Dictionary<int, PendingCreation>();
			pendingJoins = new List<PendingRequest>();
			pendingInvites = new List<PendingRequest>();
			pendingAppointments = new List<PendingRequest>();
			pendingTrustedAppointments = new List<PendingRequest>();
			pendingKickDocuments =
				new Dictionary<uint, List<string>>();
		}

		public void DB_Load(LEGIONINFO info)
		{
			mDicLegion.Clear();
			for (int index = 0; index < info.list_item.Count; index++)
			{
				LegionInfo legionInfo = info.list_item[index];
				if (FindMember(legionInfo, legionInfo.leader_name) == null)
				{
					legionInfo.list_member.Add(new LegionMember
					{
						player_id = legionInfo.leader_id,
						members_name = legionInfo.leader_name,
						rank = LeaderRank,
						boChange = true
					});
				}
				Legion legion = new Legion();
				legion.SetBaseInfo(legionInfo);
				mDicLegion[legionInfo.id] = legion;
			}
			Log.Instance().WriteLog(
				"Loaded " + mDicLegion.Count.ToString() +
				" legion record(s) from DBServer.");
		}

		public Legion GetLegion(uint id)
		{
			Legion legion;
			return mDicLegion.TryGetValue(id, out legion) ? legion : null;
		}

		public Legion GetPlayerLegion(string playerName)
		{
			foreach (Legion legion in mDicLegion.Values)
			{
				if (FindMember(legion.GetBaseInfo(), playerName) != null)
				{
					return legion;
				}
			}
			return null;
		}

		public bool IsExist(string legionName)
		{
			foreach (Legion legion in mDicLegion.Values)
			{
				if (string.Equals(
					legion.GetBaseInfo().name,
					legionName,
					StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool CreateLegion(
			int playerId,
			string legionName,
			string leaderName,
			byte title,
			long money,
			string notice,
			int creationCost = 0)
		{
			PlayerObject player =
				UserEngine.Instance().FindPlayerObjectToPlayerId(playerId);
			if (player == null ||
				player.GetLegionSystem().IsHaveLegion() ||
				pendingCreations.ContainsKey(playerId) ||
				IsExist(legionName) ||
				string.IsNullOrWhiteSpace(legionName) ||
				legionName.Length > 15 ||
				creationCost < 0 ||
				player.GetMoneyCount(MONEYTYPE.GOLD) < creationCost ||
				!DBServer.Instance().IsConnect())
			{
				if (player != null)
				{
					player.ChatNotice("The legion could not be created.");
				}
				return false;
			}

			LegionInfo info = new LegionInfo
			{
				leader_id = playerId,
				leader_name = leaderName,
				name = legionName,
				title = title,
				money = money,
				notice = notice
			};
			pendingCreations[playerId] = new PendingCreation
			{
				Info = info,
				CreationCost = creationCost
			};

			if (creationCost > 0)
			{
				player.ChangeMoney(MONEYTYPE.GOLD, -creationCost);
			}

			LegionOption option = new LegionOption();
			option.SetCreateTag();
			option.player_id = playerId;
			option.mInfo = info;
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
			return true;
		}

		public void CreateLegion_Ret(CreateLegion_Ret result)
		{
			if (result == null)
			{
				return;
			}

			PendingCreation pending;
			if (!pendingCreations.TryGetValue(result.play_id, out pending))
			{
				Log.Instance().WriteLog(
					"Ignored unmatched legion creation response for player " +
					result.play_id.ToString() + ".");
				return;
			}
			pendingCreations.Remove(result.play_id);

			PlayerObject player =
				UserEngine.Instance().FindPlayerObjectToPlayerId(result.play_id);
			if (result.ret == 0 || result.legion_id <= 0)
			{
				if (player != null && pending.CreationCost > 0)
				{
					player.ChangeMoney(
						MONEYTYPE.GOLD, pending.CreationCost);
					player.ChatNotice(
						"Legion creation failed; the creation fee was refunded.");
				}
				return;
			}

			LegionInfo info = pending.Info;
			info.id = (uint)result.legion_id;
			info.list_member.Add(new LegionMember
			{
				id = result.boss_id,
				player_id = result.play_id,
				members_name = info.leader_name,
				money = result.money,
				rank = LeaderRank
			});
			Legion legion = new Legion();
			legion.SetBaseInfo(info);
			mDicLegion[info.id] = legion;
			if (player != null)
			{
				player.GetLegionSystem().SetLegion(legion, true);
				player.ChatNotice("Legion created.");
			}
		}

		public void UpdateLegionInfo(uint legionId, int playerId)
		{
			Legion legion;
			if (!mDicLegion.TryGetValue(legionId, out legion))
			{
				return;
			}
			LegionOption option = new LegionOption();
			option.SetUpdateTag();
			option.player_id = playerId;
			option.mInfo = legion.GetBaseInfo();
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
		}

		public bool AddMember(uint legionId, PlayerObject player)
		{
			Legion legion = GetLegion(legionId);
			if (legion == null || player == null ||
				player.GetLegionSystem().IsHaveLegion())
			{
				return false;
			}

			LegionInfo info = legion.GetBaseInfo();
			if (info.list_member.Count >= byte.MaxValue)
			{
				player.ChatNotice("This legion has reached its member limit.");
				return false;
			}
			if (FindMember(info, player.GetName()) != null)
			{
				return false;
			}
			info.list_member.Add(new LegionMember
			{
				player_id = player.GetBaseAttr().player_id,
				members_name = player.GetName(),
				rank = MemberRank,
				boChange = true
			});
			player.GetLegionSystem().SetLegion(legion, true);
			UpdateLegionInfo(legionId, player.GetBaseAttr().player_id);
			player.ChatNotice("You joined " + info.name + ".");
			return true;
		}

		public bool ChangeMemberPlace(
			uint legionId,
			string playerName,
			short place)
		{
			Legion legion = GetLegion(legionId);
			if (legion == null)
			{
				return false;
			}
			LegionMember member =
				FindMember(legion.GetBaseInfo(), playerName);
			if (member == null)
			{
				return false;
			}
			member.rank = place;
			member.boChange = true;
			UpdateLegionInfo(legionId, member.player_id);
			PlayerObject online =
				UserEngine.Instance().FindPlayerObjectToName(playerName);
			if (online != null)
			{
				online.GetLegionSystem().SendLegionInfo();
			}
			return true;
		}

		public bool QuitLegion(PlayerObject player)
		{
			if (player == null)
			{
				return false;
			}
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null)
			{
				return false;
			}
			LegionInfo info = legion.GetBaseInfo();
			if (info.leader_id == player.GetBaseAttr().player_id)
			{
				if (info.list_member.Count > 1)
				{
					player.ChatNotice(
						"The legion leader must transfer leadership or remove " +
						"the other members before leaving.");
					return false;
				}
				DeleteLegion(legion, player);
				return true;
			}
			return RemoveMember(legion, player.GetName(), player, false);
		}

		public void HandleSyndicatePacket(
			PlayerObject player,
			SyndicateQueryPacket packet)
		{
			ExpireRequests();
			switch (packet.Action)
			{
			case 1:
				HandleJoinOrInviteAcceptance(player, packet.TargetId);
				break;
			case 2:
				HandleInviteOrJoinAcceptance(player, packet.TargetId);
				break;
			case 3:
				if (packet.TargetId == player.GetTypeId() &&
					QuitLegion(player))
				{
					SendSyndicate(player, 3, player.GetTypeId(), 0, 0);
				}
				break;
			case 4:
				HandleKick(player, FirstString(packet));
				break;
			case 6:
				HandleLegionNameQuery(player, packet.TargetId);
				break;
			case 11:
				HandleDonation(player, packet.TargetId, false);
				break;
			case 12:
				if (packet.TargetId == player.GetTypeId())
				{
					player.GetLegionSystem().SendLegionInfo();
					SendNoticeSnapshot(player);
				}
				break;
			case 110:
				HandleKickDocumentQuery(player);
				break;
			case 111:
				HandleKickDocumentDecision(
					player, FirstString(packet), true);
				break;
			case 112:
				HandleKickDocumentDecision(
					player, FirstString(packet), false);
				break;
			case 113:
				HandleDirectRankChange(
					player, FirstString(packet), packet.TargetId);
				break;
			case 120:
				HandleAppointmentRequest(
					player, packet.TargetId, packet.FealtyId);
				break;
			case 122:
				RemoveAppointmentFor(player.GetTypeId());
				player.ChatNotice("Legion appointment declined.");
				break;
			case 123:
				HandleAppointmentAcceptance(player, packet.TargetId);
				break;
			case 124:
				HandleDismissal(player, packet);
				break;
			case 126:
				HandleResignation(player);
				break;
			case 136:
				HandleDelayedAppointment(player, packet.TargetId);
				break;
			case 137:
				HandleTrustedAideRequest(
					player, packet.TargetId, packet.FealtyId);
				break;
			case 138:
				HandleTrustedAideAcceptance(player, packet.TargetId);
				break;
			case 139:
				HandleTrustedAideRefusal(player, packet.TargetId);
				break;
			case 140:
				HandleTrustedAideDismissal(player, packet.TargetId);
				break;
			case 141:
				HandleTrustedAideResignation(player);
				break;
			case 142:
				HandleDonation(player, packet.TargetId, true);
				break;
			case 143:
			case 144:
				HandleNoBotherReply(
					player, packet.TargetId, packet.Action);
				break;
			case 171:
				SendSyndicate(
					player, 171, 0, 0, 0, new string[0]);
				break;
			default:
				player.ChatNotice(
					"This legion feature is not available in this server build.");
				Log.Instance().WriteLog(
					"Unsupported legion action " + packet.Action.ToString() +
					" from " + player.GetName() + ".");
				break;
			}
		}

		public void HandleNameQuery(
			PlayerObject player,
			NameQueryPacket packet)
		{
			if (packet.Action != 11 && packet.Action != 38)
			{
				return;
			}
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null)
			{
				player.SendData(
					MapPacketCodec.CreateNameQueryResponse(
						player.GetGamePackKeyEx(),
						0,
						packet.Action),
					false);
				return;
			}

			LegionInfo info = legion.GetBaseInfo();
			List<string> records = new List<string>();
			for (int index = 0;
				index < info.list_member.Count &&
					records.Count < byte.MaxValue;
				index++)
			{
				LegionMember member = info.list_member[index];
				PlayerObject online =
					UserEngine.Instance().FindPlayerObjectToPlayerId(
						member.player_id);
				uint roleId = online == null ?
					unchecked((uint)member.player_id) :
					online.GetTypeId();
				int lookface =
					online == null ? 0 : online.GetLookFace();
				int level =
					online == null ? 0 : online.GetLevel();
				records.Add(string.Format(
					CultureInfo.InvariantCulture,
					"{0} {1} {2} {3} {4} {5} {6} {7} {8}",
					info.id,
					member.members_name,
					member.rank,
					roleId,
					lookface,
					level,
					online == null ? 0 : 1,
					ClampContribution(member.money, member.emoney),
					0));
			}
			player.SendData(
				MapPacketCodec.CreateNameQueryResponse(
					player.GetGamePackKeyEx(),
					info.id,
					packet.Action,
					records.ToArray()),
				false);
		}

		public void HandleMemberQuery(
			PlayerObject player,
			SyndicateMemberQueryPacket packet)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			LegionMember member = legion == null ?
				null :
				FindMember(legion.GetBaseInfo(), packet.MemberName);
			PlayerObject online = member == null ?
				null :
				UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.player_id);
			player.SendData(
				MapPacketCodec.CreateSyndicateMemberResponse(
					player.GetGamePackKeyEx(),
					unchecked((ushort)(member == null ? 0 : member.rank)),
					online != null,
					online == null ? (byte)0 : online.GetLevel(),
					online == null ?
						(byte)0 :
						online.GetBaseAttr().profession,
					online == null ?
						(member == null ?
							0U :
							unchecked((uint)member.player_id)) :
						online.GetTypeId(),
					member == null ?
						0 :
						ClampContribution(member.money, member.emoney)),
				false);
		}

		public static int ClampContribution(long value)
		{
			if (value > int.MaxValue)
			{
				return int.MaxValue;
			}
			if (value < 0)
			{
				return 0;
			}
			return (int)value;
		}

		public static int ClampContribution(long money, long emoney)
		{
			if (money <= 0)
			{
				return ClampContribution(emoney);
			}
			if (emoney <= 0)
			{
				return ClampContribution(money);
			}
			if (money >= int.MaxValue ||
				emoney >= int.MaxValue ||
				money > int.MaxValue - emoney)
			{
				return int.MaxValue;
			}
			return (int)(money + emoney);
		}

		public void UpdateNotice(
			PlayerObject player,
			string notice,
			byte[] originalPayload)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null || !player.GetLegionSystem().IsLeader())
			{
				player.ChatNotice(
					"Only the legion leader can change the announcement.");
				return;
			}
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(notice ?? "");
			if (encoded.Length == 0 || encoded.Length > 64)
			{
				player.ChatNotice(
					"Legion announcements must be between 1 and 64 bytes.");
				return;
			}

			legion.GetBaseInfo().notice = notice;
			UpdateLegionInfo(
				legion.GetBaseInfo().id,
				player.GetBaseAttr().player_id);
			UserEngine.Instance().BroadcastLegionPayload(
				player, legion, originalPayload);
			BroadcastNoticeTimestamp(legion);
		}

		private void HandleJoinOrInviteAcceptance(
			PlayerObject player,
			uint targetId)
		{
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null)
			{
				player.ChatNotice("The selected player is not online.");
				return;
			}

			PendingRequest invitation = FindRequest(
				pendingInvites, target.GetTypeId(), player.GetTypeId());
			if (invitation != null)
			{
				pendingInvites.Remove(invitation);
				Legion legion = target.GetLegionSystem().GetLegion();
				if (legion != null && CanManageMembers(target) &&
					AddMember(legion.GetBaseInfo().id, player))
				{
					target.ChatNotice(
						player.GetName() + " accepted the legion invitation.");
				}
				return;
			}

			if (player.GetLegionSystem().IsHaveLegion())
			{
				player.ChatNotice("You already belong to a legion.");
				return;
			}
			if (!CanManageMembers(target))
			{
				player.ChatNotice(
					"The selected player cannot accept legion applications.");
				return;
			}
			ReplaceRequest(
				pendingJoins,
				target.GetTypeId(),
				player.GetTypeId(),
				0);
			SendSyndicate(
				target,
				1,
				player.GetTypeId(),
				0,
				player.GetLevel());
			player.ChatNotice("Legion application sent.");
		}

		private void HandleInviteOrJoinAcceptance(
			PlayerObject player,
			uint targetId)
		{
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null)
			{
				player.ChatNotice("The selected player is not online.");
				return;
			}

			PendingRequest join = FindRequest(
				pendingJoins, player.GetTypeId(), target.GetTypeId());
			if (join != null)
			{
				pendingJoins.Remove(join);
				Legion legion = player.GetLegionSystem().GetLegion();
				if (legion != null && CanManageMembers(player) &&
					AddMember(legion.GetBaseInfo().id, target))
				{
					player.ChatNotice(
						target.GetName() + " joined the legion.");
				}
				return;
			}

			if (!CanManageMembers(player))
			{
				player.ChatNotice("You cannot invite legion members.");
				return;
			}
			if (target.GetLegionSystem().IsHaveLegion())
			{
				player.ChatNotice(
					"The selected player already belongs to a legion.");
				return;
			}
			ReplaceRequest(
				pendingInvites,
				player.GetTypeId(),
				target.GetTypeId(),
				0);
			SendSyndicate(
				target,
				2,
				player.GetTypeId(),
				0,
				player.GetLevel());
			player.ChatNotice("Legion invitation sent.");
		}

		private void HandleKick(PlayerObject player, string memberName)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(memberName) ||
				string.Equals(
					memberName,
					player.GetName(),
					StringComparison.OrdinalIgnoreCase))
			{
				player.ChatNotice("The legion leader cannot remove themselves.");
				return;
			}
			LegionMember member =
				FindMember(legion.GetBaseInfo(), memberName);
			if (member == null)
			{
				player.ChatNotice("Legion member not found.");
				return;
			}
			if (!CanManageMembers(player))
			{
				LegionMember actor =
					FindMember(legion.GetBaseInfo(), player.GetName());
				if (actor == null || actor.rank <= MemberRank)
				{
					player.ChatNotice(
						"Only a legion officer can request a member removal.");
					return;
				}
				List<string> documents = GetKickDocuments(
					legion.GetBaseInfo().id);
				if (!ContainsName(documents, member.members_name))
				{
					documents.Add(member.members_name);
				}
				player.ChatNotice(
					"The member-removal request was sent to the legion leader.");
				return;
			}
			RemoveKickDocument(
				legion.GetBaseInfo().id, member.members_name);
			KickMember(player, legion, member);
		}

		private void KickMember(
			PlayerObject player,
			Legion legion,
			LegionMember member)
		{
			PlayerObject online =
				UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.player_id);
			uint memberId = online == null ?
				unchecked((uint)member.player_id) :
				online.GetTypeId();
			if (RemoveMember(legion, member.members_name, online, true))
			{
				SendSyndicate(player, 4, memberId, 0, 0);
				if (online != null)
				{
					SendSyndicate(online, 4, memberId, 0, 0);
				}
			}
		}

		private void HandleLegionNameQuery(
			PlayerObject player,
			uint legionId)
		{
			Legion legion = GetLegion(legionId);
			if (legion == null)
			{
				SendSyndicate(player, 14, legionId, 0, 0, "", "");
				return;
			}
			LegionInfo info = legion.GetBaseInfo();
			SendSyndicate(
				player,
				14,
				info.id,
				0,
				0,
				info.name,
				info.leader_name);
		}

		private void HandleDonation(
			PlayerObject player,
			uint rawAmount,
			bool emoney)
		{
			if (!player.GetLegionSystem().IsHaveLegion() ||
				rawAmount == 0 ||
				rawAmount > int.MaxValue)
			{
				player.ChatNotice("Invalid legion donation.");
				return;
			}
			int amount = (int)rawAmount;
			MONEYTYPE moneyType =
				emoney ? MONEYTYPE.GAMEGOLD : MONEYTYPE.GOLD;
			if (player.GetMoneyCount(moneyType) < amount)
			{
				player.ChatNotice("You do not have enough currency.");
				return;
			}

			Legion legion = player.GetLegionSystem().GetLegion();
			LegionMember member =
				FindMember(legion.GetBaseInfo(), player.GetName());
			if (member == null)
			{
				player.ChatNotice("Your legion member record is missing.");
				return;
			}
			try
			{
				if (emoney)
				{
					member.emoney = checked(member.emoney + amount);
				}
				else
				{
					member.money = checked(member.money + amount);
					legion.GetBaseInfo().money = checked(
						legion.GetBaseInfo().money + amount);
				}
			}
			catch (OverflowException)
			{
				player.ChatNotice("The legion donation total is full.");
				return;
			}

			player.ChangeMoney(moneyType, -amount);
			member.boChange = true;
			UpdateLegionInfo(
				legion.GetBaseInfo().id,
				player.GetBaseAttr().player_id);
			player.GetLegionSystem().SendLegionInfo();
			player.ChatNotice(
				"Legion donation accepted: " + amount.ToString() + ".");
		}

		private void HandleKickDocumentQuery(PlayerObject player)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null || !CanManageMembers(player))
			{
				player.ChatNotice(
					"Only the legion leader can view removal records.");
				return;
			}
			SendSyndicate(
				player,
				110,
				legion.GetBaseInfo().id,
				0,
				0,
				GetKickDocuments(legion.GetBaseInfo().id).ToArray());
		}

		private void HandleKickDocumentDecision(
			PlayerObject player,
			string memberName,
			bool approve)
		{
			if (!CanManageMembers(player))
			{
				player.ChatNotice(
					"Only the legion leader can review removal requests.");
				return;
			}
			Legion legion = player.GetLegionSystem().GetLegion();
			List<string> documents = GetKickDocuments(
				legion.GetBaseInfo().id);
			if (!ContainsName(documents, memberName))
			{
				player.ChatNotice("That member-removal request no longer exists.");
				return;
			}
			RemoveKickDocument(legion.GetBaseInfo().id, memberName);
			if (!approve)
			{
				player.ChatNotice("The member-removal request was declined.");
				return;
			}
			LegionMember member = FindMember(
				legion.GetBaseInfo(), memberName);
			if (member == null)
			{
				player.ChatNotice("Legion member not found.");
				return;
			}
			KickMember(player, legion, member);
		}

		private void SendNoticeSnapshot(PlayerObject player)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null ||
				string.IsNullOrEmpty(legion.GetBaseInfo().notice))
			{
				return;
			}
			MsgTalkInfo notice = new MsgTalkInfo
			{
				rgba = 0xffffff,
				unTxtAttribute = 2111
			};
			notice.Create(null, player.GetGamePackKeyEx());
			notice.liststr.Add(legion.GetBaseInfo().leader_name);
			notice.liststr.Add("");
			notice.liststr.Add("");
			notice.liststr.Add(legion.GetBaseInfo().notice);
			player.SendData(notice.GetBuffer(), false);
			SendSyndicate(
				player,
				119,
				legion.GetBaseInfo().id,
				GetUnixTime(),
				0);
		}

		private void BroadcastNoticeTimestamp(Legion legion)
		{
			foreach (LegionMember member in legion.GetBaseInfo().list_member)
			{
				PlayerObject online =
					UserEngine.Instance().FindPlayerObjectToPlayerId(
						member.player_id);
				if (online != null)
				{
					SendSyndicate(
						online,
						119,
						legion.GetBaseInfo().id,
						GetUnixTime(),
						0);
				}
			}
		}

		private static uint GetUnixTime()
		{
			long seconds = (long)(
				DateTime.UtcNow -
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).
				TotalSeconds;
			return seconds <= 0 ? 0U : unchecked((uint)seconds);
		}

		private void HandleDirectRankChange(
			PlayerObject player,
			string memberName,
			uint rawRank)
		{
			if (!CanManageMembers(player) ||
				rawRank > short.MaxValue ||
				!IsSupportedRank((short)rawRank) ||
				rawRank == LeaderRank)
			{
				player.ChatNotice("Invalid legion rank change.");
				return;
			}
			ChangeRank(
				player,
				memberName,
				(short)rawRank,
				121);
		}

		private void HandleAppointmentRequest(
			PlayerObject player,
			uint targetId,
			uint rawRank)
		{
			if (!CanManageMembers(player) ||
				rawRank > short.MaxValue ||
				!IsSupportedRank((short)rawRank) ||
				rawRank == LeaderRank)
			{
				player.ChatNotice("Invalid legion appointment.");
				return;
			}
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null ||
				target.GetLegionSystem().GetLegion() !=
					player.GetLegionSystem().GetLegion())
			{
				player.ChatNotice("The selected legion member is not online.");
				return;
			}
			ReplaceRequest(
				pendingAppointments,
				player.GetTypeId(),
				target.GetTypeId(),
				(short)rawRank);
			SendSyndicate(
				target,
				120,
				player.GetTypeId(),
				rawRank,
				0,
				player.GetName());
			player.ChatNotice("Legion appointment sent.");
		}

		private void HandleAppointmentAcceptance(
			PlayerObject player,
			uint leaderId)
		{
			PendingRequest request = FindRequest(
				pendingAppointments, leaderId, player.GetTypeId());
			if (request == null)
			{
				player.ChatNotice("The legion appointment has expired.");
				return;
			}
			pendingAppointments.Remove(request);
			PlayerObject leader = FindOnlinePlayer(leaderId);
			if (leader == null ||
				leader.GetLegionSystem().GetLegion() !=
					player.GetLegionSystem().GetLegion() ||
				!CanManageMembers(leader))
			{
				player.ChatNotice("The legion appointment is no longer valid.");
				return;
			}
			if (ChangeRank(
				leader,
				player.GetName(),
				request.Rank,
				121))
			{
				player.ChatNotice("Legion appointment accepted.");
			}
		}

		private void HandleDismissal(
			PlayerObject player,
			SyndicateQueryPacket packet)
		{
			if (!CanManageMembers(player))
			{
				player.ChatNotice("Only the legion leader can dismiss officers.");
				return;
			}
			string memberName = FirstString(packet);
			PlayerObject target = null;
			if (packet.TargetId != 0)
			{
				target = FindOnlinePlayer(packet.TargetId);
				if (target != null)
				{
					memberName = target.GetName();
				}
			}
			if (ChangeRank(player, memberName, MemberRank, 125) &&
				target != null)
			{
				target.ChatNotice("Your legion appointment was dismissed.");
			}
		}

		private void HandleResignation(PlayerObject player)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null)
			{
				return;
			}
			if (legion.GetBaseInfo().leader_id ==
				player.GetBaseAttr().player_id)
			{
				player.ChatNotice("The legion leader cannot resign.");
				return;
			}
			ChangeRank(player, player.GetName(), MemberRank, 125, true);
		}

		private void HandleDelayedAppointment(
			PlayerObject player,
			uint targetId)
		{
			if (!CanManageMembers(player))
			{
				return;
			}
			PlayerObject target = FindOnlinePlayer(targetId);
			Legion legion = player.GetLegionSystem().GetLegion();
			LegionMember member = target == null ?
				null :
				FindMember(legion.GetBaseInfo(), target.GetName());
			if (member != null)
			{
				SendSyndicate(
					player,
					121,
					target.GetTypeId(),
					unchecked((uint)(ushort)member.rank),
					0,
					member.members_name);
			}
		}

		private void HandleTrustedAideRequest(
			PlayerObject player,
			uint targetId,
			uint rawRank)
		{
			if (!CanManageMembers(player) ||
				rawRank > short.MaxValue ||
				!IsSupportedRank((short)rawRank) ||
				rawRank == LeaderRank)
			{
				player.ChatNotice("Invalid trusted-aide appointment.");
				return;
			}
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null ||
				target.GetLegionSystem().GetLegion() !=
					player.GetLegionSystem().GetLegion())
			{
				player.ChatNotice("The selected legion member is not online.");
				return;
			}
			ReplaceRequest(
				pendingTrustedAppointments,
				player.GetTypeId(),
				target.GetTypeId(),
				(short)rawRank);
			SendSyndicate(
				target,
				137,
				player.GetTypeId(),
				rawRank,
				0,
				player.GetName());
			player.ChatNotice("Trusted-aide appointment sent.");
		}

		private void HandleTrustedAideAcceptance(
			PlayerObject player,
			uint leaderId)
		{
			PendingRequest request = FindRequest(
				pendingTrustedAppointments,
				leaderId,
				player.GetTypeId());
			if (request == null)
			{
				player.ChatNotice(
					"The trusted-aide appointment has expired.");
				return;
			}
			pendingTrustedAppointments.Remove(request);
			PlayerObject leader = FindOnlinePlayer(leaderId);
			if (leader == null ||
				leader.GetLegionSystem().GetLegion() !=
					player.GetLegionSystem().GetLegion() ||
				!CanManageMembers(leader))
			{
				player.ChatNotice(
					"The trusted-aide appointment is no longer valid.");
				return;
			}
			if (!ChangeRank(
				leader, player.GetName(), request.Rank, 121))
			{
				return;
			}
			string rank = request.Rank.ToString(CultureInfo.InvariantCulture);
			SendSyndicate(
				leader,
				138,
				leader.GetTypeId(),
				player.GetTypeId(),
				0,
				rank);
			SendSyndicate(
				player,
				138,
				leader.GetTypeId(),
				player.GetTypeId(),
				0,
				rank);
			player.ChatNotice("Trusted-aide appointment accepted.");
		}

		private void HandleTrustedAideRefusal(
			PlayerObject player,
			uint leaderId)
		{
			PendingRequest request = FindRequest(
				pendingTrustedAppointments,
				leaderId,
				player.GetTypeId());
			if (request != null)
			{
				pendingTrustedAppointments.Remove(request);
			}
			PlayerObject leader = FindOnlinePlayer(leaderId);
			if (leader != null)
			{
				leader.ChatNotice(
					player.GetName() +
					" declined the trusted-aide appointment.");
			}
		}

		private void HandleTrustedAideDismissal(
			PlayerObject player,
			uint targetId)
		{
			if (!CanManageMembers(player))
			{
				return;
			}
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null ||
				target.GetLegionSystem().GetLegion() !=
					player.GetLegionSystem().GetLegion())
			{
				player.ChatNotice("The selected legion member is not online.");
				return;
			}
			ChangeRank(player, target.GetName(), MemberRank, 140);
		}

		private void HandleTrustedAideResignation(PlayerObject player)
		{
			Legion legion = player.GetLegionSystem().GetLegion();
			if (legion == null ||
				legion.GetBaseInfo().leader_id ==
					player.GetBaseAttr().player_id)
			{
				return;
			}
			ChangeRank(player, player.GetName(), MemberRank, 141, true);
		}

		private void HandleNoBotherReply(
			PlayerObject player,
			uint targetId,
			ushort action)
		{
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target != null)
			{
				SendSyndicate(
					target,
					action,
					player.GetTypeId(),
					0,
					0);
			}
		}

		private bool ChangeRank(
			PlayerObject actor,
			string memberName,
			short rank,
			ushort responseAction,
			bool allowSelf = false)
		{
			Legion legion = actor.GetLegionSystem().GetLegion();
			if (legion == null || string.IsNullOrEmpty(memberName))
			{
				return false;
			}
			if (!allowSelf && !CanManageMembers(actor))
			{
				return false;
			}
			LegionMember member =
				FindMember(legion.GetBaseInfo(), memberName);
			if (member == null ||
				member.player_id == legion.GetBaseInfo().leader_id)
			{
				actor.ChatNotice("That legion rank cannot be changed.");
				return false;
			}
			member.rank = rank;
			member.boChange = true;
			UpdateLegionInfo(
				legion.GetBaseInfo().id,
				actor.GetBaseAttr().player_id);
			PlayerObject target =
				UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.player_id);
			uint targetId = target == null ?
				unchecked((uint)member.player_id) :
				target.GetTypeId();
			SendSyndicate(
				actor,
				responseAction,
				targetId,
				unchecked((uint)(ushort)rank),
				0,
				member.members_name);
			if (target != null && target != actor)
			{
				target.GetLegionSystem().SendLegionInfo();
				SendSyndicate(
					target,
					responseAction,
					targetId,
					unchecked((uint)(ushort)rank),
					0,
					member.members_name);
			}
			return true;
		}

		private bool RemoveMember(
			Legion legion,
			string memberName,
			PlayerObject online,
			bool kicked)
		{
			LegionInfo info = legion.GetBaseInfo();
			LegionMember member = FindMember(info, memberName);
			if (member == null)
			{
				return false;
			}
			info.list_member.Remove(member);
			if (online != null)
			{
				online.GetLegionSystem().SetLegion(null, true);
				online.ChatNotice(
					kicked ?
						"You were removed from " + info.name + "." :
						"You left " + info.name + ".");
			}
			UpdateLegionInfo(info.id, member.player_id);
			return true;
		}

		private void DeleteLegion(Legion legion, PlayerObject leader)
		{
			LegionInfo info = legion.GetBaseInfo();
			mDicLegion.Remove(info.id);
			pendingKickDocuments.Remove(info.id);
			leader.GetLegionSystem().SetLegion(null, true);
			LegionOption option = new LegionOption();
			option.SetDeleteTag();
			option.player_id = leader.GetBaseAttr().player_id;
			option.mInfo = info;
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
			leader.ChatNotice("Legion disbanded.");
		}

		private static LegionMember FindMember(
			LegionInfo info,
			string playerName)
		{
			if (info == null || string.IsNullOrEmpty(playerName))
			{
				return null;
			}
			for (int index = 0; index < info.list_member.Count; index++)
			{
				if (string.Equals(
					info.list_member[index].members_name,
					playerName,
					StringComparison.OrdinalIgnoreCase))
				{
					return info.list_member[index];
				}
			}
			return null;
		}

		private List<string> GetKickDocuments(uint legionId)
		{
			List<string> documents;
			if (!pendingKickDocuments.TryGetValue(
				legionId, out documents))
			{
				documents = new List<string>();
				pendingKickDocuments[legionId] = documents;
			}
			return documents;
		}

		private static bool ContainsName(
			List<string> names,
			string playerName)
		{
			for (int index = 0; index < names.Count; index++)
			{
				if (string.Equals(
					names[index],
					playerName,
					StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private void RemoveKickDocument(
			uint legionId,
			string playerName)
		{
			List<string> documents = GetKickDocuments(legionId);
			for (int index = documents.Count - 1; index >= 0; index--)
			{
				if (string.Equals(
					documents[index],
					playerName,
					StringComparison.OrdinalIgnoreCase))
				{
					documents.RemoveAt(index);
				}
			}
		}

		private static bool CanManageMembers(PlayerObject player)
		{
			return player != null &&
				player.GetLegionSystem().IsLeader();
		}

		private static bool IsSupportedRank(short rank)
		{
			switch (rank)
			{
			case MemberRank:
			case 510:
			case 511:
			case 610:
			case 611:
			case 840:
			case 850:
			case 880:
			case 890:
			case 920:
			case 980:
			case DeputyLeaderRank:
			case LeaderRank:
				return true;
			default:
				return false;
			}
		}

		private static string FirstString(SyndicateQueryPacket packet)
		{
			return packet.Strings == null || packet.Strings.Length == 0 ?
				"" :
				packet.Strings[0];
		}

		private static PlayerObject FindOnlinePlayer(uint id)
		{
			PlayerObject player =
				UserEngine.Instance().FindPlayerObjectToTypeID(id);
			if (player == null && id <= int.MaxValue)
			{
				player = UserEngine.Instance().FindPlayerObjectToPlayerId(
					(int)id);
			}
			return player;
		}

		private static void SendSyndicate(
			PlayerObject player,
			ushort action,
			uint targetId,
			uint fealtyId,
			byte level,
			params string[] strings)
		{
			player.SendData(
				MapPacketCodec.CreateSyndicateResponse(
					player.GetGamePackKeyEx(),
					action,
					targetId,
					fealtyId,
					level,
					strings),
				false);
		}

		private void ReplaceRequest(
			List<PendingRequest> requests,
			uint leaderTypeId,
			uint memberTypeId,
			short rank)
		{
			PendingRequest existing =
				FindRequest(requests, leaderTypeId, memberTypeId);
			if (existing != null)
			{
				requests.Remove(existing);
			}
			requests.Add(new PendingRequest
			{
				LeaderTypeId = leaderTypeId,
				MemberTypeId = memberTypeId,
				Rank = rank,
				CreatedAt = Environment.TickCount
			});
		}

		private static PendingRequest FindRequest(
			List<PendingRequest> requests,
			uint leaderTypeId,
			uint memberTypeId)
		{
			for (int index = 0; index < requests.Count; index++)
			{
				PendingRequest request = requests[index];
				if (request.LeaderTypeId == leaderTypeId &&
					request.MemberTypeId == memberTypeId)
				{
					return request;
				}
			}
			return null;
		}

		private void RemoveAppointmentFor(uint memberTypeId)
		{
			for (int index = pendingAppointments.Count - 1;
				index >= 0;
				index--)
			{
				if (pendingAppointments[index].MemberTypeId == memberTypeId)
				{
					pendingAppointments.RemoveAt(index);
				}
			}
		}

		private void ExpireRequests()
		{
			ExpireRequests(pendingJoins);
			ExpireRequests(pendingInvites);
			ExpireRequests(pendingAppointments);
			ExpireRequests(pendingTrustedAppointments);
		}

		private static void ExpireRequests(List<PendingRequest> requests)
		{
			int now = Environment.TickCount;
			for (int index = requests.Count - 1; index >= 0; index--)
			{
				if (unchecked(now - requests[index].CreatedAt) >
					RequestLifetimeMilliseconds)
				{
					requests.RemoveAt(index);
				}
			}
		}

		private static LegionManager mInstance;
		private readonly Dictionary<uint, Legion> mDicLegion;
		private readonly Dictionary<int, PendingCreation> pendingCreations;
		private readonly List<PendingRequest> pendingJoins;
		private readonly List<PendingRequest> pendingInvites;
		private readonly List<PendingRequest> pendingAppointments;
		private readonly List<PendingRequest> pendingTrustedAppointments;
		private readonly Dictionary<uint, List<string>>
			pendingKickDocuments;
	}
}
