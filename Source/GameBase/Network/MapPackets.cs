using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Core;

namespace GameBase.Network
{
	public sealed class MapConnectPacket
	{
		public uint Key1;
		public uint Key2;
		public uint ValueAt12;
		public uint ValueAt16;
		public uint ValueAt20;
		public uint ValueAt24;
		public uint ValueAt28;
		public byte ValueAt32;
		public string Identifier;
		public byte[] ReservedTail;
	}

	public sealed class SystemTimePacket
	{
		public short Status;
		public uint ServerEpoch;
		public int ServerZone;
	}

	public sealed class MonsterFaceStatusPacket
	{
		public uint RoleId;
		public byte Action;
		public byte EntryCount;
		public ushort QueryMode;
		public uint StatusId;
		public int Value;
	}

	public sealed class SyndicateQueryPacket
	{
		public ushort Action;
		public ushort Reserved;
		public uint TargetId;
		public uint FealtyId;
		public byte Level;
		public string[] Strings;
		public byte[] ReservedTail;
	}

	public sealed class FamilyQueryPacket
	{
		public ushort Action;
		public ushort Reserved;
		public uint ValueA;
		public uint ValueB;
		public uint ValueC;
		public string[] Strings;
		public byte[] ReservedTail;
	}

	public sealed class NameQueryPacket
	{
		public uint TargetId;
		public ushort Action;
		public string[] Strings;
		public byte ReservedTail;
	}

	public sealed class SyndicateMemberQueryPacket
	{
		public string MemberName;
	}

	public sealed class PkItemListPacket
	{
		public uint SubjectId;
		public ushort Action;
		public ushort Reserved;
		public uint Value;
		public uint EntryCountOrResult;
		public byte[] PkItemRecord;
	}

	public sealed class EudemonPackagePacket
	{
		public uint PackageId;
		public uint Context;
		public byte Operation;
		public byte PackageType;
		public uint OperationValue;
		public ushort EntryCount;
		public uint OperationItemId;
		public byte[] EudemonItemRecord;
	}

	public sealed class EudemonPackageItem
	{
		public uint ItemId;
		public uint ItemTypeId;
		public uint PhysicalAttackMinimum;
		public uint PhysicalAttackMaximum;
		public uint MagicAttackMinimum;
		public uint MagicAttackMaximum;
		public uint Defense;
		public uint MagicDefense;
		public uint Life;
		public uint MaximumLife;
		public ushort Fidelity;
		public ushort RebornTimes;
		public ulong Experience;
		public byte ExperienceType;
		public byte Level;
		public byte Status;
		public byte RespawnAvailable;
		public ushort Luck;
		public byte DamageType;
		public byte Talent1;
		public byte Talent2;
		public byte Talent3;
		public byte Talent4;
		public byte Talent5;
		public ushort InitialLife;
		public ushort InitialDefense;
		public ushort InitialPhysicalAttack;
		public ushort InitialMagicAttack;
		public uint IdentityCard;
		public int Quality;
		public uint RemainingSeconds;
		public ushort AdditionalRebornTimes;
		public ushort AlchemyLevel1;
		public uint SpecialFlag;
		public byte GodExperienceType;
		public byte IsGodEudemon;
		public byte GodLevel;
		public ulong GodStrength;
		public int InstantKill;
		public ushort Sagacity;
		public ushort Wisdom;
		public ushort AlchemyLevel2;
		public ushort GodRank;
		public ushort GodRankSoul;
		public string Name;
		public uint SpecialFlag2;
	}

	public sealed class WardrobePackageItem
	{
		public uint ItemId;
		public uint ItemTypeId;
		public ushort Amount;
		public ushort AmountLimit;
	}

	public sealed class Action2Packet
	{
		public uint Timestamp;
		public uint ValueAt8;
		public uint ContextAt12;
		public ushort ValueAt16;
		public ushort ValueAt18;
		public ushort ValueAt20;
		public ushort Action;
		public uint ValueAt24;
		public uint ReservedAt28;
	}

	public sealed class DataArrayPacket
	{
		public ushort Command;
		public byte Count;
		public byte Reserved;
		public uint[] Values;
		public uint TrailingReserved;
	}

	public sealed class TeamActionPacket
	{
		public ushort Action;
		public uint TargetId;
	}

	public sealed class TeamMemberPacketRecord
	{
		public string Name;
		public uint RoleId;
		public uint Look;
		public ushort Life;
		public ushort MaximumLife;
		public ushort Profession;
		public ushort Level;
		public string SyndicateName;
		public byte SyndicateRank;
		public uint TeamCreateTime;
		public ushort X;
		public ushort Y;
		public bool IsOnline;
	}

