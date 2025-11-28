using DataAssetManager.DataApiServer.Core;
using DataAssetManager.DataTableServer.Application;

using Elastic.Transport;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Core.SqlParse;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;

using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace DataAssetManager.DataApiServer.Application.Services
{
    public class ApiFormServiceHandler : IApiServices
    {
        protected readonly ISqlSugarClient _db;
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        private readonly IDistributedCacheService _cache;
        private readonly IDataSourceService _dataSourceService;
        private readonly ILogger<ApiFormServiceHandler> _logger;
        private readonly IAssetClientsService _assetClientsService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataColumnService _dataColumnService;
        private readonly DynamicTableTypeGenerator _dynTypeGen;

        public ApiFormServiceHandler(ISqlSugarClient db, IDistributedCacheService cache,
            IAssetClientsService assetClientsService, CacheDbQueryFactory cacheDbQueryFactory,
            IDataSourceService dataSourceService, ILogger<ApiFormServiceHandler> logger,
            IDataColumnService dataColumnService, DynamicTableTypeGenerator dynTypeGen,
            IDataTableService dataTableService)

        {
            _db = db;
            _cache = cache;
            _cacheDbQueryFactory = cacheDbQueryFactory;
            _dataSourceService = dataSourceService;
            _assetClientsService = assetClientsService;
            _logger = logger;
            _dynTypeGen = dynTypeGen;
            _dataColumnService = dataColumnService;
            _dataTableService = dataTableService;
        }

        /// <summary>
        /// 
        /// get 请求
        /// 从heard获取 apikey
        /// 从heard获取 secretkey
        /// 从 query 获取查询条件:pageNum,pageSize和所有表字段
        /// </summary>
        /// <param name="request"></param>
        /// <param name="apiInfo"></param>
        /// <param name="paramsData"></param>
        /// <returns></returns>
        public virtual async Task<PageResult<object>> Execute(HttpRequest request, RouteInfo apiInfo, Dictionary<String, Object> paramsData)
        {
            var tableInfo = await _dataTableService.Get(apiInfo.TableId??apiInfo.ExecuteConfig.tableId);
            var datasource = await _dataSourceService.Get(tableInfo.SourceId);

            var pageInfo = RequestUtils.GetPageInfo(paramsData, await GetLimit(apiInfo.ExecuteConfig.pageSizeLimit, apiInfo.TableId));

            //sql 替换

            #region sql 查询条件替换
            ISqlParser sqlparser = SqlParserFactory.CreateParser(datasource.DbSchema.Dbtype);
            _logger.LogInformation($"API{apiInfo.Id}:{apiInfo.ApiServiceUrl}\r\n{apiInfo.ExecuteConfig.sqlText}");

            //检查字段是否包含,并过滤不包含的字段
            var sqlText = apiInfo.ExecuteConfig.sqlText;
            sqlText = await FilterScope(sqlparser, sqlText, apiInfo.ExecuteConfig.tableId);
            //替换条件
            sqlText = sqlparser.ReplaceConditions(sqlText, paramsData);

            //如果有改真实表名，替换真实表名
            var rename = await _dataTableService.GetTableRename(datasource.SourceName,tableInfo.TableName);
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
            #endregion sql 查询条件替换

            #region sql执行
            var sw = new Stopwatch();
            sw.Start();
            var db = _cacheDbQueryFactory.CreateSqlClient(datasource.DbSchema);
            var totalnumber = new RefAsync<long>();
            List<object> list = null;
            try
            {
                var query = db.SqlQueryable<object>(sqlText);
                var queryCount = db.SqlQueryable<object>($"select count(1) as Count from ({sqlText}) tmp_table_2314");
                if (_cache.Get($"{DataAssetManagerConst.RedisKey}APITest") == "yes") list = new List<object>();
                else
                {
                    list = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    totalnumber = await queryCount.Select<long>().FirstAsync();
                    //totalnumber = query.Clone().Select<long>(it => SqlFunc.AggregateCount(1)).First();//
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"查询sql异常,当前数据库：{db.CurrentConnectionConfig.ConnectionString} \r\n{ex.Message}\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }} \r\n{ex.StackTrace}");
                throw new AppFriendlyException($"{ex.Message}.\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}", ex);
            }
            sw.Stop();
#if DEBUG
            Console.WriteLine($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
#endif
            #endregion sql执行

            return new PageResult<object>(totalnumber, list, pageInfo.pageIndex, pageInfo.pageSize, sw.ElapsedMilliseconds);
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
        public async Task<PageResult> ExecuteClass(HttpRequest request, RouteInfo apiInfo, Dictionary<string, object> paramsData = null)
        {
            var tableInfo = await _dataTableService.Get(apiInfo.TableId ?? apiInfo.ExecuteConfig.tableId);
            var datasource = await _dataSourceService.Get(apiInfo.SourceId);

            var pageInfo = RequestUtils.GetPageInfo(paramsData, await GetLimit(apiInfo.ExecuteConfig.pageSizeLimit, apiInfo.TableId));

            //sql 替换

            #region sql 查询条件替换
            ISqlParser sqlparser = SqlParserFactory.CreateParser(datasource.DbSchema.Dbtype);
            _logger.LogInformation($"API{apiInfo.Id}:{apiInfo.ApiServiceUrl}\r\n{apiInfo.ExecuteConfig.sqlText}");

            //检查字段是否包含,并过滤不包含的字段
            var sqlText = apiInfo.ExecuteConfig.sqlText;
            //检查字段是否包含,并过滤不包含的字段
            var sqlData = await FilterScopeToType(sqlparser, sqlText, apiInfo.ExecuteConfig.tableId, apiInfo.ExecuteConfig.tableName, datasource.DbSchema.Dbtype.Value);

            //替换条件
            sqlText = sqlparser.ReplaceConditions(sqlData.sqlText, paramsData);

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

            #endregion sql 查询条件替换

            #region sql执行
            var sw = new Stopwatch();
            sw.Start();
            var db = _cacheDbQueryFactory.CreateSqlClient(datasource.DbSchema);
            var totalnumber = new RefAsync<long>();
            object list = null;
            try
            {
                var query = db.SqlQueryable<object>(sqlText);
                var queryCount = db.SqlQueryable<object>($"select count(1) as Count from ({sqlText}) tmp_table_2314");
                if (_cache.Get($"{DataAssetManagerConst.RedisKey}APITest") == "yes") list = new List<object>();
                else
                {
                    var listData = await query.ToPageListAsync(pageInfo.pageIndex, pageInfo.pageSize);
                    totalnumber = await queryCount.Select<long>().FirstAsync();
                    //totalnumber = query.Clone().Select<long>(it => SqlFunc.AggregateCount(1)).First();//
                    list = listData.ToJSON().FromJsonString(sqlData.tableListType);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"查询sql异常,当前数据库：{db.CurrentConnectionConfig.ConnectionString} \r\n{ex.Message}\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }} \r\n{ex.StackTrace}");
                throw new AppFriendlyException($"{ex.Message}.\r\n {{\"sqlText\":\"{sqlText}\",\"pageIndex\":{pageInfo.pageIndex},\"pageSize\":{pageInfo.pageSize} }}", ex);
            }
            sw.Stop();
#if DEBUG
            Console.WriteLine($"执行sql耗时：{sw.ElapsedMilliseconds}毫秒");
#endif
            #endregion sql执行

            return new PageResult(totalnumber, list, pageInfo.pageIndex, pageInfo.pageSize, sw.ElapsedMilliseconds);
        }

        private async Task<string> FilterScope(ISqlParser sqlparser, string sqlText, string tableId)
        {
            //检查字段是否包含
            var newSqlText = sqlText;
            var columns = sqlparser.ExtractFields(sqlText);
            var scopes = await _assetClientsService.GetClientScopesByClientId(_assetClientsService.CurrentUser.UserId, tableId);
            if (scopes != null && scopes.IsAllColumns != true && scopes.TableColumns != null && scopes.TableColumns.Count > 0)
            {
                if (columns.Contains("*"))
                {
                    newSqlText = sqlparser.ReplaceFields(sqlText, scopes.TableColumns.Select(f => f.ColName).ToList());
                }
                else
                {
                    var columnList = scopes.TableColumns.Where(f => columns.Contains(f.ColName)).Select(f => f.ColName).ToList();
                    newSqlText = sqlparser.ReplaceFields(sqlText, columnList);
                }
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

        public async Task<IActionResult> ExecuteToExcel(HttpRequest request, RouteInfo apiInfo, Dictionary<string, object> paramsData = null)
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
            return columnConfig;
        }
    }
}
