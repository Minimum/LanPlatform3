using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Chat
{
    public class ChatAccess : EditableDatabaseObject
    {
        public long Channel { get; set; }
        public long Role { get; set; }

        // Generic Permissions
        public bool CanWrite { get; set; }
        public bool CanTextToSpeech { get; set; }
        public bool CanUpload { get; set; }

        // Admin Permissions
        public bool CanMute { get; set; }
        public bool CanSetGreeting { get; set; }

        public ChatAccess()
        {
            Channel = 0;
            Role = 0;

            CanWrite = true;
            CanTextToSpeech = false;
            CanUpload = false;

            CanMute = false;
            CanSetGreeting = false;
        }
    }
}