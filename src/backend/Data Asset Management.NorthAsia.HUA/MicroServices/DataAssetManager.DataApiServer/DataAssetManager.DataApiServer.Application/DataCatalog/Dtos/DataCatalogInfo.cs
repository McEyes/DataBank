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
    public class DataCatalogInfo : PageEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? CtlId { get{ return Id; } set { Id = value; } }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? ParentCtlId { get; set; }
        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string? ParentName { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTimeOffset? UpdateTime { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }

        public List<DataTableInfo> Tables { get; set; } 
    }

}
