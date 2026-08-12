using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000058 RID: 88
	public class MsgOpenNpc : BaseMsg
	{
		// Token: 0x060001E9 RID: 489 RVA: 0x00014E2E File Offset: 0x0001302E
		public MsgOpenNpc()
		{
			this.mMsgLen = 16;
			this.mParam = 2031;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00014E4C File Offset: 0x0001304C
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.id = packIn.ReadUInt32();
				this.param = packIn.ReadInt32();
				this.param1 = packIn.ReadInt32();
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00014EAC File Offset: 0x000130AC
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			return base.GetBuffer();
		}

		// Token: 0x040003DF RID: 991
		public uint id;

		// Token: 0x040003E0 RID: 992
		public int param;

		// Token: 0x040003E1 RID: 993
		public int param1;
	}
}
