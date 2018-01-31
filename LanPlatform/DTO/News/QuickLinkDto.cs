using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.News;

namespace LanPlatform.DTO.News
{
    public class QuickLinkDto : EditableGabionDto
    {
        public String Title { get; set; }
        public String Link { get; set; }
        public QuickLinkType LinkType { get; set; }
        public bool Local { get; set; }

        public QuickLinkDto()
        {
            Title = "";
            Link = "";
            LinkType = QuickLinkType.None;
            Local = false;
        }

        public QuickLinkDto(QuickLink link)
            : base(link)
        {
            Title = link.Title;
            Link = link.Link;
            LinkType = link.LinkType;
            Local = link.Local;
        }

        public override string GetClassname()
        {
            return "QuickLink";
        }

        public static List<GabionDto> ConvertList(ICollection<QuickLink> objects)
        {
            var models = new List<GabionDto>();

            foreach (QuickLink target in objects)
            {
                models.Add(new QuickLinkDto(target));
            }

            return models;
        }
    }
}