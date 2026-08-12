using System;
using System.Collections;
using System.IO;
using System.Text;

namespace GameBase.Config
{
	// Token: 0x02000031 RID: 49
	public class MemIniFile
	{
		// Token: 0x060000CD RID: 205 RVA: 0x000065EC File Offset: 0x000047EC
		private bool SectionExists(string SectionName)
		{
			foreach (object obj in this.List)
			{
				IniSection iniSection = (IniSection)obj;
				if (iniSection.SectionName.ToLower() == SectionName.ToLower())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00006678 File Offset: 0x00004878
		public IniSection FindSection(string SectionName)
		{
			foreach (object obj in this.List)
			{
				IniSection iniSection = (IniSection)obj;
				if (iniSection.SectionName.ToLower() == SectionName.ToLower())
				{
					return iniSection;
				}
			}
			return null;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006704 File Offset: 0x00004904
		public MemIniFile()
		{
			this.List = new ArrayList();
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000671C File Offset: 0x0000491C
		public void LoadFromStream(Stream stream)
		{
			StreamReader streamReader = new StreamReader(stream, Encoding.Default);
			this.List.Clear();
			IniSection iniSection = null;
			for (;;)
			{
				string text = streamReader.ReadLine();
				if (text == null)
				{
					break;
				}
				text = text.Trim();
				if (!(text == ""))
				{
					if (text != "" && text[0] == '[' && text[text.Length - 1] == ']')
					{
						text = text.Remove(0, 1);
						text = text.Remove(text.Length - 1, 1);
						iniSection = this.FindSection(text);
						if (iniSection == null)
						{
							iniSection = new IniSection(text);
							this.List.Add(iniSection);
						}
					}
					else
					{
						if (iniSection == null)
						{
							iniSection = this.FindSection("UnDefSection");
							if (iniSection == null)
							{
								iniSection = new IniSection("UnDefSection");
								this.List.Add(iniSection);
							}
						}
						int num = text.IndexOf('=');
						if (num != 0)
						{
							string key = text.Substring(0, num);
							string value = text.Substring(num + 1, text.Length - num - 1);
							iniSection.AddKeyValue(key, value);
						}
						else
						{
							iniSection.AddKeyValue(text, "");
						}
					}
				}
			}
			streamReader.Dispose();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000068AC File Offset: 0x00004AAC
		public void SaveToStream(Stream stream)
		{
			StreamWriter streamWriter = new StreamWriter(stream);
			foreach (object obj in this.List)
			{
				IniSection iniSection = (IniSection)obj;
				iniSection.SaveToStream(streamWriter);
			}
			streamWriter.Dispose();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00006924 File Offset: 0x00004B24
		public string ReadValue(string SectionName, string key, string defaultv)
		{
			IniSection iniSection = this.FindSection(SectionName);
			string result;
			if (iniSection != null)
			{
				result = iniSection.ReadValue(key, defaultv);
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00006954 File Offset: 0x00004B54
		public bool ReadValue(string SectionName, string key, bool defaultv)
		{
			IniSection iniSection = this.FindSection(SectionName);
			bool result;
			if (iniSection != null)
			{
				result = iniSection.ReadValue(key, defaultv);
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00006984 File Offset: 0x00004B84
		public int ReadValue(string SectionName, string key, int defaultv)
		{
			IniSection iniSection = this.FindSection(SectionName);
			int result;
			if (iniSection != null)
			{
				result = iniSection.ReadValue(key, defaultv);
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000069B4 File Offset: 0x00004BB4
		public float ReadValue(string SectionName, string key, float defaultv)
		{
			IniSection iniSection = this.FindSection(SectionName);
			float result;
			if (iniSection != null)
			{
				result = iniSection.ReadValue(key, defaultv);
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000069E4 File Offset: 0x00004BE4
		public DateTime ReadValue(string SectionName, string key, DateTime defaultv)
		{
			IniSection iniSection = this.FindSection(SectionName);
			DateTime result;
			if (iniSection != null)
			{
				result = iniSection.ReadValue(key, defaultv);
			}
			else
			{
				result = defaultv;
			}
			return result;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00006A14 File Offset: 0x00004C14
		public IniSection WriteValue(string SectionName, string key, string value)
		{
			IniSection iniSection = this.FindSection(SectionName);
			if (iniSection == null)
			{
				iniSection = new IniSection(SectionName);
				this.List.Add(iniSection);
			}
			iniSection.WriteValue(key, value);
			return iniSection;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00006A5C File Offset: 0x00004C5C
		public IniSection WriteValue(string SectionName, string key, bool value)
		{
			IniSection iniSection = this.FindSection(SectionName);
			if (iniSection == null)
			{
				iniSection = new IniSection(SectionName);
				this.List.Add(iniSection);
			}
			iniSection.WriteValue(key, value);
			return iniSection;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00006AA4 File Offset: 0x00004CA4
		public IniSection WriteValue(string SectionName, string key, int value)
		{
			IniSection iniSection = this.FindSection(SectionName);
			if (iniSection == null)
			{
				iniSection = new IniSection(SectionName);
				this.List.Add(iniSection);
			}
			iniSection.WriteValue(key, value);
			return iniSection;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006AEC File Offset: 0x00004CEC
		public IniSection WriteValue(string SectionName, string key, float value)
		{
			IniSection iniSection = this.FindSection(SectionName);
			if (iniSection == null)
			{
				iniSection = new IniSection(SectionName);
				this.List.Add(iniSection);
			}
			iniSection.WriteValue(key, value);
			return iniSection;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006B34 File Offset: 0x00004D34
		public IniSection WriteValue(string SectionName, string key, DateTime value)
		{
			IniSection iniSection = this.FindSection(SectionName);
			if (iniSection == null)
			{
				iniSection = new IniSection(SectionName);
				this.List.Add(iniSection);
			}
			iniSection.WriteValue(key, value);
			return iniSection;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006B7C File Offset: 0x00004D7C
		public bool LoadFromFile(string FileName)
		{
			bool result;
			if (!File.Exists(FileName))
			{
				result = false;
			}
			else
			{
				FileStream fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				this.LoadFromStream(fileStream);
				fileStream.Close();
				fileStream.Dispose();
				result = true;
			}
			return result;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006BC0 File Offset: 0x00004DC0
		public void SaveToFile(string FileName)
		{
			FileStream fileStream = new FileStream(Path.GetFullPath(FileName), FileMode.Create);
			this.SaveToStream(fileStream);
			fileStream.Close();
			fileStream.Dispose();
		}

		// Token: 0x04000124 RID: 292
		private ArrayList List;
	}
}
