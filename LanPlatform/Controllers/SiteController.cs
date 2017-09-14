using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Models;
using LanPlatform.Models.Responses;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/site")]
    public class SiteController : ApiController
    {
        [HttpGet]
        [Route("init")]
        public HttpResponseMessage InitializeClient()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            SiteInit data = new SiteInit(instance);
            data.LoadData();

            instance.Data = data;

            return instance.ToResponse();
        }
    }
}
