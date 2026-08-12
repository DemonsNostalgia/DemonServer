using System;
using GameBase.Core;

namespace GameBase.Network.Internal
{
	// Token: 0x02000010 RID: 16
	public class QueryRole
	{
		// Token: 0x0600004C RID: 76 RVA: 0x0000389A File Offset: 0x00001A9A
		public QueryRole(uint _gameid = 0U, int _key = 0, int _key2 = 0, byte[] _account = null)
		{
			this.mParam = 11;
			this.gameid = _gameid;
			this.key = _key;
			this.key2 = _key2;
			this.account = _account;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000038CC File Offset: 0x00001ACC
		public void Create(byte[] msg)
		{
			PackIn packIn = new PackIn(msg);
			this.mParam = packIn.ReadUInt16();
			this.gameid = packIn.ReadUInt32();
			this.key = packIn.ReadInt32();
			this.key2 = packIn.ReadInt32();
			this.account = packIn.ReadBuff(16);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003920 File Offset: 0x00001B20
		public byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(null);
			packetOut.WriteBuff(InternalPacket.HEAD);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.gameid);
			packetOut.WriteInt32(this.key);
			packetOut.WriteInt32(this.key2);
			packetOut.WriteBuff(this.account);
			packetOut.WriteBuff(InternalPacket.TAIL);
			return packetOut.GetBuffer();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003998 File Offset: 0x00001B98
		public string GetAccount()
		{
			int num = 0;
			for (int i = 0; i < this.account.Length; i++)
			{
				if (this.account[i] == 0)
				{
					num = i;
					break;
				}
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(this.account, 0, array, 0, num);
			return Coding.GetDefauleCoding().GetString(array);
		}

		// Token: 0x04000046 RID: 70
		public ushort mParam;

		// Token: 0x04000047 RID: 71
		public uint gameid;

		// Token: 0x04000048 RID: 72
		public int key;

		// Token: 0x04000049 RID: 73
		public int key2;

		// Token: 0x0400004A RID: 74
		public byte[] account;
	}
}
