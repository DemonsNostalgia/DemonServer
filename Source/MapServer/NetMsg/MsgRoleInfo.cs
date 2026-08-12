using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000061 RID: 97
	public class MsgRoleInfo : BaseMsg
	{
		// Token: 0x06000208 RID: 520 RVA: 0x00015E18 File Offset: 0x00014018
		public MsgRoleInfo()
		{
			this.mMsgLen = 180;
			this.TodayGuideCountByOther = 5;
			this.armor_id = (this.wepon_id = 0U);
			this.rid_id = 0U;
			this.dir = 5;
			this.mParam = 1014;
			for (int i = 0; i < this.param.Length; i++)
			{
				this.param[i] = 0;
			}
			this.param3 = new byte[3];
			for (int i = 0; i < this.param3.Length; i++)
			{
				this.param3[i] = 0;
			}
			for (int i = 0; i < this.param5.Length; i++)
			{
				this.param5[i] = 0;
			}
			this.str = new List<string>();
		}

		// Token: 0x06000209 RID: 521 RVA: 0x00015F39 File Offset: 0x00014139
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600020A RID: 522 RVA: 0x00015F48 File Offset: 0x00014148
		public override byte[] GetBuffer()
		{
			int wireLength = 180;
			for (int i = 0; i < this.str.Count; i++)
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.str[i]);
				if (bytes.Length > byte.MaxValue)
				{
					throw new InvalidOperationException(
						"Role-info strings cannot exceed 255 encoded bytes.");
				}
				wireLength += 1 + bytes.Length;
			}
			if (this.str.Count > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Role-info packets cannot contain more than 255 strings.");
			}
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16((ushort)wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.role_id);
			packetOut.WriteUInt32(this.face_sex);
			packetOut.WriteUInt32(this.face_sex1);
			for (int i = 0; i < this.param.Length; i++)
			{
				packetOut.WriteInt32(this.param[i]);
			}
			packetOut.WriteUInt32(this.legion_id);
			packetOut.WriteUInt32(this.armor_id);
			packetOut.WriteUInt32(this.wepon_id);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteUInt32(this.rid_id);
			if (this.rid_id > 0U)
			{
				this.param11[22] = 75;
			}
			packetOut.WriteInt16(this.x);
			packetOut.WriteInt16(this.y);
			packetOut.WriteUInt32(this.hair_id);
			packetOut.WriteByte(this.dir);
			packetOut.WriteByte(this.TodayGuideCountByOther);
			packetOut.WriteUInt16(this.param2);
			packetOut.WriteUInt32(this.action);
			packetOut.WriteByte(this.level);
			packetOut.WriteByte(this.job);
			packetOut.WriteInt16(this.param6);
			packetOut.WriteByte(this.param7);
			packetOut.WriteInt16(this.param8);
			packetOut.WriteByte(this.guanjue);
			packetOut.WriteBuff(this.param9);
			packetOut.WriteByte(this.legion_title);
			for (int i = 0; i < 6; i++)
			{
				packetOut.WriteByte(this.param10[i]);
			}
			packetOut.WriteUInt32(this.family_id);
			packetOut.WriteUInt16(this.family_rank);
			packetOut.WriteInt16(this.legion_place);
			packetOut.WriteBuff(this.param11);
			packetOut.WriteUInt32(this.legion_id1);
			for (int i = 0; i < this.param5.Length; i++)
			{
				packetOut.WriteByte(this.param5[i]);
			}
			packetOut.WriteByte((byte)this.str.Count);
			for (int i = 0; i < this.str.Count; i++)
			{
				packetOut.WriteString(this.str[i]);
			}
			for (int i = 0; i < this.param3.Length; i++)
			{
				packetOut.WriteByte(this.param3[i]);
			}
			return packetOut.Flush();
		}

		// Token: 0x0400041F RID: 1055
		public uint role_id;

		// Token: 0x04000420 RID: 1056
		public uint face_sex;

		// Token: 0x04000421 RID: 1057
		public uint face_sex1;

		// Token: 0x04000422 RID: 1058
		public int[] param = new int[8];

		// Token: 0x04000423 RID: 1059
		public uint legion_id;

		// Token: 0x04000424 RID: 1060
		public uint armor_id;

		// Token: 0x04000425 RID: 1061
		public uint wepon_id;

		// Token: 0x04000426 RID: 1062
		public int param1 = 0;

		// Token: 0x04000427 RID: 1063
		public uint rid_id;

		// Token: 0x04000428 RID: 1064
		public short x;

		// Token: 0x04000429 RID: 1065
		public short y;

		// Token: 0x0400042A RID: 1066
		public uint hair_id;

		// Token: 0x0400042B RID: 1067
		public byte dir;

		// Token: 0x0400042C RID: 1068
		public byte TodayGuideCountByOther;

		// Token: 0x0400042D RID: 1069
		public ushort param2;

		// Token: 0x0400042E RID: 1070
		public uint action;

		// Token: 0x0400042F RID: 1071
		public byte level;

		// Token: 0x04000430 RID: 1072
		public byte job;

		// Token: 0x04000431 RID: 1073
		public short param6;

		// Token: 0x04000432 RID: 1074
		public byte param7;

		// Token: 0x04000433 RID: 1075
		public short param8;

		// Token: 0x04000434 RID: 1076
		public byte guanjue;

		// Token: 0x04000435 RID: 1077
		public byte[] param9 = new byte[9];

		// Token: 0x04000436 RID: 1078
		public byte legion_title;

		// Token: 0x04000437 RID: 1079
		public byte[] param10 = new byte[12];

		public uint family_id;

		public ushort family_rank;

		// Token: 0x04000438 RID: 1080
		public short legion_place;

		// Token: 0x04000439 RID: 1081
		public byte[] param11 = new byte[32];

		// Token: 0x0400043A RID: 1082
		public uint legion_id1;

		// Token: 0x0400043B RID: 1083
		public byte[] param5 = new byte[24];

		// Token: 0x0400043C RID: 1084
		public List<string> str;

		// Token: 0x0400043D RID: 1085
		public byte[] param3 = new byte[3];
	}
}
