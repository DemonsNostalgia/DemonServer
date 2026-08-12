using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;

namespace AccServer
{
	public class SocketCallBack
	{
		public static SocketCallBack Instance()
		{
			if (SocketCallBack.mInstance == null)
			{
				SocketCallBack.mInstance = new SocketCallBack();
			}
			return SocketCallBack.mInstance;
		}

		public SocketCallBack()
		{
			this.mList = new List<SocketInfo>();
			this.mAccountMetadata = new Dictionary<Socket, string>();
		}

		public void AddData(SocketInfo info)
		{
			lock (this.mList)
			{
				this.mList.Add(info);
			}
		}

		public SocketInfo GetInfo()
		{
			SocketInfo result = null;
			lock (this.mList)
			{
				if (this.mList.Count > 0)
				{
					result = this.mList[0];
					this.mList.RemoveAt(0);
				}
			}
			return result;
		}

		public void Run()
		{
			int tickCount = Environment.TickCount;
			while (Environment.TickCount - tickCount <= 300)
			{
				SocketInfo info = this.GetInfo();
				if (info == null)
				{
					return;
				}
				if (info.s == null)
				{
					continue;
				}
				if (info.type == TYPE_CLOSE)
				{
					this.mAccountMetadata.Remove(info.s);
					continue;
				}
				if (info.type != TYPE_RECEIVE)
				{
					continue;
				}

				GameSession session = info.session;
				if (session == null)
				{
					Log.Instance().WriteLog("received data for an unknown client session");
					continue;
				}

				byte[] received = new byte[info.data.Length];
				Buffer.BlockCopy(info.data, 0, received, 0, info.data.Length);
				session.m_GamePack.ProcessNetData(received);

				byte[] data;
				while ((data = session.m_GamePack.GetData()) != null)
				{
					PackIn packet = new PackIn(data);
					ushort packetType = packet.ReadUInt16();
					#if DEBUG
					Log.Instance().WriteLog("decoded packet type " + packetType +
						", payload bytes " + data.Length);
					#endif

					if (packetType == 1083)
					{
						AccountMetadataPacket metadata;
						string error;
						if (!LoginPacketCodec.TryReadAccountMetadata(
							data, out metadata, out error))
						{
							Log.Instance().WriteLog(
								"rejected malformed account metadata: " + error);
							continue;
						}
						this.mAccountMetadata[info.s] = metadata.Account;
						Log.Instance().WriteLog("received account-stage metadata: account=" +
							metadata.Account + ", endpoint=" + metadata.AdvertisedIp + ":" +
							metadata.AdvertisedPort);
						continue;
					}

					if (packetType != 1120)
					{
						Log.Instance().WriteLog("unhandled packet type " + packetType);
						continue;
					}

					DirectGameLoginPacket login;
					string parseError;
					if (!LoginPacketCodec.TryReadDirectGameLogin(
						data, out login, out parseError))
					{
						Log.Instance().WriteLog(
							"rejected malformed direct login: " + parseError);
						SendLoginResult(session, info.s, 1, 0);
						continue;
					}

					string metadataAccount;
					if (this.mAccountMetadata.TryGetValue(
							info.s, out metadataAccount) &&
						!string.Equals(
							metadataAccount, login.Account,
							StringComparison.Ordinal))
					{
						Log.Instance().WriteLog(
							"rejected direct login because account metadata did not match the credentials packet");
						SendLoginResult(session, info.s, 1, 0);
						continue;
					}

					string validationError;
					if (!LoginRequestValidator.TryValidateDirectLogin(
						login.Account,
						login.Password,
						login.ServerName,
						out validationError))
					{
						Log.Instance().WriteLog(
							"rejected invalid direct login request: " +
							validationError);
						SendLoginResult(session, info.s, 1, 0);
						this.mAccountMetadata.Remove(info.s);
						continue;
					}

					int accountId = 0;
					bool authenticated = false;
					try
					{
						authenticated = LoginDatabase.TryAuthenticateAndIssueTicket(
							login.Account,
							login.Password,
							login.ServerName,
							out accountId);
					}
					catch (Exception ex)
					{
						Log.Instance().WriteLog("MySQL authentication error for account " +
							login.Account + ": " + ex.Message);
					}

					SendLoginResult(session, info.s, authenticated ? 0 : 1,
						authenticated ? accountId : 0);
					this.mAccountMetadata.Remove(info.s);
					Log.Instance().WriteLog((authenticated
						? "database authentication succeeded"
						: "database authentication denied") + ": account=" +
						login.Account + ", id=" + accountId + ", server=" +
						login.ServerName + ", mode=" + login.Mode);
				}
			}
		}

		private static void SendLoginResult(
			GameSession session,
			Socket socket,
			int status,
			int accountId)
		{
			byte[] response = LoginPacketCodec.CreateAccountResult(
				session.GetGamePackKeyEx(), status, accountId);
			Program.server.SendData(socket, response);
			Log.Instance().WriteLog("sent account login result 1083: status=" +
				status + ", accountId=" + accountId + ", bytes=" + response.Length);
		}

		public const byte TYPE_ONCONNECT = 0;
		public const byte TYPE_RECEIVE = 2;
		public const byte TYPE_CLOSE = 3;

		private static SocketCallBack mInstance;
		private readonly List<SocketInfo> mList;
		private readonly Dictionary<Socket, string> mAccountMetadata;
	}
}
