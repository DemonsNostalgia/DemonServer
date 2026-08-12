using System;
using System.Collections.Generic;
using GameBase.Network;
using GameStruct;

namespace NetMsg
{
	// Token: 0x0200007A RID: 122
	public class MsgUserAttribute : BaseMsg
	{
		// Token: 0x06000264 RID: 612 RVA: 0x000194E5 File Offset: 0x000176E5
		public MsgUserAttribute()
		{
			this.amount = 0;
			this.list_type = new List<UserAttribute>();
			this.list_value = new List<uint>();
			this.mParam = 1017;
			this.mMsgLen = 12;
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00019520 File Offset: 0x00017720
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0001952C File Offset: 0x0001772C
		public override byte[] GetBuffer()
		{
			if (this.list_type.Count != this.list_value.Count)
			{
				throw new InvalidOperationException(
					"Attribute type and value counts do not match.");
			}
			this.amount = this.list_type.Count;
			ushort wireLength = (ushort)(12 + this.amount * 8);
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.role_id);
			packetOut.WriteInt32(this.amount);
			for (int i = 0; i < this.amount; i++)
			{
				packetOut.WriteInt32((int)this.list_type[i]);
				packetOut.WriteUInt32(this.list_value[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000195F0 File Offset: 0x000177F0
		public void AddAttribute(UserAttribute Attribute, uint value)
		{
			this.amount++;
			this.list_type.Add(Attribute);
			this.list_value.Add(value);
		}

		// Token: 0x04000566 RID: 1382
		public uint role_id;

		// Token: 0x04000567 RID: 1383
		public int amount;

		// Token: 0x04000568 RID: 1384
		public List<UserAttribute> list_type;

		// Token: 0x04000569 RID: 1385
		public List<uint> list_value;
	}
}
