using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using ITPortal.Core.SqlParse;
using ITPortal.Core.SqlParser.Models;

using Newtonsoft.Json.Linq;

using StackExchange.Profiling.Internal;

using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataSourceService : BaseService<DataSourceEntity, DataSourceDto, string>, IDataSourceService, ITransient
    {
        private readonly CacheDbQueryFactory _cacheDbQueryFactory;
        private readonly IDataTableService _tableService;
        public DataSourceService(ISqlSugarClient db, IDistributedCacheService cache, CacheDbQueryFactory cacheDbQueryFactory, IDataTableService tableService
            ) : base(db, cache, false, true, true)
        {
            _cacheDbQueryFactory = cacheDbQueryFactory;
            _tableService = tableService;
        }

        public override ISugarQueryable<DataSourceEntity> BuildFilterQuery(DataSourceDto filter)
        {
            filter.Keyword = filter.Keyword?.Trim()?.ToLower();
            filter.SourceName = filter.SourceName?.Trim()?.ToLower();
            return CurrentDb.Queryable<DataSourceEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.SourceName), f => SqlFunc.ToLower(f.SourceName).Contains(filter.SourceName))
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                .OrderByDescending(f => f.CreateTime);
        }

        public async Task<List<DataSourceEntity>> All()
        {
            //查询表的所有
            var list = await CurrentDb.Queryable<DataSourceEntity>().Where(f => f.Status == 1)
                .OrderByDescending(f => f.CreateTime).ToListAsync();
            return list;
        }

        public async Task<List<DataSourceEntity>> AllFromCache(bool clearCache = false)
        {
            //if (clearCache) await _cache.RemoveAsync(DataAssetManagerConst.DataSource_ListKey);
            return await _cache.GetObjectAsync(DataAssetManagerConst.DataSource_ListKey, async () => await All(), null, clearCache);
        }

        public async Task<List<DataSourceEntity>> InitRedisHash()
        {
            var list = await AllFromCache();
            foreach (var item in list)
            {
                _cache.HashSet(DataAssetManagerConst.DataSource_HashKey, item.Id, item);
            }
            return list;
        }
        public async Task<int> Count()
        {
            return await CurrentDb.Queryable<DataSourceEntity>().CountAsync();
        }


        public async Task<ITPortal.Core.Services.IResult> CheckConnection(DataSourceDto dataSource)
        {
            if (dataSource == null || dataSource.DbSchema == null) throw new ArgumentNullException(nameof(dataSource));
            var data = await Get(dataSource.Id);
            var db = _cacheDbQueryFactory.CreateSqlClient(data.DbSchema);
            var date = db.GetDate();
            return new Result();
        }

        public async Task<List<DbTable>> GetDbTablesMergeLocal(string sourceId)
        {
            if (string.IsNullOrWhiteSpace(sourceId)) return new List<DbTable>();
            var dataSource = await Get(sourceId);
            if (dataSource == null) return new List<DbTable>();
            var db = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
            var isqlParser = SqlParserFactory.CreateParser(db);
            var sql = isqlParser.GetTablesSql(dataSource.DbSchema.DbName);
            var tableList = await db.SqlQueryable<DbTable>(sql.ToString()).ToListAsync();
            //显示指定开头的表
            var restrict = GetRestrict(dataSource.DbSchema.DbName);
            if (!restrict.IsNullOrWhiteSpace())
                tableList = tableList.Where(f => restrict.Contains(f.TableName.Split("_")[0].ToLower() + ")")).ToList();
            var localTables = await _tableService.AllFromCache();
            if (localTables != null)
            {
                tableList.ForEach(f => f.Type = localTables.Any(d => d.TableName.Equals(f.TableName, StringComparison.InvariantCultureIgnoreCase)) ? "1" : "0");
            }
            tableList = tableList.OrderBy(f => f.Type).ToList();
            return tableList;
        }

        /// <summary>
        /// 优先获取已保存的集合，没有保存过在获取原始数据库中的字段集合
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<DataColumnEntity>> GetDataTableColumns(string sourceId, string tableName)
        {
            if (string.IsNullOrWhiteSpace(sourceId)) return new List<DataColumnEntity>();
            var dataSource = await Get(sourceId);
            if (dataSource == null) return new List<DataColumnEntity>();
            var tableInfo = await _tableService.GetTableInfoByName(tableName);
            if (tableInfo != null)
            {
                var columnList = await _tableService.GetTableCloumnsByTableId(tableInfo.Id);
                return columnList;
            }

            var db = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
            var isqlParser = SqlParserFactory.CreateParser(db);
            var sql = isqlParser.GetTableColumnsSql(dataSource.DbSchema.DbName, tableName);
            var list = await db.SqlQueryable<DataColumnEntity>(sql.ToString()).ToListAsync();
            //var flag = list.OrderBy(f => f.ColName).Any(f => f.ColKey == "1");
            list.ForEach(f =>
            {
                f.Nullable = (f.Nullable == "1" || "true".Equals(f.Nullable, StringComparison.CurrentCultureIgnoreCase)) ? "1" : "0";
                f.ColKey = (f.ColKey == "1" || "true".Equals(f.ColKey, StringComparison.CurrentCultureIgnoreCase)) ? "1" : "0";
            });
            return list;
        }

        /// <summary>
        /// 获取数据库中的表字段集合
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async Task<List<DataColumnEntity>> GetDbTableColumns(string sourceId, string tableName)
        {
            if (string.IsNullOrWhiteSpace(sourceId)) return new List<DataColumnEntity>();
            var dataSource = await Get(sourceId);
            if (dataSource == null) return new List<DataColumnEntity>();

            var db = _cacheDbQueryFactory.CreateSqlClient(dataSource.DbSchema);
            var isqlParser = SqlParserFactory.CreateParser(db);
            var sql = isqlParser.GetTableColumnsSql(dataSource.DbSchema.DbName, tableName);
            var list = await db.SqlQueryable<DataColumnEntity>(sql.ToString()).ToListAsync();
            //var flag = list.OrderBy(f => f.ColName).Any(f => f.ColKey == "1");
            list.ForEach(f =>
            {
                f.Nullable = (f.Nullable == "1" || "true".Equals(f.Nullable, StringComparison.CurrentCultureIgnoreCase)) ? "1" : "0";
                f.ColKey = (f.ColKey == "1" || "true".Equals(f.ColKey, StringComparison.CurrentCultureIgnoreCase)) ? "1" : "0";
            });
            return list;
        }

        public string GetRestrict(string sourceName)
        {
            try
            {
                // 读取配置文件
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                var json = File.ReadAllText(configPath, Encoding.UTF8);

                // 解析 JSON
                var jsonObject = JObject.Parse(json);
                var sourceArray = jsonObject["source"]?.ToString();

                if (!string.IsNullOrWhiteSpace(sourceArray))
                {
                    var jsonArray = JArray.Parse(sourceArray);
                    foreach (var item in jsonArray)
                    {
                        var key = item["sourceName"]?.ToString() ?? string.Empty;
                        if (sourceName.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            return item["tableName"]?.ToString() ?? string.Empty;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return string.Empty;
            }
        }


        public override async Task<int> Modify<TEntity>(TEntity entity, bool clearCache = true)
        {
            var old = await Get(entity.Id);
            var mode = entity as DataSourceEntity;
            if (mode != null && mode.DbSchema?.Password == "*******")
            {
                mode.DbSchema.Password = old.DbSchema.Password;
            }
            return await base.Modify(entity, clearCache);
        }

        public override async Task<bool> ModifyHasChange<TEntity>(TEntity entity, bool clearCache = true)
        {
            var old = await Get(entity.Id);
            var mode = entity as DataSourceEntity;
            if (mode != null && mode.DbSchema?.Password == "*******")
            {
                mode.DbSchema.Password = old.DbSchema.Password;
            }
            return await base.ModifyHasChange(entity, clearCache);
        }
    }
}
