using System;
using LanPlatform.Database;

namespace LanPlatform.Content
{
    public class ContentItem : EditableDatabaseObject
    {
        public long Owner { get; set; }
        public bool Visible { get; set; }
        public String Hash { get; set; }
        public String Filename { get; set; }
        public long Size { get; set; }
        public ContentType Type { get; set; }
        public long TimeAdded { get; set; }

        public String DataMime
        {
            get
            {
                String mime = "";

                switch (Type)
                {
                    case ContentType.Binary:
                    {
                        mime = "application/octet-stream";

                        break;
                    }

                    case ContentType.ImageBmp:
                    {
                        mime = "image/bmp";

                        break;
                    }

                    case ContentType.ImageGif:
                    {
                        mime = "image/gif";

                        break;
                    }

                    case ContentType.ImageJpg:
                    {
                        mime = "image/jpeg";

                        break;
                    }

                    case ContentType.ImagePng:
                    {
                        mime = "image/png";

                        break;
                    }
                    
                    case ContentType.ImageSvg:
                    {
                        mime = "image/svg+xml";

                        break;
                    }
                }

                return mime;
            }
        }

        public bool IsImage
        {
            get
            {
                bool success = false;

                switch (Type)
                {
                    case ContentType.ImageBmp:
                    case ContentType.ImageGif:
                    case ContentType.ImageJpg:
                    case ContentType.ImagePng:
                    case ContentType.ImageSvg:
                    {
                        success = true;

                        break;
                    }
                }

                return success;
            }
        }

        public ContentItem()
        {
            Owner = 0;
            Visible = true;
            Hash = "";
            Filename = "";
            Size = 0;
            Type = ContentType.None;
            TimeAdded = 0;
        }
    }

    public enum ContentType
    {
        None = 0,
        Binary,
        ImageBmp,
        ImageGif,
        ImageJpg,
        ImagePng,
        ImageSvg
    }
}