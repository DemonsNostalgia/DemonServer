using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000087 RID: 135
	public class MsgGuanJueInfo : BaseMsg
	{
		// Token: 0x0600028C RID: 652 RVA: 0x0001A3F8 File Offset: 0x000185F8
		public MsgGuanJueInfo()
		{
			this.mParam = 2060;
			this.mMsgLen = 22;
			this.list_item = new List<MsgGuanJueItem>();
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0001A448 File Offset: 0x00018648
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			ushort wireLength =
				(ushort)(22 + this.list_item.Count * 40);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt16(this.param);
			packetOut.WriteInt32(this.page);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt16(this.param2);
			packetOut.WriteInt16((short)this.list_item.Count);
			packetOut.WriteInt32(this.param3);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.list_item[i].name);
				byte[] array;
				if (bytes.Length > 15)
				{
					array = new byte[15];
					Buffer.BlockCopy(bytes, 0, array, 0, 15);
				}
				else
				{
					array = new byte[bytes.Length];
					Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
				}
				packetOut.WriteBuff(array);
				byte[] v = new byte[16 - array.Length];
				packetOut.WriteBuff(v);
				packetOut.WriteInt32(0);
				packetOut.WriteULong(this.list_item[i].guanjue);
				packetOut.WriteInt32(1);
				packetOut.WriteInt32(this.list_item[i].pos);
				packetOut.WriteInt32(0);
			}
			return packetOut.Flush();
		}

		// Token: 0x040005DA RID: 1498
		public short param = 2;

		// Token: 0x040005DB RID: 1499
		public int page;

		// Token: 0x040005DC RID: 1500
		public int param1 = 5;

		// Token: 0x040005DD RID: 1501
		public short param2 = 0;

		// Token: 0x040005DE RID: 1502
		public int param3 = 0;

		// Token: 0x040005DF RID: 1503
		public List<MsgGuanJueItem> list_item;
	}
}
