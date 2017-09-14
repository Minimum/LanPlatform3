using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.Models;
using LanPlatform.Network;
using Newtonsoft.Json;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/message")]
    public class MessageController : ApiController
    {
        [HttpGet]
        [Route("update/{messageStart}")]
        public HttpResponseMessage UpdateClient(long messageStart)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NetMessageManager messageManager = new NetMessageManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            long target = (localAccount == null) ? 0 : localAccount.Id;

            if (messageStart < 0)
                messageStart = 0;

            instance.Data = messageManager.GetMessageOutputs(target, messageStart, 50);

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("init")]
        public HttpResponseMessage Initialize()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            NetMessageManager messageManager = new NetMessageManager(instance);
            UserAccount localAccount = instance.LocalAccount;

            long target = (localAccount == null) ? 0 : localAccount.Id;

            instance.Data = messageManager.GetLastMessageId(target) + 1;

            return instance.ToResponse();
        }
    }
}
