using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class AccessScope : EditableDatabaseObject
    {
        public String Name { get; set; }
    }
}