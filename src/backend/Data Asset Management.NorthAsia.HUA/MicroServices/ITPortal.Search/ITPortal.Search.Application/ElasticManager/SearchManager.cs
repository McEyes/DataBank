
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using ITPortal.Search.Application.OpenSearch.Dtos;
using ITPortal.Search.Application.SearchTopic.Services;
using ITPortal.Search.Application.SearchTopicGrants.Services;
using ITPortal.Search.Application.TopicDocument.Dtos;
using ITPortal.Search.Core.LightElasticSearch.Providers;
using ITPortal.Search.Core.Models.Search;

using Microsoft.Extensions.Logging;

using Nest;

using System.Reflection;
namespace ITPortal.Search.Application.ElasticManager
{
    public class SearchManager : LightElasticsearchService
    {
        //private readonly IUserRolesService _userRolesService;
        private readonly ISearchTopicService _searchTopicService;
        private readonly ISearchTopicGrantsService _searchTopicGrantsService;
        private readonly ILogger<SearchManager> _logger;


        public SearchManager(
            ILightElasticsearchProvider elasticsearchProvider,
            //IUserRolesService userRolesService,
            ISearchTopicService searchTopicService,
            ISearchTopicGrantsService searchTopicGrantsService,
            ILogger<SearchManager> logger
            ) : base(elasticsearchProvider,logger)
        {
            //_userRolesService = userRolesService;
            _searchTopicService = searchTopicService;
            _searchTopicGrantsService = searchTopicGrantsService;
            _logger = logger;
        }

        /// <summary>
        /// 获取推荐文章
        /// </summary>
        /// <returns></returns>
        public async Task<PageResult<TopicDocumentDto>> GetRecommendByTag(
            List<string> tags,
            string userId,
            int pageIndex = 1,
            int pageSize = 10)
        {
            (_, var topics) = await GetTopicByUserId(userId);

            if (tags == null || tags.Count == 0 || topics.Count == 0)
            {
                return new PageResult<TopicDocumentDto>(0, new List<TopicDocumentDto>());
            }

            string indexAliasName = GetIndexAliasName<TopicDocumentModel>();

            var result = await Client.SearchAsync(
                (SearchDescriptor<TopicDocumentModel> p) =>
                    p.Index(indexAliasName)
                    .From((pageIndex - 1) * pageSize)
                    .Size(pageSize)
                    .Sort((s) => s
                        .Descending(p => p.UpdateTime))
                    .Query(q => q
                        .Bool(b => b
                            .Should(tags.ConvertAll(tag =>
                                (Func<QueryContainerDescriptor<TopicDocumentModel>, QueryContainer>)(bs => bs
                                    .ConstantScore(cs => cs
                                        .Boost(1)
                                        .Filter(f => f
                                            .Match(m => m
                                                .Field(f => f.Tags)
                                                .Query(tag)
                                            )
                                        )
                                    )
                                ))
                            )
                            .MinimumShouldMatch(1)
                            .Must(b => b
                                .Terms(m => m
                                    .Field(f => f.Topic)
                                    .Terms(topics))
                        ))
                    )
            );

            return new PageResult<TopicDocumentDto>((int)result.Total, result.Hits
                .Select(p =>
                {
                    p.Source.Id = p.Id;
                    return p.Source.Adapt<TopicDocumentDto>();
                }).ToList());
        }

