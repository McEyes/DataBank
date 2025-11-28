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

namespace DataAssetManager.DataApiServer.Application.DataChangeRecord.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataChangeRecordDto : PageEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public int? Version { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string ObjectId { get; set; }


        /// <summary>
        /// 表注释
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string FieldOldValue { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string FieldNewValue { get; set; }

    }

}
