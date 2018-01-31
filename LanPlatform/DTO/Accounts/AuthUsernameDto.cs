using System;
using System.Collections.Generic;
using LanPlatform.Auth;

namespace LanPlatform.DTO.Accounts
{
    public class AuthUsernameDto : EditableGabionDto
    {
        public String Username { get; set; }
        public String Password { get; set; }

        public AuthUsernameDto()
        {
            Username = "";
            Password = "";
        }

        public AuthUsernameDto(AuthUsername auth)
            : base(auth)
        {
            Username = auth.Username;
            Password = "";
        }

        public override string GetClassname()
        {
            return "AuthUsername";
        }

        public static List<GabionDto> ConvertList(ICollection<AuthUsername> objects)
        {
            var models = new List<GabionDto>();

            foreach (AuthUsername target in objects)
            {
                models.Add(new AuthUsernameDto(target));
            }

            return models;
        }
    }
}