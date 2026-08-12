using System;
using GameBase.Core;

namespace GameBase.Network
{
	public sealed class AccountMetadataPacket
	{
		public string Account;
		public string Token;
		public string AdvertisedIp;
		public byte[] Reserved;
		public int AdvertisedPort;
		public int Status;
		public int AccountId;
	}

	public sealed class LegacyGameLoginPacket
	{
		public int Mode;
		public string Account;
		public byte[] PasswordBlock;
		public string ServerName;
		public byte[] Fingerprint;
		public int Value1;
		public int Value2;
		public ushort Value3;
	}

	public sealed class DirectGameLoginPacket
	{
		public int Mode;
		public string Account;
		public string Password;
		public string ServerName;
	}

	public sealed class LoginClientInfoPacket
	{
		public int Value;
		public string DeviceIdentifier;
	}

	public sealed class LoginClientStatusPacket
	{
		public int Value1;
		public int Value2;
		public string StatusText;
	}

	public static class LoginPacketCodec
	{
		public const ushort AccountMetadataType = 1083;
		public const ushort GameServerInfoType = 1057;
		public const ushort InitialKeyType = 1059;
		public const ushort ClientStatusType = 1052;
		public const ushort LegacyGameLoginType = 1095;
		public const ushort ClientInfoType = 1100;
		public const ushort DirectGameLoginType = 1120;

		public const int AccountMetadataWireLength = 256;
		public const int GameServerInfoWireLength = 84;
		public const int InitialKeyWireLength = 8;
		public const int ClientStatusWireLength = 28;
		public const int LegacyGameLoginWireLength = 340;
		public const int ClientInfoWireLength = 52;
		public const int DirectGameLoginWireLength = 392;

		public static bool TryReadAccountMetadata(
			byte[] payload,
			out AccountMetadataPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, AccountMetadataType, AccountMetadataWireLength, out error))
			{
				return false;
			}

