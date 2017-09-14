using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Events;
using LanPlatform.Models;
using Newtonsoft.Json;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        // PUT		/

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetEvent(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            LanEventManager events = new LanEventManager(instance);

            instance.Data = events.GetEventById(id);

            if (instance.Data == null)
            {
                instance.Status = AppResponseStatus.ResponseError;
                instance.StatusCode = "INVALID_EVENT";
            }

            return instance.ToResponse();
        }

        // POST	/{id}

        // DELETE	/{id}

        // PUT		/{id}/guest

        // GET		/{id}/guest

        // GET		/{id}/guest/{guestId}

        // POST	    /{id}/guest/{guestId}

        // DELETE	/{id}/guest/{guestId}
    }
}
