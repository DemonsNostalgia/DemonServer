using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000076 RID: 118
	public class MsgReCall2 : BaseMsg
	{
		// Token: 0x06000252 RID: 594 RVA: 0x00017F61 File Offset: 0x00016161
		public MsgReCall2()
		{
			this.mParam = 1010;
			this.param = -1;
			this.type = 0;
			this.tag = 9567;
			this.time = Environment.TickCount;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00017F9B File Offset: 0x0001619B
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00017FA8 File Offset: 0x000161A8
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x0400054B RID: 1355
		public int time;

		// Token: 0x0400054C RID: 1356
		public uint roleid;

		// Token: 0x0400054D RID: 1357
		public short x;

		// Token: 0x0400054E RID: 1358
		public short y;

		// Token: 0x0400054F RID: 1359
		public int type;

		// Token: 0x04000550 RID: 1360
		public int param;

		// Token: 0x04000551 RID: 1361
		public int tag;
	}
}
