using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class AssetDocDto : PageEntity<Guid>
    {

        public  Guid? Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? DocVer { get; set; }
        /// <summary>
        /// approver
        /// </summary>
        public string DisplayNameCn { get; set; }
        /// <summary>
        /// approver
        /// </summary>
        public string DisplayNameEn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Catalog { get; set; }
        public string DocIconUrl { get; set; } = "icon_doc_global_zh.png";
    }

}
