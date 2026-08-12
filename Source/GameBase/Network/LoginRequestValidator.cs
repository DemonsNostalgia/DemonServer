using System;
using GameBase.Core;

namespace GameBase.Network
{
	public static class LoginRequestValidator
	{
		public const int MaximumAccountBytes = 15;
		public const int MaximumPasswordBytes = 127;
		public const int MaximumServerNameBytes = 127;

		public static bool TryValidateDirectLogin(
			string account,
			string password,
			string serverName,
			out string error)
		{
			if (!TryValidateAccountAndServer(account, serverName, out error))
			{
				return false;
			}
			if (string.IsNullOrEmpty(password))
			{
				error = "password is empty";
				return false;
			}
			if (Coding.GetDefauleCoding().GetByteCount(password) >
				MaximumPasswordBytes)
			{
				error = "password exceeds the 127-byte client field";
				return false;
			}
			if (ContainsControlCharacter(password))
			{
				error = "password contains a control character";
				return false;
			}

			error = null;
			return true;
		}

		public static bool TryValidateLegacyLogin(
			string account,
			string serverName,
			out string error)
		{
			return TryValidateAccountAndServer(account, serverName, out error);
		}

		private static bool TryValidateAccountAndServer(
			string account,
			string serverName,
			out string error)
		{
			if (string.IsNullOrEmpty(account))
			{
				error = "account is empty";
				return false;
			}
			if (Coding.GetDefauleCoding().GetByteCount(account) >
				MaximumAccountBytes)
			{
				error = "account exceeds the 15-byte game-server handoff field";
				return false;
			}
			if (ContainsControlCharacter(account))
			{
				error = "account contains a control character";
				return false;
			}
			if (string.IsNullOrEmpty(serverName))
			{
				error = "server name is empty";
				return false;
			}
			if (Coding.GetDefauleCoding().GetByteCount(serverName) >
				MaximumServerNameBytes)
			{
				error = "server name exceeds the 127-byte client field";
				return false;
			}
			if (ContainsControlCharacter(serverName))
			{
				error = "server name contains a control character";
				return false;
			}

			error = null;
			return true;
		}

		private static bool ContainsControlCharacter(string value)
		{
			for (int index = 0; index < value.Length; index++)
			{
				if (char.IsControl(value[index]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
