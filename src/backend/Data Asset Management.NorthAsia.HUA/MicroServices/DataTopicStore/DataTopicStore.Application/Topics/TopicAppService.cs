
using System.Net;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;
using Newtonsoft.Json;

namespace DataTopicStore.Application.Topics
{
    [AppAuthorize]
    [Route("/api/topic")]
    public class TopicAppService : IDynamicApiController
    {
        private readonly ITopicService topicService;
        private readonly ITopicLikesService topicLikesService;
        private readonly ITopicRatingsService topicRatingsService;
        private readonly ITopicCommentsService topicCommentsService;
        private readonly ITopicFavoritesService topicFavoritesService;
        public TopicAppService(
            ITopicService topicService,
            ITopicLikesService topicLikesService,
            ITopicRatingsService topicRatingsService,
            ITopicCommentsService topicCommentsService,
            ITopicFavoritesService topicFavoritesService)
        {
            this.topicService = topicService;
            this.topicLikesService = topicLikesService;
            this.topicRatingsService = topicRatingsService;
            this.topicCommentsService = topicCommentsService;
            this.topicFavoritesService = topicFavoritesService;
        }

        [HttpGet("{id}")]
        public Task<object> GetResultAsync(long id) => topicService.GetResultAsync(id);

        [HttpGet("preview/{id}")]
        public Task<object> GetPreviewResultAsync(long id) => topicService.GetPreviewResultAsync(id);

        [HttpGet]
        public Task<PagedResultDto<TopicListItemDto>> GetPagingListAsync([FromQuery] SearchTopicDto input) => topicService.GetPagingListAsync(input);

        [HttpGet("ranking")]
        public Task<List<TopicListItemDto>> GetRankingListAsync([FromQuery] RankingDto input) => topicService.GetRankingListAsync(input);

        [HttpGet("details/{id}")]
        public Task<TopicDetailsDto> GetDetailsAsync(long id) => topicService.GetDetailsAsync(id);

        [HttpPost("like")]
        public Task<bool> SubmitAsync(TopicLikeOrDislikeDto input) => topicLikesService.SubmitAsync(input);

        [HttpPost("rating")]
        public Task<bool> RatingAsync(TopicRatingsSubmitDto input) => topicRatingsService.SubmitAsync(input);

        [HttpPost("comment")]
        public Task<bool> CommentAsync(TopicCommentsSubmitDto input) => topicCommentsService.SubmitAsync(input);

        [HttpPost("favorite")]
        public Task<bool> FavoriteAsync(TopicFavoritesSubmitDto input) => topicFavoritesService.SubmitAsync(input);

        [HttpGet("my-favorite-topics")]
        public Task<List<FavoriteTopicListItemDto>> MyFavoriteAsync() => topicFavoritesService.GetMyFavoriteTopicsAsync();

        [HttpPost("newdatatopic")]
        public Task<bool> NewDataTopicAsync(NewTopicDto input) => topicService.NewDataTopicAsync(input);

        [HttpGet("preview/{id}/export")]
        [ProducesResponseType(typeof(FileContentResult), (int)HttpStatusCode.OK)]
        public Task<ActionResult> ExportAsync(long id)
        {
            return topicService.GetPreviewDataToExcel(id);
        }
    }
}
