using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using LanPlatform.Database;
using LanPlatform.Models;

namespace LanPlatform.Auth
{
    public abstract class AuthAttempt : DatabaseObject
    {
        public String IPAddress { get; set; }
        public long Time { get; set; }
        public bool Success { get; set; }

        public AuthAttempt()
        {
            IPAddress = "";
            Time = 0;
            Success = false;
        }

        public AuthAttempt(bool success, AppInstance instance)
        {
            if (!String.IsNullOrEmpty(instance.RequestContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]))
            {
                IPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else if (!String.IsNullOrEmpty(instance.RequestContext.Request.ServerVariables["REMOTE_ADDR"]))
            {
                IPAddress = instance.RequestContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            else if (!String.IsNullOrEmpty(instance.RequestContext.Request.UserHostAddress))
            {
                IPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            Time = instance.Time;

            Success = success;
        }
    }
}