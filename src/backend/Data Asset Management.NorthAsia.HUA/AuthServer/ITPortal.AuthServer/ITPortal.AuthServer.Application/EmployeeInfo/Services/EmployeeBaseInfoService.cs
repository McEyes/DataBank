using Furion.LinqBuilder;

using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Dtos;
using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using Npgsql;

using System.Security.Claims;

namespace ITPortal.AuthServer.Application.EmployeeInfos.Services
{
    public class EmployeeBaseInfoService : BaseService<EmployeeBaseInfo, EmployeeBaseInfoDto, string>, IEmployeeBaseInfoService, ITransient
    {
        //private readonly EmployeeProxyService _employeeProxyService; EmployeeProxyService employeeProxyService,
        private readonly ILogger _logger;
        public EmployeeBaseInfoService(ISqlSugarClient db,ILogger<EmployeeBaseInfoService> logger,
            IDistributedCacheService cache) : base(db, cache, false)
        {
            _logger = logger;
            //_employeeProxyService = employeeProxyService;
        }

        public override ISugarQueryable<EmployeeBaseInfo> BuildFilterQuery(EmployeeBaseInfoDto filter)
        {
            filter.Keyword = filter.Keyword?.ToLower();
            filter.Work_email = filter.Work_email?.ToLower();
            filter.Ntid = filter.Ntid?.ToLower();
            var Query = CurrentDb.Queryable<EmployeeBaseInfo>()
              .WhereIF(filter.Ntid.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Ntid) == filter.Ntid)
                .WhereIF(filter.employee_workcell.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.employee_workcell) == filter.employee_workcell)
                .WhereIF(filter.company_code.IsNotNullOrWhiteSpace(), f => f.company_code == filter.company_code)
                .WhereIF(filter.Employee_first_name.IsNotNullOrWhiteSpace(), f => f.Employee_first_name == filter.Employee_first_name)
                .WhereIF(filter.Employee_last_name.IsNotNullOrWhiteSpace(), f => f.Employee_last_name == filter.Employee_last_name)
                .WhereIF(filter.Employee_chi_name.IsNotNullOrWhiteSpace(), f => f.Employee_chi_name.Contains(filter.Employee_chi_name))
                .WhereIF(filter.employee_id.IsNotNullOrWhiteSpace(), f => f.employee_id == filter.employee_id)
                .WhereIF(filter.employee_nationality.IsNotNullOrWhiteSpace(), f => f.employee_nationality == filter.employee_nationality)
                .WhereIF(filter.job_category.IsNotNullOrWhiteSpace(), f => f.job_category == filter.job_category)
                .WhereIF(filter.job_family_group.IsNotNullOrWhiteSpace(), f => f.job_family_group == filter.job_family_group)
                .WhereIF(filter.business_title.IsNotNullOrWhiteSpace(), f => f.business_title == filter.business_title)
                .WhereIF(filter.company_location.IsNotNullOrWhiteSpace(), f => f.company_location == filter.company_location)
                .WhereIF(filter.Department_name.IsNotNullOrWhiteSpace(), f => f.Department_name == filter.Department_name)
                .WhereIF(filter.direct_manager.IsNotNullOrWhiteSpace(), f => f.direct_manager == filter.direct_manager)
                .WhereIF(filter.direct_manager_ntid.IsNotNullOrWhiteSpace(), f => f.direct_manager_ntid == filter.direct_manager_ntid)
                .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Ntid) == filter.Keyword || SqlFunc.ToLower(f.Work_email).Contains(filter.Keyword) || SqlFunc.ToLower(f.Employee_first_name).Contains(filter.Keyword) || SqlFunc.ToLower(f.Employee_last_name).Contains(filter.Keyword))
                .WhereIF(filter.business_title.IsNotNullOrWhiteSpace(), f => f.business_title == filter.business_title);
            return Query;
        }


        public async Task<List<EmployeeBaseInfo>> GetAllEmployeeBaseInfo(bool clearCache = false)
        {
            var fun = async () =>
            {
                return await CurrentDb.Queryable<EmployeeBaseInfo>().Where(f => f.Ntid != null).ToListAsync();
            };
            if (NoRedis) return await fun();
            //if (clearCache) await _cache.RemoveAsync(GetRedisKey<EmployeeBaseInfo>("All"));
            return await _cache.GetObjectAsync(GetRedisKey<EmployeeBaseInfo>("All"), fun, TimeSpan.FromSeconds(10),clearCache);
        }

        public async Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false)
        {
            var list = await GetAllEmployeeBaseInfo(clearCache);
            return list.Adapt<List<JabusEmployeeInfo>>();
        }

        public async Task<List<JabusEmployeeInfo>> QueryEmployee(EmployeeQueryDto filter)
        {
            var query = CurrentDb.Queryable<EmployeeBaseInfo>().WhereIF(filter.WorkNTID.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Ntid) == filter.WorkNTID.ToLower())
                .WhereIF(filter.Workcell.IsNotNullOrWhiteSpace(), f => f.employee_workcell == filter.Workcell)
                .WhereIF(filter.CompanyCode.IsNotNullOrWhiteSpace(), f => f.company_code == filter.CompanyCode)
                .WhereIF(filter.Country.IsNotNullOrWhiteSpace(), f => f.employee_nationality == filter.Country)
                .WhereIF(filter.EnglishName.IsNotNullOrWhiteSpace(), f => filter.EnglishName.Contains(f.Employee_first_name) || filter.EnglishName.Contains(f.Employee_last_name))
                .WhereIF(filter.DepartmentName.IsNotNullOrWhiteSpace(), f => f.Department_name == filter.DepartmentName)
                .WhereIF(filter.BusinessTitleLocal.IsNotNullOrWhiteSpace(), f => f.company_location.Contains(filter.BusinessTitleLocal))
                .WhereIF(filter.EmployeeCode.IsNotNullOrWhiteSpace(), f => f.company_code == filter.EmployeeCode)
                .WhereIF(filter.JobFamily.IsNotNullOrWhiteSpace(), f => f.job_classification == filter.JobFamily)
                .WhereIF(filter.JobFamilyGroup.IsNotNullOrWhiteSpace(), f => f.job_family_group == filter.JobFamilyGroup)
                .WhereIF(filter.JobTitle.IsNotNullOrWhiteSpace(), f => f.business_title == filter.JobTitle)
                .WhereIF(filter.Location.IsNotNullOrWhiteSpace(), f => f.company_location == filter.Location)
                .WhereIF(filter.ManagementDivision.IsNotNullOrWhiteSpace(), f => f.direct_manager_wdid == filter.ManagementDivision)
                .WhereIF(filter.ManagerNTID.IsNotNullOrWhiteSpace(), f => f.direct_manager_ntid == filter.ManagerNTID)
                .WhereIF(filter.Keyword.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.Ntid) == filter.Keyword || SqlFunc.ToLower(f.Work_email).Contains(filter.Keyword) || SqlFunc.ToLower(f.Employee_first_name).Contains(filter.Keyword) || SqlFunc.ToLower(f.Employee_last_name).Contains(filter.Keyword))
                .WhereIF(filter.WorkEmail.IsNotNullOrWhiteSpace(), f => f.Work_email == filter.WorkEmail);
            var list = await query.ToListAsync();
            return list.Adapt<List<JabusEmployeeInfo>>();
        }


        public async Task<List<JabusEmployeeInfo>> QueryPageEmployee(EmployeeQueryDto filter)
        {
            List<JabusEmployeeInfo> list = await QueryEmployee(filter);
            return list.Skip(filter.SkipCount).Take(filter.PageSize).ToList();
        }

        public async Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID)
        {
            WorkNTID = WorkNTID?.ToLower();
            var Query = CurrentDb.Queryable<EmployeeBaseInfo>()
              .Where(f => SqlFunc.ToLower(f.Ntid) == WorkNTID || SqlFunc.ToLower(f.Work_email) == WorkNTID);
            return (await Query.FirstAsync()).Adapt<JabusEmployeeInfo>();
        }

        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="WorkNTIDs">ntid;,;</param>
        /// <returns></returns>
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string WorkNTIDs)
        {
            if (WorkNTIDs.IsNullOrWhiteSpace()) return new List<JabusEmployeeInfo>();
            var ntids = WorkNTIDs.ToLower().Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var Query = CurrentDb.Queryable<EmployeeBaseInfo>()
              .Where(f => ntids.Contains(SqlFunc.ToLower(f.Ntid)) || ntids.Contains(SqlFunc.ToLower(f.Work_email)));
            return (await Query.ToListAsync()).Adapt<List<JabusEmployeeInfo>>();
        }

        public async Task<List<string>> GetAllDepartments()
        {
            var query = CurrentDb.Queryable<EmployeeBaseInfo>().Where(t => !string.IsNullOrWhiteSpace(t.Department_name));
            return await query.Select(t => t.Department_name).Distinct().ToListAsync();
        }

        /// <summary>
        /// 根据员工ntid获取详细信息
        /// 实际访问：/userapi/Employee/info接口
        /// </summary>
        /// <param name="ntids">ntid</param>
        /// <returns></returns>
        //[HttpGet("http://10.114.19.82:44036/userapi/Employee/info")]
        public async Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string[] ntids)
        {
            for (int i = 0; i < ntids.Length; i++)
            {
                if (ntids.IsNullOrEmpty()) continue;
                ntids[i] = ntids[i].ToLower().Trim();
            }
            var Query = CurrentDb.Queryable<EmployeeBaseInfo>()
              .Where(f => ntids.Contains(SqlFunc.ToLower(f.Ntid)) || ntids.Contains(SqlFunc.ToLower(f.Work_email)));
            return (await Query.ToListAsync()).Adapt<List<JabusEmployeeInfo>>();
        }

        /// <summary>
        /// 根据员工ntid获取上级领导ntid
        /// </summary>
        /// <param name="ntid">ntid</param>
        /// <returns>上级etid</returns>
        public async Task<string> GetManagerNtIdAsync(string ntid)
        {
            var empInfo = await GetEmployeeInfo(ntid);
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
            var empInfo = await GetEmployeeInfo(ntid);
            if (empInfo != null && empInfo.WorkNTID.IsNotNullOrWhiteSpace())
            {
                var mamagerInfo = await GetEmployeeInfo(empInfo.ManagerNTID);
                if (mamagerInfo != null && mamagerInfo.WorkNTID.IsNotNullOrWhiteSpace()) return mamagerInfo;
            }
            return null;
        }

        /// <summary>
        /// 根据员工所在部门部门主管信息
        /// </summary>
        /// <param name="ntid">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<JabusEmployeeInfo> GetEmployeeDeptManagerAsync(string ntid)
        {
            if (ntid.IsNullOrWhiteSpace()) return null;
            var userInfo = await GetEmployeeInfo(ntid);
            return await GetDeptManagerAsync(userInfo?.DepartmentName);
        }


        /// <summary>
        /// 获取Cost Center Function Manager
        /// </summary>
        /// <param name="costCenterId">Cost Center ID，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<JabusEmployeeInfo> GetFunctionManagerAsync(string costCenterId)
        {
            if (costCenterId.IsNullOrWhiteSpace()) return null;
            costCenterId = costCenterId.Replace("CC_","",StringComparison.CurrentCultureIgnoreCase);
            var deptManager = await App.GetService<EmployeeProxyService>().GetCostCenterAsync(costCenterId);
            if (deptManager == null) return null;
            return (await Get(deptManager.plant_fm_wd_id)).Adapt<JabusEmployeeInfo>();
        }


        /// <summary>
        /// 获取员工Function Manager
        /// </summary>
        /// <param name="ntid">部门名称，区分大小写</param>
        /// <returns>部门主管wdid</returns>
        public async Task<JabusEmployeeInfo> GetEmployeeFMAsync(string ntid)
        {
            if (ntid.IsNullOrWhiteSpace()) return null;
            var userInfo = await GetEmployeeInfo(ntid);
            return await GetFunctionManagerAsync(userInfo?.CostCenterId);
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
            return (await Get(deptManager.manager_w_d_i_d)).Adapt<JabusEmployeeInfo>();
        }


        public async Task<IResult<string>> ImportData(List<EmployeeBaseInfo> list)
        {
            var result = new Result<string>();
            var oldList = await GetAllEmployeeBaseInfo(true);
            var storage = await CurrentDb.Storageable(list).ToStorageAsync();
            await RetryOnDeadlock(async () =>
            {
                await storage.AsInsertable.ExecuteCommandAsync();
            });
            await RetryOnDeadlock(async () =>
            {
                await storage.AsUpdateable.ExecuteCommandAsync();
            });
            //foreach (var item in list)
            //{
            //    try
            //    {
            //        if (oldList.Any(f => f.employee_id == item.employee_id))
            //        {
            //            await CurrentDb.Storageable(item).ExecuteCommandAsync();
            //        }
            //        else
            //        {
            //            await CurrentDb.Insertable(item).ExecuteCommandAsync();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, $"{item.Ntid} Update Error:{ex.Message}");
            //        result.SetError($"{item.Ntid} Update Error:{ex.Message}");
            //    }
            //}
            return result;
        }
        private async Task RetryOnDeadlock(Func<Task> action, int maxRetries = 3)
        {
            int retries = 0;
            while (retries < maxRetries)
            {
                try
                {
                    await action();
                    return;
                }
                catch (PostgresException ex) when (ex.SqlState == "40P01") // 死锁异常
                {
                    retries++;
                    if (retries >= maxRetries)
                        throw;
                    // 短暂延迟后重试（避免立即重试再次冲突）
                    await Task.Delay(500 * retries);
                }
            }
        }

    }
}
