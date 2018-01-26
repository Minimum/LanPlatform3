using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Chat;

namespace LanPlatform.DTO.Chat
{
    public class ChatChannelDto : EditableGabionDto
    {
        public String Title { get; set; }
        public String Greeting { get; set; }
        public bool Active { get; set; }

        public ChatChannelDto()
        {
            Title = "";
            Greeting = "";
            Active = false;
        }

        public ChatChannelDto(ChatChannel channel)
            : base(channel)
        {
            Title = channel.Title;
            Greeting = channel.Greeting;
            Active = channel.Active;
        }

        public static List<ChatChannelDto> ConvertList(ICollection<ChatChannel> objects)
        {
            var models = new List<ChatChannelDto>();

            foreach (ChatChannel target in objects)
            {
                models.Add(new ChatChannelDto(target));
            }

            return models;
        }
    }
}