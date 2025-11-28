using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;

using ITPortal.Core;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application
{
    public interface IDataApiService : IBaseService<DataApiEntity, DataApiQueryDto, string>
    {
        Task<List<RouteInfo>> All();
        Task<List<RouteInfo>> AllFromCache(bool clearCache = false);
        Task<int> Count();
        Task<int> CountFromCache();

        /// <summary>
        /// 注册数据资产API
        /// </summary>
        /// <param name="apiConfig"></param>
        /// <returns></returns>
        void Register(RouteInfo apiConfig);
        /// <summary>
        /// 注册数据资产API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void Register(string id);

        /// <summary>
        /// 注销数据资产API
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        Task Unregister(string apiUrl);
        /// <summary>
        /// 注销数据资产API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UnregisterById(string id);

        Task<string> InitRoutes(bool clearCache = false);
        Task<DataApiEntity> Copy(string id);
        Task<dynamic> GetDataApiDetailById(string id);
        Task<DataApiEntity> GetBySourceId(string sourceId);
        Task<dynamic> Cancel(RouteInfo apiInfo);
        Task<dynamic> Release(RouteInfo apiConfig);
        Task<DataApiEntity> Save(DataApiCreateDto entity, bool clearCache = true);
        //Task<List<RouteInfo>> InitRedisHash();
        Task<Result> DeleteByUrlList(List<string> urlList);
        Task<IResult<List<DataApiEntity>>> AutoCreate(List<APIAutoGenParam> paramsList);
        Task<Dictionary<string, HashSet<string>>> GetTopTopicApiList(string[] ctlIds = null, string searchkey = "");
        Task<List<DataTableEntity>> GetNoApiTables();
        Task<IResult<List<DataApiEntity>>> CreateMappTableApi(List<DataTableEntity> tableList);
    }
}
