using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.SqlParse;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using System.Data;
using System.Diagnostics;
using System.Xml;

using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataAccessService : IDataAccessService, ITransient
    {
        private readonly ILogger<DataTableService> _logger;
        private readonly ISqlSugarClient _db;
        private readonly IDistributedCacheService _cache;
        private readonly IDataSourceService _sourceService;
        private readonly IDataTableService _tableService;
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        public DataAccessService(ISqlSugarClient db,
            IDistributedCacheService cache,
             IDataSourceService source,
             IDataTableService table,
              CacheDbQueryFactory cacheDbQueryFactory,
            ILogger<DataTableService> logger)
        {
            _db = db;
            _cache = cache;
            _sourceService = source;
            _tableService = table;
            _logger = logger;
            _cacheDbQueryFactory = cacheDbQueryFactory;
        }

        public async Task<dynamic> GetDataPreview(string tableId, int pageSize = 20, string sort = "")
        {
            if (string.IsNullOrWhiteSpace(tableId))
            {
                throw new DataException("Parameter [tableId] cannot be empty");
            }
            var table = await _tableService.Get(tableId);

            if (table == null)
            {
                throw new DataException("Data table does not exist");
            }

            var dataSource = await _sourceService.Get(table.SourceId);

            //如果有改真实表名，替换真实表名
            var realTableName = table.TableName;
            var rename = await _tableService.GetTableRename(dataSource.SourceName, realTableName);
            if (rename != null)
            {
                realTableName = rename.NewTableName;
                dataSource = await _sourceService.Get(rename.NewSourceId);
                if (dataSource == null)
                {
                    _logger.LogError($"查询sql异常,Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}) ");
                    throw new AppFriendlyException($"Rename Table Source is not exsit：{rename.NewSourceName}({rename.NewSourceId}).", "500");
                }
            }
            var sqlClient = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
            ISqlParser sqlparser = SqlParserFactory.CreateParser(dataSource.DbSchema.Dbtype);
            var query = sqlparser.BuildSelectFromTable(sqlClient, realTableName);
            if (sort.IsNotNullOrWhiteSpace())
            {
                query = query.OrderBy(sort);
            }
            var dataList = await query.ToPageListAsync(1, pageSize);
            List<string> columns = new List<string>();
            if (dataList.Count > 0)
                ((IDictionary<string, object>)dataList[0]).Keys.ToList().ForEach(item => columns.Add(item.LowerFirstChar()));
            else
            {

            }
            return new { ColumnList = columns, DataList = dataList };
        }

        public async Task<dynamic> SqlRun(SqlDto dto)
        {
            List<object> results = new List<object>();

            var dataSource = await _sourceService.Get(dto.SourceId);
            var sqlClient = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
            var mutlSqls = dto.SqlText.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries);
            ISqlParser sqlparser = SqlParserFactory.CreateParser(dataSource.DbSchema.Dbtype);

            foreach (var sql in mutlSqls)
            {
                if(sql.Replace("\n","").Replace("\r", "").IsNullOrWhiteSpace()) continue;
                var pageInfo = sqlparser.ExtractPaginationInfo(sql);
                var query = sqlClient.SqlQueryable<object>(sql);
                List<object> dataList = null;
                var sw = new Stopwatch();
                sw.Start();
                if (!pageInfo.hasPagination) dataList = await query.ToPageListAsync(1, 1000);
                else dataList = await query.ToListAsync();
                List<string> columns = new List<string>();
                if (dataList.Count > 0) columns = ((IDictionary<string, object>)dataList[0]).Keys.ToList();
                sw.Stop();
                results.Add(new { ColumnList = columns, DataList = dataList, sql = sql, success = true, time = sw.ElapsedMilliseconds });
            }
            return results;
        }
    }
}