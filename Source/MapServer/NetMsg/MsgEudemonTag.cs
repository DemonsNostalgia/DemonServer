using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000081 RID: 129
	public class MsgEudemonTag : BaseMsg
	{
		// Token: 0x0600027C RID: 636 RVA: 0x00019E38 File Offset: 0x00018038
		public MsgEudemonTag()
		{
			this.mMsgLen = 28;
			this.mParam = 1010;
			this.action = 0;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00019E60 File Offset: 0x00018060
		public override byte[] GetBuffer()
		{
			this.param3 = this.eudemonid;
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.playerid);
			packetOut.WriteUInt32(this.eudemonid);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteUInt32(this.param3);
			packetOut.WriteInt32(this.action);
			return packetOut.Flush();
		}

		// Token: 0x0600027E RID: 638 RVA: 0x00019EF8 File Offset: 0x000180F8
		public void SetReCallTag()
		{
			this.action = 9545;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x00019F06 File Offset: 0x00018106
		public void SetBreakTag()
		{
			this.action = 9737;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x00019F14 File Offset: 0x00018114
		public void SetBattleTag()
		{
			this.action = 9788;
		}

		// Token: 0x040005A1 RID: 1441
		public uint playerid;

		// Token: 0x040005A2 RID: 1442
		public uint eudemonid;

		// Token: 0x040005A3 RID: 1443
		public int param1;

		// Token: 0x040005A4 RID: 1444
		public int param2;

		// Token: 0x040005A5 RID: 1445
		public uint param3;

		// Token: 0x040005A6 RID: 1446
		public int action;
	}
}
