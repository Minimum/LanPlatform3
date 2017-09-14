using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class AccessRecord : DatabaseObject
    {
        public long Account { get; set; }
        public long Time { get; set; }
        public bool Success { get; set; }
        public String Flag { get; set; }
        public String Scope { get; set; }

        public AccessRecord()
        {
            Account = 0;
            Time = 0;
            Success = false;
            Flag = "";
            Scope = "";
        }
    }
}