using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Lighting
{
    public class LightingManager
    {
        protected AppInstance Instance;
        protected PlatformContext Context;

        public LightingManager(AppInstance instance)
        {
            Instance = instance;
            Context = instance.Context;
        }


    }
}