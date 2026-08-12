using System;
using GameBase.Core;
using GameBase.Network;

namespace MapServer
{
	// Token: 0x02000009 RID: 9
	public class PtichObject : BaseObject
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00004A9C File Offset: 0x00002C9C
		public PtichObject(PlayerObject Play)
		{
			this.type = 10;
			this.mPlay = Play;
			this.typeid = (uint)(107000 + this.mPlay.GetCurrentPtichID());
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004AD0 File Offset: 0x00002CD0
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

		// Token: 0x06000070 RID: 112 RVA: 0x00004BC0 File Offset: 0x00002DC0
		public void Refresh()
		{
			this.RefreshVisibleObject();
			foreach (RefreshObject refreshObject in base.GetVisibleList().Values)
			{
				BaseObject obj = refreshObject.obj;
				this.SendInfo(obj as PlayerObject);
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004C64 File Offset: 0x00002E64
		public void SendInfo(PlayerObject play)
		{
			byte[] array = new byte[]
			{
				41,
				0,
				238,
				7,
				28,
				162,
				1,
				0,
				91,
				1,
				27,
				2,
				144,
				1,
				0,
				0,
				34,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				76,
				0,
				0,
				0,
				1,
				8,
				211,
				249,
				193,
				250,
				207,
				201,
				181,
				192,
				0,
				0,
				0
			};
			PacketOut packetOut = new PacketOut(null);
			string name = this.mPlay.GetName();
			int num = 33 + Coding.GetDefauleCoding().GetBytes(name).Length;
			packetOut.WriteInt16((short)num);
			packetOut.WriteInt16(2030);
			packetOut.WriteUInt32(base.GetTypeId());
			packetOut.WriteInt16(base.GetCurrentX());
			packetOut.WriteInt16(base.GetCurrentY());
			packetOut.WriteInt32(400);
			packetOut.WriteInt32(34);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(76);
			packetOut.WriteByte(1);
			packetOut.WriteString(name);
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			play.SendData(packetOut.Flush(), true);
		}

		// Token: 0x04000042 RID: 66
		private PlayerObject mPlay;
	}
}
