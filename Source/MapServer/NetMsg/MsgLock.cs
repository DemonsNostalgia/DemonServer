using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000077 RID: 119
	public class MsgLock : BaseMsg
	{
		// Token: 0x06000255 RID: 597 RVA: 0x00018041 File Offset: 0x00016241
		public MsgLock()
		{
			this.time = Environment.TickCount;
			this.mMsgLen = 28;
			this.mParam = 1010;
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00018078 File Offset: 0x00016278
		public void Lock()
		{
			this.tag = 9618;
		}

		// Token: 0x06000257 RID: 599 RVA: 0x00018086 File Offset: 0x00016286
		public void UnLock()
		{
			this.tag = 9619;
		}

		// Token: 0x06000258 RID: 600 RVA: 0x00018094 File Offset: 0x00016294
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000180A0 File Offset: 0x000162A0
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x04000552 RID: 1362
		public int time;

		// Token: 0x04000553 RID: 1363
		public uint id;

		// Token: 0x04000554 RID: 1364
		public short x;

		// Token: 0x04000555 RID: 1365
		public short y;

		// Token: 0x04000556 RID: 1366
		public int param = 0;

		// Token: 0x04000557 RID: 1367
		public int param1 = 1;

		// Token: 0x04000558 RID: 1368
		public int tag;
	}
}
