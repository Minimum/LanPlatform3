using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.DAL;
using LanPlatform.Settings;
using Newtonsoft.Json;

namespace LanPlatform.Models
{
    public class AppInstance
    {
        public static long CurrentTime => (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public String AppName => Properties.Resources.AppName;
        public String AppBuild => Properties.Resources.AppBuild;

        // Context Data
        [JsonIgnore]
        public AccountManager Accounts => AccountManager;
        protected AccountManager AccountManager;

        [JsonIgnore]
        public UserAccount LocalAccount => LocalUserAccount;
        protected UserAccount LocalUserAccount;

        [JsonIgnore]
        public SettingsManager Settings => SettingsManager;
        protected SettingsManager SettingsManager;

        [JsonIgnore]
        public HttpRequestMessage RequestMessage => Request;
        protected HttpRequestMessage Request;

        [JsonIgnore]
        public HttpContext RequestContext => InternalRequestContext;
        protected HttpContext InternalRequestContext;

        [JsonIgnore]
        public PlatformContext Context => DataContext;
        protected PlatformContext DataContext;

        [JsonIgnore]
        public List<CookieHeaderValue> Cookies => CookieList;
        protected List<CookieHeaderValue> CookieList;

        [JsonIgnore]
        public long Time { get; }
        [JsonIgnore]
        public bool LocalClient { get; }

        // Response Data
        public AppResponseStatus Status { get; set; }
        public String StatusCode { get; set; }

        public String DataType { get; set; }
        public Object Data { get; set; }

        [JsonIgnore]
        public EventHandler OnSaveFailure { get; set; }

        public AppInstance(HttpRequestMessage request, HttpContext requestContext)
        {
            Request = request;
            InternalRequestContext = requestContext;

            Status = AppResponseStatus.ResponseHandled;
            StatusCode = "";

            Time = CurrentTime;
            LocalClient = SettingsManager.LocalService && IsLocalAddress(requestContext.Request.UserHostAddress);

            DataType = "null";
            Data = null;

            OnSaveFailure = delegate { };

            DataContext = new PlatformContext();

            CookieList = new List<CookieHeaderValue>();

            AccountManager = new AccountManager(this);
            SettingsManager = new SettingsManager(this);

            LocalUserAccount = AccountManager.AuthenticateLocalUser();
        }

        public void SetData(Object data, String type)
        {
            Data = data;
            DataType = type;

            return;
        }

        public void AddCookie(String name, String value)
        {
            AddCookie(name, value, DateTimeOffset.Now.AddDays(7));

            return;
        }

        public void AddCookie(String name, String value, DateTimeOffset expiration)
        {
            CookieHeaderValue cookie = new CookieHeaderValue(name, value);
            cookie.Expires = expiration;
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";

            Cookies.Add(cookie);

            return;
        }

        public void SetError(String errorCode)
        {
            Status = AppResponseStatus.ResponseError;
            StatusCode = errorCode;

            return;
        }

        public void SetError(AppResponseStatus statusType, String errorCode)
        {
            Status = statusType;
            StatusCode = errorCode;

            return;
        }

        public bool CheckAccess(String flag) => Accounts.CheckAccess(flag);
        public bool CheckAccess(String flag, String scope) => Accounts.CheckAccess(flag, scope);

        public HttpResponseMessage ToResponse()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                Status = AppResponseStatus.AppError;
                
                StatusCode = "CONCURRENCY_ERROR";

                OnSaveFailure.Invoke(this, EventArgs.Empty);
            }

            response.Headers.AddCookies(Cookies);

            response.Content = new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");

            return response;
        }

        private static bool IsLocalAddress(String address)
        {
            bool local = false;

            if (address != null)
            {
                // Localhost
                // 192.168.0.0 - 192.168.255.255
                // 10.0.0.0 - 10.255.255.255
                local = address.Equals("localhost", StringComparison.OrdinalIgnoreCase) || address.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                        address.Equals("::1", StringComparison.OrdinalIgnoreCase) || 
                        address.StartsWith("10.") || address.StartsWith("192.168.");

                // 172.16.0.0 - 172.31.255.255
                if (!local && address.StartsWith("172."))
                {
                    local = address[4] == '1' && (address[5] == '6' || address[5] == '7' || address[5] == '8' || address[5] == '9') ||
                            address[4] == '2' ||
                            address[4] == '3' && (address[5] == '0' || address[5] == '1');
                }
            }

            return local;
        }
    }

    public enum AppResponseStatus
    {
        ResponseHandled = 0,    // Regular response, no errors
        ResponseError,          // Response halted due to a non-system issue
        AppNotInstalled,        // App has not been initialized yet
        AppDisabled,            // App is currently offline
        AppError,               // Response halted due to a system error (IE. database offline)
        AccessDenied            // Access is denied
    }
}