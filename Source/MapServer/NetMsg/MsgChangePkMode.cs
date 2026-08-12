using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000073 RID: 115
	public class MsgChangePkMode : BaseMsg
	{
		// Token: 0x06000248 RID: 584 RVA: 0x00017C44 File Offset: 0x00015E44
		public MsgChangePkMode()
		{
			this.mParam = 1010;
			this.mMsgLen = 28;
			this.time = Environment.TickCount;
			this.roleid = 0U;
			this.type = (this.value = 0);
			this.param = 0;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x00017C98 File Offset: 0x00015E98
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.time = packIn.ReadInt32();
				this.roleid = packIn.ReadUInt32();
				this.type = packIn.ReadInt32();
				this.param = packIn.ReadInt32();
				this.value = packIn.ReadInt32();
				this.tag = packIn.ReadInt32();
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x00017D10 File Offset: 0x00015F10
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.value);
			packetOut.WriteInt32(this.tag);
			return packetOut.Flush();
		}

		// Token: 0x0600024B RID: 587 RVA: 0x00017D9C File Offset: 0x00015F9C
		public void SetKey(GamePacketKeyEx key)
		{
			this.mKey = key;
		}

		// Token: 0x04000537 RID: 1335
		public int time;

		// Token: 0x04000538 RID: 1336
		public uint roleid;

		// Token: 0x04000539 RID: 1337
		public int type;

		// Token: 0x0400053A RID: 1338
		public int param;

		// Token: 0x0400053B RID: 1339
		public int value;

		// Token: 0x0400053C RID: 1340
		public int tag;
	}
}
