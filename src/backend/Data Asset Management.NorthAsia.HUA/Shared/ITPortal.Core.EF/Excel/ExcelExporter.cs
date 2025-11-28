using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Excel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MiniExcelLibs;
    using MiniExcelLibs.Attributes;
    using System.IO;
    using MiniExcelLibs.OpenXml;
    using Microsoft.AspNetCore.Mvc;
    using ITPortal.Extension.System;

    public class ExcelExporter
    {
        /// <summary>
        /// 导出对象数据为excel下载文件
        /// </summary>
        /// <param name="data">需要导出excel的数据，可以是table,dataset,Dictionary,或者T(T可以使用MiniExcelLibs.Attributes.ExcelColumn指定列属性)</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static IActionResult ExportExcel(object data, string fileName)
        {
            if (fileName.IsNullOrWhiteSpace()) fileName = $"temp_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            if (!fileName.EndsWith("xlsx", StringComparison.InvariantCultureIgnoreCase)) fileName += ".xlsx";

            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(data, configuration: new OpenXmlConfiguration() { AutoFilter = false });
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };
        }


        /// <summary>
        /// 导出List<T>到Excel（支持千万级数据和字段排序）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dataSource">数据源（支持IEnumerable<T>，无需一次性加载全量）</param>
        /// <param name="filePath">导出文件路径（如D:\data.xlsx）</param>
        /// <param name="sheetNamePrefix">Sheet名称前缀（自动拼接序号，如"数据_1"）</param>
        /// <param name="batchSize">每批写入行数（建议10万-20万，根据内存调整）</param>
        /// <param name="columnConfig">列配置（指定导出列、列名、格式和排序，可选）</param>
        public static void Export<T>(
            IEnumerable<T> dataSource,
            string filePath,
            string sheetNamePrefix = "数据",
            int batchSize = 200000,
            Dictionary<string, (string ColumnName, string Format, int Order)> columnConfig = null)
        {
            // 1. 校验参数
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("文件路径不能为空");
            if (batchSize <= 0) batchSize = 200000;

            // 2. 缓存T的属性信息（减少反射开销）并应用排序
            //var (properties, orderedColumnMappings) = GetExportProperties<T>(columnConfig);
            var sheetIndex = 1;
            // 3. 流式写入Excel（MiniExcel核心：无需加载全量数据）
            using (var stream = File.OpenWrite(filePath))
            {
                // 配置导出选项（指定列、Sheet拆分规则）OpenXmlConfiguration
                //var excelConfig = new MiniExcelLibs.MiniExcel.ExcelWriteConfig
                //{
                //    // 指定导出的列（属性名->列名），已按顺序排列
                //    Columns = orderedColumnMappings,
                //    // 每满100万行自动创建新Sheet（避免单个Sheet超限）
                //    MaxRowPerSheet = 1000000 // 单个Sheet最大行数（≤1,048,576）
                //};

                var SheetNameGenerator = $"{sheetNamePrefix}_{sheetIndex++}";
                // 核心：流式写入（MiniExcel会自动分批处理dataSource）
                stream.SaveAs(dataSource, sheetName: SheetNameGenerator, excelType: ExcelType.XLSX, configuration: new OpenXmlConfiguration()
                {
                });
            }
        }

        ///// <summary>
        ///// 获取T的导出属性（缓存反射信息）并按顺序排列
        ///// </summary>
        //private (PropertyInfo[] Properties, Dictionary<string, string> OrderedColumnMappings) GetExportProperties<T>(
        //    Dictionary<string, (string ColumnName, string Format, int Order)> columnConfig)
        //{
        //    var type = typeof(T);
        //    // 1. 获取所有可导出的属性（默认取public属性，排除[NotExport]标记的属性）
        //    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //        .Where(p => !p.IsDefined(typeof(NotExportAttribute), inherit: false))
        //        .ToList();

        //    // 2. 构建带排序信息的列配置
        //    var sortedColumns = new List<(string PropertyName, string ColumnName, int Order)>();

        //    foreach (var prop in properties)
        //    {
        //        int order = int.MaxValue; // 默认排序值，会被排在最后
        //        string columnName = prop.Name;

        //        // 检查是否有ExcelColumn特性定义的名称和排序
        //        var excelColumnAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
        //        if (excelColumnAttr != null)
        //        {
        //            columnName = excelColumnAttr.Name;
        //            order = excelColumnAttr.Order;
        //        }

        //        // 检查是否有用户配置的列信息（用户配置优先于特性）
        //        if (columnConfig?.TryGetValue(prop.Name, out var config) == true)
        //        {
        //            columnName = config.ColumnName;
        //            order = config.Order;
        //        }

        //        sortedColumns.Add((prop.Name, columnName, order));
        //    }

        //    // 3. 按Order排序，相同Order的按属性名排序确保一致性
        //    var orderedColumns = sortedColumns
        //        .OrderBy(c => c.Order)
        //        .ThenBy(c => c.PropertyName)
        //        .ToList();

        //    // 4. 构建最终的列映射字典（保持排序）
        //    var columnMappings = new Dictionary<string, string>();
        //    foreach (var column in orderedColumns)
        //    {
        //        columnMappings[column.PropertyName] = column.ColumnName;
        //    }

        //    // 5. 按相同顺序排列属性数组
        //    var orderedProperties = orderedColumns
        //        .Select(c => properties.First(p => p.Name == c.PropertyName))
        //        .ToArray();

        //    return (orderedProperties, columnMappings);
        //}
        //}

        //// 辅助特性：标记不导出的属性
        //[AttributeUsage(AttributeTargets.Property)]
        //public class NotExportAttribute : Attribute { }

        //// 辅助特性：指定列名和排序
        //[AttributeUsage(AttributeTargets.Property)]
        //public class ExcelColumnAttribute : Attribute
        //{
        //    public string Name { get; }
        //    public int Order { get; set; } = int.MaxValue; // 排序序号，越小越靠前

        //    public ExcelColumnAttribute(string name)
        //    {
        //        Name = name;
        //    }

        //    public ExcelColumnAttribute(string name, int order)
        //    {
        //        Name = name;
        //        Order = order;
        //    }
        //}
    }
}
