using System;
using GameBase.Config;

namespace GameBase.Core
{
	// Token: 0x0200000A RID: 10
	public class GlobalException
	{
		// Token: 0x06000038 RID: 56 RVA: 0x000033BF File Offset: 0x000015BF
		public static void InitException()
		{
			AppDomain.CurrentDomain.UnhandledException += GlobalException.CurrentDomain_UnhandledException;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000033DC File Offset: 0x000015DC
		public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			Log.Instance().WriteLog(ex.Message);
			Log.Instance().WriteLog(ex.StackTrace);
		}
	}
}
