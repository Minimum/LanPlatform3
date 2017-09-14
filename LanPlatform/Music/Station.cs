using System;
using LanPlatform.Database;

namespace LanPlatform.Music
{
    public class Station : EditableDatabaseObject
    {
        public String Name { get; set; }

        // admins, etc

        public Station()
        {
            Name = "";
        }
    }
}