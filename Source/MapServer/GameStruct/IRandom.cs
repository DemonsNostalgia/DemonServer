using System;

namespace GameStruct
{
	// Token: 0x0200002D RID: 45
	public class IRandom
	{
		// Token: 0x06000158 RID: 344 RVA: 0x0000FB38 File Offset: 0x0000DD38
		public static int Random(int min, int max)
		{
			int result;
			if (max <= min)
			{
				result = 0;
			}
			else
			{
				result = IRandom.rd.Next(min, max);
			}
			return result;
		}

		// Token: 0x06000159 RID: 345 RVA: 0x0000FB64 File Offset: 0x0000DD64
		public static byte Random(byte min, byte max)
		{
			byte result;
			if (max <= min)
			{
				result = 0;
			}
			else
			{
				result = (byte)IRandom.rd.Next((int)min, (int)max);
			}
			return result;
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000FB90 File Offset: 0x0000DD90
		public static float Random(float min, float max, int len = 1)
		{
			float result;
			if (max <= min)
			{
				result = 0f;
			}
			else
			{
				result = (float)Math.Round(IRandom.rd.NextDouble() * (double)(max - min) + (double)min, len);
			}
			return result;
		}

		// Token: 0x04000208 RID: 520
		private static Random rd = new Random();
	}
}
