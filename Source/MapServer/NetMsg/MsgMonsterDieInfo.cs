using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005E RID: 94
	public class MsgMonsterDieInfo : BaseMsg
	{
		// Token: 0x060001FF RID: 511 RVA: 0x00015940 File Offset: 0x00013B40
		public MsgMonsterDieInfo()
		{
			this.mMsgLen = 40;
			this.mParam = 1022;
			this.time = 0;
			this.roleid = (this.monsterid = (this.injuredvalue = 0U));
			this.role_x = (this.role_y = 0);
			this.tag = 14U;
			this.param = new int[3];
			for (int i = 0; i < this.param.Length; i++)
			{
				this.param[i] = 0;
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x000159D0 File Offset: 0x00013BD0
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

		// Token: 0x06000201 RID: 513 RVA: 0x00015A90 File Offset: 0x00013C90
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

		// Token: 0x04000408 RID: 1032
		public int time;

		// Token: 0x04000409 RID: 1033
		public uint roleid;

		// Token: 0x0400040A RID: 1034
		public uint monsterid;

		// Token: 0x0400040B RID: 1035
		public short role_x;

		// Token: 0x0400040C RID: 1036
		public short role_y;

		// Token: 0x0400040D RID: 1037
		public uint tag;

		// Token: 0x0400040E RID: 1038
		public uint injuredvalue;

		// Token: 0x0400040F RID: 1039
		public int[] param;
	}
}
