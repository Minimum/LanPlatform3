using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;
using LanPlatform.Apps;
using LanPlatform.Auth;
using LanPlatform.Chat;
using LanPlatform.Content;
using LanPlatform.Events;
using LanPlatform.News;
using LanPlatform.Settings;

namespace LanPlatform.DAL
{
    /*
     *  Platform Install Context
     *  ---
     *  This class is used to create the database tables.
     *  This is NOT meant to be used for normal operations, use the appropiate contexts.
     */
    public class InstallContext : DbContext
    {
        public InstallContext() : base("DataConnection")
        {
            
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

        // Logs
        public DbSet<AccessRecord> AccessRecord { get; set; }               // Admin Access History 
        public DbSet<AccountEditField> AccountEditField { get; set; }       // Account Field History
        public DbSet<AccountEditRecord> AccountEditRecord { get; set; }     //

        public DbSet<AuthSessionAttempt> AuthSessionAttempt { get; set; }   // Failed session auth attempts
        public DbSet<AuthUsernameAttempt> AuthUsernameAttempt { get; set; } // Username auth attempts

        // Apps
        public DbSet<App> App { get; set; }

        // Loaner Accounts
        public DbSet<LoanerAccount> LoanerAccount { get; set; }
        public DbSet<LoanerApp> LoanerApp { get; set; }

        // Loaner Account Logs
        public DbSet<LoanerCheckoutRecord> LoanerCheckoutLog { get; set; }

        // Content
        public DbSet<ContentItem> Content { get; set; }
        public DbSet<ContentAccess> ContentAccess { get; set; }

        // Events
        public DbSet<LanEvent> LanEvent { get; set; }
        public DbSet<LanEventGuest> LanEventGuest { get; set; }

        // News
        public DbSet<NewsStatus> NewsStatus { get; set; }
        public DbSet<WeatherStatus> WeatherStatus { get; set; }
        public DbSet<QuickLink> NewsLink { get; set; }

        // Chat
        public DbSet<ChatChannel> ChatChannel { get; set; }
        public DbSet<ChatAccess> ChatAccess { get; set; }
        public DbSet<ChatMute> ChatMute { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }

        // Settings
        public DbSet<PlatformSetting> Setting { get; set; }
    }
}