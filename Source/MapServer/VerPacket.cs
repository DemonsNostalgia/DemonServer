using System;
using System.Collections.Generic;
using System.IO;
using GameBase.Config;
using GameBase.Core;

namespace MapServer
{
	// Token: 0x0200000D RID: 13
	public class VerPacket
	{
		// Token: 0x060000A9 RID: 169 RVA: 0x000084C0 File Offset: 0x000066C0
		public VerPacket(string verpacketpath = "")
		{
			this.mListName = new List<string>();
			this.mListData = new List<byte[]>();
			this.isVer = false;
			this.m_sPath = verpacketpath;
			if (this.m_sPath.Length > 0)
			{
				this.isVer = true;
				if (!this.DownVerPack(this.m_sPath))
				{
					Log.Instance().WriteLog("Failed to read the version packet. URL: " + verpacketpath);
				}
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00008540 File Offset: 0x00006740
		public void InitPacket(byte[] data)
		{
			MemoryStream input = new MemoryStream(data);
			BinaryReader binaryReader = new BinaryReader(input);
			int num = binaryReader.ReadInt32();
			if (num != 3389)
			{
				Log.Instance().WriteLog("Version mismatch.");
			}
			else
			{
				int num2 = binaryReader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					byte[] array = binaryReader.ReadBytes(128);
					byte[] array2 = null;
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] == 0)
						{
							array2 = new byte[j];
							Buffer.BlockCopy(array, 0, array2, 0, j);
							break;
						}
					}
					string @string = Coding.GetDefauleCoding().GetString(array2);
					int count = binaryReader.ReadInt32();
					byte[] item = binaryReader.ReadBytes(count);
					this.mListName.Add(@string);
					this.mListData.Add(item);
				}
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00008640 File Offset: 0x00006840
		public byte[] LoadFileToBytes(string file)
		{
			if (this.isVer)
			{
				for (int i = 0; i < this.mListName.Count; i++)
				{
					if (this.mListName[i] == file)
					{
						return this.mListData[i];
					}
				}
			}
			else
			{
				FileStream fileStream = new FileStream(file, FileMode.Open);
				if (fileStream.Length > 0L)
				{
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					fileStream.Close();
					return array;
				}
			}
			return null;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000086F8 File Offset: 0x000068F8
		public string LoadFileToText(string file)
		{
			if (this.isVer)
			{
				for (int i = 0; i < this.mListName.Count; i++)
				{
					if (this.mListName[i] == file)
					{
						return Coding.GetDefauleCoding().GetString(this.mListData[i]);
					}
				}
			}
			else
			{
				if (!File.Exists(file))
				{
					Log.Instance().WriteLog("Failed to load file: " + file);
					return "";
				}
				FileStream fileStream = new FileStream(file, FileMode.Open);
				if (fileStream.Length > 0L)
				{
					byte[] array = new byte[fileStream.Length];
					fileStream.Read(array, 0, (int)fileStream.Length);
					fileStream.Close();
					return Coding.GetDefauleCoding().GetString(array);
				}
				fileStream.Dispose();
			}
			return "";
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00008800 File Offset: 0x00006A00
		public bool DownVerPack(string url)
		{
			return true;
		}

		// Token: 0x0400005D RID: 93
		private const int VERSION = 3389;

		// Token: 0x0400005E RID: 94
		public int mnVer;

		// Token: 0x0400005F RID: 95
		public List<string> mListName;

		// Token: 0x04000060 RID: 96
		public List<byte[]> mListData;

		// Token: 0x04000061 RID: 97
		public string m_sPath;

		// Token: 0x04000062 RID: 98
		public bool isVer;
	}
}
