using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using System.Text;

namespace DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services
{
    public class OpenSearchService : IOpenSearchService, ITransient
    {
        //private readonly EmployeeProxyService _employeeProxyService; EmployeeProxyService employeeProxyService,
        private readonly ILogger _logger;
        private readonly TopicDocumentProxyService _topicDocumentProxyService;
        private readonly string HostUrl;
        public OpenSearchService(TopicDocumentProxyService topicDocumentProxyService, ILogger<EmployeeBaseInfoService> logger,
            IDistributedCacheService cache) 
        {
            _logger = logger;
            _topicDocumentProxyService = topicDocumentProxyService;
            HostUrl = App.GetConfig<string>("RemoteApi:DataAssetHostUrl");
        }
        public async Task DeleteTopicDocumentAsync(dynamic id)
        {
            var result = await _topicDocumentProxyService.DeleteAsync(id);
            if (!result.Success) throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }

        #region DataCatalogEntity

        public async Task<string> CreateTopicDocument(DataCatalogEntity item, DataCatalogEntity parent)
        {
            var result = await _topicDocumentProxyService.CreateAsync(ConvertToCatalog(item, parent));
            if (result.Success)
                return result.Data;
            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        public async Task UpdateTopicDocumentAsync(DataCatalogEntity item, DataCatalogEntity parent)
        {
            var result = await _topicDocumentProxyService.UpdateAsync(ConvertToCatalog(item, parent));
            if (!result.Success) throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        private SearchTopicDto ConvertToCatalog(DataCatalogEntity item, DataCatalogEntity parent)
        {
            var keyword = new List<string>() { item.Code, item.Name, item.Id };
            return new SearchTopicDto()
            {
                id = item.Id,
                topic = "DataAsset_Catalog",
                title = $"{item.Name} {item.Code}",
                content = "",
                description = $"<table class=\"table table-border\"><tr><td><label>Topic Name:</label>{item.Name}</td><td><label>English Name:</label>{item.Code}</td></tr>" +
                $"<tr><td><label>Parent Subject:</label>{parent?.Name}</td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
                $"<tr><td colspan='2'><label>Remark:</label>{item.Remark}</td></tr>" +
                "</table>",
                linkUrl = "",//$"/#/dataAsset/topicDomainDefinition?id={item.Id}&type=link&edit=0&fullscreen=true",
                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
                Operator = item.CreateBy,
                enableDataSovereignty = false,
            };
        }
        #endregion DataCatalogEntity

        #region DataSourceEntity
        public async Task<string> CreateTopicDocument(DataSourceEntity item)
        {
           
            var result = await _topicDocumentProxyService.CreateAsync(ConvertToCatalog(item));
            if (result.Success)
                return result.Data;
            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        public async Task UpdateTopicDocumentAsync(DataSourceEntity item)
        {
            var result = await _topicDocumentProxyService.UpdateAsync(ConvertToCatalog(item));
            if (!result.Success) throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }

        private SearchTopicDto ConvertToCatalog(DataSourceEntity item)
        {
            var dbTypeName = DbSchema.GetDbType(item.DbType);
            var keyword = new List<string>() { item.SourceName, item.DbSchema.Host, item.DbSchema.DbName, dbTypeName.ToString(), item.Id };
            var data = new SearchTopicDto()
            {
                id = item.Id,
                topic = "DataAsset_DataSource",
                title = $"{item.SourceName} {item.DbSchema.DbName}",
                content = "",
                description = $"<table class=\"table table-border\"><tr><td><label>Source Name:</label>{item.SourceName}</td><td><label>DB Name:</label>{item.DbSchema.DbName}</td><td><label>DB Type:</label>{dbTypeName}</td></tr>" +
                $"<tr><td><label>DB Type:</label>{item.DbSchema.Host}</td><td></td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
                $"<tr><td colspan='3'><label>Remark:</label>{item.Remark}</td></tr>" +
                "</table>",
                linkUrl = "",//"",// $"/#/dataAsset/dataSource?id={item.Id}&type=link&edit=0&fullscreen=true",
                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
                Operator = item.CreateBy,
                enableDataSovereignty = false,
            };
            return data;
        }

        #endregion DataSourceEntity

        #region DataTableInfo

        public async Task<string> CreateTopicDocument(DataTableInput item)//, List<DataColumnEntity> columns
        {
            var result = await _topicDocumentProxyService.CreateAsync(ConvertToCatalog(item));//, columns
            if (result.Success)
                return result.Data;
            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        public async Task UpdateTopicDocumentAsync(DataTableInput item)
        {
            var result = await _topicDocumentProxyService.UpdateAsync(ConvertToCatalog(item));//, columns
            if (!result.Success) throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        private SearchTopicDto ConvertToCatalog(DataTableInput item)
        {
            //var dbTypeName = DbSchema.GetDbType(item.DbType);
            var keyword = new List<string>() { item.Alias, item.TableComment, item.TableName, item.SourceName, item.Id, item.Tag };
            var data = new SearchTopicDto()
            {
                id = item.Id,
                topic = "DataAsset_DataTable",
                title = $" {item.TableComment} {item.TableName}",
                content = "",
                description = $"<table class=\"table table-border\"><tr><td><label>Table Name: </label>{item.TableName}</td><td><label>Data Source:</label>{item.SourceName}</td><td><label>Alias Name:</label>{item.Alias}</td></tr>" +
                $"<tr><td><label>Owner:</label>{string.Join(";", item.OwnerList?.Select(f => f.OwnerName))}</td><td><label>Dept:</label>{item.OwnerDepart}</td><td><label>Previewable:</label>{(item.Reviewable == 1 ? "Yes" : "No")}</td></tr>" +
                $"<tr><td><label>Update Method:</label>{item.UpdateMethod}</td><td><label>Update Frequency:</label>{item.UpdateFrequency}</td><td><label>Time Range:</label>{(item.DataTimeRange)}</td></tr>" +
                $"<tr><td><label>Topic:</label>{item.CtlName}</td><td><label>Tag:</label>{item.Tag}</td><td><label>Level:</label>{(item.LevelName)}</td></tr>" +
                $"<tr><td colspan='2'><label>Table Comment:</label>{item.TableComment}</td><td><label>Need approval from superiors:</label>{(item.NeedSup == 1 ? "Yes" : "No")}</td></tr>" +
                $"<tr><td colspan='3'>{GetColumnHtml(item.ColumnList)}</td></tr>" +
                "</table>",
                linkUrl = "",//$"/#/dataAsset/dataTable?id={item.Id}&type=link&edit=0&fullscreen=true",
                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
                Operator = item.CreateBy,
                enableDataSovereignty = false,
            };
            return data;
        }

        private string GetColumnHtml(List<DataColumnInfo> columns)
        {
            if (columns == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Field Name</th><th>Field Comment</th><th>Security Level</th><th>Master Data Type</th><th>Standardized</th><th>Primary Key</th><th>Nullable</th><th>Data Type</th><th>Data Length</th><th>Requied As Condition</th></tr>");
            foreach (var item in columns)
            {
                sb.Append(@$"<tr><td>{item.ColName}</td><td>{item.ColComment}</td><td>{item.LevelName}</td><td>{item.MasterdataType}</td>
<td>{item.Standardized}</td><td>{(item.ColKey == "1" ? "Yes" : "No")}</td><td>{((item.Nullable == "1"|| item.Nullable == "True") ? "Yes" : "No")}</td>
<td>{item.DataType}</td><td>{item.DataLength}</td><td>{item.RequiredAsCondition}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        private string GetColumnHtml(List<DataColumnEntity> columns)
        {
            if (columns == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Field Name</th><th>Field Comment</th><th>Security Level</th><th>Master Data Type</th><th>Standardized</th><th>Primary Key</th><th>Nullable</th><th>Data Type</th><th>Data Length</th><th>Requied As Condition</th></tr>");
            foreach (var item in columns)
            {
                sb.Append(@$"<tr><td>{item.ColName}</td><td>{item.ColComment}</td><td>{item.LevelName}</td><td>{item.MasterdataType}</td>
<td>{item.Standardized}</td><td>{(item.ColKey == "1" ? "Yes" : "No")}</td><td>{((item.Nullable == "1"|| item.Nullable == "True") ? "Yes" : "No")}</td>
<td>{item.DataType}</td><td>{item.DataLength}</td><td>{item.RequiredAsCondition}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        #endregion DataTableInfo

        #region RouteInfo

        public async Task<string> CreateTopicDocument(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic)
        {
            var result = await _topicDocumentProxyService.CreateAsync(ConvertToCatalog(item, tableInfo, dataSource, topic));
            if (result.Success)
                return result.Data;
            else throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        public async Task UpdateTopicDocumentAsync(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic)
        {
            var result = await _topicDocumentProxyService.UpdateAsync(ConvertToCatalog(item, tableInfo, dataSource, topic));
            if (!result.Success) throw new AppFriendlyException(result.Msg?.ToString() ?? "fail", result.Code);
        }
        private SearchTopicDto ConvertToCatalog(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic)
        {

            var dbTypeName = DbSchema.GetDbType(dataSource?.DbType);
            var keyword = new List<string>() { item.ApiName, item.ApiServiceUrl,item.Id,
                 topic?.Code, topic?.Name,
                 dataSource?.SourceName, dataSource?.DbSchema.Host, dataSource?.DbSchema.DbName, dbTypeName.ToString(),
                  tableInfo?.Alias, tableInfo?.TableComment, tableInfo?.TableName
                };
            var data = new SearchTopicDto()
            {
                id = item.Id,
                topic = "DataAsset_DataApi",
                title = $"{item.ApiName} {item.ApiVersion} {item.ApiServiceUrl}",
                content = "",
                description = $"<table class=\"table table-border\"><tr><td colspan='2'><label>API Name:</label>{item.ApiName}</td><td><label>Version:</label>{item.ApiVersion}</td></tr>" +
                $"<tr ><td colspan='3'><label>Table Comment:</label>{tableInfo?.TableComment}</td></tr>" +
                $"<tr class='copyTitle' title='{HostUrl + item.ApiServiceUrl}'><td colspan='3'><label>API URL:</label><span class='{item.ReqMethod.ToLower()}-item'>[{item.ReqMethod}]</span>{HostUrl + item.ApiServiceUrl}<span></span></td></tr>" +
                $"<tr><td><label>Page Size Limit:</label>{item.ExecuteConfig.pageSizeLimit ?? 5000}</td><td><label>Limit Times:</label>{item.RateLimit.times}/{item.RateLimit.seconds}</td><td></td><td></td></tr>" +//<label>Status:</label>{(item.Status == 1 ? "Enable" : "Disable")}
                $"<tr><td colspan='3'><label>Remark:</label>{item.Remark}</td></tr>" +
                   $"<tr><td colspan='3'>{GetColumnHtml(item.ReqParams)}</td></tr>" +
                   $"<tr><td colspan='3'>{GetColumnHtml(item.ResParams)}</td></tr>" +
                "</table>",
                linkUrl = "",//$"/#/dataAsset/api?id={item.Id}&type=link&edit=0&fullscreen=true",
                keyword = keyword.Where(f => f.IsNotNullOrWhiteSpace()).ToList(),
                Operator = tableInfo?.OwnerName,
                enableDataSovereignty = false,
            };
            return data;
        }


        private string GetColumnHtml(List<ReqParam> columns)
        {
            if (columns == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Param Name</th><th>Param Comment</th><th>Where Type</th><th>Param Type</th><th>Example Value</th><th>Default Value</th>");
            foreach (var item in columns)
            {
                sb.Append(@$"<tr><td>{item.paramName}</td><td>{item.paramComment}</td><td>{item.whereType}</td><td>{item.paramType}</td>
<td>{item.exampleValue}</td><td>{item.defaultValue}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
        private string GetColumnHtml(List<ResParam> columns)
        {
            if (columns == null) return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append($"<table class=\"table table-border\"><tr style=\"font-weight:'bold';\"><th>Field Name</th><th>Field Comment</th><th>Field Alias</th><th>Data Type</th><th>Example Value</th>");
            foreach (var item in columns)
            {
                sb.Append(@$"<tr><td>{item.fieldName}</td><td>{item.fieldComment}</td><td>{item.fieldAliasName}</td><td>{item.dataType}</td><td>{item.exampleValue}</td></tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }
        #endregion RouteInfo
    }
}
