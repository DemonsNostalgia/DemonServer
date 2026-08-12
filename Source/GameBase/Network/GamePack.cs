using System;
using System.Collections.Generic;
using System.IO;

namespace GameBase.Network
{
	// Token: 0x02000005 RID: 5
	public class GamePack
	{
		// Token: 0x0600001B RID: 27 RVA: 0x0000268E File Offset: 0x0000088E
		public GamePack()
		{
			this.m_Key = new GamePacketKeyEx();
			this.m_Key.InitKey();
			this.m_stream = new MemoryStream();
			this.m_ListData = new List<byte[]>();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000026C8 File Offset: 0x000008C8
		public byte[] GetData()
		{
			lock (this.m_SyncRoot)
			{
				if (this.m_ListData.Count == 0)
				{
					return null;
				}
				byte[] result = this.m_ListData[0];
				this.m_ListData.RemoveAt(0);
				return result;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002710 File Offset: 0x00000910
		public void ProcessNetData(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				return;
			}

			lock (this.m_SyncRoot)
			{
				byte[] decoded = new byte[data.Length];
				Buffer.BlockCopy(data, 0, decoded, 0, data.Length);
				this.m_Key.DecodePacket(ref decoded, decoded.Length);
				this.m_stream.Position = this.m_stream.Length;
				this.m_stream.Write(decoded, 0, decoded.Length);

				int consumed = 0;
				byte[] buffered = this.m_stream.GetBuffer();
				while (this.m_stream.Length - consumed >= 2)
				{
					int packetLength = BitConverter.ToUInt16(buffered, consumed);
					if (packetLength < 4)
					{
						this.m_stream.SetLength(0L);
						this.m_stream.Position = 0L;
						return;
					}
					if (this.m_stream.Length - consumed < packetLength)
					{
						break;
					}

					byte[] payload = new byte[packetLength - 2];
					Buffer.BlockCopy(
						buffered, consumed + 2, payload, 0, payload.Length);
					this.m_ListData.Add(payload);
					consumed += packetLength;
				}

				if (consumed == 0)
				{
					return;
				}

				int remaining = (int)this.m_stream.Length - consumed;
				if (remaining > 0)
				{
					Buffer.BlockCopy(buffered, consumed, buffered, 0, remaining);
				}
				this.m_stream.SetLength(remaining);
				this.m_stream.Position = remaining;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000287A File Offset: 0x00000A7A
		public void SunUpdateKey(int key, int key2)
		{
			this.m_Key.SunUpdateKey(key, key2);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000288B File Offset: 0x00000A8B
		public void ResetKey()
		{
			this.m_Key.InitKey();
		}

		// Token: 0x04000008 RID: 8
		public GamePacketKeyEx m_Key;

		// Token: 0x04000009 RID: 9
		private MemoryStream m_stream;

		// Token: 0x0400000A RID: 10
		private List<byte[]> m_ListData;

		private readonly object m_SyncRoot = new object();
	}
}
