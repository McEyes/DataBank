using DataTopicStore.Application.ApproveFlow.Dtos;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicMgrService
    {
        Task<bool> SaveAsync(CreateOrUpdateTopicDto input);
        Task<bool> SetParametersInputAsync(SetParametersInputDto input);
        Task<bool> SetParametersOutputAsync(SetParametersOutputDto input);
        Task<bool> SetVerificationPassedAsync(SetVerificationPassedDto input);
        Task<bool> SetVerificationFailureAsync(SetVerificationFailureDto input);
        Task<bool> SaveITDevelopRecordsAsync(CreateITDevelopingDto input);
        Task<bool> SaveValidationResultAsync(BusinessModelValidationResultDto input);
        Task<PagedResultDto<TopicDraftItemDto>> GetTopicDraftPagingListAsync(SearchTopicDraftDto input);
        Task<PagedResultDto<TopicListItemDto>> GetMyTopicPagingListAsync(SearchMyTopicDto input);
        Task<PagedResultDto<TopicDraftItemDto>> GetMyTopicDraftPagingListAsync(SearchTopicDraftDto input);
        Task<bool> PublishAsync(LongIdInputDto input);
        Task<TopicDraftDetailsDto> GetTopicDraftInfoAsync(long id);
        Task<object> VerifyAsync(long id, IQueryCollection input);
        Task<bool> ApproveAsync(BMApproveFlowDto dto);
    }
}
