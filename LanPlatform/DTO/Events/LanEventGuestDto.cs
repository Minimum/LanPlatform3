using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Events;

namespace LanPlatform.DTO.Events
{
    public class LanEventGuestDto : EditableGabionDto
    {
        public long Account { get; set; }
        public long Event { get; set; }
        public long Invited { get; set; }
        public long Arrived { get; set; }
        public long Departed { get; set; }

        public LanEventGuestDto()
        {
            Account = 0;
            Event = 0;
            Invited = 0;
            Arrived = 0;
            Departed = 0;
        }

        public LanEventGuestDto(LanEventGuest guest)
            : base(guest)
        {
            Account = guest.Account;
            Event = guest.Event;
            Invited = guest.Invited;
            Arrived = guest.Arrived;
            Departed = guest.Departed;
        }

        public override string GetClassname()
        {
            return "LanEventGuest";
        }

        public static List<GabionDto> ConvertList(ICollection<LanEventGuest> objects)
        {
            var models = new List<GabionDto>();

            foreach (LanEventGuest target in objects)
            {
                models.Add(new LanEventGuestDto(target));
            }

            return models;
        }
    }
}