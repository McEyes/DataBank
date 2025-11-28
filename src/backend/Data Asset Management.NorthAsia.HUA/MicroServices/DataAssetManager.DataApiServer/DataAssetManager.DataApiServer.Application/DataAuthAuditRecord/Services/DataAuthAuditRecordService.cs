using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataColumn.Dtos;
using DataAssetManager.DataApiServer.Application.DataTable.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataTableServer.Application;

using Furion.DatabaseAccessor;
using Furion.JsonSerialization;

using ITPortal.Core.DistributedCache;
using ITPortal.Core.Exceptions;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.ProxyApi.Flow.Dto;
using ITPortal.Core.Services;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Text;
using System.Text.RegularExpressions;

namespace DataAssetManager.DataApiServer.Application
{
    public class DataAuthAuditRecordService : BaseService<DataAuthAuditRecordEntity, DataAuthAuditRecordDto, string>, IDataAuthAuditRecordService, ITransient
    {
        private readonly IDataAuthorizeUserService _authorizeUserService;
        //private readonly string HomeUrl;
        //private readonly string WorkflowInitUrl = "/homeapi/FlowDataGrantApply/InitFlow";

        //private readonly IUserService _userService;
        private readonly IEmployeeBaseInfoService _employeeService;
        private readonly IDataColumnService _columnService;
        private readonly IDataUserService _dataUserService;
        private readonly FlowApplyProxyService _flowApplyProxyService;
        private readonly ILogger<DataAuthAuditRecordService> _logger;

        public DataAuthAuditRecordService(ISqlSugarClient db, IDistributedCacheService cache,
             IEmployeeBaseInfoService employeeService,
             DataColumnService columnService,
             IDataUserService dataUserService,
             FlowApplyProxyService flowApplyProxyService,
             ILogger<DataAuthAuditRecordService> logger,
            IDataAuthorizeUserService authorizeUserService) : base(db, cache)
        {
            _authorizeUserService = authorizeUserService;
            //HomeUrl = App.GetConfig<string>("RemoteApi:HomeUrl");
            //var flowInitUrl = App.GetConfig<string>("RemoteApi:WorkflowInitUrl");
            //if (!flowInitUrl.IsNullOrWhiteSpace()) WorkflowInitUrl = flowInitUrl;
            _employeeService = employeeService;
            _columnService = columnService;
            _dataUserService = dataUserService;
            _flowApplyProxyService = flowApplyProxyService;
            _logger = logger;
        }

