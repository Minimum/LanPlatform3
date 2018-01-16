using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Skills
{
    public class PlayerSkill : EditableDatabaseObject
    {
        public long Player { get; set; }
        public long Skill { get; set; }

        public int Level { get; set; }
        public long Experience { get; set; }
    }
}