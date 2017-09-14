using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Models.Requests
{
    public class InstallAccountRequest
    {
        public String DisplayName { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public InstallAccountRequest()
        {
            DisplayName = "";
            Username = "";
            Password = "";
        }
    }
}