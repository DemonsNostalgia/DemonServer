using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x02000022 RID: 34
	public class ROLEDATE_EUDEMON
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00005540 File Offset: 0x00003740
		public ROLEDATE_EUDEMON()
		{
			this.list_item = new List<RoleData_Eudemon>();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005556 File Offset: 0x00003756
		public void SetLoadTag()
		{
			this.mParam = 128;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005564 File Offset: 0x00003764
		public void SetSaveTag()
		{
			this.mParam = 129;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005574 File Offset: 0x00003774
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.playerid = packIn.ReadInt32();
			this.key = packIn.ReadInt32();
			this.key2 = packIn.ReadInt32();
			int num = packIn.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				RoleData_Eudemon roleData_Eudemon = new RoleData_Eudemon();
				roleData_Eudemon.Create(null, packIn);
				this.list_item.Add(roleData_Eudemon);
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000055EC File Offset: 0x000037EC
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteInt32(this.list_item.Count);
			for (int i = 0; i < this.list_item.Count; i++)
			{
				packetOut.WriteBuff(this.list_item[i].GetBuffer());
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000E9 RID: 233
		public ushort mParam;

		// Token: 0x040000EA RID: 234
		public int playerid;

		// Token: 0x040000EB RID: 235
		public int key;

		// Token: 0x040000EC RID: 236
		public int key2;

		// Token: 0x040000ED RID: 237
		public List<RoleData_Eudemon> list_item;
	}
}
