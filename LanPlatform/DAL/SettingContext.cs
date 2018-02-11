using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LanPlatform.Settings;

namespace LanPlatform.DAL
{
    public class SettingContext : DbContext
    {
        public SettingContext() : base("DataConnection")
        {
            System.Data.Entity.Database.SetInitializer<AccountContext>(null);
        }

        // Settings
        public DbSet<PlatformSetting> Setting { get; set; }
    }
}