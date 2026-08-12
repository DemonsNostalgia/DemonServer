using System;
using System.Collections.Generic;
using GameBase.Network;
using GameStruct;

namespace MapServer
{
	public sealed class PlayerWardrobe
	{
		public const uint HairApplyShop = 1351U;
		public const uint HairUnlockShop = 1352U;
		public const ushort HairUnlockMarker = 0x0100;
		public const ushort HairApplyMarker = 0x0400;
		public const ushort HairUnlockAction = 202;
		public const ushort HairApplyAction = 203;
		public const ushort HairListAction = 206;
		public const uint HairOwnershipMessage = 30032629U;
		public const int HairCapacity = 40;
		public const ushort AvatarUnlockAction = 204;
		public const ushort AvatarApplyAction = 205;
		public const ushort AvatarListAction = 206;
		public const uint AvatarOwnershipMessage = 30032629U;
		public const uint AvatarOwnershipContext = 2U;
		public const int AvatarCapacity = 40;

		private readonly PlayerObject player;
		private readonly HashSet<uint> ownedHairStyles;
		private readonly HashSet<uint> ownedAvatarStyles;

		public PlayerWardrobe(PlayerObject player)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}
			this.player = player;
			this.ownedHairStyles = new HashSet<uint>();
			this.ownedAvatarStyles = new HashSet<uint>();
		}

		public void LoadOwned(IEnumerable<uint> hairStyles)
		{
			this.ownedHairStyles.Clear();
			if (hairStyles != null)
			{
				foreach (uint styleId in hairStyles)
				{
					if (this.ownedHairStyles.Count >= HairCapacity)
					{
						break;
					}
					if (ConfigManager.Instance().GetWardrobeHairInfo(
						styleId, this.player.GetSex()) != null)
					{
						this.ownedHairStyles.Add(styleId);
					}
				}
			}

			uint currentHair = this.player.GetBaseAttr().hair;
			if (currentHair != 0U &&
				this.ownedHairStyles.Count < HairCapacity &&
				ConfigManager.Instance().GetWardrobeHairInfo(
					currentHair, this.player.GetSex()) != null)
			{
				this.ownedHairStyles.Add(currentHair);
			}
		}

		public List<uint> GetOwnedHairStyles()
		{
			List<uint> styles = new List<uint>(this.ownedHairStyles);
			styles.Sort();
			return styles;
		}

		public void LoadOwnedAvatars(IEnumerable<uint> avatarStyles)
		{
			this.ownedAvatarStyles.Clear();
			if (avatarStyles == null)
			{
				return;
			}

			foreach (uint styleId in avatarStyles)
			{
				if (this.ownedAvatarStyles.Count >= AvatarCapacity)
				{
					break;
				}
				if (this.GetAvailableAvatarInfo(styleId) != null)
				{
					this.ownedAvatarStyles.Add(styleId);
				}
			}
		}

		public List<uint> GetOwnedAvatarStyles()
		{
			List<uint> styles = new List<uint>(this.ownedAvatarStyles);
			styles.Sort();
			return styles;
		}

		public void SendAllHairInfo()
		{
			foreach (uint styleId in this.GetOwnedHairStyles())
			{
				this.SendOwnershipUpdate(styleId);
			}
		}

		public void SendAllAvatarInfo()
		{
			foreach (uint styleId in this.GetOwnedAvatarStyles())
			{
				this.SendAvatarOwnershipUpdate(styleId);
			}
		}

		public void UnlockHair(uint styleId, ushort amount, ushort marker)
		{
			if (amount != 1 || marker != HairUnlockMarker)
			{
				this.Reject(HairUnlockAction, styleId,
					"Invalid wardrobe hair purchase request.");
				return;
			}
			this.UnlockHairWithEp(styleId, true);
		}

		public void UnlockHairFromAction(uint styleId)
		{
			this.UnlockHairWithEp(styleId, false);
		}

		private void UnlockHairWithEp(uint styleId, bool sendFailureAction)
		{

			WardrobeHairInfo info = ConfigManager.Instance()
				.GetWardrobeHairInfo(styleId, this.player.GetSex());
			if (info == null || info.purchasecurrency != 1 ||
				info.unlockprice <= 0)
			{
				this.Reject(HairUnlockAction, styleId,
					"This hairstyle cannot be purchased with EP.",
					sendFailureAction);
				return;
			}

			if (this.ownedHairStyles.Contains(styleId))
			{
				this.SendOwnershipUpdate(styleId);
				return;
			}

			if (this.ownedHairStyles.Count >= HairCapacity)
			{
				this.Reject(HairUnlockAction, styleId,
					"Your hairstyle wardrobe is full.",
					sendFailureAction);
				return;
			}

			if (this.player.GetMoneyCount(MONEYTYPE.GAMEGOLD) <
				info.unlockprice)
			{
				this.Reject(HairUnlockAction, styleId,
					"Not enough EP to unlock this hairstyle.",
					sendFailureAction);
				return;
			}

			this.SendOwnershipUpdate(styleId);
			this.ownedHairStyles.Add(styleId);
			this.player.ChangeMoney(MONEYTYPE.GAMEGOLD, -info.unlockprice);
			DBServer.Instance().SaveRoleData(this.player, false);
		}

		public void ApplyHair(uint styleId, ushort amount, ushort marker)
		{
			if (amount != 1 || marker != HairApplyMarker)
			{
				this.Reject(HairApplyAction, styleId,
					"Invalid wardrobe hairstyle request.");
				return;
			}
			this.ApplyOwnedHair(styleId, true, true);
		}

		public void ApplyHairFromAction(Action2Packet request)
		{
			if (request == null || request.Action != HairApplyAction)
			{
				return;
			}
			this.ApplyOwnedHair(request.ValueAt24, false, false, request);
		}

		public bool ApplyHairFromOwnershipAction(Action2Packet request)
		{
			if (request == null || request.Action != HairListAction ||
				request.ContextAt12 != 1U)
			{
				return false;
			}

			uint styleId = request.ValueAt24;
			if (!this.ownedHairStyles.Contains(styleId) ||
				ConfigManager.Instance().GetWardrobeHairInfo(
					styleId, this.player.GetSex()) == null)
			{
				return false;
			}

			this.player.ChangeAttribute(UserAttribute.HAIR,
				unchecked((int)styleId), true);
			DBServer.Instance().SaveRoleData(this.player, false);
			return true;
		}

		private void ApplyOwnedHair(
			uint styleId,
			bool chargeChangePrice,
			bool sendFailureAction,
			Action2Packet request = null)
		{

			WardrobeHairInfo info = ConfigManager.Instance()
				.GetWardrobeHairInfo(styleId, this.player.GetSex());
			if (info == null)
			{
				this.Reject(HairApplyAction, styleId,
					"This hairstyle is not valid for your character.",
					sendFailureAction);
				return;
			}

			if (!this.ownedHairStyles.Contains(styleId))
			{
				this.Reject(HairApplyAction, styleId,
					"You have not unlocked this hairstyle.",
					sendFailureAction);
				return;
			}

			if (chargeChangePrice && (info.changeprice < 0 ||
				this.player.GetMoneyCount(MONEYTYPE.GOLD) < info.changeprice)
				)
			{
				this.Reject(HairApplyAction, styleId,
					"Not enough gold to apply this hairstyle.",
					sendFailureAction);
				return;
			}

			if (chargeChangePrice && info.changeprice > 0)
			{
				this.player.ChangeMoney(MONEYTYPE.GOLD, -info.changeprice);
			}
			this.player.ChangeAttribute(UserAttribute.HAIR,
				unchecked((int)styleId), true);
			if (request == null)
			{
				this.SendAction(HairApplyAction, 0U, 1, styleId);
			}
			else
			{
				this.SendApplyResponse(request);
			}
			DBServer.Instance().SaveRoleData(this.player, false);
		}

		public bool PurchaseHairWithGold(HairInfo hairInfo)
		{
			if (hairInfo == null || hairInfo.hairid <= 0 ||
				hairInfo.price < 0)
			{
				return false;
			}
			if (this.player.GetSex() != hairInfo.sex)
			{
				this.player.LeftNotice(
					"Gender does not match, cannot purchase!");
				return false;
			}

			uint styleId = unchecked((uint)hairInfo.hairid);
			WardrobeHairInfo wardrobeInfo = ConfigManager.Instance()
				.GetWardrobeHairInfo(styleId, this.player.GetSex());
			if (wardrobeInfo == null)
			{
				this.player.LeftNotice(
					"This hairstyle is not valid for your character.");
				return false;
			}

			bool alreadyOwned = this.ownedHairStyles.Contains(styleId);
			if (!alreadyOwned && this.ownedHairStyles.Count >= HairCapacity)
			{
				this.player.LeftNotice("Your hairstyle wardrobe is full.");
				return false;
			}
			if (!alreadyOwned &&
				this.player.GetMoneyCount(MONEYTYPE.GOLD) < hairInfo.price)
			{
				this.player.LeftNotice(
					"Not enough gold coins, cannot purchase!");
				return false;
			}

			if (!alreadyOwned)
			{
				this.player.ChangeMoney(MONEYTYPE.GOLD, -hairInfo.price);
				this.ownedHairStyles.Add(styleId);
			}
			this.SendOwnershipUpdate(styleId);
			this.player.ChangeAttribute(UserAttribute.HAIR,
				unchecked((int)styleId), true);
			DBServer.Instance().SaveRoleData(this.player, false);
			return true;
		}

		public void UnlockAvatarFromAction(uint styleId)
		{
			WardrobeAvatarInfo info = this.GetAvailableAvatarInfo(styleId);
			if (info == null || info.purchasecurrency != 1 ||
				info.unlockprice <= 0)
			{
				this.Reject(AvatarUnlockAction, styleId,
					"This avatar cannot be purchased with EP.", false);
				return;
			}

			if (this.ownedAvatarStyles.Contains(styleId))
			{
				this.SendAvatarOwnershipUpdate(styleId);
				return;
			}

			if (this.ownedAvatarStyles.Count >= AvatarCapacity)
			{
				this.Reject(AvatarUnlockAction, styleId,
					"Your avatar wardrobe is full.", false);
				return;
			}

			if (this.player.GetMoneyCount(MONEYTYPE.GAMEGOLD) <
				info.unlockprice)
			{
				this.Reject(AvatarUnlockAction, styleId,
					"Not enough EP to unlock this avatar.", false);
				return;
			}

			this.SendAvatarOwnershipUpdate(styleId);
			this.ownedAvatarStyles.Add(styleId);
			this.player.ChangeMoney(MONEYTYPE.GAMEGOLD, -info.unlockprice);
			DBServer.Instance().SaveRoleData(this.player, false);
		}

		public void ApplyAvatarFromAction(Action2Packet request)
		{
			if (request == null || request.Action != AvatarApplyAction)
			{
				return;
			}

			this.ApplyOwnedAvatar(request.ValueAt24, request);
		}

		public bool ApplyAvatarFromOwnershipAction(Action2Packet request)
		{
			if (request == null || request.Action != AvatarListAction ||
				request.ContextAt12 != AvatarOwnershipContext)
			{
				return false;
			}

			return this.ApplyOwnedAvatar(request.ValueAt24, null);
		}

		private bool ApplyOwnedAvatar(uint styleId, Action2Packet request)
		{
			if (!this.ownedAvatarStyles.Contains(styleId) ||
				this.GetAvailableAvatarInfo(styleId) == null)
			{
				if (request != null)
				{
					this.Reject(AvatarApplyAction, styleId,
						"You have not unlocked this avatar.");
				}
				return false;
			}

			uint lookFace = this.BuildLookFace(styleId);
			this.player.ChangeAttribute(UserAttribute.LOOKFACE,
				unchecked((int)lookFace), true);
			if (request != null)
			{
				this.SendApplyResponse(request);
			}
			DBServer.Instance().SaveRoleData(this.player, false);
			return true;
		}

		private WardrobeAvatarInfo GetAvailableAvatarInfo(uint styleId)
		{
			WardrobeAvatarInfo info = ConfigManager.Instance()
				.GetWardrobeAvatarInfo(styleId, this.player.GetSex());
			if (info == null || (info.job != 0 &&
				info.job != this.player.GetJob()))
			{
				return null;
			}
			return info;
		}

		private uint BuildLookFace(uint styleId)
		{
			uint sexDigit = this.player.GetBaseAttr().lookface % 10U;
			if (sexDigit != 1U && sexDigit != 2U)
			{
				sexDigit = this.player.GetSex();
			}
			return styleId * 10000U + sexDigit;
		}

		private void Reject(
			ushort action,
			uint styleId,
			string notice,
			bool sendFailureAction = true)
		{
			if (sendFailureAction)
			{
				this.SendAction(action, 0U, 0, styleId);
			}
			this.player.LeftNotice(notice);
		}

		private void SendOwnershipUpdate(uint styleId)
		{
			byte[] packet = MapPacketCodec.CreateAction2Response(
				this.player.GetGamePackKeyEx(),
				HairOwnershipMessage,
				0U,
				1U,
				0,
				0,
				0,
				HairListAction,
				styleId,
				0U);
			this.player.SendData(packet, false);
		}

		private void SendAvatarOwnershipUpdate(uint styleId)
		{
			byte[] packet = MapPacketCodec.CreateAction2Response(
				this.player.GetGamePackKeyEx(),
				AvatarOwnershipMessage,
				0U,
				AvatarOwnershipContext,
				0,
				0,
				0,
				AvatarListAction,
				styleId,
				0U);
			this.player.SendData(packet, false);
		}

		private void SendApplyResponse(Action2Packet request)
		{
			byte[] packet = MapPacketCodec.CreateAction2Response(
				this.player.GetGamePackKeyEx(),
				request.Timestamp,
				request.ValueAt8,
				request.ContextAt12,
				request.ValueAt16,
				1,
				request.ValueAt20,
				request.Action,
				request.ValueAt24,
				request.ReservedAt28);
			this.player.SendData(packet, false);
		}

		private void SendAction(
			ushort action,
			uint context,
			ushort success,
			uint styleId)
		{
			byte[] packet = MapPacketCodec.CreateAction2Response(
				this.player.GetGamePackKeyEx(),
				unchecked((uint)Environment.TickCount),
				0U,
				context,
				0,
				success,
				0,
				action,
				styleId,
				0U);
			this.player.SendData(packet, false);
		}
	}
}
