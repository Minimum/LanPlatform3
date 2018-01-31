using System;
using System.Collections.Generic;
using LanPlatform.Events;

namespace LanPlatform.DTO.Events
{
    public class LanEventDto : EditableGabionDto
    {
        public String Name { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        public LanEventDto()
        {
            Name = "";
            StartTime = 0;
            EndTime = 0;
        }

        public LanEventDto(LanEvent lanEvent)
            : base(lanEvent)
        {
            Name = lanEvent.Name;
            StartTime = lanEvent.StartTime;
            EndTime = lanEvent.EndTime;
        }

        public override string GetClassname()
        {
            return "LanEvent";
        }

        public static List<GabionDto> ConvertList(ICollection<LanEvent> objects)
        {
            var models = new List<GabionDto>();

            foreach (LanEvent target in objects)
            {
                models.Add(new LanEventDto(target));
            }

            return models;
        }
    }
}