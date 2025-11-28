

using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Models;
using DataTopicStore.Core.Repositories;


namespace DataTopicStore.Application.Topics.Services
{
    public class TopicFavoritesService : ApplicationService, ITopicFavoritesService
    {
        private readonly Repository<TopicFavoritesEntity> topicFavoritesRepository;
        private readonly Repository<TopicEntity> topicRepository;
        public TopicFavoritesService(
            Repository<TopicFavoritesEntity> topicFavoritesRepository, Repository<TopicEntity> topicRepository)
        {
            this.topicFavoritesRepository = topicFavoritesRepository;
            this.topicRepository = topicRepository;
        }

        public async Task<bool> SubmitAsync(TopicFavoritesSubmitDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var topicEntity = await topicRepository.GetByIdAsync(input.topic_id);
            if (topicEntity == null) throw Oops.Oh("Data topic information does not exist.");
            var entity = await topicFavoritesRepository.GetFirstAsync(t => t.topic_id == input.topic_id && t.user_id == CurrentUser.Id);
            if (entity == null)
            {
                entity = input.Adapt<TopicFavoritesEntity>();
                entity.id = Guid.NewGuid();
                entity.created_time = DateTime.Now;
                entity.created_by = CurrentUser.Id;
                entity.user_id = CurrentUser.Id;
                entity.topic_id = input.topic_id;

                await topicFavoritesRepository.InsertAsync(entity);

                topicEntity.favorite_count = topicEntity.favorite_count == null?1: topicEntity.favorite_count +1;
                await topicRepository.AsUpdateable(topicEntity).UpdateColumns("favorite_count").ExecuteCommandAsync();
            }
            else
            {
                topicEntity.favorite_count = topicEntity.favorite_count == null ? 0 : topicEntity.favorite_count - 1;
                await topicRepository.AsUpdateable(topicEntity).UpdateColumns("favorite_count").ExecuteCommandAsync();
                await topicFavoritesRepository.DeleteAsync(entity);
            }

            return true;
        }

        public async Task<List<FavoriteTopicListItemDto>> GetMyFavoriteTopicsAsync()
        {
            var currentUserid = CurrentUser.Id;
            var queryTopic = topicRepository.AsQueryable();
            var queryFavorite = topicFavoritesRepository.AsQueryable();
            return await queryTopic.InnerJoin(queryFavorite.Where(t => t.user_id == currentUserid), (a, b) => a.id == b.topic_id).SelectMergeTable((a, b) => new FavoriteTopicListItemDto
            {
                id = a.id,
                name = a.name,
                cover = a.cover
            }).ToListAsync();
        }
    }
}
