using System;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000091 RID: 145
	public class PlayerLegion
	{
		// Token: 0x06000328 RID: 808 RVA: 0x000245A4 File Offset: 0x000227A4
		public Legion GetLegion()
		{
			return this.legion;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x000245BC File Offset: 0x000227BC
		public PlayerLegion(PlayerObject _play)
		{
			this.legion = null;
			this.play = _play;
		}

		// Token: 0x0600032A RID: 810 RVA: 0x000245D8 File Offset: 0x000227D8
		public void Init(Legion _legion = null)
		{
			if (_legion != null)
			{
				this.legion = _legion;
			}
			else
			{
				this.legion = LegionManager.Instance().GetPlayerLegion(this.play.GetName());
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00024614 File Offset: 0x00022814
		public LegionMember GetMember(string name)
		{
			if (this.legion == null || string.IsNullOrEmpty(name))
			{
				return null;
			}
			for (int i = 0; i < this.legion.GetBaseInfo().list_member.Count; i++)
			{
				if (string.Equals(
					this.legion.GetBaseInfo().list_member[i].members_name,
					name,
					StringComparison.OrdinalIgnoreCase))
				{
					return this.legion.GetBaseInfo().list_member[i];
				}
			}
			return null;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00024690 File Offset: 0x00022890
		public void SendLegionInfo()
		{
			MsgSelfLegionInfo msgSelfLegionInfo = new MsgSelfLegionInfo();
			msgSelfLegionInfo.Create(null, this.play.GetGamePackKeyEx());
			if (this.legion == null)
			{
				msgSelfLegionInfo.legion_id = 0U;
				this.play.SendData(msgSelfLegionInfo.GetBuffer(), false);
			}
			else
			{
				LegionInfo info = this.legion.GetBaseInfo();
				LegionMember member = this.GetMember(this.play.GetName());
				msgSelfLegionInfo.legion_id = info.id;
				msgSelfLegionInfo.proffer =
					LegionManager.ClampContribution(
						member == null ?
							0L :
							member.money,
						member == null ?
							0L :
							member.emoney);
				msgSelfLegionInfo.population = info.list_member.Count;
				msgSelfLegionInfo.rank =
					member == null ? (short)0 : member.rank;
				msgSelfLegionInfo.syndicate_rank = 1;
				msgSelfLegionInfo.member_title = info.title;
				msgSelfLegionInfo.leader_name = info.leader_name;
				this.play.SendData(msgSelfLegionInfo.GetBuffer(), false);
				MsgLegionName msgLegionName = new MsgLegionName();
				msgLegionName.Create(
					null, this.play.GetGamePackKeyEx());
				msgLegionName.legion_id = info.id;
				msgLegionName.legion_name = info.name;
				this.play.SendData(msgLegionName.GetBuffer(), false);
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00024844 File Offset: 0x00022A44
		public int GetDevote()
		{
			LegionMember member = this.GetMember(this.play.GetName());
			int result;
			if (member == null)
			{
				result = 0;
			}
			else
			{
				result = LegionManager.ClampContribution(member.money);
			}
			return result;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00024880 File Offset: 0x00022A80
		public void SetLegion(Legion _legion, bool bSendData = false)
		{
			this.legion = _legion;
			if (bSendData)
			{
				this.SendLegionInfo();
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000248A8 File Offset: 0x00022AA8
		public bool IsHaveLegion()
		{
			return this.legion != null;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000248C8 File Offset: 0x00022AC8
		public void ChangeLegionTitle(byte title)
		{
			if (this.legion != null && title >= 1 && title <= 4)
			{
				this.legion.GetBaseInfo().title = title;
				this.SendLegionInfo();
				LegionManager.Instance().UpdateLegionInfo(this.legion.GetBaseInfo().id, this.play.GetBaseAttr().player_id);
			}
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0002492C File Offset: 0x00022B2C
		public short GetPlace()
		{
			LegionMember member = this.GetMember(this.play.GetName());
			short result;
			if (member == null)
			{
				result = 0;
			}
			else
			{
				result = member.rank;
			}
			return result;
		}

		public bool IsLeader()
		{
			return this.legion != null &&
				this.legion.GetBaseInfo().leader_id ==
					this.play.GetBaseAttr().player_id;
		}

		// Token: 0x04000618 RID: 1560
		private PlayerObject play;

		// Token: 0x04000619 RID: 1561
		private Legion legion;
	}
}
