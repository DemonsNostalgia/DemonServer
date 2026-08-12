using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005B RID: 91
	public class MsgAttackInfo : BaseMsg
	{
		// Token: 0x060001F6 RID: 502 RVA: 0x0001529C File Offset: 0x0001349C
		public MsgAttackInfo()
		{
			this.mMsgLen = 40;
			this.mParam = 1022;
			this.param = new byte[12];
			this.roleId = (this.idTarget = 0U);
			this.usPosX = (this.usPosY = 0);
			this.tag = 0U;
			this.skillid = 0;
			this.usType = 0U;
			this.time = 0;
			for (int i = 0; i < this.param.Length; i++)
			{
				this.param[i] = 0;
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00015330 File Offset: 0x00013530
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.time = packIn.ReadInt32();
				this.roleId = packIn.ReadUInt32();
				this.idTarget = packIn.ReadUInt32();
				this.usPosX = packIn.ReadUInt16();
				this.usPosY = packIn.ReadUInt16();
				this.tag = packIn.ReadUInt32();
				this.usType = packIn.ReadUInt32();
				int num = 12;
				if (this.tag == 2U)
				{
					num = 11;
				}
				for (int i = 0; i < num; i++)
				{
					this.param[i] = packIn.ReadByte();
				}
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x000153EC File Offset: 0x000135EC
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleId);
			packetOut.WriteUInt32(this.idTarget);
			packetOut.WriteUInt16(this.usPosX);
			packetOut.WriteUInt16(this.usPosY);
			packetOut.WriteUInt32(this.tag);
			packetOut.WriteUInt16(this.skillid);
			packetOut.WriteUInt32(this.usType);
			for (int i = 0; i < this.param.Length; i++)
			{
				packetOut.WriteByte(this.param[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040003ED RID: 1005
		public int time;

		// Token: 0x040003EE RID: 1006
		public uint roleId;

		// Token: 0x040003EF RID: 1007
		public uint idTarget;

		// Token: 0x040003F0 RID: 1008
		public ushort usPosX;

		// Token: 0x040003F1 RID: 1009
		public ushort usPosY;

		// Token: 0x040003F2 RID: 1010
		public uint tag;

		// Token: 0x040003F3 RID: 1011
		public ushort skillid;

		// Token: 0x040003F4 RID: 1012
		public uint usType;

		// Token: 0x040003F5 RID: 1013
		public byte[] param;
	}
}
