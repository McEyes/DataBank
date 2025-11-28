using Abp.Application.Services;

using ITPortal.Search.Application.Search.Dtos;

using Jabil.Service.Extension.Customs.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.Search
{
    public interface IUserSearchHistoryAppService : IApplicationService
    {

        Task<CustomeListResultDto<UserSearchHistoryDto>> GetHistoryListAsync();

        Task<ResponseResult> DeleteAsync(Guid id);

    }
}
