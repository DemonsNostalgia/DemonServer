using System;
using System.Collections.Generic;
using System.IO;
using GameBase.Core;

namespace GameBase.Network
{
	// Token: 0x0200000C RID: 12
	public class InternalPacket
	{
		// Token: 0x06000041 RID: 65 RVA: 0x000034E1 File Offset: 0x000016E1
		public InternalPacket()
		{
			this.m_stream = new MemoryStream();
			this.m_ListData = new List<byte[]>();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003504 File Offset: 0x00001704
		public byte[] GetData()
		{
			byte[] array = null;
			byte[] result;
			if (this.m_ListData.Count > 0)
			{
				array = this.m_ListData[0];
				this.m_ListData.Remove(array);
				result = array;
			}
			else
			{
				result = array;
			}
			return result;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x0000354D File Offset: 0x0000174D
		public void ClearPacket()
		{
			this.m_ListData.Clear();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000355C File Offset: 0x0000175C
		private int FindTag(byte[] data, byte[] tag)
		{
			for (int i = 0; i < data.Length; i++)
			{
				int num = i;
				int j;
				for (j = 0; j < tag.Length; j++)
				{
					if (tag[j] != data[num])
					{
						break;
					}
					num++;
					if (num == data.Length)
					{
						j++;
						break;
					}
				}
				if (j == tag.Length)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000035DC File Offset: 0x000017DC
		public void ProcessNetMsg(byte[] data)
		{
			this.m_stream.Write(data, 0, data.Length);
			byte[] buffer = this.m_stream.GetBuffer();
			byte[] array = new byte[(int)this.m_stream.Length];
			Buffer.BlockCopy(buffer, 0, array, 0, array.Length);
			string @string = Coding.GetDefauleCoding().GetString(array);
			for (;;)
			{
				int num = this.FindTag(array, InternalPacket.HEAD);
				if (num < 0)
				{
					goto IL_11F;
				}
				int num2 = this.FindTag(array, InternalPacket.TAIL);
				if (num2 <= 0)
				{
					goto IL_11A;
				}
				int num3 = num2 - num - InternalPacket.TAIL.Length;
				byte[] array2 = new byte[num3];
				Buffer.BlockCopy(array, num + InternalPacket.HEAD.Length, array2, 0, num3);
				this.m_ListData.Add(array2);
				num3 = array.Length - (num2 + InternalPacket.TAIL.Length);
				if (num3 == 0)
				{
					break;
				}
				array2 = new byte[num3];
				Buffer.BlockCopy(array, num2 + InternalPacket.TAIL.Length, array2, 0, num3);
				array = array2;
			}
			array = null;
			IL_11A:
			IL_11F:
			this.m_stream.SetLength(0L);
			if (array != null && array.Length > 0)
			{
				this.m_stream.Write(array, 0, array.Length);
			}
		}

		// Token: 0x0400001A RID: 26
		private MemoryStream m_stream;

		// Token: 0x0400001B RID: 27
		private List<byte[]> m_ListData;

		// Token: 0x0400001C RID: 28
		public static byte[] HEAD = new byte[]
		{
			35,
			35,
			35
		};

		// Token: 0x0400001D RID: 29
		public static byte[] TAIL = new byte[]
		{
			33,
			33,
			33
		};
	}
}
