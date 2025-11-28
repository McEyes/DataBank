using ITPortal.Search.Application.Search.Dtos;
using ITPortal.Search.Application.TopicDocument.Dtos;

using Jabil.Service.Extension.Customs.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace ITPortal.Search.Application.Search
{
    public interface IOpenSearchAppService : IApplicationService
    {

        Task<PageResult<TopicDocumentDto>> GetRecommendByTag(TagRecommendQueryDto query);

        Task<List<string>> GetCandidateWords(CandidateWordQueryDto query);

        Task<TopicDocumentPageResultDto> OpenSearch(TopicDocumentQueryDto query);

        Task<PageResult<GroupByTopicDocumentDto>> OpenSearchByTopicGroup(GroupByTopicDocumentQueryDto query);

    }
}
