using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Network
{
    public class NetMessageTarget : DatabaseObject
    {
        public long Message { get; set; }
        public long User { get; set; }
    }
}