			packet = new AccountMetadataPacket
			{
				Account = ReadFixedString(payload, 2, 128),
				Token = ReadFixedString(payload, 130, 40),
				AdvertisedIp = ReadFixedString(payload, 170, 32),
				Reserved = Copy(payload, 202, 40),
				AdvertisedPort = BitConverter.ToInt32(payload, 242),
				Status = BitConverter.ToInt32(payload, 246),
				AccountId = BitConverter.ToInt32(payload, 250)
			};
			return true;
		}

		public static bool TryReadLegacyGameLogin(
			byte[] payload,
			out LegacyGameLoginPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, LegacyGameLoginType, LegacyGameLoginWireLength, out error))
			{
				return false;
			}

			packet = new LegacyGameLoginPacket
			{
				Mode = BitConverter.ToInt32(payload, 2),
				Account = ReadFixedString(payload, 6, 128),
				PasswordBlock = Copy(payload, 134, 128),
				ServerName = ReadFixedString(payload, 262, 32),
				Fingerprint = Copy(payload, 294, 32),
				Value1 = BitConverter.ToInt32(payload, 326),
				Value2 = BitConverter.ToInt32(payload, 330),
				Value3 = BitConverter.ToUInt16(payload, 334)
			};
			return true;
		}

		public static bool TryReadDirectGameLogin(
			byte[] payload,
			out DirectGameLoginPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, DirectGameLoginType, DirectGameLoginWireLength, out error))
			{
				return false;
			}

			packet = new DirectGameLoginPacket
			{
				Mode = BitConverter.ToInt32(payload, 2),
				Account = ReadFixedString(payload, 6, 128),
				Password = ReadFixedString(payload, 134, 128),
				ServerName = ReadFixedString(payload, 262, 128)
			};
			return true;
		}

		public static bool TryReadClientInfo(
			byte[] payload,
			out LoginClientInfoPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, ClientInfoType, ClientInfoWireLength, out error))
			{
				return false;
			}

			packet = new LoginClientInfoPacket
			{
				Value = BitConverter.ToInt32(payload, 2),
				DeviceIdentifier = ReadFixedString(payload, 6, 44)
			};
			return true;
		}

		public static bool TryReadClientStatus(
			byte[] payload,
			out LoginClientStatusPacket packet,
			out string error)
		{
			packet = null;
			if (!ValidatePayload(
				payload, ClientStatusType, ClientStatusWireLength, out error))
			{
				return false;
			}

			packet = new LoginClientStatusPacket
			{
				Value1 = BitConverter.ToInt32(payload, 2),
				Value2 = BitConverter.ToInt32(payload, 6),
				StatusText = ReadFixedString(payload, 10, 16)
			};
			return true;
		}

		public static byte[] CreateInitialKey(
			GamePacketKeyEx encryption,
			int initialKey)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(InitialKeyWireLength);
			output.WriteUInt16(InitialKeyType);
			output.WriteInt32(initialKey);
			return output.Flush();
		}

		public static byte[] CreateAccountResult(
			GamePacketKeyEx encryption,
			int status,
			int accountId)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(AccountMetadataWireLength);
			output.WriteUInt16(AccountMetadataType);
			WriteZeroes(output, 244);
			output.WriteInt32(status);
			output.WriteInt32(accountId);
			return output.Flush();
		}

		public static byte[] CreateGameServerSuccess(
			GamePacketKeyEx encryption,
			int sessionKey1,
			int sessionKey2,
			int primaryPort,
			int primaryRouteValue,
			string primaryAddress,
			int secondaryRouteValue,
			int secondaryPort,
			string secondaryAddress)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(GameServerInfoWireLength);
			output.WriteUInt16(GameServerInfoType);
			output.WriteInt32(sessionKey1);
			output.WriteInt32(sessionKey2);
			output.WriteInt32(primaryPort);
			output.WriteInt32(primaryRouteValue);
			WriteFixedString(output, primaryAddress, 28);
			output.WriteInt32(secondaryRouteValue);
			output.WriteInt32(secondaryPort);
			WriteFixedString(output, secondaryAddress, 28);
			return output.Flush();
		}

		public static byte[] CreateGameServerFailure(
			GamePacketKeyEx encryption,
			int errorCode)
		{
			PacketOut output = new PacketOut(encryption);
			output.WriteUInt16(GameServerInfoWireLength);
			output.WriteUInt16(GameServerInfoType);
			output.WriteInt32(0);
			output.WriteInt32(errorCode);
			WriteZeroes(output, GameServerInfoWireLength - 12);
			return output.Flush();
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
			int expectedPayloadLength = expectedWireLength - 2;
			if (payload.Length != expectedPayloadLength)
			{
				error = "payload length " + payload.Length +
					" does not match expected " + expectedPayloadLength;
				return false;
			}
			ushort actualType = BitConverter.ToUInt16(payload, 0);
			if (actualType != expectedType)
			{
				error = "packet type " + actualType +
					" does not match expected " + expectedType;
				return false;
			}
			error = null;
			return true;
		}

		private static string ReadFixedString(byte[] data, int offset, int length)
		{
			int terminator = Array.IndexOf(data, (byte)0, offset, length);
			int count = terminator < 0 ? length : terminator - offset;
			return Coding.GetDefauleCoding().GetString(data, offset, count);
		}

		private static byte[] Copy(byte[] data, int offset, int length)
		{
			byte[] result = new byte[length];
			Buffer.BlockCopy(data, offset, result, 0, length);
			return result;
		}

		private static void WriteFixedString(
			PacketOut output,
			string value,
			int fieldLength)
		{
			byte[] encoded = Coding.GetDefauleCoding().GetBytes(value ?? string.Empty);
			if (encoded.Length >= fieldLength)
			{
				throw new ArgumentException(
					"Encoded fixed string must be shorter than " + fieldLength +
					" bytes.", "value");
			}
			output.WriteBuff(encoded);
			WriteZeroes(output, fieldLength - encoded.Length);
		}

		private static void WriteZeroes(PacketOut output, int count)
		{
			for (int index = 0; index < count; index++)
			{
				output.WriteByte(0);
			}
		}
	}
}
