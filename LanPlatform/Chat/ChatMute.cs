using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Chat
{
    public class ChatMute : EditableDatabaseObject
    {
        public long Channel { get; set; }
        public long User { get; set; }
        public long Expire { get; set; }

        public long Admin { get; set; }

        public ChatMute()
        {
            Channel = 0;
            User = 0;
            Expire = 0;

            Admin = 0;
        }
    }
}