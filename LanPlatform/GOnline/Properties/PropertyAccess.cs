using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Properties
{
    public class PropertyAccess : EditableDatabaseObject
    {
        public long User { get; set; }
        public long Property { get; set; }
        public String AccessCode { get; set; }
    }
}