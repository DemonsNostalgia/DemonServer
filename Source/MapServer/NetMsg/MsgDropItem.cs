using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000070 RID: 112
	public class MsgDropItem : BaseMsg
	{
		// Token: 0x06000240 RID: 576 RVA: 0x00017A24 File Offset: 0x00015C24
		public MsgDropItem()
		{
			this.mMsgLen = 28;
			this.mParam = 1101;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00017A42 File Offset: 0x00015C42
		public void SetRefreshTag()
		{
			this.tag = 1U;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00017A4C File Offset: 0x00015C4C
		public void SetPickTag()
		{
			this.tag = 2U;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00017A58 File Offset: 0x00015C58
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.id = packIn.ReadUInt32();
				this.id ^= 9527U;
				this.typeid = packIn.ReadUInt32();
				this.x = packIn.ReadInt16();
				this.y = packIn.ReadInt16();
				this.param = packIn.ReadInt32();
				this.tag = packIn.ReadUInt32();
				this.param1 = packIn.ReadInt32();
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00017AF0 File Offset: 0x00015CF0
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.typeid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt32(this.param);
			packetOut.WriteUInt32(this.tag);
			packetOut.WriteInt32(this.param1);
			return packetOut.Flush();
		}

		// Token: 0x04000519 RID: 1305
		public uint id;

		// Token: 0x0400051A RID: 1306
		public uint typeid;

		// Token: 0x0400051B RID: 1307
		public short x;

		// Token: 0x0400051C RID: 1308
		public short y;

		// Token: 0x0400051D RID: 1309
		public int param;

		// Token: 0x0400051E RID: 1310
		public uint tag;

		// Token: 0x0400051F RID: 1311
		public int param1;
	}
}
