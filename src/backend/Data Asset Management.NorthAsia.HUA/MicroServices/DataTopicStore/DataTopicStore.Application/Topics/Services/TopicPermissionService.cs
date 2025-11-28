
using DataTopicStore.Application.ApproveFlow;
using DataTopicStore.Application.ApproveFlow.Dtos;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Application.Logs.Services;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Enums;
using DataTopicStore.Core.Repositories;

namespace DataTopicStore.Application.Topics.Services
{
    [AppAuthorize]
    public class TopicPermissionService : ApplicationService, ITopicPermissionService
    {
        private readonly ILogService<TopicPermissionService> logService;
        private readonly Repository<TopicEntity> topicRepository;
        private readonly Repository<TopicAccessRequestEntity> topicAccessRequestRepository;
        private readonly FlowApplyProxyService flowApplyProxyService;
        public TopicPermissionService(
            ILogService<TopicPermissionService> logService,
            Repository<TopicEntity> topicRepository,
            Repository<TopicAccessRequestEntity> topicAccessRequestRepository,
            FlowApplyProxyService flowApplyProxyService)
        {
            this.logService = logService;
            this.topicRepository = topicRepository;
            this.topicAccessRequestRepository = topicAccessRequestRepository;
            this.flowApplyProxyService = flowApplyProxyService;
        }

        public async Task<bool> ApplyAsync(TopicPermissionApplyDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            _ = await topicRepository.GetByIdAsync(input.topic_id) ?? throw Oops.Oh("Topic information does not exist.");
            var entity = await topicAccessRequestRepository.GetFirstAsync(t => t.user_id == CurrentUser.Id && t.topic_id == input.topic_id);
            var statusList = new ApprovalStatus[] { ApprovalStatus.New, ApprovalStatus.Approved };
            if (entity != null && statusList.Contains(entity.status))
                throw Oops.Oh("You have already submitted the application; duplicate submissions are not allowed.");

            entity = new TopicAccessRequestEntity
            {
                created_by = CurrentUser.Id,
                created_time = DateTime.Now,
                status = ApprovalStatus.New,
                user_id = CurrentUser.Id,
                user_display_name = CurrentUser.UserName,
                topic_id = input.topic_id,
                id = Guid.NewGuid()
            };


            var ctx = topicRepository.Context;

            try
            {
                await ctx.Ado.BeginTranAsync();
                await topicAccessRequestRepository.InsertAsync(entity);
                var flow_id = entity.id;
                await CreateApproveFlow(flow_id, entity.topic_id);
                await ctx.Ado.CommitTranAsync();
            }
            catch (Exception ex)
            {
                await ctx.Ado.RollbackTranAsync();
                await logService.LogErrorAsync($"Message={ex.Message},StackTrace={ex.StackTrace}", nameof(ApplyAsync));
                throw;
            }

            return true;
        }

        private async Task CreateApproveFlow(Guid flow_id,long topic_id)
        {
            // Approve flow

            var topicEntity = await topicRepository.GetByIdAsync(topic_id) ?? throw Oops.Oh("Topic data does not exist.");
            var flowActApprovers = new List<ApproveFlow.Dto.FlowActApprover>();
            flowActApprovers.Add(new ApproveFlow.Dto.FlowActApprover
            {
                ActName = "Owner Approval",
                ActorParms = new List<ApproveFlow.Dto.StaffInfo>
                {
                    new ApproveFlow.Dto.StaffInfo{ Ntid = topicEntity.author_id, Name = topicEntity.author, Email = topicEntity.email }
                },
                ActorParmsName = topicEntity.author
            });

            var dto = new ApproveFlow.Dto.StartFlowDto
            {
                FlowTempName = ApproveFlowConsts.TopicAuthApplication,
                Applicant = CurrentUser.Id,
                ApplicantName = CurrentUser.Name,
                FormId = flow_id,
                FormData = new
                {
                    request_id = flow_id,
                    topic_id = topic_id,
                },
                ActApprovers = flowActApprovers
            };
            var result = await flowApplyProxyService.InitFlowAsync(dto);

            if (!result.Success)
            {
                throw new Exception(result.Msg.ToString());
            }
        }

