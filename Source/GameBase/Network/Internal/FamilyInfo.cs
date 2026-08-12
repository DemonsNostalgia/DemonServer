using System;
using System.Collections.Generic;

namespace GameBase.Network.Internal
{
	public sealed class FamilyMember
	{
		public void Create(PackIn input)
		{
			PlayerId = input.ReadInt32();
			Name = input.ReadString();
			Rank = input.ReadUInt16();
			Proffer = input.ReadUInt32();
			JoinDate = input.ReadUInt32();
			AutoExercise = input.ReadByte();
			ExpDate = input.ReadUInt32();
		}

		public byte[] GetBuffer()
		{
			PacketOut output = new PacketOut(null);
			output.WriteInt32(PlayerId);
			output.WriteString(Name ?? "");
			output.WriteUInt16(Rank);
			output.WriteUInt32(Proffer);
			output.WriteUInt32(JoinDate);
			output.WriteByte(AutoExercise);
			output.WriteUInt32(ExpDate);
			return output.GetBuffer();
		}

		public int PlayerId;
		public string Name = "";
		public ushort Rank;
		public uint Proffer;
		public uint JoinDate;
		public byte AutoExercise;
		public uint ExpDate;
	}

	public sealed class FamilyInfo
	{
		public FamilyInfo()
		{
			AllyIds = new uint[5];
			EnemyIds = new uint[5];
			Members = new List<FamilyMember>();
		}

		public void Create(PackIn input)
		{
			Id = input.ReadUInt32();
			Name = input.ReadString();
			Rank = input.ReadByte();
			LeaderId = input.ReadInt32();
			LeaderName = input.ReadString();
			Announcement = input.ReadString();
			Money = input.ReadULong();
			Reputation = input.ReadUInt32();
			CreateDate = input.ReadUInt32();
			CreateName = input.ReadString();
			Deleted = input.ReadByte();
			StarTower = input.ReadByte();
			ChallengeMap = input.ReadUInt32();
			FamilyMap = input.ReadUInt32();
			Truce = input.ReadUInt32();
			for (int index = 0; index < AllyIds.Length; index++)
			{
				AllyIds[index] = input.ReadUInt32();
			}
			for (int index = 0; index < EnemyIds.Length; index++)
			{
				EnemyIds[index] = input.ReadUInt32();
			}
			Members.Clear();
			int memberCount = input.ReadInt32();
			for (int index = 0; index < memberCount; index++)
			{
				FamilyMember member = new FamilyMember();
				member.Create(input);
				Members.Add(member);
			}
		}

		public byte[] GetBuffer()
		{
			PacketOut output = new PacketOut(null);
			output.WriteUInt32(Id);
			output.WriteString(Name ?? "");
			output.WriteByte(Rank);
			output.WriteInt32(LeaderId);
			output.WriteString(LeaderName ?? "");
			output.WriteString(Announcement ?? "");
			output.WriteULong(Money);
			output.WriteUInt32(Reputation);
			output.WriteUInt32(CreateDate);
			output.WriteString(CreateName ?? "");
			output.WriteByte(Deleted);
			output.WriteByte(StarTower);
			output.WriteUInt32(ChallengeMap);
			output.WriteUInt32(FamilyMap);
			output.WriteUInt32(Truce);
			for (int index = 0; index < AllyIds.Length; index++)
			{
				output.WriteUInt32(AllyIds[index]);
			}
			for (int index = 0; index < EnemyIds.Length; index++)
			{
				output.WriteUInt32(EnemyIds[index]);
			}
			output.WriteInt32(Members.Count);
			for (int index = 0; index < Members.Count; index++)
			{
				output.WriteBuff(Members[index].GetBuffer());
			}
			return output.GetBuffer();
		}

		public uint Id;
		public string Name = "";
		public byte Rank;
		public int LeaderId;
		public string LeaderName = "";
		public string Announcement = "";
		public ulong Money;
		public uint Reputation;
		public uint CreateDate;
		public string CreateName = "";
		public byte Deleted;
		public byte StarTower;
		public uint ChallengeMap;
		public uint FamilyMap;
		public uint Truce;
		public uint[] AllyIds;
		public uint[] EnemyIds;
		public List<FamilyMember> Members;
	}

	public sealed class FamilyCollection
	{
		public const ushort Parameter = 142;

		public FamilyCollection()
		{
			Families = new List<FamilyInfo>();
		}

		public void Create(byte[] message)
		{
			PackIn input = new PackIn(message);
			input.ReadUInt16();
			int count = input.ReadInt32();
			for (int index = 0; index < count; index++)
			{
				FamilyInfo family = new FamilyInfo();
				family.Create(input);
				Families.Add(family);
			}
		}

		public byte[] GetBuffer()
		{
			PacketOut output = new PacketOut(null);
			output.WriteBuff(InternalPacket.HEAD);
			output.WriteUInt16(Parameter);
			output.WriteInt32(Families.Count);
			for (int index = 0; index < Families.Count; index++)
			{
				output.WriteBuff(Families[index].GetBuffer());
			}
			output.WriteBuff(InternalPacket.TAIL);
			return output.GetBuffer();
		}

		public List<FamilyInfo> Families;
	}

	public sealed class FamilyOption
	{
		public const ushort CreateParameter = 143;
		public const ushort UpdateParameter = 145;
		public const ushort DeleteParameter = 146;

		public FamilyOption()
		{
			Info = new FamilyInfo();
		}

		public void SetCreateTag()
		{
			Parameter = CreateParameter;
		}

		public void SetUpdateTag()
		{
			Parameter = UpdateParameter;
		}

		public void SetDeleteTag()
		{
			Parameter = DeleteParameter;
		}

		public void Create(byte[] message)
		{
			PackIn input = new PackIn(message);
			input.ReadUInt16();
			PlayerId = input.ReadInt32();
			Info.Create(input);
		}

		public byte[] GetBuffer()
		{
			PacketOut output = new PacketOut(null);
			output.WriteBuff(InternalPacket.HEAD);
			output.WriteUInt16(Parameter);
			output.WriteInt32(PlayerId);
			output.WriteBuff(Info.GetBuffer());
			output.WriteBuff(InternalPacket.TAIL);
			return output.GetBuffer();
		}

		public ushort Parameter;
		public int PlayerId;
		public FamilyInfo Info;
	}

	public sealed class CreateFamilyResult
	{
		public const ushort Parameter = 144;

		public void Create(byte[] message)
		{
			PackIn input = new PackIn(message);
			input.ReadUInt16();
			PlayerId = input.ReadInt32();
			FamilyId = input.ReadInt32();
			Success = input.ReadByte();
		}

		public byte[] GetBuffer()
		{
			PacketOut output = new PacketOut(null);
			output.WriteBuff(InternalPacket.HEAD);
			output.WriteUInt16(Parameter);
			output.WriteInt32(PlayerId);
			output.WriteInt32(FamilyId);
			output.WriteByte(Success);
			output.WriteBuff(InternalPacket.TAIL);
			return output.GetBuffer();
		}

		public int PlayerId;
		public int FamilyId;
		public byte Success;
	}
}
