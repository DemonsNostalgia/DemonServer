using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000054 RID: 84
	public class MsgMapInfo : BaseMsg
	{
		// Token: 0x060001DD RID: 477 RVA: 0x000141A8 File Offset: 0x000123A8
		public MsgMapInfo()
		{
			this.mParam = 1010;
			this.mMsgLen = 28;
			this.ID = (this.Param = 0);
			this.MapID = (this.MapID2 = 0U);
			this.x = (this.y = 0);
			this.LoadTag = 0;
			this.ID = Environment.TickCount;
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00014213 File Offset: 0x00012413
		public void Init(uint id, short xx, short yy, int _tag)
		{
			this.MapID = id;
			this.MapID2 = this.MapID;
			this.x = xx;
			this.y = yy;
			this.LoadTag = _tag;
		}

		public void InitLoginComplete(uint roleId)
		{
			this.MapID = roleId;
			this.MapID2 = 0U;
			this.x = 0;
			this.y = 0;
			this.Param = 0;
			this.LoadTag = COMPLETE_LOGIN;
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00014240 File Offset: 0x00012440
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.ID);
			packetOut.WriteUInt32(this.MapID);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.Param);
			packetOut.WriteUInt32(this.MapID2);
			packetOut.WriteInt32(this.LoadTag);
			return packetOut.Flush();
		}

		// Token: 0x0400038E RID: 910
		public const int ENTERMAP = 9541;

		public const int COMPLETE_LOGIN = 9542;

		public const int COMPLETE_LOGIN_ACK = 9543;

		// Token: 0x0400038F RID: 911
		public int ID;

		// Token: 0x04000390 RID: 912
		public uint MapID;

		// Token: 0x04000391 RID: 913
		public short x;

		// Token: 0x04000392 RID: 914
		public short y;

		// Token: 0x04000393 RID: 915
		public int Param;

		// Token: 0x04000394 RID: 916
		public uint MapID2;

		// Token: 0x04000395 RID: 917
		public int LoadTag;
	}
}
