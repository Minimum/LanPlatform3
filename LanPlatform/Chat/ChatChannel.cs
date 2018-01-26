using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Chat
{
    public class ChatChannel : EditableDatabaseObject
    {
        public String Title { get; set; }
        public String Greeting { get; set; }
        public bool Active { get; set; }

        public ChatChannel()
        {
            Title = "";
            Greeting = "";
            Active = true;
        }
    }
}