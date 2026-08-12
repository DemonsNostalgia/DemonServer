using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	public sealed class BatchHatchManager
	{
		public const byte PackageType = 120;
		public const uint PackageId = 1;
		// The physical button stores this package owner before it sends the
		// operation-10 availability query. The value becomes the package ID when
		// the native Batch Hatcher subsequently queries its contents.
		public const uint BatchPackageId = 99999;
		public const int Capacity = 40;
		public const int HatchDurationSeconds = 3 * 60 * 60;
		// Definitive CItem::IsSpellEudScore calls the ItemCategory lookup named
		// SpellEudScore. A hash-gated runtime query proves type 810002 belongs to
		// that category while equipment-identification scroll 810004 does not.
		public const uint AppraisalScrollItemTypeId = 810002;
		public const ushort ItemPosition =
			MsgItemInfo.ITEMPOSITION_BATCH_HATCHER;
		// Definitive 6685 CMsgAction::Process case 0x266c clamps the packet value
		// to 40 and posts native main-window command 0x588. That command is the
		// sole source-backed Batch Hatcher window opener.
		public const int OpenWindowAction = 0x266c;

		public static bool Handle(
			PlayerObject player,
			EudemonPackagePacket request)
		{
			if (request.PackageType != PackageType)
			{
				return false;
			}

			if (request.Operation == 10 &&
				request.PackageId == PackageId &&
				request.Context == 0 &&
				request.OperationValue == 0 &&
				request.EntryCount == 1)
			{
				SendOpenWindowAction(player);
				return true;
			}

			// Definitive 6685 CHero::QueryBatchHatchMainPet creates operation 9
			// with package ID 1 and writes the selected runtime Eudemon identity
			// into the record/item field at wire offset 18. A type-120 operation-9
			// reply posts client command 0x59d. Before that acknowledgement, packet
			// 1008 action 9 must populate CHero::BatchHatchMainPet. The definitive
			// CMsgItemInfo::Process action-9 branch creates or updates that CItem and
			// calls CHero::SetBatchHatchMainPet.
			if (request.Operation == 9 &&
				request.PackageId == PackageId &&
				request.Context == 0 &&
				request.OperationValue == 0)
			{
				SelectMainPet(player, request.OperationItemId);
				return true;
			}

			// Definitive 6685 CHero::QueryBatchHatchEudScore sends operation 4
			// with tag 12 for the selected reference pet and tag 13 for each
			// completed pet in type-120 storage. CMsgEudemonInfo::Process consumes
			// packet 2037 with that tag, copies its authoritative attributes into
			// the Batch Hatcher item and marks that client item appraised.
			if (request.Operation == 4)
			{
				SendAppraisalInformation(player, request);
				return true;
			}

			// CHero::SendChangeEgg emits type-120 operation 6 with value 1 and
			// the persistent Eudemon identity at wire offset 18. The matching
			// response value is a Boolean result consumed by client command 0x589.
			if (request.Operation == 6)
			{
				ChangeEgg(player, request);
				return true;
			}

			// CHero::SendBatchAppraisal sends operation 11 before the operation-4
			// attribute queries. Its value is the number of completed Eudemons the
			// client is about to appraise.
			if (request.Operation == 11)
			{
				AuthorizeAppraisals(player, request);
				return true;
			}

			if (request.PackageId != BatchPackageId ||
				request.Context != 0 || request.OperationValue != 0)
			{
				LogRejectedRequest(request, "invalid package envelope");
				return true;
			}

			switch (request.Operation)
			{
			case 0:
				SendPackage(player, 0, GetBatchItems(player));
				return true;
			case 1:
				CheckIn(player, request.OperationItemId);
				return true;
			case 2:
				CheckOut(player, request.OperationItemId);
				return true;
			case 8:
				SendFreeAppraisalStatus(player);
				return true;
			default:
				LogRejectedRequest(request, "unsupported operation");
				return true;
			}
		}

		public static EudemonPackageItem CreatePackageItem(
			RoleItemInfo item,
			RoleData_Eudemon eudemon,
			int currentTime)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			ItemTypeInfo itemType =
				ConfigManager.Instance().GetItemTypeInfo(item.itemid);
			EudemonPackageItem record = new EudemonPackageItem
			{
				ItemId = eudemon == null ? item.id : eudemon.GetTypeID(),
				ItemTypeId = item.itemid,
				RemainingSeconds = MapPacketCodec.GetRemainingHatchSeconds(
					item.property, currentTime),
				Name = eudemon == null
					? (itemType == null ? item.forgename : itemType.name)
					: eudemon.name
			};

			if (eudemon == null)
			{
				return record;
			}

			record.PhysicalAttackMinimum = ToUInt32(eudemon.atk_min);
			record.PhysicalAttackMaximum = ToUInt32(eudemon.atk_max);
			record.MagicAttackMinimum = ToUInt32(eudemon.magicatk_min);
			record.MagicAttackMaximum = ToUInt32(eudemon.magicatk_max);
			record.Defense = ToUInt32(eudemon.defense);
			record.MagicDefense = ToUInt32(eudemon.magicdef);
			record.Life = ToUInt32(eudemon.life);
			record.MaximumLife = ToUInt32(eudemon.life_max);
			record.Fidelity = ToUInt16(eudemon.intimacy);
			record.RebornTimes = ToUInt16(eudemon.recall_count);
			record.Experience = ToUInt64(eudemon.exp);
			record.Level = ToByte(eudemon.level);
			record.Status = eudemon.bDie ? (byte)1 : (byte)0;
			record.RespawnAvailable = eudemon.bDie ? (byte)0 : (byte)1;
			record.Luck = ToUInt16(eudemon.luck);
			record.InitialLife = ToUInt16(eudemon.init_life);
			record.InitialDefense = ToUInt16(eudemon.GetInitDefense());
			record.InitialPhysicalAttack = ToUInt16(eudemon.GetInitAtk());
			record.InitialMagicAttack = ToUInt16(eudemon.GetInitMagicAtk());
			record.IdentityCard = ToUInt32(eudemon.card);
			record.Quality = eudemon.quality;
			return record;
		}

		private static void CheckIn(PlayerObject player, uint itemId)
		{
			RoleItemInfo item = player.GetItemSystem().FindItem(itemId);
			if (!EudemonHatchManager.IsQueuedEgg(item))
			{
				player.LeftNotice("That egg is not waiting in your egg bag.");
				return;
			}
			if (!EudemonHatchManager.IsHatchableEudemonEggType(item.itemid))
			{
				player.LeftNotice(
					"Only hatchable eudemon eggs can enter the Batch Hatcher.");
				return;
			}

			List<RoleItemInfo> batchItems = GetBatchItems(player, false);
			if (batchItems.Count >= Capacity)
			{
				player.LeftNotice("The Batch Hatcher has no free spaces.");
				return;
			}

			item.postion = ItemPosition;
			item.property = checked(GetUnixTime() + HatchDurationSeconds);
			player.GetItemSystem().ClearItem(item.id);
			player.GetItemSystem().DB_Save();

			List<RoleItemInfo> responseItems = new List<RoleItemInfo>();
			responseItems.Add(item);
			SendPackage(player, 1, responseItems, false);
			player.LeftNotice(
				"Your eudemon egg will hatch in the Batch Hatcher after 3 hours.");
			Log.Instance().WriteLog(
				"Placed eudemon egg item " + item.id.ToString() +
				" into the Batch Hatcher for role " +
				player.GetTypeId().ToString() + " with finish time " +
				item.property.ToString() + ".");
		}

		private static void CheckOut(PlayerObject player, uint clientItemId)
		{
			RefreshCompletedHatches(player);
			RoleItemInfo item = FindBatchItem(player, clientItemId);
			if (item == null)
			{
				player.LeftNotice("That eudemon is not in your Batch Hatcher.");
				return;
			}

			if (item.itemid / EudemonHatchManager.ItemTypeFamilyDivisor ==
				EudemonHatchManager.EggItemTypeFamily)
			{
				player.LeftNotice("Your eudemon egg is not hatched yet.");
				return;
			}
			if (item.itemid / EudemonHatchManager.ItemTypeFamilyDivisor !=
				EudemonHatchManager.EudemonItemTypeFamily)
			{
				player.LeftNotice(
					"That Batch Hatcher entry is not a completed eudemon.");
				return;
			}
			if (!player.GetItemSystem().CanAcceptAtPosition(
				MsgItemInfo.ITEMPOSITION_EUDEMON_PACK))
			{
				player.GetItemSystem().NotifyPackageFull(
					MsgItemInfo.ITEMPOSITION_EUDEMON_PACK);
				return;
			}

			RoleData_Eudemon eudemon =
				player.GetEudemonSystem().FindEudemon(item.typeid);
			if (eudemon == null)
			{
				player.LeftNotice(
					"That completed eudemon has no persistent eudemon record.");
				Log.Instance().WriteLog(
					"Rejected Batch Hatcher checkout for item " +
					item.id.ToString() + " because eudemon identity " +
					item.typeid.ToString() + " was not loaded.");
				return;
			}

			item.postion = MsgItemInfo.ITEMPOSITION_EUDEMON_PACK;
			item.property = 0;
			byte[] removal = MapPacketCodec.CreateEudemonPackageRemovalResponse(
				null,
				BatchPackageId,
				0,
				PackageType,
				clientItemId);
			player.SendData(removal, true);
			player.GetItemSystem().SendItemInfo(item);
			player.GetEudemonSystem().SendEudemonInfo(eudemon, true, true);
			player.GetItemSystem().DB_Save();
			player.GetEudemonSystem().DB_Save();
			Log.Instance().WriteLog(
				"Checked completed Batch Hatcher eudemon " +
				item.typeid.ToString() + " into the eudemon bag for role " +
				player.GetTypeId().ToString() + ".");
		}

		private static void SendPackage(
			PlayerObject player,
			byte operation,
			List<RoleItemInfo> sourceItems,
			bool refreshCompleted = true)
		{
			if (refreshCompleted)
			{
				RefreshCompletedHatches(player);
				sourceItems = GetBatchItems(player, false);
			}

			int currentTime = GetUnixTime();
			List<EudemonPackageItem> records =
				new List<EudemonPackageItem>(sourceItems.Count);
			for (int index = 0; index < sourceItems.Count; index++)
			{
				RoleItemInfo item = sourceItems[index];
				RoleData_Eudemon eudemon = null;
				if (item.itemid / EudemonHatchManager.ItemTypeFamilyDivisor ==
					EudemonHatchManager.EudemonItemTypeFamily)
				{
					eudemon = player.GetEudemonSystem().FindEudemon(item.typeid);
					if (eudemon == null)
					{
						Log.Instance().WriteLog(
							"Skipped Batch Hatcher item " + item.id.ToString() +
							" because its persistent eudemon record was not loaded.");
						continue;
					}
				}
				records.Add(CreatePackageItem(item, eudemon, currentTime));
			}

			byte[] response = MapPacketCodec.CreateEudemonPackageListResponse(
				null,
				BatchPackageId,
				0,
				operation,
				PackageType,
				0,
				records);
			player.SendData(response, true);
		}

		private static void RefreshCompletedHatches(PlayerObject player)
		{
			int currentTime = GetUnixTime();
			bool changed = false;
			List<RoleItemInfo> items = GetBatchItems(player, false);
			for (int index = 0; index < items.Count; index++)
			{
				RoleItemInfo item = items[index];
				if (item.itemid / EudemonHatchManager.ItemTypeFamilyDivisor !=
					EudemonHatchManager.EggItemTypeFamily ||
					MapPacketCodec.GetRemainingHatchSeconds(
						item.property, currentTime) != 0)
				{
					continue;
				}

				uint eudemonItemTypeId;
				if (!EudemonHatchManager.TryGetEudemonItemTypeId(
					item.itemid, out eudemonItemTypeId) ||
					ConfigManager.Instance().GetItemTypeInfo(eudemonItemTypeId) == null)
				{
					Log.Instance().WriteLog(
						"Batch Hatcher item " + item.id.ToString() +
						" finished but has no valid eudemon item type for egg " +
						item.itemid.ToString() + ".");
					continue;
				}

				item.itemid = eudemonItemTypeId;
				item.property = 0;
				if (item.typeid == 0)
				{
					item.typeid = IDManager.CreateTypeId(4);
				}
				ItemTypeInfo itemType =
					ConfigManager.Instance().GetItemTypeInfo(item.itemid);
				item.forgename = itemType == null ? "" : itemType.name;
				if (player.GetEudemonSystem().FindEudemon(item.typeid) == null)
				{
					// Keep the completed Eudemon in type-120 storage. Its full record is
					// sent by the package response and it must not appear in the normal
					// Eudemon bag until operation 2 checks it out.
					player.GetEudemonSystem().AddEudemon(
						item, 1, 0, 0, false);
				}
				changed = true;
				Log.Instance().WriteLog(
					"Completed Batch Hatcher egg item " + item.id.ToString() +
					" as eudemon identity " + item.typeid.ToString() +
					" for role " + player.GetTypeId().ToString() + ".");
			}

			if (changed)
			{
				player.GetItemSystem().DB_Save();
				player.GetEudemonSystem().DB_Save();
			}
		}

		private static List<RoleItemInfo> GetBatchItems(
			PlayerObject player,
			bool refreshCompleted = true)
		{
			if (refreshCompleted)
			{
				RefreshCompletedHatches(player);
			}

			List<RoleItemInfo> items = new List<RoleItemInfo>();
			foreach (RoleItemInfo item in
				player.GetItemSystem().GetDicItem().Values)
			{
				if (item.postion == ItemPosition)
				{
					items.Add(item);
				}
			}
			items.Sort(delegate(RoleItemInfo left, RoleItemInfo right)
			{
				return left.id.CompareTo(right.id);
			});
			return items;
		}

		private static RoleItemInfo FindBatchItem(
			PlayerObject player,
			uint clientItemId)
		{
			foreach (RoleItemInfo item in
				player.GetItemSystem().GetDicItem().Values)
			{
				if (item.postion != ItemPosition)
				{
					continue;
				}
				uint itemFamily = item.itemid /
					EudemonHatchManager.ItemTypeFamilyDivisor;
				uint expectedClientId =
					itemFamily == EudemonHatchManager.EudemonItemTypeFamily
						? item.typeid
						: item.id;
				if (expectedClientId == clientItemId)
				{
					return item;
				}
			}
			return null;
		}

		private static void SendOpenWindowAction(PlayerObject player)
		{
			byte[] response = MapPacketCodec.CreateActionResponse(
				null,
				Environment.TickCount,
				player.GetTypeId(),
				0,
				0,
				Capacity,
				OpenWindowAction);
			player.SendData(response, true);
			Log.Instance().WriteLog(
				"Answered Batch Hatcher availability query with action 0x266c " +
				"and capacity " + Capacity.ToString() + " for role " +
				player.GetTypeId().ToString() + ".");
		}

		private static void SendFreeAppraisalStatus(PlayerObject player)
		{
			// The client treats any nonzero value as one free appraisal and exposes
			// a tooltip that says another eudemon may be identified for free. No
			// corresponding entitlement exists in the available server schema, so
			// the coherent fail-closed response is zero until that source is found.
			byte[] response = MapPacketCodec.CreateEudemonPackageListResponse(
				null,
				BatchPackageId,
				0,
				8,
				PackageType,
				0,
				new List<EudemonPackageItem>());
			player.SendData(response, true);
		}

		private static void SelectMainPet(
			PlayerObject player,
			uint eudemonId)
		{
			RoleData_Eudemon eudemon =
				player.GetEudemonSystem().FindEudemon(eudemonId);
			RoleItemInfo item = eudemon == null
				? null
				: player.GetItemSystem().FindItem(eudemon.itemid);
			if (eudemon == null || item == null ||
				item.postion != MsgItemInfo.ITEMPOSITION_EUDEMON_PACK)
			{
				player.LeftNotice(
					"That eudemon is not available in your eudemon bag.");
				Log.Instance().WriteLog(
					"Rejected Batch Hatcher main-pet query for runtime identity " +
					eudemonId.ToString() + " on role " +
					player.GetTypeId().ToString() + ".");
				return;
			}

			// The definitive client does not derive its selected main-pet object from
			// packet 1117. Its packet-1008 action-9 handler owns that state change.
			// SendItemInfo preserves the authoritative runtime Eudemon identity in the
			// packet ID field when the source item is in position 53.
			player.GetItemSystem().SendItemInfo(item, 9);

			byte[] response = MapPacketCodec.CreateEudemonPackageListResponse(
				null,
				PackageId,
				0,
				9,
				PackageType,
				0,
				new List<EudemonPackageItem>());
			player.SendData(response, true);
			Log.Instance().WriteLog(
				"Accepted Batch Hatcher main-pet query for runtime identity " +
				eudemonId.ToString() + " on role " +
				player.GetTypeId().ToString() + ".");
		}

		private static void SendAppraisalInformation(
			PlayerObject player,
			EudemonPackagePacket request)
		{
			if (request.PackageId != BatchPackageId ||
				request.Context != 0 ||
				(request.OperationValue != 12 &&
				 request.OperationValue != 13))
			{
				LogRejectedRequest(request, "invalid appraisal envelope");
				return;
			}

			RoleData_Eudemon eudemon =
				player.GetEudemonSystem().FindEudemon(request.OperationItemId);
			RoleItemInfo item = eudemon == null
				? null
				: player.GetItemSystem().FindItem(eudemon.itemid);
			ushort requiredPosition = request.OperationValue == 12
				? MsgItemInfo.ITEMPOSITION_EUDEMON_PACK
				: ItemPosition;
			if (eudemon == null || item == null ||
				item.postion != requiredPosition)
			{
				LogRejectedRequest(
					request,
					"appraisal target is absent from the required package");
				return;
			}
			if (request.OperationValue == 13 &&
				!player.ConsumeBatchHatchAppraisalAllowance())
			{
				LogRejectedRequest(
					request,
					"completed-pet appraisal was not authorized by operation 11");
				return;
			}

			player.GetEudemonSystem().SendBatchHatchAppraisalInfo(
				eudemon,
				checked((int)request.OperationValue));
			Log.Instance().WriteLog(
				"Answered Batch Hatcher appraisal query for eudemon " +
				request.OperationItemId.ToString() + " with packet-2037 tag " +
				request.OperationValue.ToString() + " on role " +
				player.GetTypeId().ToString() + ".");
		}

		private static void AuthorizeAppraisals(
			PlayerObject player,
			EudemonPackagePacket request)
		{
			player.SetBatchHatchAppraisalAllowance(0);
			if (request.PackageId != PackageId || request.Context != 0 ||
				request.OperationValue == 0 ||
				request.OperationValue > Capacity)
			{
				LogRejectedRequest(request, "invalid appraisal-payment envelope");
				return;
			}

			int appraisalCount = checked((int)request.OperationValue);
			// The definitive client spends one SpellEudScore item per appraisal,
			// then charges one EP per two remaining appraisals, rounding an odd
			// final appraisal upward. The server advertises no free entitlement.
			int availableScrolls = player.GetItemSystem().GetBackpackItemAmount(
				AppraisalScrollItemTypeId);
			int scrollCost;
			int epCost;
			CalculateAppraisalPayment(
				appraisalCount, availableScrolls, out scrollCost, out epCost);
			if (player.GetMoneyCount(MONEYTYPE.GAMEGOLD) < epCost)
			{
				LogRejectedRequest(request, "insufficient EP for appraisal payment");
				return;
			}
			if (!player.GetItemSystem().ConsumeBackpackItemAmount(
				AppraisalScrollItemTypeId, scrollCost))
			{
				LogRejectedRequest(
					request,
					"appraisal-scroll inventory changed during payment");
				return;
			}

			if (epCost != 0)
			{
				player.ChangeMoney(MONEYTYPE.GAMEGOLD, -epCost);
			}
			player.SetBatchHatchAppraisalAllowance(appraisalCount);
			Log.Instance().WriteLog(
				"Authorized " + appraisalCount.ToString() +
				" Batch Hatcher appraisal(s) for " +
				scrollCost.ToString() + " appraisal scroll(s) and " +
				epCost.ToString() + " EP on role " +
				player.GetTypeId().ToString() + ".");
		}

		public static void CalculateAppraisalPayment(
			int appraisalCount,
			int availableScrolls,
			out int scrollCost,
			out int epCost)
		{
			if (appraisalCount < 0)
			{
				throw new ArgumentOutOfRangeException("appraisalCount");
			}
			if (availableScrolls < 0)
			{
				throw new ArgumentOutOfRangeException("availableScrolls");
			}
			scrollCost = Math.Min(appraisalCount, availableScrolls);
			int remainingAppraisals = appraisalCount - scrollCost;
			epCost = (remainingAppraisals + 1) / 2;
		}

		private static void ChangeEgg(
			PlayerObject player,
			EudemonPackagePacket request)
		{
			if (request.PackageId != BatchPackageId || request.Context != 0 ||
				request.OperationValue != 1)
			{
				LogRejectedRequest(request, "invalid egg-exchange envelope");
				SendChangeEggResult(player, request, false);
				return;
			}

			RoleItemInfo item = FindBatchItem(player, request.OperationItemId);
			RoleData_Eudemon eudemon = item == null
				? null
				: player.GetEudemonSystem().FindEudemon(item.typeid);
			if (item == null || eudemon == null ||
				item.postion != ItemPosition ||
				item.itemid / EudemonHatchManager.ItemTypeFamilyDivisor !=
					EudemonHatchManager.EudemonItemTypeFamily)
			{
				LogRejectedRequest(
					request,
					"egg-exchange target is not a completed Batch Hatcher pet");
				SendChangeEggResult(player, request, false);
				return;
			}

			int price;
			if (!ConfigManager.Instance().TryGetChangeEggPrice(
				item.itemid, out price))
			{
				LogRejectedRequest(
					request,
					"egg-exchange target is absent from ChangeEgg.csv");
				SendChangeEggResult(player, request, false);
				return;
			}
			if (player.GetMoneyCount(MONEYTYPE.GAMEGOLD) < price)
			{
				LogRejectedRequest(request, "insufficient EP for egg exchange");
				SendChangeEggResult(player, request, false);
				return;
			}

			int previousLuck = eudemon.luck;
			int previousInitialLife = eudemon.init_life;
			if (!player.GetEudemonSystem().RerollBatchHatchEudemon(
				item, eudemon))
			{
				LogRejectedRequest(request, "egg-exchange attribute reroll failed");
				SendChangeEggResult(player, request, false);
				return;
			}

			player.ChangeMoney(MONEYTYPE.GAMEGOLD, -price);
			player.GetEudemonSystem().DB_Save();
			// Packet 2037 tag 13 updates the existing type-120 CItem in place.
			// It must precede the operation-6 acknowledgement so the completion
			// dialog cannot race ahead of the authoritative attribute refresh.
			player.GetEudemonSystem().SendBatchHatchAppraisalInfo(eudemon, 13);
			SendChangeEggResult(player, request, true);
			Log.Instance().WriteLog(
				"Exchanged Batch Hatcher eudemon " +
				request.OperationItemId.ToString() + " for " +
				price.ToString() + " EP on role " +
				player.GetTypeId().ToString() + "; luck " +
				previousLuck.ToString() + " -> " + eudemon.luck.ToString() +
				", initial life " + previousInitialLife.ToString() + " -> " +
				eudemon.init_life.ToString() + ".");
		}

		private static void SendChangeEggResult(
			PlayerObject player,
			EudemonPackagePacket request,
			bool success)
		{
			byte[] response = MapPacketCodec.CreateEudemonPackageListResponse(
				null,
				request.PackageId,
				request.Context,
				6,
				PackageType,
				success ? 1U : 0U,
				new List<EudemonPackageItem>());
			player.SendData(response, true);
		}

		private static void LogRejectedRequest(
			EudemonPackagePacket request,
			string reason)
		{
			Log.Instance().WriteLog(
				"Rejected Batch Hatcher packet 1117 variant (" + reason + "): " +
				"operation=" + request.Operation.ToString() +
				", packageId=" + request.PackageId.ToString() +
				", context=" + request.Context.ToString() +
				", value=" + request.OperationValue.ToString() +
				", entryCount=" + request.EntryCount.ToString() + ".");
		}

		private static uint ToUInt32(int value)
		{
			return value <= 0 ? 0U : (uint)value;
		}

		private static uint ToUInt32(long value)
		{
			if (value <= 0)
			{
				return 0;
			}
			return value > uint.MaxValue ? uint.MaxValue : (uint)value;
		}

		private static ulong ToUInt64(int value)
		{
			return value <= 0 ? 0UL : (ulong)value;
		}

		private static ushort ToUInt16(int value)
		{
			if (value <= 0)
			{
				return 0;
			}
			return value > ushort.MaxValue ? ushort.MaxValue : (ushort)value;
		}

		private static byte ToByte(int value)
		{
			if (value <= 0)
			{
				return 0;
			}
			return value > byte.MaxValue ? byte.MaxValue : (byte)value;
		}

		private static int GetUnixTime()
		{
			return checked((int)(DateTime.UtcNow -
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).
				TotalSeconds);
		}
	}
}
