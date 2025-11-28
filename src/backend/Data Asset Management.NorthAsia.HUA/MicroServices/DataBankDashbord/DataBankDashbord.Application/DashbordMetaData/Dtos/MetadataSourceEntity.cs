using Furion.InstantMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{

    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("metadata_source")]
    public class MetadataSourceEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? create_by { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string? create_org { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? update_by { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string? remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? db_type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? source_name { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string? db_schema { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string? system_name { get; set; }

        /// <summary>
        /// 库编码
        /// </summary>
        public string? source_code { get; set; }




    }
}
