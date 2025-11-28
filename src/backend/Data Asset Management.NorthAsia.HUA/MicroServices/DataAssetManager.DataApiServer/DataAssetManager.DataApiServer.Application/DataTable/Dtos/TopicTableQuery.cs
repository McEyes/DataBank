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

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class TopicTableQuery : PageEntity<string>
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Status { get; set; }
    

        /// <summary>
        /// 控制ID
        /// </summary>
        public string CtlId { get; set; }

        /// <summary>
        /// 控制名称
        /// </summary>
        public string ParentCtlId{ get; set; }
        /// <summary>
        /// 控制名称
        /// </summary>
        public string DataCategory { get; set; }

  
        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

    }

}
