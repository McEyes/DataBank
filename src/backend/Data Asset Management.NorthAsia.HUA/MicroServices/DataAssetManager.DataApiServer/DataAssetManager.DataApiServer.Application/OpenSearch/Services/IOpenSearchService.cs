using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Dtos;

using ITPortal.Core;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;

namespace DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services
{
    public interface IOpenSearchService
    {
        Task<string> CreateTopicDocument(DataCatalogEntity item, DataCatalogEntity parent);
        Task<string> CreateTopicDocument(DataSourceEntity item);
        Task<string> CreateTopicDocument(DataTableInput item);//, List<DataColumnEntity> columns
        Task<string> CreateTopicDocument(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic);
        Task DeleteTopicDocumentAsync(dynamic id);
        Task UpdateTopicDocumentAsync(DataCatalogEntity item, DataCatalogEntity parent);
        Task UpdateTopicDocumentAsync(DataSourceEntity item);
        Task UpdateTopicDocumentAsync(DataTableInput item);//, List<DataColumnEntity> columns
        Task UpdateTopicDocumentAsync(RouteInfo item, DataTableInfo tableInfo, DataSourceEntity dataSource, DataCatalogEntity topic);
    }
}
