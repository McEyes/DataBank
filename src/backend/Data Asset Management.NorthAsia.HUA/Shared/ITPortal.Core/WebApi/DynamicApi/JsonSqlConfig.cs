using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

namespace ITPortal.Core
{
    /// <summary>
    /// 返回参数信息Model
    /// </summary>
    public class JsonSqlConfig
    {
        public int? Limit { get; set; }
        public bool? CounTable { get; set; }
        public bool? DeleTable { get; set; }
        public bool? UpdaTable { get; set; }
        public bool? InserTable { get; set; } 
}
}
