using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x0200002C RID: 44
	public class LEGIONINFO
	{
		// Token: 0x060000AB RID: 171 RVA: 0x00005E64 File Offset: 0x00004064
		public LEGIONINFO()
		{
			this.mParam = 134;
			this.list_item = new List<LegionInfo>();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005E88 File Offset: 0x00004088
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadInt16();
			int num = packIn.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				LegionInfo legionInfo = new LegionInfo();
				legionInfo.Create(packIn);
				this.list_item.Add(legionInfo);
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00005EDC File Offset: 0x000040DC
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.list_item.Count);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				packetOut.WriteBuff(this.list_item[i].GetBuffer());
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000112 RID: 274
		public ushort mParam;

		// Token: 0x04000113 RID: 275
		public List<LegionInfo> list_item;
	}
}
