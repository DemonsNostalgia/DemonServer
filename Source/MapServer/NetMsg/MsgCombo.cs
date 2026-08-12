using System;
using GameBase.Core;
using GameBase.Network;
using GameStruct;
using MapServer;

namespace NetMsg
{
	// Token: 0x02000078 RID: 120
	public class MsgCombo : BaseMsg
	{
		// Token: 0x0600025A RID: 602 RVA: 0x0001813C File Offset: 0x0001633C
		public MsgCombo()
		{
			this.mMsgLen = 11;
			this.mParam = 1015;
			this.combo = new PacketOut(null);
			this.count = 0U;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00018191 File Offset: 0x00016391
		public override void Create(byte[] msg = null, GamePacketKeyEx key = null)
		{
			base.Create(msg, key);
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000181A0 File Offset: 0x000163A0
		public override byte[] GetBuffer()
		{
			this.combo.WriteByte(0);
			byte[] buffer = this.combo.GetBuffer();
			this.mMsgLen += (ushort)buffer.Length;
			PacketOut packetOut = new PacketOut(this.mKey);
			packetOut.WriteUInt16(this.mMsgLen);
			packetOut.WriteUInt16(this.mParam);
			packetOut.WriteUInt32(this.count);
			packetOut.WriteInt16(this.type);
			packetOut.WriteByte((byte)this.count);
			packetOut.WriteBuff(buffer);
			return packetOut.Flush();
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0001823C File Offset: 0x0001643C
		public void CalcTag(uint magicid, BaseObject attack, BaseObject target)
		{
			if ((attack.GetCurrentX() < 999 && attack.GetCurrentY() < 999) || (target.GetCurrentX() < 999 && target.GetCurrentY() < 999))
			{
				if (target.type == 3)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_102;
						case 1006U:
						case 1008U:
							goto IL_13B;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_102;
								default:
									goto IL_13B;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_102;
						}
						if (magicid != 6009U)
						{
							goto IL_13B;
						}
						this.head = 27;
						this.tail = 28;
						goto IL_13B;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_13B;
						case 7011U:
							this.head = 25;
							this.tail = 28;
							goto IL_13B;
						default:
							if (magicid != 7016U)
							{
								goto IL_13B;
							}
							break;
						}
					}
					this.head = 25;
					this.tail = 26;
					goto IL_13B;
					IL_102:
					this.head = 26;
					this.tail = 27;
					IL_13B:;
				}
				else if (target.type == 2)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_1F5;
						case 1006U:
						case 1008U:
							goto IL_21B;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_1F5;
								default:
									goto IL_21B;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_1F5;
						}
						if (magicid != 6009U)
						{
							goto IL_21B;
						}
						this.head = 28;
						this.tail = 28;
						goto IL_21B;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_21B;
						default:
							if (magicid != 7016U)
							{
								goto IL_21B;
							}
							break;
						}
					}
					this.head = 26;
					this.tail = 26;
					goto IL_21B;
					IL_1F5:
					this.head = 27;
					this.tail = 27;
					IL_21B:;
				}
				else if (target.type == 4)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_2D5;
						case 1006U:
						case 1008U:
							goto IL_2FB;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_2D5;
								default:
									goto IL_2FB;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_2D5;
						}
						if (magicid != 6009U)
						{
							goto IL_2FB;
						}
						this.head = 31;
						this.tail = 31;
						goto IL_2FB;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_2FB;
						default:
							if (magicid != 7016U)
							{
								goto IL_2FB;
							}
							break;
						}
					}
					this.head = 29;
					this.tail = 26;
					goto IL_2FB;
					IL_2D5:
					this.head = 30;
					this.tail = 27;
					IL_2FB:;
				}
			}
			else if ((target.GetCurrentX() > 999 && target.GetCurrentY() > 999) || (attack.GetCurrentX() > 999 && attack.GetCurrentY() > 999))
			{
				if (target.type == 3)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							break;
						case 1006U:
						case 1008U:
							goto IL_422;
						case 1007U:
							goto IL_3FC;
						default:
							if (magicid == 1021U)
							{
								goto IL_3FC;
							}
							switch (magicid)
							{
							case 5212U:
							case 5213U:
								break;
							default:
								goto IL_422;
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid != 5242U)
						{
							if (magicid != 6009U)
							{
								goto IL_422;
							}
							this.head = 29;
							this.tail = 30;
							goto IL_422;
						}
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							goto IL_3FC;
						case 7008U:
							goto IL_422;
						default:
							if (magicid != 7016U)
							{
								goto IL_422;
							}
							goto IL_3FC;
						}
					}
					this.head = 28;
					this.tail = 29;
					goto IL_422;
					IL_3FC:
					this.head = 27;
					this.tail = 28;
					IL_422:;
				}
				else if (target.type == 2)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_4EE;
						case 1006U:
						case 1008U:
							goto IL_501;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_4EE;
								default:
									goto IL_501;
								}
							}
							break;
						}
						this.head = 28;
						this.tail = 28;
						goto IL_501;
					}
					if (magicid > 6009U)
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_501;
						default:
							if (magicid != 7016U)
							{
								goto IL_501;
							}
							break;
						}
						this.head = 27;
						this.tail = 27;
						goto IL_501;
					}
					if (magicid != 5242U && magicid != 6009U)
					{
						goto IL_501;
					}
					IL_4EE:
					this.head = 29;
					this.tail = 29;
					IL_501:;
				}
				else if (target.type == 4)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_5CD;
						case 1006U:
						case 1008U:
							goto IL_5E0;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_5CD;
								default:
									goto IL_5E0;
								}
							}
							break;
						}
						this.head = 31;
						this.tail = 28;
						goto IL_5E0;
					}
					if (magicid > 6009U)
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_5E0;
						default:
							if (magicid != 7016U)
							{
								goto IL_5E0;
							}
							break;
						}
						this.head = 31;
						this.tail = 28;
						goto IL_5E0;
					}
					if (magicid != 5242U && magicid != 6009U)
					{
						goto IL_5E0;
					}
					IL_5CD:
					this.head = 32;
					this.tail = 29;
					IL_5E0:;
				}
			}
			else if (target.GetCurrentX() > 999 || target.GetCurrentY() > 999 || attack.GetCurrentX() > 999 || attack.GetCurrentY() > 999)
			{
				if (target.type == 3)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_6DD;
						case 1006U:
						case 1008U:
							goto IL_703;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_6DD;
								default:
									goto IL_703;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_6DD;
						}
						if (magicid != 6009U)
						{
							goto IL_703;
						}
						this.head = 28;
						this.tail = 29;
						goto IL_703;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_703;
						default:
							if (magicid != 7016U)
							{
								goto IL_703;
							}
							break;
						}
					}
					this.head = 26;
					this.tail = 27;
					goto IL_703;
					IL_6DD:
					this.head = 27;
					this.tail = 28;
					IL_703:;
				}
				else if (target.type == 2)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_7BC;
						case 1006U:
						case 1008U:
							goto IL_7E2;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_7BC;
								default:
									goto IL_7E2;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_7BC;
						}
						if (magicid != 6009U)
						{
							goto IL_7E2;
						}
						this.head = 30;
						this.tail = 30;
						goto IL_7E2;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_7E2;
						default:
							if (magicid != 7016U)
							{
								goto IL_7E2;
							}
							break;
						}
					}
					this.head = 27;
					this.tail = 27;
					goto IL_7E2;
					IL_7BC:
					this.head = 28;
					this.tail = 28;
					IL_7E2:;
				}
				else if (target.type == 4)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_89B;
						case 1006U:
						case 1008U:
							goto IL_8C1;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_89B;
								default:
									goto IL_8C1;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_89B;
						}
						if (magicid != 6009U)
						{
							goto IL_8C1;
						}
						this.head = 30;
						this.tail = 30;
						goto IL_8C1;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_8C1;
						default:
							if (magicid != 7016U)
							{
								goto IL_8C1;
							}
							break;
						}
					}
					this.head = 20;
					this.tail = 27;
					goto IL_8C1;
					IL_89B:
					this.head = 31;
					this.tail = 28;
					IL_8C1:;
				}
			}
			else if ((target.GetCurrentX() > 99 && target.GetCurrentY() < 99) || (attack.GetCurrentX() < 99 && attack.GetCurrentY() > 99))
			{
				if (target.type == 3)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_9B6;
						case 1006U:
						case 1008U:
							goto IL_9DC;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_9B6;
								default:
									goto IL_9DC;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_9B6;
						}
						if (magicid != 6009U)
						{
							goto IL_9DC;
						}
						this.head = 26;
						this.tail = 27;
						goto IL_9DC;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_9DC;
						default:
							if (magicid != 7016U)
							{
								goto IL_9DC;
							}
							break;
						}
					}
					this.head = 24;
					this.tail = 25;
					goto IL_9DC;
					IL_9B6:
					this.head = 25;
					this.tail = 26;
					IL_9DC:;
				}
				else if (target.type == 2)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_A95;
						case 1006U:
						case 1008U:
							goto IL_ABB;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_A95;
								default:
									goto IL_ABB;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_A95;
						}
						if (magicid != 6009U)
						{
							goto IL_ABB;
						}
						this.head = 27;
						this.tail = 27;
						goto IL_ABB;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_ABB;
						default:
							if (magicid != 7016U)
							{
								goto IL_ABB;
							}
							break;
						}
					}
					this.head = 25;
					this.tail = 25;
					goto IL_ABB;
					IL_A95:
					this.head = 26;
					this.tail = 26;
					IL_ABB:;
				}
				else if (target.type == 4)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_B74;
						case 1006U:
						case 1008U:
							goto IL_B9A;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_B74;
								default:
									goto IL_B9A;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_B74;
						}
						if (magicid != 6009U)
						{
							goto IL_B9A;
						}
						this.head = 30;
						this.tail = 27;
						goto IL_B9A;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_B9A;
						default:
							if (magicid != 7016U)
							{
								goto IL_B9A;
							}
							break;
						}
					}
					this.head = 28;
					this.tail = 25;
					goto IL_B9A;
					IL_B74:
					this.head = 29;
					this.tail = 26;
					IL_B9A:;
				}
			}
			else if ((target.GetCurrentX() < 99 && target.GetCurrentY() < 99) || (attack.GetCurrentX() < 99 && attack.GetCurrentY() < 99))
			{
				if (target.type == 3)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_C8F;
						case 1006U:
						case 1008U:
							goto IL_CB5;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_C8F;
								default:
									goto IL_CB5;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_C8F;
						}
						if (magicid != 6009U)
						{
							goto IL_CB5;
						}
						this.head = 25;
						this.tail = 26;
						goto IL_CB5;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_CB5;
						default:
							if (magicid != 7016U)
							{
								goto IL_CB5;
							}
							break;
						}
					}
					this.head = 23;
					this.tail = 24;
					goto IL_CB5;
					IL_C8F:
					this.head = 24;
					this.tail = 25;
					IL_CB5:;
				}
				else if (target.type == 2)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_D6E;
						case 1006U:
						case 1008U:
							goto IL_D94;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_D6E;
								default:
									goto IL_D94;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_D6E;
						}
						if (magicid != 6009U)
						{
							goto IL_D94;
						}
						this.head = 26;
						this.tail = 26;
						goto IL_D94;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_D94;
						default:
							if (magicid != 7016U)
							{
								goto IL_D94;
							}
							break;
						}
					}
					this.head = 24;
					this.tail = 24;
					goto IL_D94;
					IL_D6E:
					this.head = 25;
					this.tail = 25;
					IL_D94:;
				}
				else if (target.type == 4)
				{
					if (magicid <= 5213U)
					{
						switch (magicid)
						{
						case 1005U:
						case 1009U:
						case 1010U:
							goto IL_E4D;
						case 1006U:
						case 1008U:
							goto IL_E73;
						case 1007U:
							break;
						default:
							if (magicid != 1021U)
							{
								switch (magicid)
								{
								case 5212U:
								case 5213U:
									goto IL_E4D;
								default:
									goto IL_E73;
								}
							}
							break;
						}
					}
					else if (magicid <= 6009U)
					{
						if (magicid == 5242U)
						{
							goto IL_E4D;
						}
						if (magicid != 6009U)
						{
							goto IL_E73;
						}
						this.head = 29;
						this.tail = 26;
						goto IL_E73;
					}
					else
					{
						switch (magicid)
						{
						case 7007U:
						case 7009U:
						case 7010U:
							break;
						case 7008U:
							goto IL_E73;
						default:
							if (magicid != 7016U)
							{
								goto IL_E73;
							}
							break;
						}
					}
					this.head = 27;
					this.tail = 24;
					goto IL_E73;
					IL_E4D:
					this.head = 28;
					this.tail = 25;
					IL_E73:;
				}
			}
		}

		// Token: 0x0600025E RID: 606 RVA: 0x000190C0 File Offset: 0x000172C0
		public void AddComboInfo(uint magicid, BaseObject attack, BaseObject target, uint track_id, uint track_id2)
		{
			if (Program._Head > 0)
			{
				this.head = Program._Head;
				this.tail = Program._Tail;
			}
			byte againstDir = DIR.GetAgainstDir(target.GetDir());
			this.count += 2U;
			this.combo.WriteByte(this.head);
			string s = Convert.ToString(target.GetTypeId());
			byte[] bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(target.GetCurrentX());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(target.GetCurrentY());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(againstDir);
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(track_id2);
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(this.tail);
			s = Convert.ToString(attack.GetTypeId());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(attack.GetCurrentX());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(attack.GetCurrentY());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(attack.GetDir());
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
			this.combo.WriteByte(32);
			s = Convert.ToString(track_id);
			bytes = Coding.GetUtf8Coding().GetBytes(s);
			this.combo.WriteBuff(bytes);
		}

		// Token: 0x04000559 RID: 1369
		public uint count;

		// Token: 0x0400055A RID: 1370
		public short type = 642;

		// Token: 0x0400055B RID: 1371
		public PacketOut combo;

		// Token: 0x0400055C RID: 1372
		private byte head = 0;

		// Token: 0x0400055D RID: 1373
		private byte tail = 0;
	}
}
