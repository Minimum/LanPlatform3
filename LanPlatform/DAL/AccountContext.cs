using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Auth;

namespace LanPlatform.DAL
{
    public class AccountContext : DbContext
    {
        public AccountContext() : base("DataConnection")
        {
            System.Data.Entity.Database.SetInitializer<AccountContext>(null);
        }

        // Accounts
        public DbSet<UserAccount> Account { get; set; }                     // User Account Table
        public DbSet<UserRoleAccess> AccountRole { get; set; }              // Account Assigned Roles Table

        // Authentication
        public DbSet<AuthSession> AuthSession { get; set; }                 // Account Sessions
        public DbSet<AuthUsername> AuthUsername { get; set; }               // Account Username/Passwords

        // Roles
        public DbSet<UserRole> Role { get; set; }                           // Role Table
        public DbSet<UserPermission> RoleFlag { get; set; }                 // Role Access Flag Table
    }
}