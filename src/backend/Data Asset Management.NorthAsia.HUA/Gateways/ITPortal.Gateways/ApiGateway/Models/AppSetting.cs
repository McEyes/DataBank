namespace ApiGateway.Models
{
    public class AppSetting
    {
        public string SelfUrl { get; set; }
        public string CorsOrigins { get; set; }
        /// <summary>
        /// 默认10m
        /// </summary>
        public long MaxRequestBodySize { get; set; } = 10485760;
    }
}
