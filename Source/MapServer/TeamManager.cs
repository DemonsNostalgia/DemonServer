using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameStruct;

namespace MapServer
{
	public class TeamManager
	{
		private sealed class PendingRequest
		{
			public uint LeaderId;
			public uint MemberId;
			public DateTime ExpiresUtc;
		}

		public static TeamManager Instance()
		{
			if (mInstance == null)
			{
				mInstance = new TeamManager();
			}
			return mInstance;
		}

		public TeamManager()
		{
			mID = 1000U;
			mListTeam = new List<Team>();
			applications = new List<PendingRequest>();
			invitations = new List<PendingRequest>();
		}

		public Team CreateTeam()
		{
			mID += 1U;
			Team team = new Team(mID);
			mListTeam.Add(team);
			return team;
		}

		public void DeleteTeam(uint teamid)
		{
			for (int index = mListTeam.Count - 1; index >= 0; index--)
			{
				if (mListTeam[index].GetTeamID() == teamid)
				{
					mListTeam.RemoveAt(index);
				}
			}
		}

		public void HandlePacket(PlayerObject player, TeamActionPacket packet)
		{
			if (player == null || packet == null)
			{
				return;
			}
			ExpireRequests();
			switch (packet.Action)
			{
			case 0:
				Create(player, packet.TargetId);
				break;
			case 1:
				Apply(player, packet.TargetId);
				break;
			case 2:
				Leave(player, packet.TargetId);
				break;
			case 3:
				AcceptInvitation(player, packet.TargetId);
				break;
			case 4:
				Invite(player, packet.TargetId);
				break;
			case 5:
				AgreeApplication(player, packet.TargetId);
				break;
			default:
				Log.Instance().WriteLog(
					"Rejected unsupported team action " +
					packet.Action.ToString() + " from role " +
					player.GetTypeId().ToString() + ".");
				break;
			}
		}

		public void RemoveMember(Team team, PlayerObject player)
		{
			if (team == null || player == null || !team.Contains(player))
			{
				return;
			}

			if (team.GetCaptain() == player)
			{
				PlayerObject[] members = team.GetMembers();
				for (int index = 0; index < members.Length; index++)
				{
					SendTeamAction(members[index], 6, player.GetTypeId());
				}
				RemoveRequestsFor(player.GetTypeId());
				team.DeleteTeam();
				return;
			}

			PlayerObject[] recipients = team.GetMembers();
			for (int index = 0; index < recipients.Length; index++)
			{
				SendTeamAction(recipients[index], 2, player.GetTypeId());
			}
			team.RemoveMemberWithoutNotification(player);
			if (team.GetMemberCount() == 0)
			{
				team.DeleteTeam();
			}
			RemoveRequestsFor(player.GetTypeId());
		}

		private void Create(PlayerObject player, uint claimedRoleId)
		{
			if (claimedRoleId != player.GetTypeId())
			{
				Log.Instance().WriteLog(
					"Rejected spoofed team-create role ID " +
					claimedRoleId.ToString() + " from role " +
					player.GetTypeId().ToString() + ".");
				return;
			}
			if (player.GetTeam() != null)
			{
				player.ChatNotice("You already belong to a team.");
				return;
			}

			Team team = CreateTeam();
			if (!team.AddMember(player))
			{
				DeleteTeam(team.GetTeamID());
				player.ChatNotice("The team could not be created.");
				return;
			}
			player.SendData(
				MapPacketCodec.CreateTeamCreatedResponse(
					null, team.GetCreateTime()), true);
		}

		private void Invite(PlayerObject leader, uint targetId)
		{
			Team team = leader.GetTeam();
			if (!IsCaptain(team, leader))
			{
				leader.ChatNotice("Only the team captain can invite members.");
				return;
			}
			if (team.IsTeamFull())
			{
				leader.ChatNotice("The team is full.");
				return;
			}

			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null || target == leader)
			{
				leader.ChatNotice("The invited player is not available.");
				return;
			}
			if (target.GetTeam() != null)
			{
				leader.ChatNotice("The invited player already belongs to a team.");
				return;
			}

