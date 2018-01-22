﻿using System;
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
        [HttpGet]
        public HttpResponseMessage GetSkills()
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            GoContext context = new GoContext();

            instance.Data = SkillDto.ConvertList((from s in context.Skill select s).ToList());
            instance.DataType = "SkillList";

            return instance.ToResponse();
        }

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

        [Route("{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteSkill(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(SkillManager.FlagDeleteSkill, GoManager.FlagScope))
            {
                GoContext context = new GoContext();
                Skill skill = (from s in context.Skill where s.Id == id select s).SingleOrDefault();

                if (skill != null)
                {
                    context.Skill.Remove(skill);

                    try
                    {
                        context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception e)
                    {
                        instance.SetError("SAVE_ERROR");
                    }
                }
                else
                {
                    instance.SetError("INVALID_SKILL");
                }
            }
            else
            {
                instance.SetError("ACCESS_DENIED");
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetSkill(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            GoContext context = new GoContext();

            instance.Data = (from s in context.Skill where s.Id == id select s).SingleOrDefault();
            instance.DataType = "Skill";

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("account/{id}")]
        public HttpResponseMessage GetPlayerSkills(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            GoContext context = new GoContext();

            instance.Data = (from ps in context.PlayerSkill
                join p in context.Skill on ps.Skill equals p.Id
                where ps.Player == id
                select new {p.Id, p.DevName, ps.Level, ps.Experience}).ToList();
            instance.DataType = "PlayerSkillList";

            return instance.ToResponse();
        }


    }
}
