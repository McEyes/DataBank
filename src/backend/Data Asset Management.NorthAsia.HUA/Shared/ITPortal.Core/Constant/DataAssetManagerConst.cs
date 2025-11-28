using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core
{
    public partial class DataAssetManagerConst
    {
        public static string DESKey = "ITPORTAL.DATAASSET";

        public static string HostUrl = "http://cnhuam0itds01/gateway/dataasset";

        
#if DEBUG
        public const string HttpContext_UserInfo = "DataAsset_UserInfo_Stg";
#elif STG
        public const string HttpContext_UserInfo = "DataAsset_UserInfo_Stg";
#else
        public const string HttpContext_UserInfo = "DataAsset_UserInfo";
#endif

#if DEBUG
        public const string RedisKey ="DataAssetManager_Stg:";
#elif STG
        public const string RedisKey = "DataAssetManager_Stg:";
#else
        public const string RedisKey ="DataAsset:";
#endif



        public const string RouteRedisKey = $"{RedisKey}API:Route";
        public const string RouteRedisListKey = $"{RedisKey}API:Route:List";

        public const string RouteLimitRedisKey = $"{RedisKey}RateLimit";

        public const string SqlClientRedisKey = $"{RedisKey}Sql.DB.Client";



        public const string DataApis_Count = $"{RedisKey}DataApis.Count";
        public const string DataApis_HashKey = $"{RedisKey}API.Hash";



        public const string DataSource_HashKey = $"{RedisKey}DataSourceEntity.Hash";
        public const string DataSource_ListKey = $"{RedisKey}DataSourceEntity.List";
        public const string DataSource_Catalog_HashKey = $"{RedisKey}DataSourceEntity:Catalog.Hash";


        public const string DataTable_HashKey = $"{RedisKey}DataTableEntity:Hash";
        public const string DataTable_ListKey = $"{RedisKey}DataTableEntity:List";
        public const string DataTable_UserHashKey = $"{RedisKey}DataTableEntity:User:Hash";
        public const string DataTable_VisitedStatisticsKey = $"{RedisKey}DataTableEntity:VisitedStatistics";
        public const string DataTable_CountKey = $"{RedisKey}DataTableEntity:Count";


        public const string DataColumn_HashKey = $"{RedisKey}DataColumnEntity:Hash";


        public const string DataCatalog_HashKey = $"{RedisKey}DataCatalogEntity:Hash";
        public const string DataCatalog_ListKey = $"{RedisKey}DataCatalogEntity:List";



        public const string SySDict_HashKey = $"{RedisKey}SysDictEntity:Hash";
        public const string SySDict_ListKey = $"{RedisKey}SysDictEntity:List";

        public const string API_tp_employee_base_infoAllKey = $"{RedisKey}tp_employee_base_info:List";
        ///**
        // * 数据资产可视化数据板 - 热门读取表
        // */
        //public const string VISUALIZATION_HOT_TABLE_KEY = $"{RedisKey}visualization:hotReadTable";

        ///**
        // * 数据资产可视化数据板 - 热门读取表访问总数
        // */
        //public const string VISUALIZATION_TABLE_TOTAL_VISITS_KEY = $"{RedisKey}visualization:tableTotalVisits";

        ///**
        // * 数据资产可视化数据板 - API 访问统计
        // */
        //public const string VISUALIZATION_API_VISITED_KEY = $"{RedisKey}visualization:apiVisited";

        ///**
        // * 数据资产可视化数据板 - 主题域访问统计
        // */
        //public const string VISUALIZATION_CATALOG_VISITED_KEY = $"{RedisKey}visualization:catalogVisited";




        public const string LogRecordEvent = $"{RedisKey}Log:Insert";
        public const string ElasticRecordEvent = $"{RedisKey}Elastic:Insert";
        public const string TrackLogRecordEvent = $"{RedisKey}TrackLog:Insert";
        public const string DataChangeRecordEvent = $"{RedisKey}DataChangeRecord:DiffLog";


        public const string ElasticCatalogEvent = $"{RedisKey}Elastic:Catalog";
        public const string ElasticDataSourceEvent = $"{RedisKey}Elastic:DataSource";
        public const string ElasticDataTableEvent = $"{RedisKey}Elastic:DataTable";
        public const string ElasticDataColumnEvent = $"{RedisKey}Elastic:DataColumn";
        public const string ElasticAPIEvent = $"{RedisKey}Elastic:API";


        /// <summary>
        /// 刷新api授权数据缓存
        /// </summary>
        public const string DataAuthRefreshEvent = $"{RedisKey}DataAuthRefresh";
    }
}