			ReplaceRequest(invitations, leader.GetTypeId(), target.GetTypeId());
			PlayerAttribute attributes = leader.GetBaseAttr();
			target.SendData(
				MapPacketCodec.CreateTeamInvitationResponse(
					null,
					leader.GetTypeId(),
					ComposeLook(attributes),
					attributes.life_max,
					leader.GetSex(),
					attributes.level,
					attributes.profession,
					leader.GetName()),
				true);
		}

		private void AcceptInvitation(PlayerObject member, uint leaderId)
		{
			PendingRequest request = FindRequest(
				invitations, leaderId, member.GetTypeId());
			if (request == null)
			{
				member.ChatNotice("The team invitation is no longer valid.");
				return;
			}
			invitations.Remove(request);
			PlayerObject leader = FindOnlinePlayer(leaderId);
			Join(leader, member);
		}

		private void Apply(PlayerObject member, uint leaderId)
		{
			if (member.GetTeam() != null)
			{
				member.ChatNotice("You already belong to a team.");
				return;
			}
			PlayerObject leader = FindOnlinePlayer(leaderId);
			Team team = leader == null ? null : leader.GetTeam();
			if (!IsCaptain(team, leader) || team.IsTeamFull())
			{
				member.ChatNotice("That team is not accepting applications.");
				return;
			}

			ReplaceRequest(applications, leader.GetTypeId(), member.GetTypeId());
			SendTeamAction(leader, 1, member.GetTypeId());
		}

		private void AgreeApplication(PlayerObject leader, uint memberId)
		{
			PendingRequest request = FindRequest(
				applications, leader.GetTypeId(), memberId);
			if (request == null)
			{
				leader.ChatNotice("The team application is no longer valid.");
				return;
			}
			applications.Remove(request);
			Join(leader, FindOnlinePlayer(memberId));
		}

		private void Join(PlayerObject leader, PlayerObject member)
		{
			Team team = leader == null ? null : leader.GetTeam();
			if (member == null || !IsCaptain(team, leader) ||
				member.GetTeam() != null || team.IsTeamFull())
			{
				if (leader != null)
				{
					leader.ChatNotice("The player could not join the team.");
				}
				if (member != null)
				{
					member.ChatNotice("The team is no longer available.");
				}
				return;
			}

			PlayerObject[] existingMembers = team.GetMembers();
			if (!team.AddMember(member))
			{
				return;
			}

			TeamMemberPacketRecord joinedRecord = BuildMemberRecord(team, member);
			for (int index = 0; index < existingMembers.Length; index++)
			{
				SendMemberRecords(existingMembers[index], 0, joinedRecord);
			}

			PlayerObject[] allMembers = team.GetMembers();
			TeamMemberPacketRecord[] snapshot =
				new TeamMemberPacketRecord[allMembers.Length];
			for (int index = 0; index < allMembers.Length; index++)
			{
				snapshot[index] = BuildMemberRecord(team, allMembers[index]);
			}
			member.SendData(
				MapPacketCodec.CreateTeamMemberResponse(null, 2, snapshot), true);
			RemoveRequestsFor(member.GetTypeId());
		}

		private void Leave(PlayerObject player, uint claimedRoleId)
		{
			if (claimedRoleId != player.GetTypeId())
			{
				Log.Instance().WriteLog(
					"Rejected spoofed team-leave role ID " +
					claimedRoleId.ToString() + " from role " +
					player.GetTypeId().ToString() + ".");
				return;
			}
			RemoveMember(player.GetTeam(), player);
		}

		private static bool IsCaptain(Team team, PlayerObject player)
		{
			return team != null && player != null && team.GetCaptain() == player;
		}

		private static PlayerObject FindOnlinePlayer(uint id)
		{
			PlayerObject player = UserEngine.Instance().FindPlayerObjectToTypeID(id);
			return player ?? UserEngine.Instance().FindPlayerObjectToPlayerId(
				unchecked((int)id));
		}

