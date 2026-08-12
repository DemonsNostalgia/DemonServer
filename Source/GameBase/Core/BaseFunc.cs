using System;

namespace GameBase.Core
{
	// Token: 0x0200000B RID: 11
	public class BaseFunc
	{
		// Token: 0x0600003B RID: 59 RVA: 0x00003420 File Offset: 0x00001620
		public static uint ExchangeShortBits(uint nData, int nBits)
		{
			nData &= 65535U;
			return (nData >> nBits | nData << 16 - nBits) & 65535U;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003454 File Offset: 0x00001654
		public static uint ExchangeLongBits(ulong nData, int nBits)
		{
			ulong num = nData >> nBits | nData << 32 - nBits;
			return (uint)num;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x0000347C File Offset: 0x0000167C
		public static int MakeLong(int lo, int hi)
		{
			return (lo & 65535) | (hi & 65535) << 16;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x000034A0 File Offset: 0x000016A0
		public static short LoWord(int v)
		{
			return (short)(v & 65535);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x000034BC File Offset: 0x000016BC
		public static short HiWord(int v)
		{
			return (short)(v >> 16 & 65535);
		}
	}
}
