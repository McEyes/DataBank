using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;

namespace DataTopicStore.Application.Topics.Services
{
    public class TopicCommentsLikesService : ApplicationService, ITopicCommentsLikesService
    {
        private readonly Repository<TopicCommentsLikesEntity> topicCommentsLikesRepository;
        public TopicCommentsLikesService(
            Repository<TopicCommentsLikesEntity> topicCommentsRepository)
        {
            this.topicCommentsLikesRepository = topicCommentsRepository;
        }

        public async Task<bool> SubmitAsync(TopicCommentsLikeOrDislikeDto input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var query = topicCommentsLikesRepository.AsQueryable();
            var entity = await GetEntityAsync(input.comments_id, CurrentUser.Id);
            if (entity == null)
            {
                entity = input.Adapt<TopicCommentsLikesEntity>();
                entity.id = Guid.NewGuid();
                entity.created_time = DateTime.Now;
                entity.created_by = CurrentUser.Id;
                entity.user_id = CurrentUser.Id;
                //entity.like_type = input.like_type;
                entity.comments_id = input.comments_id;

                switch (input.like_type)
                {
                    case Core.Enums.CancelLikesType.Likes:
                        entity.like_type = Core.Enums.LikesType.Likes;
                        break;
                    case Core.Enums.CancelLikesType.Dislikes:
                        entity.like_type = Core.Enums.LikesType.Dislikes;
                        break;
                    case Core.Enums.CancelLikesType.CancelLikes:
                        break;
                    case Core.Enums.CancelLikesType.CancelDislikes:
                        break;
                    default:
                        break;
                }

                return await topicCommentsLikesRepository.InsertAsync(entity);
            }
            else
            {
                entity = input.Adapt(entity);

                entity.updated_by = CurrentUser.Id;
                entity.updated_time = DateTime.Now;

                switch (input.like_type)
                {
                    case Core.Enums.CancelLikesType.Likes:
                        entity.like_type = Core.Enums.LikesType.Likes;
                        break;
                    case Core.Enums.CancelLikesType.Dislikes:
                        entity.like_type = Core.Enums.LikesType.Dislikes;
                        break;
                    case Core.Enums.CancelLikesType.CancelLikes:
                    case Core.Enums.CancelLikesType.CancelDislikes:
                        {
                            return await topicCommentsLikesRepository.DeleteAsync(entity);
                        }
                    default:
                        break;
                }

                return await topicCommentsLikesRepository.UpdateAsync(entity);
            }
        }

        private async Task<TopicCommentsLikesEntity> GetEntityAsync(Guid comments_id, string user_id)
        {
            var query = topicCommentsLikesRepository.AsQueryable();
            var entity = await query.Where(t => t.comments_id == comments_id && t.user_id == user_id).FirstAsync();
            return entity;
        }
    }
}
