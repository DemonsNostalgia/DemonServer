using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x02000028 RID: 40
	public class GUANJUEINFO
	{
		// Token: 0x0600009F RID: 159 RVA: 0x00005AA4 File Offset: 0x00003CA4
		public GUANJUEINFO()
		{
			this.mparam = 132;
			this.list_item = new List<GuanJueInfo>();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005AC8 File Offset: 0x00003CC8
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			int num = packIn.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				GuanJueInfo guanJueInfo = new GuanJueInfo();
				guanJueInfo.Create(packIn);
				this.list_item.Add(guanJueInfo);
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005B1C File Offset: 0x00003D1C
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mparam);
			packetOut.WriteInt32(this.list_item.Count);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				packetOut.WriteBuff(this.list_item[i].GetBuffer());
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000101 RID: 257
		public ushort mparam;

		// Token: 0x04000102 RID: 258
		public List<GuanJueInfo> list_item;
	}
}
