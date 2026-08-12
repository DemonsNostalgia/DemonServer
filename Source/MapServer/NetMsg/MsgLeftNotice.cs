using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000079 RID: 121
	public class MsgLeftNotice : BaseMsg
	{
		// Token: 0x0600025F RID: 607 RVA: 0x00019310 File Offset: 0x00017510
		public MsgLeftNotice()
		{
			this.mParam = 1004;
			this.color = 16777215;
			this.type = 2005;
			this.str = new string[4];
			for (int i = 0; i < this.str.Length; i++)
			{
				this.str[i] = "";
			}
			this.str[0] = "SYSTEM";
			this.mMsgLen = 28;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x000193A1 File Offset: 0x000175A1
		public void SetRoleName(string name)
		{
			this.str[1] = name;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x000193AD File Offset: 0x000175AD
		public void SetText(string text)
		{
			this.str[3] = text;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000193B9 File Offset: 0x000175B9
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x000193C8 File Offset: 0x000175C8
		public override byte[] GetBuffer()
		{
			int wireLength = 28;
			for (int i = 0; i < this.str.Length; i++)
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.str[i]);
				if (bytes.Length > byte.MaxValue)
				{
					throw new InvalidOperationException(
						"Notice strings cannot exceed 255 encoded bytes.");
				}
				wireLength += bytes.Length + 1;
			}
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16((ushort)wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.color);
			packetOut.WriteInt16(this.type);
			packetOut.WriteInt16(this.tag);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteByte(this.amount);
			for (int i = 0; i < this.str.Length; i++)
			{
				packetOut.WriteString(this.str[i]);
			}
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			return packetOut.Flush();
		}

		// Token: 0x0400055E RID: 1374
		public int color;

		// Token: 0x0400055F RID: 1375
		public short type;

		// Token: 0x04000560 RID: 1376
		public short tag;

		// Token: 0x04000561 RID: 1377
		public int param;

		// Token: 0x04000562 RID: 1378
		public int param1 = -1;

		// Token: 0x04000563 RID: 1379
		public int param2 = 0;

		// Token: 0x04000564 RID: 1380
		public byte amount = 4;

		// Token: 0x04000565 RID: 1381
		public string[] str;
	}
}
