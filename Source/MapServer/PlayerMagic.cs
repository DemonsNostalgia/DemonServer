using System;
using System.Collections.Generic;
using GameBase.Core;
using GameBase.Network.Internal;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000092 RID: 146
	public class PlayerMagic
	{
		// Token: 0x06000332 RID: 818 RVA: 0x00024968 File Offset: 0x00022B68
		public Dictionary<uint, RoleMagicInfo> GetDicMagic()
		{
			return this.mDicMagic;
		}

		// Token: 0x06000333 RID: 819 RVA: 0x00024980 File Offset: 0x00022B80
		public bool IsLiuXingYunHuo()
		{
			return this.mbLiuXingYunHuo;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00024998 File Offset: 0x00022B98
		public bool CheckAttackSpeed()
		{
			return this.mNormalAttackSpeed.ToNextTime();
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000249C4 File Offset: 0x00022BC4
		public bool CheckMoveSpeed()
		{
			return true;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x000249D7 File Offset: 0x00022BD7
		public void SetMoveSpeed(float fSpeed)
		{
			this.mMoveSpeed.SetInterval(fSpeed);
			this.mMoveSpeed.Update();
		}

		// Token: 0x06000337 RID: 823 RVA: 0x000249F4 File Offset: 0x00022BF4
		public bool CheckMagicAttackSpeed(ushort magicid, byte magiclv)
		{
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo((uint)magicid, magiclv);
			bool result;
			if (magicTypeInfo == null)
			{
				result = false;
			}
			else if (magicTypeInfo.delay_ms == 0U)
			{
				result = true;
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < this.mMagicAttackSpeed.Count; i++)
				{
					TimeOut timeOut = this.mMagicAttackSpeed[i];
					if ((ushort)timeOut.GetObject() == magicid)
					{
						if (timeOut.ToNextTime())
						{
							flag = true;
							break;
						}
						flag2 = true;
					}
				}
				for (int i = 0; i < this.mMagicAttackSpeed.Count; i++)
				{
					this.mMagicAttackSpeed[i].Update();
				}
				if (!flag && !flag2)
				{
					TimeOut timeOut = new TimeOut();
					timeOut.SetInterval(magicTypeInfo.delay_ms);
					timeOut.SetObject(magicid);
					timeOut.Update();
					this.mMagicAttackSpeed.Add(timeOut);
					result = true;
				}
				else
				{
					result = flag;
				}
			}
			return result;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x00024B2C File Offset: 0x00022D2C
		public PlayerMagic(PlayerObject _play)
		{
			this.play = _play;
			this.mDicMagic = new Dictionary<uint, RoleMagicInfo>();
			this.mNormalAttackSpeed = new TimeOut();
			this.mNormalAttackSpeed.SetInterval(1000f);
			this.mNormalAttackSpeed.Update();
			this.mMoveSpeed = new TimeOut();
			this.mMoveSpeed.SetInterval(250f);
			this.mMoveSpeed.Update();
			this.mMagicAttackSpeed = new List<TimeOut>();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00024BB8 File Offset: 0x00022DB8
		public void AddMagicInfo(uint magidid, byte level, uint exp)
		{
			RoleMagicInfo roleMagicInfo = new RoleMagicInfo();
			roleMagicInfo.magicid = magidid;
			roleMagicInfo.level = level;
			roleMagicInfo.exp = exp;
			roleMagicInfo.id = 0;
			this.mDicMagic[magidid] = roleMagicInfo;
			this.SendMagicInfo(roleMagicInfo);
			if (magidid == 5302U)
			{
				this.mbLiuXingYunHuo = true;
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00024C14 File Offset: 0x00022E14
		public void AddMagicInfo(MagicInfo info)
		{
			RoleMagicInfo roleMagicInfo = new RoleMagicInfo();
			roleMagicInfo.magicid = info.magicid;
			roleMagicInfo.level = info.level;
			roleMagicInfo.exp = info.exp;
			roleMagicInfo.id = info.id;
			this.mDicMagic[roleMagicInfo.magicid] = roleMagicInfo;
			if (roleMagicInfo.magicid == 5302U)
			{
				this.mbLiuXingYunHuo = true;
			}
		}

		public bool EnsureMagicLevel(uint magicId, byte level)
		{
			RoleMagicInfo existing;
			if (this.mDicMagic.TryGetValue(magicId, out existing))
			{
				if (existing.level >= level)
				{
					return false;
				}
				existing.level = level;
				existing.exp = 0U;
				this.SendMagicInfo(existing);
				return true;
			}
			this.AddMagicInfo(magicId, level, 0U);
			return true;
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00024C88 File Offset: 0x00022E88
		public void SendMagicInfo(RoleMagicInfo info)
		{
			MsgMagicInfo msgMagicInfo = new MsgMagicInfo();
			msgMagicInfo.Create(null, this.play.GetGamePackKeyEx());
			msgMagicInfo.id = this.play.GetTypeId();
			msgMagicInfo.magicid = (ushort)info.magicid;
			msgMagicInfo.level = (ushort)info.level;
			msgMagicInfo.exp = info.exp;
			this.play.SendData(msgMagicInfo.GetBuffer(), false);
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00024CF8 File Offset: 0x00022EF8
		public void SendAllMagicInfo()
		{
			foreach (RoleMagicInfo info in this.mDicMagic.Values)
			{
				this.SendMagicInfo(info);
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00024D5C File Offset: 0x00022F5C
		public void DB_Save()
		{
			if (this.mDicMagic.Count > 0)
			{
				RoleData_Magic roleData_Magic = new RoleData_Magic();
				roleData_Magic.SetSaveTag();
				roleData_Magic.ownerid = this.play.GetBaseAttr().player_id;
				foreach (RoleMagicInfo roleMagicInfo in this.mDicMagic.Values)
				{
					MagicInfo magicInfo = new MagicInfo();
					magicInfo.id = roleMagicInfo.id;
					magicInfo.magicid = roleMagicInfo.magicid;
					magicInfo.level = roleMagicInfo.level;
					magicInfo.exp = roleMagicInfo.exp;
					roleData_Magic.mListMagic.Add(magicInfo);
				}
				DBServer.Instance().GetDBClient().SendData(roleData_Magic.GetBuffer());
			}
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00024E4C File Offset: 0x0002304C
		public ushort GetMagicLevel(uint typeid)
		{
			ushort result;
			if (this.mDicMagic.ContainsKey(typeid))
			{
				RoleMagicInfo roleMagicInfo = this.mDicMagic[typeid];
				result = (ushort)roleMagicInfo.level;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00024E8C File Offset: 0x0002308C
		public void AddMagicExp(uint typeid, uint exp)
		{
			if (this.mDicMagic.ContainsKey(typeid))
			{
				MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(typeid, 0);
				if (magicTypeInfo != null)
				{
					if (magicTypeInfo.need_exp != 0U)
					{
						this.mDicMagic[typeid].exp += exp;
						if (this.mDicMagic[typeid].exp >= magicTypeInfo.need_exp)
						{
							RoleMagicInfo roleMagicInfo = this.mDicMagic[typeid];
							roleMagicInfo.level += 1;
							this.mDicMagic[typeid].exp = 0U;
						}
						this.SendMagicInfo(this.mDicMagic[typeid]);
					}
				}
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00024F58 File Offset: 0x00023158
		public bool isMagic(uint typeid)
		{
			return this.mDicMagic.ContainsKey(typeid);
		}

		// Token: 0x0400061A RID: 1562
		private PlayerObject play;

		// Token: 0x0400061B RID: 1563
		private Dictionary<uint, RoleMagicInfo> mDicMagic;

		// Token: 0x0400061C RID: 1564
		private bool mbLiuXingYunHuo = false;

		// Token: 0x0400061D RID: 1565
		public TimeOut mNormalAttackSpeed;

		// Token: 0x0400061E RID: 1566
		private TimeOut mMoveSpeed;

		// Token: 0x0400061F RID: 1567
		private List<TimeOut> mMagicAttackSpeed;
	}
}
