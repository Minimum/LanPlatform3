using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.DAL;
using LanPlatform.Models;

namespace LanPlatform.Controllers.GOnline
{
    [RoutePrefix("api/gso/skill")]
    public class SkillController : ApiController
    {
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
