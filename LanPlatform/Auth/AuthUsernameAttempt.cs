using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Models;

namespace LanPlatform.Auth
{
    public class AuthUsernameAttempt : AuthAttempt
    {
        public String Username { get; set; }

        public AuthUsernameAttempt()
        {
            
        }

        public AuthUsernameAttempt(bool success, AppInstance instance, String username)
            : base(success, instance)
        {
            Username = username;
        }
    }
}