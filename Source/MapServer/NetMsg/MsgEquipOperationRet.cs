using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000083 RID: 131
	public class MsgEquipOperationRet : BaseMsg
	{
		// Token: 0x06000283 RID: 643 RVA: 0x0001A0E8 File Offset: 0x000182E8
		public MsgEquipOperationRet()
		{
			this.mMsgLen = 20;
			this.mParam = 2036;
			this.ret = 0U;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0001A110 File Offset: 0x00018310
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.type);
			packetOut.WriteUInt32(this.srcid);
			packetOut.WriteUInt32(this.destid);
			packetOut.WriteUInt32(this.ret);
			return packetOut.Flush();
		}

		// Token: 0x040005BA RID: 1466
		public uint type;

		// Token: 0x040005BB RID: 1467
		public uint srcid;

		// Token: 0x040005BC RID: 1468
		public uint destid;

		// Token: 0x040005BD RID: 1469
		public uint ret;
	}
}
