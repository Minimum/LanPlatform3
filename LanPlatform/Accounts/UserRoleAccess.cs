using System;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class UserRoleAccess : EditableDatabaseObject
    {
        public long Role { get; set; }
        public long User { get; set; }

        public UserRoleAccess()
        {
            Role = 0;
            User = 0;
        }
    }
}