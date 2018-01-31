using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.DTO
{
    public abstract class GabionDto
    {
        public long Id { get; set; }

        protected GabionDto()
        {
            Id = 0;
        }

        protected GabionDto(DatabaseObject model)
        {
            Id = model.Id;
        }

        public abstract String GetClassname();
    }
}