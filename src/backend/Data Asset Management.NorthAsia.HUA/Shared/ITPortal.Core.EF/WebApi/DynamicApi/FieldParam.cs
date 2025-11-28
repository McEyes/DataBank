using Newtonsoft.Json.Linq;

namespace ITPortal.Core
{
    /// <summary>
    /// 字段信息
    /// </summary>
    public class FieldParam
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string columnName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int? dataLength { get; set; }

        /// <summary>
        /// 数据精度
        /// </summary>
        public int? dataPrecision { get; set; }

        /// <summary>
        /// 数据小数位
        /// </summary>
        public int? dataScale { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public string columnKey { get; set; }

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public string columnNullable { get; set; }

        /// <summary>
        /// 列的序号
        /// </summary>
        public int? columnPosition { get; set; }

        /// <summary>
        /// 列默认值
        /// </summary>
        public string dataDefault { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string columnComment { get; set; }

        /// <summary>
        /// 作为请求参数
        /// </summary>
        public string reqable { get; set; }

        /// <summary>
        /// 作为返回参数
        /// </summary>
        public string resable { get; set; }
    }
}
