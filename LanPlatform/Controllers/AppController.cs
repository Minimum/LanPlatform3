using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.DTO.Accounts;
using LanPlatform.Network;
using LanPlatform.Network.Messages;
using LanPlatform.Apps;
using LanPlatform.DTO.Apps;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Models.Responses;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateApp(long id, [FromBody] AppDto app)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagAppEdit))
            {
                App newApp = new App();

                newApp.Type = app.Type;
                newApp.Title = app.Title;
                newApp.Description = app.Description;
                newApp.DownloadType = app.DownloadType;
                newApp.DownloadInfo = app.DownloadInfo;

                apps.AddApp(newApp);

                instance.Context.SaveChanges();

                instance.SetData(new AppDto(newApp), "App");
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetApp(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            App app = apps.GetAppById(id);

            if (app != null)
            {
                instance.SetData(new AppDto(app), "App");
            }
            else
            {
                instance.SetError("INVALID_APP");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditApp(long id, [FromBody] AppDto edit)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagAppEdit))
            {
                App app = apps.GetAppById(id);

                if (app != null)
                {
                    // TODO: Add edit logging

                    app.Type = edit.Type;
                    app.Title = edit.Title;
                    app.Description = edit.Description;
                    app.DownloadType = edit.DownloadType;
                    app.DownloadInfo = edit.DownloadInfo;

                    instance.SetData(new AppDto(app), "App");
                }
                else
                {
                    instance.SetError("INVALID_APP");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}")]
        public HttpResponseMessage DeleteApp(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager apps = new AppManager(instance);

            if (instance.Accounts.CheckAccess(AppManager.FlagAppEdit))
            {
                App app = apps.GetAppById(id);

                if (app != null)
                {
                    instance.SetData(new AppDto(app), "app");

                    apps.RemoveApp(app);
                }
                else
                {
                    instance.SetError("INVALID_APP");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("search")]
        public HttpResponseMessage SearchApps([FromUri] SearchAppRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            AppManager appManager = new AppManager(instance);

            List<App> dataApps = appManager.GetAppSearch(request);

            if (dataApps != null)
            {
                BrowseResult<AppDto> apps = new BrowseResult<AppDto>();

                apps.TotalResults = appManager.GetAppTotal();

                foreach (App app in dataApps)
                {
                    apps.Results.Add(new AppDto(app));
                }

                instance.SetData(apps, "AppList");
            }
            else
            {
                instance.SetError(AppResponseStatus.AppError, "DAO_ERROR");
            }

            return instance.ToResponse();
        }
    }
}
