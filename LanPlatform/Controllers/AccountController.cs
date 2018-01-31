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

        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateAccount([FromBody] UserAccountDto dto)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.Accounts.CheckAccess(instance.LocalAccount, AccountManager.FlagCreateAccount))
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
                account.Visibility = dto.Visibility;

                account.AwardsEnabled = dto.AwardsEnabled;
                account.AwardsXpEnabled = dto.AwardsXpEnabled;

                instance.Accounts.AddAccount(account);

                instance.Context.SaveChanges();

                instance.SetData(new UserAccountDto(account), "UserAccount");
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagCreateAccount);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetAccount(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (id > 0)
            {
                UserAccount account = instance.Accounts.GetAccountReadOnly(id);

                if (account != null)
                {
                    instance.SetData(new UserAccountDto(account), "UserAccount");
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

            if (instance.LoggedIn && userAccount != null)
            {
                UserAccount targetAccount = instance.Accounts.GetAccount(id);

                if (targetAccount != null)
                {
                    if (userAccount.Id == localAccount.Id || instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditAccountBasic))
                    {
                        AccountEditRecord editRecord = new AccountEditRecord();

                        // Self editable fields

                        // Gender
                        if (targetAccount.Gender != userAccount.Gender)
                        {
                            editRecord.AddField("Gender", targetAccount.Gender, userAccount.Gender);

                            targetAccount.Gender = userAccount.Gender;
                        }

                        // First Name
                        if (targetAccount.FirstName != userAccount.FirstName)
                        {
                            editRecord.AddField("FirstName", targetAccount.FirstName, userAccount.LastName);

                            targetAccount.FirstName = userAccount.FirstName;
                        }

                        // Last Name
                        if (targetAccount.LastName != userAccount.LastName)
                        {
                            editRecord.AddField("LastName", targetAccount.LastName, userAccount.LastName);

                            targetAccount.LastName = userAccount.LastName;
                        }

                        // Birthday
                        if (targetAccount.Birthday != userAccount.Birthday)
                        {
                            editRecord.AddField("Birthday", targetAccount.Birthday, userAccount.Birthday);

                            targetAccount.Birthday = userAccount.Birthday;
                        }

                        // ContactEmail
                        if (targetAccount.ContactEmail != userAccount.ContactEmail)
                        {
                            editRecord.AddField("ContactEmail", targetAccount.ContactEmail, userAccount.ContactEmail);

                            targetAccount.ContactEmail = userAccount.ContactEmail;
                        }

                        // ContactPhone
                        if (targetAccount.ContactPhone != userAccount.ContactPhone)
                        {
                            editRecord.AddField("ContactPhone", targetAccount.ContactPhone, userAccount.ContactPhone);

                            targetAccount.ContactPhone = userAccount.ContactPhone;
                        }

                        // ContactFacebook
                        if (targetAccount.ContactFacebook != userAccount.ContactFacebook)
                        {
                            editRecord.AddField("ContactFacebook", targetAccount.ContactFacebook, userAccount.ContactFacebook);

                            targetAccount.ContactFacebook = userAccount.ContactFacebook;
                        }

                        // ContactSteam
                        if (targetAccount.ContactSteam != userAccount.ContactSteam)
                        {
                            editRecord.AddField("ContactSteam", targetAccount.ContactSteam, userAccount.ContactSteam);

                            targetAccount.ContactSteam = userAccount.ContactSteam;
                        }

                        // Display Name
                        if (targetAccount.DisplayName != userAccount.DisplayName)
                        {
                            editRecord.AddField("DisplayName", targetAccount.DisplayName, userAccount.DisplayName);

                            targetAccount.DisplayName = userAccount.DisplayName;
                        }

                        // Avatar
                        if (targetAccount.Avatar != userAccount.Avatar)
                        {
                            editRecord.AddField("Avatar", targetAccount.Avatar, userAccount.Avatar);

                            targetAccount.Avatar = userAccount.Avatar;
                        }

                        // Visibility
                        if (targetAccount.Visibility != userAccount.Visibility)
                        {
                            editRecord.AddField("Visibility", targetAccount.Visibility, userAccount.Visibility);

                            targetAccount.Visibility = userAccount.Visibility;
                        }

                        // Admin only editable fields

                        if (instance.Accounts.CheckAccess(localAccount, AccountManager.FlagEditAccountAdvanced))
                        {
                            // AccountType
                            if (targetAccount.AccountType != userAccount.AccountType)
                            {
                                editRecord.AddField("AccountType", targetAccount.AccountType, userAccount.AccountType);

                                targetAccount.AccountType = userAccount.AccountType;
                            }

                            // TotalEvents
                            if (targetAccount.TotalEvents != userAccount.TotalEvents)
                            {
                                editRecord.AddField("TotalEvents", targetAccount.TotalEvents, userAccount.TotalEvents);

                                targetAccount.TotalEvents = userAccount.TotalEvents;
                            }

                            // EventOffset
                            if (targetAccount.EventOffset != userAccount.EventOffset)
                            {
                                editRecord.AddField("EventOffset", targetAccount.EventOffset, userAccount.EventOffset);

                                targetAccount.EventOffset = userAccount.EventOffset;
                            }

                            // RemoteEvents
                            if (targetAccount.RemoteEvents != userAccount.RemoteEvents)
                            {
                                editRecord.AddField("RemoteEvents", targetAccount.RemoteEvents, userAccount.RemoteEvents);

                                targetAccount.RemoteEvents = userAccount.RemoteEvents;
                            }

                            // LastEvent
                            if (targetAccount.LastEvent != userAccount.LastEvent)
                            {
                                editRecord.AddField("LastEvent", targetAccount.LastEvent, userAccount.LastEvent);

                                targetAccount.LastEvent = userAccount.LastEvent;
                            }
                        }

                        try
                        {
                            instance.Context.SaveChanges();

                            instance.SetData(new UserAccountDto(targetAccount), "UserAccount");
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

            UserAccount account = instance.LocalAccount;

            if (instance.LoggedIn)
            {
                if (account.Id == id || instance.CheckAccess(AccountManager.FlagEditAccountBasic))
                {
                    ContentItem content = contentManager.GetItemById(id);

                    if (content != null && content.Visible)
                    {
                        if (content.IsImage)
                        {
                            account.Avatar = id;

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

            if (target != null)
            {
                if (instance.Accounts.IsAccountVisible(target))
                {
                    List<UserRole> roles = instance.Accounts.GetRolesByAccount(target);

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

            if (target != null)
            {
                if (instance.Accounts.CheckAccess(AccountManager.FlagEditAccountAdvanced) 
                    && (!target.Root || target.Id == instance.LocalAccount.Id))
                {
                    List<UserRole> roles = instance.Accounts.GetRolesByAccount(id);

                    if (roles.All(s => s.Id != roleId))
                    {
                        UserRoleAccess access = new UserRoleAccess
                        {
                            Role = roleId,
                            User = id
                        };

                        instance.Accounts.AddAccountRoleAccess(access);

                        try
                        {
                            instance.Context.SaveChanges();

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
                    PlatformContext context = instance.Context;
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

            if (target != null)
            {
                if (instance.Accounts.IsAccountVisible(target))
                {
                    List<UserPermission> permissions = instance.Accounts.GetAccountFlags(target);

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

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/access/{scope}")]
        public HttpResponseMessage GetUserScopePermissions(long id, String scope)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);

            if (target != null)
            {
                if (instance.Accounts.IsAccountVisible(target))
                {
                    List<UserPermission> permissions = instance.Accounts.GetAccountFlags(id, scope);

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

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}/access/{scope}/{flag}")]
        public HttpResponseMessage GetUserSinglePermission(long id, String scope, String flag)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount target = instance.Accounts.GetAccount(id);

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
                            List<AuthUsername> usernames = instance.Accounts.GetAccountUsernames(targetAccount);

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
                            if (instance.Accounts.GetUsername(request.Username) == null)
                            {
                                AuthUsername username = instance.Accounts.CreateUsername(id, request.Username, request.Password);

                                try
                                {
                                    instance.Context.SaveChanges();

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
                        AuthUsername targetUsername = instance.Accounts.GetUsername(name);

                        if (targetUsername?.Account == id)
                        {
                            targetUsername.Salt = AuthUsername.GenerateSalt();
                            targetUsername.CryptPassword =
                                AuthUsername.EncryptPassword(username.Password, targetUsername.Salt);

                            // TODO: Log action

                            try
                            {
                                instance.Context.SaveChanges();

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

            if (target != null)
            {
                if (id == instance.LocalAccount?.Id ||
                    instance.Accounts.CheckAccess(AccountManager.FlagEditUsername) && !target.Root)
                {
                    AuthUsername username = instance.Accounts.GetUsername(name);

                    if (username?.Account == id)
                    {
                        instance.Accounts.RemoveUsername(username);

                        try
                        {
                            instance.Context.SaveChanges();

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
                            List<AuthSession> sessions = instance.Accounts.GetAccountSessions(targetAccount);

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

            if(id == instance.LocalAccount?.Id)
            {
                AuthSession session = instance.Accounts.GetSession(authId);

                if (session?.Account == id)
                {
                    instance.Accounts.RemoveSession(session);

                    try
                    {
                        instance.Context.SaveChanges();

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
            
            if (instance.LoggedIn)
            {
                HttpCookie sessionId = HttpContext.Current.Request.Cookies["LPSessionId"];

                instance.Accounts.RemoveSession(AuthSession.GetIdFromCookie(sessionId));

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
