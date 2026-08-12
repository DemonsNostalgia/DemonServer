using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000089 RID: 137
	public class MsgLegionName : BaseMsg
	{
		// Token: 0x06000291 RID: 657 RVA: 0x0001A7AA File Offset: 0x000189AA
		public MsgLegionName()
		{
			this.mParam = 1015;
			this.legion_name = "";
			this.mMsgLen = 13;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0001A7D3 File Offset: 0x000189D3
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0001A7E0 File Offset: 0x000189E0
		public override byte[] GetBuffer()
		{
			byte[] nameBytes =
				Coding.GetDefauleCoding().GetBytes(this.legion_name);
			if (nameBytes.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Legion names cannot exceed 255 encoded bytes.");
			}
			ushort wireLength = (ushort)(13 + nameBytes.Length);
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.legion_id);
			packetOut.WriteUInt16(3);
			packetOut.WriteByte(1);
			packetOut.WriteString(this.legion_name);
			packetOut.WriteByte(0);
			return packetOut.Flush();
		}

		// Token: 0x040005EE RID: 1518
		public uint legion_id;

		// Token: 0x040005EF RID: 1519
		public string legion_name;
	}
}
