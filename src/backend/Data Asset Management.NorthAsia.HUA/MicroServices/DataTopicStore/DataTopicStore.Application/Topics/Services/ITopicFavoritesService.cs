using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Topics.Services
{
    public interface ITopicFavoritesService
    {
        Task<bool> SubmitAsync(TopicFavoritesSubmitDto input);
        Task<List<FavoriteTopicListItemDto>> GetMyFavoriteTopicsAsync();
    }
}