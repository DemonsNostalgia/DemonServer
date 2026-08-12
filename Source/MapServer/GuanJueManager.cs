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
	// Token: 0x02000044 RID: 68
	public class GuanJueManager
	{
		// Token: 0x06000189 RID: 393 RVA: 0x00011AFC File Offset: 0x0000FCFC
		public static GuanJueManager Instance()
		{
			if (GuanJueManager.mInstance == null)
			{
				GuanJueManager.mInstance = new GuanJueManager();
			}
			return GuanJueManager.mInstance;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00011B2E File Offset: 0x0000FD2E
		public GuanJueManager()
		{
			this.mList = new List<GuanJueInfo>();
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00011B44 File Offset: 0x0000FD44
		public void DB_Load(GUANJUEINFO info)
		{
			this.mList.Clear();
			for (int i = 0; i < info.list_item.Count; i++)
			{
				this.mList.Add(info.list_item[i]);
			}
			Log.Instance().WriteLog("Loaded nobility-rank data from DBServer.");
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00011BA4 File Offset: 0x0000FDA4
		public void DB_Update(PlayerObject play)
		{
			UPDATEGUANJUEDATA updateguanjuedata = new UPDATEGUANJUEDATA();
			updateguanjuedata.info.id = (uint)play.GetBaseAttr().player_id;
			updateguanjuedata.info.name = play.GetName();
			updateguanjuedata.info.guanjue = play.GetBaseAttr().guanjue;
			DBServer.Instance().GetDBClient().SendData(updateguanjuedata.GetBuffer());
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00011C0C File Offset: 0x0000FE0C
		public void RequestData(PlayerObject play, byte page)
		{
			int num = (int)(page * 10);
			if (num < 0)
			{
				num = 0;
			}
			if (num < this.mList.Count)
			{
				MsgGuanJueInfo msgGuanJueInfo = new MsgGuanJueInfo();
				msgGuanJueInfo.Create(null, play.GetGamePackKeyEx());
				for (int i = num; i < num + 10; i++)
				{
					if (i >= this.mList.Count)
					{
						break;
					}
					MsgGuanJueItem msgGuanJueItem = new MsgGuanJueItem();
					msgGuanJueItem.guanjue = this.mList[i].guanjue;
					msgGuanJueItem.name = this.mList[i].name;
					msgGuanJueItem.pos = i;
					msgGuanJueInfo.list_item.Add(msgGuanJueItem);
				}
				msgGuanJueInfo.page = (int)page;
				play.SendData(msgGuanJueInfo.GetBuffer(), false);
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00011CE8 File Offset: 0x0000FEE8
		public void Donation(PlayerObject play, MONEYTYPE type, int value)
		{
			GUANGJUELEVEL guanJue = play.GetGuanJue();
			int num = 0;
			switch (type)
			{
			case MONEYTYPE.GOLD:
				if (num < 3000000)
				{
					play.LeftNotice("Minimum Contribution" + 3000000.ToString() + "Starting from 10,000 gold.");
					return;
				}
				if (play.GetMoneyCount(MONEYTYPE.GOLD) < value)
				{
					play.LeftNotice("Gold coins insufficient, cannot donate! ");
					return;
				}
				num = value;
				play.ChangeAttribute(UserAttribute.GOLD, -num, true);
				break;
			case MONEYTYPE.GAMEGOLD:
				if (play.GetMoneyCount(MONEYTYPE.GAMEGOLD) < value)
				{
					play.LeftNotice("Magic stone insufficient, cannot donate! ");
					return;
				}
				play.ChangeAttribute(UserAttribute.GAMEGOLD, -value, true);
				num = value * 7100;
				if (num < 3000000)
				{
					play.LeftNotice("Minimum Contribution" + 3000000.ToString() + "Starting from 10,000 gold.");
					return;
				}
				break;
			}
			play.GetBaseAttr().guanjue += (ulong)num;
			this.SetValue(play.GetBaseAttr().player_id, play.GetName(), play.GetBaseAttr().guanjue);
			GUANGJUELEVEL level = this.GetLevel(play);
			if (guanJue != level)
			{
				this.SendChangeGuanJueMsg(play, level);
			}
			if (level != play.GetGuanJue())
			{
				play.SetGuanJue(level);
			}
			this.SendGuanJueInfo(play);
			this.DB_Update(play);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00011E78 File Offset: 0x00010078
		public void SetValue(int play_id, string name, ulong guanjue)
		{
			for (int i = 0; i < this.mList.Count; i++)
			{
				if ((ulong)this.mList[i].id == (ulong)((long)play_id))
				{
					this.mList.RemoveAt(i);
					break;
				}
			}
			GuanJueInfo guanJueInfo = new GuanJueInfo();
			guanJueInfo.id = (uint)play_id;
			guanJueInfo.name = name;
			guanJueInfo.guanjue = guanjue;
			bool flag = false;
			for (int i = 0; i < this.mList.Count; i++)
			{
				if (guanjue > this.mList[i].guanjue)
				{
					this.mList.Insert(i, guanJueInfo);
					flag = true;
					break;
				}
			}
			if (!flag && this.mList.Count < 50)
			{
				this.mList.Add(guanJueInfo);
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00011F64 File Offset: 0x00010164
		public GUANGJUELEVEL GetLevel(PlayerObject play)
		{
			GUANGJUELEVEL guangjuelevel = GUANGJUELEVEL.NORMAL;
			int num = -1;
			for (int i = 0; i < this.mList.Count; i++)
			{
				if (this.mList[i].id == (uint)play.GetBaseAttr().player_id)
				{
					num = i;
					break;
				}
			}
			GUANGJUELEVEL result;
			if (num != -1)
			{
				if (num >= 0 && num <= 2)
				{
					guangjuelevel = ((play.GetSex() == 1) ? GUANGJUELEVEL.KING : GUANGJUELEVEL.QUEEN);
				}
				else if (num >= 3 && num <= 14)
				{
					guangjuelevel = GUANGJUELEVEL.DUKE;
				}
				else if (num >= 15 && num <= 49)
				{
					guangjuelevel = GUANGJUELEVEL.MARQUIS;
				}
				result = guangjuelevel;
			}
			else
			{
				ulong guanjue = play.GetBaseAttr().guanjue;
				if (guanjue >= 200000000UL)
				{
					guangjuelevel = GUANGJUELEVEL.EARL;
				}
				else if (guanjue >= 100000000UL)
				{
					guangjuelevel = GUANGJUELEVEL.VISCOUNT;
				}
				else if (guanjue >= 30000000UL)
				{
					guangjuelevel = GUANGJUELEVEL.LORD;
				}
				result = guangjuelevel;
			}
			return result;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00012078 File Offset: 0x00010278
		public void SendGuanJueInfo(PlayerObject play)
		{
			ulong guanjue = play.GetBaseAttr().guanjue;
			byte[] bytes = Coding.GetDefauleCoding().GetBytes(guanjue.ToString());
			PacketOut packetOut = new PacketOut(play.GetGamePackKeyEx());
			packetOut.WriteUInt16((ushort)(bytes.Length + 4 + 14));
			packetOut.WriteUInt16(1015);
			packetOut.WriteUInt32(0U);
			packetOut.WriteUInt16(113);
			packetOut.WriteByte(1);
			packetOut.WriteByte((byte)(bytes.Length + 5));
			string s = ((byte)play.GetGuanJue()).ToString();
			byte[] bytes2 = Coding.GetDefauleCoding().GetBytes(s);
			packetOut.WriteByte(bytes2[0]);
			packetOut.WriteByte(32);
			packetOut.WriteByte(45);
			packetOut.WriteByte(49);
			packetOut.WriteByte(32);
			packetOut.WriteBuff(bytes);
			packetOut.WriteByte(0);
			play.SendData(packetOut.Flush(), false);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00012160 File Offset: 0x00010360
		public void SendChangeGuanJueMsg(PlayerObject play, GUANGJUELEVEL lv)
		{
			switch (lv)
			{
			case GUANGJUELEVEL.KING:
			{
				string msg = string.Format("This day will be recorded in eternal history, player [{0}] achieved supreme glory, ascended to the throne", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.SCREEN, msg);
				break;
			}
			case GUANGJUELEVEL.QUEEN:
			{
				string msg = string.Format("This is a moment witnessed by all！[{0}]has donned the sacred crown，Let us cheer for the new Queen of Cronus！", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.SCREEN, msg);
				break;
			}
			case GUANGJUELEVEL.DUKE:
			{
				string msg = string.Format("The bell of Canossa City rings loudly, [{0}] has made significant contributions to the kingdom and is ennobled as a Royal Duke!", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.SCREEN, msg);
				break;
			}
			case GUANGJUELEVEL.MARQUIS:
			{
				string msg = string.Format("The glorious horn sounds, [{0}] is ennobled as a Royal Marquis. May his flag of glory shine forever!", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.SCREEN, msg);
				break;
			}
			case GUANGJUELEVEL.EARL:
			{
				string msg = string.Format("Congratulations to player [{0}] for being appointed as a Baron! Celebratory songs will echo throughout the city, and his name will be remembered alongside the city of Canossa.", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.CHAT, msg);
				break;
			}
			case GUANGJUELEVEL.VISCOUNT:
			{
				string msg = string.Format("Congratulations to player [{0}] being ennobled as a Baron, under the sacred light, let us jointly witness this honor!", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.CHAT, msg);
				break;
			}
			case GUANGJUELEVEL.LORD:
			{
				string msg = string.Format("Congratulations to player [{0}] for being elevated to Lord, Caunos City now has another noble guardian.", play.GetName());
				UserEngine.Instance().BroadcastMsg(BROADCASTMSGTYPE.CHAT, msg);
				break;
			}
			}
		}

		// Token: 0x04000353 RID: 851
		private const int MAX_JUEWEI = 50;

		// Token: 0x04000354 RID: 852
		private static GuanJueManager mInstance = null;

		// Token: 0x04000355 RID: 853
		private List<GuanJueInfo> mList;
	}
}
