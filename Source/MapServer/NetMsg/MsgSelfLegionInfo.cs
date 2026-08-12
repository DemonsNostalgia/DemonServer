using System;
using GameBase.Core;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x02000088 RID: 136
	public class MsgSelfLegionInfo : BaseMsg
	{
		// Token: 0x0600028E RID: 654 RVA: 0x0001A5E4 File Offset: 0x000187E4
		public MsgSelfLegionInfo()
		{
			this.mMsgLen = 108;
			this.mParam = 1106;
			this.leader_name = "";
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0001A64D File Offset: 0x0001884D
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0001A65C File Offset: 0x0001885C
		public override byte[] GetBuffer()
		{
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.legion_id);
			packetOut.WriteInt32(this.proffer);
			packetOut.WriteBuff(this.battle_pet_resources);
			packetOut.WriteInt32(this.population);
			packetOut.WriteInt32(this.fame);
			packetOut.WriteInt16(0);
			packetOut.WriteInt16(this.rank);
			packetOut.WriteByte(this.syndicate_rank);
			packetOut.WriteByte(this.syndicate_level);
			packetOut.WriteByte(this.member_title);
			packetOut.WriteByte(this.guide_status);
			packetOut.WriteBuff(this.battle_pet_status);
			WriteFixedString(packetOut, this.leader_name, 18);
			packetOut.WriteUInt32(
				this.original_legion_id == 0U ?
					this.legion_id :
					this.original_legion_id);
			packetOut.WriteBuff(this.trailing_status);
			return packetOut.Flush();
		}

		private static void WriteFixedString(
			PacketOut output,
			string value,
			int fieldLength)
		{
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(value ?? "");
			int writeLength = Math.Min(encoded.Length, fieldLength - 1);
			while (writeLength > 0)
			{
				string candidate = Coding.GetDefauleCoding().GetString(
					encoded, 0, writeLength);
				byte[] roundTrip =
					Coding.GetDefauleCoding().GetBytes(candidate);
				if (roundTrip.Length <= writeLength)
				{
					output.WriteBuff(roundTrip);
					output.WriteBuff(
						new byte[fieldLength - roundTrip.Length]);
					return;
				}
				writeLength--;
			}
			output.WriteBuff(new byte[fieldLength]);
		}

		public uint legion_id;

		public int proffer;

		public byte[] battle_pet_resources = new byte[16];

		public int population;

		public int fame;

		public short rank;

		public byte syndicate_rank;

		public byte syndicate_level = 1;

		public byte member_title;

		public byte guide_status;

		public byte[] battle_pet_status = new byte[22];

		public string leader_name;

		public uint original_legion_id;

		public byte[] trailing_status = new byte[20];
	}
}
