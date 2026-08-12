using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network;
using GameStruct;
using MapServer;

namespace NetMsg
{
	// Token: 0x0200007F RID: 127
	public class MsgStrongInfo : BaseMsg
	{
		// Token: 0x06000275 RID: 629 RVA: 0x000199A4 File Offset: 0x00017BA4
		public MsgStrongInfo()
		{
			this.tag = 1005;
			this.type = 10;
			this.param2 = 100;
			this.list_item = new List<RoleItemInfo>();
			this.mMsgLen = 24;
			this.mParam = 1102;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x000199F3 File Offset: 0x00017BF3
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00019A00 File Offset: 0x00017C00
		public static byte[] GetStrongMoneyBuffer(uint playid, int strong_gold)
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteInt16(28);
			packetOut.WriteInt16(1009);
			packetOut.WriteUInt32(playid);
			packetOut.WriteInt32(strong_gold);
			packetOut.WriteInt32(9);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(0);
			return packetOut.Flush();
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00019A68 File Offset: 0x00017C68
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			this.mMsgLen += (ushort)(152 * this.list_item.Count);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.tag);
			packetOut.WriteByte(this.param1);
			packetOut.WriteByte(this.type);
			packetOut.WriteInt16(this.action);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteUInt32(this.playid);
			packetOut.WriteInt32(this.list_item.Count);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				RoleItemInfo roleItemInfo = this.list_item[i];
				ItemTypeInfo itemTypeInfo = ConfigManager.Instance().GetItemTypeInfo(this.list_item[i].itemid);
				if (itemTypeInfo == null)
				{
					byte[] v = new byte[152];
					packetOut.WriteBuff(v);
				}
				else
				{
					packetOut.WriteUInt32(roleItemInfo.id);
					packetOut.WriteUInt32(roleItemInfo.itemid);
					packetOut.WriteUInt16(roleItemInfo.amount);
					packetOut.WriteUInt16(itemTypeInfo.amount_limit);
					packetOut.WriteByte(0);
					packetOut.WriteByte((byte)roleItemInfo.gem1);
					packetOut.WriteByte((byte)roleItemInfo.gem2);
					packetOut.WriteByte(0);
					packetOut.WriteByte(0);
					packetOut.WriteByte(roleItemInfo.GetStrongLevel());
					packetOut.WriteByte(0);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(roleItemInfo.war_ghost_exp);
					packetOut.WriteInt32(0);
					packetOut.WriteInt32(0);
					packetOut.WriteByte(roleItemInfo.di_attack);
					packetOut.WriteByte(roleItemInfo.shui_attack);
					packetOut.WriteByte(roleItemInfo.huo_attack);
					packetOut.WriteByte(roleItemInfo.feng_attack);
					packetOut.WriteByte(0);
					packetOut.WriteByte(0);
					packetOut.WriteByte(0);
					packetOut.WriteInt16(0);
					packetOut.WriteInt32(0);
					packetOut.WriteByte((byte)roleItemInfo.gem3);
					packetOut.WriteInt32(roleItemInfo.god_strong);
					packetOut.WriteInt16((short)roleItemInfo.god_exp);
					packetOut.WriteInt32(0);
					byte[] v = new byte[21];
					packetOut.WriteBuff(v);
					byte[] bytes = Coding.GetDefauleCoding().GetBytes(itemTypeInfo.name);
					packetOut.WriteBuff(bytes);
					v = new byte[68 - bytes.Length];
					packetOut.WriteBuff(v);
				}
			}
			return packetOut.Flush();
		}

		// Token: 0x04000596 RID: 1430
		public int tag;

		// Token: 0x04000597 RID: 1431
		public byte param1;

		// Token: 0x04000598 RID: 1432
		public byte type;

		// Token: 0x04000599 RID: 1433
		public short action;

		// Token: 0x0400059A RID: 1434
		public int param2;

		// Token: 0x0400059B RID: 1435
		public uint playid;

		// Token: 0x0400059C RID: 1436
		public List<RoleItemInfo> list_item;
	}
}
