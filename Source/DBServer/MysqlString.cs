using System;

namespace DBServer
{
	// Token: 0x02000006 RID: 6
	public class MysqlString
	{
		// Token: 0x0400000D RID: 13
		public const string ADDACCOUNT = "insert into account(account,password,vip) values('{0}','{1}',{2})";

		// Token: 0x0400000E RID: 14
		public const string CREATE = "insert into cq_user(accountid,name,lookface,profession,level) values({0},'{1}',{2},{3},1)";

		// Token: 0x0400000F RID: 15
		public const string SAVEROLE_ATTR = "update cq_user set name='{0}',lookface={1},hair={2},level={3},exp={4},life={5},mana={6},profession={7},pk={8},gold={9},gamegold={10},stronggold={11},mapid={12},record_x={13},record_y={14},hotkey='{15}',guanjue={16},godlevel={17},maxeudemon={18} where accountid={19} ";

		// Token: 0x04000010 RID: 16
		public const string SAVEROLE_ITEM = "insert into cq_item(playerid,itemid,postion,stronglv,gem1,gem2,forgename,amount,war_ghost_exp,di_attack,shui_attack,huo_attack,feng_attack,property,gem3,god_exp,god_strong) values({0},{1},{2},{3},{4},{5},'{6}',{7},{8},{9},{10},{11},{12},{13},{14},{15},{16})";

		// Token: 0x04000011 RID: 17
		public const string UPDATEROLE_ITEM = "update cq_item set itemid={0},postion={1},stronglv={2},gem1={3},gem2={4},forgename='{5}',amount={6},war_ghost_exp={7},di_attack={8},shui_attack={9},huo_attack={10},feng_attack={11},property={12},gem3={13},god_exp={14},god_strong={15} where playerid={16} and id={17}";

		// Token: 0x04000012 RID: 18
		public const string LOADROLEDATA_ITEM = "select * from cq_item where playerid={0}";

		// Token: 0x04000013 RID: 19
		public const string DELETEROLEDATA_ITEM = "delete from cq_item where playerid={0} and id ={1}";

		// Token: 0x04000014 RID: 20
		public const string LOADROLEDATA_MAGIC = "select * from cq_magic where ownerid={0}";

		// Token: 0x04000015 RID: 21
		public const string ADDMAGIC = "insert into cq_magic(ownerid,magicid,level,exp) values({0},{1},{2},{3})";

		// Token: 0x04000016 RID: 22
		public const string UPDATEMAGIC = "update cq_magic set magicid={0},level={1},exp={2} where ownerid={3} and id={4}";

		// Token: 0x04000017 RID: 23
		public const string UPDATEONLINESTATE = "update account set serverindex ={0} where id={1}";

		// Token: 0x04000018 RID: 24
		public const string LOADROLEDATA_EUDEMON = "select * from cq_eudemon where ownerid ={0}";

		// Token: 0x04000019 RID: 25
		public const string SAVEROLEDATA_EUDEMON = "insert into cq_eudemon(itemid,ownerid,name,phyatk_grow_rate,phyatk_grow_rate_max,magicatk_grow_rate,magicatk_grow_rate_max,life_grow_rate,defense_grow_rate,magicdef_grow_rate,life,atk_min,atk_max,magicatk_min,magicatk_max,defense,magicdef,luck,intimacy,level,card,exp,quality,wuxing,recall_count) values({0},{1},'{2}',{3},{4},{5},{6},'{7}',{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24})";

		// Token: 0x0400001A RID: 26
		public const string UPDATEROLEDATA_EUDEMON = "update cq_eudemon set itemid={0},ownerid={1},name='{2}',phyatk_grow_rate={3},phyatk_grow_rate_max={4},magicatk_grow_rate={5},magicatk_grow_rate_max={6},life_grow_rate={7},defense_grow_rate={8},magicdef_grow_rate={9},life={10},atk_min={11},atk_max={12},magicatk_min={13},magicatk_max={14},defense={15},magicdef={16},luck={17},intimacy={18},level={19},card={20},exp={21},quality={22},wuxing={23},recall_count={24} where id={25}";

		// Token: 0x0400001B RID: 27
		public const string DELETEROLEDATA_EUDEMON = "delete from cq_eudemon where id={0} and ownerid={1}";

		// Token: 0x0400001C RID: 28
		public const string LOADROLEDATA_EUDEMON_MAGIC = "select * from cq_eudemon_magic where ownerid={0}";

		// Token: 0x0400001D RID: 29
		public const string ADD_EUDEMON_MAGIC = "insert into cq_eudemon_magic(ownerid,magicid,level,exp) values({0},{1},{2},{3})";

		// Token: 0x0400001E RID: 30
		public const string UPDATE_EUDEMON_MAGIC = "update cq_eudemon_magic set magicid={0},level={1},exp={2} where ownerid={3} and id={4}";

		// Token: 0x0400001F RID: 31
		public const string DELETE_EUDEMON_MAGIC = "delete from cq_eudemon_magic where magicid={0}";

		// Token: 0x04000020 RID: 32
		public const string LOADROLEDATA_FRIEND = "select * from cq_friend where userid={0}";

		// Token: 0x04000021 RID: 33
		public const string SAVEROLEDATA_FRIEND = "insert into cq_friend(userid,friendtype,friendid,friendname) values({0},{1},{2},'{3}')";

		// Token: 0x04000022 RID: 34
		public const string DELETEROLEDATA_FRIEND = "delete from cq_friend where userid={0} and friendid={1}";

		// Token: 0x04000023 RID: 35
		public const string CREATE_LEGION = "insert into cq_legion(name,member_title,leader_id,leader_name,money,notice) values('{0}',{1},{2},'{3}',{4},'{5}')";

		// Token: 0x04000024 RID: 36
		public const string UPDATE_LEGION = "update cq_legion set name='{0}',member_title={1},leader_id={2},leader_name='{3}',money={4},notice='{5}' where id={6}";

		// Token: 0x04000025 RID: 37
		public const string CREATE_LEGION_MEMBERS = "insert into cq_legion_members(legion_id,members_name,money,rank) values({0},'{1}',{2},{3})";

		// Token: 0x04000026 RID: 38
		public const string UPDATE_LEGION_MEMBERS = "update cq_legion_members set money={0},rank={1} where legion_id={2} and members_name='{3}'";
	}
}
