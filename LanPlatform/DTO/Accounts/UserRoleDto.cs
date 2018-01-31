using System;
using System.Collections.Generic;
using LanPlatform.Accounts;

namespace LanPlatform.DTO.Accounts
{
    public class UserRoleDto : EditableGabionDto
    {
        public String Name { get; set; }

        public UserRoleDto()
        {
            Name = "";
        }

        public UserRoleDto(UserRole role)
            : base(role)
        {
            Name = role.Name;
        }

        public override string GetClassname()
        {
            return "UserRole";
        }

        public static List<GabionDto> ConvertList(ICollection<UserRole> roles)
        {
            var models = new List<GabionDto>();

            foreach (UserRole role in roles)
            {
                models.Add(new UserRoleDto(role));
            }

            return models;
        }
    }
}