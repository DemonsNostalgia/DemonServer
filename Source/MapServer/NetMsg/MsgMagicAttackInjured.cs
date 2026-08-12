using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000069 RID: 105
	public class MsgMagicAttackInjured : BaseMsg
	{
		// Token: 0x06000225 RID: 549 RVA: 0x00016F60 File Offset: 0x00015160
		public MsgMagicAttackInjured()
		{
			this.mMsgLen = 80;
			this.mParam = 1105;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00016F97 File Offset: 0x00015197
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00016FA4 File Offset: 0x000151A4
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteUInt16(this.magicid);
			packetOut.WriteUInt16(this.magiclv);
			packetOut.WriteByte(this.dir);
			packetOut.WriteByte(this.param);
			packetOut.WriteInt16(this.param1);
			for (int i = 0; i < this.param2.Length; i++)
			{
				packetOut.WriteInt32(this.param2[i]);
			}
			packetOut.WriteUInt32(this.targetid);
			packetOut.WriteUInt32(this.injured);
			for (int i = 0; i < this.param3.Length; i++)
			{
				packetOut.WriteInt32(this.param3[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040004B5 RID: 1205
		public uint id;

		// Token: 0x040004B6 RID: 1206
		public short x;

		// Token: 0x040004B7 RID: 1207
		public short y;

		// Token: 0x040004B8 RID: 1208
		public ushort magicid;

		// Token: 0x040004B9 RID: 1209
		public ushort magiclv;

		// Token: 0x040004BA RID: 1210
		public byte dir;

		// Token: 0x040004BB RID: 1211
		public byte param;

		// Token: 0x040004BC RID: 1212
		public short param1;

		// Token: 0x040004BD RID: 1213
		public int[] param2 = new int[3];

		// Token: 0x040004BE RID: 1214
		public uint targetid;

		// Token: 0x040004BF RID: 1215
		public uint injured;

		// Token: 0x040004C0 RID: 1216
		public int[] param3 = new int[10];
	}
}