        /// <summary>
        /// 获取候选词列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetCandidateWords(string keyword, string userId, int maxLenght = 10)
        {

            (_, var topics) = await GetTopicByUserId(userId);

            if (string.IsNullOrWhiteSpace(keyword) || topics.Count == 0)
            {
                return new List<string>();
            }

            string indexAliasName = GetIndexAliasName<TopicDocumentModel>();

            var candidateWords = new List<string>();
            var result = await Client.SearchAsync(
                (SearchDescriptor<TopicDocumentModel> p) =>
                    p.Index(indexAliasName)
                    .Source(s => s.Includes(f => f.Fields(f => f.Keyword)))
                    .Suggest(su => su
                        .Completion("keyword_suggest", c => c
                            .Prefix(keyword)
                            .Field(f => f.Keyword)
                            .Analyzer("ik_smart")
                            .Fuzzy(f => f.Fuzziness(Fuzziness.Auto)
                                .PrefixLength(0)
                                .Transpositions(true))
                            .SkipDuplicates(true)
                            .Size(maxLenght)))
            );
            foreach (string key in result.Suggest.Keys)
            {
                foreach (var su in result.Suggest[key])
                {
                    foreach (var option in su.Options)
                    {
                        candidateWords.Add(option.Text);
                        if (option.Source != null && option.Source.Keyword != null && option.Source.Keyword.Count > 0)
                        {
                            candidateWords.AddRange(option.Source.Keyword.Where(p => p.StartsWith(keyword)));
                        }
                    }
                }
            }

            return candidateWords.Distinct().Take(maxLenght).ToList();
        }


        /// <summary>
        /// 分组获取主题文档
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PageResult<GroupByTopicDocumentDto>> GroupByQueryTopicDocuments(
            GroupByTopicDocumentQueryModel query, string userId)
        {
            (var totalCount, var topics) = await GetTopicByUserId(userId, query.PageIndex, query.PageSize);
            var documents = new PageResult<GroupByTopicDocumentDto>() { Data = new List<GroupByTopicDocumentDto>() };

            // 没有主题权限，返回空数组
            if (topics.Count == 0) return documents;

            //string indexAliasName = GetIndexAliasName<TopicDocumentModel>();

            foreach (var topic in topics)
            {
                var document = await QueryTopicDocuments(new TopicDocumentQueryModel
                {
                    Topics = new List<string> { topic },
                    Keyword = query.Keyword,
                    Classify = query.Classify,
                    PageIndex = 1,
                    PageSize = query.MaxDocumentCount,
                }, userId);
                documents.Data.Add(new GroupByTopicDocumentDto() { Topic = topic, Documents = document.Data });
            }
            documents.Total = documents.PageSize = documents.Data.Count;
            documents.PageNum = 1;
            return documents;
        }


        public async Task<List<string>> QueryDocumentTopics(TopicDocumentQueryModel query, string userId)
        {
            (_, var topics) = await GetTopicByUserId(userId);
            return topics;
            if (topics.Count == 0)
            {
                return new List<string>();
            }

            string indexAliasName = GetIndexAliasName<TopicDocumentModel>(); 

            var result = await Client.SearchAsync(
                (SearchDescriptor<TopicDocumentModel> p) =>
                    p.Index(indexAliasName)
                    .From((query.PageIndex - 1) * query.PageSize)
                    .Size(query.PageSize)
                    .Aggregations(new AggregationDictionary
                    {
                        {
                            "unique_field_values", new TermsAggregation("unique_field_values")
                            {
                                Field = "topic.keyword",
                            }
                        }
                    })
                    .Query(q =>
                    {
                        var boolQuery = new BoolQuery();
                        var musts = new List<QueryContainer>();

                        musts.Add(new TermsQuery
                        {
                            Field = Infer.Field<TopicDocumentModel>(f => f.Topic),
                            Terms = topics,
                        });

                        // ID查询，仅允许单个条件
                        if (!string.IsNullOrWhiteSpace(query.Id))
                        {
                            musts.Add(new TermQuery
                            {
                                Field = Infer.Field("_id"),
                                Value = query.Id,
                            });
                            boolQuery.Must = musts;
                            return boolQuery;
                        }


                        if (!string.IsNullOrWhiteSpace(query.Keyword))
                        {
                            musts.Add(new MultiMatchQuery
                            {
                                Fields = Infer.Fields<TopicDocumentModel>(
                                    f => f.Title,
                                    f => f.Keyword,
                                    f => f.Description,
                                    f => f.Tags,
                                    f => f.Content),
                                Query = query.Keyword
                            });
                        }
                        if (query.Classify != null && query.Classify.Count > 0)
                        {
                            musts.Add(new TermsQuery
                            {
                                Field = Infer.Field<TopicDocumentModel>(f => f.Classify),
                                Terms = query.Classify
                            });
                        }


                        // 数据主权条件
                        if (userId.IsNotNullOrWhiteSpace())
                        {
                            var userIdTermQueryCreator = new TermQuery
                            {
                                Field = Infer.Field<TopicDocumentModel>(f => f.Creator),
                                Value = userId
                            };

                            var userIdTermQueryUpdator = new TermQuery
                            {
                                Field = Infer.Field<TopicDocumentModel>(f => f.Updater),
                                Value = userId.ToString()
                            };


                            var enableDataSovereigntyTrueQuery = new BoolQuery
                            {
                                Must = new List<QueryContainer>
                            {
                                new TermQuery
                                {
                                    Field = Infer.Field<TopicDocumentModel>(f => f.EnableDataSovereignty),
                                    Value = true
                                },
                                new BoolQuery
                                {
                                    Should = new List<QueryContainer>
                                    {
                                        userIdTermQueryCreator,
                                        userIdTermQueryUpdator
                                    }
                                }
                            }
                            };

                            var enableDataSovereigntyFalseQuery = new TermQuery
                            {
                                Field = Infer.Field<TopicDocumentModel>(f => f.EnableDataSovereignty),
                                Value = false
                            };

                            var finalQuery = new BoolQuery
                            {
                                Should = new List<QueryContainer>
                            {
                                enableDataSovereigntyTrueQuery,
                                enableDataSovereigntyFalseQuery
                            }
                            };
                            boolQuery.Should = finalQuery.Should;
                        }

                        boolQuery.Must = musts;
                        if (boolQuery.Should != null && boolQuery.Should.Count() > 0)
                        {
                            boolQuery.MinimumShouldMatch = 1;
                        }

                        return boolQuery;
                    })
            );
            if (!result.IsValid)
            {
                _logger.LogInformation($"search Valid:{result.DebugInformation}");
                return new List<string>();
            }

            var uniqueValues = result.Aggregations.Terms("unique_field_values");

            return uniqueValues.Buckets.Select(p => p.Key).ToList();
        }


