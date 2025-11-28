using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicPermissionService
    {
        Task<bool> ApplyAsync(TopicPermissionApplyDto input);
        Task<bool> ApproveAsync(TopicPermissionApproveDto input);
        Task<PagedResultDto<PermissionApprovalItemDto>> GetApprovalPagingListAsync(SearchApprovalPagingDto input);
    }
}
