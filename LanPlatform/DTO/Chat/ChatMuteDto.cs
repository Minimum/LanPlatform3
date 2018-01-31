using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Chat;

namespace LanPlatform.DTO.Chat
{
    public class ChatMuteDto : EditableGabionDto
    {
        public long Channel { get; set; }
        public long User { get; set; }
        public long Expire { get; set; }

        public long Admin { get; set; }

        public ChatMuteDto()
        {
            Channel = 0;
            User = 0;
            Expire = 0;

            Admin = 0;
        }

        public ChatMuteDto(ChatMute mute)
            : base(mute)
        {
            Channel = mute.Channel;
            User = mute.User;
            Expire = mute.Expire;

            Admin = mute.Admin;
        }

        public override string GetClassname()
        {
            return "ChatMute";
        }

        public static List<GabionDto> ConvertList(ICollection<ChatMute> objects)
        {
            var models = new List<GabionDto>();

            foreach (ChatMute target in objects)
            {
                models.Add(new ChatMuteDto(target));
            }

            return models;
        }
    }
}