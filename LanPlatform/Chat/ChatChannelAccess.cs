using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Chat
{
    public class ChatChannelAccess : EditableDatabaseObject
    {
        public long Channel { get; set; }
        public long Role { get; set; }

        // Generic Permissions
        public bool CanWrite { get; set; }

        // Admin Permissions
        public bool CanMute { get; set; }
        public bool CanSetGreeting { get; set; }

        public ChatChannelAccess()
        {
            Channel = 0;
            Role = 0;

            CanWrite = true;

            CanMute = false;
            CanSetGreeting = false;
        }
    }
}