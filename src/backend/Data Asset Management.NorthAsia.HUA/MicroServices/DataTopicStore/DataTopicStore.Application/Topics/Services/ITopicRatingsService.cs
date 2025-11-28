using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicRatingsService
    {
        Task<bool> SubmitAsync(TopicRatingsSubmitDto input);
    }
}
