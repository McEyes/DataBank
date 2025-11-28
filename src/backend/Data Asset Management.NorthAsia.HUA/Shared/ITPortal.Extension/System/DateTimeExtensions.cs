using System.Globalization;

namespace ITPortal.Extension.System
{
    /// <summary>
    /// 时间扩展操作类
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 当前时间是否周末
        /// </summary>
        /// <param name="dateTimeOffset">时间点</param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTimeOffset dateTimeOffset)
        {
            DayOfWeek[] weeks = { DayOfWeek.Saturday, DayOfWeek.Sunday };
            return weeks.Contains(dateTimeOffset.DayOfWeek);
        }

        /// <summary>
        /// 当前时间是否工作日
        /// </summary>
        /// <param name="dateTimeOffset">时间点</param>
        /// <returns></returns>
        public static bool IsWeekday(this DateTimeOffset dateTimeOffset)
        {
            DayOfWeek[] weeks = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };
            return weeks.Contains(dateTimeOffset.DayOfWeek);
        }

        /// <summary>
        /// 获取时间相对唯一字符串
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <param name="millisecond">是否使用毫秒</param>
        /// <returns></returns>
        public static string ToUniqueString(this DateTimeOffset dateTimeOffset, bool millisecond = false)
        {
            var seconds = dateTimeOffset.Hour * 3600 + dateTimeOffset.Minute * 60 + dateTimeOffset.Second;
            var value = $"{dateTimeOffset:yyyy}{dateTimeOffset.DayOfYear}{seconds}";
            if (millisecond)
            {
                return value + dateTimeOffset.ToString("fff");
            }

            return value;
        }

        /// <summary>
        /// 将时间转换为JS时间格式(Date.getTime())
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <param name="millisecond">是否使用毫秒</param>
        public static string ToJsGetTime(this DateTimeOffset dateTimeOffset, bool millisecond = true)
        {
            var utc = dateTimeOffset.ToUniversalTime();
            var span = utc.Subtract(new DateTime(1970, 1, 1));
            return Math.Round(millisecond ? span.TotalMilliseconds : span.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将JS时间格式的数值转换为时间
        /// </summary>
        public static DateTimeOffset FromJsGetTime(this long jsTime)
        {
            var length = jsTime.ToString().Length;
            if (!(length == 10 || length == 13))
            {
                throw new ArgumentOutOfRangeException(null, "JS时间数值的长度不正确，必须为10位或13位");
            }
            var start = new DateTime(1970, 1, 1);
            var result = length == 10 ? start.AddSeconds(jsTime) : start.AddMilliseconds(jsTime);
            return result.ToUniversalTime();
        }
        
        /// <summary>
        /// 获取指定日期 当天的最大时间
        /// 例如 2021-09-10 11:22:33.123456 转换后 2021-09-10 23:59:59.9999999
        /// </summary>
        public static DateTimeOffset? ToCurrentDateMaxDateTime(this DateTimeOffset? dateTimeOffset)
        {
            return dateTimeOffset?.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// 获取指定时间的下一秒
        /// 例如 2021-09-10 11:11:11.1234567 转换后 2021-09-10 11:11:12.0000000
        /// </summary>
        public static DateTime? ToNextSecondDateTime(this DateTimeOffset? dateTimeOffset)
        {
            if (!dateTimeOffset.HasValue)
            {
                return null;
            }

            return new DateTime(dateTimeOffset.Value.Year, dateTimeOffset.Value.Month, dateTimeOffset.Value.Day, dateTimeOffset.Value.Hour,
                    dateTimeOffset.Value.Minute, dateTimeOffset.Value.Second)
                .AddSeconds(1);
        }
    }
}