using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using LanPlatform.Accounts;
using LanPlatform.DTO.News;
using LanPlatform.Models;
using LanPlatform.Models.Requests;
using LanPlatform.Models.Responses;
using LanPlatform.News;
using LanPlatform.Settings;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/news")]
    public class NewsController : ApiController
    {
        // Statuses

        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateStatus([FromBody] EditNewsStatusRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            NewsManager newsManager = new NewsManager(instance);

            if (localAccount != null)
            {
                if (instance.Accounts.CheckAccess(localAccount, "NewsEditStatus"))
                {
                    NewsStatus status = new NewsStatus();

                    status.Title = request.Title;
                    status.Content = request.Content;

                    // Add and save status to DB
                    newsManager.AddStatus(status);

                    instance.Data = new NewsStatusDto(status);
                }
                else
                {
                    instance.Status = AppResponseStatus.ResponseError;
                    instance.StatusCode = "ACCESS_DENIED";
                }
            }
            else
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "ACCESS_DENIED";
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetStatusById(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);
            NewsStatus status = newsManager.GetStatusById(id);

            if (status != null)
            {
                instance.Data = new NewsStatusDto(status);
            }
            else
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "INVALID_STATUS";
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditStatusById(long id, [FromBody] EditNewsStatusRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            NewsManager newsManager = new NewsManager(instance);

            if (localAccount != null)
            {
                if (instance.Accounts.CheckAccess(localAccount, "NewsEditStatus"))
                {
                    NewsStatus status = newsManager.GetStatusById(id);

                    if (status != null)
                    {
                        status.Title = request.Title;
                        status.Content = request.Content;

                        PlatformSetting statusSetting = instance.Settings.GetSettingByName("NewsStatus");

                        instance.Data = new NewsStatusDto(status);
                    }
                    else
                    {
                        instance.Status = AppResponseStatus.ResponseError;
                        instance.StatusCode = "INVALID_STATUS";
                    }
                }
                else
                {
                    instance.Status = AppResponseStatus.ResponseError;
                    instance.StatusCode = "ACCESS_DENIED";
                }
            }
            else
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "ACCESS_DENIED";
            }

            return instance.ToResponse();
        }

        // DELETE	/{id}

        [HttpGet]
        [Route("current")]
        public HttpResponseMessage GetCurrentStatus()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);

            instance.Data = newsManager.GetCurrentStatus();

            if (instance.Data == null)
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "NEWS_NOT_FOUND";
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("current")]
        public HttpResponseMessage SetStatus([FromBody] ChangeNewsStatusRequest request)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserAccount localAccount = instance.LocalAccount;
            NewsManager newsManager = new NewsManager(instance);

            if (localAccount != null)
            {
                // TODO: 
                if (instance.Accounts.CheckAccess(localAccount, "NewsChangeStatus", "platform"))
                {
                    if (newsManager.GetStatusById(request.Id) != null)
                    {
                        instance.Settings.ChangeSetting("NewsStatus", request.Id.ToString());
                    }
                    else
                    {
                        instance.Status = AppResponseStatus.ResponseError;

                        instance.StatusCode = "ACCESS_STATUS";
                    }
                }
                else
                {
                    instance.Status = AppResponseStatus.ResponseError;

                    instance.StatusCode = "ACCESS_DENIED";
                }
            }
            else
            {
                instance.Status = AppResponseStatus.ResponseError;

                instance.StatusCode = "ACCESS_DENIED";
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("browse/status/{page}")]
        public HttpResponseMessage SearchStatus(int page)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);

            page = page < 1 ? 1 : page;

            List<NewsStatusDto> status = NewsStatusDto.ConvertList(newsManager.GetStatusList(page, 50));

            instance.Data = new BrowseResult<NewsStatusDto>(status, newsManager.GetStatusCount());

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("link")]
        public HttpResponseMessage GetActiveLinks()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);

            instance.SetData(QuickLinkDto.ConvertList(newsManager.GetActiveLinks()), "QuickLinkList");

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("link")]
        public HttpResponseMessage AddLink([FromBody] QuickLinkDto link)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(NewsManager.FlagEditLink))
            {
                NewsManager newsManager = new NewsManager(instance);
                QuickLink newLink = new QuickLink();

                newLink.Title = link.Title;
                newLink.Link = link.Link;
                newLink.LinkType = link.LinkType;

                newsManager.AddLink(newLink);

                try
                {
                    instance.Context.SaveChanges();

                    instance.SetData(new QuickLinkDto(newLink), "QuickLink");
                }
                catch (Exception e)
                {
                    instance.SetError("SAVE_ERROR");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("link/{id}")]
        public HttpResponseMessage GetLink(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);

            QuickLink link = newsManager.GetLink(id);

            if (link != null)
            {
                instance.SetData(new QuickLinkDto(link), "QuickLink");
            }
            else
            {
                instance.SetError("INVALID_LINK");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("link/{id}")]
        public HttpResponseMessage EditLink(long id, [FromBody] QuickLinkDto link)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(NewsManager.FlagEditLink))
            {
                NewsManager newsManager = new NewsManager(instance);

                QuickLink editLink = newsManager.GetLink(id);

                if (editLink != null)
                {
                    editLink.Title = link.Title;
                    editLink.Link = link.Link;
                    editLink.LinkType = link.LinkType;
                    editLink.Local = link.Local;

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(new QuickLinkDto(editLink), "QuickLink");
                    }
                    catch (Exception e)
                    {
                        instance.SetError("CONCURRENCY_ERROR");
                    }
                }
                else
                {
                    instance.SetError("INVALID_LINK");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("link/{id}")]
        public HttpResponseMessage DeleteLink(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(NewsManager.FlagEditLink))
            {
                NewsManager newsManager = new NewsManager(instance);

                QuickLink link = newsManager.GetLink(id);

                if (link != null)
                {
                    newsManager.RemoveLink(link);

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
                    instance.SetError("INVALID_LINK");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }
    }
}
