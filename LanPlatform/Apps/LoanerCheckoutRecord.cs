using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Database;
using LanPlatform.Engine;

namespace LanPlatform.Apps
{
    public class LoanerCheckoutRecord : DatabaseObject
    {
        public long User { get; set; }
        public long Loaner { get; set; }
        public bool Checkout { get; set; }
        public long Time { get; set; }

        public LoanerCheckoutRecord()
        {
            User = 0;
            Loaner = 0;
            Checkout = false;
            Time = 0;
        }

        public LoanerCheckoutRecord(UserAccount user, LoanerAccount loaner, bool checkout)
        {
            User = user.Id;
            Loaner = loaner.Id;
            Checkout = checkout;
            Time = EngineUtil.CurrentTime;
        }
    }
}