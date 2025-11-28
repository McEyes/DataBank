using Furion;
using Furion.FriendlyException;
using Furion.HttpRemote;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Emails;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using SqlSugar;

using System.Drawing.Printing;

namespace ITPortal.Core.ProxyApi
{


    /// <summary>
    /// 员工数据获取代理接口
    /// 实现了对/userapi/Employee/info接口的数据获取
    /// </summary>
    //[BaseAddress("http://10.115.63.135:4200/gateway/")]
    public class EmployeeProxyService : BaseProxyService//: IHttpDeclarative
    {
        private readonly string DataAssetHostUrl;
        private readonly string DataAsset_XToken;
        private readonly IDistributedCacheService _cache;
        private readonly ILogger<EmployeeProxyService> _logger;
        private readonly IEmailSender _emailSender;
        public EmployeeProxyService(IHttpRemoteService httpRemoteService, IDistributedCacheService cache, IEmailSender emailSender) : base(httpRemoteService)
        {
            //this.httpRemoteService = httpRemoteService;
            _cache = cache;
            DataAssetHostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
            DataAsset_XToken = App.GetConfig<string>("RemoteApi:DataAsset_XToken") ?? "77373d7e-ec37-4c61-9071-3c2d58966133";
            _emailSender = emailSender;
            _logger =App.GetService<ILogger<EmployeeProxyService>>();
        }

        /// <summary>
        /// 获取所有userInfo清单
        /// 实际请求接口：/gateway/homeapi/api/home/Common/GetUserToEmployeeModList
        /// http://cnhuam0itpoc81:4200/gateway/eureka/data/api/services/v1.0.0/HR/Base/tp_employee_base_info/query
        /// "x-token", "3b89b6aaa232a39461eb704f53656f74"
        /// </summary>
        /// <returns></returns>
        //[HttpGet("http://10.115.63.135:4200/gateway/homeapi/api/home/Common/GetUserToEmployeeModList")]
        public async Task<IResult> AllMasterEmployeeAsync()
        {
            //Result<PageResult<MasterEmployeeInfo>>
            var filter = new PageEntity<string>() { PageNum = 1, PageSize = 20000 };
            var result = await GetPageMasterEmployeeAsync(filter);
            Result<PageResult<MasterEmployeeInfo>>? ListResult = new Result<PageResult<MasterEmployeeInfo>>();
            while (result.Success)
            {
                var list = result as Result<PageResult<MasterEmployeeInfo>>;
                if (ListResult.Data == null || ListResult.Data.Total == 0)
                    ListResult = list;
                else
                    ListResult.Data.Data.AddRange(list.Data.Data);
                if (ListResult.Data.Total == ListResult.Data.Data.Count)
                {
                    return ListResult;
                }
                else if (ListResult.Data.Total > ListResult.Data.Data.Count && ListResult.Data.Data.Count > 0)
                {
                    filter.PageNum++;
                    result = await GetPageMasterEmployeeAsync(filter);
                }
            }
            return ListResult;
        }

        private async Task<IResult> GetPageMasterEmployeeAsync(PageEntity<string> filter)
        {
            //Result<PageResult<MasterEmployeeInfo>>

            var resultStr = await httpRemoteService.GetAsStringAsync($"{DataAssetHostUrl}/services/v1.0.0/HR/Base/tp_employee_base_info/query",
            builder => builder.SetContent(filter, "application/json;charset=utf-8")
            //.AddAuthentication("Bearer", GetDataAssetApiToken("DataAsset_Services"))
            .WithHeader("x-token", DataAsset_XToken));
            IResult result = resultStr.To<Result>() ?? new Result().SetError($"请求失败，返回结果为:{resultStr}");
            if (result.Success)
            {
                return resultStr.To<Result<PageResult<MasterEmployeeInfo>>>();
            }
            else
            {
                await SendEmployeeApiErrorNotice(result);
                //邮件通知
                _logger.LogError(result.Msg.ToString());
            }
            return result;
        }


