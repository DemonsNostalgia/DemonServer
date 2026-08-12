using System;
using System.Collections.Generic;
using System.IO;

namespace DBServer
{
	// Token: 0x02000002 RID: 2
	public class Filter
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static Filter Instance()
		{
			if (Filter.mInstance == null)
			{
				Filter.mInstance = new Filter();
			}
			return Filter.mInstance;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002082 File Offset: 0x00000282
		public Filter()
		{
			this.list_name = new List<string>();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002098 File Offset: 0x00000298
		public bool LoadFilterNameFile(string sPath)
		{
			bool result;
			if (!File.Exists(sPath))
			{
				result = false;
			}
			else
			{
				FileStream fileStream = new FileStream(sPath, FileMode.Open);
				StreamReader streamReader = new StreamReader(fileStream);
				for (;;)
				{
					string text = streamReader.ReadLine();
					if (text == null)
					{
						break;
					}
					if (!string.IsNullOrWhiteSpace(text))
					{
						this.list_name.Add(text);
					}
				}
				fileStream.Dispose();
				result = true;
			}
			return result;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002100 File Offset: 0x00000300
		public bool CheckFileterName(string name)
		{
			for (int i = 0; i < this.list_name.Count; i++)
			{
				if (name.IndexOf(this.list_name[i]) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000001 RID: 1
		private List<string> list_name;

		// Token: 0x04000002 RID: 2
		private static Filter mInstance = null;
	}
}
