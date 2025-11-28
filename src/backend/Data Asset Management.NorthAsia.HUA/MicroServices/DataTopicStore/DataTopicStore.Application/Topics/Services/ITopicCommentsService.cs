using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicCommentsService
    {
        Task<bool> SubmitAsync(TopicCommentsSubmitDto input);
        Task<bool> SetLikeOrDislikeAsync(TopicCommentsLikeOrDislikeDto input);
        Task<bool> CheckIsExitsAsync(Guid id);
        Task<PagedResultDto<TopicCommentListItemDto>> GetPagingListAsync(SearchTopicCommentsDto dto);
    }
}
