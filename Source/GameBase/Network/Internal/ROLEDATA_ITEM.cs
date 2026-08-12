using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001B RID: 27
	public class ROLEDATA_ITEM
	{
		private const ushort EudemonBagPosition = 53;

		// The type-120 Batch Hatcher persists completed Eudemons at position
		// 212. Those Eudemon rows must be loaded on relog even though the items
		// are exposed to the client only through packet 1117 type 120.
		private const ushort BatchHatcherPosition = 212;

		// Token: 0x0600006D RID: 109 RVA: 0x00004814 File Offset: 0x00002A14
		public ROLEDATA_ITEM()
		{
			this.key = 0;
			this.key2 = 0;
			this.mListItem = new List<RoleData_Item>();
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004838 File Offset: 0x00002A38
		public void SetLoadTag()
		{
			this.mParam = 123;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004843 File Offset: 0x00002A43
		public void SetSaveTag()
		{
			this.mParam = 124;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00004850 File Offset: 0x00002A50
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
				RoleData_Item roleData_Item = new RoleData_Item();
				roleData_Item.Create(null, packIn);
				this.mListItem.Add(roleData_Item);
			}
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000048C8 File Offset: 0x00002AC8
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.playerid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteInt32(this.mListItem.Count);
			for (int i = 0; i < this.mListItem.Count; i++)
			{
				RoleData_Item roleData_Item = this.mListItem[i];
				packetOut.WriteBuff(roleData_Item.GetBuffer());
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004980 File Offset: 0x00002B80
		public List<RoleData_Item> GetEudemonItemList()
		{
			List<RoleData_Item> list = null;
			for (int i = 0; i < this.mListItem.Count; i++)
			{
				if (this.mListItem[i].postion == EudemonBagPosition ||
					this.mListItem[i].postion == BatchHatcherPosition)
				{
					if (list == null)
					{
						list = new List<RoleData_Item>();
					}
					list.Add(this.mListItem[i]);
				}
			}
			return list;
		}

		// Token: 0x0400009E RID: 158
		public ushort mParam;

		// Token: 0x0400009F RID: 159
		public int playerid;

		// Token: 0x040000A0 RID: 160
		public int key;

		// Token: 0x040000A1 RID: 161
		public int key2;

		// Token: 0x040000A2 RID: 162
		public List<RoleData_Item> mListItem;
	}
}
