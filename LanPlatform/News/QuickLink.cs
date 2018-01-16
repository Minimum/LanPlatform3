using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.News
{
    public class QuickLink : EditableDatabaseObject
    {
        public String Title { get; set; }
        public String Link { get; set; }
        public QuickLinkType LinkType { get; set; }
        public bool Local { get; set; }

        public QuickLink()
        {
            Title = "";
            Link = "";
            LinkType = QuickLinkType.None;
            Local = false;
        }
    }

    public enum QuickLinkType
    {
        None = 0,
        CurrentPage,
        NewWindow
    }
}