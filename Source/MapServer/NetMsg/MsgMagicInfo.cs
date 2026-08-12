using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000065 RID: 101
	public class MsgMagicInfo : BaseMsg
	{
		// Token: 0x06000217 RID: 535 RVA: 0x00016A58 File Offset: 0x00014C58
		public MsgMagicInfo()
		{
			this.mMsgLen = 16;
			this.mParam = 1103;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x00016A76 File Offset: 0x00014C76
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000219 RID: 537 RVA: 0x00016A84 File Offset: 0x00014C84
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.exp);
			packetOut.WriteUInt16(this.magicid);
			packetOut.WriteUInt16(this.level);
			return packetOut.Flush();
		}

		// Token: 0x0400049B RID: 1179
		public uint id;

		// Token: 0x0400049C RID: 1180
		public uint exp;

		// Token: 0x0400049D RID: 1181
		public ushort magicid;

		// Token: 0x0400049E RID: 1182
		public ushort level;
	}
}
