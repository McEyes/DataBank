
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;

namespace DataTopicStore.Application.Topics.Services
{
    public class TopicRatingsService : ApplicationService, ITopicRatingsService
    {
        private readonly Repository<TopicRatingsEntity> topicRatingsRepository;
        private readonly Repository<TopicEntity> topicRepository;
        private readonly ITopicService topicService;
        public TopicRatingsService(
            ITopicService topicService,
            Repository<TopicRatingsEntity> topicRatingsRepository,
            Repository<TopicEntity> topicRepository)
        {
            this.topicService = topicService;
            this.topicRatingsRepository = topicRatingsRepository;
            this.topicRepository = topicRepository;
        }

        public async Task<bool> SubmitAsync(TopicRatingsSubmitDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentOutOfRangeException.ThrowIfLessThan(input.star, 1);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(input.star, 5);

            var entity = input.Adapt<TopicRatingsEntity>();
            entity.id = Guid.NewGuid();
            entity.created_time = DateTime.Now;
            entity.created_by = CurrentUser.Id;

            var topicEntity = await topicRepository.GetByIdAsync(input.topic_id);
            if (topicEntity == null)
                throw Oops.Oh("The topic information does not exist.");

            if ((await topicRatingsRepository.GetFirstAsync(t => t.topic_id == input.topic_id && t.created_by == CurrentUser.Id)) != null)
                throw Oops.Oh("You have already rated, you cannot repeat the operation.");

            await topicRatingsRepository.InsertAsync(entity);

            var query = topicRatingsRepository.AsQueryable();
            var rating = await query.Where(t => t.topic_id == input.topic_id).AvgAsync(t => (decimal)t.star);
            var rating_count = await topicRatingsRepository.CountAsync(t => t.topic_id == input.topic_id);
            return await topicService.UpdateRatingsAsync(new UpdateTopicRatingsDto { id = input.topic_id, ratings = rating, ratings_count = rating_count });
        }
    }
}
