using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000050 RID: 80
	public class BaseMsg
	{
		// Token: 0x060001C7 RID: 455 RVA: 0x00013A06 File Offset: 0x00011C06
		public BaseMsg()
		{
			this.m_Data = null;
			this.mKey = null;
			this.mMsgLen = 0;
			this.mParam = 0;
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x00013A2D File Offset: 0x00011C2D
		public virtual void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			this.m_Data = msg;
			this.mKey = key;
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00013A3E File Offset: 0x00011C3E
		public virtual void Process()
		{
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00013A41 File Offset: 0x00011C41
		public virtual void Reset()
		{
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00013A44 File Offset: 0x00011C44
		public virtual byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteBuff(this.m_Data);
			return packetOut.Flush();
		}

		// Token: 0x04000377 RID: 887
		protected byte[] m_Data;

		// Token: 0x04000378 RID: 888
		protected ushort mMsgLen;

		// Token: 0x04000379 RID: 889
		protected ushort mParam;

		// Token: 0x0400037A RID: 890
		protected GamePacketKeyEx mKey;
	}
}
