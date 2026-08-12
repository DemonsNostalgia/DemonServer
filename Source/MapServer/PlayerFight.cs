using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x0200008E RID: 142
	public class PlayerFight
	{
		// Token: 0x060002D3 RID: 723 RVA: 0x0001DA10 File Offset: 0x0001BC10
		public void RemoveQiShiTuanGuardEffect()
		{
			if (this.mListQiShiTuanGuard != null)
			{
				for (int i = 0; i < this.mListQiShiTuanGuard.Count; i++)
				{
					BaseObject baseObject = this.mListQiShiTuanGuard[i];
					baseObject.ClearThis();
					baseObject.GetGameMap().RemoveObj(baseObject);
				}
				this.mListQiShiTuanGuard.Clear();
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0001DA7C File Offset: 0x0001BC7C
		public PlayerFight(PlayerObject _play)
		{
			this.play = _play;
			this.mAutoTarget = null;
			this.mnAutoAttackTick = Environment.TickCount;
			this.mnLastAttackTick = Environment.TickCount;
			this.mnYanHunQiangIndex = (this.mnYanHunQiangExIndex = 0);
			this.mListQiShiTuanGuard = null;
			this.mLiuXingYunHuoTime = null;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001DAF0 File Offset: 0x0001BCF0
		public bool PassiveMagic(MsgAttackInfo info)
		{
			bool result;
			if (this.mAutoTarget == null)
			{
				result = false;
			}
			else if (IRandom.Random(1, 100) > 50)
			{
				result = false;
			}
			else
			{
				byte job = this.play.GetJob();
				byte b = job;
				if (b == 20)
				{
					uint[] array = new uint[]
					{
						1000U,
						1002U,
						1005U,
						1009U
					};
					List<RoleMagicInfo> list = null;
					Dictionary<uint, RoleMagicInfo> dicMagic = this.play.GetMagicSystem().GetDicMagic();
					foreach (RoleMagicInfo roleMagicInfo in dicMagic.Values)
					{
						for (int i = 0; i < array.Length; i++)
						{
							if (roleMagicInfo.magicid == array[i])
							{
								if (list == null)
								{
									list = new List<RoleMagicInfo>();
								}
								list.Add(roleMagicInfo);
								break;
							}
						}
					}
					if (list == null)
					{
						return false;
					}
					int index = IRandom.Random(0, list.Count);
					MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(list[index].magicid, list[index].level);
					if ((long)IRandom.Random(1, 100) < (long)((ulong)magicTypeInfo.percent))
					{
						info.usType = magicTypeInfo.typeid;
						info.idTarget = this.mAutoTarget.GetTypeId();
						this.MagicAttack(info);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001DCAC File Offset: 0x0001BEAC
		public void Attack(MsgAttackInfo info)
		{
			if (this.play.GetMagicSystem().CheckAttackSpeed())
			{
				BaseObject baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
				if (baseObject != null)
				{
					if (!baseObject.IsDie())
					{
						if (!baseObject.IsLock())
						{
							if (Math.Abs((int)(this.play.GetCurrentX() - baseObject.GetCurrentX())) > 3 && Math.Abs((int)(this.play.GetCurrentY() - baseObject.GetCurrentY())) > 3)
							{
								this.SetAutoAttackTarget(null);
							}
							else
							{
								this.SetAutoAttackTarget(baseObject);
								if (!this.PassiveMagic(info))
								{
									uint num = BattleSystem.AdjustDamage(this.play, baseObject, false);
									num = BattleSystem.AdjustDamage(this.play, baseObject, true);
									byte[] buffer = new MsgMonsterInjuredInfo
									{
										roleid = this.play.GetTypeId(),
										role_x = this.play.GetCurrentX(),
										role_y = this.play.GetCurrentY(),
										injuredvalue = num,
										monsterid = baseObject.GetTypeId(),
										tag = 2U
									}.GetBuffer();
									this.play.BroadcastBuffer(buffer, true);
									baseObject.Injured(this.play, num, info);
									this.play.CanPK(baseObject, true);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001DE38 File Offset: 0x0001C038
		public void Run()
		{
			this.AutoAttack();
			if (this.play.GetJob() == 10)
			{
				if (this.play.GetMagicSystem().IsLiuXingYunHuo() && this.mnLiuXingYunHuoCount < 7 && this.play.GetTimerSystem().QueryStatus(102) != null)
				{
					if (this.mLiuXingYunHuoTime == null)
					{
						this.mLiuXingYunHuoTime = new TimeOut();
						this.mLiuXingYunHuoTime.SetInterval(5);
						this.mnLiuXingYunHuoCount++;
						this.play.ChangeAttribute(UserAttribute.LIUXINGYUNHUO, this.mnLiuXingYunHuoCount, true);
					}
					if (this.mLiuXingYunHuoTime.ToNextTime())
					{
						this.mnLiuXingYunHuoCount++;
						this.play.ChangeAttribute(UserAttribute.LIUXINGYUNHUO, this.mnLiuXingYunHuoCount, true);
					}
				}
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001DF29 File Offset: 0x0001C129
		public void SetAutoAttackTarget(BaseObject obj)
		{
			this.mAutoTarget = obj;
			this.mnAutoAttackTick = Environment.TickCount;
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0001DF40 File Offset: 0x0001C140
		private void AutoAttack()
		{
			if (this.mAutoTarget != null)
			{
				if (this.mAutoTarget.IsDie())
				{
					this.mAutoTarget = null;
				}
			}
			if (!this.play.IsDie() && !this.play.IsLock())
			{
				if (this.mAutoTarget != null && !this.mAutoTarget.IsLock())
				{
					if (Environment.TickCount - this.mnAutoAttackTick > 1500)
					{
						MsgAttackInfo msgAttackInfo = new MsgAttackInfo();
						if (!this.PassiveMagic(msgAttackInfo))
						{
							msgAttackInfo.idTarget = this.mAutoTarget.GetTypeId();
							this.Attack(msgAttackInfo);
							this.mnAutoAttackTick = Environment.TickCount;
						}
					}
				}
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x0001E03C File Offset: 0x0001C23C
		public void MagicAttack(MsgAttackInfo info)
		{
			if (this.play.GetMagicSystem().isMagic(info.usType))
			{
				ushort magicLevel = this.play.GetMagicSystem().GetMagicLevel(info.usType);
				if (this.play.GetMagicSystem().CheckMagicAttackSpeed((ushort)info.usType, (byte)magicLevel))
				{
					if (info.usType == 5214U)
					{
						uint num = info.usType + (uint)this.mnYanHunQiangIndex;
						if (!this.play.GetMagicSystem().isMagic(num))
						{
							this.mnYanHunQiangIndex = 0;
							num = info.usType;
						}
						info.usType = num;
						this.mnYanHunQiangIndex += 1;
						if (this.mnYanHunQiangIndex >= 3)
						{
							this.mnYanHunQiangIndex = 0;
						}
					}
					if (info.usType == 5217U)
					{
						uint num = info.usType + (uint)this.mnYanHunQiangExIndex;
						if (!this.play.GetMagicSystem().isMagic(num))
						{
							this.mnYanHunQiangExIndex = 0;
							num = info.usType;
						}
						info.usType = num;
						this.mnYanHunQiangExIndex += 1;
						if (this.mnYanHunQiangExIndex >= 4)
						{
							this.mnYanHunQiangExIndex = 0;
						}
					}
					MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(info.usType, 0);
					if (magicTypeInfo != null)
					{
						BaseObject baseObject = null;
						if (magicTypeInfo.use_xp <= 0U || this.play.GetTimerSystem().QueryStatus(47) != null)
						{
							if (magicTypeInfo.need_exp > 0U && (uint)this.play.GetBaseAttr().level >= magicTypeInfo.need_level)
							{
								this.play.GetMagicSystem().AddMagicExp(info.usType, 1U);
							}
							if (magicTypeInfo.use_ep <= 0U || (long)this.play.GetBaseAttr().sp >= (long)((ulong)magicTypeInfo.use_ep))
							{
								if (magicTypeInfo.use_mp <= 0U || this.play.GetBaseAttr().mana >= magicTypeInfo.use_mp)
								{
									if (magicTypeInfo.use_ep > 0U && (long)this.play.GetBaseAttr().sp > (long)((ulong)magicTypeInfo.use_ep))
									{
										this.play.ChangeAttribute(UserAttribute.SP, (int)(-(int)((ulong)magicTypeInfo.use_ep)), true);
									}
									if (magicTypeInfo.use_mp > 0U && this.play.GetBaseAttr().mana > magicTypeInfo.use_mp)
									{
										this.play.ChangeAttribute(UserAttribute.MANA, (int)(-(int)((ulong)magicTypeInfo.use_mp)), true);
									}
									byte sort = magicTypeInfo.sort;
									byte dirByPos;
									MsgGroupMagicAttackInfo msgGroupMagicAttackInfo;
									List<BaseObject> list;
									uint num2;
									byte[] buffer;
									MsgMonsterMagicInjuredInfo msgMonsterMagicInjuredInfo;
									if (sort <= 14)
									{
										switch (sort)
										{
										case 1:
											break;
										case 2:
										case 3:
											return;
										case 4:
											dirByPos = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos);
											msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
											msgGroupMagicAttackInfo.nID = this.play.GetTypeId();
											msgGroupMagicAttackInfo.nX = this.play.GetCurrentX();
											msgGroupMagicAttackInfo.nY = this.play.GetCurrentY();
											msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
											msgGroupMagicAttackInfo.nMagicLv = magicLevel;
											msgGroupMagicAttackInfo.bDir = dirByPos;
											list = this.RefreshMagicVisibleObject(magicTypeInfo.typeid, info);
											if (list != null)
											{
												for (int i = 0; i < list.Count; i++)
												{
													num2 = BattleSystem.AdjustDamage(this.play, list[i], true);
													if (list[i].type == 3 && magicTypeInfo.use_xp > 0U)
													{
														num2 *= 10U;
													}
													list[i].Injured(this.play, num2, info);
													msgGroupMagicAttackInfo.AddObject(list[i].GetTypeId(), (int)num2);
												}
											}
											buffer = msgGroupMagicAttackInfo.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											return;
										case 5:
											goto IL_6A2;
										case 6:
										{
											foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
											{
												BaseObject obj = refreshObject.obj;
												if (obj.type == 3)
												{
													if ((obj as MonsterObject).GetAi().GetTargetObject() == null)
													{
														(obj as MonsterObject).GetAi().SetAttackTarget(this.play);
													}
												}
											}
											byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos2);
											buffer = new MsgMonsterMagicInjuredInfo
											{
												roleid = this.play.GetTypeId(),
												role_x = this.play.GetCurrentX(),
												role_y = this.play.GetCurrentY(),
												injuredvalue = 0U,
												monsterid = this.play.GetTypeId(),
												tag = 21U,
												magicid = (ushort)info.usType,
												magiclv = magicLevel
											}.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
											msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
											msgMagicAttackInfo.magicid = (ushort)info.usType;
											msgMagicAttackInfo.level = magicLevel;
											msgMagicAttackInfo.dir = dirByPos2;
											buffer = msgMagicAttackInfo.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											return;
										}
										default:
										{
											if (sort != 14)
											{
												return;
											}
											byte dirByPos3 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos3);
											msgMonsterMagicInjuredInfo = new MsgMonsterMagicInjuredInfo();
											msgMonsterMagicInjuredInfo.roleid = this.play.GetTypeId();
											msgMonsterMagicInjuredInfo.role_x = this.play.GetCurrentX();
											msgMonsterMagicInjuredInfo.role_y = this.play.GetCurrentY();
											msgMonsterMagicInjuredInfo.tag = 21U;
											msgMonsterMagicInjuredInfo.magicid = (ushort)info.usType;
											msgMonsterMagicInjuredInfo.magiclv = magicLevel;
											this.play.BroadcastBuffer(msgMonsterMagicInjuredInfo.GetBuffer(), true);
											msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
											msgGroupMagicAttackInfo.nID = this.play.GetTypeId();
											msgGroupMagicAttackInfo.nX = (short)info.usPosX;
											msgGroupMagicAttackInfo.nY = (short)info.usPosY;
											msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
											msgGroupMagicAttackInfo.nMagicLv = magicLevel;
											msgGroupMagicAttackInfo.bDir = this.play.GetDir();
											list = this.RefreshMagicVisibleObject(magicTypeInfo.typeid, info);
											if (list != null)
											{
												for (int i = 0; i < list.Count; i++)
												{
													num2 = BattleSystem.AdjustDamage(this.play, list[i], true);
													if (list[i].type == 3 && magicTypeInfo.use_xp > 0U)
													{
														num2 *= 10U;
													}
													list[i].Injured(this.play, num2, info);
													msgGroupMagicAttackInfo.AddObject(list[i].GetTypeId(), (int)num2);
												}
											}
											this.play.BroadcastBuffer(msgGroupMagicAttackInfo.GetBuffer(), true);
											return;
										}
										}
									}
									else
									{
										switch (sort)
										{
										case 40:
										{
											byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos2);
											buffer = new MsgMonsterMagicInjuredInfo
											{
												roleid = this.play.GetTypeId(),
												role_x = this.play.GetCurrentX(),
												role_y = this.play.GetCurrentY(),
												injuredvalue = 0U,
												monsterid = this.play.GetTypeId(),
												tag = 21U,
												magicid = (ushort)info.usType,
												magiclv = magicLevel
											}.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
											msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
											msgMagicAttackInfo.magicid = (ushort)info.usType;
											msgMagicAttackInfo.level = magicLevel;
											msgMagicAttackInfo.dir = dirByPos2;
											buffer = msgMagicAttackInfo.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											this.play.GetTimerSystem().AddStatus(99, 1800, true);
											return;
										}
										case 41:
											goto IL_6A2;
										case 42:
										{
											byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos2);
											buffer = new MsgMonsterMagicInjuredInfo
											{
												roleid = this.play.GetTypeId(),
												role_x = this.play.GetCurrentX(),
												role_y = this.play.GetCurrentY(),
												injuredvalue = 0U,
												monsterid = this.play.GetTypeId(),
												tag = 21U,
												magicid = (ushort)info.usType,
												magiclv = magicLevel
											}.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
											msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
											msgMagicAttackInfo.magicid = (ushort)info.usType;
											msgMagicAttackInfo.level = magicLevel;
											msgMagicAttackInfo.dir = dirByPos2;
											buffer = msgMagicAttackInfo.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(10795U);
											if (monsterInfo == null)
											{
												Log.Instance().WriteLog("Failed to create Guard Knight; monster ID does not exist.");
												return;
											}
											if (this.mListQiShiTuanGuard == null)
											{
												this.mListQiShiTuanGuard = new List<BaseObject>();
											}
											this.RemoveQiShiTuanGuardEffect();
											short[] array = new short[]
											{
												-5,
												-5,
												5,
												5
											};
											short[] array2 = new short[]
											{
												5,
												-5,
												-5,
												5
											};
											byte[] array3 = new byte[]
											{
												0,
												2,
												4,
												6
											};
											for (int i = 0; i < array.Length; i++)
											{
												short x = (short)(this.play.GetCurrentX() + array[i]);
												short y = (short)(this.play.GetCurrentY() + array2[i]);
												GuardKnightObject guardKnightObject = new GuardKnightObject(this.play, x, y, array3[i], monsterInfo.id, monsterInfo.ai);
												this.play.GetGameMap().AddObject(guardKnightObject, null);
												guardKnightObject.RefreshVisibleObject();
												guardKnightObject.SendInfo(this.play);
												this.mListQiShiTuanGuard.Add(guardKnightObject);
												this.play.AddVisibleObject(guardKnightObject, true);
											}
											EffectObject effectObject = new EffectObject(this.play, 6064, 10, 15, 120, this.play.GetCurrentX(), this.play.GetCurrentY());
											this.play.GetGameMap().AddObject(effectObject, null);
											effectObject.RefreshVisibleObject();
											effectObject.SendInfo(this.play, false);
											this.mListQiShiTuanGuard.Add(effectObject);
											this.play.AddVisibleObject(effectObject, true);
											return;
										}
										case 43:
										{
											byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
											this.play.SetDir(dirByPos2);
											buffer = new MsgMonsterMagicInjuredInfo
											{
												roleid = this.play.GetTypeId(),
												role_x = this.play.GetCurrentX(),
												role_y = this.play.GetCurrentY(),
												injuredvalue = 0U,
												monsterid = this.play.GetTypeId(),
												tag = 21U,
												magicid = (ushort)info.usType,
												magiclv = magicLevel
											}.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
											msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
											msgMagicAttackInfo.magicid = (ushort)info.usType;
											msgMagicAttackInfo.level = magicLevel;
											msgMagicAttackInfo.dir = dirByPos2;
											buffer = msgMagicAttackInfo.GetBuffer();
											this.play.BroadcastBuffer(buffer, true);
											MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(10836U);
											if (monsterInfo == null)
											{
												Log.Instance().WriteLog("Failed to create Dark Knight; monster ID does not exist.");
												return;
											}
											short[] array = new short[]
											{
												-5,
												-5,
												5,
												5
											};
											short[] array2 = new short[]
											{
												5,
												-5,
												-5,
												5
											};
											byte[] array4 = new byte[]
											{
												0,
												2,
												4,
												6
											};
											for (int i = 0; i < 4; i++)
											{
												short x = (short)(this.play.GetCurrentX() + array[i]);
												short y = (short)(this.play.GetCurrentY() + array2[i]);
												FightKnightObject fightKnightObject = new FightKnightObject(x, y, this.play.GetDir(), monsterInfo.id, 10);
												this.play.GetGameMap().AddObject(fightKnightObject, null);
												fightKnightObject.RefreshVisibleObject();
												fightKnightObject.SendInfo(this.play);
												this.play.AddVisibleObject(fightKnightObject, true);
											}
											return;
										}
										default:
											switch (sort)
											{
											case 81:
												break;
											case 82:
												goto IL_6A2;
											case 83:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(100, 60, true);
												return;
											}
											case 84:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												if (this.play.GetTimerSystem().QueryStatus(14) != null)
												{
													this.play.GetTimerSystem().DeleteStatus(14);
												}
												else
												{
													this.play.GetTimerSystem().AddStatus(14, 30, true);
												}
												return;
											}
											case 85:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												if (this.play.GetTimerSystem().QueryStatus(102) != null)
												{
													this.play.GetTimerSystem().DeleteStatus(102);
													this.mnLiuXingYunHuoCount = 0;
													this.play.ChangeAttribute(UserAttribute.LIUXINGYUNHUO, this.mnLiuXingYunHuoCount, true);
												}
												else
												{
													this.play.GetTimerSystem().AddStatus(102, 0, true);
												}
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												return;
											}
											case 86:
											{
												baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
												if (baseObject == null)
												{
													return;
												}
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												if (this.mnLiuXingYunHuoCount <= 0)
												{
													return;
												}
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												for (int i = 0; i < this.mnLiuXingYunHuoCount; i++)
												{
													num2 = BattleSystem.AdjustDamage(this.play, baseObject, true);
													msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
													msgGroupMagicAttackInfo.SetSigleAttack(baseObject.GetTypeId());
													msgGroupMagicAttackInfo.nID = this.play.GetTypeId();
													msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
													msgGroupMagicAttackInfo.nMagicLv = magicLevel;
													msgGroupMagicAttackInfo.bDir = this.play.GetDir();
													msgGroupMagicAttackInfo.AddObject(baseObject.GetTypeId(), (int)num2);
													this.play.BroadcastBuffer(msgGroupMagicAttackInfo.GetBuffer(), true);
													baseObject.Injured(this.play, num2, info);
												}
												this.mnLiuXingYunHuoCount = 0;
												this.play.ChangeAttribute(UserAttribute.LIUXINGYUNHUO, this.mnLiuXingYunHuoCount, true);
												return;
											}
											case 87:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(103, 0, true);
												return;
											}
											case 88:
												if (this.play.IsMountState())
												{
													this.play.TakeOffMount(0U);
												}
												else
												{
													this.play.TakeMount(0U, 1072540U);
												}
												return;
											case 89:
											case 91:
												return;
											case 90:
											{
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = this.play.GetDir();
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												if (this.play.GetTimerSystem().QueryStatus(1008) != null)
												{
													this.play.GetTimerSystem().DeleteStatus(1008);
												}
												else
												{
													this.play.GetTimerSystem().AddStatus(1008, 0, true);
												}
												return;
											}
											case 92:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												EffectObject effectObject = new EffectObject(this.play, 5678, 10, 14, 5, (short)info.usPosX, (short)info.usPosY);
												this.play.GetGameMap().AddObject(effectObject, null);
												effectObject.RefreshVisibleObject();
												effectObject.SendInfo(this.play, false);
												return;
											}
											case 93:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(105, 0, true);
												return;
											}
											case 94:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(106, 0, true);
												return;
											}
											case 95:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(107, 0, true);
												return;
											}
											case 96:
											case 97:
											case 98:
											{
												baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
												if (baseObject == null)
												{
													return;
												}
												uint id = 1434U;
												if (magicTypeInfo.sort == 97)
												{
													id = 1433U;
												}
												else if (magicTypeInfo.sort == 98)
												{
													id = 1432U;
												}
												MonsterInfo monsterInfo2 = ConfigManager.Instance().GetMonsterInfo(id);
												if (monsterInfo2 == null)
												{
													Log.Instance().WriteLog("Failed to get the Abyssal Spirit monster ID.");
													return;
												}
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												int num3 = (int)(baseObject.GetCurrentX() - DIR._DELTA_X[(int)this.play.GetDir()]);
												int num4 = (int)(baseObject.GetCurrentY() - DIR._DELTA_Y[(int)this.play.GetDir()]);
												MonsterObject monsterObject = null;
												if (magicTypeInfo.sort == 97)
												{
													monsterObject = new DiYuXieFu(this.play, baseObject, (short)num3, (short)num4, this.play.GetDir(), monsterInfo2.id, monsterInfo2.ai);
												}
												else if (magicTypeInfo.sort == 98)
												{
													monsterObject = new ShiHunWuLing(this.play, baseObject, (short)num3, (short)num4, this.play.GetDir(), monsterInfo2.id, monsterInfo2.ai);
												}
												else if (magicTypeInfo.sort == 96)
												{
													monsterObject = new ShenYuanELing(this.play, baseObject, (short)num3, (short)num4, this.play.GetDir(), monsterInfo2.id, monsterInfo2.ai);
												}
												this.play.GetGameMap().AddObject(monsterObject, null);
												monsterObject.Alive(false);
												return;
											}
											case 99:
												if (this.play.IsMountState())
												{
													this.play.TakeOffMount(0U);
												}
												else
												{
													this.play.TakeMount(0U, 1072240U);
												}
												return;
											case 100:
											{
												byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
												this.play.SetDir(dirByPos2);
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = dirByPos2;
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.GetTimerSystem().AddStatus(120, 120, true);
												return;
											}
											case 101:
											case 102:
											{
												buffer = new MsgMonsterMagicInjuredInfo
												{
													roleid = this.play.GetTypeId(),
													role_x = this.play.GetCurrentX(),
													role_y = this.play.GetCurrentY(),
													injuredvalue = 0U,
													monsterid = this.play.GetTypeId(),
													tag = 21U,
													magicid = (ushort)info.usType,
													magiclv = magicLevel
												}.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
												msgMagicAttackInfo.id = (msgMagicAttackInfo.targetid = this.play.GetTypeId());
												msgMagicAttackInfo.magicid = (ushort)info.usType;
												msgMagicAttackInfo.level = magicLevel;
												msgMagicAttackInfo.dir = this.play.GetDir();
												buffer = msgMagicAttackInfo.GetBuffer();
												this.play.BroadcastBuffer(buffer, true);
												this.play.SetDancing((short)info.usType);
												return;
											}
											default:
												return;
											}
											break;
										}
									}
									baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
									if (baseObject == null)
									{
										return;
									}
									if (baseObject.IsDie())
									{
										return;
									}
									if (baseObject.IsLock())
									{
										return;
									}
									dirByPos = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), baseObject.GetCurrentX(), baseObject.GetCurrentY());
									this.play.SetDir(dirByPos);
									if ((long)Math.Abs((int)(this.play.GetCurrentX() - baseObject.GetCurrentY())) > (long)((ulong)magicTypeInfo.distance) && (long)Math.Abs((int)(this.play.GetCurrentY() - baseObject.GetCurrentY())) > (long)((ulong)magicTypeInfo.distance))
									{
										return;
									}
									if (!this.play.CanPK(baseObject, true))
									{
										return;
									}
									if (this.IsComboMagic(magicTypeInfo.typeid))
									{
										this.ComboMagic(info, baseObject);
										if (info.usType == 7007U)
										{
											this.play.GetTimerSystem().AddStatus(1009, 60, true);
										}
										return;
									}
									num2 = BattleSystem.AdjustDamage(this.play, baseObject, true);
									if (baseObject.type == 3 && magicTypeInfo.use_xp > 0U)
									{
										num2 *= 10U;
									}
									msgMonsterMagicInjuredInfo = new MsgMonsterMagicInjuredInfo();
									msgMonsterMagicInjuredInfo.time = Environment.TickCount;
									msgMonsterMagicInjuredInfo.roleid = this.play.GetTypeId();
									msgMonsterMagicInjuredInfo.role_x = this.play.GetCurrentX();
									msgMonsterMagicInjuredInfo.role_y = this.play.GetCurrentY();
									msgMonsterMagicInjuredInfo.monsterid = baseObject.GetTypeId();
									msgMonsterMagicInjuredInfo.tag = 21U;
									msgMonsterMagicInjuredInfo.magicid = (ushort)info.usType;
									msgMonsterMagicInjuredInfo.magiclv = magicLevel;
									this.play.BroadcastBuffer(msgMonsterMagicInjuredInfo.GetBuffer(), true);
									msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
									msgGroupMagicAttackInfo.SetSigleAttack(baseObject.GetTypeId());
									msgGroupMagicAttackInfo.nID = this.play.GetTypeId();
									msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
									msgGroupMagicAttackInfo.nMagicLv = magicLevel;
									msgGroupMagicAttackInfo.bDir = this.play.GetDir();
									msgGroupMagicAttackInfo.AddObject(baseObject.GetTypeId(), (int)num2);
									this.play.BroadcastBuffer(msgGroupMagicAttackInfo.GetBuffer(), true);
									baseObject.Injured(this.play, num2, info);
									if (magicTypeInfo.sort == 81)
									{
										int num3 = (int)(baseObject.GetCurrentX() - (DIR._DELTA_X[(int)dirByPos] + DIR._DELTA_X[(int)dirByPos]));
										int num4 = (int)(baseObject.GetCurrentY() - (DIR._DELTA_Y[(int)dirByPos] + DIR._DELTA_Y[(int)dirByPos]));
										this.play.SetPoint((short)num3, (short)num4);
									}
									return;
									IL_6A2:
									dirByPos = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)info.usPosX, (short)info.usPosY);
									this.play.SetDir(dirByPos);
									if (magicTypeInfo.typeid == 6017U)
									{
										if (baseObject == null)
										{
											return;
										}
										int num3 = (int)(baseObject.GetCurrentX() - (DIR._DELTA_X[(int)dirByPos] + DIR._DELTA_X[(int)dirByPos]));
										int num4 = (int)(baseObject.GetCurrentY() - (DIR._DELTA_Y[(int)dirByPos] + DIR._DELTA_Y[(int)dirByPos]));
										this.play.SetPoint((short)num3, (short)num4);
										baseObject = this.play.GetGameMap().FindObjectForID(info.idTarget);
									}
									if (magicTypeInfo.sort == 82)
									{
										if ((long)Math.Abs((int)(this.play.GetCurrentX() - (short)info.usPosX)) > (long)((ulong)magicTypeInfo.distance) && (long)Math.Abs((int)(this.play.GetCurrentY() - (short)info.usPosY)) > (long)((ulong)magicTypeInfo.distance))
										{
											return;
										}
										this.play.SetPoint((short)info.usPosX, (short)info.usPosY);
									}
									msgGroupMagicAttackInfo = new MsgGroupMagicAttackInfo();
									msgGroupMagicAttackInfo.nID = this.play.GetTypeId();
									msgGroupMagicAttackInfo.nX = this.play.GetCurrentX();
									msgGroupMagicAttackInfo.nY = this.play.GetCurrentY();
									if (magicTypeInfo.sort == 41)
									{
										msgGroupMagicAttackInfo.nX = (short)info.usPosX;
										msgGroupMagicAttackInfo.nY = (short)info.usPosY;
									}
									msgGroupMagicAttackInfo.nMagicID = (ushort)info.usType;
									msgGroupMagicAttackInfo.nMagicLv = magicLevel;
									msgGroupMagicAttackInfo.bDir = this.play.GetDir();
									list = this.RefreshMagicVisibleObject(magicTypeInfo.typeid, info);
									if (list != null)
									{
										for (int i = 0; i < list.Count; i++)
										{
											num2 = BattleSystem.AdjustDamage(this.play, list[i], true);
											if (list[i].type == 3 && magicTypeInfo.use_xp > 0U)
											{
												num2 *= 10U;
											}
											list[i].Injured(this.play, num2, info);
											msgGroupMagicAttackInfo.AddObject(list[i].GetTypeId(), (int)num2);
										}
									}
									buffer = msgGroupMagicAttackInfo.GetBuffer();
									this.play.BroadcastBuffer(buffer, true);
									if (magicTypeInfo.typeid == 7014U)
									{
										int num5 = 0;
										int num6 = 0;
										switch (dirByPos)
										{
										case 0:
										case 4:
											num5 = 10;
											num6 = 15;
											break;
										case 1:
										case 3:
										case 5:
										case 7:
											num6 = 10;
											num5 = 10;
											break;
										case 2:
										case 6:
											num5 = 15;
											num6 = 10;
											break;
										}
										int num3 = (int)this.play.GetCurrentX() + (int)DIR._DELTA_X[(int)dirByPos] * num5;
										int num4 = (int)this.play.GetCurrentY() + (int)DIR._DELTA_Y[(int)dirByPos] * num6;
										this.play.SetPoint((short)num3, (short)num4);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00020944 File Offset: 0x0001EB44
		private void ComboMagic(MsgAttackInfo info, BaseObject target)
		{
			ushort magicLevel = this.play.GetMagicSystem().GetMagicLevel((uint)info.skillid);
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(info.usType, 0);
			byte[] buffer = new MsgMonsterMagicInjuredInfo
			{
				roleid = this.play.GetTypeId(),
				role_x = this.play.GetCurrentX(),
				role_y = this.play.GetCurrentY(),
				injuredvalue = 0U,
				monsterid = this.play.GetTypeId(),
				tag = 21U,
				magicid = (ushort)info.usType,
				magiclv = magicLevel
			}.GetBuffer();
			this.play.BroadcastBuffer(buffer, false);
			int trackTime = ConfigManager.Instance().GetTrackTime(magicTypeInfo.track_id);
			int trackTime2 = ConfigManager.Instance().GetTrackTime(magicTypeInfo.track_id2);
			int trackNumber = ConfigManager.Instance().GetTrackNumber(magicTypeInfo.track_id);
			this.play.Lock(trackTime, true);
			target.Lock(trackTime2, target.type == 2);
			for (int i = 0; i < trackNumber; i++)
			{
				uint typeId = target.GetTypeId();
				if (target.type == 2)
				{
					EudemonObject injuredEudemon = (target as PlayerObject).GetEudemonSystem().GetInjuredEudemon();
					if (injuredEudemon != null)
					{
						typeId = injuredEudemon.GetTypeId();
					}
				}
				MsgMagicAttackInfo msgMagicAttackInfo = new MsgMagicAttackInfo();
				msgMagicAttackInfo.id = this.play.GetTypeId();
				uint value = BattleSystem.AdjustDamage(this.play, target, false);
				msgMagicAttackInfo.value = value;
				msgMagicAttackInfo.magicid = (ushort)info.usType;
				msgMagicAttackInfo.level = magicLevel;
				msgMagicAttackInfo.targetid = typeId;
				buffer = msgMagicAttackInfo.GetBuffer();
				this.play.BroadcastBuffer(buffer, true);
				target.Injured(this.play, value, info);
			}
			if (magicTypeInfo.track_id > 0U)
			{
				byte dirByPos = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), target.GetCurrentX(), target.GetCurrentY());
				this.play.SetDir(dirByPos);
				target.SetDir(dirByPos);
				MsgCombo msgCombo = new MsgCombo();
				msgCombo.CalcTag(info.usType, this.play, target);
				short x = 0;
				short y = 0;
				TrackInfo trackInfo = ConfigManager.Instance().GetTrackInfo(magicTypeInfo.track_id);
				TrackInfo trackInfo2 = ConfigManager.Instance().GetTrackInfo(magicTypeInfo.track_id2);
				for (int i = 0; i < trackNumber; i++)
				{
					if (trackInfo2.step > 0)
					{
						if (DIR.GetNexPoint(target, ref x, ref y))
						{
							target.SetPoint(x, y);
						}
					}
					if (trackInfo.step > 0)
					{
						for (int j = 0; j < (int)trackInfo.step; j++)
						{
							if (DIR.GetNexPoint(this.play, ref x, ref y))
							{
								this.play.SetPoint(x, y);
							}
						}
					}
					msgCombo.AddComboInfo(info.usType, this.play, target, trackInfo.action, trackInfo2.action);
					trackInfo = ConfigManager.Instance().GetTrackInfo(trackInfo.id_next);
					if (trackInfo2.id_next != 0U)
					{
						trackInfo2 = ConfigManager.Instance().GetTrackInfo(trackInfo2.id_next);
					}
				}
				buffer = msgCombo.GetBuffer();
				this.play.BroadcastBuffer(buffer, true);
			}
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00020CF0 File Offset: 0x0001EEF0
		public List<BaseObject> RefreshMagicVisibleObject(uint magicid, MsgAttackInfo magicinfo)
		{
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicid, 0);
			List<BaseObject> list = new List<BaseObject>();
			list.Clear();
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				short currentX = this.play.GetCurrentX();
				short currentY = this.play.GetCurrentY();
				byte sort = magicTypeInfo.sort;
				if (sort <= 14)
				{
					switch (sort)
					{
					case 4:
						list = this.GetFanVisibleObj(magicinfo);
						goto IL_AD;
					case 5:
						break;
					default:
						if (sort != 14)
						{
							goto IL_AD;
						}
						list = this.GetLineVisibleObj(magicinfo);
						goto IL_AD;
					}
				}
				else
				{
					if (sort == 41)
					{
						list = this.GetPointBombVisibleObj(magicinfo);
						goto IL_AD;
					}
					if (sort != 82)
					{
						goto IL_AD;
					}
				}
				list = this.GetBombVisibleObj(magicinfo);
				IL_AD:
				result = list;
			}
			return result;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00020DB4 File Offset: 0x0001EFB4
		private List<BaseObject> GetFanVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int num = (int)(magicTypeInfo.range + 2U);
				int num2 = num * 2 + 1;
				int width = (int)magicTypeInfo.width;
				foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.IsAddMagicVisibleObj(obj))
					{
						Point point = this.play.GetPoint();
						Point point2 = new Point();
						point2.x = (short)magicinfo.usPosX;
						point2.y = (short)magicinfo.usPosY;
						if (this.play.GetPoint().CheckFanDistance(obj.GetPoint(), point2, num))
						{
							list.Add(obj);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x00020EE0 File Offset: 0x0001F0E0
		private int POS2INDEX(int x, int y, int cx, int cy)
		{
			return x + y * cx;
		}

		// Token: 0x060002DF RID: 735 RVA: 0x00020EF8 File Offset: 0x0001F0F8
		private int CutTrail(int x, int y)
		{
			return (x >= y) ? x : y;
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x00020F14 File Offset: 0x0001F114
		public bool IsAddMagicVisibleObj(BaseObject obj)
		{
			return !obj.IsDie() && !obj.IsLock() && (obj.type == 2 || obj.type == 3) && this.play.CanPK(obj, false);
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x00020F70 File Offset: 0x0001F170
		private List<BaseObject> GetPointBombVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int range = (int)magicTypeInfo.range;
				Point point = new Point();
				point.x = (short)magicinfo.usPosX;
				point.y = (short)magicinfo.usPosY;
				foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.IsAddMagicVisibleObj(obj))
					{
						if (point.CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), range))
						{
							list.Add(obj);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002E2 RID: 738 RVA: 0x00021070 File Offset: 0x0001F270
		private List<BaseObject> GetBombVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int range = (int)magicTypeInfo.range;
				foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.IsAddMagicVisibleObj(obj))
					{
						if (this.play.GetPoint().CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), range))
						{
							list.Add(obj);
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x00021158 File Offset: 0x0001F358
		private List<BaseObject> GetLineVisibleObj(MsgAttackInfo magicinfo)
		{
			List<BaseObject> list = new List<BaseObject>();
			MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(magicinfo.usType, 0);
			List<BaseObject> result;
			if (magicTypeInfo == null)
			{
				result = list;
			}
			else
			{
				int range = (int)magicTypeInfo.range;
				this.play.RefreshVisibleObject();
				Point point = this.play.GetPoint();
				byte dirByPos = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), (short)magicinfo.usPosX, (short)magicinfo.usPosY);
				foreach (RefreshObject refreshObject in this.play.GetVisibleList().Values)
				{
					BaseObject obj = refreshObject.obj;
					if (this.IsAddMagicVisibleObj(obj))
					{
						if (this.play.GetPoint().CheckVisualDistance(obj.GetCurrentX(), obj.GetCurrentY(), range))
						{
							byte dirByPos2 = DIR.GetDirByPos(this.play.GetCurrentX(), this.play.GetCurrentY(), obj.GetCurrentX(), obj.GetCurrentY());
							if (dirByPos2 == dirByPos)
							{
								list.Add(obj);
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x000212C4 File Offset: 0x0001F4C4
		public bool IsComboMagic(uint magic_id)
		{
			byte job = this.play.GetJob();
			return magic_id == 1007U || magic_id == 1010U || magic_id == 1005U || magic_id == 1009U || magic_id == 1021U || (magic_id == 7011U || magic_id == 7010U || magic_id == 7016U || magic_id == 7007U || magic_id == 7009U) || magic_id == 6009U || (magic_id == 1021U || magic_id == 5212U || magic_id == 5242U || magic_id == 5213U);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x000213A4 File Offset: 0x0001F5A4
		public bool IsFighting()
		{
			bool flag = this.play.GetPKSystem().IsPKing();
			bool result;
			if (!flag)
			{
				result = flag;
			}
			else
			{
				flag = (Environment.TickCount - this.mnLastAttackTick <= 10000);
				result = flag;
			}
			return result;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x000213EB File Offset: 0x0001F5EB
		public void SetFighting()
		{
			this.mnLastAttackTick = Environment.TickCount;
		}

		// Token: 0x04000600 RID: 1536
		private PlayerObject play;

		// Token: 0x04000601 RID: 1537
		private BaseObject mAutoTarget;

		// Token: 0x04000602 RID: 1538
		private int mnAutoAttackTick;

		// Token: 0x04000603 RID: 1539
		private int mnLastAttackTick;

		// Token: 0x04000604 RID: 1540
		private byte mnYanHunQiangIndex;

		// Token: 0x04000605 RID: 1541
		private byte mnYanHunQiangExIndex;

		// Token: 0x04000606 RID: 1542
		private List<BaseObject> mListQiShiTuanGuard;

		// Token: 0x04000607 RID: 1543
		private int mnLiuXingYunHuoCount = 0;

		// Token: 0x04000608 RID: 1544
		private TimeOut mLiuXingYunHuoTime;
	}
}
