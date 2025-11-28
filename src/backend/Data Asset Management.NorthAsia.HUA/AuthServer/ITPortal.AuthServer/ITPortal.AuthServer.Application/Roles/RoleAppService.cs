using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.AuthServer.Core.AccountService;
using ITPortal.Core.Services;

using Mapster;

using MapsterMapper;

using Microsoft.AspNetCore.Identity;

namespace ITPortal.AuthServer.Application
{
    /// <summary>
    /// 账号管理
    /// </summary>
    [AppAuthorize]
    [Authorize]
    public class RoleAppService : IDynamicApiController
    {
        private readonly IRoleService _roleService;
        public RoleAppService(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("/RoleInfo/{roleName}")]
        public async Task<RoleDto> GetRoleInfo(string roleName)
        {
            return (await _roleService.GetRoleInfo(roleName)).Adapt<RoleDto>();
        }




        #region base api


        [HttpPost()]
        public async Task<int> Create(RoleEntity entity)
        {
            return await _roleService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _roleService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<RoleEntity> Get(string id)
        {
            return await _roleService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _roleService.Delete(ids);
        }


        public async Task<RoleEntity> Single(RoleDto entity)
        {
            return await _roleService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(RoleEntity entity)
        {
            return await _roleService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(RoleEntity entity)
        {
            return await _roleService.ModifyHasChange(entity);
        }


        [HttpPost("page")]
        public async Task<PageResult<RoleEntity>> Page(RoleDto filter)
        {
            return await _roleService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<RoleEntity>> Page2([FromQuery] RoleDto filter)
        {
            return await _roleService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<RoleEntity>> Query(RoleDto entity)
        {
            return await _roleService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<RoleEntity>> Query2([FromQuery] RoleDto entity)
        {
            return await _roleService.Query(entity);
        }

        #endregion base api

    }
}
