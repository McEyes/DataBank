using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class TopicDocumentPageResultDto : PageResult<TopicDocumentDto>
    {

        public TopicDocumentPageResultDto()
        {

        }

        public TopicDocumentPageResultDto(int totalCount, List<TopicDocumentDto> items)
            : base(totalCount, items)
        {

        }

        public List<SearchTopicDto> Topics { get; set; } = new();

    }
}
