using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.DTO.GOnline.Skills
{
    public class PlayerSkillDto : EditableGabionDto
    {
        public long Player { get; set; }
        public long Skill { get; set; }

        public int Level { get; set; }
        public long Experience { get; set; }


    }
}