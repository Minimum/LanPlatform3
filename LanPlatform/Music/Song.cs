using System;
using LanPlatform.Database;

namespace LanPlatform.Music
{
    public class Song : EditableDatabaseObject
    {
        public String Title { get; set; }
        public String Artist { get; set; }
        public String Album { get; set; }
        public int TrackNumber { get; set; }
        public long Duration { get; set; }
        public int Volume { get; set; }

        public long HighQualityContent { get; set; }    // FLAC, ALAC
        public long LowQualityContent { get; set; }     // MP3, MP4, OGG

        public Song()
        {
            Title = "";
            Artist = "";
            Album = "";
            TrackNumber = 0;
            Duration = 0;
            Volume = 100;

            HighQualityContent = 0;
            LowQualityContent = 0;
        }
    }
}