	public static class MapPacketCodec
	{
		public const ushort MapConnectType = 1052;
		public const ushort ActionType = 1010;
		public const ushort TeamActionType = 1023;
		public const ushort TeamMemberType = 1026;
		public const ushort TeamInfoType = 1124;
		public const ushort MonsterFaceStatusType = 1028;
		public const ushort Action2Type = 1032;
		public const ushort NameQueryType = 1015;
		public const ushort SyndicateType = 1107;
		public const ushort SyndicateMemberType = 1112;
		public const ushort EudemonPackageType = 1117;
		public const ushort WardrobePackageType = 1102;
		public const ushort SystemTimeType = 1123;
		public const ushort PkItemListType = 1142;
		public const ushort DataArrayType = 2036;
		public const ushort FamilyType = 2051;
		public const ushort FamilyAttributeType = 2053;
		public const ushort FamilyMemberType = 2054;
		public const ushort GoddessSummaryCommand = 282;
		public const ushort GoddessUnlocksCommand = 283;
		public const ushort GoddessBaptizeStateCommand = 284;
		public const ushort GoddessRandomStateCommand = 285;
		public const ushort GoddessRandomRequestCommand = 286;
		public const int MapConnectWireLength = 52;
		public const int ActionWireLength = 28;
		public const int TeamActionWireLength = 20;
		public const int TeamCreatedWireLength = 20;
		public const int TeamInvitationWireLength = 42;
		public const int TeamMemberHeaderWireLength = 8;
		public const int TeamMemberRecordLength = 72;
		public const int MonsterFaceStatusWireLength = 27;
		public const int Action2WireLength = 32;
		public const int SyndicateMinimumWireLength = 20;
		public const int NameQueryMinimumWireLength = 12;
		public const int SyndicateMemberWireLength = 56;
		public const int EudemonPackageWireLength = 172;
		public const int EudemonPackageHeaderWireLength = 20;
		public const int EudemonPackageRecordLength = 152;
		public const int WardrobePackageEmptyWireLength = 176;
		public const int WardrobePackageHeaderWireLength = 24;
		public const int WardrobePackageRecordLength = 152;
		public const int SystemTimeWireLength = 16;
		public const int PkItemListWireLength = 148;
		public const int DataArrayBaseWireLength = 12;
		public const int FamilyMinimumWireLength = 24;
		public const int FamilyAttributeWireLength = 88;
		public const int FamilyMemberWireLength = 60;

		public static bool TryReadMapConnect(
			byte[] payload,
			out MapConnectPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, MapConnectType, MapConnectWireLength, out error))
			{
				return false;
			}

			int identifierLength = Array.IndexOf(
				payload, (byte)0, 31, 16);
			if (identifierLength < 0)
			{
				error = "map-connect identifier is not null terminated";
				return false;
			}
			identifierLength -= 31;

