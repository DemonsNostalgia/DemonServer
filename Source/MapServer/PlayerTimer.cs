using System;
using System.Collections.Generic;
using GameBase.Config;
using GameBase.Core;
using GameBase.Network;
using GameStruct;
using NetMsg;

namespace MapServer
{
	// Token: 0x02000095 RID: 149
	public class PlayerTimer
	{
		// Token: 0x060003B7 RID: 951 RVA: 0x0002AED0 File Offset: 0x000290D0
		public int GetEffect(bool hi = true)
		{
			int result;
			if (hi)
			{
				result = (int)(this.mi64Effect >> 32 & ulong.MaxValue);
			}
			else
			{
				result = (int)(this.mi64Effect & ulong.MaxValue);
			}
			return result;
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0002AF08 File Offset: 0x00029108
		public int GetEffectEx(bool hi = true)
		{
			int result;
			if (hi)
			{
				result = (int)(this.mi64EffectEx >> 32 & ulong.MaxValue);
			}
			else
			{
				result = (int)(this.mi64EffectEx & ulong.MaxValue);
			}
			return result;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0002AF40 File Offset: 0x00029140
		public PlayerTimer(PlayerObject _play)
		{
			this.mi64Effect = 0UL;
			this.mi64EffectEx = 0UL;
			this.play = _play;
			this.mXpTime = new TimeOut();
			this.mXpTime.SetInterval(30);
			this.mXpTime.Update();
			this.mnXpVal = 0;
			this.mSPTime = new TimeOut();
			this.mSPTime.SetInterval(5);
			this.mSPTime.Update();
			this.mListStatus = new List<RoleStatus>();
			this.mObject_CALL = null;
		}

		// Token: 0x060003BA RID: 954 RVA: 0x0002AFD0 File Offset: 0x000291D0
		public void Run()
		{
			int tickCount = Environment.TickCount;
			for (int i = this.mListStatus.Count - 1; i >= 0; i--)
			{
				if (this.mListStatus[i].nTime != 0 && tickCount - this.mListStatus[i].nLastTick > this.mListStatus[i].nTime)
				{
					this.DeleteStatus(this.mListStatus[i].nStatus);
				}
			}
			this.ProcXPVal();
			if (this.mSPTime.ToNextTime() && this.play.GetBaseAttr().sp < this.play.GetBaseAttr().sp_max)
			{
				if (!this.play.IsDie() && !this.play.GetFightSystem().IsFighting())
				{
					this.play.ChangeAttribute(UserAttribute.SP, 10, true);
				}
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0002B0E0 File Offset: 0x000292E0
		public void XPFull(short magicid)
		{
			if (this.QueryStatus(46) != null || GameServer.IsTestMode())
			{
				this.AddStatus(47, 60, true);
				if ((long)magicid == 3021L)
				{
					this.AddStatus(101, 60, true);
				}
				if ((long)magicid == 6008L)
				{
					this.AddStatus(104, 60, true);
					this.AddStatus(101, 60, true);
				}
			}
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0002B160 File Offset: 0x00029360
		public bool isXPIng()
		{
			return true;
		}

		// Token: 0x060003BD RID: 957 RVA: 0x0002B174 File Offset: 0x00029374
		private void ProcXPVal()
		{
			if (this.play.IsDie() || this.QueryStatus(46) != null)
			{
				this.mnXpVal = 0;
				if (this.mXpTime.ToNextTime())
				{
					this.mXpTime.SetInterval(30);
					this.DeleteStatus(46);
					this.play.ChangeAttribute(UserAttribute.XP, this.mnXpVal, true);
				}
			}
			else if (this.mXpTime.ToNextTime() && this.QueryStatus(46) == null && this.QueryStatus(47) == null)
			{
				this.mnXpVal += 15;
				if (this.mnXpVal > 100)
				{
					this.mnXpVal = 100;
				}
				this.play.ChangeAttribute(UserAttribute.XP, this.mnXpVal, true);
				if (this.mnXpVal >= 100)
				{
					this.AddStatus(46, 60, true);
					this.mnXpVal = 0;
					this.mXpTime.SetInterval(30);
				}
			}
		}

		// Token: 0x060003BE RID: 958 RVA: 0x0002B340 File Offset: 0x00029540
		public void AddStatus(int nStatus, int nTime = 0, bool bCover = true)
		{
			if (this.QueryStatus(1) == null)
			{
				bool flag = false;
				bool flag2 = true;
				for (int i = 0; i < this.mListStatus.Count; i++)
				{
					RoleStatus roleStatus = this.mListStatus[i];
					if (roleStatus.nStatus == nStatus)
					{
						if (bCover)
						{
							roleStatus.nTime = nTime * 1000;
							roleStatus.nLastTick = Environment.TickCount;
						}
						else
						{
							roleStatus.nTime += nTime * 1000;
						}
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					RoleStatus roleStatus = new RoleStatus();
					roleStatus.nStatus = nStatus;
					roleStatus.nTime = nTime * 1000;
					this.mListStatus.Add(roleStatus);
				}
				int effect = this.GetEffect(true);
				if (nStatus <= 47)
				{
					switch (nStatus)
					{
					case 1:
						this.mi64Effect |= 2UL;
						break;
					case 2:
						this.mi64Effect |= 16UL;
						break;
					default:
						if (nStatus != 14)
						{
							switch (nStatus)
							{
							case 47:
								this.DeleteStatus(46);
								this.mXpTime.SetInterval(60);
								this.mXpTime.Update();
								this.mi64Effect |= 274877906944UL;
								break;
							}
						}
						else
						{
							this.mi64Effect |= 512UL;
						}
						break;
					}
				}
				else
				{
					switch (nStatus)
					{
					case 99:
					{
						this.play.ChangeAttribute(UserAttribute.MOLONGSHOUHU_STATUS, 1, true);
						ushort magicLevel = this.play.GetMagicSystem().GetMagicLevel(5225U);
						MagicTypeInfo magicTypeInfo = ConfigManager.Instance().GetMagicTypeInfo(5225U, (byte)magicLevel);
						if (magicTypeInfo != null)
						{
							int power = (int)magicTypeInfo.power;
							byte[] array = new byte[32];
							array[20] = 1;
							byte[] v = array;
							PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
							packetOut.WriteUInt16(48);
							packetOut.WriteUInt16(1127);
							packetOut.WriteUInt32(this.play.GetTypeId());
							packetOut.WriteInt32(nTime);
							packetOut.WriteInt32(power);
							packetOut.WriteBuff(v);
							this.play.SendData(packetOut.Flush(), false);
						}
						break;
					}
					case 100:
						this.mi64Effect |= 4096UL;
						break;
					case 101:
					{
						this.mi64Effect |= 8388608UL;
						MsgUserAttribute msgUserAttribute = new MsgUserAttribute();
						msgUserAttribute.role_id = this.play.GetTypeId();
						msgUserAttribute.Create(null, null);
						msgUserAttribute.AddAttribute(UserAttribute.STATUS, (uint)this.GetEffect(false));
						msgUserAttribute.AddAttribute(UserAttribute.STATUS1, (uint)this.GetEffect(true));
						this.play.BroadcastBuffer(msgUserAttribute.GetBuffer(), true);
						flag2 = false;
						break;
					}
					case 102:
					{
						this.play.ChangeAttribute(UserAttribute.YUANSUZHANGKONG, 512, true);
						byte[] v = new byte[]
						{
							48,
							0,
							103,
							4
						};
						byte[] v2 = new byte[]
						{
							128,
							81,
							1,
							0,
							100,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							2,
							0,
							0
						};
						PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
						packetOut.WriteBuff(v);
						packetOut.WriteUInt32(this.play.GetTypeId());
						packetOut.WriteBuff(v2);
						this.play.SendData(packetOut.Flush(), false);
						flag2 = false;
						break;
					}
					case 103:
					{
						byte[] v = new byte[]
						{
							48,
							0,
							103,
							4
						};
						byte[] v2 = new byte[]
						{
							132,
							3,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							0,
							4,
							0,
							0
						};
						PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
						packetOut.WriteBuff(v);
						packetOut.WriteUInt32(this.play.GetTypeId());
						packetOut.WriteBuff(v2);
						this.play.SendData(packetOut.Flush(), false);
						flag2 = false;
						break;
					}
					case 104:
						this.mi64Effect |= 35184372088832UL;
						break;
					case 105:
					{
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(1430U);
						if (monsterInfo == null)
						{
							Log.Instance().WriteLog("Failed to create Ansha Evil Dragon; monster ID does not exist.");
						}
						else
						{
							int num = (int)(this.play.GetCurrentX() + DIR._DELTA_X[(int)this.play.GetDir()]);
							int num2 = (int)(this.play.GetCurrentY() + DIR._DELTA_Y[(int)this.play.GetDir()]);
							this.mObject_CALL = new AnShaXieLongObject(this.play, (short)num, (short)num2, this.play.GetDir(), monsterInfo.id, monsterInfo.ai);
							this.play.GetGameMap().AddObject(this.mObject_CALL, null);
							this.mObject_CALL.RefreshVisibleObject();
							this.mObject_CALL.Alive(false);
						}
						break;
					}
					case 106:
					{
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(1429U);
						if (monsterInfo == null)
						{
							Log.Instance().WriteLog("Failed to create Netherworld Maiden; monster ID does not exist.");
						}
						else
						{
							int num = (int)(this.play.GetCurrentX() + DIR._DELTA_X[(int)this.play.GetDir()]);
							int num2 = (int)(this.play.GetCurrentY() + DIR._DELTA_Y[(int)this.play.GetDir()]);
							this.mObject_CALL = new MingGuoShengNv(this.play, (short)num, (short)num2, this.play.GetDir(), monsterInfo.id, monsterInfo.ai);
							this.play.GetGameMap().AddObject(this.mObject_CALL, null);
							this.mObject_CALL.RefreshVisibleObject();
							this.mObject_CALL.Alive(false);
						}
						break;
					}
					case 107:
					{
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						MonsterInfo monsterInfo = ConfigManager.Instance().GetMonsterInfo(1431U);
						if (monsterInfo == null)
						{
							Log.Instance().WriteLog("Failed to create Wangnian Witch Spirit; monster ID does not exist.");
						}
						else
						{
							int num = (int)(this.play.GetCurrentX() + DIR._DELTA_X[(int)this.play.GetDir()]);
							int num2 = (int)(this.play.GetCurrentY() + DIR._DELTA_Y[(int)this.play.GetDir()]);
							this.mObject_CALL = new WangNianWuLing(this.play, (short)num, (short)num2, this.play.GetDir(), monsterInfo.id, monsterInfo.ai);
							this.play.GetGameMap().AddObject(this.mObject_CALL, null);
							this.mObject_CALL.RefreshVisibleObject();
							this.mObject_CALL.Alive(false);
						}
						break;
					}
					default:
						if (nStatus != 120)
						{
							switch (nStatus)
							{
							case 1001:
								this.mi64Effect |= 32UL;
								break;
							case 1002:
								this.mi64Effect |= 64UL;
								break;
							case 1003:
								this.mi64EffectEx |= 262144UL;
								break;
							case 1004:
								this.DeleteStatus(1003);
								break;
							case 1008:
								this.mi64Effect |= 140737488355328UL;
								break;
							case 1009:
							{
								this.mi64EffectEx |= 512UL;
								PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
								byte[] v3 = new byte[]
								{
									60,
									0,
									0,
									0,
									35,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									2,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0,
									0
								};
								packetOut.WriteUInt16(48);
								packetOut.WriteUInt16(1127);
								packetOut.WriteUInt32(this.play.GetTypeId());
								packetOut.WriteBuff(v3);
								this.play.SendData(packetOut.Flush(), false);
								break;
							}
							}
						}
						else
						{
							this.mi64EffectEx |= 32UL;
							PacketOut packetOut = new PacketOut(this.play.GetGamePackKeyEx());
							byte[] v3 = new byte[]
							{
								120,
								0,
								0,
								0,
								1,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								32,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0,
								0
							};
							packetOut.WriteUInt16(48);
							packetOut.WriteUInt16(1127);
							packetOut.WriteUInt32(this.play.GetTypeId());
							packetOut.WriteBuff(v3);
							this.play.SendData(packetOut.Flush(), false);
						}
						break;
					}
				}
				if (flag2)
				{
					this.SendState(null);
				}
			}
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0002BC54 File Offset: 0x00029E54
		public void SendState(PlayerObject _play = null)
		{
			MsgUserAttribute msgUserAttribute = new MsgUserAttribute();
			msgUserAttribute.role_id = this.play.GetTypeId();
			int value = this.GetEffect(true);
			int value2 = this.GetEffect(false);
			msgUserAttribute.AddAttribute(UserAttribute.STATUS, (uint)value2);
			msgUserAttribute.AddAttribute(UserAttribute.STATUS1, (uint)value);
			value = this.GetEffectEx(true);
			value2 = this.GetEffectEx(false);
			msgUserAttribute.AddAttribute(UserAttribute.STATUSEX, (uint)value2);
			if (_play != null)
			{
				_play.SendData(msgUserAttribute.GetBuffer(), true);
			}
			else
			{
				this.play.BroadcastBuffer(msgUserAttribute.GetBuffer(), true);
			}
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0002BCE8 File Offset: 0x00029EE8
		public RoleStatus QueryStatus(int nStatus)
		{
			for (int i = 0; i < this.mListStatus.Count; i++)
			{
				if (this.mListStatus[i].nStatus == nStatus)
				{
					return this.mListStatus[i];
				}
			}
			return null;
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0002BD42 File Offset: 0x00029F42
		public void ExitGame()
		{
			this.Die_DeleteState();
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0002BD4C File Offset: 0x00029F4C
		public void Die_DeleteState()
		{
			this.DeleteStatus(46);
			this.DeleteStatus(47);
			this.DeleteStatus(101);
			this.DeleteStatus(104);
			this.DeleteStatus(100);
			this.DeleteStatus(14);
			this.DeleteStatus(105);
			this.DeleteStatus(106);
			this.DeleteStatus(107);
			this.DeleteStatus(120);
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0002BDB4 File Offset: 0x00029FB4
		public void DeleteStatus(int nStatus)
		{
			bool flag = false;
			for (int i = 0; i < this.mListStatus.Count; i++)
			{
				if (this.mListStatus[i].nStatus == nStatus)
				{
					this.mListStatus.RemoveAt(i);
					flag = true;
					break;
				}
			}
			bool flag2 = true;
			if (flag)
			{
				if (nStatus <= 47)
				{
					switch (nStatus)
					{
					case 1:
						this.mi64Effect &= 18446744073709551613UL;
						break;
					case 2:
						this.mi64Effect &= 18446744073709551599UL;
						this.play.GetPKSystem().ResetPKNameType();
						break;
					default:
						if (nStatus != 14)
						{
							switch (nStatus)
							{
							case 46:
								this.mnXpVal = 0;
								break;
							case 47:
								this.mi64Effect &= 18446743798831644671UL;
								this.play.ChangeAttribute(UserAttribute.XP, 0, true);
								this.mnXpVal = 0;
								this.mXpTime.SetInterval(30);
								this.mXpTime.Update();
								break;
							}
						}
						else
						{
							this.mi64Effect &= 18446744073709551103UL;
						}
						break;
					}
				}
				else
				{
					switch (nStatus)
					{
					case 99:
						this.play.ChangeAttribute(UserAttribute.MOLONGSHOUHU_STATUS, 0, true);
						break;
					case 100:
						this.mi64Effect &= 18446744073709547519UL;
						break;
					case 101:
						this.mi64Effect &= 18446744073701163007UL;
						break;
					case 102:
						break;
					case 103:
						break;
					case 104:
						this.mi64Effect &= 18446708889337462783UL;
						break;
					case 105:
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						flag2 = false;
						break;
					case 106:
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						flag2 = false;
						break;
					case 107:
						if (this.mObject_CALL != null)
						{
							this.mObject_CALL.ClearThis();
						}
						flag2 = false;
						break;
					default:
						if (nStatus != 120)
						{
							switch (nStatus)
							{
							case 1001:
								this.mi64Effect &= 18446744073709551583UL;
								break;
							case 1002:
								this.mi64Effect &= 18446744073709551551UL;
								break;
							case 1003:
								this.mi64EffectEx &= 18446744073709289471UL;
								break;
							case 1008:
								this.mi64Effect &= 18446603336221196287UL;
								break;
							case 1009:
								this.mi64EffectEx &= 18446744073709551103UL;
								break;
							}
						}
						else
						{
							this.mi64EffectEx &= 18446744073709551583UL;
						}
						break;
					}
				}
				if (flag2)
				{
					this.SendState(null);
				}
			}
		}

		// Token: 0x04000648 RID: 1608
		private PlayerObject play;

		// Token: 0x04000649 RID: 1609
		private ulong mi64Effect;

		// Token: 0x0400064A RID: 1610
		private ulong mi64EffectEx;

		// Token: 0x0400064B RID: 1611
		private MonsterObject mObject_CALL;

		// Token: 0x0400064C RID: 1612
		private TimeOut mXpTime;

		// Token: 0x0400064D RID: 1613
		private int mnXpVal;

		// Token: 0x0400064E RID: 1614
		private TimeOut mSPTime;

		// Token: 0x0400064F RID: 1615
		private List<RoleStatus> mListStatus;
	}
}
