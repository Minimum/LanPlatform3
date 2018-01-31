using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Auth;

namespace LanPlatform.DTO.Accounts
{
    public class AuthSessionDto : EditableGabionDto
    {
        public String Key { get; set; }
        public long ExpireDate { get; set; }

        public AuthSessionDto()
        {
            Key = "";
            ExpireDate = 0;
        }

        public AuthSessionDto(AuthSession session)
            : base(session)
        {
            Key = session.Key;
            ExpireDate = session.ExpireDate;
        }

        public override string GetClassname()
        {
            return "AuthSession";
        }

        public static List<GabionDto> ConvertList(ICollection<AuthSession> objects)
        {
            var models = new List<GabionDto>();

            foreach (AuthSession target in objects)
            {
                models.Add(new AuthSessionDto(target));
            }

            return models;
        }
    }
}