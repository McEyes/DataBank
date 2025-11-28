using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/tables/", Name = "数据资产Table服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Table")]
    public class DataTableAppService : IDynamicApiController
    {
        private readonly IDataTableService _dataTableService;
            //private readonly IDataApiService _dataApiService;
            //private readonly IDataCatalogService _dataCatalogService;
        //private readonly IMapper _mapper;
        private readonly IDistributedCacheService _cache;
        private readonly IDataColumnService _columnService;

        public DataTableAppService(IDataTableService dataApiService, 
            //IDataApiService dataApiService1,
            IDataColumnService columnService,
            //IDataCatalogService dataCatalogService,   
             IDistributedCacheService cache)
        {
            _dataTableService = dataApiService;
            //_mapper = mapper;
            _cache = cache;
            _columnService = columnService;
            //_dataApiService = dataApiService1;
            //_dataCatalogService = dataCatalogService;
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="clearCache"></param>
        /// <returns></returns>
        [HttpGet("UpTableRenameCache")]
        public async Task UpTableRenameCache(bool clearCache = false)
        {
            await _dataTableService.GetAllTableRename(clearCache);
        }



        [HttpGet("GetTableTag")]
        [HttpPost("GetTableTag")]
        public async Task<List<string>> GetTableTag()
        {
            return await _dataTableService.GetTableTags();
        }


        [HttpGet("GetTopicTable")]
        public async Task<PageResult<DataTableInfo>> GetTopicTable2([FromQuery]TopicTableQuery entity)
        {
            if (entity.Tag.IsNullOrWhiteSpace())
            {
                return await _dataTableService.GetTopicTable(entity);
            }
            return await _dataTableService.GetTablesByTag(entity);
        }

        [HttpPost("GetTopicTable")]
        public async Task<PageResult<DataTableInfo>> GetTopicTable(TopicTableQuery entity)
        {
            if (entity.Tag.IsNullOrWhiteSpace())
            {
                return await _dataTableService.GetTopicTable(entity);
            }
            return await _dataTableService.GetTablesByTag(entity);
        }


        [HttpGet("Topic/{id}")]
        public async Task<List<dynamic>> GetTablesByTopic(string id)
        {
            var list = await _dataTableService.GetTableByTopic(new TopicTableQuery() { CtlId = id, PageSize = 10000, PageNum = 1 });
            return list.Select(d =>
            {
                return new
                {
                    d.Alias,
                    d.DataTimeRange,
                    d.Id,
                    d.LevelId,
                    d.LevelName,
                    d.OwnerName,
                    d.Reviewable,
                    d.SourceId,
                    d.SourceName,
                    d.TableComment,
                    d.TableName,
                    d.UpdateFrequency,
                    d.UpdateMethod,
                };
            }).ToList<dynamic>();
        }


        [HttpGet("GetUserTable")]
        public async Task<PageResult<DataTableInfo>> GetUserTable([FromQuery] PageEntity<string> filter)
        {
            return await _dataTableService.GetUserTable(filter);
        }

        [HttpPost("GetUserTable")]
        public async Task<PageResult<DataTableInfo>> GetUserTable2(PageEntity<string> filter)
        {
            return await _dataTableService.GetUserTable(filter);
        }

        [HttpGet("GetTablesByUserId/{userid}")]
        public async Task<List<DataTableInfo>> GetTablesByUserId(string userid)
        {
            return await _dataTableService.GetUserAuthTablesByUserId(userid);
        }


        [HttpPost("GetOwnerTables")]
        public async Task<PageResult<DataTableInfo>> GetOwnerTables(DataTableDto filter)
        {
            return await _dataTableService.GetOwnerTables(filter);
        }



        [HttpGet("{sourceId}/{tableName}/columns")]
        [HttpPost("{sourceId}/{tableName}/columns")]
        public async Task<List<DataColumnEntity>> GetTableCloumns(string sourceId, string tableName)
        {
            var list = await _dataTableService.AllFromCache();
            var tableInfo = list.FirstOrDefault(f => tableName.Equals(f.TableName, StringComparison.InvariantCultureIgnoreCase));
            if (tableInfo == null) return new List<DataColumnEntity>();
            return await _columnService.Query(new DataColumn.Dtos.DataColumnDto() { SourceId = sourceId, TableId = tableInfo?.Id });
        }


        [HttpGet("{sourceId}/{tableId}/column")]
        [HttpPost("{sourceId}/{tableId}/column")]
        public async Task<List<DataColumnEntity>> GetTableCloumns2(string sourceId, string tableId)
        {
            return await _columnService.Query(new DataColumn.Dtos.DataColumnDto() { SourceId = sourceId, TableId = tableId });
        }


        [HttpGet("Depts")]
        public async Task<List<string>> GetTableDepts()
        {
            return await _dataTableService.GetTableDepts();
        }



        #region base api

        [HttpPost()]
        public async Task<DataTableEntity> Post(DataTableInput entity)
        {
            return await _dataTableService.Save(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataTableService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataTableInput> Get(string id)
        {
            return await _dataTableService.GetInfo(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        //[HttpPost("batch")]
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataTableService.Delete(ids);
        }


        public async Task<DataTableEntity> Single(DataTableInfo entity)
        {
            return await _dataTableService.Single(entity);
        }

        [HttpPut()]
        [HttpPut("{id}")]
        public async Task<DataTableEntity> Put(DataTableInput entity)
        {
            return await _dataTableService.Update(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataTableEntity entity)
        {
            return await _dataTableService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataTableEntity>> Page([FromQuery] DataTableInfo filter)
        {
            return await _dataTableService.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataTableEntity>> Page2(DataTableInfo filter)
        {
            return await _dataTableService.Page(filter);
        }

        [HttpPost("list")]
        public async Task<List<DataTableEntity>> Query(DataTableInfo entity)
        {
            return await _dataTableService.Query(entity);
        }

        [HttpGet("list")]
        public async Task<List<DataTableEntity>> Query2([FromQuery] DataTableInfo entity)
        {
            return await _dataTableService.Query(entity);
        }

        #endregion base api
    }
}
