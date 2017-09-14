using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Games.Specialists.Stats
{
    public class PlayerHit : StatsEvent
    {
        public long Attacker { get; set; }
        public long Victim { get; set; }
        public int Damage { get; set; }
        public TSWeapon Weapon { get; set; }
        public bool TeamAttack { get; set; }

        public float AttackerOriginX { get; set; }
        public float AttackerOriginY { get; set; }
        public float AttackerOriginZ { get; set; }

        public float VictimOriginX { get; set; }
        public float VictimOriginY { get; set; }
        public float VictimOriginZ { get; set; }
    }

    
}