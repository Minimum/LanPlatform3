using System;
using System.Linq;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Settings
{
    public class SettingsManager
    {
        protected PlatformContext Context;

        protected AppInstance Instance;

        public SettingsManager(AppInstance instance)
        {
            Context = instance.Context;

            Instance = instance;
        }

        public bool SettingNameExists(String name)
        {
            PlatformSetting setting = GetSettingByName(name);

            return setting != null;
        }

        public PlatformSetting GetSettingById(long id)
        {
            return Context.Setting.SingleOrDefault(s => s.Id == id);
        }

        public PlatformSetting GetSettingByName(String name)
        {
            return Context.Setting.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public void AddSetting(PlatformSetting setting)
        {
            if (GetSettingByName(setting.Name) == null)
            {
                Context.Setting.Add(setting);
            }

            return;
        }

        public void RemoveSetting(PlatformSetting setting)
        {
            Context.Setting.Remove(setting);

            return;
        }

        public bool ChangeSetting(String name, String value)
        {
            PlatformSetting setting = GetSettingByName(name);
            bool success = setting != null;

            if (success)
            {
                setting.Value = value;
            }

            return success;
        }
    }
}