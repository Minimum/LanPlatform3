using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.GOnline
{
    public static class GoManager
    {
        public const String FlagScope = "GabionOnline";
    }

    public enum PlayerItemAccess
    {
        None = 0,           // Item is a WIP, not accessable by anyone
        Unlocked,           // Item is unlocked by default
        Buyable,            // Item is locked by default but can be bought
        Special             // Item can only be used by a certain role
    }

    public enum ItemType
    {
        None = 0,
        Item,
        Weapon
    }
}