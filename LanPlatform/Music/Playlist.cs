using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LanPlatform.Database;

namespace LanPlatform.Music
{
    public class Playlist : EditableDatabaseObject
    {
        public String Name { get; set; }
        public long Owner { get; set; }

        [NotMapped]
        public List<PlaylistEntry> Entries { get; set; }

        public Playlist()
        {
            Name = "";
            Owner = 0;

            Entries = new List<PlaylistEntry>();
        }
    }
}