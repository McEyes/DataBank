using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Dtos;

using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services
{
    public interface IEmployeeBaseInfoService : IBaseService<EmployeeBaseInfo, EmployeeBaseInfoDto, string>
    {
        Task<List<JabusEmployeeInfo>> GetAllEmployee(bool clearCache = false);
        Task<JabusEmployeeInfo> GetEmployeeInfo(string WorkNTID);
        Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string WorkNTIDs);
        Task<List<JabusEmployeeInfo>> GetEmployeeListAsync(string[] ntids);
        Task<JabusEmployeeInfo> GetManagerAsync(string ntid);
        Task<string> GetManagerNtIdAsync(string ntid);
        Task<Result<List<UserInfo>>> GetUsersAsync(bool clearCache = false);

        //Task<IResult<string>> ImportData(List<EmployeeBaseInfo> list);
        Task<List<JabusEmployeeInfo>> QueryEmployee(EmployeeQueryDto filter);
        Task<List<JabusEmployeeInfo>> QueryPageEmployee(EmployeeQueryDto filter);
    }
}
