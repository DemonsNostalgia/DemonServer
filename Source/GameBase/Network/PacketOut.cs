using System;
using System.IO;
using GameBase.Core;

namespace GameBase.Network
{
	// Token: 0x02000032 RID: 50
	public class PacketOut
	{
		// Token: 0x060000DE RID: 222 RVA: 0x00006BF1 File Offset: 0x00004DF1
		public PacketOut(GamePacketKeyEx key = null)
		{
			this.stream = new MemoryStream();
			this.write = new BinaryWriter(this.stream);
			this.m_key = key;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006C20 File Offset: 0x00004E20
		~PacketOut()
		{
			this.stream.Dispose();
			this.stream = null;
			this.write = null;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006C64 File Offset: 0x00004E64
		public void WriteInt32(int v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006C74 File Offset: 0x00004E74
		public void WriteUInt32(uint v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006C84 File Offset: 0x00004E84
		public void WriteInt16(short v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006C94 File Offset: 0x00004E94
		public void WriteUInt16(ushort v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006CA4 File Offset: 0x00004EA4
		public void WriteLong(long v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006CB4 File Offset: 0x00004EB4
		public void WriteULong(ulong v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006CC4 File Offset: 0x00004EC4
		public void WriteBool(bool v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006CD4 File Offset: 0x00004ED4
		public void WriteString(string v)
		{
			byte[] bytes = Coding.GetDefauleCoding().GetBytes(v);
			this.write.Write((byte)bytes.Length);
			this.write.Write(bytes);
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006D0B File Offset: 0x00004F0B
		public void WriteFloat(float v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006D1C File Offset: 0x00004F1C
		public int GetPostion()
		{
			return (int)this.write.BaseStream.Length;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006D3F File Offset: 0x00004F3F
		public void WriteBuff(byte[] v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006D4F File Offset: 0x00004F4F
		public void WriteByte(byte v)
		{
			this.write.Write(v);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00006D60 File Offset: 0x00004F60
		public byte[] GetNormalBuff()
		{
			this.write.Flush();
			byte[] array = new byte[2];
			byte[] buffer = this.stream.GetBuffer();
			array[0] = buffer[0];
			array[1] = buffer[1];
			ushort num = BitConverter.ToUInt16(array, 0);
			byte[] array2 = new byte[(int)num];
			Buffer.BlockCopy(buffer, 0, array2, 0, (int)num);
			return array2;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006DBC File Offset: 0x00004FBC
		public byte[] Flush()
		{
			this.write.Flush();
			byte[] buffer = this.stream.GetBuffer();
			ushort num = BitConverter.ToUInt16(new byte[]
			{
				buffer[0],
				buffer[1]
			}, 0);
			if (this.m_key != null)
			{
				this.m_key.EncodePacket(ref buffer, (int)num);
			}
			byte[] array = new byte[(int)num];
			Buffer.BlockCopy(buffer, 0, array, 0, (int)num);
			return array;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006E38 File Offset: 0x00005038
		public byte[] GetBuffer()
		{
			byte[] buffer = this.stream.GetBuffer();
			byte[] array = new byte[this.write.BaseStream.Length];
			Buffer.BlockCopy(buffer, 0, array, 0, (int)this.write.BaseStream.Length);
			return array;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006E89 File Offset: 0x00005089
		public void Clear(GamePacketKeyEx key = null)
		{
			this.m_key = key;
			this.stream.SetLength(0L);
		}

		// Token: 0x04000125 RID: 293
		private MemoryStream stream;

		// Token: 0x04000126 RID: 294
		private BinaryWriter write;

		// Token: 0x04000127 RID: 295
		private GamePacketKeyEx m_key;
	}
}
