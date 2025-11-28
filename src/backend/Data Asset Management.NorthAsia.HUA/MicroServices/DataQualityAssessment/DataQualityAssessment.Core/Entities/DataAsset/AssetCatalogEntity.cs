using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Enums;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.Core.Entities.DataAsset
{
    [Table("asset_catalog")]
    public class AssetCatalogEntity : EntityBase<string>
    {
        [Column("ctl_id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public int status { get; set; }
    }
}
