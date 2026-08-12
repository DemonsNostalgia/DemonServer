using System;
using GameBase.Config;

namespace GameStruct
{
	// Token: 0x02000034 RID: 52
	public class HotkeyInfo
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00010130 File Offset: 0x0000E330
		public HotkeyInfo(byte _group, string text)
		{
			try
			{
				this.group = _group;
				string[] array = text.Split(new char[]
				{
					'|'
				});
				if (array.Length == 6)
				{
					this.index = Convert.ToByte(array[0]);
					this.count = Convert.ToByte(array[1]);
					this.id = Convert.ToInt32(array[2]);
					this.type = Convert.ToInt32(array[3]);
					this.baseid = Convert.ToInt32(array[4]);
					this.amount = Convert.ToByte(array[5]);
				}
			}
			catch (Exception ex)
			{
				Log.Instance().WriteLog(ex.Message);
				Log.Instance().WriteLog(ex.StackTrace);
				Log.Instance().WriteLog("Invalid hotkey structure encountered while saving.");
			}
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00010210 File Offset: 0x0000E410
		public byte GetGroup()
		{
			return this.group;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00010228 File Offset: 0x0000E428
		public string GetString(bool isGroup = false)
		{
			string text = "";
			if (isGroup)
			{
				text = this.group.ToString() + "|";
			}
			string text2 = text;
			return string.Concat(new string[]
			{
				text2,
				this.index.ToString(),
				"|",
				this.count.ToString(),
				"|",
				this.baseid.ToString(),
				"|",
				this.type.ToString(),
				"|",
				this.id.ToString(),
				"|",
				this.amount.ToString()
			});
		}

		// Token: 0x0400027A RID: 634
		public const byte GROUP_F1 = 1;

		// Token: 0x0400027B RID: 635
		public const byte GROUP_KEY1 = 2;

		// Token: 0x0400027C RID: 636
		public const byte TYPE_ITEM = 0;

		// Token: 0x0400027D RID: 637
		public const byte TYPE_MAGIC = 2;

		// Token: 0x0400027E RID: 638
		public byte group;

		// Token: 0x0400027F RID: 639
		public byte index;

		// Token: 0x04000280 RID: 640
		public byte count;

		// Token: 0x04000281 RID: 641
		public int id;

		// Token: 0x04000282 RID: 642
		public int type;

		// Token: 0x04000283 RID: 643
		public int baseid;

		// Token: 0x04000284 RID: 644
		public byte amount;
	}
}
