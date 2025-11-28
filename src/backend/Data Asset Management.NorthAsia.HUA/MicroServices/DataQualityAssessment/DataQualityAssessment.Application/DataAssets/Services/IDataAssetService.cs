using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Application.DataAssets.Services
{
    public interface IDataAssetService
    {
        Task<(MetadataSourceModel source, MetadataTableModel table, List<MetadataColumnModel> columns)> GetDataAssetAsync(string tableId);
        List<string> GetValidTableIds();
        List<MetadataAuthorizeOwnerModel> GetMetadataAuthorizeOwners(string tableId);
        Task SyncScoreAsync();
    }
}
