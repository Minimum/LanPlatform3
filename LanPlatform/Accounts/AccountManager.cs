using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using LanPlatform.Auth;
using LanPlatform.DAL;
using LanPlatform.Engine;
using LanPlatform.Events;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Network;
using LanPlatform.Network.Messages;
using LanPlatform.Settings;

namespace LanPlatform.Accounts
{
    public class AccountManager
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

        protected PlatformContext Context;
        protected UserAccount LocalAccount;

        protected AppInstance Instance;

        public AccountManager(AppInstance instance)
        {
            Context = instance.Context;
            LocalAccount = null;

            Instance = instance;
        }

        public void Install()
        {
            SettingsManager settings = Instance.Settings;

            settings.AddSetting(new PlatformSetting(SettingAllowRegister, "Allow Account Registrations",
                "Allows new users to register.", "0"));

            settings.AddSetting(new PlatformSetting(SettingDefaultAvatar, "Default Avatar", "The default avatar for users.", "0"));

            AccessRecord accessRecord = Context.AccessRecord.SingleOrDefault(s => s.Id == 0);
            UserAccount account = Context.Account.SingleOrDefault(s => s.Id == 0);
            AccountEditField editField = Context.AccountEditField.SingleOrDefault(s => s.Id == 0);
            AccountEditRecord editRecord = Context.AccountEditRecord.SingleOrDefault(s => s.Id == 0);
            UserRoleAccess accountRole = Context.AccountRole.SingleOrDefault(s => s.Id == 0);
            AuthSession session = Context.AuthSession.SingleOrDefault(s => s.Id == 0);
            AuthSessionAttempt sessionAttempt = Context.AuthSessionAttempt.SingleOrDefault(s => s.Id == 0);
            AuthUsername username = Context.AuthUsername.SingleOrDefault(s => s.Id == 0);
            AuthUsernameAttempt usernameAttempt = Context.AuthUsernameAttempt.SingleOrDefault(s => s.Id == 0);
            UserRole role = Context.Role.SingleOrDefault(s => s.Id == 0);

            return;
        }

        /*
         * General auth
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
                    // Mark user as active if previously inactive
                    if (EngineUtil.CurrentTime >= localAccount.LastActive + 1800)
                    {
                        NetMessageManager.AddMessageBroadcastQuick(Instance, new NewActiveUserMessage(localAccount.Id));
                    }

                    // Update account's last active time
                    UpdateActivity(localAccount);
                }
                else
                {
                    // Log invalid session key attempt
                    Context.AuthSessionAttempt.Add(new AuthSessionAttempt(Instance, id, sessionKey.Value));
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

        public void PostAuthTasks(UserAccount account)
        {
            LanEventManager.PostAuthTasks(account, Instance);

            return;
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
            AuthUsername auth = null;

            // Check if username already exists
            if (GetUsername(username) == null)
            {
                // Create new auth
                auth = AuthUsername.CreateAuth(username, password);

                auth.Account = accountId;

                Context.AuthUsername.Add(auth);
            }

            return auth;
        }

        public AuthUsername GetUsername(String username)
        {
            return Context.AuthUsername.SingleOrDefault(s => s.Username.Equals(username, StringComparison.Ordinal));
        }

        public List<AuthUsername> GetAccountUsernames(UserAccount account)
        {
            return Context.AuthUsername.Where(s => s.Account == account.Id).ToList();
        }

        public void RemoveUsername(AuthUsername username)
        {
            Context.AuthUsername.Remove(username);

            return;
        }

        public UserAccount AuthByUsername(String username, String password)
        {
            UserAccount account = null;
            AuthUsername auth = GetUsername(username);

            // Authenticate
            if (auth != null && auth.Authenticate(password))
            {
                // Load account
                account = GetAccount(auth.Account);
            }

            if (account != null)
            {
                PostAuthTasks(account);
            }

            return account;
        }
        
        /*
         * Auth (Sessions)
         */

        public AuthSession GetSession(long id)
        {
            return Context.AuthSession.SingleOrDefault(s => s.Id == id);
        }

        public List<AuthSession> GetAccountSessions(UserAccount account)
        {
            return Context.AuthSession.Where(s => s.Account == account.Id).ToList();
        }

        public AuthSession CreateSession(UserAccount account)
        {
            AuthSession session = new AuthSession();

            session.Account = account.Id;
            session.Key = AuthSession.GenerateKey();

            Context.AuthSession.Add(session);

            return session;
        }

        public void RemoveSession(AuthSession session)
        {
            Context.AuthSession.Remove(session);

            return;
        }

        public void RemoveSession(long id)
        {
            AuthSession session = GetSession(id);

            if (session != null)
            {
                Context.AuthSession.Remove(session);
            }

            return;
        }

        public UserAccount AuthBySession(long sessionId, String key)
        {
            UserAccount account = null;
            AuthSession auth = GetSession(sessionId);

            // Authenticate
            if (auth != null && auth.Authenticate(sessionId, key))
            {
                // Load account
                account = GetAccount(auth.Account);
            }

            if (account != null)
            {
                PostAuthTasks(account);
            }

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
            account.LastActive = EngineUtil.CurrentTime;

            // Attempt to save last active time, not important if fails to concurrency
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e) { }

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
            bool success = Context.RoleFlag
                // Role Flag Table
                            .Where(roleFlag => flag.Equals(roleFlag.Flag, StringComparison.OrdinalIgnoreCase) &&
                                scope.Equals(roleFlag.Scope, StringComparison.OrdinalIgnoreCase) &&
                                // Role Table
                                Context.Role.Where(role =>
                                    // Account Role Table
                                    Context.AccountRole.Where(accountRole => accountRole.User == accountId)
                                    .Select(accountRole => accountRole.Role).Contains(role.Id))
                                .Select(role => role.Id).Contains(roleFlag.Role)
                            ).AsNoTracking().FirstOrDefault() != null;

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

        public void AddAccessRecord(UserAccount account, String flag, String scope, bool success)
        {
            AccessRecord record = new AccessRecord();

            record.Account = account.Id;
            record.Time = EngineUtil.CurrentTime;
            record.Success = success;
            record.Flag = flag;
            record.Scope = scope;

            AddAccessRecord(record);

            return;
        }

        public void AddAccessRecord(AccessRecord record)
        {
            Context.AccessRecord.Add(record);

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