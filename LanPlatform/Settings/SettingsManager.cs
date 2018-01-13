using System;
using System.Configuration;
using System.Linq;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Settings
{
    public class SettingsManager
    {
        public static int InstallStatus
        {
            get
            {
                int status = 0;

                Int32.TryParse(ConfigurationManager.AppSettings["LPInstallStatus"], out status);

                return status;
            }

            set => ConfigurationManager.AppSettings["LPInstallStatus"] = value.ToString(); 
        }

        public static bool LocalService
        {
            get
            {
                int status = 0;

                Int32.TryParse(ConfigurationManager.AppSettings["LPLocalService"], out status);

                return status > 0;
            }

            set
            {
                if (value)
                {
                    ConfigurationManager.AppSettings["LPLocalService"] = "1";
                }
                else
                {
                    ConfigurationManager.AppSettings["LPLocalService"] = "0";
                }
            }
        }

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