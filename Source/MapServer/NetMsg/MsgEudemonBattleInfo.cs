using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000082 RID: 130
	public class MsgEudemonBattleInfo : BaseMsg
	{
		// Token: 0x06000281 RID: 641 RVA: 0x00019F24 File Offset: 0x00018124
		public MsgEudemonBattleInfo()
		{
			this.mMsgLen = 128;
			this.mParam = 1116;
			this.name = "";
		}

		// Token: 0x06000282 RID: 642 RVA: 0x00019F88 File Offset: 0x00018188
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.name);
			this.mMsgLen += (ushort)bytes.Length;
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteBuff(this.param);
			packetOut.WriteUInt32(this.play_id);
			packetOut.WriteInt32(this.life);
			packetOut.WriteInt32(this.life_max);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteInt16(this.dir);
			packetOut.WriteByte(this.wuxing);
			packetOut.WriteBuff(this.param1);
			packetOut.WriteUInt32(this.monsterid);
			packetOut.WriteInt32(this.param3);
			packetOut.WriteInt32(this.param4);
			packetOut.WriteInt32(this.param5);
			packetOut.WriteInt32(this.star);
			packetOut.WriteBuff(this.param2);
			packetOut.WriteByte(this.count);
			packetOut.WriteString(this.name);
			packetOut.WriteInt16(0);
			return packetOut.Flush();
		}

		// Token: 0x040005A7 RID: 1447
		public uint id;

		// Token: 0x040005A8 RID: 1448
		public uint lookface;

		// Token: 0x040005A9 RID: 1449
		public byte[] param = new byte[32];

		// Token: 0x040005AA RID: 1450
		public uint play_id;

		// Token: 0x040005AB RID: 1451
		public int life;

		// Token: 0x040005AC RID: 1452
		public int life_max;

		// Token: 0x040005AD RID: 1453
		public short x;

		// Token: 0x040005AE RID: 1454
		public short y;

		// Token: 0x040005AF RID: 1455
		public short dir;

		// Token: 0x040005B0 RID: 1456
		public byte wuxing;

		// Token: 0x040005B1 RID: 1457
		public byte[] param1 = new byte[5];

		// Token: 0x040005B2 RID: 1458
		public uint monsterid;

		// Token: 0x040005B3 RID: 1459
		public int param3;

		// Token: 0x040005B4 RID: 1460
		public int param4;

		// Token: 0x040005B5 RID: 1461
		public int param5;

		// Token: 0x040005B6 RID: 1462
		public int star;

		// Token: 0x040005B7 RID: 1463
		public byte[] param2 = new byte[37];

		// Token: 0x040005B8 RID: 1464
		public byte count = 1;

		// Token: 0x040005B9 RID: 1465
		public string name;
	}
}
