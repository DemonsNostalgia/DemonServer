using System;

namespace GameBase.Core
{
	// Token: 0x02000047 RID: 71
	public class TimeOut
	{
		// Token: 0x06000140 RID: 320 RVA: 0x0000803B File Offset: 0x0000623B
		public TimeOut()
		{
			this.mnTick = 0;
			this.mnInterval = 0;
		}

		// Token: 0x06000141 RID: 321 RVA: 0x00008054 File Offset: 0x00006254
		public void SetInterval(int nSec)
		{
			this.mnInterval = nSec * 1000;
			this.Update();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000806C File Offset: 0x0000626C
		public bool IsToNextTime()
		{
			return Environment.TickCount - this.mnTick > this.mnInterval;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000809F File Offset: 0x0000629F
		public void SetInterval(float fMS)
		{
			this.mnInterval = (int)fMS;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000080AA File Offset: 0x000062AA
		public void SetObject(object obj)
		{
			this.mObject = obj;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000080B4 File Offset: 0x000062B4
		public object GetObject()
		{
			return this.mObject;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x000080CC File Offset: 0x000062CC
		public void Update()
		{
			this.mnTick = Environment.TickCount;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x000080DC File Offset: 0x000062DC
		public bool ToNextTime()
		{
			bool result;
			if (Environment.TickCount - this.mnTick > this.mnInterval)
			{
				this.Update();
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00008118 File Offset: 0x00006318
		public int GetDelayMS()
		{
			return Environment.TickCount - this.mnTick;
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00008138 File Offset: 0x00006338
		public int GetTimeOutMS()
		{
			return this.mnInterval - (Environment.TickCount - this.mnTick);
		}

		// Token: 0x040002A1 RID: 673
		private int mnTick;

		// Token: 0x040002A2 RID: 674
		private int mnInterval;

		// Token: 0x040002A3 RID: 675
		private object mObject;
	}
}
