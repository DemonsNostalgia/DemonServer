using System;
using System.Collections;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000051 RID: 81
	public class MsgNotice : BaseMsg
	{
		// Token: 0x060001CC RID: 460 RVA: 0x00013A78 File Offset: 0x00011C78
		public MsgNotice()
		{
			this.mParam = 1004;
			this.mMsgLen = 27;
			this.strlist = new ArrayList();
			this.rgba = (this.param = (this.param1 = (this.param2 = 0)));
			this.type = (this.tag = (this.param3 = (ushort)(this.btype = 0)));
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00013AF0 File Offset: 0x00011CF0
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				this.rgba = packIn.ReadInt32();
				this.type = packIn.ReadUInt16();
				this.tag = packIn.ReadUInt16();
				this.param = packIn.ReadInt32();
				this.param1 = packIn.ReadInt32();
				this.param2 = packIn.ReadInt32();
				this.btype = packIn.ReadByte();
				byte len = packIn.ReadByte();
				this.str1 = packIn.ReadString((int)len);
				len = packIn.ReadByte();
				this.str2 = packIn.ReadString((int)len);
				packIn.ReadByte();
				len = packIn.ReadByte();
				this.str3 = packIn.ReadString((int)len);
				packIn.ReadByte();
				this.param3 = packIn.ReadUInt16();
			}
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00013BC5 File Offset: 0x00011DC5
		public override void Process()
		{
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00013BC8 File Offset: 0x00011DC8
		public byte[] GetQueryNameBuff(bool isSuccess)
		{
			this.rgba = 16777215;
			this.type = 2100;
			this.tag = 0;
			this.param = 834;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			if (isSuccess)
			{
				this.str3 = "REGIST_NAME_CHECK_SUC";
			}
			else
			{
				this.str3 = "This nickname already exists, please enter a different one!";
			}
			this.param3 = 0;
			this.str4 = "";
			return this.GetBuffer();
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00013C68 File Offset: 0x00011E68
		public byte[] GetStartGameBuff()
		{
			this.rgba = 16777215;
			this.type = 2101;
			this.tag = 0;
			this.param = 834;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			this.str3 = "ANSWER_OK";
			this.param3 = 0;
			this.str4 = "";
			return this.GetBuffer();
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00013CF0 File Offset: 0x00011EF0
		public byte[] GetChatNoticeBuff(string text)
		{
			this.rgba = 16777215;
			this.type = 2000;
			this.tag = 0;
			this.param = 834;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			this.str3 = text;
			this.str4 = "";
			this.param3 = 0;
			return this.GetBuffer();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00013D74 File Offset: 0x00011F74
		public byte[] GetMsgBoxBuff(string text)
		{
			this.rgba = 16777215;
			this.type = 2112;
			this.tag = 0;
			this.param = 2325;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			this.str3 = text;
			this.str4 = "";
			this.param3 = 0;
			return this.GetBuffer();
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00013DF8 File Offset: 0x00011FF8
		public byte[] GetSceneNoticeBuff(string text)
		{
			this.rgba = 16777215;
			this.type = 2011;
			this.tag = 0;
			this.param = 834;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			this.str3 = text;
			this.str4 = "";
			this.param3 = 0;
			return this.GetBuffer();
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00013E7C File Offset: 0x0001207C
		public byte[] GetCreateRoleBuff()
		{
			this.rgba = 16777215;
			this.type = 2101;
			this.tag = 1;
			this.param = 834;
			this.param1 = -1;
			this.param2 = 0;
			this.btype = 4;
			this.str1 = "SYSTEM";
			this.str2 = "ALLUSERS";
			this.str3 = "NEW_ROLE";
			this.str4 = "";
			this.param3 = 0;
			return this.GetBuffer();
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00013F04 File Offset: 0x00012104
		public override byte[] GetBuffer()
		{
			string[] strings =
			{
				this.str1 ?? "",
				this.str2 ?? "",
				this.str3 ?? "",
				this.str4 ?? ""
			};
			PacketOut packetOut = new PacketOut(this.mKey);
			int wireLength = 28;
			for (int i = 0; i < strings.Length; i++)
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(strings[i]);
				if (bytes.Length > byte.MaxValue)
				{
					throw new InvalidOperationException(
						"Notice strings cannot exceed 255 encoded bytes.");
				}
				wireLength += bytes.Length + 1;
			}
			packetOut.WriteUInt16((ushort)wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.rgba);
			packetOut.WriteUInt16(this.type);
			packetOut.WriteUInt16(this.tag);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteByte(this.btype);
			packetOut.WriteString(strings[0]);
			packetOut.WriteString(strings[1]);
			packetOut.WriteByte(0);
			packetOut.WriteString(strings[2]);
			packetOut.WriteString(strings[3]);
			packetOut.WriteUInt16(this.param3);
			return packetOut.Flush();
		}

		// Token: 0x0400037B RID: 891
		public const byte TAG_ISROLE_TRUE = 0;

		// Token: 0x0400037C RID: 892
		public const byte TAG_ISROLE_FALSE = 1;

		// Token: 0x0400037D RID: 893
		public int rgba;

		// Token: 0x0400037E RID: 894
		public ushort type;

		// Token: 0x0400037F RID: 895
		public ushort tag;

		// Token: 0x04000380 RID: 896
		public int param;

		// Token: 0x04000381 RID: 897
		public int param1;

		// Token: 0x04000382 RID: 898
		public int param2;

		// Token: 0x04000383 RID: 899
		public byte btype;

		// Token: 0x04000384 RID: 900
		public ArrayList strlist;

		// Token: 0x04000385 RID: 901
		public string str1;

		// Token: 0x04000386 RID: 902
		public string str2;

		// Token: 0x04000387 RID: 903
		public string str3;

		// Token: 0x04000388 RID: 904
		public string str4;

		// Token: 0x04000389 RID: 905
		public ushort param3;
	}
}
