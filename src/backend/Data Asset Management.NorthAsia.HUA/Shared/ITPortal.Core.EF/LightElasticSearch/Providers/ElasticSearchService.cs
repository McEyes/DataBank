using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.QueryDsl;

using Furion.DatabaseAccessor;

using ITPortal.Extension.System;

namespace ITPortal.Core.LightElasticSearch.Providers
{
    public class ElasticSearchService<TEntity, KeyType> : IElasticSearchService<TEntity, KeyType> where TEntity : EntityBase<KeyType>
    {
        /// <summary>
        /// 自动生成，根据属性配置，可以修改
        /// </summary>
        public string IndexFullName { get; set; }
        /// <summary>
        /// 自动生成，根据属性配置，可以修改
        /// </summary>
        public string IndexAliasName { get; set; }

        public ElasticsearchClient Client { get; private set; }

        private bool NoIndexName=false;

        public ElasticSearchService()
        {
            IndexFullName = LightElasticsearchService.GetIndexFullName<TEntity>();
            Client = ElasticsearchProvider.GenElasticClient();
            NoIndexName = IndexFullName.IsNullOrWhiteSpace();
        }
        public ElasticSearchService(IElasticsearchProvider elasticsearchProvider)
        {
            IndexFullName = LightElasticsearchService.GetIndexFullName<TEntity>();
            Client = elasticsearchProvider.GetElasticClient();
            NoIndexName = IndexFullName.IsNullOrWhiteSpace();
        }

        public async Task<CreateIndexResponse> CreateIndex(CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            var response = await Client.Indices.ExistsAsync((IndexName)IndexFullName, cancellationToken);
            if (response.Exists) return new CreateIndexResponse() { Acknowledged = true, Index = IndexFullName };
            return await Client.Indices.CreateAsync<TEntity>(IndexFullName, cancellationToken);
        }

        public async Task<DeleteResponse> DeleteAsync(KeyType id, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            return await Client.DeleteAsync<TEntity>(IndexFullName, new Id(id), cancellationToken);
        }

        public async Task<DeleteByQueryResponse> DeleteManyAsync(IEnumerable<KeyType> ids, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            return await Client.DeleteByQueryAsync<TEntity>(f => f.
                    Query(d => d.
                        Bool(b => b.
                            Must(m => m.
                                Terms(ts => ts.
                                Field("Id").
                                Terms(new TermsQueryField(
                                    ids.Select(id =>
                                            FieldValue.String(id.ToString())
                                        ).ToArray())
                                    )
                                )
                            )
                        )
                    ), 
                    cancellationToken);
        }

        public async Task<TEntity> GetAsync(KeyType id, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            var result = await Client.GetAsync<TEntity>(IndexFullName, new Id(id), cancellationToken);
            if (result.Found)
            {
                return result.Source;
            }
            return null;
        }

        public async Task<List<TEntity>> GetAsync(IEnumerable<KeyType> ids, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            var result = await Client.SearchAsync<TEntity>(p =>
                p.Index(IndexAliasName)
                .Query(q => q
                    .Bool(b => b.
                        Must(m => m.
                            Terms(ts => ts.
                                Field("Id").
                                Terms(new Elastic.Clients.Elasticsearch.QueryDsl.TermsQueryField(
                                    ids.Select(id =>
                                            FieldValue.String(id.ToString())
                                        ).ToArray())
                                    )
                                )
                            )
                        )
                    )
                );
            if (result.IsSuccess())
            {
                return result.Documents.ToList();
            }
            return new List<TEntity>();
        }

        public SearchRequest<TEntity> AsQuery()
        {
            if (NoIndexName) return null;
            return new SearchRequest<TEntity>(IndexFullName);
        }

        public SearchRequest<TEntity> Query(string searchfiled, string searchKey)
        {
            if (NoIndexName) return null;
            var searchQuery = AsQuery();
            searchQuery.Query = Elastic.Clients.Elasticsearch.QueryDsl.Query.Term(new TermQuery(field: searchfiled)
            {
                Value = searchKey
            });
            return searchQuery;
        }

        //public SearchRequest<TEntity> Page(SearchRequest<TEntity> query, IPageEntity<KeyType> filter)
        //{
        //    query.Size = filter.PageSize;
        //    query.From = filter.SkipCount;
        //    return query;
        //}


        public async Task<IndexResponse> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            return await Client.IndexAsync(entity, (IndexName)IndexFullName);
        }

        public async Task<BulkResponse> InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            return await Client.IndexManyAsync(entities, (IndexName)IndexFullName);
        }

        public async Task<UpdateResponse<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (NoIndexName) return null;
            var id = entity.Id?.ToString() ?? string.Empty;
            if (id.IsNullOrWhiteSpace()) return new UpdateResponse<TEntity>() { Result = Elastic.Clients.Elasticsearch.Result.NotFound };
            //var getResult = await Client.GetAsync<TEntity>(IndexAliasName, new Id(id), cancellationToken);
            //if (!getResult.Found)
            //{
            //    return new UpdateResponse<TEntity>() { Result = Elastic.Clients.Elasticsearch.Result.NotFound };
            //}
            return await Client.UpdateAsync<TEntity, TEntity>((IndexName)IndexFullName,
                id,
                u => u.Doc(entity),
                cancellationToken);
        }

        //public async Task<BulkResponse> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        //{
        //    var bulkRequest = new BulkRequest();
        //    var operations = new List<IBulkOperation>();
        //    foreach (var entity in entities)
        //    {
        //        var updateOperation = new UpdateOperation<T>
        //        {
        //            Id = entity.Id,
        //            Index = _indexName,
        //            Doc = entity,
        //            DocAsUpsert = true
        //        };
        //        operations.Add(updateOperation);
        //    }
        //    bulkRequest.Operations = operations;

        //    return await Client.BulkAsync(bulkRequest, cancellationToken);
        //}

        //private IEnumerable<PropertyInfo> GetSearchableFields()
        //{
        //    return typeof(TEntity).GetProperties()
        //        .Where(p => p.GetCustomAttribute<ElasticsearchPropertyAttribute>()?.IsSearchable == true);
        //}
    }
}