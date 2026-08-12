using System;

namespace GameBase.Config
{
	// Token: 0x02000003 RID: 3
	internal class FieldInfo
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002368 File Offset: 0x00000568
		public FieldInfo(string[] field, string text)
		{
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array.Length != field.Length)
			{
				Log.Instance().WriteLog("load csv error! not field..code 2" + text);
			}
			this.key = new string[field.Length];
			this.value = new string[field.Length];
			for (int i = 0; i < field.Length; i++)
			{
				this.value[i] = array[i];
				this.key[i] = field[i];
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000023FC File Offset: 0x000005FC
		public string GetFieldValueToKey(string k)
		{
			for (int i = 0; i < this.key.Length; i++)
			{
				if (this.key[i] == k)
				{
					return this.value[i];
				}
			}
			return "";
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000244C File Offset: 0x0000064C
		public string GetFileValueToRow(int row)
		{
			string result;
			if (row >= this.key.Length)
			{
				result = "";
			}
			else
			{
				result = this.value[row];
			}
			return result;
		}

		// Token: 0x04000004 RID: 4
		private string[] key;

		// Token: 0x04000005 RID: 5
		private string[] value;
	}
}
