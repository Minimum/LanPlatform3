using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LanPlatform.GOnline.Skills;

namespace LanPlatform.DAL.GOnline
{
    public class GoContext : DbContext
    {
        public GoContext() : base("DataConnection")
        {

        }

        // Skills
        public DbSet<PlayerSkill> PlayerSkill { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<SkillGroup> SkillGroup { get; set; }
    }
}