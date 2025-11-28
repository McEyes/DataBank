using ITPortal.Search.Application.SearchTopic.Dtos;

using Jabil.Service.Extension.Customs.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace ITPortal.Search.Application.Search
{
    public interface ISearchTopicAppService : IApplicationService
    {

        Task<ResponseResult<SearchTopicDto>> Create(SearchTopicDto topic);

        Task<ResponseResult<SearchTopicDto>> Update(SearchTopicDto topic);

        Task<ResponseResult> Delete(Guid id);

        Task<PageResult<SearchTopicDto>> GetListByPage(string keyword, int pageIndex, int pageSize);

    }
}
