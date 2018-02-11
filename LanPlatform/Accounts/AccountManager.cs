using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using LanPlatform.Auth;
using LanPlatform.DAL;
using LanPlatform.DAL.Logs;
using LanPlatform.Events;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Platform;
using LanPlatform.Settings;

namespace LanPlatform.Accounts
{
    public class AccountManager : IPlatformManager
    {
        // Access flags
        public const String FlagViewHiddenAccount = "AccountViewHidden";
        public const String FlagCreateAccount = "AccountCreate";
        public const String FlagEditAccountBasic = "AccountEditBasic";
        public const String FlagEditAccountAdvanced = "AccountEditAdvanced";
        public const String FlagEditUsername = "AccountAuthUsernameEdit";
        public const String FlagEditSession = "AccountAuthSessionEdit";
        public const String FlagEditRoles = "AccountEditRoles";

        // Settings
        public const String SettingAllowRegister = "AccountAllowRegister";
        public const String SettingDefaultAvatar = "AccountDefaultAvatar";

        protected AccountContext Context;
        protected AccountLogContext LogContext;
        protected UserAccount LocalAccount;

        protected AppInstance Instance;

        public AccountManager(AppInstance instance)
        {
            Context = instance.AccountContext;
            LogContext = instance.AccountLogContext;
            LocalAccount = null;

            Instance = instance;
        }

        public bool Install()
        {
            SettingsManager settings = Instance.Settings;

            settings.AddSetting(new PlatformSetting(SettingAllowRegister, "Allow Account Registrations",
                "Allows new users to register.", "0"));

            //settings.AddSetting(new PlatformSetting(SettingDefaultAvatar, "Default Avatar", "The default avatar for users.", "0"));

            AccessRecord accessRecord = LogContext.AccessRecord.SingleOrDefault(s => s.Id == 0);
            UserAccount account = Context.Account.SingleOrDefault(s => s.Id == 0);
            AccountEditField editField = LogContext.AccountEditField.SingleOrDefault(s => s.Id == 0);
            AccountEditRecord editRecord = LogContext.AccountEditRecord.SingleOrDefault(s => s.Id == 0);
            UserRoleAccess accountRole = Context.AccountRole.SingleOrDefault(s => s.Id == 0);
            AuthSession session = Context.AuthSession.SingleOrDefault(s => s.Id == 0);
            AuthSessionAttempt sessionAttempt = LogContext.AuthSessionAttempt.SingleOrDefault(s => s.Id == 0);
            AuthUsername username = Context.AuthUsername.SingleOrDefault(s => s.Id == 0);
            AuthUsernameAttempt usernameAttempt = LogContext.AuthUsernameAttempt.SingleOrDefault(s => s.Id == 0);
            UserRole role = Context.Role.SingleOrDefault(s => s.Id == 0);

            return true;
        }

        /*
         * General Authentication
         */

        public UserAccount AuthenticateLocalUser()
        {
            UserAccount localAccount = null;

            // Get session cookies
            HttpCookie sessionId = HttpContext.Current.Request.Cookies["LPSessionId"];
            HttpCookie sessionKey = HttpContext.Current.Request.Cookies["LPSessionKey"];

            if (sessionId != null && sessionKey != null)
            {
                // Get session id
                long id = AuthSession.GetIdFromCookie(sessionId);

                // Authenticate session
                localAccount = AuthBySession(id, sessionKey.Value);

                if (localAccount != null)
                {
                    // Update account's last active time
                    UpdateActivity(localAccount);
                }
                else if(sessionKey.Value.Length > 0)
                {
                    // Log invalid session key attempt
                    AuthSessionAttempt attempt = new AuthSessionAttempt(Instance, id, sessionKey.Value);

                    LogContext.AuthSessionAttempt.Add(attempt);

                    try
                    {
                        LogContext.SaveChanges();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("[" + Instance.Time + "] WARNING: Failed to log security event.\n * Event: " + attempt.Address + " failed a session key challenge.");
                    }

                    // Attempt to remove failed key
                    Instance.AddCookie("LPSessionId", "", DateTimeOffset.UtcNow.AddDays(-1));
                    Instance.AddCookie("LPSessionKey", "", DateTimeOffset.UtcNow.AddDays(-1));
                }
            }

            // Set local account
            LocalAccount = localAccount;

            // Save changes
            try
            {
                Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                // If concurrency issue is detected, default to database
                e.Entries.Single().Reload();
            }

            return localAccount;
        }

        /*
         * Auth (Usernames)
         */

        public AuthUsername CreateUsername(UserAccount account, String username, String password)
        {
            return CreateUsername(account.Id, username, password);
        }

        public AuthUsername CreateUsername(long accountId, String username, String password)
        {
            AuthUsername auth = (from u in Context.AuthUsername
                where
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                select u).FirstOrDefault();

            // Check if username already exists
            if (auth == null)
            {
                // Create new auth
                auth = AuthUsername.CreateAuth(username, password);

                auth.Account = accountId;

                Context.AuthUsername.Add(auth);
            }

            return auth;
        }

