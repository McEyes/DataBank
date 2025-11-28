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
    public interface IUserService:IBaseService<UserEntity,UserDto,string>
    {

        Task<UserEntity> GetUserRoleInfo(string userName);


        Task<UserEntity> GetUserInfo(string userName);


        Task<List<RoleEntity>> GetUserRoles(string userId);
        string GenerateJwtToken(UserEntity userInfo);
        //Task<int> Modify(UserEntity userInfo);
        Task<UserEntity> GetUserRoleInfoById(string userId);
        Task<List<UserEntity>> GetAllUser();
        Task<List<RoleEntity>> GetDefaultRoles();
        Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false);
        Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID);
        Task<List<JabusEmployeeInfo>> QueryEmployee(EmployeeQueryDto filter);
        Task<List<JabusEmployeeInfo>> QueryPageEmployee(EmployeeQueryDto filter);
        Task<List<PrivilegeEntity>> GetUserMenus();
        Task<int> CreateData(UserInputDto entity);
        Task<int> ModifyData(UserInputDto entity);
    }
}
