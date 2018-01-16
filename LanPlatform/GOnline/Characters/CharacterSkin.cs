﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.GOnline.Characters
{
    public class CharacterSkin : EditableDatabaseObject
    {
        public String Name { get; set; }
        public PlayerItemAccess Access { get; set; }
    }
}