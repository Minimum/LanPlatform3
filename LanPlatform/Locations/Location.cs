using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Database;

namespace LanPlatform.Locations
{
    public class Location : EditableDatabaseObject
    {
        public long Owner { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }


    }
}