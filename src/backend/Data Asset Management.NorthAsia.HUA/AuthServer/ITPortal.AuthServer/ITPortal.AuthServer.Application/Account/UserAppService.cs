using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Core.AccountService;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

namespace ITPortal.AuthServer.Application
{
    /// <summary>
    /// 账号管理
    /// </summary>
    [AppAuthorize]
    //[Authorize]
    public class UserAppService : IDynamicApiController
    {
        private readonly IUserService _userService;
        public UserAppService(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// http://cnhuam0appprd03:6001/api/User/UserFlowCallback
        /// </summary>
        /// <returns></returns>
        [HttpPost("UserFlowCallback")]
        public async Task UserApplayCallback(Result<FlowBackDataEntity> flowCallBackResult)
        {
            if (flowCallBackResult.Success)
            {
                var flowInst = flowCallBackResult.Data.FlowInst;
                var act = flowCallBackResult.Data.CurrentAct;
                var opt = flowCallBackResult.Data.ActionType;
                if (flowInst.FlowStatus == ITPortal.Core.ProxyApi.Flow.Enums.FlowStatus.Completed && opt == ITPortal.Core.ProxyApi.Flow.Enums.FlowAction.Approval)
                {
                    await _userService.CreateData(new UserInputDto() { Id = flowInst.Applicant, RoleId = "2" });
                }
            }
        }


        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userid}")]
        public async Task<UserDto> GetUserInfoById(string userid)
        {
            return (await _userService.GetUserRoleInfoById(userid)).Adapt<UserDto>();
        }

        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("/UserInfo/{userName}")]
        public async Task<UserDto> GetUserInfo(string userName)
        {
            return (await _userService.GetUserInfo(userName)).Adapt<UserDto>();
        }

        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("/UserInfo/{userName}/Roles")]
        public async Task<UserDto> GetUserRoleInfo(string userName)
        {
            return (await _userService.GetUserRoleInfo(userName)).Adapt<UserDto>();
        }

        /// <summary>
        /// 根据用户名(ntid)或者邮件地址查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userid}/Roles")]
        public async Task<List<RoleEntity>> GetUserRoles(string userid)
        {
            return await _userService.GetUserRoles(userid);
        }


        /// <summary>
        /// 获取当前登陆用户的菜单权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("OwnerMenus")]
        public async Task<List<PrivilegeDto>> GetUserMenus()
        {
            var list = await _userService.GetUserMenus();
            return list.Adapt<List<PrivilegeDto>>();
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(UserInputDto entity)
        {
            return await _userService.CreateData(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _userService.Delete(id);
        }


        ///// <summary>
        ///// 获取详细信息
        ///// 根据url的id来获取详细信息
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[HttpGet("{id}")]
        //public async Task<UserEntity> Get(string id)
        //{
        //    return await _userService.Get(id);
        //}


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _userService.Delete(ids);
        }


        public async Task<UserEntity> Single(UserDto entity)
        {
            return await _userService.Single(entity);
        }

        [HttpPut()]
        public async Task<int> Put(UserInputDto entity)
        {
            return await _userService.ModifyData(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(UserEntity entity)
        {
            return await _userService.ModifyHasChange(entity);
        }


        [HttpPost("page")]
        public async Task<PageResult<UserEntity>> Page(UserDto filter)
        {
            return await _userService.Page(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<UserEntity>> Page2([FromQuery] UserDto filter)
        {
            return await _userService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<UserEntity>> Query(UserDto entity)
        {
            return await _userService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<UserEntity>> Query2([FromQuery] UserDto entity)
        {
            return await _userService.Query(entity);
        }

        #endregion base api

    }
}