        /// <summary>
        /// 获取所有userInfo清单
        /// 实际请求接口：/gateway/homeapi/api/home/Common/GetUserToEmployeeModList
        /// http://cnhuam0itpoc81:4200/gateway/eureka/data/api/services/v1.0.0/HR/Base/tp_employee_base_info/query
        /// "x-token", "3b89b6aaa232a39461eb704f53656f74"
        /// </summary>
        /// <returns></returns>
        //[HttpGet("http://10.115.63.135:4200/gateway/homeapi/api/home/Common/GetUserToEmployeeModList")]
        public async Task<List<JabusEmployeeInfo>> AllEmployeeAsync(bool clearCache = false)
        {
            var cacheexpiry = 1;
            var fun = async () =>
            {
                var data = new Result<List<JabusEmployeeInfo>>();
                var result = await AllMasterEmployeeAsync();
                if (result.Success)
                {
                    var dataResult = result as Result<PageResult<MasterEmployeeInfo>>;
                    if (dataResult != null)
                    {
                        data.Data = dataResult.Data.Data.Select(f => new JabusEmployeeInfo()
                        {
                            Workcell = f.employee_workcell,
                            WorkNTID = (f.Ntid + "").ToLower(),
                            WorkEmail = f.Work_email,
                            EmployeeCode = f.employee_id,
                            Name = $"{(f.Work_email.IsNotNullOrWhiteSpace() ? f.Work_email.Replace("@jabil.com", "", StringComparison.InvariantCultureIgnoreCase).Replace("_", " ") : (f.Employee_first_name + " " + f.Employee_last_name))}({(f.Ntid ?? "")})",
                            ChineseName = f.Employee_chi_name,
                            EnglishName = f.Employee_first_name + " " + f.Employee_last_name,
                            JobTitle = f.global_job_title,
                            BusinessTitleLocal = f.business_title,
                            DepartmentName = f.Department_name,
                            JobFamily = f.job_classification,
                            JobFamilyGroup = f.job_family_group,
                            Location = f.company_location,
                            Country = f.employee_nationality,
                            //Region= f.
                            PlantDivision = f.employee_workcell,
                            //ManagementDivision=
                            //LegalEntity = f.le
                            CompanyCode = f.company_code,
                            ManagerNTID = f.direct_manager_ntid,
                            ManagerEmail = f.direct_manager_email,
                            ManagerWdid = f.direct_manager_wdid,
                            ManagementDivision = f.direct_manager_wdid,
                            //Sector=f.ce
                        }).ToList();
                    }
                    else
                    {
                        cacheexpiry = 0;
                        data.Success = false;
                        data.Msg = result.Msg;
                        _logger.LogError(result.Msg.ToString());

                    }
                }
                else
                {
                    cacheexpiry = 0;
                    data.Success = false;
                    data.Msg = result.Msg;
                    _logger.LogError(result.Msg.ToString());

                }
                return data.Data;
            };

            return await fun();
            if (clearCache)
            {
                var data = await fun();
                if (data != null && data.Count() > 9000)
                    _cache.SetObject(DataAssetManagerConst.API_tp_employee_base_infoAllKey, data, TimeSpan.FromDays(cacheexpiry));
                return data;
            }
            var list = await _cache.GetObjectAsync(DataAssetManagerConst.API_tp_employee_base_infoAllKey, fun, TimeSpan.FromDays(cacheexpiry));
            if (list == null || list.Count() < 9000)
                _cache.SetObject(DataAssetManagerConst.API_tp_employee_base_infoAllKey, list, TimeSpan.FromSeconds(1));
            return list;
        }

        /// <summary>
        /// 获取所有userInfo清单
        /// 实际请求接口：/gateway/homeapi/api/home/Common/GetUserToEmployeeModList
        /// http://cnhuam0itpoc81:4200/gateway/eureka/data/api/services/v1.0.0/HR/Base/tp_employee_base_info/query
        /// "x-token", "3b89b6aaa232a39461eb704f53656f74"
        /// </summary>
        /// <returns></returns>
        //[HttpGet("http://10.115.63.135:4200/gateway/homeapi/api/home/Common/GetUserToEmployeeModList")]
        public async Task<Result<List<UserInfo>>?> GetUsersAsync(bool clearCache = false)
        {
            var allMember = await AllEmployeeAsync(clearCache) ?? new List<JabusEmployeeInfo>();
            var data = new Result<List<UserInfo>>();
            data.Data = allMember.Select(f => new UserInfo()
            {
                Id = f.WorkNTID,
                UserId = f.WorkNTID,
                Surname = f.WorkNTID,
                UserName = f.WorkNTID,
                NtId = f.WorkNTID,
                Name = f.Name,
                EnglishName = f.EnglishName,
                ChineseName = f.ChineseName,
                Email = f.WorkEmail,
                Department = f.DepartmentName,
            }).ToList();
            return data;
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
            if (WorkNTID.IsNullOrWhiteSpace()) return null;
            var allMember = await AllEmployeeAsync() ?? new List<JabusEmployeeInfo>();
            return allMember.FirstOrDefault(f => (f.WorkNTID != null && f.WorkNTID.Equals(WorkNTID, StringComparison.CurrentCultureIgnoreCase)) || (f.WorkEmail != null && f.WorkEmail.Equals(WorkNTID, StringComparison.CurrentCultureIgnoreCase))) ?? new JabusEmployeeInfo();
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
            var allMember = await AllEmployeeAsync() ?? new List<JabusEmployeeInfo>();
            return allMember.FirstOrDefault(f => (f.WorkNTID != null && f.WorkNTID.Equals(ntidOrEmail, StringComparison.CurrentCultureIgnoreCase)) || (f.WorkEmail != null && f.WorkEmail.Equals(ntidOrEmail, StringComparison.CurrentCultureIgnoreCase))) ?? new JabusEmployeeInfo();
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
            if (WorkNTIDs.IsNullOrWhiteSpace()) return new List<JabusEmployeeInfo>();
            var allMember = await AllEmployeeAsync() ?? new List<JabusEmployeeInfo>();
            var ntids = WorkNTIDs.ToLower().Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return allMember.Where(f => f.WorkNTID != null && ntids.Contains(f.WorkNTID.ToLower())).ToList();
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
            var result = new Result<JabusEmployeeInfo>();
            var allMember = await AllEmployeeAsync() ?? new List<JabusEmployeeInfo>();
            return allMember.Where(f => f.WorkNTID != null && ntids.Contains(f.WorkNTID.ToLower())).ToList();
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
            var empInfo = await GetEmployeeAsync(ntid);
            if (empInfo != null && empInfo.WorkNTID.IsNotNullOrWhiteSpace()) return empInfo.ManagerNTID;
            return string.Empty;
        }

