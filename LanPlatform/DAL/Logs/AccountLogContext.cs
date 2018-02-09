using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Auth;

namespace LanPlatform.DAL.Logs
{
    public class AccountLogContext : DbContext
    {
        public AccountLogContext() : base("LogConnection")
        {
            System.Data.Entity.Database.SetInitializer<AccountLogContext>(null);
        }

        public DbSet<AccessRecord> AccessRecord { get; set; }               // Admin Access History 
        public DbSet<AccountEditField> AccountEditField { get; set; }       // Account Edit Field History
        public DbSet<AccountEditRecord> AccountEditRecord { get; set; }     // Account Edit History

        public DbSet<AuthSessionAttempt> AuthSessionAttempt { get; set; }   // Failed session auth attempts
        public DbSet<AuthUsernameAttempt> AuthUsernameAttempt { get; set; } // Username auth attempts
    }
}