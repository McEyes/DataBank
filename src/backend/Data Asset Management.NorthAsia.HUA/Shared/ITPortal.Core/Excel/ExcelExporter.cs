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
        private const int MAX_ROWS_PER_SHEET = 1000000; // Excel单个Sheet最大行数限制
        /// <summary>
        /// 导出对象数据为excel下载文件
        /// </summary>
        /// <param name="data">需要导出excel的数据，可以是table,dataset,Dictionary,或者T(T可以使用MiniExcelLibs.Attributes.ExcelColumn指定列属性)</param>
        /// <param name="fileName">下载文件名</param>
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
        /// <param name="fileName">下载文件名</param>
        /// <param name="sheetNamePrefix">工作表名称前缀（自动拼接序号，如"数据_1"）</param>
        /// <param name="columnConfig">列配置（指定导出列、列名、格式和排序，可选）</param>
        /// <param name="enableAutoWidth">是否自动调整列宽</param>
        /// <param name="enableAutoFilter">是否启用自动筛选</param>
        public static IActionResult Export<T>(
            IEnumerable<T> dataSource,
            string fileName,
            string sheetNamePrefix = "数据",
            List<DynamicExcelColumn> columnConfig = null,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            // 1. 校验参数
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            if (!fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                fileName += ".xlsx";
            if (string.IsNullOrWhiteSpace(sheetNamePrefix))
                sheetNamePrefix = "数据";

            sheetNamePrefix = sheetNamePrefix.Replace("_sql_model", "").Replace("_table_bootstrap_model","");
            if (sheetNamePrefix.Length > 30)
            {
                sheetNamePrefix = sheetNamePrefix.Substring(0, 30);
            }

            // 2. 获取导出列映射和格式
            //var (orderedColumnMappings, columnFormats) = GetExportColumnMappings<T>(columnConfig);

            // 3. 创建Excel配置（使用实际存在的属性）
            var configuration = CreateExcelConfiguration(enableAutoWidth, enableAutoFilter, columnConfig);

            // 4. 创建内存流
            var memoryStream = new MemoryStream();

            try
            {
                // 5. 检查数据是否需要分多个Sheet
                // 先尝试获取数据总数（如果数据源支持）
                int totalCount = 0;
                bool canCount = TryGetCount(dataSource, out totalCount);

                if (canCount && totalCount <= MAX_ROWS_PER_SHEET)
                {
                    // 数据量小，直接导出到单个Sheet
                    memoryStream.SaveAs(
                        value: dataSource,
                        printHeader: true,
                        sheetName: sheetNamePrefix,
                        excelType: ExcelType.XLSX,
                        configuration: configuration);
                }
                else
                {
                    // 数据量大，需要分多个Sheet导出
                    ExportToMultipleSheets(
                        memoryStream,
                        dataSource,
                        sheetNamePrefix,
                        configuration);
                }

                // 6. 重置流位置并返回
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new FileStreamResult(
                    memoryStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
            catch
            {
                // 发生异常时释放内存流
                memoryStream.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 尝试获取数据源的记录总数
        /// </summary>
        private static bool TryGetCount<T>(IEnumerable<T> dataSource, out int count)
        {
            count = 0;

            try
            {
                // 检查是否实现了ICollection<T>接口
                if (dataSource is ICollection<T> collection)
                {
                    count = collection.Count;
                    return true;
                }

                // 检查是否实现了IQueryable<T>接口
                if (dataSource is IQueryable<T> queryable)
                {
                    count = queryable.Count();
                    return true;
                }

                // 对于其他IEnumerable类型，不尝试计数以避免加载全部数据
                return false;
            }
            catch
            {
                // 计数失败时返回false
                return false;
            }
        }

        /// <summary>
        /// 将数据导出到多个Sheet
        /// </summary>
        private static void ExportToMultipleSheets<T>(
            MemoryStream memoryStream,
            IEnumerable<T> dataSource,
            string sheetNamePrefix,
            IConfiguration configuration)
        {
            // 使用MiniExcel的动态工作表功能
            var dynamicSheets = new List<DynamicExcelSheet>();
            int sheetIndex = 1;
            int rowCount = 0;
            List<T> currentBatch = new List<T>();

            foreach (var item in dataSource)
            {
                currentBatch.Add(item);
                rowCount++;

                // 当达到单个Sheet的最大行数时，创建新的工作表
                if (rowCount >= MAX_ROWS_PER_SHEET)
                {
                    var sheetName = $"{sheetNamePrefix}_{sheetIndex}";
                    dynamicSheets.Add(new DynamicExcelSheet(sheetName)
                    {
                        Name = sheetName,
                        State = SheetState.Visible
                    });

                    sheetIndex++;
                    rowCount = 0;
                    currentBatch.Clear();
                }
            }

            // 添加最后一个批次
            if (currentBatch.Count > 0)
            {
                var sheetName = $"{sheetNamePrefix}_{sheetIndex}";
                dynamicSheets.Add(new DynamicExcelSheet(sheetName)
                {
                    Name = sheetName,
                    State = SheetState.Visible
                });
            }

            // 设置动态工作表
            if (configuration is OpenXmlConfiguration openXmlConfig)
            {
                openXmlConfig.DynamicSheets = dynamicSheets.ToArray();
            }

            // 使用第一个批次的数据作为主数据
            var firstBatch = currentBatch.Count > 0 ? currentBatch : dataSource.Take(1).ToList();

            memoryStream.SaveAs(
                value: firstBatch,
                printHeader: true,
                sheetName: $"{sheetNamePrefix}_1",
                excelType: ExcelType.XLSX,
                configuration: configuration);
        }

        ///// <summary>
        ///// 创建Excel导出配置
        ///// </summary>
        //private static IConfiguration CreateExcelConfiguration(
        //    bool enableAutoWidth,
        //    bool enableAutoFilter)
        //{
        //    return new OpenXmlConfiguration
        //    {
        //        // 自动列宽
        //        EnableAutoWidth = enableAutoWidth,
        //        MinWidth = 9.28515625,
        //        MaxWidth = 150,

        //        // 自动筛选
        //        AutoFilter = enableAutoFilter,

        //        // 冻结首行
        //        FreezeRowCount = 1,
        //        FreezeColumnCount = 0,

        //        // 性能优化配置
        //        EnableConvertByteArray = true,
        //        EnableWriteNullValueCell = true,
        //        WriteEmptyStringAsNull = false,
        //        TrimColumnNames = true,
        //        IgnoreEmptyRows = false,

        //        // 启用共享字符串缓存以提高性能
        //        EnableSharedStringCache = true,
        //        SharedStringCacheSize = 10 * 1024 * 1024, // 10MB缓存

        //        // 表格样式
        //        TableStyles = TableStyles.Default,

        //        // 启用文件路径写入
        //        EnableWriteFilePath = true
        //    };
        //}

        ///// <summary>
        ///// 获取导出列映射和格式
        ///// </summary>
        //private static (Dictionary<string, string> ColumnMappings, Dictionary<string, string> ColumnFormats)
        //    GetExportColumnMappings<T>(Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig)
        //{
        //    var type = typeof(T);
        //    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    Dictionary<string, (string ColumnName, string Format, int Index)> effectiveConfig;

        //    if (columnConfig != null && columnConfig.Count > 0)
        //    {
        //        // 使用用户指定的配置
        //        effectiveConfig = columnConfig;

        //        // 验证配置中的属性是否存在
        //        foreach (var propName in columnConfig.Keys)
        //        {
        //            if (properties.All(p => p.Name != propName))
        //            {
        //                throw new ArgumentException($"实体类型 {type.Name} 中不存在属性 {propName}", nameof(columnConfig));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 使用默认配置（所有公共属性）
        //        effectiveConfig = new Dictionary<string, (string, string, int)>();

        //        foreach (var prop in properties)
        //        {
        //            // 检查是否有MiniExcel的ExcelColumnAttribute
        //            var excelAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
        //            string columnName = excelAttr?.Name ?? prop.Name;
        //            string format = excelAttr?.Format ?? GetDefaultFormat(prop.PropertyType);

        //            // 从特性获取Index
        //            int index = excelAttr?.Index ?? 0;

        //            effectiveConfig[prop.Name] = (columnName, format, index);
        //        }
        //    }

        //    // 按Index排序，如果Index相同则按属性名排序
        //    var orderedProperties = effectiveConfig
        //        .OrderBy(kv => kv.Value.Index)
        //        .ThenBy(kv => kv.Key)
        //        .ToList();

        //    // 创建列映射（属性名 -> 列名）
        //    var columnMappings = orderedProperties
        //        .ToDictionary(kv => kv.Key, kv => kv.Value.ColumnName);

        //    // 创建列格式映射
        //    var columnFormats = orderedProperties
        //        .Where(kv => !string.IsNullOrEmpty(kv.Value.Format))
        //        .ToDictionary(kv => kv.Key, kv => kv.Value.Format);

        //    return (columnMappings, columnFormats);
        //}

        ///// <summary>
        ///// 获取默认的数据格式
        ///// </summary>
        //private static string GetDefaultFormat(Type propertyType)
        //{
        //    if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
        //        return "yyyy-MM-dd HH:mm:ss";
        //    if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
        //        return "0.00";
        //    if (propertyType == typeof(double) || propertyType == typeof(double?))
        //        return "0.00";
        //    if (propertyType == typeof(float) || propertyType == typeof(float?))
        //        return "0.00";
        //    return string.Empty;
        //}

        ////////////////////////////////////////////////////

        ///// <summary>
        ///// 导出List<T>到Excel（支持千万级数据和字段排序）
        ///// </summary>
        ///// <typeparam name="T">实体类型</typeparam>
        ///// <param name="dataSource">数据源（支持IEnumerable<T>，无需一次性加载全量）</param>
        ///// <param name="fileName">下载文件名</param>
        ///// <param name="sheetName">工作表名称（单个工作表时使用）</param>
        ///// <param name="columnConfig">列配置（指定导出列、列名、格式和排序，可选）</param>
        ///// <param name="enableAutoWidth">是否自动调整列宽</param>
        ///// <param name="enableAutoFilter">是否启用自动筛选</param>
        //public static IActionResult Export<T>(
        //    IEnumerable<T> dataSource,
        //    string fileName,
        //    string sheetName = "Sheet1",
        //    Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig = null,
        //    bool enableAutoWidth = true,
        //    bool enableAutoFilter = false)
        //{
        //    // 1. 校验参数
        //    if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        //    if (fileName.IsNullOrWhiteSpace()) fileName = $"temp_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
        //    if (!fileName.EndsWith("xlsx", StringComparison.InvariantCultureIgnoreCase)) fileName += ".xlsx";
        //    if (string.IsNullOrEmpty(sheetName)) sheetName = "Sheet1";

        //    // 2. 获取导出列映射和格式
        //    var (orderedColumnMappings, columnFormats) = GetExportColumnMappings<T>(columnConfig);

        //    // 4. 创建Excel配置（使用实际存在的属性）
        //    var configuration = CreateExcelConfiguration(enableAutoWidth, enableAutoFilter);

        //    // 5. 流式写入Excel
        //    var memoryStream = new MemoryStream();
        //    memoryStream.SaveAs(dataSource, printHeader: true, sheetName: sheetName, excelType: ExcelType.XLSX, configuration: configuration);
        //    memoryStream.Seek(0, SeekOrigin.Begin);
        //    return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        //    {
        //        FileDownloadName = fileName
        //    };
        //}

        /// <summary>
        /// 导出List<T>到Excel（支持千万级数据和字段排序）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="dataSource">数据源（支持IEnumerable<T>，无需一次性加载全量）</param>
        /// <param name="fileName">导出文件路径（如D:\data.xlsx）</param>
        /// <param name="sheetName">工作表名称（单个工作表时使用）</param>
        /// <param name="columnConfig">列配置（指定导出列、列名、格式和排序，可选）</param>
        /// <param name="enableAutoWidth">是否自动调整列宽</param>
        /// <param name="enableAutoFilter">是否启用自动筛选</param>
        public static void ExportToFile<T>(
            IEnumerable<T> dataSource,
            string filePath,
            string sheetName = "Sheet1",
            Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig = null,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            // 1. 校验参数
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("文件路径不能为空", nameof(filePath));
            if (string.IsNullOrEmpty(sheetName)) sheetName = "Sheet1";

            // 2. 获取导出列映射和格式
            var (orderedColumnMappings, columnFormats) = GetExportColumnMappings<T>(columnConfig);

            // 3. 确保输出目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 4. 创建Excel配置（使用实际存在的属性）
            var configuration = CreateExcelConfiguration(enableAutoWidth, enableAutoFilter);

            // 5. 流式写入Excel
            using (var stream = File.OpenWrite(filePath))
            {
                // 核心：流式写入（MiniExcel会自动处理数据）
                stream.SaveAs(
                    value: dataSource,
                    printHeader: true,
                    sheetName: sheetName,
                    excelType: ExcelType.XLSX,
                    configuration: configuration);
            }
        }


        /// <summary>
        /// 导出List<T>到Excel并返回文件流
        /// </summary>
        public static Stream ExportToStream<T>(
            IEnumerable<T> dataSource,
            string sheetName = "Sheet1",
            Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig = null,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
            if (string.IsNullOrEmpty(sheetName)) sheetName = "Sheet1";

            var (orderedColumnMappings, columnFormats) = GetExportColumnMappings<T>(columnConfig);

            var configuration = CreateExcelConfiguration(enableAutoWidth, enableAutoFilter);

            var stream = new MemoryStream();
            stream.SaveAs(
                value: dataSource,
                printHeader: true,
                sheetName: sheetName,
                excelType: ExcelType.XLSX,
                configuration: configuration);

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        /// <summary>
        /// 导出List<T>到Excel并返回字节数组
        /// </summary>
        public static byte[] ExportToBytes<T>(
            IEnumerable<T> dataSource,
            string sheetName = "Sheet1",
            Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig = null,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            using (var stream = ExportToStream(dataSource, sheetName, columnConfig, enableAutoWidth, enableAutoFilter))
            {
                return ((MemoryStream)stream).ToArray();
            }
        }

        /// <summary>
        /// ASP.NET Core导出Excel文件下载
        /// </summary>
        public static IActionResult ExportToActionResult<T>(
            IEnumerable<T> dataSource,
            string fileName = null,
            string sheetName = "Sheet1",
            Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig = null,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"export_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            if (!fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                fileName += ".xlsx";

            var stream = ExportToStream(dataSource, sheetName, columnConfig, enableAutoWidth, enableAutoFilter);

            return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };
        }

        /// <summary>
        /// 创建Excel导出配置（只使用实际存在的属性）
        /// </summary>
        private static IConfiguration CreateExcelConfiguration(
            bool enableAutoWidth,
            bool enableAutoFilter, List<DynamicExcelColumn> columnConfig = null)
        {
            var config = new OpenXmlConfiguration
            {
                // 自动列宽
                FastMode = enableAutoWidth,//自动宽度需要启用 fastmode
                EnableAutoWidth = enableAutoWidth,
                MinWidth = 9.28515625,
                MaxWidth = 150,
                DynamicColumns= columnConfig?.ToArray(),

                // 自动筛选
                AutoFilter = enableAutoFilter,

                // 冻结首行
                FreezeRowCount = 1,
                FreezeColumnCount = 0,

                // 性能优化配置
                EnableConvertByteArray = true,
                EnableWriteNullValueCell = true,
                WriteEmptyStringAsNull = false,
                TrimColumnNames = true,
                IgnoreEmptyRows = false,

                // 启用共享字符串缓存以提高性能
                EnableSharedStringCache = true,
                SharedStringCacheSize = 10 * 1024 * 1024, // 10MB缓存

                // 表格样式
                TableStyles = TableStyles.Default,

                // 启用文件路径写入
                EnableWriteFilePath = true,
            };

            return config;
        }

        ///// <summary>
        ///// 获取导出列映射和格式
        ///// </summary>
        //private static List<ExcelColumnAttribute> GetExportExcelColumns<T>(Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig)
        //{
        //    var type = typeof(T);
        //    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    //Dictionary<string, (string ColumnName, string Format, int Index)> effectiveConfig;
        //    List<ExcelColumnAttribute> effectiveConfig = new List<ExcelColumnAttribute>();

        //    if (columnConfig != null && columnConfig.Count > 0)
        //    {
        //        // 使用用户指定的配置
        //        //effectiveConfig = columnConfig;

        //        // 验证配置中的属性是否存在
        //        foreach (var propName in columnConfig.Keys)
        //        {
        //            if (properties.All(p => p.Name != propName))
        //            {
        //                throw new ArgumentException($"实体类型 {type.Name} 中不存在属性 {propName}", nameof(columnConfig));
        //            }
        //            effectiveConfig.Add(new ExcelColumnAttribute()
        //            {
        //                Name=propName,

        //            });
        //        }
        //    }
        //    else
        //    {
        //        // 使用默认配置（所有公共属性）
        //        effectiveConfig = new Dictionary<string, (string, string, int)>();

        //        foreach (var prop in properties)
        //        {
        //            // 检查是否有MiniExcel的ExcelColumnAttribute
        //            var excelAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
        //            string columnName = excelAttr?.Name ?? prop.Name;
        //            string format = excelAttr?.Format ?? GetDefaultFormat(prop.PropertyType);

        //            // 从特性获取Index（替换原来的Order）
        //            int index = excelAttr?.Index ?? 0;

        //            effectiveConfig[prop.Name] = (columnName, format, index);
        //        }
        //    }

        //    // 按Index排序，如果Index相同则按属性名排序
        //    var orderedProperties = effectiveConfig
        //        .OrderBy(kv => kv.Value.Index)
        //        .ThenBy(kv => kv.Key)
        //        .ToList();

        //    // 创建列映射（属性名 -> 列名）
        //    var columnMappings = orderedProperties
        //        .ToDictionary(kv => kv.Key, kv => kv.Value.ColumnName);

        //    // 创建列格式映射
        //    var columnFormats = orderedProperties
        //        .Where(kv => !string.IsNullOrEmpty(kv.Value.Format))
        //        .ToDictionary(kv => kv.Key, kv => kv.Value.Format);

        //    return (columnMappings, columnFormats);
        //}

        /// <summary>
        /// 获取导出列映射和格式
        /// </summary>
        private static (Dictionary<string, string> ColumnMappings, Dictionary<string, string> ColumnFormats)
            GetExportColumnMappings<T>(Dictionary<string, (string ColumnName, string Format, int Index)> columnConfig)
        {
            var type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Dictionary<string, (string ColumnName, string Format, int Index)> effectiveConfig;

            if (columnConfig != null && columnConfig.Count > 0)
            {
                // 使用用户指定的配置
                effectiveConfig = columnConfig;

                // 验证配置中的属性是否存在
                foreach (var propName in columnConfig.Keys)
                {
                    if (properties.All(p => p.Name != propName))
                    {
                        throw new ArgumentException($"实体类型 {type.Name} 中不存在属性 {propName}", nameof(columnConfig));
                    }
                }
            }
            else
            {
                // 使用默认配置（所有公共属性）
                effectiveConfig = new Dictionary<string, (string, string, int)>();

                foreach (var prop in properties)
                {
                    // 检查是否有MiniExcel的ExcelColumnAttribute
                    var excelAttr = prop.GetCustomAttribute<ExcelColumnAttribute>();
                    string columnName = excelAttr?.Name ?? prop.Name;
                    string format = excelAttr?.Format ?? GetDefaultFormat(prop.PropertyType);

                    // 从特性获取Index（替换原来的Order）
                    int index = excelAttr?.Index ?? 0;

                    effectiveConfig[prop.Name] = (columnName, format, index);
                }
            }

            // 按Index排序，如果Index相同则按属性名排序
            var orderedProperties = effectiveConfig
                .OrderBy(kv => kv.Value.Index)
                .ThenBy(kv => kv.Key)
                .ToList();

            // 创建列映射（属性名 -> 列名）
            var columnMappings = orderedProperties
                .ToDictionary(kv => kv.Key, kv => kv.Value.ColumnName);

            // 创建列格式映射
            var columnFormats = orderedProperties
                .Where(kv => !string.IsNullOrEmpty(kv.Value.Format))
                .ToDictionary(kv => kv.Key, kv => kv.Value.Format);

            return (columnMappings, columnFormats);
        }

        /// <summary>
        /// 获取默认的数据格式
        /// </summary>
        private static string GetDefaultFormat(Type propertyType)
        {
            if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                return "yyyy-MM-dd HH:mm:ss";
            if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                return "0.00";
            if (propertyType == typeof(double) || propertyType == typeof(double?))
                return "0.00";
            if (propertyType == typeof(float) || propertyType == typeof(float?))
                return "0.00";
            return string.Empty;
        }

        /// <summary>
        /// 导出多个数据集到多个工作表
        /// </summary>
        public static void ExportMultipleSheets(
            Dictionary<string, object> sheetsData,
            string filePath,
            bool enableAutoWidth = true,
            bool enableAutoFilter = false)
        {
            if (sheetsData == null || sheetsData.Count == 0)
                throw new ArgumentException("至少需要提供一个工作表的数据", nameof(sheetsData));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var configuration = new OpenXmlConfiguration
            {
                EnableAutoWidth = enableAutoWidth,
                AutoFilter = enableAutoFilter,
                FreezeRowCount = 1,
                EnableSharedStringCache = true,
                SharedStringCacheSize = 10 * 1024 * 1024
            };

            // 创建动态工作表
            var dynamicSheets = sheetsData.Select(kv =>
                new DynamicExcelSheet(kv.Key)
                {
                    Name = kv.Key,
                    State = SheetState.Visible
                }).ToArray();

            configuration.DynamicSheets = dynamicSheets;

            using (var stream = File.OpenWrite(filePath))
            {
                var firstSheet = sheetsData.First();
                stream.SaveAs(
                    value: firstSheet.Value,
                    printHeader: true,
                    sheetName: firstSheet.Key,
                    excelType: ExcelType.XLSX,
                    configuration: configuration);
            }
        }

        ////////////////////////////////////



        /// <summary>
        /// 导出对象数据为excel下载文件
        /// </summary>
        /// <param name="data">需要导出excel的数据，可以是table,dataset,Dictionary,或者T(T可以使用MiniExcelLibs.Attributes.ExcelColumn指定列属性)</param>
        /// <param name="fileName">文件名</param>
        /// <param name="sheetName">工作表名称</param>
        /// <param name="printHeader">是否打印表头</param>
        /// <returns></returns>
        public static IActionResult ExportExcel(
            object data,
            string fileName = null,
            string sheetName = "Sheet1",
            bool printHeader = true)
        {
            // 1. 验证数据
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // 2. 处理文件名
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"temp_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            if (!fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                fileName += ".xlsx";

            // 3. 创建Excel配置
            var configuration = CreateDefaultConfiguration();

            // 4. 创建内存流并导出数据
            using (var memoryStream = new MemoryStream())
            {
                // 使用正确的SaveAs方法参数顺序
                memoryStream.SaveAs(
                    value: data,
                    printHeader: printHeader,
                    sheetName: sheetName,
                    excelType: ExcelType.XLSX,
                    configuration: configuration);

                // 重置流位置
                memoryStream.Seek(0, SeekOrigin.Begin);

                // 返回文件流结果
                return new FileStreamResult(
                    memoryStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
        }

        /// <summary>
        /// 创建默认的Excel配置
        /// </summary>
        private static IConfiguration CreateDefaultConfiguration()
        {
            return new OpenXmlConfiguration
            {
                // 禁用自动筛选
                AutoFilter = false,

                // 冻结首行
                FreezeRowCount = 1,
                FreezeColumnCount = 0,

                // 启用自动列宽调整
                EnableAutoWidth = true,
                MinWidth = 9.28515625,
                MaxWidth = 100, // 适当限制最大宽度

                // 其他优化配置
                EnableConvertByteArray = true,
                EnableWriteNullValueCell = true,
                WriteEmptyStringAsNull = false,
                TrimColumnNames = true,
                IgnoreEmptyRows = false,

                // 启用共享字符串缓存以提高性能
                EnableSharedStringCache = true,
                SharedStringCacheSize = 5 * 1024 * 1024,

                // 表格样式
                TableStyles = TableStyles.Default,

                // 启用文件路径写入
                EnableWriteFilePath = true
            };
        }

        /// <summary>
        /// 导出Excel文件到指定路径
        /// </summary>
        public static void ExportExcelToPath(
            object data,
            string filePath,
            string sheetName = "Sheet1",
            bool printHeader = true,
            bool overwriteFile = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("文件路径不能为空", nameof(filePath));

            var configuration = CreateDefaultConfiguration();

            // 确保目录存在
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 使用文件路径保存
            MiniExcelLibs.MiniExcel.SaveAs(
                path: filePath,
                value: data,
                printHeader: printHeader,
                sheetName: sheetName,
                excelType: ExcelType.XLSX,
                configuration: configuration,
                overwriteFile: overwriteFile);
        }

        /// <summary>
        /// 导出Excel为字节数组
        /// </summary>
        public static byte[] ExportExcelToBytes(
            object data,
            string sheetName = "Sheet1",
            bool printHeader = true)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var configuration = CreateDefaultConfiguration();

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.SaveAs(
                    value: data,
                    printHeader: printHeader,
                    sheetName: sheetName,
                    excelType: ExcelType.XLSX,
                    configuration: configuration);

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// 导出多个数据集到多个工作表
        /// </summary>
        public static IActionResult ExportMultipleSheets(
            Dictionary<string, object> sheetsData,
            string fileName = null)
        {
            if (sheetsData == null || sheetsData.Count == 0)
                throw new ArgumentException("至少需要提供一个工作表的数据", nameof(sheetsData));

            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"multiple_sheets_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            if (!fileName.EndsWith(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                fileName += ".xlsx";

            var configuration = CreateDefaultConfiguration();

            // 根据提供的DynamicExcelSheet定义创建动态工作表配置
            var dynamicSheets = sheetsData.Select(kv =>
                new DynamicExcelSheet(kv.Key) // 使用构造函数参数
                {
                    Name = kv.Key, // 设置工作表名称
                    State = SheetState.Visible // 设置工作表可见状态
                }).ToArray();

            // 设置动态工作表
            ((OpenXmlConfiguration)configuration).DynamicSheets = dynamicSheets;

            using (var memoryStream = new MemoryStream())
            {
                // 使用第一个数据集作为主数据，动态工作表会处理其余的
                var firstSheet = sheetsData.First();

                memoryStream.SaveAs(
                    value: firstSheet.Value,
                    printHeader: true,
                    sheetName: firstSheet.Key,
                    excelType: ExcelType.XLSX,
                    configuration: configuration);

                memoryStream.Seek(0, SeekOrigin.Begin);

                return new FileStreamResult(
                    memoryStream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
        }

        /// <summary>
        /// 导出多个数据集到多个工作表（简化版，使用对象数组）
        /// </summary>
        public static IActionResult ExportMultipleSheets(
            params (string SheetName, object Data)[] sheets)
        {
            if (sheets == null || sheets.Length == 0)
                throw new ArgumentException("至少需要提供一个工作表的数据", nameof(sheets));

            var sheetsData = sheets.ToDictionary(s => s.SheetName, s => s.Data);
            return ExportMultipleSheets(sheetsData);
        }
    }
}
