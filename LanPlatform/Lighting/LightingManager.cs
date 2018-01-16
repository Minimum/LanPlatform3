using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.DAL;
using LanPlatform.Models;
using LanPlatform.Platform;

namespace LanPlatform.Lighting
{
    public class LightingManager : IPlatformManager
    {
        protected AppInstance Instance;
        protected PlatformContext Context;

        public LightingManager(AppInstance instance)
        {
            Instance = instance;
            Context = instance.Context;
        }

        public bool Install()
        {
            return true;
        }
    }
}