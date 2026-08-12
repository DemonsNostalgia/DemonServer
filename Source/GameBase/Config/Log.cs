using System;
using System.IO;

namespace GameBase.Config
{
	// Token: 0x0200002F RID: 47
	public class Log
	{
		// Token: 0x060000B6 RID: 182 RVA: 0x00006130 File Offset: 0x00004330
		public static Log Instance()
		{
			if (Log.m_Instance == null)
			{
				Log.m_Instance = new Log();
			}
			return Log.m_Instance;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006164 File Offset: 0x00004364
		public void Init(string sDir, bool debug = true)
		{
			this.m_bDebug = debug;
			if (!Directory.Exists(sDir))
			{
				Directory.CreateDirectory(sDir);
			}
			string text = DateTime.Now.ToString();
			text = text.Replace("/", "-");
			text = text.Replace(":", "-");
			string path = sDir + "/" + text + ".txt";
			this.m_F = new FileStream(path, FileMode.Create);
			this.m_Write = new StreamWriter(this.m_F);
			this.m_bS = true;
			this.WriteLog("init log");
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006208 File Offset: 0x00004408
		public void WriteLog(string sLog)
		{
			try
			{
				lock (this.m_Write)
				{
					if (this.m_bS)
					{
						string value = DateTime.Now.ToString() + "       " + sLog;
						this.m_Write.WriteLine(value);
						this.m_Write.Flush();
						if (this.m_bDebug)
						{
							Console.WriteLine(value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000062D4 File Offset: 0x000044D4
		public void Dispose()
		{
			lock (this.m_Write)
			{
				this.m_F.Close();
				this.m_F.Dispose();
				this.m_Write = null;
				Log.m_Instance = null;
			}
		}

		// Token: 0x0400011D RID: 285
		private static Log m_Instance = null;

		// Token: 0x0400011E RID: 286
		private FileStream m_F = null;

		// Token: 0x0400011F RID: 287
		private StreamWriter m_Write = null;

		// Token: 0x04000120 RID: 288
		private bool m_bDebug = true;

		// Token: 0x04000121 RID: 289
		private bool m_bS = false;
	}
}
