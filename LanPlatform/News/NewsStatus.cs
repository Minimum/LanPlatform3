using System;
using LanPlatform.Database;

namespace LanPlatform.News
{
    public class NewsStatus : EditableDatabaseObject
    {
        public String Title { get; set; }
        public long TitleImage { get; set; }
        public NewsStatusType ContentType { get; set; }
        public String Content { get; set; }

        public NewsStatus()
        {
            Id = 0;
            Title = "";
            TitleImage = 0;
            ContentType = NewsStatusType.Standard;
            Content = "";
        }
    }

    public enum NewsStatusType
    {
        Standard = 0,
        GeneralGame
    }
}