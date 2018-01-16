using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Weapons
{
    public class WeaponAddonGroup : EditableDatabaseObject
    {
        public long Weapon { get; set; }
        public String Name { get; set; }
    }
}