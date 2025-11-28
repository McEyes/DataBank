using ITPortal.Core.DataSource;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.DataSource.Dtos
{
    public class DbSchemaDto
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public int? Dbtype { get; set; }
        /// <summary>
        /// 数据库连接地址
        /// </summary>
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } = "*******";
        public int Port { get; set; }
        public string DbName { get; set; }
        public string Sid { get; set; }
    }
}
