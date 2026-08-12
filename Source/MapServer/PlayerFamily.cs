using GameBase.Network.Internal;

namespace MapServer
{
	public sealed class PlayerFamily
	{
		public PlayerFamily(PlayerObject player)
		{
			this.player = player;
		}

		public void Init()
		{
			family = FamilyManager.Instance().GetPlayerFamily(
				player.GetBaseAttr().player_id);
			member = family == null ? null :
				FamilyManager.FindMember(
					family, player.GetBaseAttr().player_id);
		}

		public FamilyInfo GetFamily()
		{
			return family;
		}

		public FamilyMember GetMember()
		{
			return member;
		}

		public ushort GetRank()
		{
			return member == null ? (ushort)0 : member.Rank;
		}

		public bool IsHaveFamily()
		{
			return family != null && member != null;
		}

		public bool IsLeader()
		{
			return IsHaveFamily() &&
				family.LeaderId == player.GetBaseAttr().player_id &&
				member.Rank == FamilyManager.LeaderRank;
		}

		public void SetMembership(
			FamilyInfo newFamily,
			FamilyMember newMember,
			bool sendSnapshot)
		{
			family = newFamily;
			member = newMember;
			if (sendSnapshot)
			{
				FamilyManager.Instance().SendSnapshot(player);
				FamilyManager.Instance().RefreshVisibleFamilyState(player);
			}
		}

		private readonly PlayerObject player;
		private FamilyInfo family;
		private FamilyMember member;
	}
}
