using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Core.SqlParser.Models;

using MapsterMapper;

using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/sources/", Name = "数据资产sources服务")]
    [ApiDescriptionSettings(GroupName = "数据资产sources")]
    public class DataSourceAppService : IDynamicApiController
    {
        private readonly IDataSourceService _dataSourceService;
        private readonly IDistributedCacheService _cache;
        private readonly IDataColumnService _columnService;

        public DataSourceAppService(IDataSourceService dataApiService, IDistributedCacheService cache, IDataColumnService columnService)
        {
            _dataSourceService = dataApiService;
            _cache = cache;
            _columnService = columnService;
        }


        [HttpGet()]
        [HttpPost()]
        public async Task<int> Count()
        {
            return await _dataSourceService.Count();
        }

        [HttpPost("CheckConnection")]
        public async Task<ITPortal.Core.Services.IResult> CheckConnection(DataSourceDto dataSource)
        {
            return await _dataSourceService.CheckConnection(dataSource);
        }


        [HttpGet("{sourceId}/{tableId}/column")]
        [HttpPost("{sourceId}/{tableId}/column")]
        public async Task<List<DataColumnEntity>> GetTableCloumns(string sourceId, string tableId)
        {
            return await _columnService.Query(new DataColumn.Dtos.DataColumnDto() { SourceId = sourceId, TableId = tableId });
        }

        [HttpGet("{sourceId}/GetDbTablesMergeLocal")]
        [HttpGet("{sourceId}/tables")]
        public async Task<List<DbTable>> GetDbTablesMergeLocal(string sourceId)
        {
            return await _dataSourceService.GetDbTablesMergeLocal(sourceId);
        }


        [HttpGet("{sourceId}/{tableName}/columns")]
        [HttpPost("{sourceId}/{tableName}/columns")]
        public async Task<List<DataColumnEntity>> GetDataTableColumns(string sourceId, string tableName)
        {
            return await _dataSourceService.GetDataTableColumns(sourceId, tableName);
        }

        [HttpGet("{sourceId}/{tableName}/Dbcolumns")]
        [HttpPost("{sourceId}/{tableName}/Dbcolumns")]
        public async Task<List<DataColumnEntity>> GetDbTableColumns(string sourceId, string tableName)
        {
            return await _dataSourceService.GetDbTableColumns(sourceId, tableName);
        }

        #region base api


        [HttpPost()]
        public async Task<int> Post(DataSourceEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                //entity.Id = entity.Id = Guid.NewGuid().ToString();
                return await _dataSourceService.Create(entity);
            }
            else
                return await _dataSourceService.Modify(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataSourceService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataSourceInfo> Get(string id)
        {
            return (await _dataSourceService.Get(id)).Adapt<DataSourceInfo>();
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataSourceService.Delete(ids);
        }


        public async Task<DataSourceEntity> Single(DataSourceDto entity)
        {
            return await _dataSourceService.Single(entity);
        }

        [HttpPut("{id}")]
        [HttpPut()]
        public async Task<int> Put(DataSourceEntity entity)
        {
            return await _dataSourceService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataSourceEntity entity)
        {
            return await _dataSourceService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataSourceInfo>> Page([FromQuery]DataSourceDto filter)
        {
            return (await _dataSourceService.Page(filter)).Adapt<PageResult<DataSourceInfo>>();
        }


        [HttpPost("page")]
        public async Task<PageResult<DataSourceInfo>> Page2(DataSourceDto filter)
        {
            return (await _dataSourceService.Page(filter)).Adapt<PageResult<DataSourceInfo>>();
        }

        [HttpGet("list")]
        public async Task<List<DataSourceInfo>> Query([FromQuery] DataSourceDto filter)
        {
            return (await _dataSourceService.Query(filter)).OrderBy(f=>f.SourceName).ToList().Adapt<List<DataSourceInfo>>();
        }

        [HttpPost("list")]
        public async Task<List<DataSourceInfo>> Query2([FromQuery] DataSourceDto filter)
        {
            return (await _dataSourceService.Query(filter)).OrderBy(f => f.SourceName).ToList().Adapt<List<DataSourceInfo>>();
        }

        #endregion base api
    }
}
