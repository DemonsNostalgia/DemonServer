using System;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200009C RID: 156
	public class RobotObject : BaseObject
	{
		// Token: 0x060003F0 RID: 1008 RVA: 0x0002E22C File Offset: 0x0002C42C
		public RobotObject()
		{
			this.type = 6;
			this.typeid = IDManager.CreateTypeId(2);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0002E24C File Offset: 0x0002C44C
		public void SendRobotInfo(PlayerObject play)
		{
			uint legionId = RobotLegionManager.GetInstance().GetLegionId(this.mInfo.legion_name);
			MsgRoleInfo msgRoleInfo = new MsgRoleInfo();
			msgRoleInfo.Create(null, play.GetGamePackKeyEx());
			msgRoleInfo.role_id = base.GetTypeId();
			msgRoleInfo.x = this.mInfo.x;
			msgRoleInfo.y = this.mInfo.y;
			msgRoleInfo.armor_id = this.mInfo.armor_id;
			msgRoleInfo.wepon_id = this.mInfo.wepon_id;
			msgRoleInfo.face_sex = (msgRoleInfo.face_sex1 = this.mInfo.lookface);
			msgRoleInfo.dir = this.mInfo.dir;
			msgRoleInfo.guanjue = this.mInfo.guanjue;
			msgRoleInfo.hair_id = this.mInfo.hair;
			msgRoleInfo.rid_id = this.mInfo.rid_id;
			msgRoleInfo.str.Add(this.mInfo.name);
			if (this.mInfo.legion_name.Length > 0)
			{
				msgRoleInfo.legion_id = legionId;
				msgRoleInfo.legion_title = this.mInfo.legion_title;
				msgRoleInfo.legion_place = this.mInfo.legion_place;
				msgRoleInfo.legion_id1 = legionId;
			}
			play.SendData(msgRoleInfo.GetBuffer(), false);
			if (legionId > 0U)
			{
				MsgLegionName msgLegionName = new MsgLegionName();
				msgLegionName.Create(null, play.GetGamePackKeyEx());
				msgLegionName.legion_id = legionId;
				msgLegionName.legion_name = this.mInfo.legion_name;
				play.SendData(msgLegionName.GetBuffer(), false);
			}
			this.PlayFaceAcion(210U, play);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0002E3FB File Offset: 0x0002C5FB
		public void SetRobotInfo(RobotInfo _info)
		{
			this.mInfo = _info;
			this.SetPoint(this.mInfo.x, this.mInfo.y);
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x0002E424 File Offset: 0x0002C624
		public override bool Run()
		{
			return true;
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0002E438 File Offset: 0x0002C638
		public override void RefreshVisibleObject()
		{
			base.RefreshVisibleObject();
			foreach (BaseObject baseObject in this.mGameMap.GetAllObject().Values)
			{
				if (baseObject.GetGameID() != base.GetGameID())
				{
					if (baseObject.type == 2)
					{
						if (base.GetPoint().CheckVisualDistance(baseObject.GetCurrentX(), baseObject.GetCurrentY(), 15))
						{
							base.AddVisibleObject(baseObject, false);
						}
						else if (this.mVisibleList.ContainsKey(baseObject.GetGameID()))
						{
							this.mVisibleList.Remove(baseObject.GetGameID());
						}
					}
				}
			}
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0002E524 File Offset: 0x0002C724
		public void PlayFaceAcion(uint action_id, PlayerObject play = null)
		{
			PacketOut packetOut;
			if (play == null)
			{
				packetOut = new PacketOut(null);
			}
			else
			{
				packetOut = new PacketOut(play.GetGamePackKeyEx());
			}
			packetOut.WriteUInt16(28);
			packetOut.WriteUInt16(1010);
			packetOut.WriteUInt32(0U);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteUInt32(23855267U);
			packetOut.WriteUInt32((uint)this.mInfo.dir);
			packetOut.WriteUInt32(action_id);
			packetOut.WriteUInt32(9530U);
			byte[] array = packetOut.Flush();
			if (play != null)
			{
				play.SendData(array, false);
			}
			else
			{
				foreach (RefreshObject refreshObject in this.mVisibleList.Values)
				{
					BaseObject obj = refreshObject.obj;
					if (obj.type == 2)
					{
						PlayerObject playerObject = obj as PlayerObject;
						packetOut = new PacketOut(playerObject.GetGamePackKeyEx());
						packetOut.WriteBuff(array);
						playerObject.SendData(packetOut.Flush(), false);
					}
				}
			}
		}

		// Token: 0x04000667 RID: 1639
		private RobotInfo mInfo;
	}
}
