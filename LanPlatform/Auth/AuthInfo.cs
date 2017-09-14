using LanPlatform.Database;

namespace LanPlatform.Auth
{
    public abstract class AuthInfo : EditableDatabaseObject
    {
        public long Account { get; set; }
        public bool Active { get; set; }

        public AuthInfo()
        {
            Account = 0;
            Active = true;
        }
    }
}