using System;
using System.IO;
using GameBase.Config;
using GameBase.Core;

namespace GameBase.Network
{
	// Token: 0x02000034 RID: 52
	public class PackIn
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x00006EA9 File Offset: 0x000050A9
		public PackIn(byte[] data)
		{
			this.stream = new MemoryStream(data);
			this.read = new BinaryReader(this.stream);
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006ED4 File Offset: 0x000050D4
		~PackIn()
		{
			this.stream.Close();
			this.read = null;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006F14 File Offset: 0x00005114
		public int ReadInt32()
		{
			int result;
			if (this.read.BaseStream.Position + 4L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadInt32 error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0;
			}
			else
			{
				result = this.read.ReadInt32();
			}
			return result;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006F94 File Offset: 0x00005194
		public uint ReadUInt32()
		{
			uint result;
			if (this.read.BaseStream.Position + 4L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin readuint32 error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0U;
			}
			else
			{
				result = this.read.ReadUInt32();
			}
			return result;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007014 File Offset: 0x00005214
		public short ReadInt16()
		{
			short result;
			if (this.read.BaseStream.Position + 2L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadInt16 error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0;
			}
			else
			{
				result = this.read.ReadInt16();
			}
			return result;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007094 File Offset: 0x00005294
		public ushort ReadUInt16()
		{
			ushort result;
			if (this.read.BaseStream.Position + 2L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadUInt16 error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0;
			}
			else
			{
				result = this.read.ReadUInt16();
			}
			return result;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00007114 File Offset: 0x00005314
		public long ReadLong()
		{
			long result;
			if (this.read.BaseStream.Position + 8L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadLong error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0L;
			}
			else
			{
				result = this.read.ReadInt64();
			}
			return result;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007194 File Offset: 0x00005394
		public ulong ReadULong()
		{
			ulong result;
			if (this.read.BaseStream.Position + 8L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadULong error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0UL;
			}
			else
			{
				result = this.read.ReadUInt64();
			}
			return result;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00007214 File Offset: 0x00005414
		public bool ReadBool()
		{
			bool result;
			if (this.read.BaseStream.Position + 1L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadBool error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = false;
			}
			else
			{
				result = this.read.ReadBoolean();
			}
			return result;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00007294 File Offset: 0x00005494
		public byte[] ReadBuff(int len)
		{
			byte[] result;
			if (this.read.BaseStream.Position + (long)len > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadBool error!");
				byte[] array = new byte[len];
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = array;
			}
			else
			{
				byte[] array = this.read.ReadBytes(len);
				result = array;
			}
			return result;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00007320 File Offset: 0x00005520
		public float ReadFloat()
		{
			float result;
			if (this.read.BaseStream.Position + 4L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadFloat error!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0f;
			}
			else
			{
				result = this.read.ReadSingle();
			}
			return result;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000073A4 File Offset: 0x000055A4
		public string ReadString()
		{
			string result;
			if (this.read.BaseStream.Position + 1L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadString error1!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = "";
			}
			else
			{
				byte b = this.read.ReadByte();
				if (this.read.BaseStream.Position + (long)((ulong)b) > this.read.BaseStream.Length)
				{
					Log.Instance().WriteLog("packin ReadString error2!");
					this.read.BaseStream.Position = this.read.BaseStream.Length;
					result = "";
				}
				else
				{
					byte[] bytes = this.read.ReadBytes((int)b);
					result = Coding.GetDefauleCoding().GetString(bytes);
				}
			}
			return result;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000074AC File Offset: 0x000056AC
		public string ReadString(int len)
		{
			string result;
			if (this.read.BaseStream.Position + (long)len > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadString error3!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = "";
			}
			else
			{
				byte[] bytes = this.read.ReadBytes(len);
				result = Coding.GetDefauleCoding().GetString(bytes);
			}
			return result;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000753C File Offset: 0x0000573C
		public byte ReadByte()
		{
			byte result;
			if (this.read.BaseStream.Position + 1L > this.read.BaseStream.Length)
			{
				Log.Instance().WriteLog("packin ReadByte error3!");
				this.read.BaseStream.Position = this.read.BaseStream.Length;
				result = 0;
			}
			else
			{
				result = this.read.ReadByte();
			}
			return result;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000075BC File Offset: 0x000057BC
		public bool IsComplete()
		{
			return this.read.BaseStream.Position == this.stream.Length;
		}

		// Token: 0x04000163 RID: 355
		private MemoryStream stream;

		// Token: 0x04000164 RID: 356
		private BinaryReader read;
	}
}
