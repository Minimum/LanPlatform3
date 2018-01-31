using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Apps;

namespace LanPlatform.DTO.Apps
{
    public class AppDto : EditableGabionDto
    {
        public AppType Type { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public AppDownloadType DownloadType { get; set; }
        public String DownloadInfo { get; set; }

        public AppDto()
        {
            Type = AppType.None;
            Title = "";
            Description = "";
            DownloadType = AppDownloadType.None;
            DownloadInfo = "";
        }

        public AppDto(App app)
            : base(app)
        {
            Type = app.Type;
            Title = app.Title;
            Description = app.Description;
            DownloadType = app.DownloadType;
            DownloadInfo = app.DownloadInfo;
        }

        public override string GetClassname()
        {
            return "App";
        }

        public static List<GabionDto> ConvertList(ICollection<App> objects)
        {
            var models = new List<GabionDto>();

            foreach (App target in objects)
            {
                models.Add(new AppDto(target));
            }

            return models;
        }
    }
}