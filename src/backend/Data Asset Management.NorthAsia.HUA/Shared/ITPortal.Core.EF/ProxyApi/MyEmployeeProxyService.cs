

using Furion;
using Furion.HttpRemote;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Emails;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

namespace ITPortal.Core.ProxyApi
{


    /// <summary>
    /// 员工数据获取代理接口
    /// 实现了对/userapi/Employee/info接口的数据获取
    /// </summary>
    //[BaseAddress("http://10.115.63.135:4200/gateway/")]
    public class MyEmployeeProxyService : BaseProxyService//: IHttpDeclarative
    {
        private readonly string authHostUrl;
        private readonly ILogger<MyEmployeeProxyService> _logger;
        private readonly IEmailSender _emailSender;

        public MyEmployeeProxyService(IHttpRemoteService httpRemoteService, IEmailSender emailSender, ILogger<MyEmployeeProxyService> logger) : base(httpRemoteService)
        {
            authHostUrl = App.GetConfig<string>("RemoteApi:AuthHostUrl");
            _emailSender = emailSender;
            _logger = logger;
        }

        public override string GetToken()
        {
            var token = base.GetToken();
            if (token.IsNullOrWhiteSpace())
            {
                token = JwtHelper.GenerateJwtToken(new UserInfo()
                {
                    ChineseName = "auto approve",
                    EnglishName = "auto approve",
                    Department = "auto approve",
                    Email = "Anny_Wu@jabil.com",
                    Id = "AutoApproveUser",
                    Name = "Auto Approve User",
                    NtId = "AutoApproveUser",
                    Surname = "AutoApproveUser",
                    UserId = "AutoApproveUser",
                    UserName = "AutoApproveUser",
                    PhoneNumber = "",
                    Roles = new List<string>()
                });
            }
            return token;
        }

        /// <summary>
        /// 获取所有userInfo清单
        /// 实际请求接口：/gateway/homeapi/api/home/Common/GetUserToEmployeeModList
        /// http://cnhuam0itpoc81:4200/gateway/eureka/data/api/services/v1.0.0/HR/Base/tp_employee_base_info/query
        /// "x-token", "3b89b6aaa232a39461eb704f53656f74"
        /// </summary>
        /// <returns></returns>
        //[HttpGet("http://10.115.63.135:4200/gateway/homeapi/api/home/Common/GetUserToEmployeeModList")]
        public async Task<IResult> AllEmployeeAsync()
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/all",
            builder => builder.SetContent(new { PageIndex = 1, PageSize = 100000 }, "application/json;charset=utf-8")
            .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                return resultStr.To<Result<List<JabusEmployeeInfo>>>();
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
                //await SendEmployeeApiErrorNotice(result);
            }
            return result;
        }

        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="WorkNTID">ntid or email</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<JabusEmployeeInfo> GetEmployeeAsync(string WorkNTID)
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/info/{WorkNTID}",
            builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
            .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<JabusEmployeeInfo>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return null;
        }


        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="ntidOrEmail">ntid or email</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<JabusEmployeeInfo> GetEmployeeByNtIdOrEmailAsync(string ntidOrEmail)
        {
            return await GetEmployeeAsync(ntidOrEmail);
        }
        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="ntidOrEmail">ntid or email</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<JabusEmployeeInfo> Get(string idOrwdid)
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/{idOrwdid}",
            builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
            .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<JabusEmployeeInfo>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return null;
        }

        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="WorkNTID">ntid</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string WorkNTIDs)
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/list/{WorkNTIDs}",
          builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
          .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<List<JabusEmployeeInfo>>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return new List<JabusEmployeeInfo>();
        }

        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="WorkNTID">ntid</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string[] ntids)
        {
            var resultStr = await httpRemoteService.PostAsStringAsync($"{authHostUrl}/api/Employee/list/",
                                      builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
                                      .SetContent(ntids, "application/json;charset=utf-8")
                                      .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<List<JabusEmployeeInfo>>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return new List<JabusEmployeeInfo>();
        }


        //await httpRemoteService.GetAsAsync<Result<JabusEmployeeInfo>>($"{BaseHostUrl}/userapi/Employee/info?WorkNTID={WorkNTID}",
        //builder => builder.SetContent(new object(), "application/json;charset=utf-8").AddAuthentication("Bearer", GetToken()));

        /// <summary>
        /// 根据员工ntid获取上级领导ntid
        /// </summary>
        /// <param name="ntid">ntid</param>
        /// <returns>上级etid</returns>
        public async Task<string> GetManagerNtIdAsync(string ntid)
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/master/ntid/{ntid}",
            builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
            .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<string>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据员工ntid获取上级领导
        /// </summary>
        /// <param name="ntid">ntid</param>
        /// <returns>上级信息</returns>
        public async Task<JabusEmployeeInfo> GetManagerAsync(string ntid)
        {
            var resultStr = await httpRemoteService.GetAsStringAsync($"{authHostUrl}/api/Employee/master/{ntid}",
            builder => builder.SetContent(new { PageIndex = 1, PageSize = 1 }, "application/json;charset=utf-8")
            .AddAuthentication("Bearer", GetToken()));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var data = resultStr.To<Result<JabusEmployeeInfo>>();
                return data.Data;
            }
            else
            {
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return null;
        }

        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="dept">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<JabusEmployeeInfo> GetEmployeeDeptManagerAsync(string ntid)
        {
            if (ntid.IsNullOrWhiteSpace()) return null;
            var userInfo = await GetEmployeeByNtIdOrEmailAsync(ntid);
            return await GetDeptManagerAsync(userInfo?.DepartmentName);
        }


        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="dept">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<JabusEmployeeInfo> GetDeptManagerAsync(string dept)
        {
            if (dept.IsNullOrWhiteSpace()) return null;
            var deptManager = await App.GetService<EmployeeProxyService>().GetDeptManagerAsync(dept);
            if (deptManager == null) return null;
            return await Get(deptManager.manager_w_d_i_d);
        }


        public async Task SendEmployeeApiErrorNotice(IResult data)
        {
            try
            {
                await this._emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new List<string> { "Yang_Li9954@jabil.com" },
                    Subject = "Get DataAsset Employee Api Error Notice",
                    Html = $"Get DataAsset Employee Api Error: {data.ToJSON()}",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Send Employee Api Error Notice Error: " + ex.Message, ex.StackTrace);
            }
        }
    }
}
