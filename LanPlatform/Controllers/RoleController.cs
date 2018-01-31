using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using LanPlatform.Accounts;
using LanPlatform.DTO.Accounts;
using LanPlatform.Models;

namespace LanPlatform.Controllers
{
    [RoutePrefix("api/role")]
    public class RoleController : ApiController
    {
        [HttpPut]
        [Route("")]
        public HttpResponseMessage CreateRole([FromBody] UserRoleDto role)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(AccountManager.FlagEditRoles))
            {
                UserRole newRole = new UserRole();

                newRole.Name = role.Name;

                instance.Accounts.AddRole(newRole);

                try
                {
                    instance.Context.SaveChanges();

                    instance.SetData(new UserRoleDto(newRole));
                }
                catch (Exception)
                {
                    instance.SetError("SaveError");
                }

                // TODO: Log
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagEditRoles);
            }

            return instance.ToResponse();
        }

        [HttpGet]
        [Route("{id}")]
        public HttpResponseMessage GetRole(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);
            UserRole role = instance.Accounts.GetRoleById(id);

            if (role != null)
            {
                instance.SetData(new UserRoleDto(role));
            }
            else
            {
                instance.SetError("InvalidRole");
            }

            return instance.ToResponse();
        }

        [HttpPost]
        [Route("{id}")]
        public HttpResponseMessage EditRole(long id, [FromBody] UserRoleDto roleEdit)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.CheckAccess(AccountManager.FlagEditRoles))
            {
                UserRole role = instance.Accounts.GetRoleById(id);

                if (role != null)
                {
                    role.Name = roleEdit.Name;

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(new UserRoleDto(role));
                    }
                    catch (Exception e)
                    {
                        if (e is OptimisticConcurrencyException)
                        {
                            instance.SetError("ConcurrencyError");
                        }
                        else
                        {
                            instance.SetError("SaveError");
                        }
                    }
                }
                else
                {
                    instance.SetError("InvalidRole");
                }
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagEditRoles);
            }

            return instance.ToResponse();
        }

        // DELETE	/{id}

        // Access actions

        [HttpGet]
        [Route("{id}/access")]
        public HttpResponseMessage GetRolePermissions(long id)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.LoggedIn)
            {
                List<UserPermission> permissions = instance.Accounts.GetRolePermissions(id);

                instance.SetData(UserPermissionDto.ConvertList(permissions), "UserPermissionList");
            }
            else
            {
                instance.SetAccessDenied("AnonymousUser");
            }

            return instance.ToResponse();
        }

        // GET		/{id}/access/{scope}

        // GET		/{id}/access/{scope}/{flag}

        [HttpPut]
        [Route("{id}/access/{scope}/{flag}")]
        public HttpResponseMessage AddRolePermission(long id, String scope, String flag)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            UserPermissionDto permission = new UserPermissionDto();

            permission.Flag = flag;
            permission.Scope = scope;

            if (instance.CheckAccess(AccountManager.FlagEditRoles))
            {
                if (permission.Flag.Length > 0 && permission.Scope.Length > 0)
                {
                    UserRole role = instance.Accounts.GetRoleById(id);

                    if (role != null)
                    {
                        UserPermission newPermission = instance.Accounts.GetPermission(id, permission.Flag,
                            permission.Scope);

                        bool success = newPermission != null;

                        if (!success)
                        {
                            newPermission = new UserPermission();

                            newPermission.Role = id;
                            newPermission.Flag = permission.Flag;
                            newPermission.Scope = permission.Scope;

                            instance.Accounts.AddPermission(newPermission);

                            try
                            {
                                instance.Context.SaveChanges();

                                success = true;
                            }
                            catch (Exception)
                            {
                                instance.SetError("SaveError");
                            }
                        }

                        if(success)
                            instance.SetData(new UserPermissionDto(newPermission));
                    }
                    else
                    {
                        instance.SetError("InvalidRole");
                    }
                }
                else
                {
                    instance.SetError("InvalidPermission");
                }
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagEditRoles);
            }

            return instance.ToResponse();
        }

        [HttpDelete]
        [Route("{id}/access/{scope}/{flag}")]
        public HttpResponseMessage DeleteRolePermission(long id, String scope, String flag)
        {
            AppInstance instance = new AppInstance(Request, HttpContext.Current);

            if (instance.Accounts.CheckAccess(AccountManager.FlagEditRoles))
            {
                UserPermission target = instance.Accounts.GetPermission(id, flag, scope);

                if (target != null)
                {
                    instance.Accounts.RemovePermission(target);

                    try
                    {
                        instance.Context.SaveChanges();

                        instance.SetData(true, "bool");
                    }
                    catch (Exception)
                    {
                        instance.SetError("SaveError");
                    }
                }
                else
                {
                    instance.SetError("InvalidPermission");
                }
            }
            else
            {
                instance.SetAccessDenied(AccountManager.FlagEditRoles);
            }

            return instance.ToResponse();
        }
    }
}
