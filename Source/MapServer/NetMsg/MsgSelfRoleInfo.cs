using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000055 RID: 85
	public class MsgSelfRoleInfo : BaseMsg
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x000142DC File Offset: 0x000124DC
		public MsgSelfRoleInfo()
		{
			this.mMsgLen = 247;
			this.mParam = 1006;
			this.name = "";
			this.param13 = new byte[3];
			for (int i = 0; i < this.param13.Length; i++)
			{
				this.param13[i] = 0;
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00014398 File Offset: 0x00012598
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadInt16();
				this.roleid = packIn.ReadUInt32();
				this.lookface = packIn.ReadUInt32();
				this.hair = packIn.ReadUInt32();
				this.gold = packIn.ReadUInt32();
				this.gamegold = packIn.ReadUInt32();
				this.exp = packIn.ReadUInt32();
				this.expparam = packIn.ReadUInt32();
				this.mentorexp = packIn.ReadUInt32();
				this.mercenarexp = packIn.ReadUInt32();
				this.potential = packIn.ReadUInt32();
				this.attackpower = packIn.ReadUInt16();
				this.constitution = packIn.ReadUInt16();
				this.doage = packIn.ReadUInt16();
				this.decdoage = packIn.ReadUInt16();
				this.health = packIn.ReadUInt16();
				this.magic_attack = packIn.ReadUInt16();
				this.addpoint = packIn.ReadUInt16();
				this.life = packIn.ReadUInt16();
				this.maxlife = packIn.ReadUInt16();
				this.manna = packIn.ReadUInt16();
				this.param = packIn.ReadUInt32();
				this.param1 = packIn.ReadUInt32();
				this.pk = packIn.ReadUInt16();
				this.level = packIn.ReadByte();
				this.profession = packIn.ReadByte();
				this.param2 = packIn.ReadByte();
				this.param3 = packIn.ReadByte();
				this.param4 = packIn.ReadByte();
				this.mentorlevel = packIn.ReadByte();
				this.param14 = packIn.ReadByte();
				this.guanjue = packIn.ReadByte();
				this.maxpetcall = packIn.ReadUInt16();
				this.exploit = packIn.ReadInt32();
				this.bonuspoint = packIn.ReadInt32();
				this.edubroodpacksize = packIn.ReadByte();
				this.winglevel = packIn.ReadByte();
				this.godpetpackagelimit = packIn.ReadByte();
				this.demonlev = packIn.ReadByte();
				this.demonexp = packIn.ReadInt32();
				this.demonexpparam = packIn.ReadInt32();
				this.param5 = packIn.ReadInt32();
				this.godlevel = packIn.ReadInt32();
				this.param9 = packIn.ReadByte();
				this.param11 = packIn.ReadByte();
				this.param10 = packIn.ReadUInt16();
				for (int i = 0; i < this.param6.Length; i++)
				{
					this.param6[i] = packIn.ReadInt32();
				}
				this.originalserverid = packIn.ReadInt32();
				this.wordtreeareaid = packIn.ReadUInt16();
				for (int i = 0; i < this.param7.Length; i++)
				{
					this.param7[i] = packIn.ReadInt32();
				}
				this.param8 = packIn.ReadUInt16();
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00014658 File Offset: 0x00012858
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			byte[] nameBytes = Coding.GetDefauleCoding().GetBytes(this.name);
			if (nameBytes.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Role names cannot exceed 255 encoded bytes.");
			}
			ushort wireLength = (ushort)(247 + nameBytes.Length);
			packetOut.WriteUInt16(wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.roleid);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteUInt32(this.hair);
			packetOut.WriteUInt32(this.gold);
			packetOut.WriteUInt32(this.gamegold);
			packetOut.WriteUInt32(this.exp);
			packetOut.WriteUInt32(this.expparam);
			packetOut.WriteUInt32(this.mentorexp);
			packetOut.WriteUInt32(this.mercenarexp);
			packetOut.WriteUInt32(this.potential);
			packetOut.WriteUInt16(this.attackpower);
			packetOut.WriteUInt16(this.constitution);
			packetOut.WriteUInt16(this.doage);
			packetOut.WriteUInt16(this.decdoage);
			packetOut.WriteUInt16(this.health);
			packetOut.WriteUInt16(this.magic_attack);
			packetOut.WriteUInt16(this.addpoint);
			packetOut.WriteUInt16(this.life);
			packetOut.WriteUInt16(this.maxlife);
			packetOut.WriteUInt16(this.manna);
			packetOut.WriteUInt32(this.param);
			packetOut.WriteUInt32(this.param1);
			packetOut.WriteUInt16(this.pk);
			packetOut.WriteByte(this.level);
			packetOut.WriteByte(this.profession);
			packetOut.WriteByte(this.param2);
			packetOut.WriteByte(this.param3);
			packetOut.WriteByte(this.param4);
			packetOut.WriteByte(this.mentorlevel);
			packetOut.WriteByte(this.param14);
			packetOut.WriteByte(this.guanjue);
			packetOut.WriteUInt16(this.maxpetcall);
			packetOut.WriteInt32(this.exploit);
			packetOut.WriteInt32(this.bonuspoint);
			packetOut.WriteByte(this.edubroodpacksize);
			packetOut.WriteByte(this.winglevel);
			packetOut.WriteByte(this.godpetpackagelimit);
			packetOut.WriteByte(this.demonlev);
			packetOut.WriteInt32(this.demonexp);
			packetOut.WriteInt32(this.demonexpparam);
			packetOut.WriteInt32(this.param5);
			packetOut.WriteInt32(this.godlevel);
			packetOut.WriteByte(this.param9);
			packetOut.WriteByte(this.param11);
			packetOut.WriteUInt16(this.param10);
			for (int i = 0; i < this.param6.Length; i++)
			{
				packetOut.WriteInt32(this.param6[i]);
			}
			packetOut.WriteInt32(this.originalserverid);
			packetOut.WriteUInt16(this.wordtreeareaid);
			for (int i = 0; i < this.param7.Length; i++)
			{
				packetOut.WriteInt32(this.param7[i]);
			}
			packetOut.WriteUInt16(this.param8);
			packetOut.WriteByte(2);
			packetOut.WriteString(this.name);
			for (int i = 0; i < this.param13.Length; i++)
			{
				packetOut.WriteByte(this.param13[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x04000396 RID: 918
		public string name;

		// Token: 0x04000397 RID: 919
		public uint roleid;

		// Token: 0x04000398 RID: 920
		public uint lookface;

		// Token: 0x04000399 RID: 921
		public uint hair;

		// Token: 0x0400039A RID: 922
		public uint gold;

		// Token: 0x0400039B RID: 923
		public uint gamegold;

		// Token: 0x0400039C RID: 924
		public uint exp;

		// Token: 0x0400039D RID: 925
		public uint expparam;

		// Token: 0x0400039E RID: 926
		public uint mentorexp;

		// Token: 0x0400039F RID: 927
		public uint mercenarexp;

		// Token: 0x040003A0 RID: 928
		public uint potential;

		// Token: 0x040003A1 RID: 929
		public ushort attackpower;

		// Token: 0x040003A2 RID: 930
		public ushort constitution;

		// Token: 0x040003A3 RID: 931
		public ushort doage;

		// Token: 0x040003A4 RID: 932
		public ushort decdoage;

		// Token: 0x040003A5 RID: 933
		public ushort health;

		// Token: 0x040003A6 RID: 934
		public ushort magic_attack;

		// Token: 0x040003A7 RID: 935
		public ushort addpoint;

		// Token: 0x040003A8 RID: 936
		public ushort life;

		// Token: 0x040003A9 RID: 937
		public ushort maxlife;

		// Token: 0x040003AA RID: 938
		public ushort manna;

		// Token: 0x040003AB RID: 939
		public uint param;

		// Token: 0x040003AC RID: 940
		public uint param1;

		// Token: 0x040003AD RID: 941
		public ushort pk;

		// Token: 0x040003AE RID: 942
		public byte level;

		// Token: 0x040003AF RID: 943
		public byte profession;

		// Token: 0x040003B0 RID: 944
		public byte param2;

		// Token: 0x040003B1 RID: 945
		public byte param3 = 1;

		// Token: 0x040003B2 RID: 946
		public byte param4 = 1;

		// Token: 0x040003B3 RID: 947
		public byte mentorlevel = 5;

		// Token: 0x040003B4 RID: 948
		public byte param14 = 1;

		// Token: 0x040003B5 RID: 949
		public byte guanjue = 1;

		// Token: 0x040003B6 RID: 950
		public ushort maxpetcall = 2;

		// Token: 0x040003B7 RID: 951
		public int exploit;

		// Token: 0x040003B8 RID: 952
		public int bonuspoint;

		// Token: 0x040003B9 RID: 953
		public byte edubroodpacksize;

		// Token: 0x040003BA RID: 954
		public byte winglevel;

		// Token: 0x040003BB RID: 955
		public byte godpetpackagelimit;

		// Token: 0x040003BC RID: 956
		public byte demonlev;

		// Token: 0x040003BD RID: 957
		public int demonexp;

		// Token: 0x040003BE RID: 958
		public int demonexpparam;

		// Token: 0x040003BF RID: 959
		public int param5 = 262164;

		// Token: 0x040003C0 RID: 960
		public int godlevel;

		// Token: 0x040003C1 RID: 961
		public byte param9;

		// Token: 0x040003C2 RID: 962
		public byte param11;

		// Token: 0x040003C3 RID: 963
		public ushort param10;

		// Token: 0x040003C4 RID: 964
		public int[] param6 = new int[21];

		// Token: 0x040003C5 RID: 965
		public int originalserverid;

		// Token: 0x040003C6 RID: 966
		public ushort wordtreeareaid;

		// Token: 0x040003C7 RID: 967
		public int[] param7 = new int[9];

		// Token: 0x040003C8 RID: 968
		public ushort param8 = 0;

		// Token: 0x040003C9 RID: 969
		public byte[] param13;
	}
}
