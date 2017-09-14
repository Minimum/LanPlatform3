using System;
using LanPlatform.Database;

namespace LanPlatform.DTO
{
    public abstract class EditableGabionDto : GabionDto
    {
        public UInt64 Version { get; set; }

        protected EditableGabionDto()
        {
            Version = 0;
        }

        protected EditableGabionDto(EditableDatabaseObject model)
            : base(model)
        {
            Version = model.Version;
        }
    }
}