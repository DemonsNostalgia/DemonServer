using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	// Token: 0x0200001C RID: 28
	public class RoleData_Magic
	{
		// Token: 0x06000073 RID: 115 RVA: 0x000049F4 File Offset: 0x00002BF4
		public RoleData_Magic()
		{
			this.mListMagic = new List<MagicInfo>();
			this.key = (this.key2 = (this.ownerid = 0));
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004A2E File Offset: 0x00002C2E
		public void SetLoadTag()
		{
			this.mParam = 125;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004A39 File Offset: 0x00002C39
		public void SetSaveTag()
		{
			this.mParam = 126;
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004A44 File Offset: 0x00002C44
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.ownerid = packIn.ReadInt32();
			this.key = packIn.ReadInt32();
			this.key2 = packIn.ReadInt32();
			int num = packIn.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				MagicInfo magicInfo = new MagicInfo();
				magicInfo.id = packIn.ReadInt32();
				magicInfo.magicid = packIn.ReadUInt32();
				magicInfo.level = packIn.ReadByte();
				magicInfo.exp = packIn.ReadUInt32();
				this.mListMagic.Add(magicInfo);
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004AE4 File Offset: 0x00002CE4
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.ownerid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteInt32(this.mListMagic.Count);
			for (int i = 0; i < this.mListMagic.Count; i++)
			{
				MagicInfo magicInfo = this.mListMagic[i];
				packetOut.WriteInt32(magicInfo.id);
				packetOut.WriteUInt32(magicInfo.magicid);
				packetOut.WriteByte(magicInfo.level);
				packetOut.WriteUInt32(magicInfo.exp);
			}
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x040000A3 RID: 163
		public ushort mParam;

		// Token: 0x040000A4 RID: 164
		public int ownerid;

		// Token: 0x040000A5 RID: 165
		public int key;

		// Token: 0x040000A6 RID: 166
		public int key2;

		// Token: 0x040000A7 RID: 167
		public List<MagicInfo> mListMagic;
	}
}
