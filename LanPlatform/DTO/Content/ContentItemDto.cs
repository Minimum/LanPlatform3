using System;
using LanPlatform.Content;

namespace LanPlatform.DTO.Content
{
    public class ContentItemDto : EditableGabionDto
    {
        public long Owner { get; set; }
        public bool Visible { get; set; }
        public String Hash { get; set; }
        public String Filename { get; set; }
        public long Size { get; set; }
        public ContentType Type { get; set; }
        public long TimeAdded { get; set; }

        public ContentItemDto()
        {
            Owner = 0;
            Visible = false;
            Hash = "";
            Filename = "";
            Size = 0;
            Type = ContentType.None;
            TimeAdded = 0;
        }

        public ContentItemDto(ContentItem item)
            : base(item)
        {
            Owner = item.Owner;
            Visible = item.Visible;
            Hash = item.Hash;
            Filename = item.Filename;
            Size = item.Size;
            Type = item.Type;
            TimeAdded = item.TimeAdded;
        }

        public override string GetClassname()
        {
            return "ContentItem";
        }
    }
}