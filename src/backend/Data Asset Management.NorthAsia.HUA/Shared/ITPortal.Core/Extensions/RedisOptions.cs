using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Extensions
{
    public class RedisOptions
    {
        public int db { get; set; } = 1;
        /// <summary>
        /// EndPoints = { "127.0.0.1:6379" },
        /// 多个 server1:6379,server2:6380
        /// </summary>
        public string Host { get; set; } = "cnhuam0itpoc81";
        public int Port { get; set; } = 6379;
        public string Password { get; set; } = "KDxRpxz0HrGaQZaH";
        /// <summary>
        /// ConnectTimeout = 5000,
        /// </summary>
        public string timeout { get; set; } = "6000ms";
        /// <summary>
        /// syncTimeout  = 10000,
        /// </summary>
        public int SyncTimeout { get; set; } = 10000;
        public int PoolSize = 200;
        /// <summary>
        /// 如果设置为 false，当连接失败时，客户端会尝试在后台重新连接。默认值为 true。
        /// 示例：abortConnect=false
        /// </summary>
        public bool AbortConnect { get; set; } = true;
        /// <summary>
        /// ssl：如果要使用 SSL 连接到 Redis 服务器，将此参数设置为 true。默认值为 false。
        /// 示例：ssl=true
        /// </summary>
        public bool Ssl { get; set; } = false;
        /// <summary>
        /// allowAdmin：如果需要执行 Redis 的管理命令（如 FLUSHALL），将此参数设置为 true。默认值为 false。
        /// 示例：allowAdmin=true
        /// </summary>
        public bool AllowAdmin { get; set; } = false;


        /// <summary>
        /// "127.0.0.1:6379,password=yourpassword,poolsize=100,connectTimeout=5000,syncTimeout=10000,abortConnect=false,ssl=false,allowAdmin=true"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Host},password={Password},poolsize={PoolSize}";
        }
    }
}
