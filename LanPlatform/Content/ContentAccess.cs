using LanPlatform.Database;

namespace LanPlatform.Content
{
    public class ContentAccess : EditableDatabaseObject
    {
        public long Content { get; set; }
        public ContentAccessType Type { get; set; }
        public long User { get; set; }

        public ContentAccess()
        {
            Content = 0;
            Type = ContentAccessType.None;
            User = 0;
        }
    }

    public enum ContentAccessType
    {
        None = 0,
        User,
        Role
    }
}