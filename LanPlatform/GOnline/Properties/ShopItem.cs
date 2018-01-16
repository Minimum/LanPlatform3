using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Properties
{
    public class ShopItem : EditableDatabaseObject
    {
        public long Shop { get; set; }
        public long Item { get; set; }
        public ItemType ItemType { get; set; }

        public long Price { get; set; }
    }
}