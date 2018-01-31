using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LanPlatform.DAL.GOnline
{
    public class GoInstallContext : DbContext
    {
        public GoInstallContext() : base("DataConnection")
        {
            
        }
    }
}