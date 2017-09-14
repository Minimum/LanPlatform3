using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Games.Specialists.Stats
{
    public class PlayerKill : StatsEvent
    {
        public KillFlags Flags { get; set; }

        public float AttackerOriginX { get; set; }
        public float AttackerOriginY { get; set; }
        public float AttackerOriginZ { get; set; }

        public float VictimOriginX { get; set; }
        public float VictimOriginY { get; set; }
        public float VictimOriginZ { get; set; }
    }

    
}