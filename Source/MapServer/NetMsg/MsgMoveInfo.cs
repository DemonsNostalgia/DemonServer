using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000056 RID: 86
	public class MsgMoveInfo : BaseMsg
	{
		// Token: 0x060001E3 RID: 483 RVA: 0x000149BC File Offset: 0x00012BBC
		public MsgMoveInfo()
		{
			this.mMsgLen = 24;
			this.mParam = 3005;
			this.time = Environment.TickCount;
			this.id = 0U;
			this.x = (this.y = 0);
			this.ucMode = (this.dir = 0);
			this.param = 0;
			this.param2 = 0;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00014A28 File Offset: 0x00012C28
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null && msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.time = packIn.ReadInt32();
				this.id = packIn.ReadUInt32();
				this.x = packIn.ReadInt16();
				this.y = packIn.ReadInt16();
				this.dir = packIn.ReadByte();
				this.ucMode = packIn.ReadByte();
				this.param = packIn.ReadUInt16();
				this.param2 = packIn.ReadInt32();
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00014ACC File Offset: 0x00012CCC
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteByte(this.dir);
			packetOut.WriteByte(this.ucMode);
			packetOut.WriteUInt16(this.param);
			packetOut.WriteInt32(this.param2);
			return packetOut.Flush();
		}

		// Token: 0x040003CA RID: 970
		public int time;

		// Token: 0x040003CB RID: 971
		public uint id;

		// Token: 0x040003CC RID: 972
		public short x;

		// Token: 0x040003CD RID: 973
		public short y;

		// Token: 0x040003CE RID: 974
		public byte dir;

		// Token: 0x040003CF RID: 975
		public byte ucMode;

		// Token: 0x040003D0 RID: 976
		public ushort param;

		// Token: 0x040003D1 RID: 977
		public int param2;
	}
}
