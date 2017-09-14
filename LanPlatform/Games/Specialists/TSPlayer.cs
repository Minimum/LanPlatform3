using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Games.Specialists
{
    public class TSPlayer : EditableDatabaseObject
    {
        public long Kills { get; set; }
        public long Deaths { get; set; }
        public long Damage { get; set; }
        public long Headshots { get; set; }

        public long TimePlayed { get; set; }

        // Frag Types
        public long FragFirearms { get; set; }          // Kills with firearms
        public long FragKungFu { get; set; }            // Kills with Kung Fu
        public long FragKatana { get; set; }            // Kills with katanas
        public long FragKnife { get; set; }             // Kills with knives

        public long FragStunt { get; set; }             // Kills while stunting
        public long FragSliding { get; set; }           // Kills while sliding
        public long FragDouble { get; set; }            // Double+ kills
        public long FragSpecialist { get; set; }        // Number of specialists killed

        // TS Killing Sprees
        public long SpreeKiller { get; set; }           // 3+ Kills
        public long SpreeDemolitionMan { get; set; }    // 6+ Kills
        public long SpreeTheSpecialist { get; set; }    // 9+ Kills
        public long SpreeUnstoppable { get; set; }      // 10+ Kills

        // Pickups Used
        public long PickupSlowMotion { get; set; }      // Slow Motion
        public long PickupSlowPause { get; set; }
        public long PickupSuperjump { get; set; }
        public long PickupInfiniteAmmo { get; set; }
        public long PickupDoubleFirerate { get; set; }
        public long PickupHealth { get; set; }
        public long PickupKungFu { get; set; }
        public long PickupGrenade { get; set; }
        public long PickupKevlar { get; set; }

        public TSPlayer()
        {
            Kills = 0;
            Deaths = 0;
            Damage = 0;

            TimePlayed = 0;
        }
    }
}