        public async Task<PagedResultDto<PermissionApprovalItemDto>> GetApprovalPagingListAsync(SearchApprovalPagingDto input)
        {
            input.PageIndex = input.PageIndex <= 0 ? 1 : input.PageIndex;
            input.PageSize = input.PageSize <= 0 ? 10 : input.PageSize;

            var queryAccessRequest = topicAccessRequestRepository.AsQueryable();
            var queryTopic = topicRepository.AsQueryable();
            var queryResults = queryAccessRequest.LeftJoin(queryTopic, (a, b) => a.topic_id == b.id).Select((a, b) => new PermissionApprovalItemDto
            {
                Id = a.id,
                Status = a.status,
                TopicName = b.name,
                TopicId = a.topic_id,
                CreatedBy = a.created_by,
                CreatedTime = a.created_time,
                Remark = a.remark,
                UserId = a.user_id,
                UserDisplayName = a.user_display_name
            }).MergeTable().Where(a => !string.IsNullOrWhiteSpace(a.TopicName))
            .WhereIF(!string.IsNullOrWhiteSpace(input.Ntid), t => t.UserId == input.Ntid)
            .WhereIF(!string.IsNullOrWhiteSpace(input.TopicName), t => t.TopicName.Contains(input.TopicName))
            .WhereIF(input.Status != null, t => t.Status == input.Status)
            .OrderByDescending(t => t.CreatedTime);

            PagedResultDto<PermissionApprovalItemDto> result = new();

            result.TotalCount = await queryResults.CountAsync();
            result.Data = await queryResults.ToPageListAsync(input.PageIndex, input.PageSize);

            return result;
        }

        public async Task<bool> ApproveAsync(TopicPermissionApproveDto input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (input.Status == ApprovalStatus.New) throw Oops.Oh("The status must be Approved or Rejected.");

            var client = topicAccessRequestRepository.AsSugarClient();
            var entity = await topicAccessRequestRepository.GetByIdAsync(input.AccessRequestId);
            if (entity is TopicAccessRequestEntity { status: ApprovalStatus.New })
            {
                // approval records
                var approvalRecordEntity = new TopicAccessRequestApprovalRecordsEntity
                {
                    access_request_id = input.AccessRequestId,
                    approval_user_id = CurrentUser.Id,
                    created_by = CurrentUser.Id,
                    created_time = DateTime.Now,
                    remark = input.Remark,
                    id = Guid.NewGuid(),
                    status = input.Status,
                };

                // access request
                entity.status = input.Status;
                entity.updated_by = CurrentUser.Id;
                entity.updated_time = DateTime.Now;
                entity.remark = input.Remark;

                try
                {
                    await client.Ado.BeginTranAsync();

                    await client.Insertable(approvalRecordEntity).ExecuteCommandAsync();
                    await client.Updateable(entity).ExecuteCommandAsync();

                    // save authorization user
                    if (input.Status == ApprovalStatus.Approved)
                    {
                        var authorizedUser = new TopicAuthorizationUserEntity
                        {
                            created_by = CurrentUser.Id,
                            created_time = DateTime.Now,
                            topic_id = entity.topic_id,
                            user_id = entity.user_id,
                            user_display_name = entity.user_display_name
                        };
                        await client.Insertable(authorizedUser).ExecuteCommandAsync();
                    }

                    await FlowApproveAsync(input);
                    await client.Ado.CommitTranAsync();
                }
                catch (Exception ex)
                {
                    await client.Ado.RollbackTranAsync();
                    await logService.LogErrorAsync($"Message={ex.Message},StackTrace={ex.StackTrace}");
                    throw;
                }
            }
            else
            {
                throw Oops.Oh("The current status does not allow modifications or the information does not exist.");
            }

            return true;
        }


        public async Task<bool> FlowApproveAsync(TopicPermissionApproveDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            HttpResult results = null;
            var flowDto = new ApproveFlow.Dto.FlowAuditDto
            {
                AuditContent = dto.Remark,
                FlowInstId = dto.AccessRequestId
            };
            switch (dto.Status)
            {
                case ApprovalStatus.Approved:
                    results = await flowApplyProxyService.SendApproveAsync(flowDto);
                    break;
                case ApprovalStatus.Rejected:
                    results = await flowApplyProxyService.SendRejectEndAsync(flowDto);
                    break;
                default:
                    break;
            }

            ArgumentNullException.ThrowIfNull(results);
            if (!results.Success) throw new Exception(results.Msg.ToString());
            return results.Success;
        }
    }
}