		private static void SendTeamAction(
			PlayerObject player,
			ushort action,
			uint targetId)
		{
			if (player != null)
			{
				player.SendData(
					MapPacketCodec.CreateTeamActionResponse(
						null, action, targetId), true);
			}
		}

		private static void SendMemberRecords(
			PlayerObject player,
			byte action,
			params TeamMemberPacketRecord[] records)
		{
			player.SendData(
				MapPacketCodec.CreateTeamMemberResponse(null, action, records), true);
		}

		private static TeamMemberPacketRecord BuildMemberRecord(
			Team team,
			PlayerObject player)
		{
			PlayerAttribute attributes = player.GetBaseAttr();
			Legion legion = player.GetLegionSystem().GetLegion();
			short legionRank = player.GetLegionSystem().GetPlace();
			return new TeamMemberPacketRecord
			{
				Name = player.GetName(),
				RoleId = player.GetTypeId(),
				Look = ComposeLook(attributes),
				Life = ClampUInt16(attributes.life),
				MaximumLife = ClampUInt16(attributes.life_max),
				Profession = attributes.profession,
				Level = attributes.level,
				SyndicateName = legion == null ? "" : legion.GetBaseInfo().name,
				SyndicateRank = legionRank <= 0 ? (byte)0 :
					unchecked((byte)Math.Min(byte.MaxValue, legionRank)),
				TeamCreateTime = team.GetCreateTime(),
				X = ClampUInt16(unchecked((uint)Math.Max(
					0, (int)player.GetCurrentX()))),
				Y = ClampUInt16(unchecked((uint)Math.Max(
					0, (int)player.GetCurrentY()))),
				IsOnline = player.GetGameSession() != null
			};
		}

		private static uint ComposeLook(PlayerAttribute attributes)
		{
			return unchecked(
				attributes.lookface * 10000U + attributes.hair % 1000U);
		}

		private static ushort ClampUInt16(uint value)
		{
			return unchecked((ushort)Math.Min(ushort.MaxValue, value));
		}

		private void ReplaceRequest(
			List<PendingRequest> list,
			uint leaderId,
			uint memberId)
		{
			PendingRequest existing = FindRequest(list, leaderId, memberId);
			if (existing != null)
			{
				list.Remove(existing);
			}
			list.Add(new PendingRequest
			{
				LeaderId = leaderId,
				MemberId = memberId,
				ExpiresUtc = DateTime.UtcNow.AddSeconds(60)
			});
		}

		private static PendingRequest FindRequest(
			List<PendingRequest> list,
			uint leaderId,
			uint memberId)
		{
			for (int index = 0; index < list.Count; index++)
			{
				if (list[index].LeaderId == leaderId &&
					list[index].MemberId == memberId)
				{
					return list[index];
				}
			}
			return null;
		}

		private void ExpireRequests()
		{
			ExpireRequests(applications);
			ExpireRequests(invitations);
		}

		private static void ExpireRequests(List<PendingRequest> list)
		{
			for (int index = list.Count - 1; index >= 0; index--)
			{
				if (list[index].ExpiresUtc <= DateTime.UtcNow)
				{
					list.RemoveAt(index);
				}
			}
		}

		private void RemoveRequestsFor(uint roleId)
		{
			RemoveRequestsFor(applications, roleId);
			RemoveRequestsFor(invitations, roleId);
		}

		private static void RemoveRequestsFor(
			List<PendingRequest> list,
			uint roleId)
		{
			for (int index = list.Count - 1; index >= 0; index--)
			{
				if (list[index].LeaderId == roleId ||
					list[index].MemberId == roleId)
				{
					list.RemoveAt(index);
				}
			}
		}

		private static TeamManager mInstance;
		private uint mID;
		private readonly List<Team> mListTeam;
		private readonly List<PendingRequest> applications;
		private readonly List<PendingRequest> invitations;
	}
}
