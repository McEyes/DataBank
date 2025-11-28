using DataBankDashbord.Application.DashbordMetaData.Dtos;
using Elastic.Clients.Elasticsearch.Core.HealthReport;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Furion.EventBus;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;
using Pipelines.Sockets.Unofficial.Buffers;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Services
{
    public class DashbordMetaDataService : BaseService<DashbordMetadataEntity, DashbordMetadataDto, Guid>, IDashbordMetaDataService, ITransient
    {
        private readonly IEventPublisher _eventPublisher;
        public DashbordMetaDataService(ISqlSugarClient db, IEventPublisher eventPublisher, IDistributedCacheService cache) : base(db, cache, true, true)
        {
            _eventPublisher = eventPublisher;
        }

        public override ISugarQueryable<DashbordMetadataEntity> BuildFilterQuery(DashbordMetadataDto filter)
        {

            return CurrentDb.Queryable<DashbordMetadataEntity>();
        }

        public async Task<List<DashbordMetadataDto>> GetDashbordList()
        {
            return (await CurrentDb.Queryable<DashbordMetadataEntity>().OrderByDescending(f => f.CreateTime).ToListAsync()).Adapt<List<DashbordMetadataDto>>();
        }

        public async Task<List<DashbordMetadataDto>> GetDashbordListForOwnerId(string ownerId)
        {

            return (await CurrentDb.Queryable<DashbordMetadataEntity>().Where(f => f.OwnerId == ownerId).OrderByDescending(f => f.CreateTime).ToListAsync()).Adapt<List<DashbordMetadataDto>>();

        }

        public async Task<List<DashbordMetadataDto>> getDashbordListForColumnId(List<string> columnIdList)
        {
            return (await CurrentDb.Queryable<DashbordMetadataEntity>().Where(f => columnIdList.Contains(f.ColumnId)).OrderByDescending(f => f.CreateTime).ToListAsync()).Adapt<List<DashbordMetadataDto>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<IndicatorCodeDto>> GetAllIndicatorCode()
        {

            var list = await CurrentDb.Queryable<MetadataColumnEntity, MetadataTableEntity, MetadataSourceEntity, AssetDictItemEntity>((a, b, c, d) => new JoinQueryInfos(
                   JoinType.Inner, a.table_id == b.id,
                   JoinType.Inner, c.id == a.source_id,
                   JoinType.Inner, d.item_text == b.owner_depart
                   )).Where((a, b, c, d) => d.dict_id == "0775606144814157824")
               .OrderBy((a, b, c, d) => new { b.owner_depart, b.table_name, a.id })
            .Select((a, b, c, d) => new IndicatorCodeDto()
            {
                SourceName = c.source_name,
                SourceCode = c.source_code,
                TableName = b.table_name,
                OwnerDepart = b.owner_depart,
                DepartCode = d.item_value,
                ColumnName = a.column_name,
                ColumnId = a.id
            }).ToListAsync();

            var allTable = list.Select(s => s.TableName).Distinct().ToList();
            var dis = new Dictionary<string, string>();
            var tableIndex = 1;
            foreach (var item in allTable)
            {
                var index = 1;
                var tbc = list.Where(s => s.TableName == item);
                foreach (var tableColumn in tbc)
                {
                    if (string.IsNullOrWhiteSpace(tableColumn.ColumnName))
                        continue;
                    var key = tableColumn.ColumnName.ToLower();
                    if (dis.ContainsKey(key))
                    {

                        //tableColumn.TableCode = dis[key].Substring(7, 4);

                        //tableColumn.TableIndexCode = string.Format("{0}", int.Parse(tableColumn.TableCode));


                        tableColumn.ColumnIndexCode = dis[key].Substring(11, 3);
                        var oldColumnindx = 0;
                        int.TryParse(tableColumn.ColumnIndexCode, out oldColumnindx);
                        tableColumn.ColumnIndexCode = string.Format("{0}", oldColumnindx);


                        tableColumn.IndicatorCode = dis[key];
                    }
                    else
                    {
                        tableColumn.TableIndexCode = string.Format("{0}", tableIndex);
                        tableColumn.TableCode = tableIndex.ToString("D4");
                        var codel = index.ToString("D3");
                        tableColumn.ColumnIndexCode = string.Format("{0}", index);
                        tableColumn.ColumnCode = codel;
                        tableColumn.IndicatorCode = string.Format("H{0}{1}{2}{3}", tableColumn.DepartCode, tableColumn.SourceCode, tableColumn.TableCode, codel);
                        dis.Add(tableColumn.ColumnName.ToLower(), tableColumn.IndicatorCode);
                        index++;
                    }
                }
                var tbmodel = await CurrentDb.Queryable<MetadataTableEntity>().Where(s => s.table_name == item).FirstAsync();
                tbmodel.table_code = string.Format("{0}", tableIndex);
                CurrentDb.Updateable(tbmodel).ExecuteCommand();

                tableIndex++;
            }
            foreach (var item in list)
            {

                var model = await CurrentDb.Queryable<MetadataColumnEntity>().Where(s => s.id == item.ColumnId).FirstAsync();
                model.indicator_code = item.IndicatorCode;
                model.column_code = item.ColumnIndexCode;
                CurrentDb.Updateable(model).ExecuteCommand();

            }
            return list;
        }






        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CraeteIndicatorCode()
        {
            try
            {


                var list = await CurrentDb.Queryable<MetadataColumnEntity, MetadataTableEntity, MetadataSourceEntity, MetadataAuthorizeOwnerEntity, EmployeeBaseInfoEntity, AssetDictItemEntity>((a, b, c, d, e, f) => new JoinQueryInfos(
                        JoinType.Left, a.table_id == b.id,
                        JoinType.Left, c.id == a.source_id,
                        JoinType.Left, b.id == d.object_id,
                        JoinType.Left, e.ntid == d.owner_id,
                        JoinType.Left, f.item_text == e.department_name && f.dict_id == "0775606144814157824"
                        )).Where((a, b, c, d, e, f) => !string.IsNullOrEmpty(f.item_value))
                    .OrderBy((a, b, c, d, e, f) => new { e.department_name, b.table_name, a.id })
                 .Select((a, b, c, d, e, f) => new IndicatorCodeDto()
                 {
                     SourceName = c.source_name,
                     SourceCode = c.source_code,
                     TableName = b.table_name,
                     TableId = b.id,
                     TableCode = b.table_code,
                     OwnerDepart = e.department_name,
                     DepartCode = f.item_value,
                     ColumnName = a.column_name.ToLower(),
                     ColumnCode = a.column_code,
                     IndicatorCode = a.indicator_code,
                     ColumnIndexCode = b.table_code,
                     ColumnId = a.id
                 }).ToListAsync();


                var ss = list.Where(s => s.TableName == "tp_ods_nexim_glue_materials").ToList();

                //获取已经存在的tabke
                #region 更新新增列
                var exisisTable = list.Where(s => !string.IsNullOrEmpty(s.IndicatorCode)).Select(s => s.TableName).Distinct().ToList();

                //获取需要更新的数据---并且之前有生成的
                var neetUpdateTB = list.Where(s => string.IsNullOrEmpty(s.IndicatorCode) && exisisTable.Contains(s.TableName));
                {
                    var allTable = neetUpdateTB.Select(s => s.TableName).Distinct().ToList();

                    //循环需要更新的table name
                    foreach (var item in allTable)
                    {
                        //获取对应表的无IndicatorCode列
                        var tbc = neetUpdateTB.Where(s => s.TableName == item.Trim() && string.IsNullOrEmpty(s.IndicatorCode)).ToList();

                        //获取当前列的最大列序号
                        //var modessssl = list.Where(s => s.TableName == item && !string.IsNullOrEmpty(s.DepartCode) && !string.IsNullOrEmpty(s.IndicatorCode) && !string.IsNullOrEmpty(s.ColumnCode) && s.IndicatorCode.Contains(s.DepartCode)).FirstOrDefault();
                        var model = list.Where(s => s.TableName == item.Trim() && !string.IsNullOrEmpty(s.DepartCode) && !string.IsNullOrEmpty(s.IndicatorCode) && !string.IsNullOrEmpty(s.ColumnCode) && s.IndicatorCode.Contains(s.DepartCode)).OrderByDescending(s => int.Parse(s.ColumnCode)).FirstOrDefault();
                        var indexNum = 1;
                        if (model != null)
                        {
                            indexNum = int.Parse(model.ColumnCode) + 1;
                        }
                        var dis = new Dictionary<string, string>();
                        var index = indexNum;
                        foreach (var tableColumn in tbc)
                        {
                            if (string.IsNullOrWhiteSpace(tableColumn.ColumnName))
                                continue;
                            var key = tableColumn.ColumnName.ToLower();
                            tableColumn.TableCode = index.ToString("D4");

                            //查询历史列，看看是否存在，如果存在则直接存进字典中
                            var old = list.Where(s => !string.IsNullOrEmpty(s.IndicatorCode) && s.ColumnName == key);
                            if (old.Any())
                            {
                                if (old != null)
                                {
                                    if (!dis.ContainsKey(key))
                                    {
                                        dis.Add(key, old.FirstOrDefault().IndicatorCode);
                                    }

                                }
                            }

                            if (dis.ContainsKey(key))
                            {
                                tableColumn.ColumnIndexCode = dis[key].Substring(11, 3);
                                var oldColumnindx = 0;
                                int.TryParse(tableColumn.ColumnIndexCode, out oldColumnindx);
                                tableColumn.ColumnIndexCode = string.Format("{0}", oldColumnindx);
                                tableColumn.IndicatorCode = dis[key];
                            }
                            else
                            {
                                var codel = index.ToString("D3");
                                tableColumn.ColumnIndexCode = string.Format("{0}", index);
                                tableColumn.ColumnCode = codel;
                                tableColumn.IndicatorCode = string.Format("H{0}{1}{2}{3}", tableColumn.DepartCode, tableColumn.SourceCode, tableColumn.TableCode, codel);
                                dis.Add(tableColumn.ColumnName.ToLower(), tableColumn.IndicatorCode);
                                index++;
                            }
                        }

                        foreach (var updateitem in tbc)
                        {

                            var model_upodae = await CurrentDb.Queryable<MetadataColumnEntity>().Where(s => s.id == updateitem.ColumnId).FirstAsync();
                            model_upodae.indicator_code = updateitem.IndicatorCode;
                            model_upodae.column_code = updateitem.ColumnIndexCode;
                            CurrentDb.Updateable(model_upodae).ExecuteCommand();
                        }

                    }

                }

                #endregion


                //获取需要更新的数据---并且之前没生成的
                var neetADDTB = list.Where(s => string.IsNullOrEmpty(s.IndicatorCode) && !exisisTable.Contains(s.TableName));
                if (neetADDTB.Any())
                {

                    ///获取之前tablecode最大值
                    var topTB = await CurrentDb.Queryable<MetadataTableEntity>().Where(s => !string.IsNullOrEmpty(s.table_code)).OrderByDescending(s => int.Parse(s.table_code)).FirstAsync();

                    //获取插入列数据源
                    var allTable = neetADDTB.Select(s => s.TableName).Distinct().ToList();
                    var dis = new Dictionary<string, string>();
                    var tableIndex = int.Parse(topTB.table_code) + 1;
                    foreach (var item in allTable)
                    {
                        var index = 1;
                        var tbc = neetADDTB.Where(s => s.TableName == item.Trim()).ToList();
                        foreach (var tableColumn in tbc)
                        {
                            if (string.IsNullOrWhiteSpace(tableColumn.ColumnName))
                                continue;
                            var key = tableColumn.ColumnName.ToLower().Trim();

                            //查询历史列，看看是否存在，如果存在则直接存进字典中
                            var old = list.Where(s => !string.IsNullOrEmpty(s.IndicatorCode) && s.ColumnName.ToLower() == key);
                            if (old.Any())
                            {
                                if (!dis.ContainsKey(key))
                                {
                                    dis.Add(key, old.First().IndicatorCode);
                                }

                            }

                            if (dis.ContainsKey(key))
                            {
                                tableColumn.ColumnIndexCode = dis[key].Substring(11, 3);
                                var oldColumnindx = 0;
                                int.TryParse(tableColumn.ColumnIndexCode, out oldColumnindx);
                                tableColumn.ColumnIndexCode = string.Format("{0}", oldColumnindx);
                                tableColumn.IndicatorCode = dis[key];
                            }
                            else
                            {
                                tableColumn.TableIndexCode = string.Format("{0}", tableIndex);
                                tableColumn.TableCode = tableIndex.ToString("D4");
                                var codel = index.ToString("D3");
                                tableColumn.ColumnIndexCode = string.Format("{0}", index);
                                tableColumn.ColumnCode = codel;
                                tableColumn.IndicatorCode = string.Format("H{0}{1}{2}{3}", tableColumn.DepartCode, tableColumn.SourceCode, tableColumn.TableCode, codel);
                                dis.Add(tableColumn.ColumnName.ToLower(), tableColumn.IndicatorCode);
                                index++;
                            }
                        }
                        var tbmodel = await CurrentDb.Queryable<MetadataTableEntity>().Where(s => s.table_name == item).FirstAsync();
                        tbmodel.table_code = string.Format("{0}", tableIndex);
                        CurrentDb.Updateable(tbmodel).ExecuteCommand();


                        foreach (var itemadd in tbc)
                        {

                            var model = await CurrentDb.Queryable<MetadataColumnEntity>().Where(s => s.id == itemadd.ColumnId).FirstAsync();
                            model.indicator_code = itemadd.IndicatorCode;
                            model.column_code = itemadd.ColumnIndexCode;
                            CurrentDb.Updateable(model).ExecuteCommand();

                        }


                        tableIndex++;
                    }


                }
            }
            catch (Exception ex)
            {
                var ss = ex.Message;
                var ssss = ex.Message;

            }


            return true;
        }



        public async Task<bool> UpdateDashbordStatu(apiDashbordDto dto)
        {

            if (dto != null && dto.list.Any())
            {
                try
                {

                    foreach (var item in dto.list)
                    {

                        var model = CurrentDb.Queryable<DashbordMetadataEntity>().Where(s => s.SourceId == item.source_id && s.ColumnId == item.column_id && s.TableName == item.table_name).First();

                        if (model == null)
                        {
                            model = new DashbordMetadataEntity();
                            model.Id = Guid.NewGuid();
                            model.SourceId = item.source_id;
                            model.IsDashboard = item.IsDashboard;
                            model.IsIndicator = item.IsIndicator;
                            model.CreateTime = DateTime.Now;
                            model.CreateBy = base.CurrentUser.UserId;
                            model.TableName = item.table_name;
                            model.ColumnId = item.column_id;
                            model.ColumnName = item.column_name;
                            model.OwnerId = item.owner_id;
                            model.IndicatorCode = item.IndicatorCode;
                            await CurrentDb.Insertable(model).ExecuteCommandAsync();
                        }
                        else
                        {
                            model.IsDashboard = item.IsDashboard;
                            model.IsIndicator = item.IsIndicator;
                            model.IndicatorCode = item.IndicatorCode;
                            model.UpdateTime = DateTime.Now;
                            model.UpdateBy = base.CurrentUser.UserId;
                            await CurrentDb.Updateable(model).ExecuteCommandAsync();
                        }
                    }

                }
                catch (Exception)
                {

                    return false;
                }
            }

            return true; ;
        }


    }
}
