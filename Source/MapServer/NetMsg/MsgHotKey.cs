using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200007B RID: 123
	public class MsgHotKey : BaseMsg
	{
		// Token: 0x06000268 RID: 616 RVA: 0x0001961B File Offset: 0x0001781B
		public MsgHotKey()
		{
			this.mParam = 1015;
			this.mMsgLen = 14;
			this.str = "";
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00019650 File Offset: 0x00017850
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.type = packIn.ReadInt32();
				this.tag = packIn.ReadInt16();
				this.tag2 = packIn.ReadByte();
				if (!packIn.IsComplete())
				{
					this.str = packIn.ReadString();
				}
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x000196C0 File Offset: 0x000178C0
		public override byte[] GetBuffer()
		{
			byte[] bytes = Coding.GetUtf8Coding().GetBytes(this.str);
			if (bytes.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Name-packet strings cannot exceed 255 encoded bytes.");
			}
			ushort wireLength = (ushort)(15 + bytes.Length);
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.type);
			packetOut.WriteInt16(this.tag);
			packetOut.WriteByte(this.tag2);
			packetOut.WriteString(this.str);
			for (int i = 0; i < this.param1.Length; i++)
			{
				packetOut.WriteByte(this.param1[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00019784 File Offset: 0x00017984
		public string[] GetHotKeyArr()
		{
			string[] result;
			if (this.str.Length <= 0)
			{
				result = null;
			}
			else
			{
				result = this.str.Split(new char[]
				{
					'-'
				});
			}
			return result;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x000197C4 File Offset: 0x000179C4
		public byte GetGroup()
		{
			return (byte)this.type;
		}

		// Token: 0x0400056A RID: 1386
		public const int TAG_SAVEHOTKEY = 215;

		// Token: 0x0400056B RID: 1387
		public const int TAG_WANGLING_STATE = 477;

		// Token: 0x0400056C RID: 1388
		public const int WORLD_CHAT = 28;

		// Token: 0x0400056D RID: 1389
		public const int CHANGE_EUDEMON_NAME = 24;

		// Token: 0x0400056E RID: 1390
		public int type;

		// Token: 0x0400056F RID: 1391
		public short tag;

		// Token: 0x04000570 RID: 1392
		public byte tag2;

		// Token: 0x04000571 RID: 1393
		public string str;

		// Token: 0x04000572 RID: 1394
		public byte[] param1 = new byte[3];
	}
}
