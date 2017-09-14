using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Models;
using LanPlatform.Models.Requests;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/lighting")]
    public class LightingController : ApiController
    {
        [HttpPost]
        [Route("discover")]
        public HttpResponseMessage Discover()
        {
            AppInstance appResponse = new AppInstance(Request, HttpContext.Current);



            return appResponse.ToResponse();
        }

        [HttpGet]
        [Route("light/{id}")]
        public HttpResponseMessage GetLight(long id)
        {
            AppInstance appResponse = new AppInstance(Request, HttpContext.Current);



            return appResponse.ToResponse();
        }

        [HttpGet]
        [Route("group/{id}")]
        public HttpResponseMessage GetGroup(long id)
        {
            AppInstance appResponse = new AppInstance(Request, HttpContext.Current);



            return appResponse.ToResponse();
        }

        [HttpPost]
        [Route("light/{id}/brightness")]
        public HttpResponseMessage SetLightBrightness(long id, [FromBody] SetLightBrightnessRequest request)
        {
            AppInstance appResponse = new AppInstance(Request, HttpContext.Current);



            return appResponse.ToResponse();
        }

        [HttpPost]
        [Route("group/{id}/brightness")]
        public HttpResponseMessage SetGroupBrightness(long id, [FromBody] SetLightBrightnessRequest request)
        {
            AppInstance appResponse = new AppInstance(Request, HttpContext.Current);



            return appResponse.ToResponse();
        }
    }
}
