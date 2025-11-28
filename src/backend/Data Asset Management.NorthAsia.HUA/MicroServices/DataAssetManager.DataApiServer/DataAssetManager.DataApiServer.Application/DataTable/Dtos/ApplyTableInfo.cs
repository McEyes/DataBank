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
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 申请表单信息
    /// </summary>
    [Serializable]
    public class ApplyTableInfo 
    {
        /// <summary>
        /// 表单id
        /// </summary>
        public  string Id { get; set; }

        /// <summary>
        /// 申请字段清单配置
        /// </summary>
        public  List<DataColumnEntity> Columns { get; set; }

        /// <summary>
        /// 是否包含了全部字段
        /// </summary>
        public bool? IsAllColumns { get; set; }
    }

}
