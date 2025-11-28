using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicLikesService
    {
        Task<bool> SubmitAsync(TopicLikeOrDislikeDto input);
    }
}
