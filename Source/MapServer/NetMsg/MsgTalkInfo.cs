using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006D RID: 109
	public class MsgTalkInfo : BaseMsg
	{
		// Token: 0x06000230 RID: 560 RVA: 0x00017474 File Offset: 0x00015674
		public MsgTalkInfo()
		{
			this.mMsgLen = 28;
			this.mParam = 1004;
			this.rgba = 0;
			this.unTxtAttribute = 0;
			this.tag = 0;
			this.param = (this.param1 = (this.param2 = (this.param3 = 0)));
			this.strcount = 0;
			this.liststr = new List<string>();
		}

		// Token: 0x06000231 RID: 561 RVA: 0x000174E8 File Offset: 0x000156E8
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				this.liststr.Clear();
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.rgba = packIn.ReadInt32();
				this.unTxtAttribute = packIn.ReadUInt16();
				this.tag = packIn.ReadUInt16();
				this.param = packIn.ReadInt32();
				this.param1 = packIn.ReadInt32();
				this.param2 = packIn.ReadInt32();
				byte b = packIn.ReadByte();
				this.strcount = b;
				for (int i = 0; i < (int)b; i++)
				{
					string item = packIn.ReadString();
					this.liststr.Add(item);
				}
				packIn.ReadByte();
				packIn.ReadByte();
				packIn.ReadByte();
			}
		}

		// Token: 0x06000232 RID: 562 RVA: 0x000175A8 File Offset: 0x000157A8
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			if (this.liststr.Count > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Talk packets cannot contain more than 255 strings.");
			}
			int wireLength = 28;
			for (int i = 0; i < this.liststr.Count; i++)
			{
				byte[] bytes = Coding.GetDefauleCoding().GetBytes(this.liststr[i]);
				if (bytes.Length > byte.MaxValue)
				{
					throw new InvalidOperationException(
						"Talk strings cannot exceed 255 encoded bytes.");
				}
				wireLength += 1 + bytes.Length;
			}
			this.strcount = (byte)this.liststr.Count;
			packetOut.WriteUInt16((ushort)wireLength);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteInt32(this.rgba);
			packetOut.WriteUInt16(this.unTxtAttribute);
			packetOut.WriteUInt16(this.tag);
			packetOut.WriteInt32(this.param);
			packetOut.WriteInt32(this.param1);
			packetOut.WriteInt32(this.param2);
			packetOut.WriteByte(this.strcount);
			for (int i = 0; i < (int)this.strcount; i++)
			{
				packetOut.WriteString(this.liststr[i]);
			}
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			packetOut.WriteByte(0);
			return packetOut.Flush();
		}

		// Token: 0x06000233 RID: 563 RVA: 0x000176B8 File Offset: 0x000158B8
		public string GetTalkRoleText()
		{
			string result;
			if (this.liststr.Count != 4)
			{
				result = "";
			}
			else
			{
				result = this.liststr[0];
			}
			return result;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x000176F0 File Offset: 0x000158F0
		public string GetTalkTargetText()
		{
			string result;
			if (this.liststr.Count != 4)
			{
				result = "";
			}
			else
			{
				result = this.liststr[1];
			}
			return result;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x00017728 File Offset: 0x00015928
		public string GetEmtionText()
		{
			string result;
			if (this.liststr.Count != 4)
			{
				result = "";
			}
			else
			{
				result = this.liststr[2];
			}
			return result;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x00017760 File Offset: 0x00015960
		public string GetTalkText()
		{
			string result;
			if (this.liststr.Count != 4)
			{
				result = "";
			}
			else
			{
				result = this.liststr[3];
			}
			return result;
		}

		public bool TryValidatePlayerMessage(
			string expectedSender,
			out string error)
		{
			if (this.liststr.Count != 4)
			{
				error = "talk packet must contain four strings";
				return false;
			}
			if (!string.Equals(
				this.GetTalkRoleText(),
				expectedSender,
				StringComparison.Ordinal))
			{
				error = "talk packet sender does not match the authenticated role";
				return false;
			}
			if (string.IsNullOrEmpty(this.GetTalkText()))
			{
				error = "talk packet message is empty";
				return false;
			}
			if (this.unTxtAttribute == _TXTATR_PRIVATE &&
				string.IsNullOrEmpty(this.GetTalkTargetText()))
			{
				error = "private-message target is empty";
				return false;
			}
			error = null;
			return true;
		}

		// Token: 0x040004F7 RID: 1271
		public const ushort _TXTATR_PRIVATE = 2001;

		// Token: 0x040004F8 RID: 1272
		public const ushort _TXTATR_ACTION = 2002;

		// Token: 0x040004F9 RID: 1273
		public const ushort _TXTATR_TEAM = 2003;

		// Token: 0x040004FA RID: 1274
		public const ushort _TXTATR_SYNDICATE = 2004;

		// Token: 0x040004FB RID: 1275
		public const ushort _TXTATR_SYSTEM = 2005;

		// Token: 0x040004FC RID: 1276
		public const ushort _TXTATR_FAMILY = 2006;

		// Token: 0x040004FD RID: 1277
		public const ushort _TXTATR_TALK = 2007;

		// Token: 0x040004FE RID: 1278
		public const ushort _TXTATR_YELP = 2008;

		// Token: 0x040004FF RID: 1279
		public const ushort _TXTATR_FRIEND = 2009;

		// Token: 0x04000500 RID: 1280
		public const ushort _TXTATR_GLOBAL = 2010;

		// Token: 0x04000501 RID: 1281
		public const ushort _TXTATR_GM = 2011;

		// Token: 0x04000502 RID: 1282
		public const ushort _TXTATR_WHISPER = 2022;

		// Token: 0x04000503 RID: 1283
		public const ushort _TXTATR_GHOST = 2023;

		// Token: 0x04000504 RID: 1284
		public const ushort _TXTATR_SERVE = 2024;

		// Token: 0x04000505 RID: 1285
		public const ushort _TXTATR_REJECT = 2113;

		public int rgba;

		// Token: 0x04000506 RID: 1286
		public int param;

		// Token: 0x04000507 RID: 1287
		public ushort unTxtAttribute;

		// Token: 0x04000508 RID: 1288
		public ushort tag;

		// Token: 0x04000509 RID: 1289
		public int param1;

		// Token: 0x0400050A RID: 1290
		public int param2;

		// Token: 0x0400050B RID: 1291
		public int param3;

		// Token: 0x0400050C RID: 1292
		public byte strcount;

		// Token: 0x0400050D RID: 1293
		public List<string> liststr;
	}
}
