using Elastic.Clients.Elasticsearch;

using Furion;
using Furion.JsonSerialization;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Services;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Encrypt;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

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

using static Grpc.Core.Metadata;

namespace ITPortal.AuthServer.Core.AccountService
{
    //[AppAuthorize]
    public class UserService : BaseService<UserEntity, UserDto, string>, IUserService, ITransient
    {
        private readonly IEmployeeBaseInfoService _employeeProxyService;
        public UserService(ISqlSugarClient db, IEmployeeBaseInfoService employeeProxyService,
            IDistributedCacheService cache) : base(db, cache, false)
        {
            _employeeProxyService = employeeProxyService;
        }

        public override ISugarQueryable<UserEntity> BuildFilterQuery(UserDto filter)
        {
            filter.Keyword = filter.Keyword?.ToLower();
            filter.Name = filter.Name?.ToLower();
            filter.UserName = filter.UserName?.ToLower();
            filter.Email = filter.Email?.ToLower();
            var roleQuery = CurrentDb.Queryable<RoleEntity>().Where(r => r.Status == 1);
            var Query = CurrentDb.Queryable<UserEntity>()
                .WhereIF(filter.Id.IsNotNullOrWhiteSpace(), f => f.Id == filter.Id)
                .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Id) == filter.Keyword || SqlFunc.ToLower(f.Name).Contains(filter.Keyword) || SqlFunc.ToLower(f.Email).Contains(filter.Keyword) || SqlFunc.ToLower(f.EnglishName).Contains(filter.Keyword))
                .WhereIF(filter.Name.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Name).Contains(filter.Name))
                .WhereIF(filter.UserName.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.UserName).Contains(filter.UserName))
                .WhereIF(filter.Email.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Email).Contains(filter.Email))
                .WhereIF(filter.Surname.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Surname) == filter.Surname);
            return Query.InnerJoin<UserRolesEntity>((f, ur) => f.Id == ur.UserId)
                 .InnerJoin(roleQuery, (f, ur, r) => ur.RoleId == r.Id)
                 .Select((f, ur, r) => new UserEntity()
                 {
                     Id = f.Id,
                     Name = f.Name,
                     Surname = f.Surname,
                     CreateBy = f.CreateBy,
                     CreateTime = f.CreateTime,
                     Department = f.Department,
                     Email = f.Email,
                     EnglishName = f.EnglishName,
                     //PasswordHash = f.PasswordHash,
                     PhoneNumber = f.PhoneNumber,
                     Status = f.Status,
                     RoleId = r.Id,
                     RoleName = r.Name,
                 }).Distinct()
                 .OrderByDescending(f => f.CreateTime);
        }




        public async Task<List<PrivilegeEntity>> GetUserMenus()
        {
            var roles = await GetUserRoles(CurrentUser.UserId);
            var queyr = CurrentDb.Queryable<RolePrivilegeEntity>().Where(r => roles.Select(f => f.Id).Contains(r.RoleId))
               .InnerJoin(CurrentDb.Queryable<PrivilegeEntity>(), (r, p) => r.PrivilegeId == p.Id)
               .Select((r, p) => p).Distinct();
            return await queyr.ToListAsync();
        }

        [Furion.DatabaseAccessor.UnitOfWork()]
        public async Task<int> CreateData(UserInputDto entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var empInfo = await GetEmployeeInfo(entity.Id);
                var userInfo = empInfo.Adapt<UserEntity>();
                var user = await Get(entity.Id);
                if (user == null || user.Id.IsNullOrWhiteSpace())
                {
                    await base.Create(userInfo);
                }
                else
                {
                    await base.ModifyHasChange(userInfo);
                    await CurrentDb.Deleteable<UserRolesEntity>().Where(f => SqlFunc.ToLower(f.UserId) == user.Id.ToLower()).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                }
                await CurrentDb.Insertable(new UserRolesEntity() { RoleId = entity.RoleId, UserId = entity.Id }).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
                uow.Commit();
            }
            return 1;
        }

        public async Task<int> ModifyData(UserInputDto entity)
        {
            using (var uow = CurrentDb.CreateContext())
            {
                var user = await GetEmployeeInfo(entity.Id);
                var userInfo = user.Adapt<UserEntity>();
                var result = await base.ModifyHasChange(userInfo);
                if (result)
                {
                    await CurrentDb.Insertable(new UserRolesEntity() { RoleId = entity.RoleId, UserId = entity.Id }).ExecuteCommandAsync();
                }
                uow.Commit();
            }
            return 1;
        }

        public override async Task<bool> Delete(string id, bool clearCache = true)
        {
            id = id?.ToLower();
            using (var uow = CurrentDb.CreateContext())
            {
                await CurrentDb.Deleteable<UserRolesEntity>().Where(f => SqlFunc.ToLower(f.UserId) == id).ExecuteCommandAsync();
                await base.Delete(id, clearCache);
                uow.Commit();
            }
            return true;
        }

        public async Task<List<UserEntity>> GetAllUser()
        {
            return await CurrentDb.Queryable<UserEntity>().ToListAsync();
        }

        public async Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false)
        {
            return await _employeeProxyService.GetAllEmployee(clearCache);
        }

        public async Task<List<JabusEmployeeInfo>> QueryEmployee(EmployeeQueryDto filter)
        {
            return await _employeeProxyService.QueryEmployee(filter);
        }

        public async Task<List<JabusEmployeeInfo>> QueryPageEmployee(EmployeeQueryDto filter)
        {
            List<JabusEmployeeInfo> list = await QueryEmployee(filter);
            return list.Skip(filter.SkipCount).Take(filter.PageSize).ToList();
        }

        public async Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID)
        {
            return await _employeeProxyService.GetEmployeeInfo(WorkNTID);
        }



        public async Task<UserEntity> GetUserRoleInfoById(string userId)
        {
            userId = userId?.ToLower();
            var userInfo = await CurrentDb.Queryable<UserEntity>()
                .Where(x => SqlFunc.ToLower(x.Id) == userId || SqlFunc.ToLower(x.Surname) == userId).FirstAsync();
            if (userInfo == null)
            {
                var emp = await _employeeProxyService.GetEmployeeInfo(userId);
                if (emp != null)
                {
                    userInfo = emp.Adapt(userInfo);
                    userInfo.Roles = (await GetDefaultRoles()).Select(f => f.Name).ToList();
                }
                return userInfo;
            }
            userInfo.Roles = (await GetUserRoles(userInfo.Id)).Select(f => f.Name).ToList();
            return userInfo;
        }


        public async Task<UserEntity> GetUserRoleInfo(string userName)
        {
            userName = userName?.ToLower();
            var userInfo = await CurrentDb.Queryable<UserEntity>()
                .Where(x => SqlFunc.ToLower(x.UserName) == userName || SqlFunc.ToLower(x.Surname) == userName || SqlFunc.ToLower(x.Email) == userName).FirstAsync();
            if (userInfo == null)
            {
                var emp = await _employeeProxyService.GetEmployeeInfo(userName);
                if (emp != null)
                {
                    userInfo = emp.Adapt(userInfo);
                    userInfo.Roles = (await GetDefaultRoles()).Select(f => f.Name).ToList();
                }
                return userInfo;
            }
            userInfo.Roles = (await GetUserRoles(userInfo.Id)).Select(f => f.Name).ToList();
            return userInfo;
        }


        public async Task<UserEntity> GetUserInfo(string userName)
        {
            userName = userName?.ToLower();
            return await CurrentDb.Queryable<UserEntity>()
                .Where(x => SqlFunc.ToLower(x.UserName) == userName || SqlFunc.ToLower(x.Surname) == userName || SqlFunc.ToLower(x.Email) == userName).FirstAsync();
        }


        public async Task<List<RoleEntity>> GetUserRoles(string userId)
        {
            userId = userId?.ToLower();
            var list = await CurrentDb.Queryable<RoleEntity>().RightJoin(
              CurrentDb.Queryable<UserRolesEntity>().Where(x => SqlFunc.ToLower(x.UserId )== userId)
              , (r, ur) => r.Id == ur.RoleId// && r.TenantId == ur.TenantId
              ).Where(r => r.Name != "").Select((r, ur) => r)
              .Distinct().ToListAsync();
            if (list.Count > 0) return list;
            return await CurrentDb.Queryable<RoleEntity>().Where(f => f.IsDefault).ToListAsync();
        }

        public async Task<List<RoleEntity>> GetDefaultRoles()
        {
            return await CurrentDb.Queryable<RoleEntity>().Where(f => f.IsDefault).ToListAsync();
        }



        //public async Task<int> Modify(UserEntity userInfo)
        //{
        //    return await _db.Updateable(userInfo).ExecuteCommandAsync();
        //}

        //public async Task<int> Create(UserEntity userInfo)
        //{
        //    return await _db.Insertable(userInfo).ExecuteCommandAsync();
        //}

        public string GenerateJwtToken(UserEntity userInfo)
        {
            var claims = new List<Claim>()
            {
                new Claim("aud",App.GetConfig<string>("JWTSettings:ValidAudience") ),//_jwtOptions.Audience
                 new Claim("iss",App.GetConfig<string>("JWTSettings:ValidIssuer")  ),// _jwtOptions.Issuer ),
                 new Claim("id", userInfo.Id.ToString() ),
                 new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString() ),
                 new Claim(ClaimTypes.Name, userInfo.UserName ),
                 new Claim("name", userInfo.UserName??"" ),
                 new Claim("enname", userInfo.EnglishName??"" ),
                 new Claim( ClaimTypes.GivenName, userInfo.Name??"" ),
                 new Claim(ClaimTypes.Email, userInfo.Email ??""),
                 new Claim(ClaimTypes.Surname, userInfo.Surname??"" ),
                 new Claim("depart", userInfo.Department??"" ),
                 new Claim(ClaimTypes.MobilePhone,userInfo.PhoneNumber??"" ),
                 new Claim(ClaimTypes.HomePhone,userInfo.PhoneNumber??"" )
            };

            foreach (var item in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            return JwtHelper.GenerateToken(claims.ToArray());
        }

    }
}
