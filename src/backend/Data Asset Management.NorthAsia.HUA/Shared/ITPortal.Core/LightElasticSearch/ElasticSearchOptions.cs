using StackExchange.Profiling.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.LightElasticSearch
{
    /// <summary>
    /// 字段映射类型
    /// </summary>
    public class ElasticSearchOptions
    {
        public bool Enabled { get; set; }=false;
        /// <summary>
        /// 逗号分隔多个地址
        /// </summary>
        public string Url { get; set; }
        public string API_KEY { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
