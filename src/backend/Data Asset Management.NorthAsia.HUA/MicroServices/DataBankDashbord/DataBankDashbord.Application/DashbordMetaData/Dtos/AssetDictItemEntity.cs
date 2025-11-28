using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{


    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    [SugarTable("asset_dict_item")]
    public class AssetDictItemEntity
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
        public string? update_by { get; set; }



        /// <summary>
        /// 
        /// </summary>
        public string? remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? dict_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? item_text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? item_value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? item_sort { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? item_data { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string? item_text_en { get; set; }

    }
}
