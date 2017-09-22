using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.Apps;
using LanPlatform.DTO.Apps;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Network;
using LanPlatform.Network.Messages;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/loaner")]
    public class LoanerController : ApiController
    {
        [HttpGet]
        [Route("all")]
        public HttpResponseMessage GetAllLoaners()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            instance.Data = LoanerAccountDto.ConvertList(apps.GetLoanerAccounts());

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateLoaner([FromBody] LoanerAccountDto account)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (instance.Accounts.CheckAccess(localAccount, AppManager.FlagLoanerEdit))
            {
                if (account.Username.Length > 0)
                {
                    LoanerAccount loaner = new LoanerAccount();

                    loaner.Username = account.Username;
                    loaner.Password = account.Password;

                    apps.AddLoanerAccount(loaner);

                    instance.Context.SaveChanges();

                    instance.SetData(loaner, "LoanerAccount");
                }
                else
                {
                    instance.SetError("INVALID_ACCOUNT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetLoaner(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            instance.Data = new LoanerAccountDto(apps.GetLoanerAccount(id));

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditLoaner(long id, [FromBody] LoanerAccountDto loaner)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (loaner != null)
            {
                if (instance.Accounts.CheckAccess(AppManager.FlagLoanerEdit))
                {
                    LoanerAccount dataLoaner = apps.GetLoanerAccount(id);

                    if (dataLoaner != null)
                    {
                        dataLoaner.Username = loaner.Username;
                        dataLoaner.Password = loaner.Password;

                        instance.Context.SaveChanges();
                    }
                    else
                    {
                        instance.SetError("INVALID_ACCOUNT");
                    }
                }
                else
                {
                    instance.SetError("ACCESS_DENIED");
                }
            }
            else
            {
                instance.SetError("INVALID_DTO");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}")]
        public HttpResponseMessage DeleteLoaner(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagLoanerDelete))
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner != null)
                {
                    List<LoanerApp> loanerApps = apps.GetLoanerApps(loaner);

                    foreach (LoanerApp app in loanerApps)
                    {
                        apps.RemoveLoanerApp(app);
                    }

                    apps.RemoveLoanerAccount(loaner);

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception e)
                    {
                        instance.SetError("SAVE_ERROR");
                    }
                }
                else
                {
                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        // Loaner apps

        [HttpPut]
        [Route("{id}/app/{appId}")]
        public HttpResponseMessage AddLoanerApp(long id, long appId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagLoanerEdit))
            {
                LoanerAccount account = apps.GetLoanerAccount(id);

                if (account != null)
                {
                    LoanerApp loanerApp = apps.GetLoanerApp(account, appId);

                    if (loanerApp == null)
                    {
                        App app = apps.GetAppById(appId);

                        if (app != null)
                        {
                            loanerApp = new LoanerApp();

                            loanerApp.Account = id;
                            loanerApp.App = appId;

                            apps.AddLoanerApp(loanerApp);

                            instance.Context.SaveChanges();

                            instance.SetData(loanerApp, "LoanerApp");
                        }
                        else
                        {
                            instance.SetError("INVALID_APP");
                        }
                    }
                    else
                    {
                        instance.SetData(loanerApp, "LoanerApp");
                    }
                }
                else
                {
                    instance.SetError("INVALID_ACCOUNT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}/app/{appId}")]
        public HttpResponseMessage DeleteLoanerApp(long id, long appId)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagLoanerEdit))
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner != null)
                {
                    LoanerApp app = apps.GetLoanerApp(loaner, appId);

                    if (app != null)
                    {
                        instance.SetData(app, "LoanerApp");

                        apps.RemoveLoanerApp(app);
                    }
                    else
                    {
                        instance.SetError("INVALID_APP");
                    }
                }
                else
                {
                    instance.SetError("INVALID_ACCOUNT");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        // Checkout

        [HttpPost]
        [Route("{id}/checkout")]
        public HttpResponseMessage CheckoutLoaner(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (localAccount == null)
            {
                instance.SetError("ACCESS_DENIED");
            }
            else if (apps.GetUserCheckoutCount(localAccount) > 0)
            {
                instance.SetError("CHECKOUT_LIMIT_HIT");
            }
            else if (id > 0)
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner == null)
                {
                    instance.SetError("INVALID_ACCOUNT");
                }
                else if (loaner.CheckoutUser != 0)
                {
                    instance.SetError("ACCOUNT_IN_USE");
                }
                else
                {
                    // Change checkout value
                    loaner.CheckoutUser = localAccount.Id;
                    loaner.CheckoutChallenge++;

                    // Add message
                    NetMessageManager.AddMessageQuick(instance, new LoanerCheckoutMessage(loaner));

                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetError("INVALID_ACCOUNT");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}/checkin")]
        public HttpResponseMessage CheckinLoaner(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (localAccount == null)
            {
                instance.SetError("ACCESS_DENIED");
            }
            else if (id > 0)
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner == null)
                {
                    instance.SetError("INVALID_ACCOUNT");
                }
                else if (loaner.CheckoutUser != localAccount.Id && !instance.Accounts.CheckAccess(localAccount, AppManager.FlagLoanerCheckout))
                {
                    instance.SetError("ACCESS_DENIED");
                }
                else
                {
                    // Change checkout value
                    loaner.CheckoutUser = 0;

                    // Add message
                    NetMessageManager.AddMessageQuick(instance, new LoanerCheckoutMessage(loaner));

                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetError("INVALID_ACCOUNT");
            }

            return instance.ToResponse();
        }

        // Steamcode

        [HttpPost]
        [Route("{id}/steamcode")]
        public HttpResponseMessage SetSteamCode(long id, [FromBody] SetSteamCodeRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (localAccount != null && instance.Accounts.CheckAccess(localAccount, AppManager.FlagLoanerSteamCode))
            {
                LoanerAccount account = apps.GetLoanerAccount(id);

                if (account != null)
                {
                    if (account.CheckoutChallenge == request.Challenge && account.CheckoutUser != 0)
                    {
                        account.SteamCode = request.Code;

                        NetMessageManager.AddMessageSingleQuick(instance, account.CheckoutUser, new NewSteamCodeMessage(request.Code));
                    }
                    else
                    {
                        instance.Status = AppResponseStatus.ResponseError;
                        instance.StatusCode = "INVALID_CHALLENGE";
                    }
                }
                else
                {
                    instance.Status = AppResponseStatus.ResponseError;
                    instance.StatusCode = "INVALID_ACCOUNT";
                }
            }
            else
            {
                instance.Status = AppResponseStatus.AccessDenied;
                instance.StatusCode = AppManager.FlagLoanerSteamCode;
            }

            return instance.ToResponse();
        }

        // Search

        // GET		/search?{args}
    }
    
}
