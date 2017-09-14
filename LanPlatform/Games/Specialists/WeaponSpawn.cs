using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;
using LanPlatform.Games.Specialists.Stats;

namespace LanPlatform.Games.Specialists
{
    public class WeaponSpawn : EditableDatabaseObject
    {
        public String Name { get; set; }
        public String Map { get; set; }
        public long Server { get; set; }

        public TSWeapon Weapon { get; set; }
        public TSWeaponAddons Addons { get; set; }

        public float OriginX { get; set; }
        public float OriginY { get; set; }
        public float OriginZ { get; set; }

        public WeaponSpawn()
        {
            Name = "";
            Map = "";
            Server = 0;

            Weapon = TSWeapon.None;
            Addons = TSWeaponAddons.None;

            OriginX = 0;
            OriginY = 0;
            OriginZ = 0;
        }
    }
}