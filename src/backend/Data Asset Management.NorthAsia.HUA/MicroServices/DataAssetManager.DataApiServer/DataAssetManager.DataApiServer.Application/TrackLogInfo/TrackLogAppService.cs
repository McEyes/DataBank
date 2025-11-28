using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.LightElasticSearch.Providers;
using ITPortal.Core.Services;

using MapsterMapper;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/TrackLog/", Name = "数据资产ApiTrackLogInfo服务")]
    [ApiDescriptionSettings(GroupName = "数据资产ApiTrackLogInfo")]
    public class TrackLogAppService : IDynamicApiController
    {
        private readonly ITrackLogService _TrackLogService;
        private readonly ILogger<TrackLogAppService> _logger;
        protected LightElasticsearchService _elasticSearchService { get; private set; }

        public TrackLogAppService(ITrackLogService dataApiService, LightElasticsearchService elasticSearchService, ILogger<TrackLogAppService> logger)
        {
            _TrackLogService = dataApiService;
            _elasticSearchService = elasticSearchService;
            _logger = logger;
        }


        #region base api


        [HttpPost()]
        public async Task<int> Create(ApiTrackLogInfo entity)
        {
            var elasResult = await _elasticSearchService.InsertAsync<ApiTrackLogInfo, Guid>(entity);
            if (!elasResult.IsValidResponse)
            {
                _logger.LogError($"Insert Elastic Index Error:{elasResult.DebugInformation}");
            }
            return await _TrackLogService.Create(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _TrackLogService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiTrackLogInfo> Get(Guid id)
        {
            return await _TrackLogService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(dynamic[] ids)
        {
            return await _TrackLogService.Delete(ids);
        }


        public async Task<ApiTrackLogInfo> Single(TrackLogDto entity)
        {
            return await _TrackLogService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, ApiTrackLogInfo entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _TrackLogService.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(ApiTrackLogInfo entity)
        {
            return await _TrackLogService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<ApiTrackLogInfo>> Page([FromQuery]TrackLogDto filter)
        {
            return await _TrackLogService.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<ApiTrackLogInfo>> Page2(TrackLogDto filter)
        {
            return await _TrackLogService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<ApiTrackLogInfo>> Query([FromQuery] TrackLogDto entity)
        {
            return await _TrackLogService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<ApiTrackLogInfo>> Query2(TrackLogDto entity)
        {
            return await _TrackLogService.Query(entity);
        }

        #endregion base api
    }
}
