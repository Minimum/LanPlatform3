using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Properties
{
    public class Shop : EditableDatabaseObject
    {
        public long Property { get; set; }
        public String Name { get; set; }
    }
}