using LanPlatform.Database;

namespace LanPlatform.Awards
{
    public class PlayerAward : EditableDatabaseObject
    {
        public long Account { get; set; }
        public long Award { get; set; }
        public long TimeAwarded { get; set; }
        public long ExperienceEntry { get; set; }

        public PlayerAward()
        {
            Account = 0;
            Award = 0;
            TimeAwarded = 0;
            ExperienceEntry = 0;
        }
    }
}