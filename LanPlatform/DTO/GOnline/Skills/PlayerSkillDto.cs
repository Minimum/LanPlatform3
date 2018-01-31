using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.GOnline.Skills;

namespace LanPlatform.DTO.GOnline.Skills
{
    public class PlayerSkillDto : EditableGabionDto
    {
        public long Player { get; set; }
        public long Skill { get; set; }

        public int Level { get; set; }
        public long Experience { get; set; }

        public PlayerSkillDto()
        {
            Player = 0;
            Skill = 0;

            Level = 0;
            Experience = 0;
        }

        public PlayerSkillDto(PlayerSkill skill)
            : base(skill)
        {
            Player = skill.Player;
            Skill = skill.Skill;

            Level = skill.Level;
            Experience = skill.Experience;
        }

        public override string GetClassname()
        {
            return "PlayerSkill";
        }

        public static List<GabionDto> ConvertList(ICollection<PlayerSkill> objects)
        {
            var models = new List<GabionDto>();

            foreach (PlayerSkill target in objects)
            {
                models.Add(new PlayerSkillDto(target));
            }

            return models;
        }
    }
}