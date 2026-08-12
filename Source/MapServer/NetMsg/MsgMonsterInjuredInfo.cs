using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005C RID: 92
	public class MsgMonsterInjuredInfo : BaseMsg
	{
		// Token: 0x060001F9 RID: 505 RVA: 0x000154BC File Offset: 0x000136BC
		public MsgMonsterInjuredInfo()
		{
			this.mMsgLen = 40;
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

		// Token: 0x060001FA RID: 506 RVA: 0x0001554C File Offset: 0x0001374C
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
				this.injuredvalue = packIn.ReadUInt32();
				for (int i = 0; i < this.param.Length; i++)
				{
					this.param[i] = packIn.ReadInt32();
				}
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0001560C File Offset: 0x0001380C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteUInt32(this.monsterid);
			packetOut.WriteInt16(this.role_x);
			packetOut.WriteInt16(this.role_y);
			packetOut.WriteUInt32(this.tag);
			packetOut.WriteUInt32(this.injuredvalue);
			for (int i = 0; i < this.param.Length; i++)
			{
				packetOut.WriteInt32(this.param[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040003F6 RID: 1014
		public int time;

		// Token: 0x040003F7 RID: 1015
		public uint roleid;

		// Token: 0x040003F8 RID: 1016
		public uint monsterid;

		// Token: 0x040003F9 RID: 1017
		public short role_x;

		// Token: 0x040003FA RID: 1018
		public short role_y;

		// Token: 0x040003FB RID: 1019
		public uint tag;

		// Token: 0x040003FC RID: 1020
		public uint injuredvalue;

		// Token: 0x040003FD RID: 1021
		public int[] param;
	}
}
