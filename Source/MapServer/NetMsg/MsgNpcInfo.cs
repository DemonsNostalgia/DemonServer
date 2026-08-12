using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000053 RID: 83
	public class MsgNpcInfo : BaseMsg
	{
		// Token: 0x060001D9 RID: 473 RVA: 0x000140A8 File Offset: 0x000122A8
		public MsgNpcInfo()
		{
			this.mMsgLen = 32;
			this.mParam = 2030;
			this.mnX = (this.mnY = 0);
			this.mnID = 0U;
			this.lookface = 0;
		}

		// Token: 0x060001DA RID: 474 RVA: 0x000140EF File Offset: 0x000122EF
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x000140FB File Offset: 0x000122FB
		public void Init(uint id, short x, short y, int _lookface)
		{
			this.mnID = id;
			this.mnX = x;
			this.mnY = y;
			this.lookface = _lookface;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0001411C File Offset: 0x0001231C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.mnID);
			packetOut.WriteInt16(this.mnX);
			packetOut.WriteInt16(this.mnY);
			packetOut.WriteInt32(this.lookface);
			byte[] array = new byte[16];
			array[0] = 2;
			array[2] = 1;
			byte[] v = array;
			packetOut.WriteBuff(v);
			return packetOut.Flush();
		}

		// Token: 0x0400038A RID: 906
		public uint mnID;

		// Token: 0x0400038B RID: 907
		public short mnX;

		// Token: 0x0400038C RID: 908
		public short mnY;

		// Token: 0x0400038D RID: 909
		public int lookface;
	}
}
