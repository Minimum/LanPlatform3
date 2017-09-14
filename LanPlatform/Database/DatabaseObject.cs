using System.ComponentModel.DataAnnotations;

namespace LanPlatform.Database
{
    public abstract class DatabaseObject
    {
        [Key]
        public long Id { get; set; }

        public DatabaseObject()
        {
            Id = 0;
        }
    }

    
}