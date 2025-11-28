using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [SugarTable("metadata_authorize_owner")]
    public class MetadataAuthorizeOwnerEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? object_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? owner_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? object_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? owner_name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? owner_dept { get; set; }

    }
}
