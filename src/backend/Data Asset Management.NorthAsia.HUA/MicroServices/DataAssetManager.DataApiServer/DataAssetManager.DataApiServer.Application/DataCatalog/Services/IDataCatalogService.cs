using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.Services;


namespace DataAssetManager.DataTableServer.Application
{
    public interface IDataCatalogService : IBaseService<DataCatalogEntity, DataCatalogDto, string>
    {
        Task<List<DataCatalogEntity>> All();
        Task<List<DataCatalogEntity>> AllFromCache(bool clearCache = false);
        Task<List<DataCatalogEntity>> GetChildrensTopic(string[] ctlIds, string searchkey="");
        Task<List<DataCatalogEntity>> GetParentTopic(string[] ctlIds);
        Task<List<DataCatalogEntity>> GetTopic(string[] ctlIds, string searchkey = "");
        Task<List<DataCatalogEntity>> GetTopTopic();
        Task<List<TreeEntity>> GetTreeTopic(string[] ctlIds, string searchkey="");
        Task<List<TreeEntity>> GetTreeTopicFromCache();
        Task<List<DataCatalogEntity>> InitRedisHash();
        Task<PageResult<DataCatalogInfo>> QueryPage(DataCatalogDto filter);
        //Task<List<APIAutoGenParam>> SaveMapping(CatalogMappingDto data,List<string> urlList);
    }
}
