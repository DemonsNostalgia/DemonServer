using System;
using System.Collections.Generic;
using GameBase.Network;
using GameStruct;

namespace NetMsg
{
	// Token: 0x02000080 RID: 128
	public class MsgEudemonInfo : BaseMsg
	{
		// Token: 0x06000279 RID: 633 RVA: 0x00019D18 File Offset: 0x00017F18
		public MsgEudemonInfo()
		{
			this.mParam = 2037;
			this.mMsgLen = 16;
			this.list_item = new List<EudemonAttribute>();
			this.list_value = new List<int>();
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00019D53 File Offset: 0x00017F53
		public void AddAttribute(EudemonAttribute attr, int value)
		{
			this.list_item.Add(attr);
			this.list_value.Add(value);
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00019D70 File Offset: 0x00017F70
		public override byte[] GetBuffer()
		{
			if (this.list_item.Count != this.list_value.Count)
			{
				throw new InvalidOperationException(
					"Eudemon attribute and value counts do not match.");
			}
			PacketOut packetOut = new PacketOut(this.mKey);
			ushort wireLength =
				(ushort)(16 + this.list_value.Count * 8);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.tag);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.list_item.Count);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				packetOut.WriteInt32((int)this.list_item[i]);
				packetOut.WriteInt32(this.list_value[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x0400059D RID: 1437
		private List<EudemonAttribute> list_item;

		// Token: 0x0400059E RID: 1438
		private List<int> list_value;

		// Token: 0x0400059F RID: 1439
		public int tag = 1;

		// Token: 0x040005A0 RID: 1440
		public uint id;
	}
}
