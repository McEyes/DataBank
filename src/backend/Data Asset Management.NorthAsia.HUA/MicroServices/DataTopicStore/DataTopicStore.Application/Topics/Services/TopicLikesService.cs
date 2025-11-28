

using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Models;
using DataTopicStore.Core.Repositories;


namespace DataTopicStore.Application.Topics.Services
{
    public class TopicLikesService : ApplicationService, ITopicLikesService
    {
        private readonly Repository<TopicLikesEntity> topicLikesRepository;
        public TopicLikesService(
            Repository<TopicLikesEntity> topicCommentsRepository)
        {
            this.topicLikesRepository = topicCommentsRepository;
        }

        public async Task<bool> SubmitAsync(TopicLikeOrDislikeDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var query = topicLikesRepository.AsQueryable();
            var entity = await GetEntityAsync(input.topic_id, CurrentUser.Id);
            if (entity == null)
            {
                entity = input.Adapt<TopicLikesEntity>();
                entity.id = Guid.NewGuid();
                entity.created_time = DateTime.Now;
                entity.created_by = CurrentUser.Id;
                entity.user_id = CurrentUser.Id;
                entity.like_type = input.like_type;
                entity.topic_id = input.topic_id;

                return await topicLikesRepository.InsertAsync(entity);
            }
            else
            {
                entity = input.Adapt(entity);

                entity.updated_by = CurrentUser.Id;
                entity.updated_time = DateTime.Now;

                return await topicLikesRepository.UpdateAsync(entity);
            }
        }

        private async Task<TopicLikesEntity> GetEntityAsync(long topic_id, string user_id)
        {
            var query = topicLikesRepository.AsQueryable();
            var entity = await query.Where(t => t.topic_id == topic_id && t.user_id == user_id).FirstAsync();
            return entity;
        }
    }
}
