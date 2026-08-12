using System;
using System.Collections.Generic;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000068 RID: 104
	public class MsgGroupMagicAttackInfo : BaseMsg
	{
		// Token: 0x06000221 RID: 545 RVA: 0x00016D58 File Offset: 0x00014F58
		public MsgGroupMagicAttackInfo()
		{
			this.List_Obj = new List<uint>();
			this.List_Value = new List<int>();
			this.mParam = 1105;
			this.mMsgLen = 32;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x00016D93 File Offset: 0x00014F93
		public void AddObject(uint nTypeId, int nInjured)
		{
			this.List_Obj.Add(nTypeId);
			this.List_Value.Add(nInjured);
		}

		// Token: 0x06000223 RID: 547 RVA: 0x00016DB0 File Offset: 0x00014FB0
		public void SetSigleAttack(uint id)
		{
			this.nTargetID = id;
			this.bSigle = true;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x00016DC4 File Offset: 0x00014FC4
		public override byte[] GetBuffer()
		{
			byte[] array = new byte[18];
			PacketOut packetOut = new PacketOut(this.mKey);
			this.mMsgLen += (ushort)(this.List_Obj.Count * 29 + array.Length);
			if (this.bSigle)
			{
				this.mMsgLen += 13;
			}
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.nID);
			if (this.bSigle)
			{
				packetOut.WriteUInt32(this.nTargetID);
			}
			else
			{
				packetOut.WriteInt16(this.nX);
				packetOut.WriteInt16(this.nY);
			}
			packetOut.WriteUInt16(this.nMagicID);
			packetOut.WriteUInt16(this.nMagicLv);
			packetOut.WriteByte(this.bDir);
			packetOut.WriteByte((byte)this.List_Obj.Count);
			packetOut.WriteBuff(array);
			if (this.bSigle)
			{
				array = new byte[43];
			}
			else
			{
				array = new byte[20];
			}
			for (int i = 0; i < this.List_Obj.Count; i++)
			{
				packetOut.WriteUInt32(this.List_Obj[i]);
				packetOut.WriteInt32(this.List_Value[i]);
				packetOut.WriteBuff(array);
			}
			packetOut.WriteInt32(0);
			packetOut.WriteInt32(0);
			packetOut.WriteBuff(array);
			return packetOut.Flush();
		}

		// Token: 0x040004AB RID: 1195
		public uint nID;

		// Token: 0x040004AC RID: 1196
		public short nX;

		// Token: 0x040004AD RID: 1197
		public short nY;

		// Token: 0x040004AE RID: 1198
		public uint nTargetID;

		// Token: 0x040004AF RID: 1199
		public ushort nMagicID;

		// Token: 0x040004B0 RID: 1200
		public ushort nMagicLv;

		// Token: 0x040004B1 RID: 1201
		public byte bDir;

		// Token: 0x040004B2 RID: 1202
		private List<uint> List_Obj;

		// Token: 0x040004B3 RID: 1203
		private List<int> List_Value;

		// Token: 0x040004B4 RID: 1204
		private bool bSigle = false;
	}
}
