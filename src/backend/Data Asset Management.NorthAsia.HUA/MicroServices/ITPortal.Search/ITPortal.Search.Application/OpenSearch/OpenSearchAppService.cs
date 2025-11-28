using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Search.Application.ElasticManager;
using ITPortal.Search.Application.OpenSearch.Dtos;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Application.SearchTopic.Services;
using ITPortal.Search.Application.TopicDocument.Dtos;
using ITPortal.Search.Application.UserSearchHistory.Services;
using ITPortal.Search.Core.Models;
using ITPortal.Search.Core.Models.Search;

namespace ITPortal.Search.Application.OpenSearch
{
    [AppAuthorize]
    [Route("api/OpenSearch/", Name = "全局搜索 开放搜索服务")]
    [ApiDescriptionSettings(GroupName = "全局搜索 开放搜索")]
    public class OpenSearchAppService : IDynamicApiController
    {
        private readonly SearchManager _searchManager;
        private readonly ISearchTopicService _searchTopicService;
        private readonly IUserSearchHistoryService _userSearchHistoryService;
        //private readonly IService<InnerUser> _innerUserrepository;

        public OpenSearchAppService(
            SearchManager searchManager,
            ISearchTopicService searchTopicService,
           //ICurrentUser currentUser,
           //IService<InnerUser> innerUserrepository,
           IUserSearchHistoryService userSearchHistoryService
            )
        {
            _searchManager = searchManager;
            _searchTopicService = searchTopicService;
            //_currentUser = currentUser;
            //_innerUserrepository = innerUserrepository;
            _userSearchHistoryService = userSearchHistoryService;
        }

        /// <summary>
        /// 检查config参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public dynamic GetConfig(string key = "RemoteApi:AppHostUrl")
        {
            return App.GetConfig<string>(key);
        }


        /// <summary>
        /// 获取搜索候选词
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("CandidateWords")]
        //[SwaggerOperation(summary: "获取搜索候选词")]
        public Task<List<string>> GetCandidateWords([FromQuery] CandidateWordQueryDto query)
        {
            return _searchManager.GetCandidateWords(query.Keyword, _searchTopicService.CurrentUser.Id, query.MaxLenght);
        }

        /// <summary>
        /// 根据标签获取推荐主题文档
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("Recommend")]
        //[SwaggerOperation(summary: "根据标签获取推荐主题文档")]
        public async Task<PageResult<TopicDocumentDto>> GetRecommendByTag([FromQuery] TagRecommendQueryDto query)
        {
            var list = await _searchManager.GetRecommendByTag(query.Tags, _searchTopicService.CurrentUser.Id, query.PageIndex, query.PageSize);
            return list;
        }

        /// <summary>
        /// 开放搜索-主题分组模式
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("TopicGroup")]
        //[SwaggerOperation(summary: "开放搜索-主题分组模式")]
        public async Task<PageResult<GroupByTopicDocumentDto>> OpenSearchByTopicGroup([FromQuery] GroupByTopicDocumentQueryDto query)
        {
            var queryable = query.Adapt<GroupByTopicDocumentQueryModel>();
            return await _searchManager
                .GroupByQueryTopicDocuments(queryable, _searchTopicService.CurrentUser.Id);
        }

        /// <summary>
        /// 开放搜索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("")]
        //[SwaggerOperation(summary: "开放搜索")]
        public async Task<TopicDocumentPageResultDto> OpenSearch([FromQuery] TopicDocumentQueryDto query)
        {
            string userId = _searchTopicService.CurrentUser.Id;
            if (userId == Guid.Empty.ToString()) userId = null;
            query.Keyword = query.Keyword.Trim();
            if (string.IsNullOrWhiteSpace(query.Keyword))
            {
                return new TopicDocumentPageResultDto();
            }
            if (userId.IsNotNullOrWhiteSpace())
            {
                var historyIds = await _userSearchHistoryService.AsQueryable()
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreationTime)
                    .Skip(9)
                    .Select(p => p.Id)
                    .ToListAsync();
                if (historyIds.Any())
                {
                    await _userSearchHistoryService.Delete(historyIds);
                }
                await _userSearchHistoryService.Delete(p => p.UserId == userId && p.Keyword == query.Keyword);
                await _userSearchHistoryService.Create(new UserSearchHistoryEntity
                {
                    Keyword = query.Keyword,
                    UserId = userId,
                    CreationTime = DateTimeOffset.Now,
                });
            }

            var queryable = query.Adapt<TopicDocumentQueryModel>();// ObjectMapper.Map<TopicDocumentQueryDto, TopicDocumentQueryModel>(query);

            if (queryable.Classify == null) query.Classify = null;

            var topics = await _searchManager.QueryDocumentTopics(queryable, userId);
            var topicEntities = await _searchTopicService.GetListAsync(p => topics.Contains(p.Topic));

            var page = await _searchManager.QueryTopicDocuments(queryable, userId);



            page.Topics = topics.Select(p =>
            {
                return topicEntities.Where(t => t.Topic == p).FirstOrDefault()?.Adapt<SearchTopicDto>();
            })
            .Where(p => p != null)
            .ToList();

            //UserInfo user = new UserInfo();// (await _innerUserrepository.GetListAsync(p => p.Id == userId)).FirstOrDefault();
            if (userId.IsNotNullOrWhiteSpace())
            {
                for (int i = 0; i < page.Data.Count; i++)
                {
                    var item = page.Data[i];
                    if (item.Payload != null && item.Payload.Buttons != null && item.Payload.Buttons.Count > 0)
                    {
                        var buttons = new List<TopicDocumentButtonCreateDto>();
                        foreach (var btn in item.Payload.Buttons)
                        {
                            if (btn.HasPermissionUserIds.Count == 0 && btn.HasPermissionWorkcells.Count == 0 ||
                                btn.HasPermissionUserIds.Contains(userId))//|| (user != null && btn.HasPermissionWorkcells.Contains(user.Workcell ?? ""))
                            {
                                buttons.Add(btn);
                            }
                        }
                        item.Payload.Buttons = buttons;
                    }
                }
            }

            return page;
        }

    }
}
