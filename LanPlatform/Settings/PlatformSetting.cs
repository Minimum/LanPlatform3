using System;
using System.Data;
using LanPlatform.Database;

namespace LanPlatform.Settings
{
    public class PlatformSetting : EditableDatabaseObject
    {
        public String Name { get; set; }
        public String Value { get; set; }

        public String Title { get; set; }
        public String Description { get; set; }

        public PlatformSetting()
        {
            Name = "";
            Value = "";

            Title = "";
            Description = "";
        }

        public PlatformSetting(String name, String title, String description, String value)
        {
            Name = name;
            Value = value;

            Title = title;
            Description = description;
        }

        public Int64 ToInt64()
        {
            Int64 value;

            Int64.TryParse(Value, out value);

            return value;
        }

        public Int32 ToInt32()
        {
            Int32 value;

            Int32.TryParse(Value, out value);

            return value;
        }
    }
}