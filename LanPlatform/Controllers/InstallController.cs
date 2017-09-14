using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Auth;
using LanPlatform.Accounts;
using LanPlatform.Apps;
using LanPlatform.Events;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.News;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/install")]
    public class InstallController : ApiController
    {
        [HttpGet]
        [Route("account")]
        public HttpResponseMessage InitAccount()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("account")]
        public HttpResponseMessage InitAccount([FromBody] InstallAccountRequest accountRequest)
        {
            String installStatus = ConfigurationManager.AppSettings["LPInstallStatus"];
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            // Check if an install is needed
            if (installStatus == null || installStatus.Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);

                // Check if account details are valid
                if (accountRequest != null && accountRequest.DisplayName.Length > 0
                    && accountRequest.Username.Length > 0
                    && accountRequest.Password.Length > 0)
                {
                    AccountManager manager = instance.Accounts;

                    // Create account
                    UserAccount account = new UserAccount();

                    account.Root = true;
                    account.DisplayName = accountRequest.DisplayName;

                    manager.AddAccount(account);

                    // Create account login
                    manager.CreateUsername(1, accountRequest.Username, accountRequest.Password);

                    // Save changes to database
                    instance.Context.SaveChanges();

                    // Set install status to the next step
                    ConfigurationManager.AppSettings["LPInstallStatus"] = "2";

                    // Root account successfully installed
                    response.Content = new StringContent("0", Encoding.UTF8, "text/plain");
                }
                else
                {
                    // Invalid account details
                    response.Content = new StringContent("1", Encoding.UTF8, "text/plain");
                }
            }

            return response;
        }

        [HttpGet]
        [Route("settings")]
        public HttpResponseMessage InitSettingsGet()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [HttpPost]
        [Route("settings")]
        public HttpResponseMessage InitSettings()
        {
            String installStatus = ConfigurationManager.AppSettings["LPInstallStatus"];
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NotFound);
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            // Check if an install is needed
            if (installStatus != null && installStatus.Equals("2", StringComparison.OrdinalIgnoreCase) ||
                instance.LocalAccount != null && instance.Accounts.CheckAccess(instance.LocalAccount, "TrustedInstaller"))
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);

                // Install settings
                instance.Accounts.Install();
                new LanEventManager(instance).Install();
                new NewsManager(instance).Install();
                new AppManager(instance).Install();

                // Save settings
                instance.Context.SaveChanges();

                // Set install status to finished
                ConfigurationManager.AppSettings["LPInstallStatus"] = "0";

                // Settings successfully installed
                response.Content = new StringContent("0", Encoding.UTF8, "text/plain");
            }

            return response;
        }
    }
}