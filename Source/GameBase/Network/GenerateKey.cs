using System;

namespace GameBase.Network
{
	// Token: 0x02000007 RID: 7
	public class GenerateKey
	{
		// Token: 0x0600002B RID: 43 RVA: 0x000031E4 File Offset: 0x000013E4
		public static void Init(int _key = 0, int _key2 = 0)
		{
			GenerateKey.key = _key;
			GenerateKey.key2 = _key2;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000031F3 File Offset: 0x000013F3
		public static void GenerateKey_(ref int _key, ref int _key2)
		{
			GenerateKey.key++;
			GenerateKey.key2++;
			_key = GenerateKey.key;
			_key2 = GenerateKey.key2;
		}

		// Token: 0x04000012 RID: 18
		private static int key = 0;

		// Token: 0x04000013 RID: 19
		private static int key2 = 0;
	}
}
