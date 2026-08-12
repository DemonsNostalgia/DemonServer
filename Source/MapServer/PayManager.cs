using System;
using System.Collections.Generic;
using GameBase.Network.Internal;
using GameStruct;

namespace MapServer
{
	// Token: 0x0200008C RID: 140
	public class PayManager
	{
		// Token: 0x0600029F RID: 671 RVA: 0x0001AB38 File Offset: 0x00018D38
		public static PayManager Instance()
		{
			if (PayManager.mInstance == null)
			{
				PayManager.mInstance = new PayManager();
			}
			return PayManager.mInstance;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0001AB6A File Offset: 0x00018D6A
		public PayManager()
		{
			this.mDicPayRecInfo = new Dictionary<string, PayRecInfo>();
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0001AB80 File Offset: 0x00018D80
		public void SetPayTag(string account)
		{
			PackUpdatePayRecInfo packUpdatePayRecInfo = new PackUpdatePayRecInfo();
			packUpdatePayRecInfo.account = account;
			DBServer.Instance().GetDBClient().SendData(packUpdatePayRecInfo.GetBuffer());
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0001ABB4 File Offset: 0x00018DB4
		public void GetMoney(PlayerObject play)
		{
			string sAccount = play.GetBaseAttr().sAccount;
			if (!this.mDicPayRecInfo.ContainsKey(sAccount))
			{
				play.MsgBox("No extractable magic stones!");
			}
			else
			{
				int money = this.mDicPayRecInfo[sAccount].money;
				play.ChangeMoney(MONEYTYPE.GAMEGOLD, money);
				play.MsgBox("Extract Magic Stone [" + money.ToString() + "]Click!");
				this.SetPayTag(sAccount);
				this.mDicPayRecInfo.Remove(sAccount);
			}
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0001AC3C File Offset: 0x00018E3C
		public void DB_Load(PackPayRecInfo info)
		{
			if (this.mDicPayRecInfo.ContainsKey(info.account))
			{
				this.mDicPayRecInfo[info.account].money = info.money;
			}
			else
			{
				PayRecInfo payRecInfo = new PayRecInfo();
				payRecInfo.account = info.account;
				payRecInfo.id = info.id;
				payRecInfo.order = info.order;
				payRecInfo.money = info.money;
				this.mDicPayRecInfo[info.account] = payRecInfo;
			}
		}

		// Token: 0x040005F6 RID: 1526
		private static PayManager mInstance = null;

		// Token: 0x040005F7 RID: 1527
		private Dictionary<string, PayRecInfo> mDicPayRecInfo;
	}
}
