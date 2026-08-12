using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000057 RID: 87
	public class MsgMonsterInfo : BaseMsg
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x00014B74 File Offset: 0x00012D74
		public MsgMonsterInfo()
		{
			this.mMsgLen = 72;
			this.mParam = 2069;
			this.param2 = new int[7];
			for (int i = 0; i < this.param2.Length; i++)
			{
				this.param2[i] = 0;
			}
			this.lookface = 0U;
			this.id = (this.typeid = 0U);
			this.param = (this.param1 = (this.dir = 0));
			this.maxhp = (this.hp = 0);
			this.level = (this.hp_ = 0);
			this.x = (this.y = 0);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00014C30 File Offset: 0x00012E30
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null && msg.Length == (int)(this.mMsgLen - 2))
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.id = packIn.ReadUInt32();
				this.param = packIn.ReadInt32();
				this.param = packIn.ReadInt32();
				for (int i = 0; i < this.param2.Length; i++)
				{
					this.param2[i] = packIn.ReadInt32();
				}
				this.lookface = packIn.ReadUInt32();
				this.x = packIn.ReadInt16();
				this.y = packIn.ReadInt16();
				this.hp_ = packIn.ReadUInt16();
				this.level = packIn.ReadUInt16();
				this.typeid = packIn.ReadUInt32();
				this.maxhp = packIn.ReadInt32();
				this.hp = packIn.ReadInt32();
				this.dir = packIn.ReadInt32();
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00014D2C File Offset: 0x00012F2C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.id);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			for (int i = 0; i < this.param2.Length; i++)
			{
				packetOut.WriteInt32(this.param2[i]);
			}
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteUInt16(this.hp_);
			packetOut.WriteUInt16(this.level);
			packetOut.WriteUInt32(this.typeid);
			packetOut.WriteInt32(this.maxhp);
			packetOut.WriteInt32(this.hp);
			packetOut.WriteInt32(this.dir);
			return packetOut.Flush();
		}

		// Token: 0x040003D2 RID: 978
		public uint id;

		// Token: 0x040003D3 RID: 979
		public int param;

		// Token: 0x040003D4 RID: 980
		public int param1;

		// Token: 0x040003D5 RID: 981
		public int[] param2;

		// Token: 0x040003D6 RID: 982
		public uint lookface;

		// Token: 0x040003D7 RID: 983
		public short x;

		// Token: 0x040003D8 RID: 984
		public short y;

		// Token: 0x040003D9 RID: 985
		public ushort hp_;

		// Token: 0x040003DA RID: 986
		public ushort level;

		// Token: 0x040003DB RID: 987
		public uint typeid;

		// Token: 0x040003DC RID: 988
		public int maxhp;

		// Token: 0x040003DD RID: 989
		public int hp;

		// Token: 0x040003DE RID: 990
		public int dir;
	}
}
