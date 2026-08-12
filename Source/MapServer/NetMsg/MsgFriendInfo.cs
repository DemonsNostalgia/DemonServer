using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000084 RID: 132
	public class MsgFriendInfo : BaseMsg
	{
		// Token: 0x06000285 RID: 645 RVA: 0x0001A182 File Offset: 0x00018382
		public MsgFriendInfo()
		{
			this.mMsgLen = 52;
			this.mParam = 1019;
			this.name = "";
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0001A1B4 File Offset: 0x000183B4
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.playerid = packIn.ReadUInt32();
				this.fightpower = packIn.ReadUInt32();
				this.type = packIn.ReadByte();
				this.Online = packIn.ReadByte();
				this.level = packIn.ReadByte();
				this.param = packIn.ReadByte();
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0001A22C File Offset: 0x0001842C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.playerid);
			packetOut.WriteUInt32(this.fightpower);
			packetOut.WriteByte(this.type);
			packetOut.WriteByte(this.Online);
			packetOut.WriteByte(this.level);
			packetOut.WriteByte(this.param);
			byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.name);
			if (bytes.Length > 35)
			{
				throw new InvalidOperationException(
					"Friend names cannot exceed 35 encoded bytes.");
			}
			packetOut.WriteBuff(bytes);
			byte[] v = new byte[36 - bytes.Length];
			packetOut.WriteBuff(v);
			return packetOut.Flush();
		}

		// Token: 0x040005BE RID: 1470
		public const byte TYPE_ONLINE = 12;

		// Token: 0x040005BF RID: 1471
		public const byte TYPE_OFFLIE = 13;

		// Token: 0x040005C0 RID: 1472
		public const byte TYPE_KILL = 14;

		// Token: 0x040005C1 RID: 1473
		public const byte TYPE_FRIEND = 15;

		public const byte TYPE_ENEMY_ONLINE = 16;

		public const byte TYPE_ENEMY_OFFLINE = 17;

		public const byte TYPE_ENEMY_KILL = 18;

		public const byte TYPE_ENEMY = 19;

		// Token: 0x040005C2 RID: 1474
		public const byte TYPE_ADDFRIEND = 10;

		// Token: 0x040005C3 RID: 1475
		public const byte TYPE_AGREED = 11;

		// Token: 0x040005C4 RID: 1476
		public const byte TYPE_REFUSE = 21;

		// Token: 0x040005C5 RID: 1477
		public uint playerid;

		// Token: 0x040005C6 RID: 1478
		public uint fightpower;

		// Token: 0x040005C7 RID: 1479
		public byte type;

		// Token: 0x040005C8 RID: 1480
		public byte Online;

		// Token: 0x040005C9 RID: 1481
		public byte level;

		// Token: 0x040005CA RID: 1482
		public byte param = 0;

		// Token: 0x040005CB RID: 1483
		public string name;
	}
}
