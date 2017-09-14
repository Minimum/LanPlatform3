using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Locations
{
    public class LocationRoom : EditableDatabaseObject
    {
        public long Location { get; set; }
        public String Name { get; set; }


    }
}