using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Models;

namespace LanPlatform.Auth
{
    public class AuthSessionAttempt : AuthAttempt
    {
        public long SessionId { get; set; }
        public String Key { get; set; }
        public String Address { get; set; }

        public AuthSessionAttempt()
        {
            SessionId = 0;
            Key = "";
            Address = "";
        }

        public AuthSessionAttempt(AppInstance instance, long id, String key)
            : base(false, instance)
        {
            SessionId = id;
            Key = key;
            Address = instance.RequestContext.Request.UserHostAddress;
        }
    }
}