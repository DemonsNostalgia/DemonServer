using System;
using System.Collections.Generic;
using GameBase.Core;
using NetMsg;

namespace MapServer
{
	public class WorldPigeon
	{
		public const int BroadcastPrice = 5;
		public const byte MinimumBroadcastLevel = 50;
		public const int MaximumMessageBytes = 80;
		public const ushort BroadcastTalkAttribute = 2017;

		public static WorldPigeon Instance()
		{
			if (mInstance == null)
			{
				mInstance = new WorldPigeon();
			}
			return mInstance;
		}

		public WorldPigeon()
		{
			this.pendingMessages = new List<PigeonInfo>();
			this.sendTimer = new TimeOut();
			this.sendTimer.SetInterval(60);
		}

		public void Run()
		{
			if (this.pendingMessages.Count > 0 && this.sendTimer.ToNextTime())
			{
				this.Send(this.pendingMessages[0]);
				this.pendingMessages.RemoveAt(0);
			}
		}

		public int AddText(string name, uint roleId, string text)
		{
			for (int index = 0; index < this.pendingMessages.Count; index++)
			{
				if (this.pendingMessages[index].RoleId == roleId)
				{
					return -1;
				}
			}

			this.pendingMessages.Add(new PigeonInfo
			{
				Name = name,
				RoleId = roleId,
				Text = text
			});
			return this.pendingMessages.Count;
		}

		public int GetPendingCount()
		{
			return this.pendingMessages.Count;
		}

		public static bool TryValidateMessage(string text, out string error)
		{
			if (string.IsNullOrEmpty(text))
			{
				error = "Please input the message you wish to broadcast.";
				return false;
			}
			if (Coding.GetDefauleCoding().GetByteCount(text) > MaximumMessageBytes)
			{
				error = "Your message is too long to be broadcast.";
				return false;
			}
			for (int index = 0; index < text.Length; index++)
			{
				if (char.IsControl(text[index]))
				{
					error = "Broadcast messages cannot contain control characters.";
					return false;
				}
			}

			error = null;
			return true;
		}

		public static bool TryValidateClientMessage(
			uint roleId, string wireText, out string error)
		{
			string prefix = "<[" + roleId.ToString() + "]> ";
			if (string.IsNullOrEmpty(wireText) ||
				!wireText.StartsWith(prefix, StringComparison.Ordinal))
			{
				error = "The Broadcast sender prefix is invalid.";
				return false;
			}

			return TryValidateMessage(wireText.Substring(prefix.Length), out error);
		}

		public static byte[] CreateBroadcastPacket(string name, string text)
		{
			MsgTalkInfo message = new MsgTalkInfo
			{
				rgba = 0x00ffffff,
				unTxtAttribute = BroadcastTalkAttribute,
				tag = 0,
				param = 1419,
				param1 = -1,
				param2 = 0
			};
			message.liststr.Add(name);
			message.liststr.Add("ALLUSERS");
			message.liststr.Add("1241350");
			message.liststr.Add(text);
			return message.GetBuffer();
		}

		private void Send(PigeonInfo info)
		{
			UserEngine.Instance().BrocatBuffer(
				CreateBroadcastPacket(info.Name, info.Text));
		}

		private static WorldPigeon mInstance;
		private readonly List<PigeonInfo> pendingMessages;
		private readonly TimeOut sendTimer;
	}
}
