using ITPortal.Extension.System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Nest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ITPortal.Search.Core.LightElasticSearch.Providers
{
    public interface IESDocument
    {
        public string Id { get; set; }
    }
    public abstract class LightElasticsearchService : Furion.DependencyInjection.ITransient
    {
        private readonly ILightElasticsearchProvider _elasticsearchProvider;
        private readonly IElasticClient _client;
        private static Dictionary<string, bool> _existsIndexDictionary;
        private readonly ILogger<LightElasticsearchService> _logger;

        public LightElasticsearchService(ILightElasticsearchProvider elasticsearchProvider,ILogger<LightElasticsearchService> logger)
        {
            _elasticsearchProvider = elasticsearchProvider;
            _client = _elasticsearchProvider.GetElasticClient();
            _existsIndexDictionary = new Dictionary<string, bool>();
            _logger = logger;
        }

        protected IElasticClient Client => _client;

        private void CreateIndex<TEntity>() where TEntity : class
        {
            var indexAliasName = GetIndexAliasName<TEntity>();
            indexAliasName = indexAliasName.ToLower();
            var indexName = $"{indexAliasName}-{DateTimeOffset.Now:yyyyMMdd}";

            // return if exits
            if (_existsIndexDictionary.ContainsKey(indexName)) return;

            var createResponse = _client.Indices.Create(indexName, createIndexDescriptor =>
            {
                return createIndexDescriptor
                .Settings(s => s.NumberOfShards(2).NumberOfReplicas(0))
                .Map<TEntity>(m => m.AutoMap());
            });


            var putAliasResponse = _client.Indices.PutAlias(indexName, indexAliasName);
            if (createResponse != null && createResponse.Acknowledged)
            {
                _existsIndexDictionary.AddIfNotContains(new KeyValuePair<string, bool>(indexName, true));
            }

            _existsIndexDictionary.AddIfNotContains(new KeyValuePair<string, bool>(indexName, true));
        }

        /// <summary>
        /// write multiple pieces of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        protected async Task<ResponseBase> InsertManyAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            var indexName = GetIndexName<TEntity>();
            CreateIndex<TEntity>();

            return await _client.BulkAsync((a) => a.IndexMany(entities, (descriptor, s) => descriptor.Index(indexName)));
        }

        /// <summary>
        /// Write a single piece of data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected async Task<ResponseBase> InsertAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            return await InsertManyAsync(new List<TEntity> { entity });
        }

        protected async Task<ResponseBase> UpdateAsync<TEntity>(TEntity entity)
           where TEntity : class
        {
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty("Id");
            if (propertyInfo == null) throw new Exception("Entity missing Id property info");
            var idValue = propertyInfo.GetValue(entity);
            if (idValue == null) return null;

            var indexAliasName = GetIndexAliasName<TEntity>();
            indexAliasName = indexAliasName.ToLower();
            var getResult = await GetAsync<TEntity>(idValue.ToString() ?? string.Empty);
            if (getResult == null || getResult.Hits == null)
            {
                _logger.LogInformation($"Update Index Valid:{getResult?.DebugInformation}");
                return null;
            }
            else if (getResult.Hits.Count == 0)
            {
                return await InsertManyAsync(new List<TEntity> { entity });
            }
            DocumentPath<TEntity> _id = new DocumentPath<TEntity>(entity);
            var indexName = getResult.Hits.FirstOrDefault().Index;
            return await _client.UpdateAsync(_id, u => u.Index(indexName).Doc(entity));
        }

        protected async Task<ISearchResponse<TEntity>> GetAsync<TEntity>(string id)
           where TEntity : class
        {
            var indexAliasName = GetIndexAliasName<TEntity>();
            indexAliasName = indexAliasName.ToLower();
            var reuslt = await _client.SearchAsync<TEntity>(p => p.Index(indexAliasName).Query(q => q.Term(t => t.Field("_id").Value(id))));
            return reuslt;
        }

        protected async Task<ISearchResponse<TEntity>> GetManyAsync<TEntity>(IEnumerable<string> ids)
           where TEntity : class
        {
            var indexAliasName = GetIndexAliasName<TEntity>();
            indexAliasName = indexAliasName.ToLower();
            var reuslt = await _client.SearchAsync<TEntity>(p => p.Index(indexAliasName).Query(q => q.Terms(ff => ff.Field("_id").Terms(ids))));
            return reuslt;
        }

        protected async Task<DeleteByQueryResponse> DeleteManyAsync<TEntity>(IEnumerable<string> ids)
            where TEntity : class
        {
            var indexAliasName = GetIndexAliasName<TEntity>();
            indexAliasName = indexAliasName.ToLower();
            List<Id> list = new List<Id>();
            foreach (var item in ids)
                list.Add(item);
            return await _client.DeleteByQueryAsync<TEntity>(f => f.Index(indexAliasName).Query(q => q.Ids(ids => ids.Values(list))));
        }

        private string GetIndexName<TEntity>() where TEntity : class
        {
            var indexAliasName = GetIndexAliasName<TEntity>();
            return $"{indexAliasName.ToLower()}-{DateTimeOffset.Now:yyyyMMdd}";
        }

        /// <summary>
        /// Get the index name
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static string GetIndexAliasName<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            var esAttr = type.GetCustomAttribute<ElasticsearchTypeAttribute>();
            if (esAttr == null) throw new ArgumentNullException(" The RelationName of ElasticsearchTypeAttribute can not be NULL or Empty ");
            var indexName = esAttr.RelationName;

            return indexName;
        }
    }
}