        /// <summary>
        /// 查询主题文档
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<TopicDocumentPageResultDto> QueryTopicDocuments(TopicDocumentQueryModel query, string userId)
        {
            (_, var topics) = await GetTopicByUserId(userId);
            var grantsTopics = topics;
            if (query.Topics != null && query.Topics.Count > 0)
            {
                grantsTopics = query.Topics.Where(p => topics.Any(t => t == p)).ToList();
            }
            if (grantsTopics.Count == 0)
            {
                return new TopicDocumentPageResultDto(0, new List<TopicDocumentDto>());
            }

            string indexAliasName = GetIndexAliasName<TopicDocumentModel>();

            var result = await Client.SearchAsync(
                (SearchDescriptor<TopicDocumentModel> p) =>
                    p.Index(indexAliasName)
                    .From((query.PageIndex - 1) * query.PageSize)
                    .Size(query.PageSize)
                    //.Sort((SortDescriptor<TopicDocumentModel> s) =>
                    //    s.Descending(p => p.CreateTime)
                    //.Descending("_id")
                    //)
                    //.Sort((SortDescriptor<TopicDocumentModel> s) =>
                    //    s.Descending(p => p.UpdateTime))
                    .Query(q =>
                    {
                    // ID查询，仅允许单个条件
                    if (!string.IsNullOrWhiteSpace(query.Id))
                    {
                        return q.Term(t => t
                            .Field("_id")
                            .Value(query.Id));
                    }
                    var boolQuery = new BoolQuery();
                    var musts = new List<QueryContainer>();

                    if (query.Topics != null && query.Topics.Count > 0)
                    {
                        musts.Add(new TermQuery
                        {
                            Field = Infer.Field<TopicDocumentModel>(f => f.Topic),
                            Value = query.Topics.First()
                        });
                    }

                        // ID查询，仅允许单个条件
                        if (!string.IsNullOrWhiteSpace(query.Id))
                        {
                            musts.Add(new TermQuery
                            {
                                Field = Infer.Field("_id"),
                                Value = query.Id,
                            });
                            boolQuery.Must = musts;
                            return boolQuery;
                        }


                        if (!string.IsNullOrWhiteSpace(query.Keyword))
                        {
                            var multiMatch = new MultiMatchQuery
                            {
                                //Fields = fields,
                                Fields = Infer.Fields("title^3",       // Title字段权重最高
                                            "keyword^2.5",     // Keyword次之
                                            "tags^2",      // Tags权重更低
                                            "description^1.5", // Description再次之
                                            "content"        // Content默认权重1
                                            ),
                                Query = query.Keyword,
                                Type = TextQueryType.BestFields,
                                Operator = Operator.Or
                            };
                            musts.Add(multiMatch);
                        }
                        if (query.Classify != null && query.Classify.Count > 0)
                        {
                            musts.Add(new TermsQuery
                            {
                                Field = Infer.Field<TopicDocumentModel>(f => f.Classify),
                                Boost = 10,
                                Terms = query.Classify
                            });
                        }


                        // 数据主权条件
                        //if (userId.IsNotNullOrWhiteSpace())
                        //{
                        //    var userIdTermQueryCreator = new TermQuery
                        //    {
                        //        Field = Infer.Field<TopicDocumentModel>(f => f.Creator),
                        //        Value = userId
                        //    };

                        //    var userIdTermQueryUpdator = new TermQuery
                        //    {
                        //        Field = Infer.Field<TopicDocumentModel>(f => f.Updater),
                        //        Value = userId.ToString()
                        //    };


                        //    var enableDataSovereigntyTrueQuery = new BoolQuery
                        //    {
                        //        Must = new List<QueryContainer>
                        //    {
                        //        new TermQuery
                        //        {
                        //            Field = Infer.Field<TopicDocumentModel>(f => f.EnableDataSovereignty),
                        //            Value = true
                        //        },
                        //        new BoolQuery
                        //        {
                        //            Should = new List<QueryContainer>
                        //            {
                        //                userIdTermQueryCreator,
                        //                userIdTermQueryUpdator
                        //            }
                        //        }
                        //    }
                        //    };

                        //    var enableDataSovereigntyFalseQuery = new TermQuery
                        //    {
                        //        Field = Infer.Field<TopicDocumentModel>(f => f.EnableDataSovereignty),
                        //        Value = false
                        //    };

                        //    var finalQuery = new BoolQuery
                        //    {
                        //        Should = new List<QueryContainer>
                        //    {
                        //        enableDataSovereigntyTrueQuery,
                        //        enableDataSovereigntyFalseQuery
                        //    }
                        //    };
                        //    boolQuery.Should = finalQuery.Should;
                        //}

                        boolQuery.Must = musts;
                        if (boolQuery.Should != null && boolQuery.Should.Count() > 0)
                        {
                            boolQuery.MinimumShouldMatch = 1;
                        }

                        return boolQuery;
                    })
                    .Sort(ss => ss.Descending(SortSpecialField.Score)) // 按评分降序排列
                    .Explain()
                    .Highlight(h => h
                        .PreTags("<strong>")
                        .PostTags("</strong>")
                        //.FragmentSize(800000)
                        //.NumberOfFragments(0)
                        //.RequireFieldMatch(false)
                        .Fields(
                            f => f.Field(f => f.Title),
                            f => f.Field(f => f.Description),
                            f => f.Field(f => f.Content)
                        )
                    )
            );

            return new TopicDocumentPageResultDto(result.Total == -1 ? 0 : (int)result.Total, result.Hits
                .Select(p =>
                {
                    var hightlight = new Dictionary<string, string>();
                    foreach (string key in p.Highlight.Keys)
                    {
                        hightlight[key] = p.Highlight[key].FirstOrDefault() ?? "";
                    }

                    p.Source.Id = p.Id;
                    p.Source.Highlight = hightlight;
                    return p.Source.Adapt<TopicDocumentDto>();
                }).ToList());
        }

