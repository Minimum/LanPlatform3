using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Characters
{
    public class Character : EditableDatabaseObject
    {
        public String Name { get; set; }
        public PlayerItemAccess Access { get; set; }


    }
}