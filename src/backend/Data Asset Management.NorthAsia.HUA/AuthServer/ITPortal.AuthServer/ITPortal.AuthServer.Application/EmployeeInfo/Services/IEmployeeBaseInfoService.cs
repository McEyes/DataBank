using ITPortal.AuthServer.Application.Account.Dtos;
using ITPortal.AuthServer.Application.EmployeeInfos.Dtos;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace ITPortal.AuthServer.Application.EmployeeInfos.Services
{
    public interface IEmployeeBaseInfoService : IBaseService<EmployeeBaseInfo, EmployeeBaseInfoDto, string>
    {
        Task<List<string>> GetAllDepartments();
        Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false);
        Task<JabusEmployeeInfo> GetDeptManagerAsync(string dept);
        Task<JabusEmployeeInfo> GetEmployeeDeptManagerAsync(string ntid);
        Task<JabusEmployeeInfo> GetEmployeeFMAsync(string ntid);
        Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID);
        Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string WorkNTIDs);
        Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string[] ntids);
        Task<JabusEmployeeInfo> GetFunctionManagerAsync(string costCenterId);
        Task<JabusEmployeeInfo> GetManagerAsync(string ntid);
        Task<string> GetManagerNtIdAsync(string ntid);
        Task<IResult<string>> ImportData(List<EmployeeBaseInfo> list);
        Task<List<JabusEmployeeInfo>> QueryEmployee(EmployeeQueryDto filter);
        Task<List<JabusEmployeeInfo>> QueryPageEmployee(EmployeeQueryDto filter);
    }
}
