using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Transport.Products.Elasticsearch;

using Furion;
using Furion.DependencyInjection;

using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Reflection;

namespace ITPortal.Core.LightElasticSearch.Providers
{
    public class LightElasticsearchService : ISingleton
    {
        //private readonly IElasticsearchProvider _elasticsearchProvider;
        private readonly ElasticsearchClient _client;
        private static Dictionary<string, bool> _existsIndexDictionary;
        private readonly ILogger<LightElasticsearchService> _logger;

        private ElasticIndexNameAttribute indexNameAttribute;
        private ElasticSearchOptions elasticOptions;

        public LightElasticsearchService()
        {
            _existsIndexDictionary = new Dictionary<string, bool>();
            //_elasticsearchProvider = ElasticsearchProvider.GenElasticClient();
            elasticOptions = App.GetConfig<ElasticSearchOptions>("ElasticSearch") ?? new ElasticSearchOptions();
            if (elasticOptions.Enabled) _client = ElasticsearchProvider.GenElasticClient();
            //_logger = App.GetService<ILogger<LightElasticsearchService>>();
        }

        public LightElasticsearchService(IElasticsearchProvider elasticsearchProvider, ILogger<LightElasticsearchService> logger)
        {
            //_elasticsearchProvider = elasticsearchProvider;
            elasticOptions = App.GetConfig<ElasticSearchOptions>("ElasticSearch") ?? new ElasticSearchOptions();
            if (elasticOptions.Enabled) _client = elasticsearchProvider.GetElasticClient();
            _existsIndexDictionary = new Dictionary<string, bool>();
            _logger = logger;
        }

        protected ElasticsearchClient Client => _client;

        public async Task<CreateIndexResponse> CreateIndex<TEntity, KeyType>() where TEntity : class, IEntity<KeyType>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new CreateIndexResponse() { Acknowledged = false };
            if (_existsIndexDictionary.ContainsKey(indexName)) return new CreateIndexResponse() { Acknowledged = true, Index = indexName };
            var response = await _client.Indices.ExistsAsync(indexName);
            if (response.Exists) return new CreateIndexResponse() { Acknowledged = true, Index = indexName };
            return await _client.Indices.CreateAsync<TEntity>(indexName);
        }

