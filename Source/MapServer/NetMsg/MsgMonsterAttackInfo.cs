using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000060 RID: 96
	public class MsgMonsterAttackInfo : BaseMsg
	{
		// Token: 0x06000205 RID: 517 RVA: 0x00015CE8 File Offset: 0x00013EE8
		public MsgMonsterAttackInfo()
		{
			this.mMsgLen = 40;
			this.tag = 2U;
			this.mParam = 1022;
			this.param = new int[3];
			for (int i = 0; i < this.param.Length; i++)
			{
				this.param[i] = 0;
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x00015D46 File Offset: 0x00013F46
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00015D54 File Offset: 0x00013F54
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.time);
			packetOut.WriteUInt32(this.monsterid);
			packetOut.WriteUInt32(this.roleid);
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

		// Token: 0x04000417 RID: 1047
		public int time;

		// Token: 0x04000418 RID: 1048
		public uint monsterid;

		// Token: 0x04000419 RID: 1049
		public uint roleid;

		// Token: 0x0400041A RID: 1050
		public short role_x;

		// Token: 0x0400041B RID: 1051
		public short role_y;

		// Token: 0x0400041C RID: 1052
		public uint tag;

		// Token: 0x0400041D RID: 1053
		public uint injuredvalue;

		// Token: 0x0400041E RID: 1054
		public int[] param;
	}
}