        /// <summary>
        /// 使用ID查询记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TopicDocumentModel> GetTopicDocumentById(string id)
        {
            //string indexAliasName = GetIndexAliasName<TopicDocumentModel>();

            var result = await GetAsync<TopicDocumentModel>(id);
            if (result == null || result.Hits == null || result.Hits.Count == 0)
            {
                return null;
            }
            return result.Hits.Select(p => p.Source).FirstOrDefault();
        }

        /// <summary>
        /// 创建主题文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<string> CreateTopicDocument(TopicDocumentModel document)
        {
            // 关键字为空则分词生成关键字
            if (document.Keyword == null || document.Keyword.Count == 0)
            {
                var texts = new List<string>();
                if (!string.IsNullOrWhiteSpace(document.Title)) texts.Add(document.Title);
                if (!string.IsNullOrWhiteSpace(document.Description)) texts.Add(document.Description);
                if (!string.IsNullOrWhiteSpace(document.Content)) texts.Add(document.Content);
                if (document.Classify != null && document.Classify.Count > 0) texts.AddRange(document.Classify);
                if (document.Tags != null && document.Tags.Count > 0) texts.AddRange(document.Tags);

                var analyzeResponse = await Client.Indices.AnalyzeAsync(new AnalyzeRequest
                {
                    Text = texts,
                    Analyzer = "ik_max_word"
                });
                if (analyzeResponse.IsValid)
                {
                    document.Keyword = analyzeResponse.Tokens.Select(p => p.Token).Distinct().ToList();
                }
            }

            var doc = await GetTopicDocumentById(document.Id);
            if (doc != null)
            {
                var updateResult = await UpdateAsync(document);
                if (updateResult == null || !updateResult.IsValid)
                {
                    //if (((Nest.UpdateResponse<TopicDocumentModel>)updateResult).)
                    {
                        _logger.LogInformation($"Update Index Valid:{updateResult?.DebugInformation}");
                        return null;
                    }
                }
            }
            else
            {
                var insertResult = await InsertAsync(document);
                if (insertResult == null || !insertResult.IsValid)
                {
                    if (((Nest.BulkResponse)insertResult).Errors)
                    {
                        _logger.LogInformation($"insert Index Valid:{insertResult?.DebugInformation}");
                        return null;
                    }
                }
            }
            return document.Id;
        }

