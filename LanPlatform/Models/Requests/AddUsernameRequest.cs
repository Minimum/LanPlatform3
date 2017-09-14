using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Models.Requests
{
    public class AddUsernameRequest
    {
        public String Username { get; set; }
        public String Password { get; set; }

        public AddUsernameRequest()
        {
            Username = "";
            Password = "";
        }
    }
}