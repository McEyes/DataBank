using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ITPortal.Core
{
    /// <summary>
    /// 限流Model
    /// </summary>
    public class RateLimit
    {
        /// <summary>
        /// 是否限流：0:否，1：是
        /// </summary>
        public string enable { get; set; } = "1";
        /// <summary>
        /// 请求次数默认3次
        /// </summary>
        public int times { get; set; } = 5;
        /// <summary>
        /// 请求时间范围默认60秒
        /// </summary>
        public int seconds { get; set; } = 1;
        ///// <summary>
        ///// 是否启用 IP 限制
        ///// </summary>
        //[JsonIgnore]
        //public bool enableIpLimit { get; set; } = true;

        ///// <summary>
        ///// 是否启用用户限制
        ///// </summary>
        //[JsonIgnore]
        //public bool enableUserLimit { get; set; } = false;
    }
}
