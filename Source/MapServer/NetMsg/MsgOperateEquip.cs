using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006F RID: 111
	public class MsgOperateEquip : BaseMsg
	{
		// Token: 0x0600023B RID: 571 RVA: 0x0001791C File Offset: 0x00015B1C
		public MsgOperateEquip()
		{
			this.mMsgLen = 28;
			this.mParam = 1009;
			this.equipid = 0U;
			this.postion = 0;
			this.tag = (this.param = (this.param1 = (this.param2 = 0)));
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00017975 File Offset: 0x00015B75
		public void SetTagEquip()
		{
			this.tag = 5;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0001797F File Offset: 0x00015B7F
		public void SetTagUnEquip()
		{
			this.tag = 6;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00017989 File Offset: 0x00015B89
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00017998 File Offset: 0x00015B98
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.equipid);
			packetOut.WriteInt32(this.postion);
			packetOut.WriteInt32(this.tag);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.param2);
			return packetOut.Flush();
		}

		// Token: 0x04000513 RID: 1299
		public uint equipid;

		// Token: 0x04000514 RID: 1300
		public int postion;

		// Token: 0x04000515 RID: 1301
		public int tag;

		// Token: 0x04000516 RID: 1302
		public int param;

		// Token: 0x04000517 RID: 1303
		public int param1;

		// Token: 0x04000518 RID: 1304
		public int param2;
	}
}
