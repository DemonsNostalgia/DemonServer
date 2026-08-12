using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200007D RID: 125
	public class MsgOpenDialog : BaseMsg
	{
		// Token: 0x0600026F RID: 623 RVA: 0x00019871 File Offset: 0x00017A71
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0001987D File Offset: 0x00017A7D
		public void SetDialogType(int dwData)
		{
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00019880 File Offset: 0x00017A80
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(28);
			packetOut.WriteUInt16(1010);
			packetOut.WriteUInt32(this.playid);
			packetOut.WriteUInt32(this.npcid);
			packetOut.WriteInt16(this.npc_x);
			packetOut.WriteInt16(this.npc_y);
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(this.dialog_type);
			packetOut.WriteInt32(9596);
			return packetOut.Flush();
		}

		// Token: 0x04000584 RID: 1412
		public const int OPENDIALOGTYPE_STRONG = 3;

		// Token: 0x04000585 RID: 1413
		public uint playid;

		// Token: 0x04000586 RID: 1414
		public uint npcid;

		// Token: 0x04000587 RID: 1415
		public short npc_x;

		// Token: 0x04000588 RID: 1416
		public short npc_y;

		// Token: 0x04000589 RID: 1417
		public int dialog_type;
	}
}
