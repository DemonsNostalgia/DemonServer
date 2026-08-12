using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	public sealed class EudemonHatchManager
	{
		public const byte PackageType = 70;
		public const int IncubatorCapacity = 3;
		public const int EggBagCapacity = 6;
		// The definitive client's exit warning and the preserved server's
		// observed incubation schedule both use a four-hour hatch. The earlier
		// twelve-hour value left every live countdown eight hours too long.
		public const int HatchDurationSeconds = 4 * 60 * 60;
		public const uint EudemonItemTypeFamily = 107;
		public const uint EggItemTypeFamily = 108;
		public const uint ItemTypeFamilyDivisor = 10000;
		public const uint EggItemTypeOffset = 10000;

		public static bool Handle(
			PlayerObject player,
			EudemonPackagePacket request)
		{
			if (request.PackageType != PackageType)
			{
				return false;
			}

			if (request.PackageId != player.GetTypeId() ||
				request.Context != 0 || request.OperationValue != 0)
			{
				Log.Instance().WriteLog(
					"Rejected invalid incubator packet 1117 envelope for role " +
					player.GetTypeId().ToString() + ".");
				return true;
			}

			switch (request.Operation)
			{
			case 0:
				SendPackage(player, 0, GetIncubatingItems(player));
				return true;
			case 1:
				CheckIn(player, request.OperationItemId);
				return true;
			case 2:
				CheckOut(player, request.OperationItemId);
				return true;
			default:
				Log.Instance().WriteLog(
					"Rejected unsupported incubator packet 1117 operation " +
					request.Operation.ToString() + ".");
				return true;
			}
		}

		public static bool TryMoveEggToEggBagFromInventoryUse(
			PlayerObject player,
			RoleItemInfo item)
		{
			if (item == null ||
				item.postion != MsgItemInfo.ITEMPOSITION_BACKPACK ||
				!IsHatchableEudemonEgg(item))
			{
				return false;
			}
			if (!CanAcceptQueuedEggs(player, 1))
			{
				player.LeftNotice("Your egg bag is full.");
				return true;
			}

			MoveToEggBag(player, item);
			return true;
		}

		public static bool TryAwardQueuedEggs(
			PlayerObject player,
			uint eudemonItemTypeId,
			int count)
		{
			uint eggItemTypeId;
			if (player == null || count <= 0 ||
				!TryGetEggItemTypeId(
					eudemonItemTypeId, out eggItemTypeId) ||
				!IsHatchableEudemonEggType(eggItemTypeId))
			{
				Log.Instance().WriteLog(
					"Rejected invalid queued eudemon egg award. Item type: " +
					eudemonItemTypeId.ToString() + " Count: " + count.ToString() + ".");
				return false;
			}
			if (!CanAcceptQueuedEggs(player, count))
			{
				player.LeftNotice("Your egg bag does not have enough free slots.");
				return false;
			}

			for (int index = 0; index < count; index++)
			{
				RoleItemInfo queuedEgg = CreateQueuedEggItem(eggItemTypeId);
				RoleItemInfo awardedEgg =
					player.GetItemSystem().AwardItem(queuedEgg);
				if (awardedEgg == null)
				{
					Log.Instance().WriteLog(
						"Failed to add eudemon egg item " +
							eggItemTypeId.ToString() + " for role " +
							player.GetTypeId().ToString() + ".");
					return false;
				}
				Log.Instance().WriteLog(
					"Added queued eudemon egg item " + awardedEgg.id.ToString() +
					" to the egg bag for role " +
					player.GetTypeId().ToString() + ".");
			}
			return true;
		}

		public static bool IsItemTriggeredEggPackage(
			uint actionId,
			RoleItemInfo sourceItem,
			ItemTypeInfo sourceItemType)
		{
			return sourceItem != null && sourceItemType != null &&
				sourceItem.postion == MsgItemInfo.ITEMPOSITION_BACKPACK &&
				sourceItem.itemid == actionId &&
				sourceItemType.actionid == actionId;
		}

		public static bool CanCreateQueuedEggFromEudemonType(
			uint eudemonItemTypeId)
		{
			uint eggItemTypeId;
			return TryGetEggItemTypeId(
				eudemonItemTypeId, out eggItemTypeId) &&
				IsHatchableEudemonEggType(eggItemTypeId);
		}

		public static RoleItemInfo CreateQueuedEggItem(uint eggItemTypeId)
		{
			return new RoleItemInfo
			{
				itemid = eggItemTypeId,
				postion = MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK,
				amount = 1,
				property = 0
			};
		}

		public static bool IsIncubatingEgg(RoleItemInfo item)
		{
			return item != null &&
				item.postion == MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK &&
				item.property > 0;
		}

		public static bool ShouldSendAsRegularItem(RoleItemInfo item)
		{
			return item == null ||
				item.postion != MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK ||
				item.property <= 0;
		}

		public static bool IsQueuedEgg(RoleItemInfo item)
		{
			return item != null &&
				item.postion == MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK &&
				item.property == 0;
		}

		private static bool CanAcceptQueuedEggs(
			PlayerObject player,
			int incomingCount)
		{
			if (player == null || incomingCount <= 0)
			{
				return false;
			}

			int queuedCount = 0;
			foreach (RoleItemInfo item in
				player.GetItemSystem().GetDicItem().Values)
			{
				if (IsQueuedEgg(item))
				{
					queuedCount++;
				}
			}
			return queuedCount <= EggBagCapacity - incomingCount;
		}

		private static void MoveToEggBag(
			PlayerObject player,
			RoleItemInfo item)
		{
			item.postion = MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK;
			item.property = 0;
			player.GetItemSystem().SendItemInfo(item);
			player.GetItemSystem().DB_Save();
			Log.Instance().WriteLog(
				"Moved eudemon egg item " + item.id.ToString() +
				" from inventory to the egg bag for role " +
				player.GetTypeId().ToString() + ".");
		}

		private static void CheckIn(PlayerObject player, uint itemId)
		{
			RoleItemInfo item = player.GetItemSystem().FindItem(itemId);
			if (item == null ||
				item.postion != MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK ||
				item.property != 0)
			{
				player.LeftNotice("That egg is not waiting in your egg bag.");
				return;
			}
			if (!IsHatchableEudemonEgg(item))
			{
				player.LeftNotice("That item is not a hatchable eudemon egg.");
				return;
			}

			List<RoleItemInfo> incubatingItems = GetIncubatingItems(player);
			if (incubatingItems.Count >= IncubatorCapacity)
			{
				player.LeftNotice("All three incubator slots are occupied.");
				return;
			}

			int currentTime = GetUnixTime();
			item.property = checked(currentTime + HatchDurationSeconds);
			player.GetItemSystem().ClearItem(item.id);
			player.GetItemSystem().DB_Save();

			List<RoleItemInfo> responseItems = new List<RoleItemInfo>();
			responseItems.Add(item);
			SendPackage(player, 1, responseItems);
			player.LeftNotice(
				"Your eudemon will be born after 4 hours of hatching.");
			Log.Instance().WriteLog(
				"Placed eudemon egg item " + item.id.ToString() +
				" into the incubator for role " + player.GetTypeId().ToString() +
				" with finish time " + item.property.ToString() + ".");
		}

		private static void CheckOut(PlayerObject player, uint itemId)
		{
			RoleItemInfo item = player.GetItemSystem().FindItem(itemId);
			if (item == null ||
				item.postion != MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK ||
				item.property <= 0)
			{
				player.LeftNotice("That egg is not in your incubator.");
				return;
			}
			if (!IsHatchableEudemonEgg(item))
			{
				player.LeftNotice("That incubator item is not a eudemon egg.");
				return;
			}

			uint eudemonItemTypeId;
			if (!TryGetEudemonItemTypeId(
				item.itemid, out eudemonItemTypeId))
			{
				player.LeftNotice("That incubator item has no matching eudemon.");
				return;
			}

			uint remainingSeconds = MapPacketCodec.GetRemainingHatchSeconds(
				item.property, GetUnixTime());
			if (remainingSeconds != 0)
			{
				if (!CanAcceptQueuedEggs(player, 1))
				{
					player.LeftNotice("Your egg bag is full.");
					return;
				}
				// The definitive client uses operation 2 for both actions available
				// from the brood package. An active egg is taken back to the normal
				// egg package, while an expired egg is collected as a eudemon.
				// Clearing the absolute finish time makes a later operation-1
				// check-in start the full hatch duration again.
				item.property = 0;
				byte[] activeRemoval =
					MapPacketCodec.CreateEudemonPackageRemovalResponse(
						null,
						player.GetTypeId(),
						0,
						PackageType,
						item.id);
				player.SendData(activeRemoval, true);
				player.GetItemSystem().SendItemInfo(item);
				player.GetItemSystem().DB_Save();
				Log.Instance().WriteLog(
					"Returned active eudemon egg item " + item.id.ToString() +
					" from the incubator to the egg bag for role " +
					player.GetTypeId().ToString() +
					" and cleared its hatch timer.");
				return;
			}
			if (!player.GetItemSystem().CanAcceptAtPosition(
				MsgItemInfo.ITEMPOSITION_EUDEMON_PACK))
			{
				player.LeftNotice("Your eudemon bag is full.");
				return;
			}

			item.itemid = eudemonItemTypeId;
			item.postion = MsgItemInfo.ITEMPOSITION_EUDEMON_PACK;
			item.property = 0;
			item.typeid = IDManager.CreateTypeId(4);

			byte[] removalResponse =
				MapPacketCodec.CreateEudemonPackageRemovalResponse(
					null,
					player.GetTypeId(),
					0,
					PackageType,
					item.id);
			player.SendData(removalResponse, true);
			player.GetItemSystem().SendItemInfo(item);
			player.GetEudemonSystem().AddEudemon(item);
			player.GetItemSystem().DB_Save();
			player.GetEudemonSystem().DB_Save();
			Log.Instance().WriteLog(
				"Hatched eudemon egg item " + item.id.ToString() +
				" for role " + player.GetTypeId().ToString() + ".");
		}

		private static void SendPackage(
			PlayerObject player,
			byte operation,
			List<RoleItemInfo> sourceItems)
		{
			int currentTime = GetUnixTime();
			List<EudemonPackageItem> records =
				new List<EudemonPackageItem>(sourceItems.Count);
			for (int index = 0; index < sourceItems.Count; index++)
			{
				RoleItemInfo item = sourceItems[index];
				ItemTypeInfo itemType =
					ConfigManager.Instance().GetItemTypeInfo(item.itemid);
				records.Add(new EudemonPackageItem
				{
					ItemId = item.id,
					ItemTypeId = item.itemid,
					RemainingSeconds = MapPacketCodec.GetRemainingHatchSeconds(
						item.property, currentTime),
					Name = itemType == null ? item.forgename : itemType.name
				});
			}

			byte[] response = MapPacketCodec.CreateEudemonPackageListResponse(
				null,
				player.GetTypeId(),
				0,
				operation,
				PackageType,
				0,
				records);
			player.SendData(response, true);
		}

		private static List<RoleItemInfo> GetIncubatingItems(
			PlayerObject player)
		{
			List<RoleItemInfo> items = new List<RoleItemInfo>();
			bool correctedLegacyItem = false;
			foreach (RoleItemInfo item in
				player.GetItemSystem().GetDicItem().Values)
			{
				if (item.postion == MsgItemInfo.ITEMPOSITION_EUDEMONEGG_PACK &&
					item.property > 0)
				{
					uint eggItemTypeId;
					if (TryGetEggItemTypeId(item.itemid, out eggItemTypeId) &&
						IsHatchableEudemonEggType(eggItemTypeId))
					{
						Log.Instance().WriteLog(
							"Corrected legacy incubator item " + item.id.ToString() +
							" from eudemon type " + item.itemid.ToString() +
							" to egg type " + eggItemTypeId.ToString() + ".");
						item.itemid = eggItemTypeId;
						correctedLegacyItem = true;
					}
					items.Add(item);
				}
			}
			if (correctedLegacyItem)
			{
				player.GetItemSystem().DB_Save();
			}
			items.Sort(delegate(RoleItemInfo left, RoleItemInfo right)
			{
				return left.id.CompareTo(right.id);
			});
			return items;
		}

		private static bool IsHatchableEudemonEgg(RoleItemInfo item)
		{
			return item != null && IsHatchableEudemonEggType(item.itemid);
		}

		public static bool IsHatchableEudemonEggType(uint itemTypeId)
		{
			uint eudemonItemTypeId;
			if (!TryGetEudemonItemTypeId(
				itemTypeId, out eudemonItemTypeId))
			{
				return false;
			}

			ItemTypeInfo eggItemType =
				ConfigManager.Instance().GetItemTypeInfo(itemTypeId);
			ItemTypeInfo eudemonItemType =
				ConfigManager.Instance().GetItemTypeInfo(eudemonItemTypeId);
			if (eggItemType == null || eudemonItemType == null ||
				eudemonItemType.monster_type == 0 ||
				!eggItemType.client_monopoly_known ||
				eggItemType.IsClientMonopolyItem())
			{
				return false;
			}

			uint baseItemId =
				eudemonItemTypeId - (eudemonItemTypeId % 10U);
			return ConfigManager.Instance().GetEudemonInfo(baseItemId) != null;
		}

		public static bool TryGetEggItemTypeId(
			uint eudemonItemTypeId,
			out uint eggItemTypeId)
		{
			eggItemTypeId = 0;
			if (eudemonItemTypeId / ItemTypeFamilyDivisor !=
				EudemonItemTypeFamily)
			{
				return false;
			}

			eggItemTypeId = checked(eudemonItemTypeId + EggItemTypeOffset);
			return true;
		}

		public static bool TryGetEudemonItemTypeId(
			uint eggItemTypeId,
			out uint eudemonItemTypeId)
		{
			eudemonItemTypeId = 0;
			if (eggItemTypeId / ItemTypeFamilyDivisor != EggItemTypeFamily)
			{
				return false;
			}

			eudemonItemTypeId = eggItemTypeId - EggItemTypeOffset;
			return true;
		}

		private static int GetUnixTime()
		{
			return checked((int)(DateTime.UtcNow -
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).
				TotalSeconds);
		}
	}
}
