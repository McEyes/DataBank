using Furion;
using Furion.JsonSerialization;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
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

namespace ITPortal.AuthServer.Core.AccountService
{
    public class RoleService : BaseService<RoleEntity, RoleDto, string>, IRoleService, ITransient
    {
        public RoleService(ISqlSugarClient db, IDistributedCacheService cache) : base(db, cache, false)
        {
        }

        public override ISugarQueryable<RoleEntity> BuildFilterQuery(RoleDto filter)
        {
            filter.Keyword = filter.Keyword?.ToLower();
            return CurrentDb.Queryable<RoleEntity>()
                .Where(f => f.Status == 1)
                .WhereIF(filter.Id.IsNotNullOrWhiteSpace(), f => f.Id == filter.Id)
                .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Id) == filter.Keyword || SqlFunc.ToLower(f.Name) == filter.Keyword || SqlFunc.ToLower(f.Code) == filter.Keyword)
                .WhereIF(filter.Name.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Name).Contains(filter.Name))
                .WhereIF(filter.Code.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Code).Contains(filter.Code));
        }


        public async Task<List<RoleEntity>> GetAllRole()
        {
            return await CurrentDb.Queryable<RoleEntity>().ToListAsync();
        }

        public async Task<RoleEntity> GetRoleInfo(string roleName)
        {
            roleName= roleName?.ToLower();
            return await CurrentDb.Queryable<RoleEntity>()
                .Where(x => SqlFunc.ToLower(x.Name) == roleName || SqlFunc.ToLower(x.Code) == roleName).FirstAsync();
        }

        public async Task<List<RoleEntity>> GetDefaultRoles()
        {
            return await CurrentDb.Queryable<RoleEntity>().Where(f => f.IsDefault).ToListAsync();
        }


        public override async Task<bool> Delete(string id, bool clearCache = true)
        {
            var role =await Get(id);
            if (role != null && role.IsStatic) throw new AppFriendlyException("System roles cannot be deleted！", 6001);
            return await base.Delete(id, clearCache);
        }

    }
}