			packet = new MapConnectPacket
			{
				Key1 = BitConverter.ToUInt32(payload, 2),
				Key2 = BitConverter.ToUInt32(payload, 6),
				ValueAt12 = BitConverter.ToUInt32(payload, 10),
				ValueAt16 = BitConverter.ToUInt32(payload, 14),
				ValueAt20 = BitConverter.ToUInt32(payload, 18),
				ValueAt24 = BitConverter.ToUInt32(payload, 22),
				ValueAt28 = BitConverter.ToUInt32(payload, 26),
				ValueAt32 = payload[30],
				Identifier = Encoding.Default.GetString(
					payload, 31, identifierLength),
				ReservedTail = new byte[3]
			};
			Buffer.BlockCopy(payload, 47, packet.ReservedTail, 0, 3);
			return true;
		}

		public static bool TryReadTeamAction(
			byte[] payload,
			out TeamActionPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, TeamActionType, TeamActionWireLength, out error))
			{
				return false;
			}
			if (BitConverter.ToUInt32(payload, 4) != 0 ||
				BitConverter.ToUInt16(payload, 8) != 0 ||
				BitConverter.ToUInt32(payload, 14) != 0)
			{
				error = "team action reserved fields are nonzero";
				return false;
			}

			packet = new TeamActionPacket
			{
				Action = BitConverter.ToUInt16(payload, 2),
				TargetId = BitConverter.ToUInt32(payload, 10)
			};
			error = null;
			return true;
		}

		public static byte[] CreateTeamActionResponse(
			GamePacketKeyEx encryption,
			ushort action,
			uint targetId)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(TeamActionWireLength);
			output.WriteUInt16(TeamActionType);
			output.WriteUInt16(action);
			output.WriteUInt32(0);
			output.WriteUInt16(0);
			output.WriteUInt32(targetId);
			output.WriteUInt32(0);
			return output.Flush();
		}

		public static byte[] CreateTeamCreatedResponse(
			GamePacketKeyEx encryption,
			uint teamCreateTime)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(TeamCreatedWireLength);
			output.WriteUInt16(TeamInfoType);
			output.WriteUInt16(1);
			output.WriteUInt16(0);
			output.WriteUInt32(0);
			output.WriteUInt32(teamCreateTime);
			output.WriteUInt32(0);
			return output.Flush();
		}

		public static byte[] CreateTeamInvitationResponse(
			GamePacketKeyEx encryption,
			uint inviterId,
			uint look,
			uint maximumLife,
			byte sex,
			byte level,
			byte profession,
			string inviterName)
		{
			byte[] packet = new byte[TeamInvitationWireLength];
			WriteUInt16(packet, 0, TeamInvitationWireLength);
			WriteUInt16(packet, 2, TeamInfoType);
			WriteUInt16(packet, 4, 0);
			WriteUInt32(packet, 8, inviterId);
			WriteUInt32(packet, 12, look);
			WriteUInt32(packet, 16, maximumLife);
			packet[20] = sex > 0 ? unchecked((byte)(sex - 1)) : (byte)0;
			packet[21] = level;
			packet[22] = profession;
			WriteFixedString(packet, 26, 16, inviterName);
			return EncryptPacket(packet, encryption);
		}

		public static byte[] CreateTeamMemberResponse(
			GamePacketKeyEx encryption,
			byte action,
			TeamMemberPacketRecord[] members)
		{
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			if (members.Length > byte.MaxValue)
			{
				throw new ArgumentOutOfRangeException(
					"members", "A team packet supports at most 255 members.");
			}

			int wireLength = checked(
				TeamMemberHeaderWireLength +
				members.Length * TeamMemberRecordLength);
			byte[] packet = new byte[wireLength];
			WriteUInt16(packet, 0, unchecked((ushort)wireLength));
			WriteUInt16(packet, 2, TeamMemberType);
			packet[4] = action;
			packet[5] = (byte)members.Length;
			for (int index = 0; index < members.Length; index++)
			{
				TeamMemberPacketRecord member = members[index];
				if (member == null)
				{
					throw new ArgumentException(
						"Team member records cannot be null.", "members");
				}
				int offset = TeamMemberHeaderWireLength +
					index * TeamMemberRecordLength;
				WriteFixedString(packet, offset, 16, member.Name);
				WriteUInt32(packet, offset + 0x10, member.RoleId);
				WriteUInt32(packet, offset + 0x14, member.Look);
				WriteUInt16(packet, offset + 0x18, member.Life);
				WriteUInt16(packet, offset + 0x1a, member.MaximumLife);
				WriteUInt16(packet, offset + 0x1c, member.Profession);
				WriteUInt16(packet, offset + 0x1e, member.Level);
				WriteFixedString(packet, offset + 0x20, 16,
					member.SyndicateName);
				packet[offset + 0x30] = member.SyndicateRank;
				WriteUInt32(packet, offset + 0x34, member.TeamCreateTime);
				WriteUInt16(packet, offset + 0x38, member.X);
				WriteUInt16(packet, offset + 0x3a, member.Y);
				packet[offset + 0x3e] = member.IsOnline ? (byte)1 : (byte)0;
			}
			return EncryptPacket(packet, encryption);
		}

		public static bool TryReadMonsterFaceStatus(
			byte[] payload,
			out MonsterFaceStatusPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload,
				MonsterFaceStatusType,
				MonsterFaceStatusWireLength,
				out error))
			{
				return false;
			}

			packet = new MonsterFaceStatusPacket
			{
				RoleId = BitConverter.ToUInt32(payload, 2),
				Action = payload[6],
				EntryCount = payload[7],
				QueryMode = BitConverter.ToUInt16(payload, 8),
				StatusId = BitConverter.ToUInt32(payload, 10),
				Value = BitConverter.ToInt32(payload, 14)
			};
			return true;
		}

		public static bool TryReadAction2(
			byte[] payload,
			out Action2Packet packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, Action2Type, Action2WireLength, out error))
			{
				return false;
			}

			packet = new Action2Packet
			{
				Timestamp = BitConverter.ToUInt32(payload, 2),
				ValueAt8 = BitConverter.ToUInt32(payload, 6),
				ContextAt12 = BitConverter.ToUInt32(payload, 10),
				ValueAt16 = BitConverter.ToUInt16(payload, 14),
				ValueAt18 = BitConverter.ToUInt16(payload, 16),
				ValueAt20 = BitConverter.ToUInt16(payload, 18),
				Action = BitConverter.ToUInt16(payload, 20),
				ValueAt24 = BitConverter.ToUInt32(payload, 22),
				ReservedAt28 = BitConverter.ToUInt32(payload, 26)
			};
			return true;
		}

		public static byte[] CreateAction2Response(
			GamePacketKeyEx encryption,
			uint timestamp,
			uint valueAt8,
			uint contextAt12,
			ushort valueAt16,
			ushort valueAt18,
			ushort valueAt20,
			ushort action,
			uint valueAt24,
			uint reservedAt28)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(Action2WireLength);
			output.WriteUInt16(Action2Type);
			output.WriteUInt32(timestamp);
			output.WriteUInt32(valueAt8);
			output.WriteUInt32(contextAt12);
			output.WriteUInt16(valueAt16);
			output.WriteUInt16(valueAt18);
			output.WriteUInt16(valueAt20);
			output.WriteUInt16(action);
			output.WriteUInt32(valueAt24);
			output.WriteUInt32(reservedAt28);
			return output.Flush();
		}

		public static byte[] CreateActionResponse(
			GamePacketKeyEx encryption,
			int timestamp,
			uint roleId,
			int type,
			int parameter,
			int value,
			int action)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(ActionWireLength);
			output.WriteUInt16(ActionType);
			output.WriteInt32(timestamp);
			output.WriteUInt32(roleId);
			output.WriteInt32(type);
			output.WriteInt32(parameter);
			output.WriteInt32(value);
			output.WriteInt32(action);
			return output.Flush();
		}

		public static bool TryReadDataArray(
			byte[] payload,
			out DataArrayPacket packet,
			out string error)
		{
			packet = null;
			if (payload == null)
			{
				error = "payload is null";
				return false;
			}
			if (payload.Length < DataArrayBaseWireLength - 2)
			{
				error = "payload length " + payload.Length +
					" is shorter than minimum " +
					(DataArrayBaseWireLength - 2);
				return false;
			}

			ushort packetType = BitConverter.ToUInt16(payload, 0);
			if (packetType != DataArrayType)
			{
				error = "packet type " + packetType +
					" does not match expected " + DataArrayType;
				return false;
			}

			byte count = payload[4];
			int expectedPayloadLength =
				DataArrayBaseWireLength - 2 + count * 4;
			if (payload.Length != expectedPayloadLength)
			{
				error = "payload length " + payload.Length +
					" does not match count-derived expected " +
					expectedPayloadLength;
				return false;
			}

			uint[] values = new uint[count];
			for (int index = 0; index < count; index++)
			{
				values[index] = BitConverter.ToUInt32(
					payload, 6 + index * 4);
			}

			packet = new DataArrayPacket
			{
				Command = BitConverter.ToUInt16(payload, 2),
				Count = count,
				Reserved = payload[5],
				Values = values,
				TrailingReserved = BitConverter.ToUInt32(
					payload, 6 + count * 4)
			};
			error = null;
			return true;
		}

		public static bool TryReadSyndicateQuery(
			byte[] payload,
			out SyndicateQueryPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidateMinimumPayload(
				payload,
				SyndicateType,
				SyndicateMinimumWireLength,
				out error))
			{
				return false;
			}

			string[] strings;
			byte[] reservedTail;
			if (!TryReadStringPacker(
				payload, 15, 2, out strings, out reservedTail, out error))
			{
				return false;
			}

			packet = new SyndicateQueryPacket
			{
				Action = BitConverter.ToUInt16(payload, 2),
				Reserved = BitConverter.ToUInt16(payload, 4),
				TargetId = BitConverter.ToUInt32(payload, 6),
				FealtyId = BitConverter.ToUInt32(payload, 10),
				Level = payload[14],
				Strings = strings,
				ReservedTail = reservedTail
			};
			error = null;
			return true;
		}

		public static bool TryReadFamilyQuery(
			byte[] payload,
			out FamilyQueryPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidateMinimumPayload(
				payload, FamilyType, FamilyMinimumWireLength, out error))
			{
				return false;
			}

			string[] strings;
			byte[] reservedTail;
			if (!TryReadStringPacker(
				payload, 18, 3, out strings, out reservedTail, out error))
			{
				return false;
			}

			packet = new FamilyQueryPacket
			{
				Action = BitConverter.ToUInt16(payload, 2),
				Reserved = BitConverter.ToUInt16(payload, 4),
				ValueA = BitConverter.ToUInt32(payload, 6),
				ValueB = BitConverter.ToUInt32(payload, 10),
				ValueC = BitConverter.ToUInt32(payload, 14),
				Strings = strings,
				ReservedTail = reservedTail
			};
			error = null;
			return true;
		}

		public static byte[] CreateFamilyResponse(
			GamePacketKeyEx encryption,
			ushort action,
			uint valueA,
			uint valueB,
			uint valueC,
			params string[] strings)
		{
			if (strings == null)
			{
				strings = new string[0];
			}
			if (strings.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Family packets cannot contain more than 255 strings.");
			}

			int wireLength = FamilyMinimumWireLength;
			byte[][] encodedStrings = EncodeStrings(strings, ref wireLength);
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(checked((ushort)wireLength));
			output.WriteUInt16(FamilyType);
			output.WriteUInt16(action);
			output.WriteUInt16(0);
			output.WriteUInt32(valueA);
			output.WriteUInt32(valueB);
			output.WriteUInt32(valueC);
			WriteStringPacker(output, encodedStrings);
			output.WriteBuff(new byte[3]);
			return output.Flush();
		}

		public static byte[] CreateFamilyAttributeResponse(
			GamePacketKeyEx encryption,
			byte[] attributeRecord)
		{
			if (attributeRecord == null || attributeRecord.Length != 80)
			{
				throw new ArgumentException(
					"Family attribute records must contain exactly 80 bytes.",
					"attributeRecord");
			}
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(FamilyAttributeWireLength);
			output.WriteUInt16(FamilyAttributeType);
			output.WriteUInt16(1);
			output.WriteUInt16(0);
			output.WriteBuff(attributeRecord);
			return output.Flush();
		}

		public static byte[] CreateFamilyRelationResponse(
			GamePacketKeyEx encryption,
			ushort action,
			uint familyId,
			uint[] relations)
		{
			if (action != 2 && action != 3)
			{
				throw new ArgumentOutOfRangeException("action");
			}
			if (relations == null || relations.Length != 5)
			{
				throw new ArgumentException(
					"Family relation records require five family IDs.",
					"relations");
			}
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(32);
			output.WriteUInt16(FamilyAttributeType);
			output.WriteUInt16(action);
			output.WriteUInt16(0);
			output.WriteUInt32(familyId);
			for (int index = 0; index < relations.Length; index++)
			{
				output.WriteUInt32(relations[index]);
			}
			return output.Flush();
		}

		public static byte[] CreateFamilyMemberResponse(
			GamePacketKeyEx encryption,
			byte[] memberRecord)
		{
			if (memberRecord == null || memberRecord.Length != 56)
			{
				throw new ArgumentException(
					"Family member records must contain exactly 56 bytes.",
					"memberRecord");
			}
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(FamilyMemberWireLength);
			output.WriteUInt16(FamilyMemberType);
			output.WriteBuff(memberRecord);
			return output.Flush();
		}

		public static byte[] CreateSyndicateResponse(
			GamePacketKeyEx encryption,
			ushort action,
			uint targetId,
			uint fealtyId,
			byte level,
			params string[] strings)
		{
			return CreateStringPackerPacket(
				encryption,
				SyndicateType,
				action,
				targetId,
				fealtyId,
				level,
				2,
				strings);
		}

		public static bool TryReadNameQuery(
			byte[] payload,
			out NameQueryPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidateMinimumPayload(
				payload,
				NameQueryType,
				NameQueryMinimumWireLength,
				out error))
			{
				return false;
			}

			string[] strings;
			byte[] reservedTail;
			if (!TryReadStringPacker(
				payload, 8, 1, out strings, out reservedTail, out error))
			{
				return false;
			}

			packet = new NameQueryPacket
			{
				TargetId = BitConverter.ToUInt32(payload, 2),
				Action = BitConverter.ToUInt16(payload, 6),
				Strings = strings,
				ReservedTail = reservedTail[0]
			};
			error = null;
			return true;
		}

		public static byte[] CreateNameQueryResponse(
			GamePacketKeyEx encryption,
			uint targetId,
			ushort action,
			params string[] strings)
		{
			if (strings == null)
			{
				strings = new string[0];
			}
			if (strings.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Name packets cannot contain more than 255 strings.");
			}

			int wireLength = NameQueryMinimumWireLength;
			byte[][] encodedStrings = EncodeStrings(strings, ref wireLength);
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(checked((ushort)wireLength));
			output.WriteUInt16(NameQueryType);
			output.WriteUInt32(targetId);
			output.WriteUInt16(action);
			WriteStringPacker(output, encodedStrings);
			output.WriteByte(0);
			return output.Flush();
		}

		public static bool TryReadSyndicateMemberQuery(
			byte[] payload,
			out SyndicateMemberQueryPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload,
				SyndicateMemberType,
				SyndicateMemberWireLength,
				out error))
			{
				return false;
			}

			int terminator = Array.IndexOf(payload, (byte)0, 14, 16);
			if (terminator < 0)
			{
				error = "syndicate member name is not null terminated";
				return false;
			}

			packet = new SyndicateMemberQueryPacket
			{
				MemberName = Encoding.Default.GetString(
					payload, 14, terminator - 14)
			};
			error = null;
			return true;
		}

		public static byte[] CreateSyndicateMemberResponse(
			GamePacketKeyEx encryption,
			ushort rank,
			bool online,
			byte level,
			byte profession,
			uint roleId,
			int contribution)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(SyndicateMemberWireLength);
			output.WriteUInt16(SyndicateMemberType);
			output.WriteUInt16(rank);
			output.WriteByte(online ? (byte)1 : (byte)0);
			output.WriteByte(level);
			output.WriteByte(0);
			output.WriteByte(0);
			output.WriteUInt16(profession);
			output.WriteUInt32(roleId);
			output.WriteBuff(new byte[32]);
			output.WriteInt32(contribution);
			output.WriteByte(0);
			output.WriteByte(0);
			output.WriteUInt16(1);
			return output.Flush();
		}

		public static bool TryReadPkItemList(
			byte[] payload,
			out PkItemListPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, PkItemListType, PkItemListWireLength, out error))
			{
				return false;
			}

			packet = new PkItemListPacket
			{
				SubjectId = BitConverter.ToUInt32(payload, 2),
				Action = BitConverter.ToUInt16(payload, 6),
				Reserved = BitConverter.ToUInt16(payload, 8),
				Value = BitConverter.ToUInt32(payload, 10),
				EntryCountOrResult = BitConverter.ToUInt32(payload, 14),
				PkItemRecord = new byte[128]
			};
			Buffer.BlockCopy(payload, 18, packet.PkItemRecord, 0, 128);
			return true;
		}

		public static bool TryReadEudemonPackage(
			byte[] payload,
			out EudemonPackagePacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload,
				EudemonPackageType,
				EudemonPackageWireLength,
				out error))
			{
				return false;
			}

			packet = new EudemonPackagePacket
			{
				PackageId = BitConverter.ToUInt32(payload, 2),
				Context = BitConverter.ToUInt32(payload, 6),
				Operation = payload[10],
				PackageType = payload[11],
				OperationValue = BitConverter.ToUInt32(payload, 12),
				EntryCount = BitConverter.ToUInt16(payload, 16),
				OperationItemId = BitConverter.ToUInt32(payload, 16),
				EudemonItemRecord = new byte[152]
			};
			Buffer.BlockCopy(payload, 18, packet.EudemonItemRecord, 0, 152);
			return true;
		}

		public static bool TryReadSystemTime(
			byte[] payload,
			out SystemTimePacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, SystemTimeType, SystemTimeWireLength, out error))
			{
				return false;
			}

			packet = new SystemTimePacket
			{
				Status = BitConverter.ToInt16(payload, 2),
				ServerEpoch = BitConverter.ToUInt32(payload, 6),
				ServerZone = BitConverter.ToInt32(payload, 10)
			};
			error = null;
			return true;
		}

		public static byte[] CreateSystemTimeResponse(
			GamePacketKeyEx encryption,
			short status,
			uint serverEpoch,
			int serverZone)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(SystemTimeWireLength);
			output.WriteUInt16(SystemTimeType);
			output.WriteInt16(status);
			output.WriteUInt16(0);
			output.WriteUInt32(serverEpoch);
			output.WriteInt32(serverZone);
			return output.Flush();
		}

		public static byte[] CreateEmptyEudemonPackageResponse(
			GamePacketKeyEx encryption,
			uint packageId,
			uint context,
			byte packageType)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(EudemonPackageWireLength);
			output.WriteUInt16(EudemonPackageType);
			output.WriteUInt32(packageId);
			output.WriteUInt32(context);
			output.WriteByte(0);
			output.WriteByte(packageType);
			output.WriteUInt32(0);
			output.WriteUInt16(0);
			for (int index = 0; index < 152; index++)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateEudemonPackageListResponse(
			GamePacketKeyEx encryption,
			uint packageId,
			uint context,
			byte operation,
			byte packageType,
			uint operationValue,
			IList<EudemonPackageItem> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (items.Count > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException("items");
			}

			int wireLength = items.Count == 0
				? EudemonPackageWireLength
				: EudemonPackageHeaderWireLength +
					(EudemonPackageRecordLength * items.Count);
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16((ushort)wireLength);
			output.WriteUInt16(EudemonPackageType);
			output.WriteUInt32(packageId);
			output.WriteUInt32(context);
			output.WriteByte(operation);
			output.WriteByte(packageType);
			output.WriteUInt32(operationValue);
			output.WriteUInt16((ushort)items.Count);
			for (int index = 0; index < items.Count; index++)
			{
				output.WriteBuff(CreateEudemonPackageRecord(items[index]));
			}
			while (output.GetPostion() < wireLength)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateEudemonPackageRemovalResponse(
			GamePacketKeyEx encryption,
			uint packageId,
			uint context,
			byte packageType,
			uint itemId)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(EudemonPackageWireLength);
			output.WriteUInt16(EudemonPackageType);
			output.WriteUInt32(packageId);
			output.WriteUInt32(context);
			output.WriteByte(2);
			output.WriteByte(packageType);
			output.WriteUInt32(0);
			output.WriteUInt32(itemId);
			while (output.GetPostion() < EudemonPackageWireLength)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateWardrobePackageListResponse(
			GamePacketKeyEx encryption,
			byte packageType,
			ushort maximumItems,
			IList<WardrobePackageItem> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (items.Count > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException("items");
			}

			int wireLength = items.Count == 0
				? WardrobePackageEmptyWireLength
				: WardrobePackageHeaderWireLength +
					(WardrobePackageRecordLength * items.Count);
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(checked((ushort)wireLength));
			output.WriteUInt16(WardrobePackageType);
			output.WriteUInt32(0);
			output.WriteByte(0);
			output.WriteByte(packageType);
			output.WriteInt16(0);
			output.WriteUInt32(maximumItems);
			output.WriteUInt32(0);
			output.WriteUInt16((ushort)items.Count);
			output.WriteUInt16(0);
			for (int index = 0; index < items.Count; index++)
			{
				output.WriteBuff(CreateWardrobePackageRecord(items[index]));
			}
			while (output.GetPostion() < wireLength)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateWardrobeMountStateResponse(
			GamePacketKeyEx encryption,
			uint roleId,
			uint mountServerType,
			uint mountItemId)
		{
			const ushort mountStateWireLength = 36;
			const ushort mountStateCommand = 209;
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(mountStateWireLength);
			output.WriteUInt16(DataArrayType);
			output.WriteUInt16(mountStateCommand);
			output.WriteByte(7);
			output.WriteByte(0);
			output.WriteUInt32(roleId);
			output.WriteUInt32(mountServerType);
			output.WriteUInt32(mountItemId);
			output.WriteUInt32(75);
			output.WriteUInt32(1);
			output.WriteUInt32(20);
			output.WriteUInt32(0);
			return output.Flush();
		}

		public static uint GetRemainingHatchSeconds(
			int finishTime,
			int currentTime)
		{
			if (finishTime <= currentTime)
			{
				return 0;
			}

			return (uint)((long)finishTime - currentTime);
		}

		private static byte[] CreateEudemonPackageRecord(
			EudemonPackageItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			byte[] record = new byte[EudemonPackageRecordLength];
			Buffer.BlockCopy(BitConverter.GetBytes(item.ItemId), 0, record, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.ItemTypeId), 0, record, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.PhysicalAttackMinimum),
				0, record, 8, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.PhysicalAttackMaximum),
				0, record, 12, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.MagicAttackMinimum),
				0, record, 16, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.MagicAttackMaximum),
				0, record, 20, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Defense), 0, record, 24, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.MagicDefense),
				0, record, 28, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Life), 0, record, 32, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.MaximumLife),
				0, record, 36, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Fidelity), 0, record, 40, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.RebornTimes),
				0, record, 42, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Experience),
				0, record, 48, 8);
			record[56] = item.ExperienceType;
			record[57] = item.Level;
			record[58] = item.Status;
			record[59] = item.RespawnAvailable;
			Buffer.BlockCopy(BitConverter.GetBytes(item.Luck), 0, record, 60, 2);
			record[62] = item.DamageType;
			record[63] = item.Talent1;
			record[64] = item.Talent2;
			record[65] = item.Talent3;
			record[66] = item.Talent4;
			record[67] = item.Talent5;
			Buffer.BlockCopy(BitConverter.GetBytes(item.InitialLife),
				0, record, 68, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.InitialDefense),
				0, record, 70, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.InitialPhysicalAttack),
				0, record, 72, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.InitialMagicAttack),
				0, record, 74, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.IdentityCard),
				0, record, 76, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Quality),
				0, record, 80, 4);
			Buffer.BlockCopy(
				BitConverter.GetBytes(item.RemainingSeconds), 0, record, 84, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.AdditionalRebornTimes),
				0, record, 88, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.AlchemyLevel1),
				0, record, 90, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.SpecialFlag),
				0, record, 92, 4);
			record[96] = item.GodExperienceType;
			record[97] = item.IsGodEudemon;
			record[98] = item.GodLevel;
			Buffer.BlockCopy(BitConverter.GetBytes(item.GodStrength),
				0, record, 104, 8);
			Buffer.BlockCopy(BitConverter.GetBytes(item.InstantKill),
				0, record, 112, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Sagacity),
				0, record, 116, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Wisdom),
				0, record, 118, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.AlchemyLevel2),
				0, record, 120, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.GodRank),
				0, record, 122, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.GodRankSoul),
				0, record, 124, 2);
			byte[] nameBytes = Coding.GetDefauleCoding().GetBytes(item.Name ?? "");
			int nameLength = Math.Min(nameBytes.Length, 16);
			Buffer.BlockCopy(nameBytes, 0, record, 126, nameLength);
			Buffer.BlockCopy(BitConverter.GetBytes(item.SpecialFlag2),
				0, record, 144, 4);
			return record;
		}

		private static byte[] CreateWardrobePackageRecord(
			WardrobePackageItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			byte[] record = new byte[WardrobePackageRecordLength];
			Buffer.BlockCopy(BitConverter.GetBytes(item.ItemId), 0, record, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.ItemTypeId), 0, record, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(item.Amount), 0, record, 8, 2);
			Buffer.BlockCopy(BitConverter.GetBytes(item.AmountLimit), 0, record, 10, 2);
			return record;
		}

		public static byte[] CreateEmptyPkItemListResponse(
			GamePacketKeyEx encryption,
			ushort action)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(PkItemListWireLength);
			output.WriteUInt16(PkItemListType);
			output.WriteUInt32(0);
			output.WriteUInt16(action);
			output.WriteUInt16(0);
			output.WriteUInt32(0);
			output.WriteUInt32(0);
			for (int index = 0; index < 128; index++)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateMonsterFaceStatusResponse(
			GamePacketKeyEx encryption,
			uint roleId,
			byte action,
			byte entryCount,
			ushort queryMode,
			uint statusId,
			int value)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(MonsterFaceStatusWireLength);
			output.WriteUInt16(MonsterFaceStatusType);
			output.WriteUInt32(roleId);
			output.WriteByte(action);
			output.WriteByte(entryCount);
			output.WriteUInt16(queryMode);
			output.WriteUInt32(statusId);
			output.WriteInt32(value);
			for (int index = 0; index < 7; index++)
			{
				output.WriteByte(0);
			}
			return output.Flush();
		}

		public static byte[] CreateDataArray(
			GamePacketKeyEx encryption,
			ushort command,
			uint[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			if (values.Length > byte.MaxValue)
			{
				throw new ArgumentOutOfRangeException(
					"values", "A data-array packet supports at most 255 values.");
			}

			int wireLength = DataArrayBaseWireLength + values.Length * 4;
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16((ushort)wireLength);
			output.WriteUInt16(DataArrayType);
			output.WriteUInt16(command);
			output.WriteByte((byte)values.Length);
			output.WriteByte(0);
			for (int index = 0; index < values.Length; index++)
			{
				output.WriteUInt32(values[index]);
			}
			output.WriteUInt32(0);
			return output.Flush();
		}

		private static byte[] EncryptPacket(
			byte[] packet,
			GamePacketKeyEx encryption)
		{
			if (encryption != null)
			{
				encryption.EncodePacket(ref packet, packet.Length);
			}
			return packet;
		}

		private static void WriteFixedString(
			byte[] target,
			int offset,
			int size,
			string value)
		{
			byte[] encoded = Encoding.Default.GetBytes(value ?? "");
			Buffer.BlockCopy(encoded, 0, target, offset,
				Math.Min(size - 1, encoded.Length));
		}

		private static void WriteUInt16(
			byte[] target,
			int offset,
			uint value)
		{
			byte[] encoded = BitConverter.GetBytes(unchecked((ushort)value));
			Buffer.BlockCopy(encoded, 0, target, offset, encoded.Length);
		}

		private static void WriteUInt32(
			byte[] target,
			int offset,
			uint value)
		{
			byte[] encoded = BitConverter.GetBytes(value);
			Buffer.BlockCopy(encoded, 0, target, offset, encoded.Length);
		}

		private static bool ValidatePayload(
			byte[] payload,
			ushort expectedType,
			int expectedWireLength,
			out string error)
		{
			if (payload == null)
			{
				error = "payload is null";
				return false;
			}
			if (payload.Length != expectedWireLength - 2)
			{
				error = "payload length " + payload.Length +
					" does not match expected " + (expectedWireLength - 2);
				return false;
			}
			ushort packetType = BitConverter.ToUInt16(payload, 0);
			if (packetType != expectedType)
			{
				error = "packet type " + packetType +
					" does not match expected " + expectedType;
				return false;
			}
			error = null;
			return true;
		}

		private static bool ValidateMinimumPayload(
			byte[] payload,
			ushort expectedType,
			int minimumWireLength,
			out string error)
		{
			if (payload == null)
			{
				error = "payload is null";
				return false;
			}
			if (payload.Length < minimumWireLength - 2)
			{
				error = "payload length " + payload.Length +
					" is shorter than minimum " + (minimumWireLength - 2);
				return false;
			}
			ushort packetType = BitConverter.ToUInt16(payload, 0);
			if (packetType != expectedType)
			{
				error = "packet type " + packetType +
					" does not match expected " + expectedType;
				return false;
			}
			error = null;
			return true;
		}

		private static bool TryReadStringPacker(
			byte[] payload,
			int countOffset,
			int reservedTailLength,
			out string[] strings,
			out byte[] reservedTail,
			out string error)
		{
			strings = null;
			reservedTail = null;
			if (payload.Length < countOffset + 1 + reservedTailLength)
			{
				error = "string packer is truncated";
				return false;
			}

			int count = payload[countOffset];
			int offset = countOffset + 1;
			strings = new string[count];
			for (int index = 0; index < count; index++)
			{
				if (offset >= payload.Length - reservedTailLength)
				{
					error = "string " + index + " length is missing";
					return false;
				}
				int stringLength = payload[offset++];
				if (offset + stringLength >
					payload.Length - reservedTailLength)
				{
					error = "string " + index + " exceeds packet length";
					return false;
				}
				strings[index] = Encoding.Default.GetString(
					payload, offset, stringLength);
				offset += stringLength;
			}

			if (offset != payload.Length - reservedTailLength)
			{
				error = "string packer has " +
					(payload.Length - reservedTailLength - offset) +
					" unclaimed bytes";
				return false;
			}

			reservedTail = new byte[reservedTailLength];
			Buffer.BlockCopy(
				payload, offset, reservedTail, 0, reservedTailLength);
			error = null;
			return true;
		}

		private static byte[] CreateStringPackerPacket(
			GamePacketKeyEx encryption,
			ushort packetType,
			ushort action,
			uint targetId,
			uint fealtyId,
			byte level,
			int reservedTailLength,
			string[] strings)
		{
			if (strings == null)
			{
				strings = new string[0];
			}
			if (strings.Length > byte.MaxValue)
			{
				throw new InvalidOperationException(
					"Syndicate packets cannot contain more than 255 strings.");
			}

			int wireLength = SyndicateMinimumWireLength;
			byte[][] encodedStrings = EncodeStrings(strings, ref wireLength);
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(checked((ushort)wireLength));
			output.WriteUInt16(packetType);
			output.WriteUInt16(action);
			output.WriteUInt16(0);
			output.WriteUInt32(targetId);
			output.WriteUInt32(fealtyId);
			output.WriteByte(level);
			WriteStringPacker(output, encodedStrings);
			output.WriteBuff(new byte[reservedTailLength]);
			return output.Flush();
		}

		private static byte[][] EncodeStrings(
			string[] strings,
			ref int wireLength)
		{
			byte[][] encodedStrings = new byte[strings.Length][];
			for (int index = 0; index < strings.Length; index++)
			{
				encodedStrings[index] = Encoding.Default.GetBytes(
					strings[index] ?? "");
				if (encodedStrings[index].Length > byte.MaxValue)
				{
					throw new InvalidOperationException(
						"Packet strings cannot exceed 255 encoded bytes.");
				}
				wireLength = checked(
					wireLength + 1 + encodedStrings[index].Length);
			}
			return encodedStrings;
		}

		private static void WriteStringPacker(
			PacketOut output,
			byte[][] encodedStrings)
		{
			output.WriteByte((byte)encodedStrings.Length);
			for (int index = 0; index < encodedStrings.Length; index++)
			{
				output.WriteByte((byte)encodedStrings[index].Length);
				output.WriteBuff(encodedStrings[index]);
			}
		}
	}
}
