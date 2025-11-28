using Jabil.Service.Extension.Customs.Dtos;
using jb.hmc.Service.Domain.Aggregates;
using jb.hmcsearch.Service.Application.Contracts.Search;
using jb.hmcsearch.Service.Application.Contracts.Search.Dtos;
using jb.hmcsearch.Service.Domain.Aggregates;
using jb.hmcsearch.Service.Domain.IRepositories;
using jb.hmcsearch.Service.Domain.Managers;
using jb.hmcsearch.Service.Domain.Shared.Models.Search;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace jb.hmcsearch.Service.Application.Search
{
    public class OpenSearchAppService : HMCSearchAppService, IOpenSearchAppService
    {
        private readonly SearchManager _searchManager;
        private readonly ISearchTopicRepository _searchTopicRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<InnerUser> _innerUserrepository;
        private readonly IRepository<UserSearchHistory> _userSearchHistoryRepository;

        public OpenSearchAppService(
            SearchManager searchManager,
            ISearchTopicRepository searchTopicRepository,
            ICurrentUser currentUser,
            IRepository<InnerUser> innerUserrepository,
            IRepository<UserSearchHistory> userSearchHistoryRepository
            )
        {
            _searchManager = searchManager;
            _searchTopicRepository = searchTopicRepository;
            _currentUser = currentUser;
            _innerUserrepository = innerUserrepository;
            _userSearchHistoryRepository = userSearchHistoryRepository;
        }

        public Task<List<string>> GetCandidateWords(CandidateWordQueryDto query)
        {
            return _searchManager.GetCandidateWords(query.Keyword, _currentUser.GetId(), query.MaxLenght);
        }


        public async Task<PageResult<TopicDocumentDto>> GetRecommendByTag(TagRecommendQueryDto query)
        {
            var res = await _searchManager.GetRecommendByTag(query.Tags, _currentUser.GetId(), query.PageIndex, query.PageSize);

            return new PageResult<TopicDocumentDto>(
                res.TotalCount,
                res.Items
                    .Select(p =>
                    {
                        var dto = ObjectMapper.Map<TopicDocumentModel, TopicDocumentDto>(p);
                        if (p.Attachments != null)
                        {
                            dto.AttachmentList = p.Attachments
                                .Select(ObjectMapper.Map<TopicDocumentAttachmentsModel, TopicDocumentAttachmentDto>)
                                .ToList();
                        }
                        if (!string.IsNullOrEmpty(p.PayloadStr))
                        {
                            dto.Payload = JsonConvert.DeserializeObject<TopicDocumentPayloadDto>(p.PayloadStr) ?? new TopicDocumentPayloadDto();
                        }

                        return dto;
                    })
                    .ToList());
        }

        public async Task<PageResult<GroupByTopicDocumentDto>> OpenSearchByTopicGroup(GroupByTopicDocumentQueryDto query)
        {
            var queryable = ObjectMapper.Map<GroupByTopicDocumentQueryDto, GroupByTopicDocumentQueryModel>(query);
            (var totalCount, var items) = await _searchManager
                .GroupByQueryTopicDocuments(queryable, _currentUser.GetId());

            return new PageResult<GroupByTopicDocumentDto>(
                totalCount,
                items.Keys
                    .Select(t =>
                    {
                        string topicName = items.Keys.Where(p => t.Equals(p)).FirstOrDefault("");
                        IEnumerable<TopicDocumentModel> docs = string.IsNullOrWhiteSpace(topicName)
                            ? new List<TopicDocumentModel>() : items[topicName];

                        return new GroupByTopicDocumentDto
                        {
                            Topic = t,
                            Documents = docs
                                .Select(p =>
                                {
                                    var dto = ObjectMapper.Map<TopicDocumentModel, TopicDocumentDto>(p);
                                    if (p.Attachments != null)
                                    {
                                        dto.AttachmentList = p.Attachments
                                            .Select(ObjectMapper.Map<TopicDocumentAttachmentsModel, TopicDocumentAttachmentDto>)
                                            .ToList();
                                    }
                                    if (!string.IsNullOrEmpty(p.PayloadStr))
                                    {
                                        dto.Payload = JsonConvert.DeserializeObject<TopicDocumentPayloadDto>(p.PayloadStr) ?? new TopicDocumentPayloadDto();
                                    }
                                    return dto;
                                })
                                .ToList()
                        };
                    })
                    .ToList());
        }

        public async Task<TopicDocumentPageResultDto> OpenSearch(TopicDocumentQueryDto query)
        {
            Guid userId = _currentUser.GetId();

            if (string.IsNullOrWhiteSpace(query.Keyword))
            {
                return new TopicDocumentPageResultDto();
            }

            var historyIds = (await _userSearchHistoryRepository.GetQueryableAsync())
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreationTime)
                .ToList()
                .Skip(9)
                .Select(p => p.Id);
            if (historyIds.Any())
            {
                await _userSearchHistoryRepository.DeleteAsync(p => historyIds.Contains(p.Id));
            }
            await _userSearchHistoryRepository.DeleteAsync(p => p.UserId == userId && p.Keyword == query.Keyword.Trim());
            await _userSearchHistoryRepository.InsertAsync(new UserSearchHistory
            {
                Keyword = query.Keyword.Trim(),
                UserId = userId,
                CreationTime = DateTime.Now,
            });


            var queryable = ObjectMapper.Map<TopicDocumentQueryDto, TopicDocumentQueryModel>(query);

            if (queryable.Classify == null) query.Classify = null;

            var topics = await _searchManager.QueryDocumentTopics(queryable, userId);
            var topicEntities = await _searchTopicRepository.GetListAsync(p => topics.Contains(p.Topic));

            var res = await _searchManager.QueryTopicDocuments(queryable, userId);

            var page = new TopicDocumentPageResultDto(res.TotalCount, res.Items
                .Select(p =>
                {
                    var dto = ObjectMapper.Map<TopicDocumentModel, TopicDocumentDto>(p);
                    if (p.Attachments != null)
                    {
                        dto.AttachmentList = p.Attachments
                            .Select(ObjectMapper.Map<TopicDocumentAttachmentsModel, TopicDocumentAttachmentDto>)
                            .ToList();
                    }
                    if (!string.IsNullOrEmpty(p.PayloadStr))
                    {
                        dto.Payload = JsonConvert.DeserializeObject<TopicDocumentPayloadDto>(p.PayloadStr) ?? new TopicDocumentPayloadDto();
                    }

                    return dto;
                })
                .ToList());

            page.Topics = topics.Select(p =>
            {
                var t = topicEntities.Where(t => t.Topic == p).FirstOrDefault();
                return ObjectMapper.Map<SearchTopic, SearchTopicDto>(t);
            })
            .Where(p => p != null)
            .ToList();

            var user = (await _innerUserrepository.GetListAsync(p => p.Id == userId)).FirstOrDefault();
            for (int i = 0; i < page.Items.Count; i++)
            {
                var item = page.Items[i];
                if (item.Payload != null && item.Payload.Buttons != null && item.Payload.Buttons.Count > 0)
                {
                    var buttons = new List<TopicDocumentButtonCreateDto>();
                    foreach (var btn in item.Payload.Buttons)
                    {
                        if ((btn.HasPermissionUserIds.Count == 0 && btn.HasPermissionWorkcells.Count == 0) ||
                             (btn.HasPermissionUserIds.Contains(userId) || (user != null && btn.HasPermissionWorkcells.Contains(user.Workcell ?? ""))))
                        {
                            buttons.Add(btn);
                        }
                    }

                    item.Payload.Buttons = buttons;
                }
            }

            return page;
        }

    }
}
