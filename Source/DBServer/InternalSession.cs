using System;
using System.Collections.Generic;
using System.Net.Sockets;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameBase.Network.Internal;

namespace DBServer
{
	// Token: 0x02000004 RID: 4
	public class InternalSession
	{
		// Token: 0x0600000C RID: 12 RVA: 0x000023D4 File Offset: 0x000005D4
		public Socket GetSocket()
		{
			return this.mSocket;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000023EC File Offset: 0x000005EC
		public InternalPacket GetPacket()
		{
			return this.mPacket;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002404 File Offset: 0x00000604
		public byte GetSessionType()
		{
			return this.mType;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000241C File Offset: 0x0000061C
		public void SetSessionType(byte _type)
		{
			this.mType = _type;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002428 File Offset: 0x00000628
		public string GetSessionName()
		{
			return this.mName;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002440 File Offset: 0x00000640
		public void SetSessionName(string _name)
		{
			this.mName = _name;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000244C File Offset: 0x0000064C
		public int GetLastTime()
		{
			return this.lastTime;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002464 File Offset: 0x00000664
		public void SetLastTime(int _lasttime)
		{
			this.lastTime = _lasttime;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002470 File Offset: 0x00000670
		public TcpServer GetTcpServer()
		{
			return this.mTcpServer;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002488 File Offset: 0x00000688
		public InternalSession(TcpServer server, Socket s)
		{
			this.mPacket = new InternalPacket();
			this.mName = "";
			this.mType = 0;
			this.lastTime = Environment.TickCount;
			this.mTcpServer = server;
			this.mSocket = s;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000024DC File Offset: 0x000006DC
		public void Run()
		{
			byte[] data = this.mPacket.GetData();
			if (data != null)
			{
				PackIn packIn = new PackIn(data);
				ushort num = packIn.ReadUInt16();
				ushort num2 = num;
				switch (num2)
				{
				case 10:
					break;
				case 11:
					this.ProcessQueryRole(data);
					return;
				default:
					switch (num2)
					{
					case 111:
						break;
					case 112:
					case 116:
					case 118:
					case 121:
					case 122:
					case 123:
					case 125:
					case 127:
					case 128:
					case 130:
					case 132:
					case 134:
					case 135:
						return;
					case 113:
						this.ProcessRoleInfo_Ret(data);
						return;
					case 114:
						this.ProcessQueryRoleName(data);
						return;
					case 115:
						this.ProcessCreateRole(data);
						return;
					case 117:
						this.ProcessSaveRoleData_Attr(data);
						return;
					case 119:
						this.ProcessDeleteRoleData_Item(data);
						return;
					case 120:
						this.ProcessAddRoleData_Item(data);
						return;
					case 124:
						this.ProcessSaveRoleData_Item(data);
						return;
					case 126:
						this.ProcessSaveRoleData_Magic(data);
						return;
					case 129:
						this.ProcessSaveRoleData_Eudemon(data);
						return;
					case 131:
						this.ProcessSaveRoleData_Friend(data);
						return;
					case 133:
						this.ProcessUpdateGuanJueData(data);
						return;
					case 136:
						this.ProcessUpdateLegion(data);
						return;
					case 137:
						this.ProcessCreateLegion(data);
						return;
					case 141:
						this.ProcessDeleteLegion(data);
						return;
					case FamilyOption.CreateParameter:
						this.ProcessCreateFamily(data);
						return;
					case FamilyOption.UpdateParameter:
						this.ProcessUpdateFamily(data);
						return;
					case FamilyOption.DeleteParameter:
						this.ProcessDeleteFamily(data);
						return;
					default:
						if (num2 != 140)
						{
							return;
						}
						this.ProcessUpdatePayrecInfo(data);
						return;
					}
					break;
				}
				this.mType = packIn.ReadByte();
				this.mName = packIn.ReadString();
				Log.Instance().WriteLog("server connect...type:" + this.mType.ToString() + " name:" + this.mName);
				if (this.mType == 5)
				{
					GuanJue.GetInstance().SendData(0);
					Legion.GetInstance().SendData(0);
					Family.GetInstance().SendData(0);
					PayManager.Instance().SendData(0);
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000026E0 File Offset: 0x000008E0
		private void ProcessQueryRole(byte[] data)
		{
			QueryRole queryRole = new QueryRole(0U, 0, 0, null);
			queryRole.Create(data);
			byte b = 0;
			string account = queryRole.GetAccount();
			int num = Data.QueryAccount(account);
			if (num != -1)
			{
				b = 1;
				int mapid = -1;
				if (Data.IsOnline(account, ref mapid))
				{
					b = 2;
					KickGamePlay kickGamePlay = new KickGamePlay();
					kickGamePlay.accountid = num;
					SessionManager.Instance().SendMapServer(mapid, kickGamePlay.GetBuffer());
					Data.SetOnlineState(num, -1);
				}
				if (b == 1)
				{
					Log.Instance().WriteLog("Role query succeeded. Account: " + account + "id:" + num.ToString());
					RoleInfo roleInfo = Data.QueryRoleInfo(num);
					roleInfo.gameid = queryRole.gameid;
					roleInfo.mKey = queryRole.key;
					roleInfo.mKey1 = queryRole.key2;
					roleInfo.sAccount = account;
					SessionManager.Instance().SendMapServer(roleInfo.mapid, roleInfo.GetBuffer());
					ROLEDATA_ITEM roledata_ITEM = new ROLEDATA_ITEM();
					roledata_ITEM.key = queryRole.key;
					roledata_ITEM.key2 = queryRole.key2;
					roledata_ITEM.playerid = roleInfo.playerid;
					roledata_ITEM.SetLoadTag();
					Data.LoadRoleData_Item(roledata_ITEM);
					if (roledata_ITEM.mListItem.Count > 0)
					{
						SessionManager.Instance().SendMapServer(0, roledata_ITEM.GetBuffer());
					}
					RoleData_Magic roleData_Magic = new RoleData_Magic();
					roleData_Magic.SetLoadTag();
					roleData_Magic.ownerid = roleInfo.playerid;
					roleData_Magic.key = roleInfo.mKey;
					roleData_Magic.key2 = roleInfo.mKey1;
					Data.LoadRoleData_Magic(roleData_Magic);
					if (roleData_Magic.mListMagic.Count > 0)
					{
						SessionManager.Instance().SendMapServer(0, roleData_Magic.GetBuffer());
					}
					List<RoleData_Item> eudemonItemList = roledata_ITEM.GetEudemonItemList();
					if (eudemonItemList != null)
					{
						ROLEDATE_EUDEMON roledate_EUDEMON = new ROLEDATE_EUDEMON();
						roledate_EUDEMON.SetLoadTag();
						roledate_EUDEMON.playerid = roleInfo.playerid;
						roledate_EUDEMON.key = roleInfo.mKey;
						roledate_EUDEMON.key2 = roleInfo.mKey1;
						Data.LoadRoleData_Eudemon(roledate_EUDEMON);
						SessionManager.Instance().SendMapServer(0, roledate_EUDEMON.GetBuffer());
					}
					ROLEDATA_FRIEND roledata_FRIEND = new ROLEDATA_FRIEND();
					roledata_FRIEND.SetLoadTag();
					roledata_FRIEND.playerid = roleInfo.playerid;
					roledata_FRIEND.key = roleInfo.mKey;
					roledata_FRIEND.key2 = roleInfo.mKey1;
					Data.LoadRoleData_Friend(roledata_FRIEND);
					SessionManager.Instance().SendMapServer(0, roledata_FRIEND.GetBuffer());
				}
				else
				{
					Log.Instance().WriteLog("Role query failed. Account: " + account + "id:" + num.ToString());
				}
			}
			if (b != 1)
			{
				QueryRole_Ret queryRole_Ret = new QueryRole_Ret();
				queryRole_Ret.gameid = queryRole.gameid;
				queryRole_Ret.key = queryRole.key;
				queryRole_Ret.key2 = queryRole.key2;
				queryRole_Ret.ret = b;
				this.mTcpServer.SendData(this.mSocket, queryRole_Ret.GetBuffer());
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002A04 File Offset: 0x00000C04
		private void ProcessRoleInfo_Ret(byte[] data)
		{
			RoleInfo_Ret roleInfo_Ret = new RoleInfo_Ret();
			roleInfo_Ret.Create(data);
			Data.SetOnlineState(roleInfo_Ret.accountid, 0);
			QueryRole_Ret queryRole_Ret = new QueryRole_Ret();
			queryRole_Ret.gameid = roleInfo_Ret.gameid;
			queryRole_Ret.key = roleInfo_Ret.key;
			queryRole_Ret.key2 = roleInfo_Ret.key2;
			queryRole_Ret.ret = 1;
			SessionManager.Instance().SendLoginServer(queryRole_Ret.GetBuffer());
			Log.Instance().WriteLog("Notifying LoginServer of the authenticated role: " + roleInfo_Ret.accountid.ToString());
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A90 File Offset: 0x00000C90
		private void ProcessQueryRoleName(byte[] data)
		{
			QueryRoleName queryRoleName = new QueryRoleName();
			queryRoleName.Create(data);
			QueryRoleName_Ret queryRoleName_Ret = Data.QueryRoleName(queryRoleName.name);
			queryRoleName_Ret.gameid = queryRoleName.gameid;
			SessionManager.Instance().SendMapServer(0, queryRoleName_Ret.GetBuffer());
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002AD8 File Offset: 0x00000CD8
		public void ProcessCreateRole(byte[] data)
		{
			CreateRole createRole = new CreateRole();
			createRole.Create(data);
			CreateRole_Ret createRole_Ret = new CreateRole_Ret();
			createRole_Ret.gameid = createRole.gameid;
			createRole.name = Coding.GB2312ToLatin1(createRole.name);
			createRole_Ret.tag = Data.CreateRole(createRole.accountid, createRole.name, createRole.lookface, createRole.profession, ref createRole_Ret.playerid);
			SessionManager.Instance().SendMapServer(0, createRole_Ret.GetBuffer());
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002B54 File Offset: 0x00000D54
		public void ProcessSaveRoleData_Attr(byte[] data)
		{
			SaveRoleData_Attr saveRoleData_Attr = new SaveRoleData_Attr();
			saveRoleData_Attr.Create(data);
			if (saveRoleData_Attr.IsExit)
			{
				Data.SetOnlineState(saveRoleData_Attr.accountid, -1);
			}
			if (!Data.SaveRoleData_Attr(saveRoleData_Attr))
			{
				Log.Instance().WriteLog("Failed to save role data. Role ID: " + saveRoleData_Attr.accountid.ToString() + " Role name: " + saveRoleData_Attr.name);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002BC4 File Offset: 0x00000DC4
		public void ProcessAddRoleData_Item(byte[] data)
		{
			AddRoleData_Item addRoleData_Item = new AddRoleData_Item();
			addRoleData_Item.Create(data);
			uint id = 0U;
			if (!Data.AddRoleData_Item(addRoleData_Item, ref id))
			{
				Log.Instance().WriteLog("Failed to save role item data. Role ID: " + addRoleData_Item.item.playerid.ToString());
			}
			AddRoleData_Item_Ret addRoleData_Item_Ret = new AddRoleData_Item_Ret();
			addRoleData_Item_Ret.id = id;
			addRoleData_Item_Ret.gameid = addRoleData_Item.gameid;
			addRoleData_Item_Ret.sordid = addRoleData_Item.sortid;
			SessionManager.Instance().SendMapServer(0, addRoleData_Item_Ret.GetBuffer());
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002C4C File Offset: 0x00000E4C
		public void ProcessDeleteRoleData_Item(byte[] data)
		{
			DeleteItemByID deleteItemByID = new DeleteItemByID();
			deleteItemByID.Create(data);
			if (deleteItemByID.postion == 53)
			{
				if (!Data.DeleteDroppedEudemonItem(
					deleteItemByID.playerid,
					deleteItemByID.id))
				{
					Log.Instance().WriteLog(
						"Failed to delete dropped Eudemon data. Role ID: " +
						deleteItemByID.playerid.ToString());
				}
			}
			else if (!Data.DeleteRoleData_Item(
				deleteItemByID.playerid, deleteItemByID.id))
			{
				Log.Instance().WriteLog("Failed to delete role item data. Role ID: " + deleteItemByID.playerid.ToString());
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002CC4 File Offset: 0x00000EC4
		public void ProcessSaveRoleData_Item(byte[] data)
		{
			ROLEDATA_ITEM roledata_ITEM = new ROLEDATA_ITEM();
			roledata_ITEM.Create(data);
			Data.SaveRoleData_Item(roledata_ITEM);
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002CE8 File Offset: 0x00000EE8
		public void ProcessSaveRoleData_Magic(byte[] data)
		{
			RoleData_Magic roleData_Magic = new RoleData_Magic();
			roleData_Magic.Create(data);
			Data.SaveRoleData_Magic(roleData_Magic);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002D0C File Offset: 0x00000F0C
		public void ProcessSaveRoleData_Eudemon(byte[] data)
		{
			ROLEDATE_EUDEMON roledate_EUDEMON = new ROLEDATE_EUDEMON();
			roledate_EUDEMON.Create(data);
			Data.SaveRoleData_Eudemon(roledate_EUDEMON);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002D30 File Offset: 0x00000F30
		public void ProcessSaveRoleData_Friend(byte[] data)
		{
			ROLEDATA_FRIEND roledata_FRIEND = new ROLEDATA_FRIEND();
			roledata_FRIEND.Create(data);
			Data.SaveRoleData_Friend(roledata_FRIEND);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002D54 File Offset: 0x00000F54
		public void ProcessUpdateGuanJueData(byte[] data)
		{
			UPDATEGUANJUEDATA updateguanjuedata = new UPDATEGUANJUEDATA();
			updateguanjuedata.Create(data);
			GuanJue.GetInstance().UpdateGuanJueInfo(updateguanjuedata.info);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002D84 File Offset: 0x00000F84
		private void ProcessCreateLegion(byte[] data)
		{
			LegionOption legionOption = new LegionOption();
			legionOption.Create(data);
			Legion.GetInstance().CreateLegion(legionOption.mInfo, legionOption.player_id);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002DB8 File Offset: 0x00000FB8
		private void ProcessUpdateLegion(byte[] data)
		{
			LegionOption legionOption = new LegionOption();
			legionOption.Create(data);
			Legion.GetInstance().UpdateLegion(legionOption.mInfo);
		}

		private void ProcessDeleteLegion(byte[] data)
		{
			LegionOption legionOption = new LegionOption();
			legionOption.Create(data);
			Legion.GetInstance().DeleteLegion(legionOption.mInfo.id);
		}

		private void ProcessCreateFamily(byte[] data)
		{
			FamilyOption option = new FamilyOption();
			option.Create(data);
			Family.GetInstance().CreateFamily(option.Info, option.PlayerId);
		}

		private void ProcessUpdateFamily(byte[] data)
		{
			FamilyOption option = new FamilyOption();
			option.Create(data);
			Family.GetInstance().UpdateFamily(option.Info);
		}

		private void ProcessDeleteFamily(byte[] data)
		{
			FamilyOption option = new FamilyOption();
			option.Create(data);
			Family.GetInstance().DeleteFamily(option.Info.Id);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002DE8 File Offset: 0x00000FE8
		private void ProcessUpdatePayrecInfo(byte[] data)
		{
			PackUpdatePayRecInfo packUpdatePayRecInfo = new PackUpdatePayRecInfo();
			packUpdatePayRecInfo.Create(data);
			PayManager.Instance().SetPayTag(packUpdatePayRecInfo.account);
		}

		// Token: 0x04000005 RID: 5
		private Socket mSocket = null;

		// Token: 0x04000006 RID: 6
		private InternalPacket mPacket;

		// Token: 0x04000007 RID: 7
		private byte mType;

		// Token: 0x04000008 RID: 8
		private string mName;

		// Token: 0x04000009 RID: 9
		public int lastTime;

		// Token: 0x0400000A RID: 10
		private TcpServer mTcpServer;
	}
}
