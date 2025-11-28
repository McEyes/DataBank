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
    public class DataAuthorizeUserDto : PageEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public override string Id { get; set; }



        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "object_id")]
        public string? ObjectId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "object_name")]
        public string? ObjectName { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "user_id")]
        public string? UserId { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        [SugarColumn(ColumnName = "user_name")]
        public string? UserName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        [SugarColumn(ColumnName = "ctl_id")]
        public string? CtlId { get; set; }
    }

}
