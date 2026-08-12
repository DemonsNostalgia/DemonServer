using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x02000024 RID: 36
	public class ROLEDATA_FRIEND
	{
		// Token: 0x06000091 RID: 145 RVA: 0x0000574A File Offset: 0x0000394A
		public ROLEDATA_FRIEND()
		{
			this.list_item = new List<RoleData_Friend>();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005760 File Offset: 0x00003960
		public void SetLoadTag()
		{
			this.mParam = 130;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000576E File Offset: 0x0000396E
		public void SetSaveTag()
		{
			this.mParam = 131;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000577C File Offset: 0x0000397C
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
				RoleData_Friend roleData_Friend = new RoleData_Friend();
				roleData_Friend.Create(null, packIn);
				this.list_item.Add(roleData_Friend);
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000057F4 File Offset: 0x000039F4
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

		// Token: 0x040000F2 RID: 242
		public ushort mParam;

		// Token: 0x040000F3 RID: 243
		public int playerid;

		// Token: 0x040000F4 RID: 244
		public int key;

		// Token: 0x040000F5 RID: 245
		public int key2;

		// Token: 0x040000F6 RID: 246
		public List<RoleData_Friend> list_item;
	}
}
