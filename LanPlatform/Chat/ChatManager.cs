using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Chat
{
    public static class ChatManager
    {
        public const String FlagAddChannel = "ChatAddChannel";
        public const String FlagDeleteChannel = "ChatDeleteChannel";
        public const String FlagEditChannel = "ChatEditChannel";

        public const String FlagAddMessage = "ChatAddMessage";
        public const String FlagEditMessage = "ChatEditMessage";
        public const String FlagDeleteMessage = "ChatDeleteMessage";

        public const String FlagAddAccess = "ChatAddAccess";
        public const String FlagEditAccess = "ChatEditAccess";
        public const String FlagDeleteAccess = "ChatDeleteAccess";
    }
}