        public UserAccount AuthByUsername(String username, String password)
        {
            UserAccount account = null;
            AuthUsername auth = (from u in Context.AuthUsername
                where
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                    u.Active
                select u).FirstOrDefault();

            // Authenticate
            if (auth != null && auth.Authenticate(password))
            {
                // Load account
                account = GetAccount(auth.Account);
            }

            return account;
        }
        
        /*
         * Auth (Sessions)
         */

        public AuthSession CreateSession(UserAccount account)
        {
            AuthSession session = new AuthSession();

            session.Account = account.Id;
            session.Key = AuthSession.GenerateKey();

            Context.AuthSession.Add(session);

            return session;
        }

        public UserAccount AuthBySession(long sessionId, String key)
        {
            UserAccount account = (from s in Context.AuthSession
                                   join a in Context.Account on s.Account equals a.Id
                                   where s.Id == sessionId &&
                                   s.Active &&
                                   (s.ExpireDate == 0 || s.ExpireDate > Instance.Time) &&
                                   s.Key.Equals(key, StringComparison.OrdinalIgnoreCase)
                                   select a).FirstOrDefault();

            return account;
        }

        /*
         * Accounts
         */

        public UserAccount GetAccount(long id)
        {
            return Context.Account.SingleOrDefault(s => s.Id == id);
        }

        public UserAccount GetAccountReadOnly(long id)
        {
            return Context.Account.Where(s => s.Id == id).AsNoTracking().SingleOrDefault();
        }

        public UserAccount GetAccountByUrl(String url)
        {
            UserAccount account = null;

            if (url.Length > 0)
            {
                account = Context.Account.SingleOrDefault(s => s.CustomUrl.Equals(url, StringComparison.OrdinalIgnoreCase));
            }

            return account;
        }

        public List<UserAccount> GetAccountSearch(SearchAccountRequest request)
        {
            request.SanityCheck();

            long startPos = (request.Page - 1) * request.PageSize;

            IQueryable<UserAccount> query;

            // Only show accounts visible to user
            if (LocalAccount != null)
            {
                if (CheckAccess(LocalAccount, FlagViewHiddenAccount, false))
                {
                    query = Context.Account.Where(s => s.Id > 0);
                }
                else
                {
                    query = Context.Account.Where(s => s.Visibility != AccountVisibility.HiddenFromUsers);
                }
            }
            else
            {
                query = Context.Account.Where(s => s.Visibility == AccountVisibility.Visible);
            }

            // Sort
            switch (request.SortBy)
            {
                case SearchAccountSort.DisplayName:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.DisplayName).ThenBy(s => s.Id) 
                        : query.OrderBy(s => s.DisplayName).ThenBy(s => s.Id);

                    break;
                }

                case SearchAccountSort.FirstName:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.FirstName).ThenBy(s => s.Id) 
                        : query.OrderBy(s => s.FirstName).ThenBy(s => s.Id);

