using ITPortal.Core.DataSource;
using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataApi.Dtos
{
    /// <summary>
    /// 数据源信息表
    /// </summary>
    [Serializable]
    public class DataSourceDto : PageEntity<string>
    {
        /// <summary>
        /// 数据源类型
        /// </summary>
        public int? DbType
        {
            get { return _DbType; }
            set
            {
                if (_DbType != value)
                {
                    _DbType = value;
                    if (this.DbSchema != null)
                        this.DbSchema.Dbtype = value;
                }
            }
        }
        private int? _DbType;

        /// <summary>
        /// 数据源名称
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 元数据同步（0否，1同步中, 2是 3-出错）
        /// </summary>
        public string IsSync { get; set; }

        /// <summary>
        /// 数据源连接信息
        /// </summary>
        public DbSchema DbSchema
        {
            get { return _DbSchema; }
            set
            {
                if (value != _DbSchema)
                {
                    _DbSchema = value;
                    if (_DbSchema != null) _DbSchema.Dbtype = this.DbType;
                }
            }
        }
        public DbSchema _DbSchema;
        /// <summary>
        /// 数据源连接信息
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 数据源连接信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 子表信息（不映射到数据库）
        /// </summary>
        [JsonIgnore]
        public List<DataTableEntity> Children { get; set; }

    }
}