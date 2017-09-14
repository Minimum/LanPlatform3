using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Accounts;

namespace LanPlatform.DTO.Accounts
{
    public class UserPermissionDto : EditableGabionDto
    {
        public long Role { get; set; }
        public String Scope { get; set; }
        public String Flag { get; set; }

        public UserPermissionDto()
        {
            Role = 0;
            Scope = "";
            Flag = "";
        }

        public UserPermissionDto(UserPermission permission)
            : base(permission)
        {
            Role = permission.Role;
            Scope = permission.Scope;
            Flag = permission.Flag;
        }

        public static List<UserPermissionDto> ConvertList(ICollection<UserPermission> permissions)
        {
            List<UserPermissionDto> models = new List<UserPermissionDto>();

            foreach (UserPermission permission in permissions)
            {
                models.Add(new UserPermissionDto(permission));
            }

            return models;
        }
    }
}