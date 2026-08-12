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
	public sealed class FamilyManager
	{
		public const ushort LeaderRank = 100;
		public const ushort SpouseRank = 50;
		public const ushort MemberRank = 10;
		public const int MemberLimit = 12;
		public const int MinimumCreationLevel = 50;
		public const int CreationCost = 500000;

		private sealed class PendingCreation
		{
			public FamilyInfo Info;
			public int Cost;
		}

		private sealed class PendingRequest
		{
			public uint LeaderTypeId;
			public uint MemberTypeId;
			public DateTime ExpiresUtc;
		}

		private FamilyManager()
		{
			families = new Dictionary<uint, FamilyInfo>();
			pendingCreations = new Dictionary<int, PendingCreation>();
			applications = new List<PendingRequest>();
			invitations = new List<PendingRequest>();
		}

		public static FamilyManager Instance()
		{
			if (instance == null)
			{
				instance = new FamilyManager();
			}
			return instance;
		}

		public void DB_Load(FamilyCollection collection)
		{
			families.Clear();
			if (collection != null)
			{
				for (int index = 0; index < collection.Families.Count; index++)
				{
					FamilyInfo family = collection.Families[index];
					if (family.Deleted == 0)
					{
						families[family.Id] = family;
					}
				}
			}
			Log.Instance().WriteLog(
				"Loaded " + families.Count.ToString() +
				" family record(s) from DBServer.");
		}

		public FamilyInfo GetFamily(uint familyId)
		{
			FamilyInfo family;
			return families.TryGetValue(familyId, out family) ? family : null;
		}

		public FamilyInfo GetPlayerFamily(int playerId)
		{
			foreach (FamilyInfo family in families.Values)
			{
				if (FindMember(family, playerId) != null)
				{
					return family;
				}
			}
			return null;
		}

		public bool IsExist(string familyName)
		{
			foreach (FamilyInfo family in families.Values)
			{
				if (string.Equals(
					family.Name, familyName, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public bool CreateFamily(PlayerObject player, string familyName)
		{
			if (player == null || string.IsNullOrWhiteSpace(familyName))
			{
				return false;
			}
			familyName = familyName.Trim();
			int encodedLength = Coding.GetDefauleCoding().GetByteCount(familyName);
			if (encodedLength < 1 || encodedLength > 15)
			{
				player.ChatNotice("Family names must be between 1 and 15 bytes.");
				return false;
			}
			if (player.GetFamilySystem().IsHaveFamily())
			{
				player.ChatNotice("You already belong to a family.");
				return false;
			}
			if (pendingCreations.ContainsKey(player.GetBaseAttr().player_id))
			{
				player.ChatNotice("Your family creation request is still pending.");
				return false;
			}
			if (IsExist(familyName))
			{
				player.ChatNotice("That family name is already in use.");
				return false;
			}
			if (player.GetLevel() < MinimumCreationLevel)
			{
				player.ChatNotice("You must be level 50 to create a family.");
				return false;
			}
			if (player.GetMoneyCount(MONEYTYPE.GOLD) < CreationCost)
			{
				player.ChatNotice("Creating a family costs 500,000 gold.");
				return false;
			}
			if (!DBServer.Instance().IsConnect())
			{
				player.ChatNotice("Family creation is unavailable while the database is offline.");
				return false;
			}

			uint now = GetUnixTime();
			FamilyInfo info = new FamilyInfo
			{
				Name = familyName,
				Rank = 1,
				LeaderId = player.GetBaseAttr().player_id,
				LeaderName = player.GetName(),
				Announcement = "Welcome to " + familyName + ".",
				CreateDate = now,
				CreateName = player.GetName()
			};
			info.Members.Add(new FamilyMember
			{
				PlayerId = player.GetBaseAttr().player_id,
				Name = player.GetName(),
				Rank = LeaderRank,
				JoinDate = now
			});
			pendingCreations[player.GetBaseAttr().player_id] =
				new PendingCreation { Info = info, Cost = CreationCost };
			player.ChangeMoney(MONEYTYPE.GOLD, -CreationCost);

			FamilyOption option = new FamilyOption();
			option.SetCreateTag();
			option.PlayerId = player.GetBaseAttr().player_id;
			option.Info = info;
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
			return true;
		}

		public void HandleCreateResult(CreateFamilyResult result)
		{
			if (result == null)
			{
				return;
			}
			PendingCreation pending;
			if (!pendingCreations.TryGetValue(result.PlayerId, out pending))
			{
				Log.Instance().WriteLog(
					"Ignored unmatched family creation response for player " +
					result.PlayerId.ToString() + ".");
				return;
			}
			pendingCreations.Remove(result.PlayerId);
			PlayerObject player =
				UserEngine.Instance().FindPlayerObjectToPlayerId(result.PlayerId);
			if (result.Success == 0 || result.FamilyId <= 0)
			{
				if (player != null)
				{
					player.ChangeMoney(MONEYTYPE.GOLD, pending.Cost);
					player.ChatNotice(
						"Family creation failed; the creation fee was refunded.");
				}
				return;
			}

			pending.Info.Id = checked((uint)result.FamilyId);
			families[pending.Info.Id] = pending.Info;
			if (player != null)
			{
				player.GetFamilySystem().SetMembership(
					pending.Info, pending.Info.Members[0], true);
				player.ChatNotice("Family created.");
			}
		}

		public void HandlePacket(PlayerObject player, FamilyQueryPacket packet)
		{
			ExpireRequests();
			switch (packet.Action)
			{
			case 2:
				ApplyToFamily(player, packet.ValueA);
				break;
			case 3:
				DecideApplication(player, packet.ValueA, packet.ValueB != 0);
				break;
			case 4:
				InviteToFamily(player, packet.ValueA);
				break;
			case 5:
				DecideInvitation(player, packet.ValueA, packet.ValueB != 0);
				break;
			case 6:
				LeaveFamily(player);
				break;
			case 7:
				KickMember(player, packet.ValueA);
				break;
			case 8:
				DisbandFamily(player);
				break;
			case 10:
				SendSnapshot(player);
				break;
			case 11:
				SendMemberList(player);
				break;
			case 13:
				SendMemberDetail(player, packet.ValueA);
				break;
			case 15:
				SendBasicInfo(player, packet.ValueB);
				break;
			case 20:
				SendAnnouncement(player);
				break;
			case 21:
				TransferLeadership(player, packet.ValueA);
				break;
			case 22:
				Donate(player, packet.ValueB);
				break;
			case 24:
				OpenOccupyDialog(player, packet.ValueA);
				break;
			case 25:
				SendOccupyQuery(player, packet.ValueA);
				break;
			case 26:
			case 27:
			case 28:
			case 29:
			case 30:
			case 32:
			case 34:
				player.ChatNotice(
					"Family battle and territory events are not currently active.");
				break;
			default:
				Log.Instance().WriteLog(
					"Unsupported family action " + packet.Action.ToString() +
					" from " + player.GetName() + ".");
				break;
			}
		}

		public void SendSnapshot(PlayerObject player)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			FamilyMember member = player.GetFamilySystem().GetMember();
			if (family == null || member == null)
			{
				SendFamilyPacket(player, 1, player.GetTypeId(), 0, 0);
				return;
			}

			SendFamilyPacket(
				player, 1, player.GetTypeId(), family.Id, member.Rank);
			SendFamilyPacket(
				player, 16, player.GetTypeId(), family.Id, member.Rank,
				family.Name);
			player.SendData(
				MapPacketCodec.CreateFamilyAttributeResponse(
					null, BuildAttributeRecord(family, member)), true);
			player.SendData(
				MapPacketCodec.CreateFamilyRelationResponse(
					null, 2, family.Id, family.AllyIds), true);
			player.SendData(
				MapPacketCodec.CreateFamilyRelationResponse(
					null, 3, family.Id, family.EnemyIds), true);
			SendAnnouncement(player);
		}

		public void UpdateAnnouncement(
			PlayerObject player,
			string announcement,
			byte[] originalPayload)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			if (family == null || !player.GetFamilySystem().IsLeader())
			{
				player.ChatNotice(
					"Only the family leader can change the announcement.");
				return;
			}
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(announcement ?? "");
			if (encoded.Length < 1 || encoded.Length > 127)
			{
				player.ChatNotice(
					"Family announcements must be between 1 and 127 bytes.");
				return;
			}
			if (!CanPersist(player))
			{
				return;
			}
			family.Announcement = announcement;
			SaveFamily(family, player.GetBaseAttr().player_id);
			UserEngine.Instance().BroadcastFamilyPayload(
				player, family, originalPayload);
		}

		public void RefreshVisibleFamilyState(PlayerObject player)
		{
			if (player == null)
			{
				return;
			}
			List<RefreshObject> visible =
				new List<RefreshObject>(player.GetVisibleList().Values);
			for (int index = 0; index < visible.Count; index++)
			{
				PlayerObject viewer = visible[index].obj as PlayerObject;
				if (viewer != null && viewer.GetGameSession() != null)
				{
					viewer.SendRoleInfo(player);
				}
			}
		}

		public static FamilyMember FindMember(FamilyInfo family, int playerId)
		{
			if (family == null)
			{
				return null;
			}
			for (int index = 0; index < family.Members.Count; index++)
			{
				if (family.Members[index].PlayerId == playerId)
				{
					return family.Members[index];
				}
			}
			return null;
		}

		private void ApplyToFamily(PlayerObject applicant, uint targetId)
		{
			if (applicant.GetFamilySystem().IsHaveFamily())
			{
				applicant.ChatNotice("You already belong to a family.");
				return;
			}
			PlayerObject selected = FindOnlinePlayer(targetId);
			FamilyInfo family = selected == null ? null :
				selected.GetFamilySystem().GetFamily();
			if (family == null)
			{
				applicant.ChatNotice("The selected player does not belong to a family.");
				return;
			}
			if (family.Members.Count >= MemberLimit)
			{
				applicant.ChatNotice("That family already has 12 members.");
				return;
			}
			PlayerObject leader =
				UserEngine.Instance().FindPlayerObjectToPlayerId(family.LeaderId);
			if (leader == null)
			{
				applicant.ChatNotice("The family leader is not online.");
				return;
			}
			ReplaceRequest(applications, leader.GetTypeId(), applicant.GetTypeId());
			SendFamilyPacket(
				leader, 4, applicant.GetTypeId(), family.Id, 0,
				string.Format(
					CultureInfo.InvariantCulture,
					"{0} {1} {2}", family.Name,
					applicant.GetLevel(), applicant.GetBaseAttr().profession));
			applicant.ChatNotice("Family application sent.");
		}

		private void DecideApplication(
			PlayerObject leader,
			uint applicantId,
			bool accept)
		{
			PendingRequest request = FindRequest(
				applications, leader.GetTypeId(), applicantId);
			if (request == null || !leader.GetFamilySystem().IsLeader())
			{
				leader.ChatNotice("That family application is no longer valid.");
				return;
			}
			applications.Remove(request);
			PlayerObject applicant = FindOnlinePlayer(applicantId);
			if (applicant == null)
			{
				leader.ChatNotice("The applicant is no longer online.");
				return;
			}
			if (!accept)
			{
				SendFamilyPacket(applicant, 3, leader.GetTypeId(), 0, 0);
				applicant.ChatNotice("Your family application was declined.");
				return;
			}
			if (AddMember(leader.GetFamilySystem().GetFamily(), applicant))
			{
				SendFamilyPacket(
					applicant, 3, leader.GetTypeId(),
					leader.GetFamilySystem().GetFamily().Id, 1);
				leader.ChatNotice(applicant.GetName() + " joined the family.");
			}
		}

		private void InviteToFamily(PlayerObject leader, uint targetId)
		{
			if (!leader.GetFamilySystem().IsLeader())
			{
				leader.ChatNotice("Only the family leader can invite members.");
				return;
			}
			FamilyInfo family = leader.GetFamilySystem().GetFamily();
			if (family.Members.Count >= MemberLimit)
			{
				leader.ChatNotice("Your family already has 12 members.");
				return;
			}
			PlayerObject target = FindOnlinePlayer(targetId);
			if (target == null || target.GetFamilySystem().IsHaveFamily())
			{
				leader.ChatNotice("The selected player cannot be invited.");
				return;
			}
			ReplaceRequest(invitations, leader.GetTypeId(), target.GetTypeId());
			SendFamilyPacket(
				target, 2, leader.GetTypeId(), family.Id, 0);
			leader.ChatNotice("Family invitation sent.");
		}

		private void DecideInvitation(
			PlayerObject target,
			uint leaderId,
			bool accept)
		{
			PendingRequest request = FindRequest(
				invitations, leaderId, target.GetTypeId());
			if (request == null)
			{
				target.ChatNotice("That family invitation is no longer valid.");
				return;
			}
			invitations.Remove(request);
			PlayerObject leader = FindOnlinePlayer(leaderId);
			if (leader == null || !leader.GetFamilySystem().IsLeader())
			{
				target.ChatNotice("The family leader is no longer available.");
				return;
			}
			if (!accept)
			{
				SendFamilyPacket(leader, 5, target.GetTypeId(), 0, 0);
				leader.ChatNotice(target.GetName() + " declined the invitation.");
				return;
			}
			if (AddMember(leader.GetFamilySystem().GetFamily(), target))
			{
				SendFamilyPacket(
					leader, 5, target.GetTypeId(),
					leader.GetFamilySystem().GetFamily().Id, 1);
			}
		}

		private bool AddMember(FamilyInfo family, PlayerObject player)
		{
			if (family == null || player == null ||
				player.GetFamilySystem().IsHaveFamily() ||
				family.Members.Count >= MemberLimit ||
				!CanPersist(player))
			{
				return false;
			}
			FamilyMember member = new FamilyMember
			{
				PlayerId = player.GetBaseAttr().player_id,
				Name = player.GetName(),
				Rank = MemberRank,
				JoinDate = GetUnixTime()
			};
			family.Members.Add(member);
			player.GetFamilySystem().SetMembership(family, member, true);
			SaveFamily(family, member.PlayerId);
			NotifyFamilyChanged(family);
			player.ChatNotice("You joined " + family.Name + ".");
			return true;
		}

		private void LeaveFamily(PlayerObject player)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			FamilyMember member = player.GetFamilySystem().GetMember();
			if (family == null || member == null)
			{
				return;
			}
			if (player.GetFamilySystem().IsLeader())
			{
				player.ChatNotice(
					"Transfer leadership or disband the family before leaving.");
				return;
			}
			if (!CanPersist(player))
			{
				return;
			}
			RemoveMember(family, member, player);
		}

		private void KickMember(PlayerObject leader, uint memberId)
		{
			FamilyInfo family = leader.GetFamilySystem().GetFamily();
			if (family == null || !leader.GetFamilySystem().IsLeader())
			{
				leader.ChatNotice("Only the family leader can remove members.");
				return;
			}
			FamilyMember member = ResolveMember(family, memberId);
			if (member == null || member.PlayerId == family.LeaderId)
			{
				leader.ChatNotice("Family member not found.");
				return;
			}
			if (!CanPersist(leader))
			{
				return;
			}
			PlayerObject online =
				UserEngine.Instance().FindPlayerObjectToPlayerId(member.PlayerId);
			RemoveMember(family, member, online);
			leader.ChatNotice(member.Name + " was removed from the family.");
		}

		private void RemoveMember(
			FamilyInfo family,
			FamilyMember member,
			PlayerObject online)
		{
			uint clientId = online == null ?
				unchecked((uint)member.PlayerId) : online.GetTypeId();
			family.Members.Remove(member);
			SaveFamily(family, member.PlayerId);
			NotifyMemberRemoved(family, clientId);
			if (online != null)
			{
				online.GetFamilySystem().SetMembership(null, null, true);
				online.ChatNotice("You left " + family.Name + ".");
			}
			NotifyFamilyChanged(family);
		}

		private void DisbandFamily(PlayerObject leader)
		{
			FamilyInfo family = leader.GetFamilySystem().GetFamily();
			if (family == null || !leader.GetFamilySystem().IsLeader())
			{
				leader.ChatNotice("Only the family leader can disband the family.");
				return;
			}
			if (!CanPersist(leader))
			{
				return;
			}
			List<FamilyMember> members = new List<FamilyMember>(family.Members);
			FamilyOption option = new FamilyOption();
			option.SetDeleteTag();
			option.PlayerId = leader.GetBaseAttr().player_id;
			option.Info = family;
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
			families.Remove(family.Id);
			for (int index = 0; index < members.Count; index++)
			{
				PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
					members[index].PlayerId);
				if (online != null)
				{
					online.GetFamilySystem().SetMembership(null, null, true);
					online.ChatNotice("The family was disbanded.");
				}
			}
		}

		private void TransferLeadership(PlayerObject leader, uint memberId)
		{
			FamilyInfo family = leader.GetFamilySystem().GetFamily();
			if (family == null || !leader.GetFamilySystem().IsLeader())
			{
				leader.ChatNotice("Only the family leader can transfer leadership.");
				return;
			}
			FamilyMember successor = ResolveMember(family, memberId);
			FamilyMember current = leader.GetFamilySystem().GetMember();
			if (successor == null || current == null || successor == current)
			{
				leader.ChatNotice("Select another family member as the new leader.");
				return;
			}
			if (!CanPersist(leader))
			{
				return;
			}
			current.Rank = MemberRank;
			successor.Rank = LeaderRank;
			family.LeaderId = successor.PlayerId;
			family.LeaderName = successor.Name;
			SaveFamily(family, successor.PlayerId);
			NotifyFamilyChanged(family);
			leader.ChatNotice("Family leadership transferred to " + successor.Name + ".");
		}

		private void Donate(PlayerObject player, uint amount)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			FamilyMember member = player.GetFamilySystem().GetMember();
			if (family == null || member == null || amount == 0 || amount > int.MaxValue)
			{
				player.ChatNotice("Enter a valid family donation amount.");
				return;
			}
			if (player.GetMoneyCount(MONEYTYPE.GOLD) < (int)amount)
			{
				player.ChatNotice("You do not have enough gold.");
				return;
			}
			if (!CanPersist(player))
			{
				return;
			}
			player.ChangeMoney(MONEYTYPE.GOLD, -(int)amount);
			family.Money = ulong.MaxValue - family.Money < amount ?
				ulong.MaxValue : family.Money + amount;
			member.Proffer = uint.MaxValue - member.Proffer < amount ?
				uint.MaxValue : member.Proffer + amount;
			SaveFamily(family, member.PlayerId);
			SendSnapshot(player);
			player.ChatNotice("Family donation accepted.");
		}

		private void SendBasicInfo(PlayerObject player, uint familyId)
		{
			FamilyInfo family = familyId == 0 ?
				player.GetFamilySystem().GetFamily() : GetFamily(familyId);
			if (family == null)
			{
				SendFamilyPacket(player, 16, player.GetTypeId(), 0, 0, "");
				return;
			}
			FamilyMember member = FindMember(
				family, player.GetBaseAttr().player_id);
			SendFamilyPacket(
				player, 16, player.GetTypeId(), family.Id,
				member == null ? 0U : member.Rank, family.Name);
		}

		private void SendMemberList(PlayerObject player)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			if (family == null)
			{
				SendFamilyPacket(player, 12, 0, 0, 0);
				return;
			}
			List<string> records = new List<string>();
			for (int index = 0; index < family.Members.Count; index++)
			{
				FamilyMember member = family.Members[index];
				PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.PlayerId);
				uint id = online == null ?
					unchecked((uint)member.PlayerId) : online.GetTypeId();
				records.Add(string.Format(
					CultureInfo.InvariantCulture,
					"{0} {1} {2} {3}", id, member.Rank,
					online == null ? 0 : 1, member.Name));
			}
			player.SendData(
				MapPacketCodec.CreateFamilyResponse(
					null, 12, family.Id,
					unchecked((uint)family.Members.Count), 0,
					records.ToArray()), true);
		}

		private void SendMemberDetail(PlayerObject player, uint memberId)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			FamilyMember member = ResolveMember(family, memberId);
			if (family == null || member == null)
			{
				player.ChatNotice("Family member not found.");
				return;
			}
			player.SendData(
				MapPacketCodec.CreateFamilyMemberResponse(
					null, BuildMemberRecord(family, member)), true);
		}

		private void SendAnnouncement(PlayerObject player)
		{
			FamilyInfo family = player.GetFamilySystem().GetFamily();
			if (family == null || string.IsNullOrEmpty(family.Announcement))
			{
				return;
			}
			MsgTalkInfo notice = new MsgTalkInfo
			{
				rgba = 0xffffff,
				unTxtAttribute = 2130
			};
			notice.Create(null, player.GetGamePackKeyEx());
			notice.liststr.Add(family.LeaderName);
			notice.liststr.Add("");
			notice.liststr.Add("");
			notice.liststr.Add(family.Announcement);
			player.SendData(notice.GetBuffer(), false);
		}

		private static void OpenOccupyDialog(PlayerObject player, uint npcId)
		{
			if (!player.GetFamilySystem().IsHaveFamily())
			{
				player.ChatNotice("You must belong to a family.");
				return;
			}
			if (npcId == 0)
			{
				player.ChatNotice("The family territory target is invalid.");
				return;
			}
			SendFamilyPacket(player, 24, npcId, 0, 0);
		}

		private static void SendOccupyQuery(PlayerObject player, uint npcId)
		{
			if (!player.GetFamilySystem().IsHaveFamily() || npcId == 0)
			{
				return;
			}
			// The client requires three strings even when a territory has no owner.
			SendFamilyPacket(player, 25, npcId, 0, 0, "0", "", "0");
		}

		private void NotifyFamilyChanged(FamilyInfo family)
		{
			for (int index = 0; index < family.Members.Count; index++)
			{
				FamilyMember member = family.Members[index];
				PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.PlayerId);
				if (online != null)
				{
					online.GetFamilySystem().SetMembership(family, member, false);
					SendSnapshot(online);
					SendMemberList(online);
					RefreshVisibleFamilyState(online);
				}
			}
		}

		private void NotifyMemberRemoved(FamilyInfo family, uint clientId)
		{
			for (int index = 0; index < family.Members.Count; index++)
			{
				PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
					family.Members[index].PlayerId);
				if (online != null)
				{
					SendFamilyPacket(online, 19, clientId, family.Id, 0);
				}
			}
		}

		private static byte[] BuildAttributeRecord(
			FamilyInfo family,
			FamilyMember member)
		{
			byte[] record = new byte[80];
			WriteUInt32(record, 0x00, family.Id);
			WriteFixedString(record, 0x04, 16, family.Name);
			WriteUInt16(record, 0x14, family.Rank);
			WriteUInt32(record, 0x18, family.Reputation);
			WriteUInt32(record, 0x1c, unchecked((uint)family.Members.Count));
			WriteUInt32(record, 0x20, member.Proffer);
			WriteUInt32(record, 0x28, unchecked((uint)(family.Money >> 32)));
			WriteUInt32(record, 0x2c, unchecked((uint)family.Money));
			WriteUInt32(record, 0x38, family.ChallengeMap);
			WriteUInt32(record, 0x3c, family.FamilyMap);
			WriteUInt32(record, 0x40, family.Truce);
			WriteUInt32(record, 0x44, family.StarTower);
			WriteUInt16(record, 0x48, member.Rank);
			WriteUInt32(record, 0x4c, member.JoinDate);
			return record;
		}

		private static byte[] BuildMemberRecord(
			FamilyInfo family,
			FamilyMember member)
		{
			byte[] record = new byte[56];
			PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
				member.PlayerId);
			uint id = online == null ?
				unchecked((uint)member.PlayerId) : online.GetTypeId();
			WriteUInt32(record, 0x00, id);
			WriteFixedString(record, 0x10, 16, member.Name);
			WriteUInt32(record, 0x20, member.Proffer);
			record[0x24] = online == null ? (byte)0 : online.GetLevel();
			record[0x25] = online == null ? (byte)0 :
				online.GetBaseAttr().profession;
			WriteUInt32(record, 0x28, online == null ? 0U :
				unchecked((uint)Math.Max(0, (int)online.GetBaseAttr().pk)));
			WriteUInt16(record, 0x2e, member.Rank);
			WriteUInt32(record, 0x30, online == null ? 0U : 1U);
			WriteUInt16(record, 0x34, family.StarTower);
			return record;
		}

		private void SaveFamily(FamilyInfo family, int playerId)
		{
			FamilyOption option = new FamilyOption();
			option.SetUpdateTag();
			option.PlayerId = playerId;
			option.Info = family;
			DBServer.Instance().GetDBClient().SendData(option.GetBuffer());
		}

		private static bool CanPersist(PlayerObject player)
		{
			if (DBServer.Instance().IsConnect())
			{
				return true;
			}
			player.ChatNotice("Family changes are unavailable while the database is offline.");
			return false;
		}

		private static FamilyMember ResolveMember(
			FamilyInfo family,
			uint memberId)
		{
			if (family == null)
			{
				return null;
			}
			for (int index = 0; index < family.Members.Count; index++)
			{
				FamilyMember member = family.Members[index];
				if (unchecked((uint)member.PlayerId) == memberId)
				{
					return member;
				}
				PlayerObject online = UserEngine.Instance().FindPlayerObjectToPlayerId(
					member.PlayerId);
				if (online != null && online.GetTypeId() == memberId)
				{
					return member;
				}
			}
			return null;
		}

		private static PlayerObject FindOnlinePlayer(uint id)
		{
			PlayerObject player = UserEngine.Instance().FindPlayerObjectToTypeID(id);
			return player ?? UserEngine.Instance().FindPlayerObjectToPlayerId(
				unchecked((int)id));
		}

		private static void SendFamilyPacket(
			PlayerObject player,
			ushort action,
			uint valueA,
			uint valueB,
			uint valueC,
			params string[] strings)
		{
			player.SendData(
				MapPacketCodec.CreateFamilyResponse(
					null, action, valueA, valueB, valueC, strings), true);
		}

		private void ReplaceRequest(
			List<PendingRequest> list,
			uint leaderTypeId,
			uint memberTypeId)
		{
			PendingRequest existing = FindRequest(
				list, leaderTypeId, memberTypeId);
			if (existing != null)
			{
				list.Remove(existing);
			}
			list.Add(new PendingRequest
			{
				LeaderTypeId = leaderTypeId,
				MemberTypeId = memberTypeId,
				ExpiresUtc = DateTime.UtcNow.AddSeconds(60)
			});
		}

		private static PendingRequest FindRequest(
			List<PendingRequest> list,
			uint leaderTypeId,
			uint memberTypeId)
		{
			for (int index = 0; index < list.Count; index++)
			{
				if (list[index].LeaderTypeId == leaderTypeId &&
					list[index].MemberTypeId == memberTypeId)
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

		private static uint GetUnixTime()
		{
			return checked((uint)(DateTime.UtcNow -
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
		}

		private static void WriteFixedString(
			byte[] target,
			int offset,
			int size,
			string value)
		{
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(value ?? "");
			Buffer.BlockCopy(encoded, 0, target, offset,
				Math.Min(size - 1, encoded.Length));
		}

		private static void WriteUInt16(byte[] target, int offset, uint value)
		{
			byte[] encoded = BitConverter.GetBytes(unchecked((ushort)value));
			Buffer.BlockCopy(encoded, 0, target, offset, encoded.Length);
		}

		private static void WriteUInt32(byte[] target, int offset, uint value)
		{
			byte[] encoded = BitConverter.GetBytes(value);
			Buffer.BlockCopy(encoded, 0, target, offset, encoded.Length);
		}

		private static FamilyManager instance;
		private readonly Dictionary<uint, FamilyInfo> families;
		private readonly Dictionary<int, PendingCreation> pendingCreations;
		private readonly List<PendingRequest> applications;
		private readonly List<PendingRequest> invitations;
	}
}
