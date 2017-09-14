using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

namespace LanPlatform.Database
{
    public abstract class EditableDatabaseObject : DatabaseObject
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }

        [NotMapped]
        public UInt64 Version
        {
            get
            {
                if (RowVersion != null)
                {
                    return BitConverter.ToUInt64(RowVersion.Reverse().ToArray(), 0);
                }
                else
                {
                    return 0;
                }
            }

            set { RowVersion = BitConverter.GetBytes(value).Reverse().ToArray(); }
        }

        public EditableDatabaseObject()
        {
            RowVersion = null;
        }
    }
}