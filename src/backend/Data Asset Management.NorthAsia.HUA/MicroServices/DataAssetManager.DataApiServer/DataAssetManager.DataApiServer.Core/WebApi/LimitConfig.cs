
namespace DataAssetManager.DataApiServer.Core
{
    public class LimitConfig
    {
        /// <summary>
        /// 是否启用限制
        /// </summary>
        public bool Enable { get; set; } // 
        /// <summary>
        /// 允许的调用次数
        /// </summary>
        public int Times { get; set; } // 允许的调用次数
        /// <summary>
        ///  时间窗口（秒）
        /// </summary>
        public int Seconds { get; set; } // 时间窗口（秒）

        /// <summary>
        /// 是否启用 IP 限制
        /// </summary>
        public bool EnableIpLimit { get; set; }

        /// <summary>
        /// 是否启用用户限制
        /// </summary>
        public bool EnableUserLimit { get; set; }

        /// <summary>
        /// 是否启用设备名称限制
        /// </summary>
        public bool EnableDeviceNameLimit { get; set; }
    }

    public class RequestParam
    {
        public bool Nullable { get; set; } // 参数是否可为空
        public string ParamName { get; set; } // 参数名称
        public string ParamType { get; set; } // 参数类型（数据库类型）
        public string ParamComment { get; set; } // 参数说明
    }
}