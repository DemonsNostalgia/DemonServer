using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006A RID: 106
	public class MsgMagicAttackInfoEx : BaseMsg
	{
		// Token: 0x06000228 RID: 552 RVA: 0x000170B4 File Offset: 0x000152B4
		public MsgMagicAttackInfoEx()
		{
			this.mMsgLen = 54;
			this.mParam = 1105;
			this.roleid = 0U;
			this.x = (this.y = 0);
			this.magicid = (this.magiclv = 0);
			this.dir = (this.param = 0);
			for (int i = 0; i < this.param1.Length; i++)
			{
				this.param1[i] = 0;
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00017143 File Offset: 0x00015343
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x00017150 File Offset: 0x00015350
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteUInt16(this.magicid);
			packetOut.WriteUInt16(this.magiclv);
			packetOut.WriteByte(this.dir);
			packetOut.WriteByte(this.param);
			for (int i = 0; i < this.param1.Length; i++)
			{
				packetOut.WriteInt32(this.param1[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x040004C1 RID: 1217
		public uint roleid;

		// Token: 0x040004C2 RID: 1218
		public short x;

		// Token: 0x040004C3 RID: 1219
		public short y;

		// Token: 0x040004C4 RID: 1220
		public ushort magicid;

		// Token: 0x040004C5 RID: 1221
		public ushort magiclv;

		// Token: 0x040004C6 RID: 1222
		public byte dir;

		// Token: 0x040004C7 RID: 1223
		public byte param;

		// Token: 0x040004C8 RID: 1224
		public int[] param1 = new int[9];
	}
}
