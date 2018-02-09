using System;
using System.Collections.Generic;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class AccountEditRecord : DatabaseObject
    {
        public long Account { get; set; }
        public long Editor { get; set; }
        public long Version { get; set; }
        public long Time { get; set; }

        public String Description { get; set; }

        public AccountEditRecord()
        {
            Account = 0;
            Editor = 0;
            Version = 0;

            Description = "";
        }
    }
}