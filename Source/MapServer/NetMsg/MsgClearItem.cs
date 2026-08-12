using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000071 RID: 113
	public class MsgClearItem : BaseMsg
	{
		// Token: 0x06000245 RID: 581 RVA: 0x00017B89 File Offset: 0x00015D89
		public MsgClearItem()
		{
			this.mMsgLen = 28;
			this.mParam = 1009;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x00017BB0 File Offset: 0x00015DB0
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.param1);
			packetOut.WriteUInt32(this.tag);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteUInt32(this.param2);
			packetOut.WriteUInt32(this.param3);
			return packetOut.Flush();
		}

		// Token: 0x04000520 RID: 1312
		public uint id;

		// Token: 0x04000521 RID: 1313
		public uint param1;

		// Token: 0x04000522 RID: 1314
		public uint tag = 3U;

		// Token: 0x04000523 RID: 1315
		public uint roleid;

		// Token: 0x04000524 RID: 1316
		public uint param2;

		// Token: 0x04000525 RID: 1317
		public uint param3;
	}
}
