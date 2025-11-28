using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [Route("api/OpenSearch/", Name = "数据资产OpenSearch服务")]
    [ApiDescriptionSettings(GroupName = "数据资产OpenSearch")]
    public class OpenSearchAppService : IDynamicApiController
    {
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataSourceService _dataSourceService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataColumnService _dataColumnService;
        private readonly IDataApiService _dataApiService;
        private readonly IOpenSearchService _openSearchService;
        private readonly IDistributedCacheService _cache;

        public OpenSearchAppService(IDataCatalogService dataCatalogService,
            IDataSourceService dataSourceService,
            IDataTableService dataTableService,
            IDataColumnService dataColumnService,
            IDataApiService dataApiService,
            IOpenSearchService openSearchService,
            IDistributedCacheService cache)
        {
            _cache = cache;
            _dataCatalogService = dataCatalogService;
            _dataSourceService = dataSourceService;
            _dataTableService = dataTableService;
            _dataColumnService = dataColumnService;
            _dataApiService = dataApiService;
            _openSearchService = openSearchService;
        }


        /// <summary>
        /// 初始化dataasset的topic，datasouce，table，api等信息到eslatic
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitDataAssetDocument")]
        public async Task InitDataAssetDocument()
        {
            var topicList = await _dataCatalogService.AllFromCache();
            var dataSourceList = await _dataSourceService.AllFromCache();
            var dataTableList = await _dataTableService.AllFromCache();
            var dataColumnsList = await _dataColumnService.AsQueryable().ToListAsync();
            var apiList = await _dataApiService.AllFromCache();
            var tableIdList = await _dataTableService.GetCategoryMapTableIdList();
            var apiIdList = await _dataTableService.GetCategoryMapApiList();
            var ownerList = await _dataTableService.CurrentDb.Queryable<DataAuthorizeOwnerEntity>().Where(f => f.ObjectType == "table").ToListAsync();
            var tagList = await _dataTableService.CurrentDb.Queryable<MetaDataExtEntity>().Where(f => f.Tag != null && f.Tag != "" && f.ObjectType == "table").ToListAsync();
            //if (extInfo != null)
            //{
            //    entity.NeedSup = extInfo.NeedSup;
            //    entity.Tag = extInfo.Tag;
            //}
            //if (entity.OwnerDepart.IsNullOrWhiteSpace() && entity.OwnerList.Count() > 0)
            //{
            //    entity.OwnerDepart = string.Join(";", entity.OwnerList.Where(f => f.OwnerDept.IsNotNullOrWhiteSpace()).Select(f => f.OwnerDept).Distinct().ToArray());
            //}


            foreach (var topic in topicList)
                await _openSearchService.CreateTopicDocument(topic, topicList.FirstOrDefault(f => f.Id == topic.ParentCtlId));

            foreach (var topic in dataSourceList)
                await _openSearchService.CreateTopicDocument(topic);

            foreach (var topic in dataTableList)
            {
                if (tableIdList.Any(f => f == topic.Id))
                {
                    var data = topic.Adapt<DataTableInput>();
                    data.OwnerList = ownerList.Where(f => f.ObjectId == topic.Id).ToList();
                    data.ColumnList = dataColumnsList.Where(f => f.TableId == topic.Id).ToList().Adapt<List<DataColumnInfo>>();
                    var extInfo = tagList.FirstOrDefault(f => f.Id == data.Id);
                    if (extInfo != null)
                    {
                        data.NeedSup = extInfo.NeedSup;
                        data.Tag = extInfo.Tag;
                    }
                    if (data.OwnerDepart.IsNullOrWhiteSpace() && data.OwnerList.Count() > 0)
                    {
                        data.OwnerDepart = string.Join(";", data.OwnerList.Where(f => f.OwnerDept.IsNotNullOrWhiteSpace()).Select(f => f.OwnerDept).Distinct().ToArray());
                    }
                    await _openSearchService.CreateTopicDocument(data);
                }
            }

            foreach (var topic in apiList)
            {
                if (apiIdList.Any(f => f == topic.Id))
                    await _openSearchService.CreateTopicDocument(topic, dataTableList.FirstOrDefault(f => f.Id == topic.TableId), dataSourceList.FirstOrDefault(f => f.Id == topic.SourceId), topicList.FirstOrDefault(f => f.Id == topic.CtlId));
            }
        }


//        [HttpPost("CreateTopic")]
//        public async Task<string> CreateTopicDocument(DataCatalogEntity item, DataCatalogEntity parent)
//        {
//            var keyword = new List<string>() { item.Code, item.Name, item.Id };
//            var data = new SearchTopicDto()
//            {
//                id = item.Id,
//                topic = "DataAsset_Catalog",
//                title = $"{item.Name} {item.Code}",
//                content = "",
//                description = $"<table class=\"table table-border\"><tr><td><label>Topic Name:</label>{item.Name}</td><td><label>English Name:</label>{item.Code}</td></tr>" +
//                $"<tr><td><label>Parent Subject:</label>{parent?.Name}</td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
//                $"<tr><td colspan='2'>Remark:</label>{item.Remark}</td></tr>" +
//                "</table>",
//                linkUrl = "",//$"/#/dataAsset/topicDomainDefinition?id={item.Id}&type=link&edit=0&fullscreen=true",
//                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
//                Operator = item.CreateBy,
//                enableDataSovereignty = false,
//            };
//            var result = await _topicDocumentProxyService.CreateAsync(data);
//            if (result.Success)
//                return result.Data;
//            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
//        }

//        [HttpPost("CreateDataSource")]
//        public async Task<string> CreateTopicDocument(DataSourceEntity item)
//        {
//            var dbTypeName = DbSchema.GetDbType(item.DbType);
//            var keyword = new List<string>() { item.SourceName, item.DbSchema.Host, item.DbSchema.DbName, dbTypeName.ToString(), item.Id };
//            var data = new SearchTopicDto()
//            {
//                id = item.Id,
//                topic = "DataAsset_DataSource",
//                title = $"{item.SourceName} {item.DbSchema.DbName}",
//                content = "",
//                description = $"<table class=\"table table-border\"><tr><td><label>Source Name:</label>{item.SourceName}</td><td><label>DB Name:</label>{item.DbSchema.DbName}</td><td><label>DB Type:</label>{dbTypeName}</td></tr>" +
//                $"<tr><td><label>DB Type:</label>{item.DbSchema.Host}</td><td></td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
//                $"<tr><td colspan='3'>Remark:</label>{item.Remark}</td></tr>" +
//                "</table>",
//                linkUrl = "",//"",// $"/#/dataAsset/dataSource?id={item.Id}&type=link&edit=0&fullscreen=true",
//                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
//                Operator = item.CreateBy,
//                enableDataSovereignty = false,
//            };
//            var result = await _topicDocumentProxyService.CreateAsync(data);
//            if (result.Success)
//                return result.Data;
//            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
//        }

//        [HttpPost("CreateDataTable")]
//        public async Task<string> CreateTopicDocument(DataTableInfo item, List<DataColumnEntity> columns)
//        {
//            //var dbTypeName = DbSchema.GetDbType(item.DbType);
//            var keyword = new List<string>() { item.Alias, item.TableComment, item.TableName, item.SourceName, item.SourceId, item.Id };
//            var data = new SearchTopicDto()
//            {
//                id = item.Id,
//                topic = "DataAsset_DataTable",
//                title = $" {item.TableComment} {item.TableName}",
//                content = "",
//                description = $"<table class=\"table table-border\"><tr><td><label>Table Name: </label>{item.TableName}</td><td><label>Data Source:</label>{item.SourceName}</td><td><label>Alias Name:</label>{item.Alias}</td></tr>" +
//                $"<tr><td><label>Owner:</label>{item.OwnerName}</td><td><label>Dept:</label>{item.OwnerDept}</td><td><label>Previewable:</label>{(item.Reviewable == 1 ? "Yes" : "No")}</td></tr>" +
//                $"<tr><td><label>Update Method:</label>{item.UpdateMethod}</td><td><label>Update Frequency:</label>{item.UpdateFrequency}</td><td><label>Time Range:</label>{(item.DataTimeRange)}</td></tr>" +
//                $"<tr><td><label>Topic:</label>{item.CtlName}</td><td><label>Tag:</label>{item.Tag}</td><td><label>Level:</label>{(item.LevelName)}</td></tr>" +
//                $"<tr><td colspan='3'>Table Comment:</label>{item.TableComment}</td></tr>" +
//                $"<tr><td colspan='3'>{GetColumnHtml(columns)}</td></tr>" +
//                "</table>",
//                linkUrl = "",//$"/#/dataAsset/dataTable?id={item.Id}&type=link&edit=0&fullscreen=true",
//                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
//                Operator = item.CreateBy,
//                enableDataSovereignty = false,
//            };
//            var result = await _topicDocumentProxyService.CreateAsync(data);
//            if (result.Success)
//                return result.Data;
//            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
//        }

//        private string GetColumnHtml(List<DataColumnEntity> columns)
//        {
//            if (columns == null) return string.Empty;
//            StringBuilder sb = new StringBuilder();
//            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Field Name</th><th>Field Comment</th><th>Security Level</th><th>Master Data Type</th><th>Standardized</th><th>Primary Key</th><th>Nullable</th><th>Data Type</th><th>Data Length</th><th>Requied As Condition</th></tr>");
//            foreach (var item in columns)
//            {
//                sb.Append(@$"<tr><td>{item.ColName}</td><td>{item.ColComment}</td><td>{item.LevelName}</td><td>{item.MasterdataType}</td>
//<td>{item.Standardized}</td><td>{(item.ColKey == "1" ? "Yes" : "No")}</td><td>{(item.Nullable == "1" ? "Yes" : "No")}</td>
//<td>{item.DataType}</td><td>{item.DataLength}</td><td>{item.RequiredAsCondition}</td></tr>");
//            }
//            sb.Append("</table>");
//            return sb.ToString();
//        }


//        [HttpPost("CreateDataApi")]
//        public async Task<string> CreateTopicDocument(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic)
//        {
//            var dbTypeName = DbSchema.GetDbType(dataSource?.DbType);
//            var keyword = new List<string>() { item.ApiName, item.ApiServiceUrl,item.Id,
//                 topic?.Code, topic?.Name,
//                 dataSource?.SourceName, dataSource?.DbSchema.Host, dataSource?.DbSchema.DbName, dbTypeName.ToString(),
//                  tableInfo?.Alias, tableInfo?.TableComment, tableInfo?.TableName
//                };
//            var data = new SearchTopicDto()
//            {
//                id = item.Id,
//                topic = "DataAsset_DataApi",
//                title = $"{item.ApiName} {item.ApiVersion} {item.ApiServiceUrl}",
//                content = "",
//                description = $"<table class=\"table table-border\"><tr><td colspan='3'><label>API Name:</label>{item.ApiName} <span>{item.ApiVersion}</span></td></tr>" +
//                $"<tr class='copyTitle' title='{HostUrl + item.ApiServiceUrl}'><td colspan='3'><label>API URL:<span>[{item.ReqMethod}]</span>{HostUrl+item.ApiServiceUrl}<span></span></td></tr>" +
//                $"<tr><td><label>Page Size Limit:</label>{item.ExecuteConfig.pageSizeLimit ?? 1000}</td><td><label>Limit Times:</label>{item.RateLimit.times}/{item.RateLimit.seconds}</td><td></td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
//                $"<tr><td colspan='3'>Remark:</label>{item.Remark}</td></tr>" +
//                   $"<tr><td colspan='3'>{GetColumnHtml(item.ReqParams)}</td></tr>" +
//                   $"<tr><td colspan='3'>{GetColumnHtml(item.ResParams)}</td></tr>" +
//                "</table>",
//                linkUrl = "",//$"/#/dataAsset/api?id={item.Id}&type=link&edit=0&fullscreen=true",
//                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
//                Operator = tableInfo?.OwnerName,
//                enableDataSovereignty = false,
//            };
//            var result = await _topicDocumentProxyService.CreateAsync(data);
//            if (result.Success)
//                return result.Data;
//            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
//        }

//        private string GetColumnHtml(List<ReqParam> columns)
//        {
//            if (columns == null) return string.Empty;
//            StringBuilder sb = new StringBuilder();
//            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Param Name</th><th>Param Comment</th><th>Where Type</th><th>Param Type</th><th>Example Value</th><th>Default Value</th>");
//            foreach (var item in columns)
//            {
//                sb.Append(@$"<tr><td>{item.paramName}</td><td>{item.paramComment}</td><td>{item.whereType}</td><td>{item.paramType}</td>
//<td>{item.exampleValue}</td><td>{item.defaultValue}</td></tr>");
//            }
//            sb.Append("</table>");
//            return sb.ToString();
//        }
//        private string GetColumnHtml(List<ResParam> columns)
//        {
//            if (columns == null) return string.Empty;
//            StringBuilder sb = new StringBuilder();
//            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Field Name</th><th>Field Comment</th><th>Field Alias</th><th>Data Type</th><th>Example Value</th>");
//            foreach (var item in columns)
//            {
//                sb.Append(@$"<tr><td>{item.fieldName}</td><td>{item.fieldComment}</td><td>{item.fieldAliasName}</td><td>{item.dataType}</td><td>{item.exampleValue}</td></tr>");
//            }
//            sb.Append("</table>");
//            return sb.ToString();
//        }

    }
}
