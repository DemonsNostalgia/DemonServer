using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000067 RID: 103
	public class MsgCreateRoleInfo : BaseMsg
	{
		// Token: 0x0600021D RID: 541 RVA: 0x00016BEC File Offset: 0x00014DEC
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.tag = packIn.ReadString(16);
				this.name = packIn.ReadString(16);
				this.tag1 = packIn.ReadString(16);
				this.hardwaretag = packIn.ReadString(44);
				this.version = packIn.ReadInt32();
				this.lookface = packIn.ReadUInt32();
				this.profession = packIn.ReadUInt16();
				this.param = packIn.ReadUInt16();
				this.param1 = packIn.ReadInt32();
				this.param2 = packIn.ReadInt32();
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x00016CA0 File Offset: 0x00014EA0
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			return null;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x00016CBC File Offset: 0x00014EBC
		public string GetName()
		{
			string result;
			if (this.name == null)
			{
				result = "";
			}
			else
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.name);
				int num = 0;
				for (int i = 0; i < bytes.Length; i++)
				{
					if (bytes[i] == 0)
					{
						num = i;
						break;
					}
				}
				byte[] array = new byte[num];
				Buffer.BlockCopy(bytes, 0, array, 0, num);
				string @string = Coding.GetDefauleCoding().GetString(array);
				result = @string;
			}
			return result;
		}

		// Token: 0x040004A1 RID: 1185
		public string tag;

		// Token: 0x040004A2 RID: 1186
		public string name;

		// Token: 0x040004A3 RID: 1187
		public string tag1;

		// Token: 0x040004A4 RID: 1188
		public string hardwaretag;

		// Token: 0x040004A5 RID: 1189
		public int version;

		// Token: 0x040004A6 RID: 1190
		public uint lookface;

		// Token: 0x040004A7 RID: 1191
		public ushort profession;

		// Token: 0x040004A8 RID: 1192
		public ushort param;

		// Token: 0x040004A9 RID: 1193
		public int param1;

		// Token: 0x040004AA RID: 1194
		public int param2;
	}
}
