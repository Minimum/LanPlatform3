using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using LanPlatform.Accounts;
using LanPlatform.DAL;
using LanPlatform.Models;
using LanPlatform.Models.Requests;

namespace LanPlatform.Apps
{
    public class AppManager
    {
        public const String FlagAppEdit = "AppEdit";
        public const String FlagLoanerCreate = "AppLoanerCreate";
        public const String FlagLoanerEdit = "AppLoanerEdit";
        public const String FlagLoanerDelete = "AppLoanerDelete";
        public const String FlagLoanerCheckout = "AppLoanerCheckout";
        public const String FlagLoanerSteamCode = "AppLoanerSteamCode";

        protected PlatformContext Context;

        protected AppInstance Instance;

        public AppManager(AppInstance instance)
        {
            Context = instance.Context;

            Instance = instance;
        }

        public void Install()
        {
            
        }

        // Apps 

        public App GetAppById(long id)
        {
            return Context.App.FirstOrDefault(s => s.Id == id);
        }

        public List<App> GetApps()
        {
            return Context.App.Where(s => s.Id > 0).ToList();
        }

        public List<App> GetAppSearch(SearchAppRequest request)
        {
            request.SanityCheck();

            long startPos = (request.Page - 1) * request.PageSize;

            IQueryable<App> query = Context.App.Where(s => s.Id > 0);

            // Sort
            switch (request.SortBy)
            {
                case SearchAppSort.AppType:
                    {
                        query = request.SortDescending
                            ? query.OrderByDescending(s => s.Type).ThenBy(s => s.Id)
                            : query.OrderBy(s => s.Type).ThenBy(s => s.Id);

                        break;
                    }

                case SearchAppSort.Description:
                    {
                        query = request.SortDescending
                            ? query.OrderByDescending(s => s.Description).ThenBy(s => s.Id)
                            : query.OrderBy(s => s.Description).ThenBy(s => s.Id);

                        break;
                    }

                case SearchAppSort.Title:
                    {
                        query = request.SortDescending
                            ? query.OrderByDescending(s => s.Title).ThenBy(s => s.Id)
                            : query.OrderBy(s => s.Title).ThenBy(s => s.Id);

                        break;
                    }

                case SearchAppSort.DownloadType:
                    {
                        query = request.SortDescending
                            ? query.OrderByDescending(s => s.DownloadType).ThenBy(s => s.Id)
                            : query.OrderBy(s => s.DownloadType).ThenBy(s => s.Id);

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

            return query.Skip((int)startPos).Take(request.PageSize).AsNoTracking().ToList();
        }

        public long GetAppTotal()
        {
            return Context.App.LongCount();
        }

        public void AddApp(App app)
        {
            Context.App.Add(app);

            return;
        }

        public void RemoveApp(App app)
        {
            Context.App.Remove(app);

            return;
        }

        // Loaner Accounts

        public List<LoanerAccount> GetLoanerAccounts()
        {
            List<LoanerAccount> accounts = Context.LoanerAccount.Where(s => s.Id > 0).ToList();

            // Load apps
            foreach (LoanerAccount account in accounts)
            {
                account.Apps =
                    Context.App.Where(
                        app =>
                            Context.LoanerApp.Where(loanerApp => loanerApp.Account == account.Id)
                                .Select(loanerApp => loanerApp.App)
                                .Contains(app.Id)).ToList();
            }

            return accounts;
        }

        public LoanerAccount GetLoanerAccount(long id)
        {
            LoanerAccount account = Context.LoanerAccount.FirstOrDefault(s => s.Id == id);

            // Load apps
            if (account != null)
            {
                account.Apps =
                    Context.App.Where(
                        app =>
                            Context.LoanerApp.Where(loanerApp => loanerApp.Account == account.Id)
                                .Select(loanerApp => loanerApp.App)
                                .Contains(app.Id)).ToList();
            }

            return account;
        }

        public LoanerAccount GetLoanerAccountByName(String username)
        {
            LoanerAccount account =
                Context.LoanerAccount.FirstOrDefault(
                    s => s.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            // Load apps
            if (account != null)
            {
                account.Apps =
                    Context.App.Where(
                        app =>
                            Context.LoanerApp.Where(loanerApp => loanerApp.Account == account.Id)
                                .Select(loanerApp => loanerApp.App)
                                .Contains(app.Id)).ToList();
            }

            return account;
        }

        public int GetUserCheckoutCount(UserAccount user)
        {
            return Context.LoanerAccount.Count(s => s.CheckoutUser == user.Id);
        }

        public void AddLoanerAccount(LoanerAccount account)
        {
            Context.LoanerAccount.Add(account);

            return;
        }

        public void RemoveLoanerAccount(LoanerAccount account)
        {
            Context.LoanerAccount.Remove(account);

            return;
        }

        // Loaner Apps

        public List<LoanerApp> GetLoanerApps(LoanerAccount account)
        {
            return Context.LoanerApp.Where(s => s.Account == account.Id).ToList();
        }

        public LoanerApp GetLoanerApp(LoanerAccount account, long appId)
        {
            return GetLoanerApp(account.Id, appId);
        }

        public LoanerApp GetLoanerApp(long accountId, long appId)
        {
            return Context.LoanerApp.FirstOrDefault(s => s.Account == accountId && s.App == appId);
        }

        public void AddLoanerApp(LoanerApp app)
        {
            Context.LoanerApp.Add(app);

            return;
        }

        public void RemoveLoanerApp(LoanerApp app)
        {
            Context.LoanerApp.Remove(app);

            return;
        }
        
    }
}