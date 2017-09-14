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

        public static List<UserRoleDto> ConvertList(ICollection<UserRole> roles)
        {
            List<UserRoleDto> models = new List<UserRoleDto>();

            foreach (UserRole role in roles)
            {
                models.Add(new UserRoleDto(role));
            }

            return models;
        }
    }
}