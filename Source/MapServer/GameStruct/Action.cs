using System;
using System.Collections.Generic;

namespace GameStruct
{
	// Token: 0x0200002C RID: 44
	public class Action
	{
		// Token: 0x06000152 RID: 338 RVA: 0x0000FA04 File Offset: 0x0000DC04
		public Action(byte _action, byte[] _data = null)
		{
			this.action = _action;
			this.data = null;
			if (_data != null)
			{
				this.data = new byte[_data.Length];
				Buffer.BlockCopy(_data, 0, this.data, 0, _data.Length);
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x0000FA54 File Offset: 0x0000DC54
		public byte GetAction()
		{
			return this.action;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000FA6C File Offset: 0x0000DC6C
		public byte[] GetBuff()
		{
			return this.data;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x0000FA84 File Offset: 0x0000DC84
		public int GetObjectCount()
		{
			int result;
			if (this.param == null)
			{
				result = 0;
			}
			else
			{
				result = this.param.Count;
			}
			return result;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000FAB8 File Offset: 0x0000DCB8
		public object GetObject(int index)
		{
			object result;
			if (this.param == null)
			{
				result = null;
			}
			else if (index >= this.param.Count)
			{
				result = null;
			}
			else
			{
				result = this.param[index];
			}
			return result;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000FB00 File Offset: 0x0000DD00
		public void AddObject(object obj)
		{
			if (this.param == null)
			{
				this.param = new List<object>();
			}
			this.param.Add(obj);
		}

		// Token: 0x040001FE RID: 510
		public const byte NORMAL = 0;

		// Token: 0x040001FF RID: 511
		public const byte IDLE = 1;

		// Token: 0x04000200 RID: 512
		public const byte MOVE = 2;

		// Token: 0x04000201 RID: 513
		public const byte ATTACK = 3;

		// Token: 0x04000202 RID: 514
		public const byte DIE = 4;

		// Token: 0x04000203 RID: 515
		public const byte ALIVE = 5;

		// Token: 0x04000204 RID: 516
		public const byte INJURED = 6;

		// Token: 0x04000205 RID: 517
		private byte action;

		// Token: 0x04000206 RID: 518
		private byte[] data;

		// Token: 0x04000207 RID: 519
		private List<object> param;
	}
}
