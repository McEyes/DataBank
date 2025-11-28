using ITPortal.Core.Services;
using ITPortal.Search.Application.OpenSearch.Dtos;
using ITPortal.Search.Application.TopicDocument.Dtos;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ITPortal.Search.Application.OpenSearch.Services
{
    public interface IOpenSearchAppService
    {

        Task<PageResult<TopicDocumentDto>> GetRecommendByTag(TagRecommendQueryDto query);

        Task<List<string>> GetCandidateWords(CandidateWordQueryDto query);

        Task<PageResult<TopicDocumentDto>> OpenSearch(TopicDocumentQueryDto query);

        Task<PageResult<GroupByTopicDocumentDto>> OpenSearchByTopicGroup(GroupByTopicDocumentQueryDto query);

    }
}
