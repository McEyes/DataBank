using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using Furion.JsonSerialization;
using Furion.Logging;
using System.Drawing.Printing;
using DataAssetManager.DataTableServer.Application;
using StackExchange.Profiling.Internal;

namespace DataAssetManager.DataApiServer.Application.DataAsset
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/access/", Name = "数据资产系统服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Api")]
    public class DataAccessAppService : IDynamicApiController
    {
        private readonly IDataAccessService _dataAccessService;
        private readonly IDataCatalogService _dataCatalogService;
        public DataAccessAppService(IDataAccessService dataApiService, IDataCatalogService dDataCatalogService)
        {
            _dataAccessService = dataApiService;
            _dataCatalogService = dDataCatalogService; 
        }

        [HttpGet("dataPreview/{tableId}")]
        public async Task<dynamic> GetDataPreview(string tableId, int pageSize = 20, string sort = "")
        {
            if (string.IsNullOrWhiteSpace(tableId))
            {
                throw new DataException("Parameter [tableId] cannot be empty");
            }
            return await _dataAccessService.GetDataPreview(tableId, pageSize, sort);
        }


        [HttpGet("/api/sql/run")]
        [HttpPost("/api/sql/run")]
        public async Task<dynamic> SqlRun(SqlDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SqlText))
            {
                throw new DataException("Parameter [SqlText] cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(dto.SourceId))
            {
                throw new DataException("Parameter [SourceId] cannot be empty");
            }
            return await _dataAccessService.SqlRun(dto);
        }
    }
}
