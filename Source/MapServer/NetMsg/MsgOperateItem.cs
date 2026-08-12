using System;
using GameBase.Network;

namespace NetMsg
{
	// Token: 0x0200006C RID: 108
	public class MsgOperateItem : BaseMsg
	{
		// Token: 0x0600022E RID: 558 RVA: 0x000173B7 File Offset: 0x000155B7
		public MsgOperateItem()
		{
			this.mParam = 1009;
			this.id = 0U;
			this.usAction = 0;
			this.dwData = 0U;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x000173E4 File Offset: 0x000155E4
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
			if (msg != null)
			{
				PackIn packIn = new PackIn(msg);
				packIn.ReadUInt16();
				this.id = packIn.ReadUInt32();
				this.dwData = packIn.ReadUInt32();
				this.usAction = packIn.ReadUInt16();
				this.param = packIn.ReadUInt16();
				this.amount = packIn.ReadUInt16();
				this.param1 = packIn.ReadUInt16();
				this.param2 = packIn.ReadUInt32();
				this.param3 = packIn.ReadInt32();
			}
		}

		// Token: 0x040004D3 RID: 1235
		public const ushort ITEMACT_BUY = 1;

		// Token: 0x040004D4 RID: 1236
		public const ushort ITEMACT_SELL = 2;

		// Token: 0x040004D5 RID: 1237
		public const ushort ITEMACT_USE = 4;

		// Token: 0x040004D6 RID: 1238
		public const ushort ITEMACT_EQUIP = 5;

		// Token: 0x040004D7 RID: 1239
		public const ushort ITEMACT_UNEQUIP = 6;

		// Token: 0x040004D8 RID: 1240
		public const ushort ITEMACT_DROP = 3;

		// Token: 0x040004D9 RID: 1241
		public const ushort ITEMACT_REPAIREQUIP = 14;

		// Token: 0x040004DA RID: 1242
		public const ushort STRONGACT_SAVEMONEY = 10;

		// Token: 0x040004DB RID: 1243
		public const ushort STRONGACT_GIVEMONEY = 11;

		// Token: 0x040004DC RID: 1244
		public const ushort ITEMACT_DROPEQUIPMENT = 18;

		// Token: 0x040004DD RID: 1245
		public const ushort PTICH_SELL_ITEM_GOLD = 22;

		// Token: 0x040004DE RID: 1246
		public const ushort PTICH_GETBACK_SELLITEM = 23;

		// Token: 0x040004DF RID: 1247
		public const ushort PTICH_BUY_ITEM = 24;

		// Token: 0x040004E0 RID: 1248
		public const ushort EUDEMON_EVOLUTION = 28;

		// Token: 0x040004E1 RID: 1249
		public const ushort ITEMACT_OPENGEM = 59;

		// Token: 0x040004E2 RID: 1250
		public const ushort EUDEMONACT_RECALL = 32;

		// Token: 0x040004E3 RID: 1251
		public const ushort EUDEMONACT_FIT = 35;

		// Token: 0x040004E4 RID: 1252
		public const ushort EUDEMONACT_BREAK_UP = 36;

		// Token: 0x040004E5 RID: 1253
		public const ushort EUDEMON_DELETE_MAGIC = 41;

		// Token: 0x040004E6 RID: 1254
		public const ushort GET_EXPBALL_EXP = 50;

		// Token: 0x040004E7 RID: 1255
		public const ushort PTICH_SELL_ITEM_GAMEGOLD = 52;

		// Token: 0x040004E8 RID: 1256
		public const ushort USE_EXPBALL_EXP = 63;

		// Token: 0x040004E9 RID: 1257
		public const ushort EUDEMON_FOOD = 101;

		// Token: 0x040004EA RID: 1258
		public const ushort TAKEMOUNT = 110;

		// Token: 0x040004EB RID: 1259
		public const ushort TAKEOFFMOUNT = 111;

		// Token: 0x040004EC RID: 1260
		public const ushort GET_REMOTE_PTICH_ID = 114;

		// Token: 0x040004ED RID: 1261
		public const ushort GET_REMOTE_PTICH = 115;

		// Token: 0x040004EE RID: 1262
		public const ushort BUY_REMOTE_PTICH_ITEM = 116;

		// Token: 0x040004EF RID: 1263
		public uint id;

		// Token: 0x040004F0 RID: 1264
		public uint dwData;

		// Token: 0x040004F1 RID: 1265
		public ushort usAction;

		// Token: 0x040004F2 RID: 1266
		public ushort param;

		// Token: 0x040004F3 RID: 1267
		public ushort amount;

		// Token: 0x040004F4 RID: 1268
		public ushort param1;

		// Token: 0x040004F5 RID: 1269
		public uint param2;

		// Token: 0x040004F6 RID: 1270
		public int param3;
	}
}
