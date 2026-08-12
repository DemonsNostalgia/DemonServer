using System;
using System.Collections.Generic;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x020000A8 RID: 168
	public class Team
	{
		// Token: 0x06000452 RID: 1106 RVA: 0x000333D9 File Offset: 0x000315D9
		public Team(uint id)
		{
			this.mID = id;
			this.mCreateTime = GetUnixTime();
			this.mlistMember = new List<PlayerObject>();
		}

		// Token: 0x06000453 RID: 1107 RVA: 0x000333F8 File Offset: 0x000315F8
		public uint GetTeamID()
		{
			return this.mID;
		}

		// Token: 0x06000454 RID: 1108 RVA: 0x00033410 File Offset: 0x00031610
		public bool IsTeamFull()
		{
			return this.mlistMember.Count >= MAX_TEAM_COUNT;
		}

		// Token: 0x06000455 RID: 1109 RVA: 0x00033433 File Offset: 0x00031633
		public bool AddMember(PlayerObject play)
		{
			if (play == null || this.IsTeamFull() || this.Contains(play))
			{
				return false;
			}
			this.mlistMember.Add(play);
			play.SetTeam(this);
			return true;
		}

		// Token: 0x06000456 RID: 1110 RVA: 0x00033443 File Offset: 0x00031643
		public void DeleteMember(PlayerObject play)
		{
			this.ExitTeam(play);
		}

		// Token: 0x06000457 RID: 1111 RVA: 0x00033450 File Offset: 0x00031650
		public PlayerObject GetCaptain()
		{
			PlayerObject result;
			if (this.mlistMember.Count <= 0)
			{
				result = null;
			}
			else
			{
				result = this.mlistMember[0];
			}
			return result;
		}

		// Token: 0x06000458 RID: 1112 RVA: 0x00033484 File Offset: 0x00031684
		public void ExitTeam(PlayerObject _play)
		{
			TeamManager.Instance().RemoveMember(this, _play);
		}

		// Token: 0x06000459 RID: 1113 RVA: 0x00033510 File Offset: 0x00031710
		public void DeleteTeam()
		{
			for (int i = 0; i < this.mlistMember.Count; i++)
			{
				PlayerObject playerObject = this.mlistMember[i];
				playerObject.SetTeam(null);
			}
			this.mlistMember.Clear();
			TeamManager.Instance().DeleteTeam(this.mID);
		}

		// Token: 0x0600045A RID: 1114 RVA: 0x0003356C File Offset: 0x0003176C
		public void ShareInfo(PlayerObject obj)
		{
			for (int i = 0; i < this.mlistMember.Count; i++)
			{
				PlayerObject playerObject = this.mlistMember[i];
				if (playerObject.GetBaseAttr().player_id != obj.GetBaseAttr().player_id)
				{
					MsgUserAttribute msgUserAttribute = new MsgUserAttribute();
					msgUserAttribute.Create(null, playerObject.GetGamePackKeyEx());
					msgUserAttribute.role_id = playerObject.GetTypeId();
					msgUserAttribute.AddAttribute(UserAttribute.LIFE, obj.GetBaseAttr().life);
					msgUserAttribute.AddAttribute(UserAttribute.LIFE_MAX, obj.GetBaseAttr().life_max);
					playerObject.SendData(msgUserAttribute.GetBuffer(), false);
				}
			}
		}

		public uint GetCreateTime()
		{
			return this.mCreateTime;
		}

		public int GetMemberCount()
		{
			return this.mlistMember.Count;
		}

		public bool Contains(PlayerObject player)
		{
			if (player == null)
			{
				return false;
			}
			for (int index = 0; index < this.mlistMember.Count; index++)
			{
				if (this.mlistMember[index] == player ||
					this.mlistMember[index].GetTypeId() == player.GetTypeId())
				{
					return true;
				}
			}
			return false;
		}

		public bool RemoveMemberWithoutNotification(PlayerObject player)
		{
			if (player == null)
			{
				return false;
			}
			for (int index = 0; index < this.mlistMember.Count; index++)
			{
				if (this.mlistMember[index] == player ||
					this.mlistMember[index].GetTypeId() == player.GetTypeId())
				{
					this.mlistMember.RemoveAt(index);
					player.SetTeam(null);
					return true;
				}
			}
			return false;
		}

		public PlayerObject[] GetMembers()
		{
			return this.mlistMember.ToArray();
		}

		public void BroadcastChat(PlayerObject sender, byte[] payload)
		{
			if (sender == null || payload == null)
			{
				return;
			}
			PacketOut output = new PacketOut(null);
			output.WriteUInt16((ushort)(payload.Length + 2));
			output.WriteBuff(payload);
			byte[] packet = output.Flush();
			for (int index = 0; index < this.mlistMember.Count; index++)
			{
				PlayerObject recipient = this.mlistMember[index];
				if (recipient == sender || recipient.GetGameSession() == null)
				{
					continue;
				}
				BaseMsg encrypted = new BaseMsg();
				encrypted.Create(packet, recipient.GetGamePackKeyEx());
				recipient.SendData(encrypted.GetBuffer(), false);
			}
		}

		// Token: 0x04000695 RID: 1685
		private const int MAX_TEAM_COUNT = 5;

		// Token: 0x04000696 RID: 1686
		private uint mID;

		private uint mCreateTime;

		// Token: 0x04000697 RID: 1687
		private List<PlayerObject> mlistMember;

		private static uint GetUnixTime()
		{
			return checked((uint)(DateTime.UtcNow -
				new DateTime(1970, 1, 1, 0, 0, 0,
					DateTimeKind.Utc)).TotalSeconds);
		}
	}
}
