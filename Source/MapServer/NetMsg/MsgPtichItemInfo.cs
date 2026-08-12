using System;
using GameBase.Core;
using GameBase.Network;
using GameStruct;

namespace NetMsg
{
	// Token: 0x02000063 RID: 99
	public class MsgPtichItemInfo : BaseMsg
	{
		// Token: 0x0600020E RID: 526 RVA: 0x0001631C File Offset: 0x0001451C
		public MsgPtichItemInfo(RoleItemInfo item, uint _ptich_obj_id, int _price, byte sell_byte, bool isRemote = false)
		{
			this.mMsgLen = 88;
			this.mParam = 1108;
			if (item.typeid >= 2000000000U)
			{
				this.postion = 53;
				this.id = item.typeid;
				this.max_dura = (this.cur_dura = 0);
			}
			else
			{
				this.id = item.id;
				this.postion = 50;
				this.max_dura = (this.cur_dura = (short)item.amount);
			}
			this.forgetname = item.forgename;
			this.ptich_obj_id = _ptich_obj_id;
			this.price = _price;
			this.base_item_id = item.itemid;
			this.gem1 = item.GetGemType(0);
			this.gem2 = item.GetGemType(1);
			this.gem3 = item.GetGemType(2);
			this.strong_lv = item.GetStrongLevel();
			this.soul_lv = (short)item.war_ghost_exp;
			this.di_attack = item.di_attack;
			this.shui_attack = item.shui_attack;
			this.huo_attack = item.huo_attack;
			this.feng_attack = item.feng_attack;
			if (isRemote)
			{
				if (sell_byte == 52)
				{
					this.tag = 5;
				}
				else
				{
					this.tag = 4;
				}
			}
			else if (sell_byte == 52)
			{
				this.tag = 3;
			}
			else
			{
				this.tag = 1;
			}
		}

		// Token: 0x0600020F RID: 527 RVA: 0x000164CC File Offset: 0x000146CC
		public override byte[] GetBuffer()
		{
			if (this.forgetname.Length > 0)
			{
				this.mMsgLen = (ushort)((int)this.mMsgLen + Coding.GetDefauleCoding().GetBytes(this.forgetname).Length + 1);
			}
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteUInt32(this.ptich_obj_id);
			packetOut.WriteInt32(this.price);
			packetOut.WriteUInt32(this.base_item_id);
			packetOut.WriteInt16(this.max_dura);
			packetOut.WriteInt16(this.cur_dura);
			packetOut.WriteByte(this.tag);
			packetOut.WriteByte(this.status);
			packetOut.WriteByte(this.postion);
			packetOut.WriteByte(this.gem1);
			packetOut.WriteByte(this.gem2);
			packetOut.WriteInt16(this.param1);
			packetOut.WriteByte(this.strong_lv);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteInt32(this.param3);
			packetOut.WriteInt16(this.soul_lv);
			packetOut.WriteBuff(this.param4);
			packetOut.WriteByte(this.di_attack);
			packetOut.WriteByte(this.shui_attack);
			packetOut.WriteByte(this.huo_attack);
			packetOut.WriteByte(this.feng_attack);
			packetOut.WriteByte(this.effect);
			packetOut.WriteByte(this.gem3);
			packetOut.WriteBuff(this.param5);
			if (this.forgetname.Length > 0)
			{
				packetOut.WriteInt32(0);
				packetOut.WriteInt16(0);
				packetOut.WriteByte(1);
				packetOut.WriteString(this.forgetname);
				packetOut.WriteByte(0);
				packetOut.WriteByte(0);
				packetOut.WriteByte(0);
			}
			else
			{
				packetOut.WriteInt32(0);
			}
			return packetOut.Flush();
		}

		// Token: 0x04000442 RID: 1090
		public static byte _tag = 5;

		// Token: 0x04000443 RID: 1091
		private uint id;

		// Token: 0x04000444 RID: 1092
		private uint ptich_obj_id;

		// Token: 0x04000445 RID: 1093
		private int price;

		// Token: 0x04000446 RID: 1094
		private uint base_item_id;

		// Token: 0x04000447 RID: 1095
		private short max_dura;

		// Token: 0x04000448 RID: 1096
		private short cur_dura;

		// Token: 0x04000449 RID: 1097
		private byte tag;

		// Token: 0x0400044A RID: 1098
		private byte status = 0;

		// Token: 0x0400044B RID: 1099
		private byte postion;

		// Token: 0x0400044C RID: 1100
		private byte gem1;

		// Token: 0x0400044D RID: 1101
		private byte gem2;

		// Token: 0x0400044E RID: 1102
		private short param1 = 0;

		// Token: 0x0400044F RID: 1103
		private byte strong_lv;

		// Token: 0x04000450 RID: 1104
		private int param2 = 0;

		// Token: 0x04000451 RID: 1105
		private int param3 = 0;

		// Token: 0x04000452 RID: 1106
		private short soul_lv;

		// Token: 0x04000453 RID: 1107
		private byte[] param4 = new byte[10];

		// Token: 0x04000454 RID: 1108
		private byte di_attack;

		// Token: 0x04000455 RID: 1109
		private byte shui_attack;

		// Token: 0x04000456 RID: 1110
		private byte huo_attack;

		// Token: 0x04000457 RID: 1111
		private byte feng_attack;

		// Token: 0x04000458 RID: 1112
		private byte effect = 0;

		// Token: 0x04000459 RID: 1113
		private byte gem3;

		// Token: 0x0400045A RID: 1114
		private byte[] param5 = new byte[20];

		// Token: 0x0400045B RID: 1115
		private string forgetname;
	}
}