        /// <summary>
        /// 根据员工ntid获取上级领导
        /// </summary>
        /// <param name="ntid">ntid</param>
        /// <returns>上级信息</returns>
        public async Task<JabusEmployeeInfo> GetManagerAsync(string ntid)
        {
            var empInfo = await GetEmployeeAsync(ntid);
            if (empInfo != null && empInfo.WorkNTID.IsNotNullOrWhiteSpace())
            {
                var mamagerInfo = await GetEmployeeAsync(empInfo.ManagerNTID);
                if (mamagerInfo != null && mamagerInfo.WorkNTID.IsNotNullOrWhiteSpace()) return mamagerInfo;
            }
            return null;
        }

        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="dept">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<DeptManagerInfo> GetDeptManagerAsync(string dept)
        {
            PageEntity<string> filter = new PageEntity<string>() { PageNum = 1, PageSize = 1000 };
            var resultStr = await httpRemoteService.GetAsStringAsync($"{DataAssetHostUrl}/services/v1.0.0/Trace/dataxaccess/s_function/query?name={dept}",
            builder => builder.SetContent(filter, "application/json;charset=utf-8")
            //.AddAuthentication("Bearer", GetDataAssetApiToken("DataAsset_Services"))
            .WithHeader("x-token", DataAsset_XToken));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var resultData = resultStr.To<Result<PageResult<DeptManagerInfo>>>();
                if (resultData.Data != null && resultData.Data.Total > 0)
                    return resultData.Data.Data.First();
            }
            else
            {
                _logger.LogError(result.Msg.ToString());
                //throw new AppFriendlyException(result.Msg.ToString(), result.Code);
            }
            return null;
        }

        /// <summary>
        /// 根据员工所在成本中心，获取主管信息
        /// </summary>
        /// <param name="costCenter">成本中心</param>
        /// <returns>部门主管wdid</returns>
        public async Task<CostCenterInfo> GetCostCenterAsync(string costCenter)
        {
            PageEntity<string> filter = new PageEntity<string>() { PageNum = 1, PageSize = 1000 };
            var resultStr = await httpRemoteService.GetAsStringAsync($"{DataAssetHostUrl}/services/v1.0.0/Trace/dataxaccess/s_cost_center/query?cost_center_id={costCenter}",
            builder => builder.SetContent(filter, "application/json;charset=utf-8")
            //.AddAuthentication("Bearer", GetDataAssetApiToken("DataAsset_Services"))
            .WithHeader("x-token", DataAsset_XToken));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var resultData = resultStr.To<Result<PageResult<CostCenterInfo>>>();
                if (resultData.Data != null && resultData.Data.Total > 0)
                    return resultData.Data.Data.FirstOrDefault();
            }
            else
            {
                _logger.LogError(result.Msg.ToString());
                //throw new AppFriendlyException(result.Msg.ToString(), result.Code);
            }
            return null;
        }

        /// <summary>
        /// 根据员工所在成本中心，获取主管信息
        /// </summary>
        /// <param name="costCenter">成本中心</param>
        /// <returns>部门主管wdid</returns>
        public async Task<List<CostCenterInfo>> GetCostCenterAllAsync(string costCenter)
        {
            PageEntity<string> filter = new PageEntity<string>() { PageNum = 1, PageSize = 10000 };
            var resultStr = await httpRemoteService.GetAsStringAsync($"{DataAssetHostUrl}/services/v1.0.0/Trace/dataxaccess/s_cost_center/query",
            builder => builder.SetContent(filter, "application/json;charset=utf-8")
            //.AddAuthentication("Bearer", GetDataAssetApiToken("DataAsset_Services"))
            .WithHeader("x-token", DataAsset_XToken));
            IResult result = resultStr.To<Result>();
            if (result.Success)
            {
                var resultData = resultStr.To<Result<PageResult<CostCenterInfo>>>();
                if (resultData.Data != null && resultData.Data.Total > 0)
                    return resultData.Data.Data;
            }
            else
            {
                _logger.LogError(result.Msg.ToString());
                //throw new AppFriendlyException(result.Msg.ToString(), result.Code);
            }
            return new List<CostCenterInfo>();
        }

        public async Task SendEmployeeApiErrorNotice(IResult data)
        {
            try
            {
                await this._emailSender.SendAsync(new EmailMessage()
                {
                    MailTo = new List<string> { "Yang_Li9954@jabil.com" },
                    Subject = "Get Employee Api Error Notice",
                    Html = $"Get Employee Api Error: {data.ToJSON()}",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Send Employee Api Error Notice Error: " + ex.Message, ex.StackTrace);
            }
        }
    }
}