        /// <summary>
        /// 更新主题文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTopicDocument(TopicDocumentModel document)
        {
            var res = await UpdateAsync(document);
            return res != null && res.IsValid;
        }

        /// <summary>
        /// 删除主题文档
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTopicDocument(List<string> ids)
        {
            var res = await DeleteManyAsync<TopicDocumentModel>(ids);
            return res != null && res.IsValid;
        }


        private async Task<(long totalCount, List<string> topics)> GetTopicByUserId(string userId, int pageIndex = -1, int pageSize = -1)
        {
            //if (userId.IsNullOrWhiteSpace())
            //{
            var allTopic = await _searchTopicService.Query(new SearchTopic.Dtos.SearchTopicDto() { IsEnable = true });
            return (allTopic.Count(), allTopic.Select(p => p.Topic).ToList());
            //}

            var publicTopics = await _searchTopicService.Query(new SearchTopic.Dtos.SearchTopicDto() { IsPublic = true, IsEnable = true });

            List<string> roleIds = new List<string>();// await _userRolesService.GetRolesByUserId(userId.Value);
            List<Guid> topicIds = await _searchTopicGrantsService.GetTopicIdsByRoleIds(roleIds);
            topicIds.AddRange(publicTopics.Select(p => p.Id));
            var result = await _searchTopicService.GetByTopicIds(topicIds, pageIndex, pageSize);
            var topicNames = result.items.Select(p => p.Topic).ToList();
            return (result.totalCount, topicNames);
        }


        private static string GetIndexAliasName<TEntity>() where TEntity : class
        {
            Type typeFromHandle = typeof(TEntity);
            ElasticsearchTypeAttribute customAttribute = typeFromHandle.GetCustomAttribute<ElasticsearchTypeAttribute>();
            if (customAttribute == null)
            {
                throw new ArgumentNullException(" The RelationName of ElasticsearchTypeAttribute can not be NULL or Empty ");
            }

            return customAttribute.RelationName.ToLower();// +"*";
        }

    }
}
