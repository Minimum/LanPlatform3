using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.DAL;
using LanPlatform.DTO.GOnline.Skills;
using LanPlatform.GOnline;
using LanPlatform.GOnline.Skills;
using LanPlatform.Models;

namespace LanPlatform.Controllers.GOnline
{
    [RoutePrefix("api/go/skill")]
    public class SkillController : ApiController
    {
        [Route("")]
        [HttpPut]
        public HttpResponseMessage AddSkill([FromBody] SkillDto skill)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(SkillManager.FlagAddSkill, GoManager.FlagScope))
            {
                GoContext context = new GoContext();
                Skill newSkill = new Skill();

                newSkill.DevName = skill.DevName;
                newSkill.Name = skill.Name;
                newSkill.Description = skill.Description;

                newSkill.BaseExperience = skill.BaseExperience;
                newSkill.LevelModifier = skill.LevelModifier;

                context.Skill.Add(newSkill);

                try
                {
                    context.SaveChanges();

                    instance.SetData(newSkill, "Skill");
                }
                catch (Exception e)
                {
                    instance.SetError("SAVE_ERROR");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [Route("account/{id}")]
        [HttpGet]
        public HttpResponseMessage GetPlayerSkills(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            GoContext context = new GoContext();

            instance.Data = (from ps in context.PlayerSkill
                join p in context.Skill on ps.Skill equals p.Id
                where ps.Player == id
                select new {p.DevName, ps.Level, ps.Experience}).ToList();

            return instance.ToResponse();
        }


    }
}
