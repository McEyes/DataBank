
using System.Net;
using DataTopicStore.Application.ApproveFlow.Dtos;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;
using Newtonsoft.Json;

namespace DataTopicStore.Application.Topics
{
    [AppAuthorize]
    public class TopicMgrAppService : IDynamicApiController
    {
        private readonly ITopicMgrService topicMgrService;
        public TopicMgrAppService(ITopicMgrService topicMgrService)
        {
            this.topicMgrService = topicMgrService;
        }

        [HttpPost]
        public Task<bool> SaveAsync(CreateOrUpdateTopicDto input) => topicMgrService.SaveAsync(input);

        [HttpPost]
        public Task<bool> SetParametersInputAsync(SetParametersInputDto input) => topicMgrService.SetParametersInputAsync(input);

        [HttpPost]
        public Task<bool> SetParametersOutputAsync(SetParametersOutputDto input) => topicMgrService.SetParametersOutputAsync(input);

        //[HttpPost]
        //public Task<bool> SetVerificationPassedAsync(SetVerificationPassedDto input) => topicMgrService.SetVerificationPassedAsync(input);

        //[HttpPost]
        //public Task<bool> SetVerificationFailureAsync(SetVerificationFailureDto input) => topicMgrService.SetVerificationFailureAsync(input);

        [HttpPost]
        public Task<bool> SaveITDevelopRecordsAsync(CreateITDevelopingDto input) => topicMgrService.SaveITDevelopRecordsAsync(input);

        [HttpPost]
        public Task<bool> PublishAsync(LongIdInputDto input) => topicMgrService.PublishAsync(input);

        [HttpGet("topic-draft-details/{id}")]
        public Task<TopicDraftDetailsDto> GetTopicDraftDetails(long id) => topicMgrService.GetTopicDraftInfoAsync(id);

        [HttpGet]
        public Task<PagedResultDto<TopicListItemDto>> GetMyTopicPagingListAsync([FromQuery] SearchMyTopicDto input) => topicMgrService.GetMyTopicPagingListAsync(input);

        [HttpGet]
        public Task<PagedResultDto<TopicDraftItemDto>> GetTopicDraftPagingListAsync([FromQuery] SearchTopicDraftDto input) => topicMgrService.GetTopicDraftPagingListAsync(input);

        [HttpGet]
        public Task<PagedResultDto<TopicDraftItemDto>> GetMyTopicDraftPagingListAsync([FromQuery] SearchTopicDraftDto input) => topicMgrService.GetMyTopicDraftPagingListAsync(input);

        [HttpGet("verify/{id}")]
        public async Task<object> Verify(long id)
        {
            return await topicMgrService.VerifyAsync(id, App.HttpContext.Request.Query);
        }

        [HttpPost]
        public Task<bool> ApproveAsync(BMApproveFlowDto input) => topicMgrService.ApproveAsync(input);
    }
}
