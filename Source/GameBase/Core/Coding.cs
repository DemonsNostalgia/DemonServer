using System;
using System.Text;
using GameBase.Config;

namespace GameBase.Core
{
	// Token: 0x02000002 RID: 2
	public class Coding
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static Encoding GetDefauleCoding()
		{
			if (Coding.gb2312 == null)
			{
				Coding.gb2312 = Encoding.GetEncoding("gb2312");
				if (Coding.gb2312 == null)
				{
					Log.Instance().WriteLog("Failed to get the system default encoding; using GB2312.");
				}
			}
			return Coding.gb2312;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A8 File Offset: 0x000002A8
		public static Encoding GetLatin1()
		{
			if (Coding.latin == null)
			{
				Coding.latin = Encoding.GetEncoding("latin1");
				if (Coding.latin == null)
				{
					Log.Instance().WriteLog("Failed to get the Latin-1 encoding.");
				}
			}
			return Coding.latin;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002100 File Offset: 0x00000300
		public static Encoding GetUtf8Coding()
		{
			if (Coding.utf8 == null)
			{
				Coding.utf8 = Encoding.GetEncoding(65001);
				if (Coding.utf8 == null)
				{
					Log.Instance().WriteLog("Failed to get the UTF-8 encoding.");
				}
			}
			return Coding.utf8;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002158 File Offset: 0x00000358
		public static string Utf8ToGB2312(byte[] text)
		{
			Coding.Init();
			return Coding.gb2312.GetString(text);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x0000217C File Offset: 0x0000037C
		public static string Utf8ToGB2312(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.utf8.GetBytes(text);
			return Coding.Utf8ToGB2312(bytes);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021A8 File Offset: 0x000003A8
		public static string GB2312ToUtf8(byte[] text)
		{
			Coding.Init();
			return Coding.utf8.GetString(text);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000021CC File Offset: 0x000003CC
		public static string GB2312ToUtf8(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.gb2312.GetBytes(text);
			return Coding.GB2312ToUtf8(bytes);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000021F8 File Offset: 0x000003F8
		public static string GB2312ToLatin1(byte[] text)
		{
			Coding.Init();
			return Coding.latin.GetString(text);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x0000221C File Offset: 0x0000041C
		public static string GB2312ToLatin1(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.gb2312.GetBytes(text);
			return Coding.GB2312ToLatin1(bytes);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002248 File Offset: 0x00000448
		public static string Latin1ToGB2312(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.latin.GetBytes(text);
			return Coding.Latin1ToGB2312(bytes);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002274 File Offset: 0x00000474
		public static string Latin1ToGB2312(byte[] text)
		{
			Coding.Init();
			return Coding.gb2312.GetString(text);
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002298 File Offset: 0x00000498
		public static string Latin1ToUft8(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.latin.GetBytes(text);
			return Coding.Latin1ToUft8(bytes);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000022C4 File Offset: 0x000004C4
		public static string Latin1ToUft8(byte[] text)
		{
			Coding.Init();
			return Coding.utf8.GetString(text);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000022E8 File Offset: 0x000004E8
		public static string Uft8ToLatin1(string text)
		{
			Coding.Init();
			byte[] bytes = Coding.utf8.GetBytes(text);
			return Coding.Uft8ToLatin1(bytes);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002314 File Offset: 0x00000514
		public static string Uft8ToLatin1(byte[] text)
		{
			Coding.Init();
			return Coding.latin.GetString(text);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002337 File Offset: 0x00000537
		private static void Init()
		{
			Coding.GetDefauleCoding();
			Coding.GetUtf8Coding();
			Coding.GetLatin1();
		}

		// Token: 0x04000001 RID: 1
		private static Encoding gb2312 = null;

		// Token: 0x04000002 RID: 2
		private static Encoding utf8 = null;

		// Token: 0x04000003 RID: 3
		private static Encoding latin = null;
	}
}
