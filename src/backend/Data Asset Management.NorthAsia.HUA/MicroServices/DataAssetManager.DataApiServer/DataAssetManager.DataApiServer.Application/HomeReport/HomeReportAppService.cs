using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Excel;

using MiniExcelLibs;

using System.Collections.Generic;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [Route("api/HomeReport/", Name = "数据资产HomeReport服务")]
    [ApiDescriptionSettings(GroupName = "数据资产HomeReport")]
    public class HomeReportAppService : IDynamicApiController
    {
        private readonly IDataApiLogService _dataApiLogService;
        //private readonly IDataApiService _dataApiService;
        private readonly IDistributedCacheService _cache;
        private readonly IDataTableService _dataTableService;

        public HomeReportAppService(IDataTableService dataTableService, IDataApiLogService dataApilogService, IDistributedCacheService cache)
        {
            _cache = cache;
            _dataApiLogService = dataApilogService;
            _dataTableService = dataTableService;
        }

        /// <summary>
        /// 类型数据统计
        /// 十秒钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCategoryStats")]
        public async Task<dynamic> GetCategoryStatistics()
        {
            return await Task.FromResult(new
            {
                TopicCount = await _dataTableService.GetCount<DataCatalogEntity>(f => f.Status == 1),
                DataSourceCount = await _dataTableService.GetCount<DataSourceEntity>(f => f.Status == 1),
                TableCount = await _dataTableService.GetCategoryMapTableCount(),//.GetCount<DataTableEntity>(f => f.Status == 1),
                ColumnCount = await _dataTableService.GetCategoryMapColumnCount(),//.GetCount<DataColumnEntity>(),
                ApiCount = await _dataTableService.GetCategoryMapApiCount(),//.GetCount<DataApiEntity>(f => f.Status == 2),
                ApiVisited = await _dataTableService.GetCount<DataApiLogEntity>(f => f.CallerDate > DateTime.Now.AddMonths(-1)),
            });//
        }

        /// <summary>
        /// 数据调用统计
        /// 十分钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTableStats")]
        public async Task<List<TableApiVisitStatics>> GetTableStatistics()
        {
            return await _dataTableService.GetTableStatistics();
        }



        /// <summary>
        /// 数据调用统计
        /// 十分钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("DownTableStats"), NonUnify]
        public async Task<IActionResult> DownTableStatistics()
        {
            var list= await _dataTableService.GetTableStatistics();
            return ExcelExporter.ExportExcel(list, "TableStatistics");
        }

        /// <summary>
        /// 数据标准化统计
        /// 一分钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStandardizationStats")]
        [HttpGet("GetStandardizationStats/{dept}")]
        public async Task<List<TableStandardizedRateEntity>> GetStandardizationStatistics(string dept)
        {
            return await _dataTableService.GetStandardizationStatistics(dept);
        }


        /// <summary>
        /// 数据标准化统计
        /// 一分钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("DownStandardizationStats"), NonUnify]
        [HttpGet("DownStandardizationStats/{dept}")]
        public async Task<IActionResult> DownStandardizationStatistics(string dept)
        {
            var list = await _dataTableService.GetStandardizationStatistics(dept);
            return ExcelExporter.ExportExcel(list, "StandardizationStatistics");
        }

        /// <summary>
        /// 数据标准化统计
        /// 一分钟更新一次数据
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Export/Excel/Demo1"), NonUnify]
        public async Task<IActionResult> ExportDemo()
        {
            var list = await _dataTableService.GetTableStatistics();
            return ExcelExporter.Export(list, "TableStatistics",
                columnConfig: new List<MiniExcelLibs.Attributes.DynamicExcelColumn>() {
                    new MiniExcelLibs.Attributes.DynamicExcelColumn("TableCode"){   Name="表真实名称",Index=3},
                    new MiniExcelLibs.Attributes.DynamicExcelColumn("TableName"){   Name="表名"},
                    new MiniExcelLibs.Attributes.DynamicExcelColumn("Dept"){   Name="所属部门"},
                    new MiniExcelLibs.Attributes.DynamicExcelColumn("Count"){   Name="访问量" },
                });
        }



        /// <summary>
        /// api，服务每日调用频率
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetApiDailyStats")]
        public async Task<List<DayVisitedDto>> GetApiDailyStats(int day = 14)
        {
            var sDay = -Math.Abs(day);
            var fun = async () =>
            {
                var day = DateTime.Now.Date.AddDays(sDay);
                return await _dataApiLogService.GetApiDailyStats(day);
            };
            return await _cache.GetObjectAsync(_dataTableService.GetRedisKey<TableStandardizedRateEntity>("ApiDailyStats"), fun, TimeSpan.FromSeconds(10));
        }



    }
}
