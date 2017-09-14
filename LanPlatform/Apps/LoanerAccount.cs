using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LanPlatform.Database;
using Newtonsoft.Json;

namespace LanPlatform.Apps
{
    public class LoanerAccount : EditableDatabaseObject
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public String SteamCode { get; set; }
        public long CheckoutUser { get; set; }

        [JsonIgnore]
        public long CheckoutChallenge { get; set; }

        [NotMapped]
        public List<App> Apps { get; set; } 

        public LoanerAccount()
        {
            Username = "";
            Password = "";
            SteamCode = "";
            CheckoutUser = 0;
            CheckoutChallenge = 1;

            Apps = new List<App>();
        }
    }
}