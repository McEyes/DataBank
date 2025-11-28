using Elastic.Clients.Elasticsearch;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.Account.Services.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Services;
using ITPortal.AuthServer.Core.AccountService;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Security.Principal;

namespace ITPortal.AuthServer.Application.EmployeeInfos
{
    /// <summary>
    /// 账号管理
    /// </summary>
    [AppAuthorize]
    //[Authorize]
    [Route("api/Employee/", Name = "Employee服务")]
    public class EmployeeAppService : IDynamicApiController
    {
        private readonly IEmployeeBaseInfoService _userService;
        private readonly ILogger<AccountAppService> _logger;
        public EmployeeAppService(IEmployeeBaseInfoService userService, ILogger<AccountAppService> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        /// <summary>
        /// 检查config参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public dynamic GetConfig(string key = "RemoteApi:AppHostUrl")
        {
            return App.GetConfig<string>(key);
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
        public async Task<List<UserInfoDto>> GetAllUserInfo(bool clearCache = false)
        {
            var list = await _userService.GetAllEmployee(clearCache);
            return list.Adapt<List<UserInfoDto>>();
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
        /// 根据ID/wdid获取员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("Info/wdid/{wdid}")]
        public async Task<JabusEmployeeInfo> GetEmployeeInfoByWdid(string wdid)
        {
            return (await _userService.Get(wdid)).Adapt<JabusEmployeeInfo>();
        }

        /// <summary>
        /// 根据ID/wdid获取员工信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<JabusEmployeeInfo> GetEmployeeInfoByid(string id)
        {
            return (await _userService.Get(id)).Adapt<JabusEmployeeInfo>();
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


        [HttpGet("UpdateMasterEmployee")]
        public async Task<ITPortal.Core.Services.IResult> UpdateMasterEmployee()
        {
            var result = await App.GetService<EmployeeProxyService>().AllMasterEmployeeAsync();
            if (result?.Success == true)
            {
                var dataResult = result as Result<PageResult<MasterEmployeeInfo>>;
                var list = dataResult.Data.Data.Adapt<List<EmployeeBaseInfo>>();
                return await _userService.ImportData(list);
            }
            else
            {
                return result;
            }
        }


        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="ntid">ntid，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        [HttpGet("EmployeeDeptManager/{ntid}")]
        [HttpGet("GetEmployeeDeptManager")]
        public async Task<JabusEmployeeInfo> GetEmployeeDeptManagerAsync(string ntid)
        {
            return await _userService.GetEmployeeDeptManagerAsync(ntid);
        }

        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="dept">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        [HttpGet("GetDeptManager")]
        [HttpGet("DeptManager/{dept}")]
        public async Task<JabusEmployeeInfo> GetDeptManagerAsync(string dept)
        {
            return await _userService.GetDeptManagerAsync(dept);
        }


        /// <summary>
        ///  获取Cost Center Function Manager
        /// </summary>
        /// <param name="costCenterId">Cost Center ID，区分大小写</param>
        /// <returns>Function Manager</returns>
        [HttpGet("GetFunctionManager")]
        [HttpGet("FunctionManager/{costCenterId}")]
        public async Task<JabusEmployeeInfo> GetFunctionManager(string costCenterId)
        {
            return await _userService.GetFunctionManagerAsync(costCenterId);
        }

        /// <summary>
        ///  获取员工 Function Manager
        /// </summary>
        /// <param name="ntid">ntid，区分大小写</param>
        /// <returns>Function Manager</returns>
        [HttpGet("EmployeeFM/{ntid}")]
        [HttpGet("GetEmployeeFM")]
        public async Task<JabusEmployeeInfo> GetEmployeeFMAsync(string ntid)
        {
            return await _userService.GetEmployeeFMAsync(ntid);
        }

        /// <summary>
        /// 所有部门清单
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllDepartments")]
        [HttpGet("/api/Department/All")]
        public async Task<List<string>> GetAllDepartments()
        {
            return await _userService.GetAllDepartments();
        }
    }
}
