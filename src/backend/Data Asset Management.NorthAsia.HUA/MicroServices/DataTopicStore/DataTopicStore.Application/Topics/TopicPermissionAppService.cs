
using System.Net;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;
using Newtonsoft.Json;

namespace DataTopicStore.Application.Topics
{
    [AppAuthorize]
    public class TopicPermissionAppService : IDynamicApiController
    {
        private readonly ITopicPermissionService topicPermissionService;
        public TopicPermissionAppService(ITopicPermissionService topicPermissionService)
        {
            this.topicPermissionService = topicPermissionService;
        }

        [HttpPost]
        public Task<bool> ApplyAsync(TopicPermissionApplyDto input) => topicPermissionService.ApplyAsync(input);

        [HttpPost]
        public Task<bool> ApproveAsync(TopicPermissionApproveDto input) => topicPermissionService.ApproveAsync(input);

        [HttpPost]
        public Task<PagedResultDto<PermissionApprovalItemDto>> GetApprovalPagingListAsync(SearchApprovalPagingDto input) => topicPermissionService.GetApprovalPagingListAsync(input);

    }
}
