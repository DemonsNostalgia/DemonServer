using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000064 RID: 100
	public class MsgItemInfo : BaseMsg
	{
		// Token: 0x06000211 RID: 529 RVA: 0x000166E0 File Offset: 0x000148E0
		public MsgItemInfo()
		{
			this.mMsgLen = 87;
			this.mParam = 1008;
			this.tag = 1;
			this.param1 = 3;
			this.time = Environment.TickCount;
			this.param3 = 0;
			this.param10 = 0;
			this.lock_time = 0;
			this.warghost_exp = 0;
			this.param4 = 0;
			this.param5 = 0;
			this.di_attack = 0;
			this.shui_attack = 0;
			this.huo_attack = 0;
			this.feng_attack = 0;
			this.add_eff = 0;
			this.param6 = 0;
			this.param7 = 0;
			this.properties = 0;
			this.gem3 = 0;
			this.god_strong = 0;
			this.god_exp = 0;
			this.param8 = 0;
			this.pram9 = 1;
			this.name = "";
			for (int i = 0; i < this.param2.Length; i++)
			{
				this.param2[i] = 0;
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x000167DE File Offset: 0x000149DE
		public void SetTradTag()
		{
			this.tag = 2;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x000167E8 File Offset: 0x000149E8
		public void SetLookEquipTag()
		{
			this.tag = 4;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x000167F2 File Offset: 0x000149F2
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x000167FE File Offset: 0x000149FE
		public void SetLookEudemonTag()
		{
			this.tag = 7;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00016808 File Offset: 0x00014A08
		public override byte[] GetBuffer()
		{
			byte[] nameBytes = Coding.GetDefauleCoding().GetBytes(this.name);
			if (nameBytes.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Item names cannot exceed 255 encoded bytes.");
			}
			ushort wireLength = (ushort)(87 + nameBytes.Length);
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.item_id);
			packetOut.WriteUInt16(this.amount);
			packetOut.WriteUInt16(this.amount_limit);
			packetOut.WriteByte(this.tag);
			packetOut.WriteByte(this.status);
			packetOut.WriteByte(this.postion);
			packetOut.WriteByte(this.gem);
			packetOut.WriteByte(this.gem2);
			packetOut.WriteByte(this.magic);
			packetOut.WriteByte(this.magic2);
			packetOut.WriteByte(this.magic3);
			packetOut.WriteInt32(this.param3);
			packetOut.WriteInt32(this.lock_time);
			packetOut.WriteInt32(this.warghost_exp);
			packetOut.WriteInt32(this.param4);
			packetOut.WriteInt32(this.param5);
			packetOut.WriteByte(this.di_attack);
			packetOut.WriteByte(this.shui_attack);
			packetOut.WriteByte(this.huo_attack);
			packetOut.WriteByte(this.feng_attack);
			packetOut.WriteByte(this.add_eff);
			packetOut.WriteByte(this.param6);
			packetOut.WriteByte(this.param7);
			packetOut.WriteInt32(this.properties);
			packetOut.WriteInt16(this.param10);
			packetOut.WriteByte(this.gem3);
			packetOut.WriteInt32(this.god_strong);
			packetOut.WriteInt16(this.param12);
			packetOut.WriteInt32(this.god_exp);
			packetOut.WriteInt32(this.param8);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteByte(this.pram9);
			packetOut.WriteString(this.name);
			for (int i = 0; i < this.param2.Length; i++)
			{
				packetOut.WriteByte(this.param2[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x0400045C RID: 1116
		public const byte ITEMPOSITION_HELMET = 1;

		// Token: 0x0400045D RID: 1117
		public const byte ITEMPOSITION_NECKLACE = 2;

		// Token: 0x0400045E RID: 1118
		public const byte ITEMPOSITION_ARMOR = 3;

		// Token: 0x0400045F RID: 1119
		public const byte ITEMPOSITION_WEAPONR = 4;

		// Token: 0x04000460 RID: 1120
		public const byte ITEMPOSITION_WEAPONL = 5;

		// Token: 0x04000461 RID: 1121
		public const byte ITEMPOSITION_RINGR = 6;

		// Token: 0x04000462 RID: 1122
		public const byte ITEMPOSITION_TREASURE = 7;

		// Token: 0x04000463 RID: 1123
		public const byte ITEMPOSITION_SHOES = 8;

		// Token: 0x04000464 RID: 1124
		public const byte ITEMPOSITION_MOUNT = 9;

		// Token: 0x04000465 RID: 1125
		public const byte ITEMPOSITION_SPRITE = 10;

		// Token: 0x04000466 RID: 1126
		public const byte ITEMPOSITION_FASHION = 12;

		// Token: 0x04000467 RID: 1127
		public const byte ITEMPOSTION_RUB_SHUGUANGZHANHUN = 13;

		// Token: 0x04000468 RID: 1128
		public const byte ITEMPOSTION_RUB_DILONGZHILEI = 14;

		// Token: 0x04000469 RID: 1129
		public const byte ITEMPOSTION_RUB_SHENGYAOFUWEN = 15;

		// Token: 0x0400046A RID: 1130
		public const byte ITEMPOSTION_WEPON_SOUL = 26;

		// Token: 0x0400046B RID: 1131
		public const byte ITEMPOSITION_CHEST = 44;

		// Token: 0x0400046C RID: 1132
		public const byte ITEMPOSITION_CHEST_SOUL = 49;

		// Token: 0x0400046D RID: 1133
		public const byte ITEMPOSITION_BACKPACK = 50;

		// Token: 0x0400046E RID: 1134
		public const byte ITEMPOSITION_EUDEMONEGG_PACK = 52;

		// Token: 0x0400046F RID: 1135
		public const byte ITEMPOSITION_EUDEMON_PACK = 53;

		// Type-120 Batch Hatcher entries are persisted outside the normal item
		// and Eudemon bags. The active storage-position formula in the available
		// reference implementation is 200 + packageType / 10, which yields 212.
		// The definitive client never receives this database position; it sees
		// the contents only through packet 1117 type 120.
		public const ushort ITEMPOSITION_BATCH_HATCHER = 212;

		// Token: 0x04000470 RID: 1136
		public const byte ITEMPOSTION_STRONG_PACK = 100;

		// Token: 0x04000471 RID: 1137
		public const byte ITEMPOSTION_PTICH_PACK = 111;

		// Token: 0x04000472 RID: 1138
		public const byte TAG_ROLEITEM = 1;

		// Token: 0x04000473 RID: 1139
		public const byte TAG_TRADITEM = 2;

		// Token: 0x04000474 RID: 1140
		public const byte TAG_ROLEEUDEMONPACK = 3;

		// Token: 0x04000475 RID: 1141
		public const byte TAG_LOOKROLEINFO = 4;

		// Token: 0x04000476 RID: 1142
		public const byte TAG_LOOKROLEEUDEMONINFO = 7;

		// Token: 0x04000477 RID: 1143
		public int time;

		// Token: 0x04000478 RID: 1144
		public uint id;

		// Token: 0x04000479 RID: 1145
		public uint item_id;

		// Token: 0x0400047A RID: 1146
		public ushort amount;

		// Token: 0x0400047B RID: 1147
		public ushort amount_limit;

		// Token: 0x0400047C RID: 1148
		public byte tag;

		// Token: 0x0400047D RID: 1149
		public byte status;

		// Token: 0x0400047E RID: 1150
		public byte postion;

		// Token: 0x0400047F RID: 1151
		public byte gem;

		// Token: 0x04000480 RID: 1152
		public byte gem2;

		// Token: 0x04000481 RID: 1153
		public byte magic;

		// Token: 0x04000482 RID: 1154
		public byte magic2;

		// Token: 0x04000483 RID: 1155
		public byte magic3;

		// Token: 0x04000484 RID: 1156
		public int param3;

		// Token: 0x04000485 RID: 1157
		public int lock_time;

		// Token: 0x04000486 RID: 1158
		public int warghost_exp;

		// Token: 0x04000487 RID: 1159
		public int param4;

		// Token: 0x04000488 RID: 1160
		public int param5;

		// Token: 0x04000489 RID: 1161
		public byte di_attack;

		// Token: 0x0400048A RID: 1162
		public byte shui_attack;

		// Token: 0x0400048B RID: 1163
		public byte huo_attack;

		// Token: 0x0400048C RID: 1164
		public byte feng_attack;

		// Token: 0x0400048D RID: 1165
		public byte add_eff;

		// Token: 0x0400048E RID: 1166
		public byte param6;

		// Token: 0x0400048F RID: 1167
		public byte param7;

		// Token: 0x04000490 RID: 1168
		public int properties;

		// Token: 0x04000491 RID: 1169
		public short param10;

		// Token: 0x04000492 RID: 1170
		public byte gem3;

		// Token: 0x04000493 RID: 1171
		public int god_strong;

		// Token: 0x04000494 RID: 1172
		public short param12;

		// Token: 0x04000495 RID: 1173
		public int god_exp;

		// Token: 0x04000496 RID: 1174
		public int param8;

		// Token: 0x04000497 RID: 1175
		public int param1;

		// Token: 0x04000498 RID: 1176
		public byte[] param2 = new byte[3];

		// Token: 0x04000499 RID: 1177
		public byte pram9;

		// Token: 0x0400049A RID: 1178
		public string name;
	}
}
