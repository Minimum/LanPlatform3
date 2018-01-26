using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.Chat
{
    public class ChatMessage : EditableDatabaseObject
    {
        public long Author { get; set; }
        public String AuthorName { get; set; }
        public long Channel { get; set; }
        public long Time { get; set; }
        public bool Hidden { get; set; }

        public long Editor { get; set; }
        public String EditorName { get; set; }
        public long EditTime { get; set; }

        public String Message { get; set; }

        public ChatMessage()
        {
            Author = 0;
            Channel = 0;
            Time = 0;
            Hidden = false;

            Editor = 0;
            EditorName = "";
            EditTime = 0;

            Message = "";
        }
    }
}