using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.DbContextLocators;
using DataQualityAssessment.Core.Entities;
using DataQualityAssessment.Core.Entities.DataAsset;
using DataQualityAssessment.Core.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace DataQualityAssessment.Application.DataAssets.Services
{
    public class DataAssetService : IDataAssetService, ITransient
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DataAssetService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<(MetadataSourceModel source, MetadataTableModel table, List<MetadataColumnModel> columns)> GetDataAssetAsync(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var sourceRepository = Db.GetRepository<SourceEntity, DataAssetDbContextLocator>(serviceProvider);
                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serviceProvider);
                var columnRepository = Db.GetRepository<ColumnEntity, DataAssetDbContextLocator>(serviceProvider);

                var tableEntity = await tableRepository.FirstOrDefaultAsync(t => t.Id == tableId);
                ArgumentNullException.ThrowIfNull(tableEntity, $"tableEntity tableId={tableId}");
                var sourceEntity = await sourceRepository.FirstOrDefaultAsync(t => t.Id == tableEntity.source_id);
                ArgumentNullException.ThrowIfNull(sourceEntity, $"sourceEntity source_id={tableEntity.source_id}");
                var columnData = columnRepository.Where(t => t.table_id == tableId).Select(t => t).ToList();
                ArgumentNullException.ThrowIfNull(columnData, "columnData");

                return (sourceEntity.Adapt<MetadataSourceModel>(), tableEntity.Adapt<MetadataTableModel>(), columnData.Adapt<List<MetadataColumnModel>>());
            }

        }

        public List<MetadataAuthorizeOwnerModel> GetMetadataAuthorizeOwners(string tableId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var authorizeOwnerRepository = Db.GetRepository<AuthorizeOwnerEntity, DataAssetDbContextLocator>(serviceProvider);
                var authorizeOwnerData = authorizeOwnerRepository.Where(t => t.object_id == tableId && t.object_type == "table").Select(t => t).ToList();

                return authorizeOwnerData?.Adapt<List<MetadataAuthorizeOwnerModel>>();
            }
        }

        public async Task SyncScoreAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serviceProvider);
                var assessmentResultRepository = Db.GetRepository<AssessmentResultEntity>(serviceProvider);
                var results = assessmentResultRepository.Where(t => t.Status == Core.Enums.ProgressStatus.Finished);

                foreach (var item in results)
                {
                    TableEntity entity = new TableEntity
                    {
                        Id = item.Id,
                        quality_score = (int)(item.Score * 100),
                        last_score = item.LastScore == null ? null : (int)(item.LastScore * 100)
                    };

                    await tableRepository.UpdateIncludeAsync(entity, "quality_score,last_score".Split(","));
                }

                await tableRepository.SaveNowAsync();
            }
        }

        public List<string> GetValidTableIds()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var tableRepository = Db.GetRepository<TableEntity, DataAssetDbContextLocator>(serviceProvider);
                var assetCatalogRepository = Db.GetRepository<AssetCatalogEntity, DataAssetDbContextLocator>(serviceProvider);
                var metadataCatalogTableMappingRepository = Db.GetRepository<MetadataCatalogTableMappingEntity, DataAssetDbContextLocator>(serviceProvider);

                var queryTable = tableRepository.AsQueryable();
                var queryAssetCatalog = assetCatalogRepository.AsQueryable();
                var queryMetadataCatalogTableMapping = metadataCatalogTableMappingRepository.AsQueryable();

                var tableIds = queryTable.Where(t => t.status != 0 && t.status != null)
                    .Join(queryMetadataCatalogTableMapping, a => a.Id, a => a.metadata_table_id, (a, b) => new
                    {
                        a.Id,
                        a.status,
                        b.catalog_id,
                    }).Join(queryAssetCatalog.Where(t=>t.status == 1), a => a.catalog_id, a => a.Id, (a, b) => a.Id)
                    
                    .Select(t => t).Distinct().ToList();

                return tableIds;
            }
        }
    }
}
