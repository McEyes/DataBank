using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataQualityAssessment.Core.Entities.DataAsset
{
    [Table("metadata_catalog_table_mapping")]
    public class MetadataCatalogTableMappingEntity : EntityBase<string>, IEntityTypeBuilder<MetadataCatalogTableMappingEntity>
    {
        public override string Id => $"{catalog_id}_{metadata_table_id}";

        [Key]
        public string catalog_id { get; set; }

        [Key]
        public string metadata_table_id { get; set; }

        public void Configure(EntityTypeBuilder<MetadataCatalogTableMappingEntity> entityBuilder, DbContext dbContext, Type dbContextLocator)
        {
            entityBuilder.HasKey(u => new { u.catalog_id,u.metadata_table_id });
        }
    }
}
