using System;
using LanPlatform.Database;

namespace LanPlatform.Awards
{
    public class Award : EditableDatabaseObject
    {
        public String Title { get; set; }
        public String Description { get; set; }
        public long Group { get; set; }
        public long Image { get; set; }
        public long Experience { get; set; }

        public Award()
        {
            Title = "";
            Description = "";
            Group = 0;
            Image = 0;
            Experience = 0;
        }
    }
}