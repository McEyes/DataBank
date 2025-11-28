using Elastic.Clients.Elasticsearch;

using Furion.DatabaseAccessor;

namespace ITPortal.Core.LightElasticSearch.Providers
{
    public interface IElasticSearchService<TEntity, KeyType> where TEntity : EntityBase<KeyType>//, IEntity<KeyType>
    {
        /// <summary>
        /// 自动生成，根据属性配置，可以修改
        /// </summary>
         string IndexFullName { get; set; }
        string IndexAliasName { get; }
        ElasticsearchClient Client { get; }
        //Task<CreateIndexResponse> CreateIndex(CancellationToken cancellationToken = default);
        ///
        Task<DeleteResponse> DeleteAsync(KeyType id, CancellationToken cancellationToken = default);
        Task<DeleteByQueryResponse> DeleteManyAsync(IEnumerable<KeyType> ids, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync(KeyType id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAsync(IEnumerable<KeyType> ids, CancellationToken cancellationToken = default);
        SearchRequest<TEntity> AsQuery();
        SearchRequest<TEntity> Query(string searchfiled, string searchKey);
        //SearchRequest<TEntity> Page(SearchRequest<TEntity> query, IPageEntity<KeyType> filter);
        Task<IndexResponse> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<BulkResponse> InsertManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<UpdateResponse<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        //Task<BulkResponse> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    }

    public interface IElasticSearchService<T> where T :EntityBase<string>// class//, IEntity<string>
    {
        
    }
}