using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataApiServer.Application.ThirdAppInfo.Dtos;
using DataAssetManager.DataTableServer.Application;

using Furion.DatabaseAccessor;
using Furion.JsonSerialization;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Exceptions;
using ITPortal.Core.Extensions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using Microsoft.Extensions.Logging;

using MySqlX.XDevAPI.Relational;

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using static Grpc.Core.Metadata;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataAuthApplyService : BaseService<DataAuthApplyEntity, DataAuthApplyDto, Guid>, IDataAuthApplyService, ITransient
    {
        private readonly IAssetClientsService _assetClientService;
        private readonly IEmployeeBaseInfoService _employeeService;
        private readonly IDataColumnService _columnService;
        private readonly IDataUserService _dataUserService;
        private readonly IDataTableService _tableService;
        private readonly FlowApplyProxyService _flowApplyProxyService;
        private readonly ILogger<DataAuthApplyService> _logger;

        public DataAuthApplyService(ISqlSugarClient db, IDistributedCacheService cache,
                IEmployeeBaseInfoService employeeService,
                DataColumnService columnService,
                IDataUserService dataUserService,
                FlowApplyProxyService flowApplyProxyService,
                ILogger<DataAuthApplyService> logger,
                IAssetClientsService assetClientService, IDataTableService tableService) : base(db, cache)
        {
            _assetClientService = assetClientService;
            _employeeService = employeeService;
            _columnService = columnService;
            _dataUserService = dataUserService;
            _flowApplyProxyService = flowApplyProxyService;
            _logger = logger;
            _tableService = tableService;
        }

        public override ISugarQueryable<DataAuthApplyEntity> BuildFilterQuery(DataAuthApplyDto filter)
        {
            return CurrentDb.Queryable<DataAuthApplyEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f =>SqlFunc.ToLower( f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.SmeId), f => SqlFunc.ToLower(f.SmeId) == filter.SmeId.ToLower())
                .WhereIF(filter.AppId.IsNotNullOrWhiteSpace(), f => SqlFunc.ToLower(f.AppId) == filter.AppId.ToLower())
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status);
        }



        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult> ApplyAuth(DataAuthApplyInfo authApplyDto)
        {
            CheckFlowValid(authApplyDto);

            var userId = authApplyDto.UserId ?? CurrentUser.UserId;
            var userName = authApplyDto.UserName ?? CurrentUser.Name;

            var entity = authApplyDto.Adapt<DataAuthApplyEntity>();
            List<DataAuthTableInfo> tableList = authApplyDto.TableList;

            //已有权限
            var authResult = await _assetClientService.CheckAuth(authApplyDto.Adapt<DataAuthCheckDto>());
            if (!authResult.Success) return authResult;
            var ownerTables = authResult.Data.Select(f => f.ObjectId).ToList();

            var errorMsg = new StringBuilder();

            var resultList = new List<DataAuthTableInfo>();
            foreach (var table in tableList)
            {//填充数据
                var tableInfo = await _tableService.GetInfo(id: table.Id);
                tableInfo.Adapt(table);
                table.OwnerId = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerId).Distinct());
                table.OwnerName = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerName).Distinct());
                table.OwnerDept = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerDept).Distinct());
                table.AllColumns = table.ColumnList?.Count == tableInfo.ColumnList.Count;
                if (table.AllColumns == true || table.ColumnList == null || table.ColumnList.Count == 0)
                    table.ColumnList = tableInfo.ColumnList;
                else
                    table.ColumnList = tableInfo.ColumnList.Where(d => table.ColumnList.Select(f => f.Id).Contains(d.Id)).ToList();

                resultList.Add(table);
            }

            using (var uow = CurrentDb.CreateContext())
            {
                foreach (var tableEntity in resultList)
                {
                    try
                    {
                        entity.Id = Guid.NewGuid();
                        entity.ColumnList = tableEntity.ColumnList;
                        entity.CtlCode = tableEntity.CtlCode;
                        entity.CtlName = tableEntity.CtlName;
                        entity.CtlId = tableEntity.CtlId;
                        entity.CtlRemark = tableEntity.CtlRemark;
                        entity.LevelId = tableEntity.LevelId;
                        entity.NeedSup = tableEntity.NeedSup;
                        entity.TableId = tableEntity.TableId;
                        entity.AppOwner = tableEntity.OwnerId;
                        entity.SmeId = tableEntity.OwnerId;
                        entity.SmeName = tableEntity.OwnerName;
                        entity.SmeDept = tableEntity.OwnerDept;
                        var maxlevelId = tableEntity.ColumnList.Max(f => f.LevelId);
                        if (!maxlevelId.IsNullOrWhiteSpace()) entity.LevelId = maxlevelId;
                        tableEntity.Adapt(entity);
                        entity.Id = Guid.NewGuid();
                        entity.Remark = JSON.Serialize(new dynamic[] { new { tableName = tableEntity.Alias, tableComment = tableEntity.TableComment, column = entity.ColumnList.Count > 0 ? string.Join(",", entity.ColumnList.Where(f => f.ColName != null && f.ColName != "").Select(x => x.ColName)) : null, api = tableEntity.Data } });
                        entity.TableName = tableEntity.CtlName + ":" + tableEntity.TableName;
                        entity.TableCode = tableEntity.CtlCode + ":" + tableEntity.Alias;
                        var result = await StartFlow(entity);
                        if (result.Success)
                        {
                            entity.Status = result.Success ? -1 : -2;
                            entity.FlowNo = result.Data.FlowNo;
                            await Save(authApplyDto, entity);
                        }
                        else
                        {
                            await _flowApplyProxyService.DeleteAsync(entity.Id);
                        }
                        return result;
                    }
                    catch (Exception ex)
                    {
                        await _flowApplyProxyService.DeleteAsync(entity.Id);
                        throw ex;
                    }
                }
                uow.Commit();
                //await RefreshCache();
            }
            return await Task.FromResult(new Result<bool>() { Data = true });
        }


        //[UnitOfWork(true)]
        //public async Task<ITPortal.Core.Services.IResult> ApplyAuth2(DataAuthApplyInfo authApplyDto)
        //{
        //    CheckFlowValid(authApplyDto);

        //    var userId = authApplyDto.UserId;
        //    var userName = authApplyDto.UserName;

        //    var entity = authApplyDto.Adapt<DataAuthApplyEntity>();
        //    List<DataAuthTableInfo> tableList = authApplyDto.TableList;

        //    //已有权限
        //    var authResult = await _assetClientService.CheckAuth(authApplyDto.Adapt<DataAuthCheckDto>());
        //    if (!authResult.Success) return authResult;
        //    var ownerTables = authResult.Data.Select(f => f.ObjectId).ToList();

        //    var errorMsg = new StringBuilder();

        //    var resultList = new List<DataAuthTableInfo>();
        //    foreach (var table in tableList)
        //    {//填充数据
        //        var tableInfo = await _tableService.GetInfo(id: table.Id);
        //        tableInfo.Adapt(table);
        //        table.OwnerId = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerId).Distinct());
        //        table.OwnerName = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerName).Distinct());
        //        table.OwnerDept = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerDept).Distinct());
        //        table.AllColumns = table.ColumnList?.Count == tableInfo.ColumnList.Count;
        //        if (table.AllColumns == true || table.ColumnList == null || table.ColumnList.Count == 0)
        //            table.ColumnList = tableInfo.ColumnList;
        //        else
        //            table.ColumnList = tableInfo.ColumnList.Where(d => table.ColumnList.Select(f => f.Id).Contains(d.Id)).ToList();

        //        resultList.Add(table);
        //    }

        //    using (var uow = CurrentDb.CreateContext())
        //    {
        //        foreach (var tableEntity in resultList)
        //        {
        //            try
        //            {
        //                entity.ColumnList = tableEntity.ColumnList;
        //                entity.CtlCode = tableEntity.CtlCode;
        //                entity.CtlName = tableEntity.CtlName;
        //                entity.CtlId = tableEntity.CtlId;
        //                entity.CtlRemark = tableEntity.CtlRemark;
        //                entity.LevelId = tableEntity.LevelId;
        //                var maxlevelId = tableEntity.ColumnList.Max(f => f.LevelId);
        //                if (!maxlevelId.IsNullOrWhiteSpace()) entity.LevelId = maxlevelId;
        //                tableEntity.Adapt(entity);
        //                var result = await StartFlow(tableEntity, entity, authApplyDto);
        //                //log.Info("申请结果：" + body);
        //                entity.Status = result.Success ? -1 : -2;
        //                await Save(authApplyDto, entity);
        //            }
        //            catch (Exception)
        //            {
        //                await _flowApplyProxyService.DeleteAsync(entity.FlowNo);
        //                throw;
        //            }
        //        }
        //        uow.Commit();
        //        await RefreshCache();
        //    }
        //    return await Task.FromResult(new Result<bool>() { Data = true });
        //}

        private bool CheckFlowValid(DataAuthApplyInfo authApplyDto)
        {
            if (authApplyDto.ApplyType.ToInt() <= 0)
            {
                throw new DataQueryException("申请类型不能为空");
            }
            else if (authApplyDto.ApplyType == AssetClients.Dtos.AuthApplyType.Application
                && authApplyDto.AppName.IsNullOrWhiteSpace())
            {
                throw new DataQueryException("应用申请时[APP Name]不能为空");
            }
            if (authApplyDto.UserId.IsNullOrWhiteSpace())
            {
                throw new DataQueryException("申请人不能为空");
            }
            if (authApplyDto.TableList == null || authApplyDto.TableList.Count <= 0)
            {
                throw new DataQueryException("申请表不能为空");
            }
            if (authApplyDto.Reason.IsNullOrWhiteSpace())
            {
                throw new DataQueryException("申请目的不能为空");
            }
            foreach (var table in authApplyDto.TableList)
            {
                if (table == null) throw new DataQueryException("申请表不能为null");
                table.AllColumns = table.ColumnList == null || table.ColumnList.Count <= 0;
            }

            return true;
        }



        [UnitOfWork(true)]
        public async Task<Result<FlowInstEntity>> StartFlow(DataAuthApplyEntity entity)
        {
            //查询表有哪些审批人
            List<MetaDataUserEntity> userList = await _dataUserService.Query(new MetaDataUserDto() { ObjectId = entity.TableId });
            var userMap = userList.GroupBy(x => x.Sort).ToDictionary(g => g.Key, g => g.Select(f => new ApproveUser() { UserId = f.UserId, UserName = f.UserName }).ToList());
            List<ApproveDto> appList = new List<ApproveDto>();
            foreach (var item in userMap)
            {
                ApproveDto app = new ApproveDto();
                app.Sort = item.Key;
                app.UserList = item.Value;
                appList.Add(app);
            }
            //查询申请人上级id --需要上级
            if (entity.NeedSup == 1 && !entity.IsPublicSecurityLevel)
            {
                JabusEmployeeInfo manage = await _employeeService.GetManagerAsync(entity.UserId);
                appList.Add(new ApproveDto() { Sort = appList.Count(), UserList = new List<ApproveUser>() { new ApproveUser() { UserId = manage.WorkNTID, UserName = manage.Name, } } });
            }
            var result = await InitApplyFlow(entity, appList);
            return result;
        }


        public async Task<Result<FlowInstEntity>> InitApplyFlow(DataAuthApplyEntity authEntity, List<ApproveDto> appList)
        {
            StartFlowDto flowData = new StartFlowDto()
            {
                FlowTempName = "ITPortal_DataAuthApplication",
                FormId = authEntity.Id,
                FormData = authEntity,
                Applicant = authEntity.UserId,
                ApplicantName = authEntity.UserName,
            };

            var tempInfo = await _flowApplyProxyService.GetFlowTempInfo("ITPortal_DataAuthApplication");
            var SmeATNode = tempInfo.Data.FlowActs.FirstOrDefault(f => f.ActName == "SME AT");
            if (SmeATNode == null) throw new AppFriendlyException("审批节点“SEM AT”名称错误，审批人配置失败", "5004");

            var nodeApproler = new FlowActApprover()
            {
                ActName = SmeATNode.ActName,
            };
            //添加owner审批人
            var ownerList = await _employeeService.GetEmployeeListAsync(authEntity.AppOwner);
            foreach (var ownerInfo in ownerList)
            {
                nodeApproler.ActorParms.Add(new StaffInfo()
                {
                    Ntid = ownerInfo.WorkNTID,
                    Department = ownerInfo.DepartmentName,
                    Email = ownerInfo.WorkEmail,
                    Name = ownerInfo.Name
                });
            }
            //添加主管，等其他审批人
            foreach (var item in appList)
            {
                foreach (var item2 in item.UserList)
                {
                    var empInfo = await _employeeService.GetEmployeeInfo(item2.UserId);
                    nodeApproler.ActorParms.Add(new StaffInfo()
                    {
                        Ntid = empInfo.WorkNTID,
                        Department = empInfo.DepartmentName,
                        Email = empInfo.WorkEmail,
                        Name = empInfo.Name
                    });
                }
            }

            flowData.ActApprovers.Add(nodeApproler);

            var result = await _flowApplyProxyService.InitFlowAsync(flowData);
            if (result.Success)
            {
                authEntity.FlowNo = result.Data.FlowNo;
                List<FlowActInstEntity> currentActivityList = result.Data.FlowActs.Where(f => f.ActStatus == ITPortal.Core.ProxyApi.Flow.Enums.ActivityStatus.Running).ToList();
                if (authEntity.IsPublicSecurityLevel)
                {//公共及别的流程自动审批
                    await Task.Run(() =>
                    {
                        Task.Delay(5000);
                        foreach (var activity in currentActivityList)
                        {
                            FlowAuditDto auditData = new FlowAuditDto()
                            {
                                Id = activity.Id,
                                ActOperate = ITPortal.Core.ProxyApi.Flow.Enums.ActivityStatus.Approval.ToString(),
                                AuditContent = "公共级别自动审批"
                            };
                            var result = _flowApplyProxyService.SendApproveAsync(auditData).Result;
                            if (!result.Success)
                            {
                                _logger.LogError($"自动审批失败:{auditData.ToJSON()}");
                            }
                        }
                    });
                }
            }
            return result;
        }



        /// <summary>
        /// 根据审批结果 输入入库
        /// </summary>
        /// <param name="flowCallBackData"></param>
        /// <returns></returns>
        [UnitOfWork(true)]
        public async Task<string> AuthBack(Result<FlowBackDataEntity> flowCallBackData)
        {
            if (flowCallBackData.Data == null || flowCallBackData.Data.FlowInst == null) throw new AppFriendlyException("Data Auth Apply CallBack error,no call back data param", "5201");
            var callBackData = flowCallBackData.Data;
            if (callBackData.FlowInst != null && (callBackData.FlowInst.FlowStatus == ITPortal.Core.ProxyApi.Flow.Enums.FlowStatus.Completed
                || callBackData.FlowInst.FlowStatus == ITPortal.Core.ProxyApi.Flow.Enums.FlowStatus.Cancel))
            {
                using (var uow = CurrentDb.CreateContext())
                {
                    var entity = await CurrentDb.Queryable<DataAuthApplyEntity>().FirstAsync(f => f.Id == callBackData.FlowInst.Id);
                    if (entity == null) throw new KeyNotFoundException($"Auth Back:{callBackData.FlowInst.Id} Data does not exist！");
                    var detail = await CurrentDb.Queryable<DataAuthApplyDetailEntity>().FirstAsync(f => f.FlowId == callBackData.FlowInst.Id);
                    if (detail == null) throw new KeyNotFoundException($"Auth Back:{callBackData.FlowInst.Id} Detailed data does not exist！");

                    if (callBackData.ActionType == ITPortal.Core.ProxyApi.Flow.Enums.FlowAction.Approval)
                    {
                        var userInfo = (await _employeeService.GetUsersAsync()).Data?.FirstOrDefault(f => entity.UserId.Equals(f.UserId, StringComparison.CurrentCultureIgnoreCase));
                        //流程通过，回写client和token信息
                        var client = new ApplyAuthCallbackDto()
                        {
                            ClientType = entity.ApplyType,
                            ClientId = entity.ApplyType == 2 ? entity.AppId : entity.UserId,
                            ClientName = entity.ApplyType == 2 ? entity.AppName : entity.UserName,
                            FlowId = entity.FlowId,
                            FlowNo = entity.FlowNo,
                            FlowUserId = entity.UserId,
                            FlowDetails = new List<DataAuthApplyDetailEntity>() { detail },
                            //Scopes = entity.ColumnList.Adapt<List<AssetClientScopesEntity>>(),
                            Description = entity.Reason,
                            Enabled = true,
                            Owner = entity.UserId,
                            OwnerName = entity.UserName,
                            OwnerDept = userInfo?.Department,
                            OwnerNtid = userInfo?.NtId,
                            TableOwner = entity.SmeId,
                            TableOwnerName = entity.SmeName,
                            TableOwnerDept = entity?.SmeDept,
                        };

                        var resultData = await _assetClientService.CreateClientInfo(client);
                        entity.Token = resultData.Data;
                        entity.Status = 0;
                    }
                    else if (callBackData.ActionType == ITPortal.Core.ProxyApi.Flow.Enums.FlowAction.Cancel)
                    {
                        entity.Status = -3;
                    }
                    else
                    {
                        entity.Status = 1;
                    }
                    entity.Approver = this.CurrentUser.UserId;
                    entity.AuditTime = entity.CompletionTime = DateTimeOffset.Now;
                    await ModifyHasChange(entity,false);
                    uow.Commit();
                }
            }
            return string.Empty;
        }



        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult> Save(DataAuthApplyInfo authApplyDto, DataAuthApplyEntity entity)
        {
            var result = new Result<bool>();
            using (var uow = CurrentDb.CreateContext())
            {
                var data = await Create(entity,false);

                List<DataAuthTableInfo> tableList = authApplyDto.TableList;
                var details = new List<DataAuthApplyDetailEntity>();
                foreach (var table in tableList)
                {
                    var item = table.Adapt<DataAuthApplyDetailEntity>();
                    item.FlowId = entity.Id;
                    item.FlowNo = entity.FlowNo;
                    item.TableColumns = table.ColumnList.Adapt<List<DataColumnEntity>>();
                    details.Add(item);
                }

                await CurrentDb.Storageable(details).ExecuteSqlBulkCopyAsync();
                uow.Commit();
            }
            return result;
        }

    }
}
