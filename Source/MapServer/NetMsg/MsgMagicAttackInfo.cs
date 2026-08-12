using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006B RID: 107
	public class MsgMagicAttackInfo : BaseMsg
	{
		// Token: 0x0600022B RID: 555 RVA: 0x00017214 File Offset: 0x00015414
		public MsgMagicAttackInfo()
		{
			this.mMsgLen = 84;
			this.mParam = 1105;
			this.type = 1;
			this.param = 0;
			this.param1 = new int[4];
			for (int i = 0; i < this.param1.Length; i++)
			{
				this.param1[i] = 0;
			}
			this.param2 = new int[10];
			for (int i = 0; i < this.param2.Length; i++)
			{
				this.param2[i] = 0;
			}
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000172A8 File Offset: 0x000154A8
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x000172B4 File Offset: 0x000154B4
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.targetid);
			packetOut.WriteUInt16(this.magicid);
			packetOut.WriteUInt16(this.level);
			packetOut.WriteByte(this.dir);
			packetOut.WriteByte(this.type);
			packetOut.WriteInt16(this.param);
			for (int i = 0; i < this.param1.Length; i++)
			{
				packetOut.WriteInt32(this.param1[i]);
			}
			packetOut.WriteUInt32(this.targetid);
			packetOut.WriteUInt32(this.value);
			for (int i = 0; i < this.param2.Length; i++)
			{
				packetOut.WriteInt32(this.param2[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040004C9 RID: 1225
		public uint id;

		// Token: 0x040004CA RID: 1226
		public uint targetid;

		// Token: 0x040004CB RID: 1227
		public ushort magicid;

		// Token: 0x040004CC RID: 1228
		public ushort level;

		// Token: 0x040004CD RID: 1229
		public byte dir;

		// Token: 0x040004CE RID: 1230
		public byte type;

		// Token: 0x040004CF RID: 1231
		public short param;

		// Token: 0x040004D0 RID: 1232
		public int[] param1;

		// Token: 0x040004D1 RID: 1233
		public uint value;

		// Token: 0x040004D2 RID: 1234
		public int[] param2;
	}
}
