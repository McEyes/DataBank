using ITPortal.Core.Services;

using Microsoft.Extensions.FileSystemGlobbing.Internal;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Npgsql.TypeHandlers.DateTimeHandlers;

using SqlSugar;

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
    public class TreeEntity
    {
        /// <summary>
        /// 主键
        /// </summary> 
        [SugarColumn(IsTreeKey = true)]
        public string Key { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        public string Type { get; set; } = "catagroy";

        /// <summary>
        /// 
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        public List<TreeEntity> Children { get; set; } = new List<TreeEntity>();
    }
}
