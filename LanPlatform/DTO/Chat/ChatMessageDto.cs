using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Chat;

namespace LanPlatform.DTO.Chat
{
    public class ChatMessageDto : EditableGabionDto
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

        public ChatMessageDto()
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

        public ChatMessageDto(ChatMessage message)
            : base(message)
        {
            Author = message.Author;
            Channel = message.Channel;
            Time = message.Time;
            Hidden = message.Hidden;

            Editor = message.Editor;
            EditorName = message.EditorName;
            EditTime = message.EditTime;

            Message = message.Message;
        }

        public static List<ChatMessageDto> ConvertList(ICollection<ChatMessage> objects)
        {
            var models = new List<ChatMessageDto>();

            foreach (ChatMessage target in objects)
            {
                models.Add(new ChatMessageDto(target));
            }

            return models;
        }
    }
}