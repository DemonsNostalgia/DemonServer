using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200005A RID: 90
	public class MsgNpcReply : BaseMsg
	{
		// Token: 0x060001EF RID: 495 RVA: 0x0001500E File Offset: 0x0001320E
		public MsgNpcReply()
		{
			this.Reset();
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00015020 File Offset: 0x00013220
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.param = packIn.ReadInt32();
				this.param2 = packIn.ReadUInt16();
				this.optionid = packIn.ReadByte();
				this.interactType = packIn.ReadUInt16();
				byte len = packIn.ReadByte();
				this.text = packIn.ReadString((int)len);
				for (int i = 0; i < this.param3.Length; i++)
				{
					this.param3[i] = packIn.ReadByte();
				}
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x000150BA File Offset: 0x000132BA
		public override void Reset()
		{
			this.mMsgLen = 14;
			this.mParam = 2032;
			this.param3 = new byte[3];
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x000150DC File Offset: 0x000132DC
		public override byte[] GetBuffer()
		{
			this.mMsgLen += (ushort)Coding.GetDefauleCoding().GetBytes(this.text).Length;
			this.mMsgLen += (ushort)this.param3.Length;
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.param);
			packetOut.WriteUInt16(this.param2);
			packetOut.WriteByte(this.optionid);
			packetOut.WriteUInt16(this.interactType);
			packetOut.WriteString(this.text);
			for (int i = 0; i < this.param3.Length; i++)
			{
				packetOut.WriteByte(this.param3[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x000151BC File Offset: 0x000133BC
		public byte[] Flush()
		{
			byte[] array = new byte[MsgNpcReply.flushdata.Length];
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteBuff(MsgNpcReply.flushdata);
			return packetOut.Flush();
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00015208 File Offset: 0x00013408
		public byte[] NpcImage(ushort imageid)
		{
			byte[] v = new byte[]
			{
				16,
				0,
				240,
				7,
				0,
				0,
				0,
				0
			};
			byte[] array = new byte[6];
			array[0] = byte.MaxValue;
			array[1] = 4;
			byte[] v2 = array;
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteBuff(v);
			packetOut.WriteUInt16(imageid);
			packetOut.WriteBuff(v2);
			return packetOut.Flush();
		}

		// Token: 0x040003E6 RID: 998
		public int param;

		// Token: 0x040003E7 RID: 999
		public ushort param2;

		// Token: 0x040003E8 RID: 1000
		public byte optionid;

		// Token: 0x040003E9 RID: 1001
		public ushort interactType;

		// Token: 0x040003EA RID: 1002
		public string text;

		// Token: 0x040003EB RID: 1003
		public byte[] param3;

		// Token: 0x040003EC RID: 1004
		private static byte[] flushdata = new byte[]
		{
			16,
			0,
			240,
			7,
			0,
			0,
			0,
			0,
			0,
			0,
			byte.MaxValue,
			100,
			0,
			0,
			0,
			0
		};
	}
}
