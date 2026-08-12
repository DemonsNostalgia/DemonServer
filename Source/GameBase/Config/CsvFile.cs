using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameBase.Core;

namespace GameBase.Config
{
	// Token: 0x02000004 RID: 4
	public class CsvFile
	{
		// Token: 0x06000016 RID: 22 RVA: 0x00002480 File Offset: 0x00000680
		public CsvFile(string text)
		{
			this.mDic = new Dictionary<int, FieldInfo>();
			byte[] bytes = Coding.GetDefauleCoding().GetBytes(text);
			MemoryStream memoryStream = new MemoryStream(bytes);
			this.mField = null;
			StreamReader streamReader = new StreamReader(memoryStream, Encoding.Default);
			int num = 0;
			for (;;)
			{
				string text2 = streamReader.ReadLine();
				if (text2 == null)
				{
					break;
				}
				if (text2.Length > 1)
				{
					if (text2[0] != '/' || text2[1] != '/')
					{
						if (text2[0] == '#')
						{
							this.mField = text2.Split(new char[]
							{
								','
							});
							this.mField[0] = this.mField[0].Substring(1);
						}
						else
						{
							if (this.mField == null)
							{
								goto Block_6;
							}
							FieldInfo value = new FieldInfo(this.mField, text2);
							this.mDic[num] = value;
							num++;
						}
					}
				}
			}
			memoryStream.Dispose();
			return;
			Block_6:
			Log.Instance().WriteLog("load csv error! not field.." + text);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000025D4 File Offset: 0x000007D4
		public int GetCol()
		{
			return this.mField.Length;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000025F0 File Offset: 0x000007F0
		public int GetLine()
		{
			return this.mDic.Count;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002610 File Offset: 0x00000810
		public string GetFieldInfoToRow(int line, int row)
		{
			string result;
			if (this.mDic.ContainsKey(line))
			{
				FieldInfo fieldInfo = this.mDic[line];
				result = fieldInfo.GetFileValueToRow(row);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002650 File Offset: 0x00000850
		public string GetFieldInfoToValue(int line, string row)
		{
			string result;
			if (this.mDic.ContainsKey(line))
			{
				FieldInfo fieldInfo = this.mDic[line];
				result = fieldInfo.GetFieldValueToKey(row);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x04000006 RID: 6
		private Dictionary<int, FieldInfo> mDic;

		// Token: 0x04000007 RID: 7
		private string[] mField;
	}
}
