using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicService
    {
        Task<object> GetResultAsync(long id);
        Task<object> GetPreviewResultAsync(long id);
        Task<PagedResultDto<TopicListItemDto>> GetPagingListAsync(SearchTopicDto input);
        Task<TopicDetailsDto> GetDetailsAsync(long id);
        Task<bool> UpdateRatingsAsync(UpdateTopicRatingsDto input);
        Task<bool> NewDataTopicAsync(NewTopicDto input);
        List<SwaggerTopicApiParameters> GetSwaggerTopicApiParameters();
        Task<bool> CheckIsExitsAsync(long id);
        Task<List<TopicListItemDto>> GetRankingListAsync(RankingDto input);
        Task<ActionResult> GetPreviewDataToExcel(long id);
    }
}
