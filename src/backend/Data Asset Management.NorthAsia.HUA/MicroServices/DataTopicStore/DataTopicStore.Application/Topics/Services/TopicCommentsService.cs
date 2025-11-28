

using System.Linq;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Categories.Services;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Enums;
using DataTopicStore.Core.Models;
using DataTopicStore.Core.Repositories;


namespace DataTopicStore.Application.Topics.Services
{
    public class TopicCommentsService : ApplicationService, ITopicCommentsService
    {
        private readonly Repository<TopicCommentsEntity> topicCommentsRepository;
        private readonly Repository<TopicCommentsLikesEntity> topicCommentsLikesRepository;
        private readonly Repository<TopicEntity> topicRepository;
        private readonly ITopicService topicService;
        public TopicCommentsService(
            ITopicService topicService,
            Repository<TopicEntity> topicRepository,
            Repository<TopicCommentsEntity> topicCommentsRepository,
            Repository<TopicCommentsLikesEntity> topicCommentsLikesRepository)
        {
            this.topicService = topicService;
            this.topicRepository = topicRepository;
            this.topicCommentsRepository = topicCommentsRepository;
            this.topicCommentsLikesRepository = topicCommentsLikesRepository;
        }

        public async Task<bool> SubmitAsync(TopicCommentsSubmitDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentException.ThrowIfNullOrWhiteSpace(input.content);

            var entity = input.Adapt<TopicCommentsEntity>();
            entity.id = Guid.NewGuid();
            entity.created_time = DateTime.Now;
            entity.created_by = CurrentUser.Id;
            entity.reviewer = CurrentUser.Name;

            var topicEntity = await topicRepository.GetByIdAsync(input.topic_id);
            if (topicEntity == null)
                throw Oops.Oh("The topic information does not exist");

            topicEntity.comments_count = topicEntity.comments_count == null ? 1 : topicEntity.comments_count + 1;
            await topicRepository.AsUpdateable(topicEntity).UpdateColumns("comments_count").ExecuteCommandAsync();
            await topicCommentsRepository.InsertAsync(entity);

            return true;
        }

        public async Task<bool> CheckIsExitsAsync(Guid id)
        {
            var entity = await topicCommentsRepository.GetByIdAsync(id);
            return entity != null;
        }

        public async Task<bool> SetLikeOrDislikeAsync(TopicCommentsLikeOrDislikeDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            var entity = input.Adapt<TopicCommentsEntity>();

            entity.id = Guid.NewGuid();
            entity.created_time = DateTime.Now;
            entity.created_by = CurrentUser.Id;

            if (!(await this.CheckIsExitsAsync(input.comments_id)))
                throw Oops.Oh("The comment information does not exist");

            return await topicCommentsRepository.UpdateAsync(entity);
        }

        public async Task<PagedResultDto<TopicCommentListItemDto>> GetPagingListAsync(SearchTopicCommentsDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var currentUserId = CurrentUser.Id;
            dto.PageIndex = dto.PageIndex <= 0 ? 1 : dto.PageIndex;
            dto.PageSize = dto.PageSize <= 10 ? 10 : dto.PageSize;

            var totalCount = await topicCommentsRepository.CountAsync(t => t.topic_id == dto.topic_id);
            var data = await topicCommentsRepository.GetPageListAsync(t => t.topic_id == dto.topic_id, new PageModel
            {
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
                TotalCount = totalCount
            }, t => t.created_time, OrderByType.Desc);
            var pagedResult = new PagedResultDto<TopicCommentListItemDto>();
            pagedResult.TotalCount = totalCount;
            pagedResult.Data = data.Adapt<List<TopicCommentListItemDto>>();

            if (pagedResult.Data is List<TopicCommentListItemDto> { Count: > 0 })
            {
                var likeList = await topicCommentsLikesRepository.GetListAsync(t => pagedResult.Data.Select(a => a.id).Contains(t.comments_id));// && t.user_id == currentUserId);
                foreach (var item in pagedResult.Data)
                {
                    if (likeList != null)
                    {
                        var likeEntity = likeList.FirstOrDefault(t => t.comments_id == item.id && t.user_id == currentUserId);
                        item.is_liked = likeEntity == null ? null : likeEntity.like_type == LikesType.Likes;

                        item.liked_count = likeList.Where(t => t.like_type == LikesType.Likes).Count();
                        item.disliked_count = likeList.Where(t => t.like_type == LikesType.Dislikes).Count();
                    }

                }
            }

            return pagedResult;
        }
    }
}
