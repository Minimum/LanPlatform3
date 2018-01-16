using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Characters
{
    public class CharacterPreset : EditableDatabaseObject
    {
        public long Owner { get; set; }
        public String Name { get; set; }

        public long Character { get; set; }
        public long Skin { get; set; }
    }
}