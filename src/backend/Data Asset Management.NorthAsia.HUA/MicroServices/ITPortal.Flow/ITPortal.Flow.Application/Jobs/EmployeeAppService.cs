
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using IResult = ITPortal.Core.Services.IResult;


namespace ITPortal.Flow.Application.Jobs
{
    /// <summary>
    /// 账号管理
    /// </summary>
    [AppAuthorize]
    //[Authorize]
    [Route("api/Employee/", Name = "Employee服务")]
    public class EmployeeAppService : IDynamicApiController
    {
        private readonly MyEmployeeProxyService _userService;
        private readonly ILogger<EmployeeAppService> _logger;
        public EmployeeAppService(MyEmployeeProxyService userService, ILogger<EmployeeAppService> logger)
        {
            _userService = userService;
            _logger = logger;
        }



        /// <summary>
        /// 获取所有员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<IResult> GetAllEmployee()
        {
            return await _userService.AllEmployeeAsync();
        }


        /// <summary>
        /// 根据ntid获取员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Info")]
        [HttpGet("Info/{WorkNTID}")]
        public async Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID)
        {
            return await _userService.GetEmployeeAsync(WorkNTID);
        }

        [HttpGet("List")]
        [HttpGet("List/{ntids}")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string ntids)
        {
            return await _userService.GetEmployeeListAsync(ntids);
        }

        [HttpPost("List")]
        public async Task<List<JabusEmployeeInfo>> GetManagerNtIdAsync([FromBody] string[] ntids)
        {
            return await _userService.GetEmployeeListAsync(ntids);
        }



        /// <summary>
        /// 获取主管员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Master/{ntid}")]
        public async Task<JabusEmployeeInfo> getMasterByUserId(string ntid)
        {
            return await _userService.GetManagerAsync(ntid);
        }

        /// <summary>
        /// 获取主管ntid
        /// </summary>
        /// <returns></returns>
        [HttpGet("Master/Ntid/{ntid}")]
        public async Task<string> GetManagerNtIdAsync(string ntid)
        {
            return await _userService.GetManagerNtIdAsync(ntid);
        }


    }
}
