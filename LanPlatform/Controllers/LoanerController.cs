using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/loaner")]
    public class LoanerController : ApiController
    {
        // TODO: Remove this, replace with browse
        [HttpGet]
        [Route("all")]
        public HttpResponseMessage GetAllLoaners()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            instance.SetData(LoanerAccountDto.ConvertList(apps.GetLoanerAccounts()));

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

                    instance.SetData(new LoanerAccountDto(loaner));
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetAccessDenied(AppManager.FlagLoanerEdit);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetLoaner(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            instance.SetData(new LoanerAccountDto(apps.GetLoanerAccount(id)));

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

                        try
                        {
                            instance.Context.SaveChanges();

                            instance.SetData(new LoanerAccountDto(dataLoaner));
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
                        instance.SetError("InvalidAccount");
                    }
                }
                else
                {
                    instance.SetAccessDenied(AppManager.FlagLoanerEdit);
                }
            }
            else
            {
                instance.SetError("InvalidRequestObject");
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
                instance.SetAccessDenied(AppManager.FlagLoanerDelete);
            }

            return instance.ToResponse();
        }

        // Loaner apps

        [HttpGet]
        [Route("{id}/app")]
        public HttpResponseMessage GetLoanerApps(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);



            return instance.ToResponse();
        }

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

                            try
                            {
                                instance.Context.SaveChanges();

                                instance.SetData(new LoanerAppDto(loanerApp), "LoanerApp");
                            }
                            catch (Exception)
                            {
                                instance.SetError("SaveError");
                            }
                        }
                        else
                        {
                            instance.SetError("InvalidApp");
                        }
                    }
                    else
                    {
                        instance.SetData(loanerApp, "LoanerApp");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetAccessDenied(AppManager.FlagLoanerEdit);
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
                        instance.SetData(new LoanerAppDto(app), "LoanerApp");

                        apps.RemoveLoanerApp(app);
                    }
                    else
                    {
                        instance.SetError("InvalidApp");
                    }
                }
                else
                {
                    instance.SetError("InvalidAccount");
                }
            }
            else
            {
                instance.SetAccessDenied(AppManager.FlagLoanerEdit);
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

            if (instance.Anonymous)
            {
                instance.SetAccessDenied("AnonymousUser");
            }
            else if (apps.GetUserCheckoutCount(localAccount) > 0)
            {
                instance.SetError("CheckoutLimitHit");
            }
            else if (id > 0)
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner == null)
                {
                    instance.SetError("InvalidAccount");
                }
                else if (loaner.CheckoutUser != 0)
                {
                    instance.SetError("AccountInUse");
                }
                else
                {
                    // Change checkout value
                    loaner.CheckoutUser = localAccount.Id;
                    loaner.CheckoutChallenge++;

                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetError("InvalidAccount");
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

            if (instance.Anonymous)
            {
                instance.SetAccessDenied("AnonymousUser");
            }
            else if (id > 0)
            {
                LoanerAccount loaner = apps.GetLoanerAccount(id);

                if (loaner == null)
                {
                    instance.SetError("InvalidLoaner");
                }
                else if (loaner.CheckoutUser != localAccount.Id && !instance.Accounts.CheckAccess(localAccount, AppManager.FlagLoanerCheckout))
                {
                    instance.SetAccessDenied(AppManager.FlagLoanerCheckout);
                }
                else
                {
                    // Change checkout value
                    loaner.CheckoutUser = 0;

                    instance.SetData(true, "bool");
                }
            }
            else
            {
                instance.SetError("InvalidLoaner");
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
                    }
                    else
                    {
                        instance.SetError("InvalidChallenge");
                    }
                }
                else
                {
                    instance.SetError("InvalidLoaner");
                }
            }
            else
            {
                instance.SetAccessDenied(AppManager.FlagLoanerSteamCode);
            }

            return instance.ToResponse();
        }

        // Search

        // GET		/search?{args}
    }
    
}
