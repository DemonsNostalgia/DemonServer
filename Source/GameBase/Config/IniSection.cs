using System;
using System.Collections.Generic;
using System.IO;

namespace GameBase.Config
{
	// Token: 0x02000030 RID: 48
	public class IniSection
	{
		// Token: 0x060000BC RID: 188 RVA: 0x0000636D File Offset: 0x0000456D
		public IniSection(string SName)
		{
			this.FSectionName = SName;
			this.FDictionary = new Dictionary<string, string>();
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x060000BD RID: 189 RVA: 0x0000638C File Offset: 0x0000458C
		public string SectionName
		{
			get
			{
				return this.FSectionName;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x060000BE RID: 190 RVA: 0x000063A4 File Offset: 0x000045A4
		public int Count
		{
			get
			{
				return this.FDictionary.Count;
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000063C1 File Offset: 0x000045C1
		public void Clear()
		{
			this.FDictionary.Clear();
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000063D0 File Offset: 0x000045D0
		public void AddKeyValue(string key, string value)
		{
			if (this.FDictionary.ContainsKey(key))
			{
				this.FDictionary[key] = value;
			}
			else
			{
				this.FDictionary.Add(key, value);
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000640F File Offset: 0x0000460F
		public void WriteValue(string key, string value)
		{
			this.AddKeyValue(key, value);
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000641B File Offset: 0x0000461B
		public void WriteValue(string key, bool value)
		{
			this.AddKeyValue(key, Convert.ToString(value));
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000642C File Offset: 0x0000462C
		public void WriteValue(string key, int value)
		{
			this.AddKeyValue(key, Convert.ToString(value));
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000643D File Offset: 0x0000463D
		public void WriteValue(string key, float value)
		{
			this.AddKeyValue(key, Convert.ToString(value));
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x0000644E File Offset: 0x0000464E
		public void WriteValue(string key, DateTime value)
		{
			this.AddKeyValue(key, Convert.ToString(value));
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006460 File Offset: 0x00004660
		public string ReadValue(string key, string defaultv)
		{
			string result;
			if (this.FDictionary.ContainsKey(key))
			{
				result = this.FDictionary[key];
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006498 File Offset: 0x00004698
		public bool ReadValue(string key, bool defaultv)
		{
			string value = this.ReadValue(key, Convert.ToString(defaultv));
			return Convert.ToBoolean(value);
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000064C0 File Offset: 0x000046C0
		public int ReadValue(string key, int defaultv)
		{
			string value = this.ReadValue(key, Convert.ToString(defaultv));
			return Convert.ToInt32(value);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000064E8 File Offset: 0x000046E8
		public float ReadValue(string key, float defaultv)
		{
			string value = this.ReadValue(key, Convert.ToString(defaultv));
			return Convert.ToSingle(value);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006510 File Offset: 0x00004710
		public DateTime ReadValue(string key, DateTime defaultv)
		{
			string value = this.ReadValue(key, Convert.ToString(defaultv));
			return Convert.ToDateTime(value);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00006538 File Offset: 0x00004738
		public void SaveToStream(Stream stream)
		{
			StreamWriter streamWriter = new StreamWriter(stream);
			this.SaveToStream(streamWriter);
			streamWriter.Dispose();
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000655C File Offset: 0x0000475C
		public void SaveToStream(StreamWriter SW)
		{
			SW.WriteLine("[" + this.FSectionName + "]");
			foreach (KeyValuePair<string, string> keyValuePair in this.FDictionary)
			{
				SW.WriteLine(keyValuePair.Key + "=" + keyValuePair.Value);
			}
		}

		// Token: 0x04000122 RID: 290
		private Dictionary<string, string> FDictionary;

		// Token: 0x04000123 RID: 291
		private string FSectionName;
	}
}