        public async Task<CreateIndexResponse> CreateIndex<TEntity>() where TEntity : class, IEntity<string>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new CreateIndexResponse() { Acknowledged = false };
            if (_existsIndexDictionary.ContainsKey(indexName)) return new CreateIndexResponse() { Acknowledged = true, Index = indexName };
            var response = await _client.Indices.ExistsAsync(indexName);
            if (response.Exists) return new CreateIndexResponse() { Acknowledged = true, Index = indexName };
            return await _client.Indices.CreateAsync<TEntity>(indexName);
        }

        /// <summary>
        /// write multiple pieces of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<BulkResponse> InsertManyAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<string>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new BulkResponse() { Errors = false };
            return await _client.IndexManyAsync(entities, indexName);
        }

        /// <summary>
        /// write multiple pieces of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<BulkResponse> InsertManyAsync<TEntity, KeyType>(IEnumerable<TEntity> entities) where TEntity : class, IEntity<KeyType>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new BulkResponse() { Errors = false };
            return await _client.IndexManyAsync(entities, indexName);
        }

        /// <summary>
        /// Write a single piece of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IndexResponse> InsertAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<string>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new IndexResponse() { Result = Elastic.Clients.Elasticsearch.Result.NoOp };
            return await _client.IndexAsync(entity, (IndexName)indexName);
            //return await InsertManyAsync(new List<TEntity> { entity });
        }

        /// <summary>
        /// Write a single piece of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IndexResponse> InsertAsync(object data,Type type)
        {
            var indexName = GetIndexFullName(type);
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new IndexResponse() { Result = Elastic.Clients.Elasticsearch.Result.NoOp };
            return await _client.IndexAsync(data, (IndexName)indexName);
        }

        /// <summary>
        /// Write a single piece of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<IndexResponse> InsertAsync<TEntity, KeyType>(TEntity entity) where TEntity : class, IEntity<KeyType>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new IndexResponse() { Result = Elastic.Clients.Elasticsearch.Result.NoOp };
            return await _client.IndexAsync(entity, (IndexName)indexName);
            //return await InsertManyAsync(new List<TEntity> { entity });
        }

        public async Task<ElasticsearchResponse> UpdateAsync<TEntity>(TEntity entity) where TEntity : class, IEntity<string>
        {
            return await UpdateAsync<TEntity, string>(entity);
        }


        public async Task<ElasticsearchResponse> UpdateAsync<TEntity, KeyType>(TEntity entity) where TEntity : class, IEntity<KeyType>
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new UpdateResponse<TEntity>() { Result = Elastic.Clients.Elasticsearch.Result.NoOp };
            var id = entity.Id?.ToString() ?? string.Empty;
            if (id.IsNullOrWhiteSpace()) return new UpdateResponse<TEntity>() { Result = Elastic.Clients.Elasticsearch.Result.NotFound };

            var getResult = await this.GetAsync<TEntity>(id);
            if (!getResult.Found)
            {
                return await _client.IndexAsync(entity, (IndexName)indexName);
                //return new UpdateResponse<TEntity>() { Result = Elastic.Clients.Elasticsearch.Result.NotFound };
            }
            return await _client.UpdateAsync<TEntity, TEntity>((IndexName)indexName,
                id,
                u => u.Doc(entity));
        }

        public async Task<GetResponse<TEntity>> GetAsync<TEntity>(string id)
           where TEntity : class
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new GetResponse<TEntity>() { Found = false };
            if (id.IsNullOrWhiteSpace()) return new GetResponse<TEntity>() { Found = false };
            return await _client.GetAsync<TEntity>((IndexName)indexName, id);
        }



        public async Task<SearchResponse<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> ids)
           where TEntity : class
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new SearchResponse<TEntity>();
            var reuslt = await _client.SearchAsync<TEntity>(p =>
                p.Index(indexName)
                .Query(q => q
                    .Bool(b => b.
                        Must(m => m.
                            Terms(ts => ts.
                                Field("Id").
                                Terms(new Elastic.Clients.Elasticsearch.QueryDsl.TermsQueryField(
                                    ids.Select(id =>
                                            FieldValue.String(id)
                                        ).ToArray())
                                    )
                                )
                            )
                        )
                    )
                );
            return reuslt;
        }

        public async Task<DeleteResponse> DeleteAsync<TEntity>(dynamic id)
            where TEntity : class
        {
            var indexName = GetIndexFullName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new DeleteResponse() { Result = Elastic.Clients.Elasticsearch.Result.NoOp };
            return await _client.DeleteAsync<TEntity>(indexName, new Id(id));
        }

        public async Task<DeleteByQueryResponse> DeleteManyAsync<TEntity>(IEnumerable<string> ids)
            where TEntity : class
        {
            var indexName = GetIndexAliasName<TEntity>();
            if (indexName.IsNullOrWhiteSpace() || !elasticOptions.Enabled) return new DeleteByQueryResponse();
            return await _client.DeleteByQueryAsync<TEntity>(f => f.
                    Indices(indexName).
                    Query(d => d.
                        Bool(b => b.
                            Must(m => m.
                                Terms(ts => ts.
                                Field("Id").
                                Terms(new Elastic.Clients.Elasticsearch.QueryDsl.TermsQueryField(
                                    ids.Select(id =>
                                            FieldValue.String(id)
                                        ).ToArray())
                                    )
                                )
                            )
                        )
                    )
                );
        }

        /// <summary>
        /// Get the index name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IndexName GetIndexName<TEntity>() where TEntity : class
        {
            return (IndexName)GetIndexFullName<TEntity>();
        }
        /// <summary>
        /// Get the index name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetIndexFullName<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var elAttr = type.GetCustomAttribute<ElasticIndexNameAttribute>();
            if (elAttr == null) return string.Empty;// throw new ArgumentNullException(" The Elastic IndexName of ElasticIndexNameAttribute can not be NULL or Empty ");
            return elAttr.IndexFullName;
        }
        /// <summary>
        /// Get the index name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetIndexFullName(Type type)
        {
            var elAttr = type.GetCustomAttribute<ElasticIndexNameAttribute>();
            if (elAttr == null) return string.Empty;// throw new ArgumentNullException(" The Elastic IndexName of ElasticIndexNameAttribute can not be NULL or Empty ");
            return elAttr.IndexFullName;
        }
        /// <summary>
        /// Get the index alias name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetIndexAliasName<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var elAttr = type.GetCustomAttribute<ElasticIndexNameAttribute>();
            if (elAttr == null) return string.Empty;// throw new ArgumentNullException(" The Elastic IndexName of ElasticIndexNameAttribute can not be NULL or Empty ");
            return elAttr.IndexAliasName;// +"*";
        }

        /// <summary>
        /// Get the index name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ElasticIndexNameAttribute GetIndexNameAttribute<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            ElasticIndexNameAttribute? indexNameAttribute = type.GetCustomAttribute<ElasticIndexNameAttribute>();
            if (indexNameAttribute == null) return null;// throw new ArgumentNullException(" The Elastic IndexName of ElasticIndexNameAttribute can not be NULL or Empty ");
            return indexNameAttribute;
        }
    }
}
