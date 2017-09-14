using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Games.Specialists.Stats
{
    public class StatsEvent : DatabaseObject
    {
        public long GameSession { get; set; }
        public float GameTime { get; set; }
        public long TimeRecorded { get; set; }

        public StatsEvent()
        {
            GameSession = 0;
            GameTime = 0;
            TimeRecorded = 0;
        }
    }

    public enum HitLocation
    {

    }

    public enum TSWeapon
    {
        None = 0,
        Glock18 = 1,
        MiniUzi = 3,
        BenelliM3 = 4,
        M4A1 = 5,
        MP5SD = 6,
        MP5K = 7,
        AkimboBerettas = 8,
        SocomMK23 = 9,
        USAS12 = 11,
        DesertEagle = 12,
        AK47 = 13,
        FiveSeven = 14,
        SteyrAug = 15,
        SteyrTmp = 17,
        BarrettM82A1 = 18,
        MP7 = 19,
        SPAS12 = 20,
        GoldenColts = 21,
        Glock20 = 22,
        Mac10 = 23,
        M61Grenade = 24,
        CombatKnife = 25,
        Mossberg500 = 26,
        M16A4 = 27,
        RugerMk1 = 28,
        RagingBull = 31,
        M60E3 = 32,
        SawedOff = 33,
        Katana = 34,
        SealKnife = 35
    }

    [Flags]
    public enum TSWeaponAddons
    {
        None = 0,
        Silencer = 1 << 0,
        Lasersight = 1 << 1,
        Flashlight = 1 << 2,
        Scope = 1 << 3
    }

    public enum TSPowerup
    {
        Random = 0,
        SlowMotion = 1,
        InfiniteClip = 2,
        KungFu = 4,
        SlowPause = 8,
        DoubleFirerate = 16,
        Grenade = 32,
        Health = 64,
        Armor = 128,
        Superjump = 256
    }

    [Flags]
    public enum KillFlags
    {
        None = 0,
        StuntKill = 1 << 0,
        SlidingKill = 1 << 1,
        DoubleKill = 1 << 2,
        IsSpecialist = 1 << 3,
        KilledSpecialist = 1 << 4
    }
}