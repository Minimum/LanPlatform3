using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Locations
{
    public class LocationRoomWall : EditableDatabaseObject
    {
        public long Room { get; set; }

        public int StartX { get; set; }
        public int StartY { get; set; }

        public int EndX { get; set; }
        public int EndY { get; set; }
    }
}