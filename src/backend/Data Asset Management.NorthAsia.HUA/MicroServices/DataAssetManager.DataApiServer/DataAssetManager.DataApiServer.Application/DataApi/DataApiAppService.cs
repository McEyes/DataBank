using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using ITPortal.Core.Extensions;
using StackExchange.Profiling.Internal;
using ITPortal.Core;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/dataApis/", Name = "数据资产Api服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Api")]
    public class DataApiAppService : IDynamicApiController
    {
        private readonly IDataApiService _dataApiService;
        public DataApiAppService(IDataApiService dataApiService)
        {
            _dataApiService = dataApiService;

        }

        /// <summary>
        /// 获取关联了主题的但是没有成功生成api的table清单
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public async Task<List<DataTableEntity>> GetNoApiTables()
        {
            return await _dataApiService.GetNoApiTables();
        }

        /// <summary>
        /// 获取关联了主题的但是没有成功生成api的table清单,并创建api
        /// </summary>
        /// <returns></returns>
        //[AllowAnonymous]
        public async Task<IResult<List<DataApiEntity>>> CreateMappTableApi()
        {
            var list = await _dataApiService.GetNoApiTables();
            return await _dataApiService.CreateMappTableApi(list);
        }

        #region base api
        public async Task<List<RouteInfo>> All()
        {
            return await _dataApiService.All();
        }

        public async Task<List<RouteInfo>> AllFromCache()
        {
            return await _dataApiService.AllFromCache();
        }

        public async Task<int> CountFromCache()
        {
            return await _dataApiService.CountFromCache();
        }

        [HttpGet()]
        [HttpPost()]
        public async Task<int> Count()
        {
            return await _dataApiService.Count();
        }

        [HttpPost("{id}")]
        [HttpPost()]
        public async Task<int> Create(DataApiCreateDto entity)
        {
            var result = await _dataApiService.Save(entity);
            if (entity.Status == ApiState.RELEASE.ToInt().ToString())
            {
                _dataApiService.Register(entity.Id);
            }
            return 1;
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _dataApiService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("/api/dataApi/{id}")]
        [HttpGet("{id}")]
        public async Task<DataApiEntity> Get(string id)
        {
            return await _dataApiService.Get(id);
        }


        /// <summary>
        /// 取消API发布
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/cancel")]
        public async Task<dynamic> Cancel(string id)
        {
            return await _dataApiService.Cancel(new RouteInfo() { Id = id });
        }


        /// <summary>
        /// api发布
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/release")]
        public async Task<dynamic> Release(string id)
        {
            return await _dataApiService.Release(new RouteInfo() { Id = id });
        }

        /// <summary>
        /// 复制API
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/copy")]
        public async Task<DataApiEntity> Copy(string id)
        {
            return await _dataApiService.Copy(id);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("Batch")]
        [HttpDelete("Batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _dataApiService.Delete(ids);
        }

        /// <summary>
        /// 数据API信息表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("detail/{id}")]
        public async Task<dynamic> GetDataApiDetailById(string id)
        {
            return await _dataApiService.GetDataApiDetailById(id);
        }

        /// <summary>
        /// 数据API信息表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("FastSave")]
        public async Task<int> FastSave(DataApiCreateDto model)
        {
            var result = await _dataApiService.Save(model);
            if (model.Status == ApiState.RELEASE.ToInt().ToString())
            {
                await Task.Run(() =>
                  {
                      Task.Delay(2000);
                      _dataApiService.Register(model.Id);
                  });
            }
            return 1;
        }

        /// <summary>
        /// 数据API信息表
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [HttpGet("source/{sourceId}")]
        public async Task<DataApiEntity> GetBySourceId(string sourceId)
        {
            return await _dataApiService.GetBySourceId(sourceId);
        }
        /// <summary>
        /// 数据API信息表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        [HttpGet("sql/parse")]
        public async Task<DataApiEntity> SqlParse(string sql)
        {
            return await _dataApiService.GetBySourceId(sql);
        }
        public async Task<int> SaveByTableId(DataApiEntity entity)
        {
            return await _dataApiService.Modify(entity);
        }



        public async Task<DataApiEntity> Single(DataApiQueryDto entity)
        {
            return await _dataApiService.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(string id, [FromBody] DataApiCreateDto entity)
        {
            if (!id.IsNullOrWhiteSpace()) entity.Id = id;
            var result = await _dataApiService.Save(entity);
            if (entity.Status == ApiState.RELEASE.ToInt().ToString())
            {
                _dataApiService.Register(id);
            }
            return 1;
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(DataApiEntity entity)
        {
            return await _dataApiService.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<DataApiEntity>> Page([FromQuery] DataApiQueryDto filter)
        {
            return await _dataApiService.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<DataApiEntity>> Page2(DataApiQueryDto filter)
        {
            return await _dataApiService.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<DataApiEntity>> Query2([FromQuery] DataApiQueryDto entity)
        {
            return await _dataApiService.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<DataApiEntity>> Query(DataApiQueryDto entity)
        {
            return await _dataApiService.Query(entity);
        }
        #endregion base api



    }
}
