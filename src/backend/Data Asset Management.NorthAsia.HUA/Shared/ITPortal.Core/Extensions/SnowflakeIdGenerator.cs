using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ITPortal.Core.Extensions
{
    /// <summary>
    /// 雪花id，64位
    /// </summary>
    public class SnowflakeIdGenerator
    {
        public static SnowflakeIdGenerator IdGenerator;
        private static object _locker = new object();


        // 起始时间戳，这里设置为 2020-01-01 00:00:00
        private const long Epoch = 1577836800000L;
        // 机器 ID 所占的位数
        private const int WorkerIdBits = 5;
        // 数据中心 ID 所占的位数
        private const int DatacenterIdBits = 5;
        // 序列号所占的位数
        private const int SequenceBits = 12;

        // 机器 ID 的最大值
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        // 数据中心 ID 的最大值
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        // 机器 ID 向左移位数
        private const int WorkerIdShift = SequenceBits;
        // 数据中心 ID 向左移位数
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        // 时间戳向左移位数
        private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        // 序列号的掩码
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private readonly long _workerId;
        private readonly long _datacenterId;
        private long _sequence = 0L;
        private long _lastTimestamp = -1L;

        /// <summary>
        /// 初始化雪花算法生成器，传入机器 ID 和数据中心 ID
        /// </summary>
        /// <param name="workerId">传入机器 ID</param>
        /// <param name="datacenterId"></param>
        /// <exception cref="ArgumentException">数据中心 ID</exception>
        public SnowflakeIdGenerator(long workerId, long datacenterId)
        {
            if (workerId > MaxWorkerId || workerId < 0)
            {
                throw new ArgumentException($"Worker ID 不能大于 {MaxWorkerId} 或小于 0");
            }
            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException($"数据中心 ID 不能大于 {MaxDatacenterId} 或小于 0");
            }

            _workerId = workerId;
            _datacenterId = datacenterId;
        }

        public static SnowflakeIdGenerator GetIdGenerator()
        {
            if (IdGenerator == null) lock (_locker) if (IdGenerator == null) IdGenerator = new SnowflakeIdGenerator(1, 1);
            return IdGenerator;
        }

        public static long NextUid()
        {
           return GetIdGenerator().NextId();
        }

        public static string NextUid(int length=19)
        {
            return GetIdGenerator().NextId(length);
        }

        public static string NextOldAssetId(int length=19)
        {
            return GetIdGenerator().NextId(length);
        }
        public long NextId()
        {
            long timestamp = GetCurrentTimestamp();
            long sequence;

            if (timestamp < _lastTimestamp)
            {
                throw new Exception("系统时钟回拨，拒绝生成 ID  " + (timestamp - _lastTimestamp) + " 毫秒");
            }

            if (timestamp == _lastTimestamp)
            {
                sequence = Interlocked.Increment(ref _sequence) & SequenceMask;
                if (sequence == 0)
                {
                    timestamp = WaitNextMillis(_lastTimestamp);
                }
            }
            else
            {
                Interlocked.Exchange(ref _sequence, 0);
                sequence = 0;
            }

            Interlocked.Exchange(ref _lastTimestamp, timestamp);

            return ((timestamp - Epoch) << TimestampLeftShift) |
                   (_datacenterId << DatacenterIdShift) |
                   (_workerId << WorkerIdShift) |
                   sequence;
        }

        public string NextId(int length)
        {
            var id = NextId();
            string idStr = id.ToString();
            if (idStr.Length > length)
            {
                idStr = idStr.Substring(idStr.Length - length);
            }
            else if (idStr.Length < length)
            {
                idStr = idStr.PadLeft(length, '0');
            }
            return idStr;
        }

        private long GetCurrentTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private long WaitNextMillis(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }



}
