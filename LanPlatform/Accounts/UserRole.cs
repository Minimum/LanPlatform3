using System;
using LanPlatform.Database;

namespace LanPlatform.Accounts
{
    public class UserRole : EditableDatabaseObject
    {
        public String Name { get; set; }

        public UserRole()
        {
            Name = "";
        }
    }
}