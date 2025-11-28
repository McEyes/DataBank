using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using DataAssetManager.DataTableServer.Application;

using Elastic.Clients.Elasticsearch.Ingest;

using Furion.LinqBuilder;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Exceptions;
using ITPortal.Core.Services;
using ITPortal.Core.SqlParse;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using Mysqlx.Crud;

using MySqlX.XDevAPI.Relational;

using SqlSugar;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataAssetManager.DataApiServer.Application.Services
{
    public class ApiSqlServiceHandler : IApiServices
    {
        protected readonly ISqlSugarClient _db;
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        private readonly IDistributedCacheService _cache;
        private readonly IDataSourceService _dataSourceService;
        private readonly ILogger<ApiSqlServiceHandler> _logger;
        private readonly IAssetClientsService _assetClientsService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataColumnService _dataColumnService;
        private readonly DynamicTableTypeGenerator _dynTypeGen;

        public ApiSqlServiceHandler(ISqlSugarClient db, IDistributedCacheService cache, CacheDbQueryFactory cacheDbQueryFactory, IDataSourceService dataSourceService
            , ILogger<ApiSqlServiceHandler> logger, IAssetClientsService assetClientsService, IDataTableService dataTableService, DynamicTableTypeGenerator dynTypeGen, IDataColumnService dataColumnService)
        {
            _db = db;
            _cache = cache;
            _cacheDbQueryFactory = cacheDbQueryFactory;
            _dataSourceService = dataSourceService;
            _logger = logger;
            _assetClientsService = assetClientsService;
            _dataTableService = dataTableService;
            _dynTypeGen = dynTypeGen;
            _dataColumnService = dataColumnService;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        /// <exception cref="DataQueryException"></exception>
        public virtual async Task<PageResult<object>> Execute(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData)
        {
            var tableInfo = await _dataTableService.Get(apiInfo.TableId);
            var datasource = await _dataSourceService.Get(tableInfo.SourceId);

            #region 参数校验
            var total = 0;
            if (paramsData.ContainsKey("total")) int.TryParse(paramsData["total"] + "", out total);
            var pageInfo = RequestUtils.GetPageInfo(paramsData, await GetLimit(apiInfo.ExecuteConfig.pageSizeLimit, apiInfo.TableId));
            string sqlText = paramsData["sqlText"] + "";
            if (sqlText.IsNullOrEmpty()) throw new DataQueryException("参数[sqlText]不能为空！Parameter [sqlText] cannot be empty!");

            #endregion 参数校验

            #region sql检查和校验
            ISqlParser sqlparser = SqlParserFactory.CreateParser(datasource.DbSchema.Dbtype);
            var tableNames = sqlparser.ExtractTableNames(sqlText);
            if ((!tableNames.Contains(tableInfo.TableName) && !tableNames.Contains(tableInfo.Alias)) || tableNames.Count > 1)
            {
                throw new SqlQueryIllegalSQLDataException($"API({apiInfo.Id}) execute sqlQuery", apiInfo.ExecuteConfig.tableName);
            }
            //如果用了表别名，替换真正表名
            if (!tableNames.Contains(tableInfo.TableName) && tableNames.Contains(tableInfo.Alias))
            {
                sqlText = sqlText.Replace($" {tableInfo.Alias}", $" {tableInfo.TableName}");
            }
            //如果有改真实表名，替换真实表名
            var rename = await _dataTableService.GetTableRename(datasource.SourceName, tableInfo.TableName);
            if (rename != null)
            {
                sqlText = sqlText.Replace($" {tableInfo.TableName}", $" {rename.NewTableName}");
                datasource = await _dataSourceService.Get(rename.NewSourceId);
                if (datasource == null)
                {
                    _logger.LogError($"查询sql异常,Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}) \r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}");
                    throw new AppFriendlyException($"Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}).\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}", "500");
                }
            }
            //检查字段是否包含,并过滤不包含的字段
            sqlText = await FilterScope(sqlparser, sqlText, apiInfo.ExecuteConfig.tableId);

            sqlText = sqlparser.RemovePagination(sqlText);

            var orderList = sqlparser.ExtractOrderInfos(sqlText);

            #endregion sql检查和校验

            #region sql执行
            var sw = new Stopwatch();
            sw.Start();

            var db = _cacheDbQueryFactory.CreateSqlClient(datasource.DbSchema);
            var query = db.SqlQueryable<object>(sqlText);
            var queryCount = db.SqlQueryable<object>($"select count(1) as Count from ({sqlText}) tmp_table_2314");
            //query.AddParameters(paramsData)
            List<object> list = null;
            var totalnumber = new RefAsync<long>();
            if (_cache.Get($"{DataAssetManagerConst.RedisKey}APITest") == "yes") list = new List<object>();
            else
            {
                if (orderList.Count > 0)
                {
                    list = await query.OrderBy(string.Join(',', orderList.Select(f => $"{f.FieldName} {f.OrderByType}")))
                        .ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    if (total == 0) totalnumber = await queryCount.Select<long>().FirstAsync();
                }
                else if (total == 0)
                {
                    list = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    totalnumber = await queryCount.Select<long>().FirstAsync();
                }
                else
                {
                    list = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                }
            }
            sw.Stop();
#if DEBUG
            Console.WriteLine($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
            _logger.LogInformation($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
#endif
            #endregion sql执行

            return new PageResult<object>(totalnumber, list, pageInfo.pageIndex, pageInfo.pageSize, sw.ElapsedMilliseconds);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        /// <exception cref="DataQueryException"></exception>
        public virtual async Task<PageResult> ExecuteClass(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData)
        {
            var tableInfo = await _dataTableService.Get(apiInfo.TableId);
            var datasource = await _dataSourceService.Get(tableInfo.SourceId);

            #region 参数校验
            var total = 0;
            if (paramsData.ContainsKey("total")) int.TryParse(paramsData["total"] + "", out total);
            var pageInfo = RequestUtils.GetPageInfo(paramsData, await GetLimit(apiInfo.ExecuteConfig.pageSizeLimit, apiInfo.TableId));
            string sqlText = paramsData["sqlText"] + "";
            if (sqlText.IsNullOrEmpty()) throw new DataQueryException("参数[sqlText]不能为空！Parameter [sqlText] cannot be empty!");

            #endregion 参数校验

            #region sql检查和校验
            ISqlParser sqlparser = SqlParserFactory.CreateParser(datasource.DbSchema.Dbtype);
            var tableNames = sqlparser.ExtractTableNames(sqlText);
            if ((!tableNames.Contains(tableInfo.TableName) && !tableNames.Contains(tableInfo.Alias)) || tableNames.Count > 1)
            {
                throw new SqlQueryIllegalSQLDataException($"API({apiInfo.Id}) execute sqlQuery", apiInfo.ExecuteConfig.tableName);
            }
            //如果用了表别名，替换真正表名
            if (!tableNames.Contains(tableInfo.TableName) && tableNames.Contains(tableInfo.Alias))
            {
                sqlText = sqlText.Replace($" {tableInfo.Alias}", $" {tableInfo.TableName}");
            }
            //如果有改真实表名，替换真实表名
            var rename = await _dataTableService.GetTableRename(datasource.SourceName, tableInfo.TableName);
            if (rename != null)
            {
                sqlText = sqlText.Replace($" {tableInfo.TableName}", $" {rename.NewTableName}");
                datasource = await _dataSourceService.Get(rename.NewSourceId);
                if (datasource == null)
                {
                    _logger.LogError($"查询sql异常,Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}) \r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}");
                    throw new AppFriendlyException($"Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}).\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}", "500");
                }
            }
            //检查字段是否包含,并过滤不包含的字段
            var sqlData = await FilterScopeToType(sqlparser, sqlText, apiInfo.ExecuteConfig.tableId, apiInfo.ExecuteConfig.tableName, datasource.DbSchema.Dbtype.Value);

            sqlText = sqlparser.RemovePagination(sqlText);

            var orderList = sqlparser.ExtractOrderInfos(sqlText);

            #endregion sql检查和校验

            #region sql执行
            var sw = new Stopwatch();
            sw.Start();

            var db = _cacheDbQueryFactory.CreateSqlClient(datasource.DbSchema);
            var query = db.SqlQueryable<object>(sqlData.sqlText);
            var queryCount = db.SqlQueryable<object>($"select count(1) as Count from ({sqlText}) tmp_table_2314");
            //query.AddParameters(paramsData)
            dynamic list = null;
            var totalnumber = new RefAsync<long>();
            if (_cache.Get($"{DataAssetManagerConst.RedisKey}APITest") == "yes") list = new List<object>();
            else
            {
                List<object> listData = null;
                if (orderList.Count > 0)
                {
                    listData = await query.OrderBy(string.Join(',', orderList.Select(f => $"{f.FieldName} {f.OrderByType}"))).ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    if (total == 0) totalnumber = await queryCount.Select<long>().FirstAsync();
                }
                else if (total == 0)
                {
                    listData = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    totalnumber = await queryCount.Select<long>().FirstAsync();
                }
                else
                {
                    listData = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                }
                list = listData.ToJSON().FromJsonString(sqlData.tableListType);
            }
            sw.Stop();
#if DEBUG
            Console.WriteLine($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
            _logger.LogInformation($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
#endif
            #endregion sql执行

            return new PageResult(totalnumber, list, pageInfo.pageIndex, pageInfo.pageSize, sw.ElapsedMilliseconds);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        /// <exception cref="DataQueryException"></exception>
        public virtual async Task<IActionResult> ExecuteToExcel(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData)
        {
            var result = await Execute(request, apiInfo, paramsData);

            string fileName = apiInfo.ApiName;
            if (paramsData.ContainsKey("excel_name"))
            {
                fileName = paramsData["excel_name"]?.ToString();
            }
            if (fileName.IsNullOrWhiteSpace()) fileName = apiInfo.ApiName;

            return ITPortal.Core.Excel.ExcelExporter.Export(result.Data, apiInfo.ApiName, fileName, columnConfig: await GetParamColumn(apiInfo, paramsData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        private async Task<List<MiniExcelLibs.Attributes.DynamicExcelColumn>> GetParamColumn(RouteInfo apiInfo, Dictionary<String, Object> paramsData)
        {
            //columnConfig: new List<MiniExcelLibs.Attributes.DynamicExcelColumn>() {
            //            new MiniExcelLibs.Attributes.DynamicExcelColumn("TableCode"){   Name="表真实名称",Index=3},
            //            new MiniExcelLibs.Attributes.DynamicExcelColumn("TableName"){   Name="表名"},
            //            new MiniExcelLibs.Attributes.DynamicExcelColumn("Dept"){   Name="所属部门"},
            //            new MiniExcelLibs.Attributes.DynamicExcelColumn("Count"){   Name="访问量" },
            // }
            var columnConfig = new List<MiniExcelLibs.Attributes.DynamicExcelColumn>();
            var columnDict = new Dictionary<string, MiniExcelLibs.Attributes.DynamicExcelColumn>();
            if (paramsData.ContainsKey("column_info"))
            {
                var columnData = paramsData["column_info"];
                if (columnData != null)
                {
                    var column_format = columnData.ToString().ToLower();
                    var columns = column_format.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var column in columns)
                    {
                        if (column.Contains("="))
                        {
                            var confs = column.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            if (confs[1].Contains(","))
                            {
                                var formats = confs[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                columnDict.Add(confs[0], new MiniExcelLibs.Attributes.DynamicExcelColumn(confs[0]) { Name = formats[0], Format = formats[1] });
                            }
                            else
                            {
                                columnDict.Add(confs[0], new MiniExcelLibs.Attributes.DynamicExcelColumn(confs[0]) { Name = confs[1] });
                            }
                        }
                        else
                        {
                            columnDict.Add(column, new MiniExcelLibs.Attributes.DynamicExcelColumn(column) { Name = column });
                        }
                    }
                }
            }
            var coumns = await _dataColumnService.GetByTableId(apiInfo.TableId);
            if (selectColumns.Count > 0 && !selectColumns.Any(f => f == "*"))
            {
                foreach (var item in selectColumns)
                {
                    if (columnDict.ContainsKey(item.ToLower()))
                    {
                        columnConfig.Add(columnDict[item.ToLower()]);
                    }
                    else
                    {
                        var column = coumns.FirstOrDefault(f => f.ColName.Equals(item, StringComparison.CurrentCultureIgnoreCase));
                        if (column != null)
                        {
                            columnConfig.Add(new MiniExcelLibs.Attributes.DynamicExcelColumn(column.ColName) { Name = column.ColComment });
                        }
                        else
                        {
                            columnConfig.Add(new MiniExcelLibs.Attributes.DynamicExcelColumn(item) { Name = item });
                        }
                    }
                }
            }
            else
            {
                foreach (var column in coumns)
                {
                    if (columnDict.ContainsKey(column.ColName.ToLower()))
                    {
                        columnConfig.Add(columnDict[column.ColName.ToLower()]);
                    }
                    else
                    {
                        columnConfig.Add(new MiniExcelLibs.Attributes.DynamicExcelColumn(column.ColName) { Name = column.ColComment });
                    }
                }
            }
            return columnConfig;
        }



        private async Task<int?> GetLimit(int? limit, string tableid)
        {
            var clientLimit = await _assetClientsService.GetClientScopesByClientId(_assetClientsService.CurrentUser.UserId, tableid);
            if (clientLimit != null && clientLimit.ConfigRule != null && clientLimit.ConfigRule.Limit > limit)
            {
                limit = clientLimit.ConfigRule.Limit;
            }
            return limit;
        }
        /// 查询字段解析
        private List<string> selectColumns =new List<string>();
        private async Task<string> FilterScope(ISqlParser sqlparser, string sqlText, string tableId)
        {
            //检查字段是否包含
            var newSqlText = sqlText.TrimEnd(' ').TrimEnd(';');
            var columns = selectColumns = sqlparser.ExtractFields(sqlText);

            var scopes = await _assetClientsService.GetClientScopesByClientId(_assetClientsService.CurrentUser.UserId, tableId);
            if (scopes != null && scopes.IsAllColumns != true && scopes.TableColumns != null && scopes.TableColumns.Count > 0)
            {
                newSqlText = sqlparser.ReplaceFields(sqlText, GetAllowedColumns(columns, scopes.TableColumns.Select(f => f.ColName).ToList()), "1");
            }
            else
            {
                var tableInfo = await _dataTableService.GetInfo(tableId);
                newSqlText = sqlparser.ReplaceFields(sqlText, GetAllowedColumns(columns, tableInfo.ColumnList.Select(f => f.ColName).ToList()), "1");
            }
            return newSqlText;
        }

        private async Task<(Type tableListType, Type tableType, string sqlText)> FilterScopeToType(ISqlParser sqlparser, string sqlText, string tableId, string tableName, int dbType)
        {
            //检查字段是否包含
            Type tableTypeGen = null;
            var newSqlText = sqlText;
            var columns = sqlparser.ExtractFields(sqlText);
            var scopes = await _assetClientsService.GetClientScopesByClientId(_assetClientsService.CurrentUser.UserId, tableId);
            if (scopes != null && scopes.IsAllColumns != true && scopes.TableColumns != null && scopes.TableColumns.Count > 0)
            {
                if (columns.Contains("*"))
                {
                    tableTypeGen = _dynTypeGen.GetTableType(tableName, scopes.TableColumns.OrderBy(f => f.ColPosition).ToList(), DbSchema.GetDbType(dbType));
                    newSqlText = sqlparser.ReplaceFields(sqlText, scopes.TableColumns.Select(f => f.ColName).ToList());
                }
                else
                {
                    var columnList = scopes.TableColumns.Where(f => columns.Contains(f.ColName)).OrderBy(f => f.ColPosition).ToList();
                    tableTypeGen = _dynTypeGen.GetTableType(tableName, columnList, DbSchema.GetDbType(dbType));
                    newSqlText = sqlparser.ReplaceFields(sqlText, columnList.Select(f => f.ColName).ToList());
                }
            }
            else
            {
                var columnList = await _dataColumnService.GetByTableId(tableId);
                tableTypeGen = _dynTypeGen.GetTableType(tableName, columnList, DbSchema.GetDbType(dbType));
            }
            Type listType = typeof(List<>).MakeGenericType(tableTypeGen);
            return (listType, tableTypeGen, newSqlText);
        }


        #region 检查列的聚合函数和是否容许列

        // 按数据库类型定义聚合函数
        private static readonly HashSet<string> AggregateFunctions = new HashSet<string>
            {
                "count", "avg", "sum", "max", "min", "group_concat",
                "std", "stddev", "variance", "any_value", "bit_and",
                "bit_or", "bit_xor", "count_distinct", "json_arrayagg",
                "json_objectagg",

                "stdev", "stdevp", "var", "varp", "checksum_agg", "string_agg",
                "count_big", "approx_count_distinct", "sumx", "avgx","array_agg",
               "bool_and", "bool_or", "json_agg",

                "jsonb_agg", "json_object_agg", "jsonb_object_agg",
                "xmlagg", "stddev_samp", "stddev_pop",
                "var_samp", "var_pop", "covar_samp", "covar_pop",
                "corr", "regr_avgx", "regr_avgy","date"
            };

        // 所有数据库在SELECT和FROM之间常用的关键字
        private static readonly HashSet<string> CommonSelectKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // 通用关键字
                "distinct", "all", "top", "as", "case", "when", "then", "else", "end",
                "cast", "convert", "coalesce", "nullif", "exists", "not exists",
    
                // MySQL特有
                "sql_calc_found_rows", "sql_small_result", "sql_big_result",
                "sql_buffer_result", "straight_join", "high_priority",
    
                // MS SQL Server特有
                "with ties", "offset", "fetch", "next", "cube", "rollup",
                "grouping sets", "pivot", "unpivot", "percent",
    
                // MariaDB特有
                "sql_cache", "sql_no_cache", "use index", "ignore index", "force index"
            };


        // 提取函数中的列名的正则表达式
        private static readonly Regex ColumnExtractorRegex = new Regex(@"\(([^)]+)\)", RegexOptions.Compiled);    
    

        // 用于提取函数参数的正则表达式（支持嵌套函数）
        private static readonly Regex FunctionParameterRegex = new Regex(
            @"(?<function>\w+)\s*\(\s*(?<parameter>.*?)\s*\)",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        // 用于匹配数字的正则表达式
        private static readonly Regex NumericRegex = new Regex(@"^\d+(\.\d+)?$");

        /// <summary>
        /// 检查列集合中是否包含指定数据库的聚合函数，且仅包含允许的列 GetAllownColumns
        /// </summary>
        /// <param name="columns">要检查的列集合</param>
        /// <param name="allowColumnList">允许的列名集合</param>
        /// <returns>包含聚合函数的有效列集合</returns>
        public static List<string> GetAllowedColumns(
            List<string> columns,
            List<string> allowColumnList)
        {
            var validColumns = new List<string>();

            if (columns == null || columns.Count == 0 || allowColumnList == null)
                return validColumns;

            foreach (var column in columns)
            {
                if (IsValidColumn(column, allowColumnList))
                {
                    validColumns.Add(column);
                }
            }

            return validColumns;
        }

        /// <summary>
        /// 验证单个列是否有效，支持CASE表达式等复杂语法
        /// </summary>
        private static bool IsValidColumn(string column, List<string> allowColumnList)
        {
            // 处理带别名的列（如 "count(id) as total" 或 "CASE ... END AS name"）
            string columnWithoutAlias = RemoveAlias(column);

            // 检查是否包含CASE表达式
            if (columnWithoutAlias.IndexOf("CASE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
                //return IsValidCaseExpression(columnWithoutAlias, allowColumnList);
            }

            // 检查是否包含聚合函数
            if (columnWithoutAlias.Contains('('))
            {
                return true;
                //foreach (var func in AggregateFunctions)
                //{
                //    if (columnWithoutAlias.IndexOf($"{func}(", StringComparison.OrdinalIgnoreCase) >= 0)
                //    {
                //        return IsValidAggregateFunction(columnWithoutAlias, allowColumnList);
                //    }
                //}
            }

            // 检查简单列
            return IsValidSimpleColumn(columnWithoutAlias, allowColumnList);
        }

        /// <summary>
        /// 验证CASE表达式是否有效
        /// </summary>
        private static bool IsValidCaseExpression(string caseExpression, List<string> allowColumnList)
        {
            // 提取CASE表达式中的所有列引用
            var columnReferences = ExtractColumnReferences(caseExpression);

            // 检查所有引用的列是否都在允许列表中
            foreach (var columnRef in columnReferences)
            {
                if (!allowColumnList.Contains(columnRef, StringComparer.OrdinalIgnoreCase) &&
                    !NumericRegex.IsMatch(columnRef) &&
                    columnRef != "*")
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 验证聚合函数是否有效
        /// </summary>
        private static bool IsValidAggregateFunction(string functionExpression, List<string> allowColumnList)
        {
            var match = FunctionParameterRegex.Match(functionExpression);
            if (!match.Success)
                return false;

            string parameter = match.Groups["parameter"].Value.Trim();

            // 处理嵌套函数
            if (AggregateFunctions.Any(f => parameter.IndexOf($"{f}(", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return IsValidAggregateFunction(parameter, allowColumnList);
            }

            // 检查参数是否有效
            return IsValidSimpleColumn(parameter, allowColumnList);
        }

        /// <summary>
        /// 验证简单列是否有效
        /// </summary>
        private static bool IsValidSimpleColumn(string column, List<string> allowColumnList)
        {
            // 处理带表别名的列（如 "t1.id" 或 "schema.table.column"）
            string columnName = GetSimpleName(column);

            return columnName == "*" ||
                   NumericRegex.IsMatch(columnName) ||
                   allowColumnList.Contains(columnName, StringComparer.OrdinalIgnoreCase) ||
                   CommonSelectKeywords.Contains(columnName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 移除列的别名部分
        /// </summary>
        private static string RemoveAlias(string column)
        {
            // 处理 AS 别名（如 "column AS alias"）
            var asMatch = Regex.Match(column, @"^(.*?)\s+AS\s+\w+", RegexOptions.IgnoreCase);
            if (asMatch.Success)
            {
                return asMatch.Groups[1].Value.Trim();
            }

            // 处理无AS的别名（如 "column alias"）
            var parts = column.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1 && !IsSqlKeyword(parts.Last()))
            {
                return string.Join(" ", parts.Take(parts.Length - 1)).Trim();
            }

            return column;
        }

        /// <summary>
        /// 提取表达式中的所有列引用
        /// </summary>
        private static List<string> ExtractColumnReferences(string expression)
        {
            var references = new List<string>();
            int bracketDepth = 0;
            bool inString = false;
            char stringDelimiter = '\0';
            int startIndex = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                // 处理字符串
                if (c == '\'' || c == '"')
                {
                    inString = !inString || (inString && c == stringDelimiter);
                    stringDelimiter = inString ? c : '\0';
                    continue;
                }

                if (inString)
                    continue;

                // 处理括号
                if (c == '(') bracketDepth++;
                else if (c == ')')
                {
                    if (bracketDepth > 0) bracketDepth--;
                }
                // 处理运算符和分隔符
                else if ((char.IsWhiteSpace(c) || IsOperator(c)) && bracketDepth == 0)
                {
                    if (i > startIndex)
                    {
                        string token = expression.Substring(startIndex, i - startIndex).Trim();
                        if (!string.IsNullOrEmpty(token) && !IsSqlKeyword(token) && !IsFunction(token))
                        {
                            references.Add(GetSimpleName(token));
                        }
                    }
                    startIndex = i + 1;
                }
            }

            // 添加最后一个标记
            if (startIndex < expression.Length)
            {
                string token = expression.Substring(startIndex).Trim();
                if (!string.IsNullOrEmpty(token) && !IsSqlKeyword(token) && !IsFunction(token))
                {
                    references.Add(GetSimpleName(token));
                }
            }

            return references.Distinct().ToList();
        }

        /// <summary>
        /// 获取简单列名（移除表别名和schema）
        /// </summary>
        private static string GetSimpleName(string qualifiedName)
        {
            var parts = qualifiedName.Split(new[] { '.', '[', ']', '"', '`' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.LastOrDefault() ?? qualifiedName;
        }

        /// <summary>
        /// 检查是否为SQL关键字
        /// </summary>
        private static bool IsSqlKeyword(string token)
        {
            // 扩展关键字列表以满足需求
            var keywords = new List<string> { "CASE", "WHEN", "THEN", "ELSE", "END", "IN", "AND", "OR", "NOT" }
                .Concat(AggregateFunctions)
                .Concat(CommonSelectKeywords);

            return keywords.Contains(token, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否为SQL函数
        /// </summary>
        private static bool IsFunction(string token)
        {
            return AggregateFunctions.Contains(token, StringComparer.OrdinalIgnoreCase) ||
                   new List<string> { "LOWER", "TRIM", "SPLIT_PART", "CAST", "CONVERT" }
                       .Contains(token, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查是否为运算符
        /// </summary>
        private static bool IsOperator(char c)
        {
            return new[] { ',', '=', '<', '>', '+', '-', '*', '/', '%', '!', '&', '|', '^' }.Contains(c);
        }
        ///// <summary>
        ///// 检查列集合中是否包含指定数据库的聚合函数，且仅包含允许的列
        ///// </summary>
        ///// <param name="columns">要检查的列集合</param>
        ///// <param name="allowColumnList">允许的列名集合</param>
        ///// <returns>输出参数：包含聚合函数的有效列集合</returns>
        //public static List<string> GetAllowedColumns(
        //    List<string> columns,
        //    List<string> allowColumnList)
        //{
        //    //var invalidColumns = new List<string>();
        //    var validColumns = new List<string>();

        //    if (columns == null || columns.Count == 0)
        //        return validColumns;

        //    foreach (var column in columns)
        //    {
        //        // 处理带别名的列（如 "count(id) as total"）
        //        var columnWithoutAlias = column.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
        //                                      .FirstOrDefault() ?? column;

        //        // 检查是否包含聚合函数
        //        //bool hasAggregate = aggregateFunctions.Any(func =>
        //        //    columnWithoutAlias.IndexOf($"{func}(", StringComparison.OrdinalIgnoreCase) >= 0);

        //        // 提取函数中的列名（如从 "count(id)" 中提取 "id"）
        //        // 提取函数中的参数（如从 "count(id)" 中提取 "id"，从 "count(1)" 中提取 "1"）
        //        var match = ColumnExtractorRegex.Match(columnWithoutAlias);
        //        string functionParameter = match.Success ? match.Groups[1].Value.Trim() : columnWithoutAlias;

        //        // 处理通配符、数字和列名三种情况
        //        bool isWildcard = functionParameter.Equals("*", StringComparison.Ordinal);
        //        bool isNumeric = NumericRegex.IsMatch(functionParameter);
        //        bool isAllowedColumn = allowColumnList.Contains(functionParameter);
        //        bool isAllowedSelectKeywords = CommonSelectKeywords.Contains(functionParameter);

        //        // 只要满足以下条件之一即为有效：通配符、数字、允许的列名
        //        if (isWildcard || isNumeric || isAllowedColumn || isAllowedSelectKeywords)// || hasAggregate
        //        {
        //            validColumns.Add(column);
        //        }
        //    }

        //    return validColumns;
        //}

        #endregion 检查列的聚合函数和是否容许列
    }
}
