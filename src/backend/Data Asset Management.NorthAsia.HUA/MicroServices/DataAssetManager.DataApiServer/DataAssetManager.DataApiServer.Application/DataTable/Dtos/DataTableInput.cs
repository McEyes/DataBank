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
using ITPortal.Core.SqlParser.Models;
using ITPortal.Core.ProxyApi.Dto;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using ITPortal.Core;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataTableInput
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

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
        /// 别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 能否预览
        /// 可审核
        /// </summary>
        public int Reviewable { get; set; }

        /// <summary>
        /// 表操作规则配置
        /// </summary>
        public JsonSqlConfig JsonSqlConfig { get; set; }



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


        /// <summary>
        /// 数据时间范围
        /// </summary>
        public string DataTimeRange { get; set; }

        /// <summary>
        /// 更新频率
        /// </summary>
        public string UpdateFrequency { get; set; }
        /// <summary>
        ///  更新方式  手动(默认)/自动 
        /// </summary>
        public string UpdateMethod { get; set; }

        /// <summary>
        /// 层级ID
        /// </summary>
        public string LevelId { get; set; }

        /// <summary>
        /// 层级名称
        /// </summary>
        public string LevelName { get; set; }


        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 是否需要上级审批
        /// </summary>
        public int? NeedSup { get; set; }
        /// <summary>
        /// 表所属所属部门,从拥有者所属部门获取
        /// </summary>

        public string OwnerDepart { get; set; }
        /// <summary>
        /// 数据分类：主数据和参考数据、业务记录数据、指标数据
        /// </summary>

        public string DataCategory { get; set; }
        /// <summary>
        /// 更新方法：全量，增量
        /// </summary>
        public string UpdateCategory { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>

        public string CreateBy { get; set; }

        /// <summary>
        /// 审批人列表
        /// </summary>
        public List<ApproveDto> ApproverList { get; set; }

        /// <summary>
        /// 所有者列表
        /// </summary>
        public List<DataTable.Dtos.DataAuthorizeOwnerEntity> OwnerList { get; set; }

        public List<DataColumnInfo> ColumnList { get; set; }
        /// <summary>
        /// 表的级别由column的级别判断
        /// </summary>
        public bool IsPublicSecurityLevel
        {
            get
            {
                if (ColumnList != null && ColumnList.Count > 0)
                {
                    return !ColumnList.Any(f => !f.IsPublicSecurityLevel);
                }
                return LevelId == "1" || LevelId == "2";
            }
        }

    }

}
