using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Skills
{
    public class Skill : EditableDatabaseObject
    {
        public String DevName { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public long BaseExperience { get; set; }
        public float LevelModifier { get; set; }
    }
}