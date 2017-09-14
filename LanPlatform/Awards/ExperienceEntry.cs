using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Awards
{
    public class ExperienceEntry : DatabaseObject
    {
        public long Account { get; set; }
        public int Amount { get; set; }
        public long Time { get; set; }

        public ExperienceEntry()
        {
            Account = 0;
            Amount = 0;
            Time = 0;
        }
    }
}