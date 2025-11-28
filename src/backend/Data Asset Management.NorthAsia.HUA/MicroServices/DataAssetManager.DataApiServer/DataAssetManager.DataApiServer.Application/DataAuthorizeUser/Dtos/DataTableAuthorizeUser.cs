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

namespace DataAssetManager.DataApiServer.Application.DataAuthorizeUser.Dtos
{
    /// <summary>
    /// 数据库表信息表
    /// </summary>
    [Serializable]
    public class DataTableAuthorizeUser : PageEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public override string Id { get; set; }

        ///// <summary>
        ///// 所属数据源
        ///// </summary>
        //public string SourceId { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        ///// <summary>
        ///// 别名
        ///// </summary>
        //public string Alias { get; set; }


        ///// <summary>
        ///// 状态
        ///// </summary>
        //public string Status { get; set; }

        ///// <summary>
        ///// 层级ID
        ///// </summary>
        //public string LevelId { get; set; }

        /// <summary>
        /// 所有者ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 所有者ID
        /// </summary>
        public List<string> UserList { get; set; }

    }

}
