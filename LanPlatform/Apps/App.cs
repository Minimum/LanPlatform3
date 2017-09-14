using System;
using LanPlatform.Database;

namespace LanPlatform.Apps
{
    public class App : EditableDatabaseObject
    {
        public AppType Type { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public AppDownloadType DownloadType { get; set; }
        public String DownloadInfo { get; set; }

        public long LoanerCount { get; set; }

        public App()
        {
            Type = AppType.None;
            Title = "";
            Description = "";
            DownloadType = AppDownloadType.None;
            DownloadInfo = "";

            LoanerCount = 0;
        }
    }

    public enum AppType
    {
        None = 0,
        App,
        Game,
        Mod
    }

    public enum AppDownloadType
    {
        None = 0,
        Url,
        Steam,
        Content
    }
}