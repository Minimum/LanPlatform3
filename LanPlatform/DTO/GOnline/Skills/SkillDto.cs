using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.GOnline.Skills;

namespace LanPlatform.DTO.GOnline.Skills
{
    public class SkillDto : EditableGabionDto
    {
        public String DevName { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public long BaseExperience { get; set; }
        public float LevelModifier { get; set; }

        public SkillDto()
        {
            DevName = "";
            Name = "";
            Description = "";

            BaseExperience = 0;
            LevelModifier = 0;
        }

        public SkillDto(Skill skill)
            : base(skill)
        {
            DevName = skill.DevName;
            Name = skill.Name;
            Description = skill.Description;

            BaseExperience = skill.BaseExperience;
            LevelModifier = skill.LevelModifier;
        }

        public override string GetClassname()
        {
            return "Skill";
        }

        public static List<GabionDto> ConvertList(ICollection<Skill> objects)
        {
            var models = new List<GabionDto>();

            foreach (Skill target in objects)
            {
                models.Add(new SkillDto(target));
            }

            return models;
        }
    }
}