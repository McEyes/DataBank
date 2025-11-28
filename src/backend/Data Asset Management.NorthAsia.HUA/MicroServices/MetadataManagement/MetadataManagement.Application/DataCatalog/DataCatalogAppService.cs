using ITPortal.Core.Services;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    /// <summary>
    /// 系统服务接口
    /// </summary>
    public class DataCatalogAppService : IDynamicApiController
    {
        private readonly IDataCatalogService _service;
        public DataCatalogAppService(IDataCatalogService service)
        {
            _service = service;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Create(DataCatalogEntity entity)
        {
            return await _service.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(long id)
        {
            return await _service.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<DataCatalogEntity> Get(long id)
        {
            return await _service.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _service.Delete(ids);
        }


        public async Task<DataCatalogEntity> Single(DataCatalogQuery entity)
        {
            return await _service.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(long id, DataCatalogEntity entity)
        {
            if (id > 0) entity.Id = id;
            return await _service.Modify(entity);
        }


        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataCatalogEntity entity)
        {
            return await _service.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataCatalogEntity>> Page([FromQuery] DataCatalogQuery filter)
        {
            return await _service.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<DataCatalogEntity>> Page2(DataCatalogQuery filter)
        {
            return await _service.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<DataCatalogEntity>> Query([FromQuery] DataCatalogQuery entity)
        {
            return await _service.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<DataCatalogEntity>> Query2(DataCatalogQuery entity)
        {
            return await _service.Query(entity);
        }

        #endregion base api
    }
}
