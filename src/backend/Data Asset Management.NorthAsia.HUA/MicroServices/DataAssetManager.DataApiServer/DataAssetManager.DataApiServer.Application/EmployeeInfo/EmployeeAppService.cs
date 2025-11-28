using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;

using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.EmployeeInfo
{
    /// <summary>
    /// 账号管理
    /// </summary>
    //[AppAuthorize]
    //[Authorize]
    [Route("api/Employee/", Name = "Employee服务")]
    public class EmployeeAppService : IDynamicApiController
    {
        private readonly IEmployeeBaseInfoService _userService;
        private readonly ILogger<EmployeeAppService> _logger;
        public EmployeeAppService(IEmployeeBaseInfoService userService, ILogger<EmployeeAppService> logger)
        {
            _userService = userService;
            _logger = logger;
        }



        /// <summary>
        /// 获取所有员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false)
        {
            return await _userService.GetAllEmployee(clearCache);
        }



        /// <summary>
        /// 获取所有员工用户格式信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("UserInfo/All")]
        public async Task<Result<List<UserInfo>>> GetAllUserInfo(bool clearCache = false)
        {
            return await _userService.GetUsersAsync(clearCache);
        }



        /// <summary>
        /// 根据ntid获取员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Info")]
        [HttpGet("Info/{WorkNTID}")]
        public async Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID)
        {
            return await _userService.GetEmployeeInfo(WorkNTID);
        }


        /// <summary>
        /// 获取所有员工用户格式信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("List/{ntids}")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string ntids)
        {
            return await _userService.GetEmployeeListAsync(ntids);
        }


        /// <summary>
        /// 获取所有员工用户格式信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string[] ntids)
        {
            return await _userService.GetEmployeeListAsync(ntids);
        }


        /// <summary>
        /// 获取所有员工用户格式信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Manager")]
        public async Task<JabusEmployeeInfo> GetManagerAsync(string ntid)
        {
            return await _userService.GetManagerAsync(ntid);
        }

        /// <summary>
        /// 获取所有员工用户格式信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("ManagerNtid")]
        public async Task<string> GetManagerNtIdAsync(string ntid)
        {
            return await _userService.GetManagerNtIdAsync(ntid);
        }


        /// <summary>
        /// 已查询员工信心
        /// </summary>
        /// <returns></returns>
        [HttpPost("Query")]
        public async Task<List<JabusEmployeeInfo>> Query(EmployeeQueryDto filter)
        {
            return await _userService.QueryPageEmployee(filter);
        }

        /// <summary>
        /// 分页查询员工信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("Page")]
        public async Task<List<JabusEmployeeInfo>> Page(EmployeeQueryDto filter)
        {
            return await _userService.QueryEmployee(filter);
        }


        /// <summary>
        /// 获取指数主管员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Master/{ntid}")]
        public async Task<JabusEmployeeInfo> getMasterByUserId(string ntid)
        {
            var emp = await _userService.GetEmployeeInfo(ntid);
            return await _userService.GetEmployeeInfo(emp?.ManagerNTID);
        }
    }
}
