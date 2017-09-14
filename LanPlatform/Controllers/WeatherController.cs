using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.Models;
using LanPlatform.News;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/weather")]
    public class WeatherController : ApiController
    {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetCurrentWeatherReport()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);

            instance.Data = newsManager.GetCurrentWeatherStatus();

            if (instance.Data == null)
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "STATUS_NOT_FOUND";
            }

            return instance.ToResponse();
        }

        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateWeatherReport([FromBody] WeatherStatus status)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NewsManager newsManager = new NewsManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            if (status != null)
            {
                if (localAccount != null && instance.Accounts.CheckAccess(localAccount, NewsManager.FlagChangeWeather))
                {
                    newsManager.AddWeatherStatus(status);
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
                instance.StatusCode = "INVALID_REQUEST";
            }

            return instance.ToResponse();
        }
    }
}
