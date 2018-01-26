using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Chat;

namespace LanPlatform.DTO.Chat
{
    public class ChatAccessDto : EditableGabionDto
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

        public ChatAccessDto()
        {
            Channel = 0;
            Role = 0;

            CanWrite = true;
            CanTextToSpeech = false;
            CanUpload = false;

            CanMute = false;
            CanSetGreeting = false;
        }

        public ChatAccessDto(ChatAccess access)
            : base(access)
        {
            Channel = access.Channel;
            Role = access.Role;

            CanWrite = access.CanWrite;
            CanTextToSpeech = access.CanTextToSpeech;
            CanUpload = access.CanUpload;

            CanMute = access.CanMute;
            CanSetGreeting = access.CanSetGreeting;
        }

        public static List<ChatAccessDto> ConvertList(ICollection<ChatAccess> objects)
        {
            var models = new List<ChatAccessDto>();

            foreach (ChatAccess target in objects)
            {
                models.Add(new ChatAccessDto(target));
            }

            return models;
        }
    }
}