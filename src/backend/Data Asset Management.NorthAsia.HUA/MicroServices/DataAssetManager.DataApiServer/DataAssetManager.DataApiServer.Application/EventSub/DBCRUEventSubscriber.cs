using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;
using Elastic.Clients.Elasticsearch.TextStructure;

using Furion.EventBus;

using ITPortal.Core;
using ITPortal.Core.DataSource;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.EvenBus;
using ITPortal.Core.LightElasticSearch;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections.Generic;
using System.Text;

using static Elastic.Clients.Elasticsearch.JoinField;

namespace DataAssetManager.DataApiServer.Application.EventSub
{

    /// <summary>
    /// 数据库增删改查事件订阅
    /// </summary>
    public class DBCRUEventSubscriber : IEventSubscriber, ISingleton
    {
        private readonly ILogger<DBCRUEventSubscriber> _logger;
        //private readonly TopicDocumentProxyService _topicDocumentProxyService;
        private readonly IDataCatalogService _dataCatalogService;
        private readonly IDataSourceService _dataSourceService;
        private readonly IDataTableService _dataTableService;
        private readonly IDataColumnService _dataColumnService;
        //private readonly IDataApiService _dataApiService;
        private readonly AssetClientsService _assetClientsService;
        private readonly IOpenSearchService _openSearchService;
        //protected IDistributedCacheService _cache { get; private set; }
        public DBCRUEventSubscriber(ILogger<DBCRUEventSubscriber> logger,
            IDataCatalogService dataCatalogService,
            IDataSourceService dataSourceService,
            IDataTableService dataTableService,
            IDataColumnService dataColumnService,
            IDataApiService dataApiService,
            AssetClientsService assetClientsService,
            IDistributedCacheService cache,
            IOpenSearchService openSearchService)
        {
            _dataCatalogService = dataCatalogService;
            _dataSourceService = dataSourceService;
            _dataTableService = dataTableService;
            _dataColumnService = dataColumnService;
            //_dataApiService = dataApiService;
            _assetClientsService = assetClientsService;
            //_topicDocumentProxyService = topicDocumentProxyService;
            _openSearchService = openSearchService;
            //_cache = cache;
            _logger = logger;
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        private async Task ClearCache(Type sourceType)
        {
            var key = $"{DataAssetManagerConst.RedisKey}{sourceType.Name}";
            //await _cache.DelayRemoveAsync($"{key}:Hash");
            //await _cache.DelayRemoveAsync($"{key}:List");

            if (sourceType == typeof(AssetClientEntity))
            {
                await _assetClientsService.RefreshCache();
            }
            else if (sourceType == typeof(DataTableEntity))
            {
                await _dataTableService.RefreshCache();
            }
            //else if (sourceType == typeof(DataApiEntity))
            //{
            //   await _dataApiService.InitRoutes(true);
            //}
        }

        [EventSubscribe($"{DataAssetManagerConst.RedisKey}DB:Create")]
        public async Task CreateElastic(EventHandlerExecutingContext context)
        {
            var source = context.Source as DBEventSource;
            if (source != null && (source.ModelType != typeof(DataApiLogEntity) && source.ModelType != typeof(ApiTrackLogInfo)))
            {
                await Task.Delay(5000);//5秒后在更新数据
                try
                {
                    //清理缓存
                    await ClearCache(source.ModelType);
                    if (source.ModelType == typeof(DataCatalogEntity))
                    {
                        var topicList = await _dataCatalogService.AllFromCache();
                        var topic = source.GetPayload<DataCatalogEntity>();
                        await _openSearchService.CreateTopicDocument(topic, topicList.FirstOrDefault(f => f.Id == topic.ParentCtlId));
                    }
                    else if (source.ModelType == typeof(DataSourceEntity))
                    {
                        await _openSearchService.CreateTopicDocument(source.GetPayload<DataSourceEntity>());
                    }
                    else if (source.ModelType == typeof(DataTableEntity))
                    {
                        var entity = source.GetPayload<DataTableEntity>();//.Adapt<DataTableInfo>();
                        var topic = await _dataTableService.GetInfo(entity.Id);
                        //var dataColumnsList = await _dataColumnService.AsQueryable().ToListAsync();
                        await _openSearchService.CreateTopicDocument(topic);
                    }
                    else if (source.ModelType == typeof(DataApiEntity))
                    {
                        var topic = source.GetPayload<DataApiEntity>().Adapt<RouteInfo>();
                        var topicList = await _dataCatalogService.AllFromCache();
                        var dataSourceList = await _dataSourceService.AllFromCache();
                        var dataTableList = await _dataTableService.AllFromCache();
                        await _openSearchService.CreateTopicDocument(topic, dataTableList.FirstOrDefault(f => f.Id == topic.TableId), dataSourceList.FirstOrDefault(f => f.Id == topic.SourceId), topicList.FirstOrDefault(f => f.Id == topic.CtlId));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{context.Source.EventId}:Error:{ex.Message}\r\n{ex.StackTrace}\r\n{context.Source.ToJSON()}");
                }
                _logger.LogError($"DBCRU Event Subscriber:{context.Source.EventId}");
            }
        }

        [EventSubscribe($"{DataAssetManagerConst.RedisKey}DB:Modify")]
        public async Task UpdataElastic(EventHandlerExecutingContext context)
        {
            var source = context.Source as DBEventSource;
            if (source != null)
            {
                await Task.Delay(5000);//5秒后在更新数据
                try
                {
                    //清理缓存
                    var key = $"{DataAssetManagerConst.RedisKey}{source.ModelType.Name}";
                    await ClearCache(source.ModelType);
                    if (source.ModelType == typeof(DataCatalogEntity))
                    {
                        var topicList = await _dataCatalogService.AllFromCache();
                        var topic = source.GetPayload<DataCatalogEntity>();
                        await _openSearchService.UpdateTopicDocumentAsync(topic, topicList.FirstOrDefault(f => f.Id == topic.ParentCtlId));
                    }
                    else if (source.ModelType == typeof(DataSourceEntity))
                    {
                        await _openSearchService.UpdateTopicDocumentAsync(source.GetPayload<DataSourceEntity>());
                    }
                    else if (source.ModelType == typeof(DataTableEntity))
                    {
                        var entity = source.GetPayload<DataTableEntity>();//.Adapt<DataTableInfo>();
                        var topic = await _dataTableService.GetInfo(entity.Id);
                        //var dataColumnsList = await _dataColumnService.AsQueryable().ToListAsync();
                        await _openSearchService.UpdateTopicDocumentAsync(topic);//, dataColumnsList.Where(f => f.TableId == topic.Id).ToList()
                    }
                    else if (source.ModelType == typeof(DataApiEntity))
                    {
                        var topic = source.GetPayload<DataApiEntity>().Adapt<RouteInfo>();
                        var topicList = await _dataCatalogService.AllFromCache();
                        var dataSourceList = await _dataSourceService.AllFromCache();
                        var dataTableList = await _dataTableService.AllFromCache();
                        await _openSearchService.UpdateTopicDocumentAsync(topic, dataTableList.FirstOrDefault(f => f.Id == topic.TableId), dataSourceList.FirstOrDefault(f => f.Id == topic.SourceId), topicList.FirstOrDefault(f => f.Id == topic.CtlId));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{context.Source.EventId}:Error:{ex.Message}\r\n{ex.StackTrace}\r\n{context.Source.ToJSON()}");
                }
            }
            else
            {
                _logger.LogError($"{context.Source.EventId}:Error:{context.Source.ToJSON()}");
            }
        }


        [EventSubscribe($"{DataAssetManagerConst.RedisKey}DB:Delete")]
        public async Task DeleteElastic(EventHandlerExecutingContext context)
        {
            var source = context.Source as DBEventSource;
            if (source != null && source.Payload.GetType() == typeof(string))
            {
                try
                {
                    await _openSearchService.DeleteTopicDocumentAsync(source.Payload);
                    //清理缓存
                    await ClearCache(source.ModelType);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{context.Source.EventId}:Error:{ex.Message}\r\n{ex.StackTrace}\r\n{context.Source.ToJSON()}");
                }
            }
            else
            {
                _logger.LogError($"{context.Source.EventId}:Error:{context.Source.ToJSON()}");
            }
        }


        [EventSubscribe($"{DataAssetManagerConst.RedisKey}DB:Deletes")]
        public async Task DeletesElastic(EventHandlerExecutingContext context)
        {
            var source = context.Source as DBEventSource;
            if (source != null && source.ModelType == typeof(string))
            {
                try
                {
                    await _openSearchService.DeleteTopicDocumentAsync(source.Payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{context.Source.EventId}:Error:{ex.Message}\r\n{ex.StackTrace}\r\n{context.Source.ToJSON()}");
                }
            }
            else
            {
                _logger.LogError($"{context.Source.EventId}:Error:{context.Source.ToJSON()}");
            }
        }


        [EventSubscribe($"{DataAssetManagerConst.RedisKey}DB:DiffLog")]
        public async Task DiffLog(EventHandlerExecutingContext context)
        {
            var source = context.Source as DBEventSource;
            if (source != null && source.ModelType == typeof(string))
            {
                try
                {
                    await _openSearchService.DeleteTopicDocumentAsync(source.Payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{context.Source.EventId}:Error:{ex.Message}\r\n{ex.StackTrace}\r\n{context.Source.ToJSON()}");
                }
            }
            else
            {
                _logger.LogError($"{context.Source.EventId}:Error:{context.Source.ToJSON()}");
            }
        }


    }
}