                    break;
                }

                case SearchAccountSort.LastName:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.LastName).ThenBy(s => s.Id) 
                        : query.OrderBy(s => s.LastName).ThenBy(s => s.Id);

                    break;
                }

                case SearchAccountSort.LastActive:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.LastActive).ThenBy(s => s.Id) 
                        : query.OrderBy(s => s.LastActive).ThenBy(s => s.Id);

                    break;
                }

                case SearchAccountSort.TotalLans:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.TotalEvents).ThenBy(s => s.Id) 
                        : query.OrderBy(s => s.TotalEvents).ThenBy(s => s.Id);

                    break;
                }

                default:
                {
                    query = request.SortDescending 
                        ? query.OrderByDescending(s => s.Id) 
                        : query.OrderBy(s => s.Id);

                    break;
                }
            }

            while (startPos > Int32.MaxValue)
            {
                query = query.Skip(Int32.MaxValue);

                startPos -= Int32.MaxValue;
            }

            return query.Skip((int) startPos).Take(request.PageSize).AsNoTracking().ToList();
        }

        public long GetAccountCount()
        {
            return Context.Account.LongCount();
        }

        public void AddAccount(UserAccount account)
        {
            Context.Account.Add(account);

            return;
        }

        public bool IsAccountVisible(UserAccount account)
        {
            return account.Visibility == AccountVisibility.Visible
                   || LocalAccount != null && (LocalAccount.Id == account.Id
                                               || account.Visibility == AccountVisibility.HiddenFromGuests
                                               || CheckAccess(FlagViewHiddenAccount));
        }

        public void UpdateActivity(UserAccount account)
        {
            account.LastActive = Instance.Time;

            // Attempt to save last active time, not important if fails to concurrency
            try
            {
                Context.SaveChanges();
            }
            catch (Exception) { }

            return;
        }

        /*
         * Account Access
         */

        public bool CheckAccess(String flag)
        {
            return LocalAccount != null && CheckAccess(LocalAccount, flag);
        }

        public bool CheckAccess(String flag, String scope)
        {
            return LocalAccount != null && CheckAccess(LocalAccount, flag, scope);
        }

        public bool CheckAccess(UserAccount account, String flag, String scope)
        {
            return CheckAccess(account, flag, scope, true);
        }

        public bool CheckAccess(UserAccount account, String flag, String scope, bool record)
        {
            bool success = account.Root || CheckAdminAccess(account.Id, flag, scope);

            if(record)
                AddAccessRecord(account, flag, scope, success);

            return success;
        }

        public bool CheckAccess(UserAccount account, String flag)
        {
            return CheckAccess(account, flag, "platform", true);
        }

        public bool CheckAccess(UserAccount account, String flag, bool record)
        {
            return CheckAccess(account, flag, "platform", record);
        }

        // Check the database for admin flags relating to an account ID
        protected bool CheckAdminAccess(long accountId, String flag, String scope)
        {
            bool success = (from p in Context.RoleFlag
                join r in Context.AccountRole on p.Role equals r.Role
                where r.User == accountId
                && p.Scope.Equals(scope, StringComparison.OrdinalIgnoreCase)
                && p.Flag.Equals(flag, StringComparison.OrdinalIgnoreCase)
                select p).AsNoTracking().FirstOrDefault() != null;

            return success;
        }

        public List<UserPermission> GetAccountFlags(UserAccount account)
        {
            return GetAccountFlags(account.Id);
        }

        public List<UserPermission> GetAccountFlags(long accountId)
        {
            List<UserPermission> accountFlags =
                Context.RoleFlag.Where(
                        flag =>
                            Context.AccountRole.Where(
                                role => role.User == accountId).Select(
                                role => role.Role).Contains(flag.Id))
                    .ToList();

            return accountFlags;
        }

        public List<UserPermission> GetAccountFlags(UserAccount account, String scope)
        {
            return GetAccountFlags(account.Id, scope);
        }

        public List<UserPermission> GetAccountFlags(long accountId, String scope)
        {
            return Context.RoleFlag.Where(
                        flag =>
                            Context.AccountRole.Where(
                                role => role.User == accountId).Select(
                                role => role.Role).Contains(flag.Id)
                                && flag.Scope.Equals(scope, StringComparison.OrdinalIgnoreCase))
                    .ToList();
        }

        public void AddAccessRecord(UserAccount account, String flag, String scope, bool success)
        {
            AccessRecord record = new AccessRecord();

            record.Account = account.Id;
            record.Time = Instance.Time;
            record.Success = success;
            record.Flag = flag;
            record.Scope = scope;

            AddAccessRecord(record);

            return;
        }

        public void AddAccessRecord(AccessRecord record)
        {
            LogContext.AccessRecord.Add(record);

            return;
        }

        /*
         * Roles
         */

        public void AddRole(UserRole role)
        {
            if (role.Id == 0)
            {
                Context.Role.Add(role);
            }

            return;
        }

        public List<UserRole> GetRolesByAccount(UserAccount account)
        {
            return GetRolesByAccount(account.Id);
        }

        public List<UserRole> GetRolesByAccount(long accountId)
        {
            return Context.Role.Where(role => Context.AccountRole.Where(accountRole => accountRole.User == accountId).Select(accountRole => accountRole.Role).Contains(role.Id)).ToList();
        }

        public UserRole GetRoleById(long id)
        {
            return Context.Role.SingleOrDefault(s => s.Id == id);
        }

        public UserRole GetRoleByName(String name)
        {
            return Context.Role.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public List<UserPermission> GetRolePermissions(long id)
        {
            return Context.RoleFlag.Where(s => s.Role == id).ToList();
        }

        public UserPermission GetPermission(long roleId, String flag, String scope)
        {
            return Context.RoleFlag.FirstOrDefault(s => s.Role == roleId &&
                                                        s.Flag.Equals(flag, StringComparison.OrdinalIgnoreCase) &&
                                                        s.Scope.Equals(scope, StringComparison.OrdinalIgnoreCase));
        }

        public List<UserRoleAccess> GetAccountRoleAccess(UserAccount account)
        {
            return GetAccountRoleAccess(account.Id);
        }

        public List<UserRoleAccess> GetAccountRoleAccess(long accountId)
        {
            return Context.AccountRole.Where(s => s.User == accountId).ToList();
        }

        public void AddAccountRoleAccess(UserRoleAccess access)
        {
            Context.AccountRole.Add(access);

            return;
        }

        public void AddPermission(UserPermission permission)
        {
            if (permission.Id == 0)
            {
                Context.RoleFlag.Add(permission);
            }

            return;
        }

        public void RemovePermission(UserPermission permission)
        {
            Context.RoleFlag.Remove(permission);

            return;
        }

        public void RemoveRoleAccess(UserRoleAccess access)
        {
            Context.AccountRole.Remove(access);

            return;
        }
    }
}