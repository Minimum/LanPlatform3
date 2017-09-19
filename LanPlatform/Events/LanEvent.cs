using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LanPlatform.Database;

namespace LanPlatform.Events
{
    public class LanEvent : EditableDatabaseObject
    {
        public String Name { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }

        [NotMapped]
        public List<LanEventGuest> GuestRecords { get; set; }

        public LanEvent()
        {
            Name = "";
            StartTime = 0;
            EndTime = 0;

            GuestRecords = new List<LanEventGuest>();
        }
    }
}