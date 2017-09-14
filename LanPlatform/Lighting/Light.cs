using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Lighting
{
    public class Light : EditableDatabaseObject
    {
        // General Properties
        public String Name { get; set; }
        public long Group { get; set; }

        // Network Properties
        public String MacAddress { get; set; }
        public String IpAddress { get; set; }

        // Lighting Properties
        public byte Brightness { get; set; }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        // Location Properties
        public long Room { get; set; }
    }
}