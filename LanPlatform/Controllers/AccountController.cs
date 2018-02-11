using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using LanPlatform.Content;
using LanPlatform.Accounts;
using LanPlatform.Auth;
using LanPlatform.DAL;
using LanPlatform.DAL.Logs;
using LanPlatform.DTO;
using LanPlatform.DTO.Accounts;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Models.Responses;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        // Basic account actions

        /*
         *  PUT api/account
         *  ---
         *  Info: Create a new user account.
         *  Access: Platform:AccountCreate
         */
        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateAccount([FromBody] UserAccountDto dto)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AccountContext context = new AccountContext();

            if (instance.CheckAccess(AccountManager.FlagCreateAccount))
            {
                UserAccount account = new UserAccount();

                account.AccountType = dto.AccountType;
                account.Gender = dto.Gender;
                account.FirstName = dto.FirstName;
                account.LastName = dto.LastName;
                account.Birthday = dto.Birthday;
                account.ContactEmail = dto.ContactEmail;
                account.ContactPhone = dto.ContactPhone;
                account.ContactFacebook = dto.ContactFacebook;
                account.ContactSteam = dto.ContactSteam;

                account.TotalEvents = dto.TotalEvents;
                account.EventOffset = dto.EventOffset;
                account.RemoteEvents = dto.RemoteEvents;
                account.LastEvent = dto.LastEvent;
                account.DisplayName = dto.DisplayName;
                // CustomUrl (default)
                // LastActive (default)
                // Avatar (default)
                account.Visibility = dto.Visibility;

                account.AwardsEnabled = dto.AwardsEnabled;
                account.AwardsXpEnabled = dto.AwardsXpEnabled;
                // AwardsLevel (default)
                // AwardsXp (default)

                context.Account.Add(account);

                try
                {
                    context.SaveChanges();

                    instance.SetData(new UserAccountDto(account), "UserAccount");
                }
                catch (Exception)
                {
                    instance.SetError("SaveError");
                }
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagCreateAccount);
            }

            return instance.ToResponse();
        }

        /*
         *  GET api/account/{accountId}
         *  ---
         *  Info: Get a user account info.
         */
        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetAccount(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (id > 0)
            {
                AccountContext context = new AccountContext();

                UserAccount account = (from a in context.Account where a.Id == id select a).FirstOrDefault();

                if (account != null)
                {
                    if (account.Visibility == AccountVisibility.Visible ||
                        account.Visibility == AccountVisibility.HiddenFromGuests && instance.LoggedIn ||
                        instance.CheckAccess(AccountManager.FlagViewHiddenAccount))
                    {
                        instance.SetData(new UserAccountDto(account));
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditAccount(long id, [FromBody] UserAccountDto userAccount)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            AccountContext context = instance.AccountContext;

            if (instance.LoggedIn)
            {
                UserAccount targetAccount = (from a in context.Account where a.Id == id select a).FirstOrDefault();

                if (targetAccount != null)
                {
                    if (userAccount.Id == localAccount.Id || instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditAccountBasic))
                    {
                        AccountLogContext logContext = instance.AccountLogContext;
                        AccountEditRecord editRecord = new AccountEditRecord();
                        HashSet<AccountEditField> editFields = new HashSet<AccountEditField>(); 

                        // Self editable fields

                        // Gender
                        if (targetAccount.Gender != userAccount.Gender)
                        {
                            editFields.Add(new AccountEditField("Gender", targetAccount.Gender, userAccount.Gender));

                            targetAccount.Gender = userAccount.Gender;
                        }

                        // First Name
                        if (targetAccount.FirstName != userAccount.FirstName)
                        {
                            editFields.Add(new AccountEditField("FirstName", targetAccount.FirstName, userAccount.LastName));

                            targetAccount.FirstName = userAccount.FirstName;
                        }

                        // Last Name
                        if (targetAccount.LastName != userAccount.LastName)
                        {
                            editFields.Add(new AccountEditField("LastName", targetAccount.LastName, userAccount.LastName));

                            targetAccount.LastName = userAccount.LastName;
                        }

                        // Birthday
                        if (targetAccount.Birthday != userAccount.Birthday)
                        {
                            editFields.Add(new AccountEditField("Birthday", targetAccount.Birthday, userAccount.Birthday));

                            targetAccount.Birthday = userAccount.Birthday;
                        }

                        // ContactEmail
                        if (targetAccount.ContactEmail != userAccount.ContactEmail)
                        {
                            editFields.Add(new AccountEditField("ContactEmail", targetAccount.ContactEmail, userAccount.ContactEmail));

                            targetAccount.ContactEmail = userAccount.ContactEmail;
                        }

                        // ContactPhone
                        if (targetAccount.ContactPhone != userAccount.ContactPhone)
                        {
                            editFields.Add(new AccountEditField("ContactPhone", targetAccount.ContactPhone, userAccount.ContactPhone));

                            targetAccount.ContactPhone = userAccount.ContactPhone;
                        }

                        // ContactFacebook
                        if (targetAccount.ContactFacebook != userAccount.ContactFacebook)
                        {
                            editFields.Add(new AccountEditField("ContactFacebook", targetAccount.ContactFacebook, userAccount.ContactFacebook));

                            targetAccount.ContactFacebook = userAccount.ContactFacebook;
                        }

                        // ContactSteam
                        if (targetAccount.ContactSteam != userAccount.ContactSteam)
                        {
                            editFields.Add(new AccountEditField("ContactSteam", targetAccount.ContactSteam, userAccount.ContactSteam));

                            targetAccount.ContactSteam = userAccount.ContactSteam;
                        }

                        // Display Name
                        if (targetAccount.DisplayName != userAccount.DisplayName)
                        {
                            editFields.Add(new AccountEditField("DisplayName", targetAccount.DisplayName, userAccount.DisplayName));

                            targetAccount.DisplayName = userAccount.DisplayName;
                        }

                        // Avatar
                        if (targetAccount.Avatar != userAccount.Avatar)
                        {
                            editFields.Add(new AccountEditField("Avatar", targetAccount.Avatar, userAccount.Avatar));

                            targetAccount.Avatar = userAccount.Avatar;
                        }

                        // Visibility
                        if (targetAccount.Visibility != userAccount.Visibility)
                        {
                            editFields.Add(new AccountEditField("Visibility", targetAccount.Visibility, userAccount.Visibility));

                            targetAccount.Visibility = userAccount.Visibility;
                        }

                        // Admin only editable fields

                        if (instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditAccountAdvanced))
                        {
                            // AccountType
                            if (targetAccount.AccountType != userAccount.AccountType)
                            {
                                editFields.Add(new AccountEditField("AccountType", targetAccount.AccountType, userAccount.AccountType));

                                targetAccount.AccountType = userAccount.AccountType;
                            }

                            // TotalEvents
                            if (targetAccount.TotalEvents != userAccount.TotalEvents)
                            {
                                editFields.Add(new AccountEditField("TotalEvents", targetAccount.TotalEvents, userAccount.TotalEvents));

                                targetAccount.TotalEvents = userAccount.TotalEvents;
                            }

                            // EventOffset
                            if (targetAccount.EventOffset != userAccount.EventOffset)
                            {
                                editFields.Add(new AccountEditField("EventOffset", targetAccount.EventOffset, userAccount.EventOffset));

                                targetAccount.EventOffset = userAccount.EventOffset;
                            }

                            // RemoteEvents
                            if (targetAccount.RemoteEvents != userAccount.RemoteEvents)
                            {
                                editFields.Add(new AccountEditField("RemoteEvents", targetAccount.RemoteEvents, userAccount.RemoteEvents));

                                targetAccount.RemoteEvents = userAccount.RemoteEvents;
                            }

                            // LastEvent
                            if (targetAccount.LastEvent != userAccount.LastEvent)
                            {
                                editFields.Add(new AccountEditField("LastEvent", targetAccount.LastEvent, userAccount.LastEvent));

                                targetAccount.LastEvent = userAccount.LastEvent;
                            }
                        }

                        // Attempt to save updated account info
                        bool saved = false;

                        try
                        {
                            context.SaveChanges();

                            instance.SetData(new UserAccountDto(targetAccount), "UserAccount");

                            saved = true;
                        }
                        catch (Exception e)
                        {
                            if (e is OptimisticConcurrencyException)
                            {
                                instance.SetError("ConcurrencyError");
                            }
                            else
                            {
                                instance.SetError("SaveError");
                            }
                        }

                        // Save logs if successful
                        if (saved)
                        {
                            bool logSuccess = false;

                            logContext.AccountEditRecord.Add(editRecord);

                            try
                            {
                                logContext.SaveChanges();

                                logSuccess = true;
                            }
                            catch (Exception)
                            {
                                System.Console.WriteLine("Failed to save account edit record.");
                            }

                            if (logSuccess)
                            {
                                foreach (AccountEditField field in editFields)
                                {
                                    field.Action = editRecord.Id;

                                    logContext.AccountEditField.Add(field);
                                }

                                try
                                {
                                    logContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    System.Console.WriteLine("Failed to save account edit record fields.");
                                }
                            }
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied(AccountManager.FlagEditAccountBasic);
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetAccessDenied("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}/avatar")]
        public HttpResponseMessage SetAvatar(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            ContentManager contentManager = new ContentManager(instance);
            AccountContext context = instance.AccountContext;

            if (instance.LoggedIn)
            {
                UserAccount target = instance.LocalAccount.Id == id
                    ? instance.LocalAccount
                    : (from a in context.Account where a.Id == id select a).FirstOrDefault();

                if (target != null)
                {
                    if (instance.LocalAccount.Id == id || (!target.Root && instance.CheckAccess(AccountManager.FlagEditAccountBasic)))
                    {
                        ContentItem content = contentManager.GetItemById(id);

                        if (content != null && content.Visible)
                        {
                            if (content.IsImage)
                            {
                                target.Avatar = id;

                                instance.SetData(true, "bool");
                            }
                            else
                            {
                                instance.SetError("InvalidContentType");
                            }
                        }
                        else
                        {
                            instance.SetError("InvalidContent");
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied(AccountManager.FlagEditAccountBasic);
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetAccessDenied("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/role")]
        public HttpResponseMessage GetRoles(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);
            AccountContext context = instance.AccountContext;

            if (target != null)
            {
                if (instance.Accounts.IsAccountVisible(target))
                {
                    List<UserRole> roles = (from r in context.Role
                        join ar in context.AccountRole on r.Id equals ar.Role
                        where ar.User == id
                        select r).ToList();

                    instance.SetData(UserRoleDto.ConvertList(roles), "UserRoleList");
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("{id}/role/{roleId}")]
        public HttpResponseMessage AddRole(long id, long roleId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);
            AccountContext context = instance.AccountContext;

            if (target != null)
            {
                if (instance.Accounts.CheckAccess(AccountManager.FlagEditAccountAdvanced) 
                    && (!target.Root || target.Id == instance.LocalAccount.Id))
                {
                    int roleCount = (from r in context.AccountRole where r.User == id && r.Role == roleId select r).Count();

                    if (roleCount < 1)
                    {
                        UserRoleAccess access = new UserRoleAccess
                        {
                            Role = roleId,
                            User = id
                        };

                        context.AccountRole.Add(access);

                        try
                        {
                            context.SaveChanges();

                            instance.SetData(true, "bool");
                        }
                        catch (Exception)
                        {
                            instance.SetError("SaveError");
                        }
                    }
                    else
                    {
                        instance.SetData(true, "bool");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditAccountAdvanced);
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}/role/{roleId}")]
        public HttpResponseMessage RemoveRole(long id, long roleId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);

            if (target != null)
            {
                if (instance.Accounts.CheckAccess(AccountManager.FlagEditAccountAdvanced)
                    && (!target.Root || target.Id == instance.LocalAccount.Id))
                {
                    AccountContext context = instance.AccountContext;
                    List<UserRoleAccess> roles = (from a in context.AccountRole where a.User == id && a.Role == roleId select a).ToList();

                    context.AccountRole.RemoveRange(roles);

                    try
                    {
                        context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception)
                    {
                        instance.SetError("SaveError");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditAccountAdvanced);
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/access")]
        public HttpResponseMessage GetUserPermissions(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);
            AccountContext context = instance.AccountContext;

            if (instance.LoggedIn)
            {
                if (target != null)
                {
                    if (instance.Accounts.IsAccountVisible(target))
                    {
                        List<UserPermission> permissions = (from p in context.RoleFlag
                            join r in context.AccountRole on p.Role equals r.Role
                            where r.User == id
                            select p).ToList();

                        instance.SetData(UserPermissionDto.ConvertList(permissions), "UserPermissionList");
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/access/{scope}")]
        public HttpResponseMessage GetUserScopePermissions(long id, String scope)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);
            AccountContext context = instance.AccountContext;

            if (instance.LoggedIn)
            {
                if (target != null)
                {
                    if (instance.Accounts.IsAccountVisible(target))
                    {
                        List<UserPermission> permissions = (from p in context.RoleFlag
                            join r in context.AccountRole on p.Role equals r.Role
                            where r.User == id
                            && p.Scope.Equals(scope, StringComparison.OrdinalIgnoreCase)
                            select p).ToList();

                        instance.SetData(UserPermissionDto.ConvertList(permissions), "UserPermissionList");
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/access/{scope}/{flag}")]
        public HttpResponseMessage GetUserSinglePermission(long id, String scope, String flag)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);

            if (instance.LoggedIn)
            {
                if (target != null)
                {
                    if (instance.Accounts.IsAccountVisible(target))
                    {
                        instance.SetData(instance.Accounts.CheckAccess(target, flag, scope), "bool");
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("AnonymousUser");
            }

            return instance.ToResponse();
        }

        // Account auth (username) actions

        [HttpGet]
        [Route("{id}/auth/user")]
        public HttpResponseMessage GetAccountUsernames(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;

            // Check if request is valid
            if (id > 0)
            {
                // Check if user has access
                if (localAccount != null && (localAccount.Id == id || instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditUsername)))
                {
                    UserAccount targetAccount = instance.Accounts.GetAccountReadOnly(id);

                    // Check if target is valid
                    if (targetAccount != null)
                    {
                        // Check if target is protected
                        if (!targetAccount.Root || targetAccount.Id == localAccount.Id)
                        {
                            List<AuthUsername> usernames = (from u in instance.AccountContext.AuthUsername
                                where u.Account == targetAccount.Id select u).ToList();

                            instance.SetData(AuthUsernameDto.ConvertList(usernames), "AuthUsernameList");
                        }
                        else
                        {
                            instance.SetError("TargetImmunity");
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditUsername);
                }
            }
            else
            {
                instance.SetError("InvalidRequest");
            }

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("{id}/auth/user")]
        public HttpResponseMessage CreateUsername(long id, [FromBody] AddUsernameRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            AccountContext context = instance.AccountContext;

            // Check if request is valid
            if (id > 0 && request.Username.Length > 0 && request.Password.Length > 0)
            {
                // Check user's access
                if (localAccount != null && instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditUsername))
                {
                    UserAccount targetAccount = instance.Accounts.GetAccount(id);

                    // Check if target is valid
                    if (targetAccount != null)
                    {
                        // Check if target is protected
                        if (!targetAccount.Root || targetAccount.Id == localAccount.Id)
                        {
                            // Check if username already exists
                            AuthUsername username = (from u in context.AuthUsername
                                where u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)
                                select u).FirstOrDefault();

                            if (username == null)
                            {
                                username = instance.Accounts.CreateUsername(id, request.Username, request.Password);

                                try
                                {
                                    context.SaveChanges();

                                    instance.SetData(new AuthUsernameDto(username), "AuthUsername");
                                }
                                catch (Exception)
                                {
                                    instance.SetError("SaveError");
                                }
                            }
                            else
                            {
                                instance.SetError("UsernameExists");
                            }
                        }
                        else
                        {
                            instance.SetError("TargetImmunity");
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditUsername);
                }
            }
            else
            {
                instance.SetError("InvalidRequest");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}/auth/user/{username}")]
        public HttpResponseMessage EditUsername(long id, String name, [FromBody] AuthUsernameDto username)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AccountContext context = instance.AccountContext;

            // Check if request is valid
            if (id > 0 && username.Password.Length > 0)
            {
                UserAccount target = instance.Accounts.GetAccount(id);

                if (target != null)
                {
                    // Check user's access
                    if (id == instance.LocalAccount?.Id ||
                        instance.Accounts.CheckAccess(AccountManager.FlagEditUsername) && !target.Root)
                    {
                        AuthUsername targetUsername = (from u in context.AuthUsername
                            where u.Username.Equals(name, StringComparison.OrdinalIgnoreCase) && u.Account == id
                            select u).FirstOrDefault();

                        if (targetUsername?.Account == id)
                        {
                            targetUsername.Salt = AuthUsername.GenerateSalt();
                            targetUsername.Password =
                                AuthUsername.EncryptPassword(username.Password, targetUsername.Salt);

                            // TODO: Log action

                            try
                            {
                                context.SaveChanges();

                                instance.SetData(true, "bool");
                            }
                            catch (Exception e)
                            {
                                if (e is OptimisticConcurrencyException)
                                {
                                    instance.SetError("ConcurrencyError");
                                }
                                else
                                {
                                    instance.SetError("SaveError");
                                }
                            }
                        }
                        else
                        {
                            instance.SetError("InvalidUsername");
                        }
                    }
                    else
                    {
                        instance.SetAccessDenied(AccountManager.FlagEditUsername);
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetError("InvalidRequest");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}/auth/user/{username}")]
        public HttpResponseMessage DeleteUsername(long id, String name)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);
            AccountContext context = instance.AccountContext;

            if (target != null)
            {
                if (id == instance.LocalAccount?.Id ||
                    instance.Accounts.CheckAccess(AccountManager.FlagEditUsername) && !target.Root)
                {
                    AuthUsername username = (from u in context.AuthUsername
                        where u.Username.Equals(name, StringComparison.OrdinalIgnoreCase) && u.Account == id
                        select u).FirstOrDefault();

                    if (username?.Account == id)
                    {
                        context.AuthUsername.Remove(username);

                        try
                        {
                            context.SaveChanges();

                            instance.SetData(true, "bool");
                        }
                        catch (Exception)
                        {
                            instance.SetError("SaveError");
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidUsername");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditUsername);
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }

        // Account auth (session) actions

        [HttpGet]
        [Route("{id}/auth/session")]
        public HttpResponseMessage GetAccountSessions(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            AccountContext context = instance.AccountContext;

            // Check if request is valid
            if (id > 0)
            {
                // Check if user has access
                if (localAccount != null && (localAccount.Id == id || instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditSession)))
                {
                    UserAccount targetAccount = instance.Accounts.GetAccountReadOnly(id);

                    // Check if target is valid
                    if (targetAccount != null)
                    {
                        // Check if target is protected
                        if (!targetAccount.Root || targetAccount.Id == localAccount.Id)
                        {
                            List<AuthSession> sessions = (from s in context.AuthSession where s.Account == id select s).ToList();

                            instance.SetData(AuthSessionDto.ConvertList(sessions), "AuthSessionList");
                        }
                        else
                        {
                            instance.SetError("TargetImmunity");
                        }
                    }
                    else
                    {
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AccountManager.FlagEditSession);
                }
            }
            else
            {
                instance.SetError("InvalidRequest");
            }

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("account/{id}/auth/session")]
        public HttpResponseMessage CreateSession(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (id == instance.LocalAccount?.Id)
            {
                AuthSession session = instance.Accounts.CreateSession(instance.LocalAccount);

                try
                {
                    instance.Context.SaveChanges();

                    instance.SetData(new AuthSessionDto(session), "AuthSession");
                }
                catch (Exception)
                {
                    instance.SetError("SaveError");
                }
            }
            else
            {
                instance.SetAccessDenied("InvalidUser");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("account/{id}/auth/session/{authId}")]
        public HttpResponseMessage DeleteSession(long id, long authId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AccountContext context = instance.AccountContext;

            if (id == instance.LocalAccount?.Id)
            {
                AuthSession session = (from s in context.AuthSession where s.Id == authId && s.Account == id select s).FirstOrDefault();

                if (session != null)
                {
                    context.AuthSession.Remove(session);

                    try
                    {
                        context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception)
                    {
                        instance.SetError("SaveError");
                    }
                }
                else
                {
                    instance.SetError("InvalidSession");
                }
            }
            else
            {
                instance.SetAccessDenied("InvalidUser");
            }

            return instance.ToResponse();
        }

        // Account auth (challenge) actions
        // This is a feature planned for the future to allow outside services such as game servers to properly authenticate accounts

        // GET		/{id}/auth/challenge

        // PUT		/{id}/auth/challenge

        // GET		/{id}/auth/challenge/{challengeId}

        // DELETE	/{id}/auth/challenge/{challengeId}

        // Account search

        [HttpGet]
        [Route("search")]
        public HttpResponseMessage SearchAccounts([FromUri] SearchAccountRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            request.SanityCheck();

            List<UserAccount> dataAccounts = instance.Accounts.GetAccountSearch(request);

            if (dataAccounts != null)
            {
                BrowseResult<GabionDto> accounts = new BrowseResult<GabionDto>();

                accounts.TotalResults = instance.Accounts.GetAccountCount();

                accounts.AddRange(UserAccountDto.ConvertList(dataAccounts));

                instance.SetData(accounts, "UserAccountBrowseList");
            }
            else
            {
                instance.SetError(AppResponseStatus.AppError, "LoadError");
            }

            return instance.ToResponse();
        }

        // Local account actions

        [HttpGet]
        [Route("local")]
        public HttpResponseMessage WhoAmI()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.LoggedIn)
            {
                instance.SetData(new UserAccountDto(instance.LocalAccount), "UserAccount");
            }
            else
            {
                instance.SetError("AnonymousUser");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("local/access/{scope}/{flag}")]
        public HttpResponseMessage CheckLocalAccess(String flag, String scope)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;

            if (instance.LoggedIn)
            {
                instance.SetData(instance.Accounts.CheckAccess(localAccount, flag, scope, false), "bool");
            }
            else
            {
                instance.SetError("AnonymousUser");
            }

            return instance.ToResponse();
        }

        // Authentication

        [HttpPost]
        [Route("logout")]
        public HttpResponseMessage Logout()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AccountContext context = instance.AccountContext;

            if (instance.LoggedIn)
            {
                HttpCookie sessionId = HttpContext.Current.Request.Cookies["LPSessionId"];

                AuthSession session = (from s in context.AuthSession
                    where s.Id == AuthSession.GetIdFromCookie(sessionId) && s.Account == instance.LocalAccount.Id
                    select s).FirstOrDefault();

                if(session != null)
                    context.AuthSession.Remove(session);

                CookieHeaderValue sessionIdCookie = new CookieHeaderValue("LPSessionId", "");
                CookieHeaderValue sessionKeyCookie = new CookieHeaderValue("LPSessionKey", "");

                sessionIdCookie.Expires = DateTimeOffset.Now.AddDays(-1d);
                sessionKeyCookie.Expires = DateTimeOffset.Now.AddDays(-1d);

                instance.Cookies.Add(sessionIdCookie);
                instance.Cookies.Add(sessionKeyCookie);
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("login/user")]
        public HttpResponseMessage LoginUsername([FromBody] UserLoginRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.Accounts.AuthByUsername(request.Username, request.Password);

            if (localAccount != null)
            {
                AuthSession session = instance.Accounts.CreateSession(localAccount);

                instance.Context.SaveChanges();

                DateTimeOffset expiration = DateTimeOffset.Now.AddDays(30);

                instance.AddCookie("LPSessionId", session.Id.ToString(), expiration);
                instance.AddCookie("LPSessionKey", session.Key, expiration);

                instance.Accounts.UpdateActivity(localAccount);

                // TODO: Log successful attempt

                instance.SetData(new UserAccountDto(localAccount), "UserAccount");
            }
            else
            {
                // TODO: Log failed attempt
                
                instance.SetError("InvalidAccount");
            }

            return instance.ToResponse();
        }
    }
}
