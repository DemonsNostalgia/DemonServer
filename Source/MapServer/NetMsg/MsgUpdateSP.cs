using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000062 RID: 98
	public class MsgUpdateSP : BaseMsg
	{
		// Token: 0x0600020B RID: 523 RVA: 0x00016210 File Offset: 0x00014410
		public MsgUpdateSP()
		{
			this.mMsgLen = 20;
			this.mParam = 1017;
			this.amount = 1U;
			this.value = 9U;
			this.sp = 100U;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00016248 File Offset: 0x00014448
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.role_id = packIn.ReadUInt32();
				this.amount = packIn.ReadUInt32();
				this.value = packIn.ReadUInt32();
				this.sp = packIn.ReadUInt32();
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x000162A8 File Offset: 0x000144A8
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.role_id);
			packetOut.WriteUInt32(this.amount);
			packetOut.WriteUInt32(this.value);
			packetOut.WriteUInt32(this.sp);
			return packetOut.Flush();
		}

		// Token: 0x0400043E RID: 1086
		public uint role_id;

		// Token: 0x0400043F RID: 1087
		public uint amount;

		// Token: 0x04000440 RID: 1088
		public uint value;

		// Token: 0x04000441 RID: 1089
		public uint sp;
	}
}
