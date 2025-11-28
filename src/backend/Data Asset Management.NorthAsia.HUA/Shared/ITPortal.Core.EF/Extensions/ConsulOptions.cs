using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Extensions
{
    public class ConsulOptions
    {
        /// <summary>
        /// 服务id名称，唯一id，不可重复
        /// </summary>
        public string Id { get; set; }
        public string Host { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// 服务地址, host:port
        /// </summary>
        public string ServiceUrl { get; set; }
        public string ServiceHost { get; set; }
        public int Port { get; set; } = 80;
        /// <summary>
        /// 健康检查地址,默认：health
        /// </summary>
        public string healthUrl { get; set; } = "health";
        /// <summary>
        /// 健康检查时间间隔，或者称为心跳 间隔
        /// 默认10秒
        /// </summary>
        public int Interval { get; set; } = 10;
        /// <summary>
        /// 超时时间，默认15秒
        /// </summary>
        public int Timeout { get; set; } = 15;
        /// <summary>
        /// 服务停止多久后注销，默认5秒
        /// </summary>
        public int DeregisterCriticalServiceAfter { get; set; } = 5;


        /// <summary>
        /// 权重，默认1秒
        /// </summary>
        public int Weight { get; set; } = 1;

        public string Tags { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }
    }
}
