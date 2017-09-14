using LanPlatform.Database;

namespace LanPlatform.Events
{
    public class LanEventGuest : EditableDatabaseObject
    {
        public long Account { get; set; }
        public long Event { get; set; }
        public long Invited { get; set; }
        public long Arrived { get; set; }
        public long Departed { get; set; }

        public LanEventGuest()
        {
            Account = 0;
            Event = 0;
            Invited = 0;
            Arrived = 0;
            Departed = 0;
        }
    }
}