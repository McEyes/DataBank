using Furion;
using Furion.JsonSerialization;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.Core.Encrypt;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using MapsterMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.AuthServer.Core.AccountService
{
    public interface IRoleService:IBaseService<RoleEntity,RoleDto,string>
    {

        Task<RoleEntity> GetRoleInfo(string roleName);

        Task<List<RoleEntity>> GetAllRole();
        Task<List<RoleEntity>> GetDefaultRoles();
    }
}
