using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;

namespace DataTopicStore.Application.Topics
{
    [AppAuthorize]
    public class TopicCommentsAppService : IDynamicApiController
    {
        private readonly ITopicCommentsService topicCommentsService;
        private readonly ITopicCommentsLikesService topicCommentsLikesService;
        public TopicCommentsAppService(ITopicCommentsService topicCommentsService, ITopicCommentsLikesService topicCommentsLikesService)
        {
            this.topicCommentsService = topicCommentsService;
            this.topicCommentsLikesService = topicCommentsLikesService;
        }

        [HttpPost("like")]
        public Task<bool> SubmitAsync(TopicCommentsLikeOrDislikeDto input) => topicCommentsLikesService.SubmitAsync(input);

        [HttpGet("list")]
        public Task<PagedResultDto<TopicCommentListItemDto>> GetPagingListAsync([FromQuery]SearchTopicCommentsDto input) => topicCommentsService.GetPagingListAsync(input);
    }
}