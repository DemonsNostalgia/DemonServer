using System;

namespace GameBase.Network.Internal
{
	// Token: 0x0200002D RID: 45
	public class LegionOption
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00005F66 File Offset: 0x00004166
		public LegionOption()
		{
			this.mInfo = new LegionInfo();
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005F7C File Offset: 0x0000417C
		public void SetCreateTag()
		{
			this.mParam = 137;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005F8A File Offset: 0x0000418A
		public void SetUpdateTag()
		{
			this.mParam = 136;
		}

		public void SetDeleteTag()
		{
			this.mParam = 141;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005F98 File Offset: 0x00004198
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			packIn.ReadUInt16();
			this.player_id = packIn.ReadInt32();
			this.mInfo.Create(packIn);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005FD0 File Offset: 0x000041D0
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.player_id);
			packetOut.WriteBuff(this.mInfo.GetBuffer());
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x04000114 RID: 276
		public ushort mParam;

		// Token: 0x04000115 RID: 277
		public int player_id;

		// Token: 0x04000116 RID: 278
		public LegionInfo mInfo;
	}
}
