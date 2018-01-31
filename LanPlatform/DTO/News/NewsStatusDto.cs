using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.News;

namespace LanPlatform.DTO.News
{
    public class NewsStatusDto : EditableGabionDto
    {
        public String Title { get; set; }
        public long TitleImage { get; set; }
        public NewsStatusType ContentType { get; set; }
        public String Content { get; set; }

        public NewsStatusDto()
        {
            Title = "";
            TitleImage = 0;
            ContentType = NewsStatusType.Standard;
            Content = "";
        }

        public NewsStatusDto(NewsStatus status)
            : base(status)
        {
            Title = status.Title;
            TitleImage = status.TitleImage;
            ContentType = status.ContentType;
            Content = status.Content;
        }

        public override string GetClassname()
        {
            return "NewsStatus";
        }

        public static List<GabionDto> ConvertList(ICollection<NewsStatus> accounts)
        {
            var models = new List<GabionDto>();

            foreach (NewsStatus account in accounts)
            {
                models.Add(new NewsStatusDto(account));
            }

            return models;
        }
    }
}