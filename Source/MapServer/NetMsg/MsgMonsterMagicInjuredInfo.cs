using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005D RID: 93
	public class MsgMonsterMagicInjuredInfo : BaseMsg
	{
		// Token: 0x060001FC RID: 508 RVA: 0x000156D0 File Offset: 0x000138D0
		public MsgMonsterMagicInjuredInfo()
		{
			this.mMsgLen = 44;
			this.mParam = 1022;
			this.time = 0;
			this.roleid = (this.monsterid = (this.injuredvalue = 0U));
			this.role_x = (this.role_y = 0);
			this.tag = 2U;
			this.param = new int[3];
			for (int i = 0; i < this.param.Length; i++)
			{
				this.param[i] = 0;
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00015760 File Offset: 0x00013960
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null && msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.time = packIn.ReadInt32();
				this.roleid = packIn.ReadUInt32();
				this.monsterid = packIn.ReadUInt32();
				this.role_x = packIn.ReadInt16();
				this.role_y = packIn.ReadInt16();
				this.tag = packIn.ReadUInt32();
				this.magicid = packIn.ReadUInt16();
				this.magiclv = packIn.ReadUInt16();
				this.injuredvalue = packIn.ReadUInt32();
				for (int i = 0; i < this.param.Length; i++)
				{
					this.param[i] = packIn.ReadInt32();
				}
			}
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00015838 File Offset: 0x00013A38
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			if (this.injuredvalue == 0U)
			{
				this.mMsgLen = 40;
			}
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteUInt32(this.monsterid);
			packetOut.WriteInt16(this.role_x);
			packetOut.WriteInt16(this.role_y);
			packetOut.WriteUInt32(this.tag);
			packetOut.WriteUInt16(this.magicid);
			packetOut.WriteUInt16(this.magiclv);
			if (this.injuredvalue > 0U)
			{
				packetOut.WriteUInt32(this.injuredvalue);
			}
			for (int i = 0; i < this.param.Length; i++)
			{
				packetOut.WriteInt32(this.param[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040003FE RID: 1022
		public int time;

		// Token: 0x040003FF RID: 1023
		public uint roleid;

		// Token: 0x04000400 RID: 1024
		public uint monsterid;

		// Token: 0x04000401 RID: 1025
		public short role_x;

		// Token: 0x04000402 RID: 1026
		public short role_y;

		// Token: 0x04000403 RID: 1027
		public uint tag;

		// Token: 0x04000404 RID: 1028
		public ushort magicid;

		// Token: 0x04000405 RID: 1029
		public ushort magiclv;

		// Token: 0x04000406 RID: 1030
		public uint injuredvalue;

		// Token: 0x04000407 RID: 1031
		public int[] param;
	}
}
