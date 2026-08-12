using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000066 RID: 102
	public class MsgQueryCreateRoleName : BaseMsg
	{
		// Token: 0x0600021A RID: 538 RVA: 0x00016AF6 File Offset: 0x00014CF6
		public MsgQueryCreateRoleName()
		{
			this.Name = null;
			this.version = 0;
		}

		// Token: 0x0600021B RID: 539 RVA: 0x00016B10 File Offset: 0x00014D10
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.Name = packIn.ReadBuff(16);
				this.version = packIn.ReadInt32();
			}
		}

		// Token: 0x0600021C RID: 540 RVA: 0x00016B5C File Offset: 0x00014D5C
		public string GetName()
		{
			string result;
			if (this.Name == null)
			{
				result = "";
			}
			else
			{
				int num = 0;
				for (int i = 0; i < this.Name.Length; i++)
				{
					if (this.Name[i] == 0)
					{
						num = i;
						break;
					}
				}
				byte[] array = new byte[num];
				Buffer.BlockCopy(this.Name, 0, array, 0, num);
				string @string = Coding.GetDefauleCoding().GetString(array);
				result = @string;
			}
			return result;
		}

		// Token: 0x0400049F RID: 1183
		public byte[] Name;

		// Token: 0x040004A0 RID: 1184
		public int version;
	}
}
