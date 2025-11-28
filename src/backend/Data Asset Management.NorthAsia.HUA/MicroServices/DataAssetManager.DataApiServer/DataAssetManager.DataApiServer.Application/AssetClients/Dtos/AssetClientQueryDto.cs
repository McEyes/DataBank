using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Core;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class AssetClientQueryDto : PageEntity<Guid>
    {

        /// <summary>
        /// 申请类型：1 Individual个人,2 Application应用程序
        /// </summary>
        public int? ClientType { get; set; }

        /// <summary>
        /// client code,个人是保存的是申请人id(=owner)，应用程序是新id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// client code
        /// </summary>
        public string ClientName { get; set; }
        ///// <summary>
        ///// client code
        ///// </summary>
        //public string Description { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool? Enabled { get; set; }
        /// <summary>
        /// 拥有人
        /// </summary>
        public string? Owner { get; set; }
        /// <summary>
        /// SME
        /// </summary>
        public string? Sme { get; set; }
        public string BelongArea { get; set; }
        ///// <summary>
        ///// 拥有人姓名
        ///// </summary>
        //public string? OwnerName { get; set; }

        /// <summary>
        /// 拥有人部门
        /// </summary>
        public string? OwnerDept { get; set; }

        ///// <summary>
        ///// 拥有人部门
        ///// </summary>
        //public string? OwnerNtid { get; set; }

    }

}
