using LanPlatform.Database;

namespace LanPlatform.Music
{
    public class PlaylistEntry : EditableDatabaseObject
    {
        public long Playlist { get; set; }
        public long Song { get; set; }
        
        public long Position { get; set; }

        public PlaylistEntry()
        {
            Playlist = 0;
            Song = 0;

            Position = 0;
        }
    }
}