using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicCommentsLikesService
    {
        Task<bool> SubmitAsync(TopicCommentsLikeOrDislikeDto input);
    }
}
