using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using MapsterMapper;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/Doc/", Name = "数据资产 文档管理 服务")]
    [ApiDescriptionSettings(GroupName = "数据资产AssetDoc")]
    public class AssetDocAppService : IDynamicApiController
    {
        private readonly IAssetDocService _assetDocService;

        public AssetDocAppService(IAssetDocService dataApiService)
        {
            _assetDocService = dataApiService;
        }

        #region base api


        [HttpPost()]
        public async Task<int> Post(AssetDocEntity entity)
        {
            return await _assetDocService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _assetDocService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<AssetDocEntity> Get(Guid id)
        {
            return await _assetDocService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _assetDocService.Delete(ids);
        }


        public async Task<AssetDocEntity> Single(AssetDocDto entity)
        {
            return await _assetDocService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, [FromBody] AssetDocEntity entity)
        {
            if (entity.Id == Guid.Empty) entity.Id = id;
            return await _assetDocService.Modify(entity);
        }


        [HttpPut()]
        public async Task<int> Put(AssetDocEntity entity)
        {
            return await _assetDocService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(AssetDocEntity entity)
        {
            return await _assetDocService.ModifyHasChange(entity);
        }

        [HttpPost("Page")]
        public async Task<PageResult<AssetDocEntity>> Page(AssetDocDto filter)
        {
            return await _assetDocService.Page(filter);
        }

        //[HttpGet("QueryPage")]
        //public async Task<PageResult<AssetDocEntity>> Page3([FromQuery] AssetDocDto filter)
        //{
        //    return await _assetDocService.Page(filter);
        //}

        [HttpPost("List")]
        public async Task<List<AssetDocEntity>> Query(AssetDocDto entity)
        {
            return await _assetDocService.Query(entity);
        }

        //[HttpGet("QueryList")]
        //public async Task<List<AssetDocEntity>> Query3([FromQuery] AssetDocDto entity)
        //{
        //    return await _assetDocService.Query(entity);
        //}

        #endregion base api
    }
}
