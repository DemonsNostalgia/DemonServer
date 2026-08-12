using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	public class MsgFriendDetail : BaseMsg
	{
		public MsgFriendDetail()
		{
			this.mMsgLen = 52;
			this.mParam = 2033;
			this.mate = "";
			this.extendedValue = 1;
		}

		public override void Create(
			byte[] msg = null,
			GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.playerId);
			packetOut.WriteUInt32(this.lookface);
			packetOut.WriteByte(this.level);
			packetOut.WriteByte(this.profession);
			packetOut.WriteInt16(this.pkPoints);
			packetOut.WriteUInt32(this.legionIdAndRank);
			WriteFixedString(packetOut, this.mate, 16);
			packetOut.WriteByte(this.nobilityRank);
			packetOut.WriteByte(this.relationType);
			packetOut.WriteByte(this.extendedByte);
			packetOut.WriteByte(0);
			packetOut.WriteUInt16(this.extendedShort);
			packetOut.WriteUInt16(0);
			packetOut.WriteUInt32(this.extendedDword);
			packetOut.WriteUInt16(this.extendedValue);
			packetOut.WriteUInt16(0);
			return packetOut.Flush();
		}

		private static void WriteFixedString(
			PacketOut output,
			string value,
			int fieldLength)
		{
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(value ?? "");
			if (encoded.Length >= fieldLength)
			{
				throw new InvalidOperationException(
					"Friend detail strings must fit their fixed field.");
			}
			output.WriteBuff(encoded);
			output.WriteBuff(new byte[fieldLength - encoded.Length]);
		}

		public const byte RELATION_FRIEND = 0;

		public const byte RELATION_ENEMY = 1;

		public const byte RELATION_SECONDARY_FRIEND = 2;

		public uint playerId;

		public uint lookface;

		public byte level;

		public byte profession;

		public short pkPoints;

		public uint legionIdAndRank;

		public string mate;

		public byte nobilityRank;

		public byte relationType;

		public byte extendedByte;

		public ushort extendedShort;

		public uint extendedDword;

		public ushort extendedValue;
	}
}
