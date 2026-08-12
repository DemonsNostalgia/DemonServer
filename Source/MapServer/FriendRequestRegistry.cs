using System;
using System.Collections.Generic;

namespace MapServer
{
	public sealed class FriendRequestRegistry
	{
		public FriendRequestRegistry()
			: this(TimeSpan.FromMinutes(2.0))
		{
		}

		public FriendRequestRegistry(TimeSpan lifetime)
		{
			if (lifetime <= TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("lifetime");
			}
			this.lifetime = lifetime;
			this.pending = new Dictionary<ulong, DateTime>();
		}

		public void Register(uint requesterId, uint recipientId)
		{
			if (requesterId == 0U || recipientId == 0U ||
				requesterId == recipientId)
			{
				return;
			}
			lock (this.syncRoot)
			{
				this.RemoveExpired(DateTime.UtcNow);
				this.pending[MakeKey(requesterId, recipientId)] =
					DateTime.UtcNow.Add(this.lifetime);
			}
		}

		public bool Contains(uint requesterId, uint recipientId)
		{
			lock (this.syncRoot)
			{
				this.RemoveExpired(DateTime.UtcNow);
				return this.pending.ContainsKey(
					MakeKey(requesterId, recipientId));
			}
		}

		public bool Consume(uint requesterId, uint recipientId)
		{
			lock (this.syncRoot)
			{
				this.RemoveExpired(DateTime.UtcNow);
				return this.pending.Remove(
					MakeKey(requesterId, recipientId));
			}
		}

		public void RemoveForPlayer(uint playerId)
		{
			lock (this.syncRoot)
			{
				List<ulong> remove = new List<ulong>();
				foreach (ulong key in this.pending.Keys)
				{
					if ((uint)(key >> 32) == playerId ||
						(uint)key == playerId)
					{
						remove.Add(key);
					}
				}
				for (int i = 0; i < remove.Count; i++)
				{
					this.pending.Remove(remove[i]);
				}
			}
		}

		public int Count
		{
			get
			{
				lock (this.syncRoot)
				{
					this.RemoveExpired(DateTime.UtcNow);
					return this.pending.Count;
				}
			}
		}

		private static ulong MakeKey(uint requesterId, uint recipientId)
		{
			return ((ulong)requesterId << 32) | recipientId;
		}

		private void RemoveExpired(DateTime now)
		{
			List<ulong> remove = null;
			foreach (KeyValuePair<ulong, DateTime> item in this.pending)
			{
				if (item.Value > now)
				{
					continue;
				}
				if (remove == null)
				{
					remove = new List<ulong>();
				}
				remove.Add(item.Key);
			}
			if (remove == null)
			{
				return;
			}
			for (int i = 0; i < remove.Count; i++)
			{
				this.pending.Remove(remove[i]);
			}
		}

		private readonly object syncRoot = new object();

		private readonly TimeSpan lifetime;

		private readonly Dictionary<ulong, DateTime> pending;
	}
}
