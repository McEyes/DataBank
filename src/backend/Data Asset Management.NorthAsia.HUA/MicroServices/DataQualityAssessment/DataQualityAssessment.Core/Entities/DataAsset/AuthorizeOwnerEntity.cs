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
    [Table("metadata_authorize_owner")]
    public class AuthorizeOwnerEntity : EntityBase<string>
    {
        [Column("id")]
        public override string Id { get => base.Id; set => base.Id = value; }
        public string object_id { get; set; }
        public string object_type { get; set; }
        public string owner_id { get; set; }
        public string owner_dept { get; set; }
        public string owner_name { get; set; }
    }
}
