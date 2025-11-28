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
using StackExchange.Profiling.Internal;
using ITPortal.Core;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataTableDto : PageEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// 所属数据源
        /// </summary>
        public string SourceId { get; set; }
        /// <summary>
        /// 所属数据源
        /// </summary>
        public string SourceName { get; set; }


        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表注释
        /// </summary>
        public string TableComment { get; set; }

        /// <summary>
        /// 能否预览
        /// 可审核
        /// </summary>
        public int? Reviewable { get; set; }



        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

  
        /// <summary>
        /// 所有者ID
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// 所有者名称
        /// </summary>
        public string OwnerName { get; set; }
        /// <summary>
        /// 所有者名称
        /// </summary>
        public string OwnerDept { get; set; }

        /// <summary>
        /// 层级ID
        /// </summary>
        public string LevelId { get; set; }

        /// <summary>
        /// 层级名称
        /// </summary>
        public string LevelName { get; set; }

        /// <summary>
        /// topic ID
        /// </summary>
        public string CtlId { get; set; }

        /// <summary>
        /// topic名称
        /// </summary>
        public string CtlName { get; set; }

        /// <summary>
        /// topic代码
        /// </summary>
        public string CtlCode { get; set; }

        /// <summary>
        /// topic备注
        /// </summary>
        public string CtlRemark { get; set; }

        /// <summa
        /// <summary>
        /// 更新频率
        /// </summary>
        public string UpdateFrequency { get; set; }


        /// <summary>
        /// 更新方式  手动(默认)/自动 
        /// </summary>
        public string UpdateMethod { get; set; }
        /// <summary>
        /// 数据时间范围
        /// </summary>
        public string DataTimeRange { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public int? DbType { get; set; }



        /// <summary>
        /// 数据分类：主数据和参考数据、业务记录数据、指标数据
        /// </summary>

        public string DataCategory { get; set; }
        /// <summary>
        /// 质量分数,上一次
        /// </summary>

        public int? LastScore { get; set; }
        /// <summary>
        /// 更新方法：全量，增量
        /// </summary>
        public string UpdateCategory { get; set; }

    }

}