        public override ISugarQueryable<DataAuthAuditRecordEntity> BuildFilterQuery(DataAuthAuditRecordDto filter)
        {
            return CurrentDb.Queryable<DataAuthAuditRecordEntity>()
                .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), f => f.Id == filter.Id)
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApplyFormNo), f =>SqlFunc.ToLower( f.ApplyFormNo) == filter.ApplyFormNo.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.UserId), f => SqlFunc.ToLower(f.UserId) == filter.UserId.ToLower())
                .WhereIF(!string.IsNullOrWhiteSpace(filter.TableId), f => SqlFunc.JsonField(f.TableId, "$.arrayField[*].tableId").Contains(filter.TableId))
                .WhereIF(!string.IsNullOrWhiteSpace(filter.ApiToken), f => SqlFunc.ToLower(f.ApiToken) == filter.ApiToken.ToLower())
                .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status);
        }

        //public async Task<Result<List<JabusUserInfo>>> GetUserInfo()
        //{
        //    return await _userService.GetUsersAsync();
        //    //return null;
        //}

        //public async Task<Result<JabusEmployeeInfo>> GetEmployeeAsync(string ntid)
        //{
        //    return await _employeeService.GetEmployeeAsync(ntid);
        //}

        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult> ApplyAuth(DataAuthApply authApplyDto)
        {
            //var url = HomeUrl + WorkflowInitUrl;
            var userId = authApplyDto.UserId;
            var userName = authApplyDto.UserName;

            var entity = new DataAuthAuditRecordEntity();
            //Snowflake snowflake = IdUtil.getSnowflake(1, 1);

            entity.UserId = userId;  //申请人Id
            entity.UpdateBy = CurrentUser?.UserName ?? "test";
            if (!string.IsNullOrEmpty(authApplyDto.Remark))
            {
                entity.Remark = authApplyDto.Remark;
            }
            if (!string.IsNullOrEmpty(authApplyDto.Reason))
            {
                entity.Reason = authApplyDto.Reason;
            }
            if (authApplyDto.RequireDate != null)
            {
                entity.RequireTime = authApplyDto.RequireDate;
            }

            var tableList = authApplyDto.TableList;

            //已有权限
            var authList = await _authorizeUserService.Query(new DataAuthorizeUserDto() { UserId = userId });
            var ownerTables = authList.Select(f => f.ObjectId).Distinct().ToList();

            //正在申请
            var recordList = await CurrentDb.Queryable<DataAuthAuditRecordEntity>().
                Where(f => f.UserId == userId).In("status", new string[] { "-1", "-2" }).ToListAsync();
            var recordTables = new List<string>();
            recordList.ForEach(f => recordTables.AddRange(f.TableId.Select(d => d.tableId)));
            recordTables = recordTables.Distinct().ToList();

            var resultList = new List<DataTableInfo>();
            tableList.ForEach(table =>
            {
                if (!table.OwnerId.IsNullOrWhiteSpace() && !ownerTables.Contains(table.Id) && !recordTables.Contains(table.Id))
                {
                    var ownsId = table.OwnerId?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                    ownsId.Remove(userId);
                    if (ownsId.Count == 0) return;

                    string[] splitName = table.OwnerName.Split(',');
                    HashSet<string> ownsNames = new HashSet<string>(splitName);
                    ownsNames.Remove(userName);

                    table.OwnerId = string.Join(",", ownsId);
                    table.OwnerName = string.Join(",", ownsNames);
                    resultList.Add(table);
                }
            });

            using (var uow = CurrentDb.CreateContext())
            {
                foreach (var tableEntity in resultList)
                {
                    try
                    {
                        var result = await StartFlow(tableEntity, entity, authApplyDto);
                        //log.Info("申请结果：" + body);
                        entity.Status = result.Success ? -1 : -2;
                        await Create(entity);
                    }
                    catch (Exception e)
                    {
                        await _flowApplyProxyService.DeleteAsync(entity.ApplyFormNo);
                        throw;
                    }
                }
                uow.Commit();
            }
            return await Task.FromResult(new Result<bool>() { Data = true });
        }

        [UnitOfWork(true)]
        private async Task<Result> StartFlow(DataTableInfo tableEntity, DataAuthAuditRecordEntity entity, DataAuthApply authApplyDto)
        {
            //查询申请人上级id --需要上级
            string employeeId = "";
            if (tableEntity.NeedSup != null && tableEntity.NeedSup == 1)
            {
                string ntId = GetNtId(authApplyDto.UserName);
                employeeId = await _employeeService.GetManagerNtIdAsync(ntId);
            }

            //JSONArray jsonArray = new JSONArray();
            //string id = snowflake.NextIdStr();
            //string no = "JB_DataAuth_" + DateTimeOffset.Now.ToString("yyyy-MM-dd");
            //var count = _cache.GetInt(no, () => 1, TimeSpan.FromDays(1)) ?? 1;
            var formid = Guid.NewGuid(); //申请Id;
            entity.ApplyFormId = formid.ToString(); //申请Id
            //entity.ApplyFormNo = no + "_" + count.ToString("D4");// decimalFormat.Format(count);//申请编号  JB_DataAuth_2025-03-26_0001
            entity.SmeId = tableEntity.OwnerId; // 审核人Id
            entity.SmeName = tableEntity.OwnerName; // 审核人姓名
            entity.TableId = new AuthTableInfo[] { new AuthTableInfo() { ctlId = tableEntity.CtlId, ctlName = tableEntity.CtlName, tableId = tableEntity.Id } };
            //查询table的字段
            List<DataColumnEntity> columnList = await _columnService.Query(new DataColumnDto() { TableId = tableEntity.Id });
            HashSet<string> column = columnList.Where(f => f.ColComment != null && f.ColComment != "").Select(x => x.ColComment).Distinct().ToHashSet();
            if (column.Count == 0)
            {
                column = columnList.Where(f => f.ColName != null && f.ColName != "").Select(x => x.ColName).Distinct().ToHashSet();
            }
            entity.Remark = JSON.Serialize(new dynamic[] { new { tableName = tableEntity.Alias, tableComment = tableEntity.TableComment, column = string.Join(",", column), api = tableEntity.Data } });


            string tabName = tableEntity.CtlName + ":" + tableEntity.Alias;
            string[] tables = new string[1];
            tables[0] = tabName;
            entity.TableName = tabName;
            string tabCode = tableEntity.CtlCode + ":" + tableEntity.Alias;
            entity.TableCode = tabCode;
            //查询表有哪些审批人
            List<MetaDataUserEntity> userList = await _dataUserService.Query(new MetaDataUserDto() { ObjectId = tableEntity.Id });
            var userMap = userList.GroupBy(x => x.Sort).ToDictionary(g => g.Key, g => g.Select(f => new ApproveUser() { UserId = f.UserId, UserName = f.UserName }).ToList());
            List<ApproveDto> appList = new List<ApproveDto>();
            foreach (var item in userMap)
            {
                ApproveDto app = new ApproveDto();
                app.Sort = item.Key;
                app.UserList = item.Value;
                appList.Add(app);
            }

            //var nodeList = GetNodeList(appList, tableEntity.OwnerId, employeeId, authApplyDto.UserId);
            //entity.ApplyNode = nodeList.ToString();
            //var fromData = new StartFlowDto()
            //{
            //    FormId = formid,
            //    FormNo = entity.ApplyFormNo,
            //    Applicant = entity.UserId,
            //    SMEId = entity.SmeId,
            //    Remark = entity.Remark,
            //    Reason = entity.Reason,
            //    Tables = tables,
            //    NodeList = nodeList
            //};
            //var result = await _flowApplyProxyService.InitFlowAsync(fromData);
            //if (result.Success) _cache.SetInt(no, count + 1, TimeSpan.FromDays(1));
            //else
            //{
            //    _logger.LogError($"{entity.ApplyFormNo}流程发起失败！\r\n失败原因[{result.Code}]：{result.Msg}");
            //    _logger.LogError($"{entity.ApplyFormNo}流程参数信息:{JSON.Serialize(fromData)}");
            //}
            //return result;
            return new Result();
        }


        public static string GetNtId(string str)
        {
            // 定义正则表达式，匹配括号内的内容
            string pattern = @"\((.*?)\)";
            Match match = Regex.Match(str, pattern);

            // 如果找到匹配项，返回第一个捕获组的内容
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            // 如果没有找到匹配项，返回原始字符串
            return str;
        }
        public List<ApplyFlowCommonNodeInput> GetNodeList(List<ApproveDto> approverList, string dataOwner, string manageId, string userId)
        {
            var nodeList = new List<ApplyFlowCommonNodeInput>();
            int index;

            if (!string.IsNullOrEmpty(manageId))
            {
                index = 2;
                nodeList.Add(new ApplyFlowCommonNodeInput
                {
                    HasRole = false,
                    PassType = 1,
                    PassStatus = "SME AT",
                    NodeIndex = 1,
                    RejectIndex = 0,
                    ReturnIndex = 0,
                    ApproverList = new List<ApplyFlowApproverItemInput>() { new ApplyFlowApproverItemInput { UserId = manageId } }
                });
            }
            else
            {
                index = 1;
            }

            var approverList1 = new List<ApplyFlowApproverItemInput>();
            var split = dataOwner.Split(',');
            foreach (var owner in split)
            {
                var trim = owner.Trim();
                if (!trim.Equals(userId, StringComparison.OrdinalIgnoreCase))
                {
                    approverList1.Add(new ApplyFlowApproverItemInput()
                    {
                        UserId = trim
                    });
                }
            }

            nodeList.Add(new ApplyFlowCommonNodeInput
            {
                HasRole = false,
                PassType = 1,
                PassStatus = "SME AT",
                NodeIndex = index,
                RejectIndex = 0,
                ReturnIndex = 0,
                ApproverList = approverList1
            });
            index++;

            approverList.Sort((a, b) => a.Sort.CompareTo(b.Sort));
            for (int i = 0; i < approverList.Count; i++)
            {
                var approval = approverList[i];
                var userList = approval.UserList;
                var approverList3 = new List<ApplyFlowApproverItemInput>();
                foreach (var user in userList)
                {
                    if (!user.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                    {
                        var approver = new ApplyFlowApproverItemInput
                        {
                            UserId = user.UserId
                        };
                        approverList3.Add(approver);
                    }
                }
                if (approverList3.Count > 0)
                {
                    var node = new ApplyFlowCommonNodeInput
                    {
                        HasRole = false,
                        PassType = 1,
                        PassStatus = "SME AT",
                        NodeIndex = index++,
                        RejectIndex = 0,
                        ReturnIndex = 0,
                        ApproverList = approverList3
                    };
                    nodeList.Add(node);
                }
            }

            return nodeList;
        }



        public async Task<ITPortal.Core.Services.IResult> CheckAuth(DataAuthApply authApplyDto)
        {
            var result = await GetAuthInfo(authApplyDto);
            var noOwner = result.noOwner;
            var reOwner = result.reOwner;
            var reFlow = result.reFlow;
            var onSuccess = result.onSuccess;
            if (onSuccess.Count() == 0)
            {
                StringBuilder sb = new StringBuilder();
                if (noOwner.Count() > 0)
                {
                    sb.Append($"无数据拥有者:{string.Join(',', noOwner)}");
                    //sb.Append(I18nMessage.getMessage("no.dataOwner")).append(":").append(noOwner.ToString());
                }
                if (reOwner.Count() > 0)
                {
                    sb.Append($"已有权限:{string.Join(',', reOwner)}");
                    //sb.Append(I18nMessage.getMessage("already.have.permission")).append(":").append(reOwner.ToString());
                }
                if (reFlow.Count() > 0)
                {
                    sb.Append($"正在审核中:{string.Join(',', reFlow)}");
                    //sb.Append(I18nMessage.getMessage("under.applied")).append(":").append(reFlow.ToString());
                }
                throw new DataQueryException(sb.ToString());
            }
            return await Task.FromResult(new Result());
        }


        public async Task<(List<string> noOwner, List<string> reOwner, List<string> reFlow, List<string> onSuccess)> GetAuthInfo(DataAuthApply authApplyDto)
        {
            var userId = authApplyDto.UserId;
            var tableList = authApplyDto.TableList;
            //已有权限
            var authList = await _authorizeUserService.Query(new DataAuthorizeUserDto() { UserId = userId });
            var ownerTables = authList.Select(f => f.ObjectId).Distinct().ToList();

            //正在申请
            var recordList = await CurrentDb.Queryable<DataAuthAuditRecordEntity>().
                Where(f => f.UserId == userId).In("status", new string[] { "-1", "-2" }).ToListAsync();
            var recordTables = new List<string>();
            recordList.ForEach(f => recordTables.AddRange(f.TableId.Select(d => d.tableId)));
            recordTables = recordTables.Distinct().ToList();

            var noOwner = new List<string>();
            var reOwner = new List<string>();
            var reFlow = new List<string>();
            var onSuccess = new List<string>();
            tableList.ForEach(table =>
            {
                var ownsId = table.OwnerId?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
                ownsId.Remove(userId);
                if (table.OwnerId.IsNullOrWhiteSpace())
                { //没有owner
                    noOwner.Add(table.TableName);
                }
                else if (ownsId.Count() == 0 || ownerTables.Contains(table.Id))//table.IsPublicSecurityLevel || 
                {//已有权限,自己的和申请过的才表示有权限
                    reOwner.Add(table.TableName);
                }
                else if (recordTables.Contains(table.Id))
                {
                    //没有权限 但有申请
                    reFlow.Add(table.TableName);
                }
                else // if (!table.IsPublicSecurityLevel)
                {
                    onSuccess.Add(table.TableName);
                }
            });
            return await Task.FromResult((noOwner, reOwner, reFlow, onSuccess));
        }

        /// <summary>
        /// 根据审批结果 输入入库
        /// </summary>
        /// <param name="authResultDto"></param>
        /// <returns></returns>
        [UnitOfWork(true)]
        public async Task<ITPortal.Core.Services.IResult<string>> UpdateAuth(AuthResultDto authResultDto)
        {
            return await Task.FromResult(new Result<string>() { Data = "" });
        }

    }
}
