using ITPortal.Core.ProxyApi.Dto;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.EmailSendRecord.Dtos;
using ITPortal.Flow.Application.EmailTemplate.Dtos;
using ITPortal.Flow.Application.FlowActInst.Dtos;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;
using ITPortal.Flow.Application.FlowInst.Dtos;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;
using ITPortal.Flow.Application.FlowTempAct.Dtos;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;
using ITPortal.Flow.Application.FlowTemplate.Dtos;
using ITPortal.Flow.Application.FlowTodo.Dtos;

namespace DataAssetManager.DataApiServer.Application.MappingProfile
{
    public partial class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<FlowActInstDto, FlowActInstEntity>();
            config.ForType<FlowActInstEntity, FlowActInstDto>();
            config.ForType<FlowActInstEntity, FlowAuditRecordEntity>()
                .Ignore(f=>f.CreateTime)
                .Map(f => f.UpdateTime,s=>s.CompleteTime)
                .Map(f => f.ActOperate, s => s.ActStatus == ITPortal.Flow.Core.ActivityStatus.Running ? s.ActStatus.ToString() : "");

            config.ForType<FlowAuditRecordDto, FlowAuditRecordEntity>();
            config.ForType<FlowAuditRecordEntity, FlowAuditRecordDto>();

            config.ForType<FlowInstDto, FlowInstEntity>();
            config.ForType<StartFlowDto, FlowInstEntity>();
            config.ForType<FlowInstEntity, FlowInstDto>();

            config.ForType<FlowInstActRouteDto, FlowInstActRouteEntity>();
            config.ForType<FlowInstActRouteEntity, FlowInstActRouteDto>();
            config.ForType<FlowInstActRouteDto, FlowInstActRouteEntity>();
            config.ForType<FlowInstActRouteEntity, FlowInstActRouteEntity>();

            config.ForType<FlowTempActDto, FlowTempActEntity>();
            config.ForType<FlowTempActEntity, FlowTempActDto>();

            config.ForType<FlowTempActRouteDto, FlowTempActRouteEntity>();
            config.ForType<FlowTempActRouteEntity, FlowTempActRouteDto>();

            config.ForType<FlowTemplateDto, FlowTemplateEntity>();
            config.ForType<FlowTemplateEntity, FlowTemplateDto>();

            config.ForType<FlowTodoDto, FlowTodoEntity>();
            config.ForType<FlowTodoEntity, FlowTodoDto>();

            config.ForType<FlowAttachmentInfo, FlowAttachmentEntity>();
            config.ForType<FlowAttachmentEntity, FlowAttachmentInfo>();
            config.ForType<FlowAttachmentDto, FlowAttachmentEntity>();
            config.ForType<FlowAttachmentEntity, FlowAttachmentDto>();
            config.ForType<FlowAttachmentDto, FlowAttachmentInfo>();
            config.ForType<FlowAttachmentInfo, FlowAttachmentDto>();

            config.ForType<EmailTemplateEntity, EmailTemplateDto>();
            config.ForType<EmailTemplateDto, EmailTemplateEntity>();
            config.ForType<EmailTemplateEntity, EmailSendRecordEntity>()
                .IgnoreNullValues(true)
                .Map(dest => dest.EmailHtmlBody, src => src.EmailTemplate);
            config.ForType<EmailTemplateDto, EmailSendRecordEntity>()
                .IgnoreNullValues(true)
                .Map(dest => dest.EmailHtmlBody, src => src.EmailTemplate);

            config.ForType<JabusEmployeeInfo, StaffInfo>()
                .Map(dest => dest.Email, src => src.WorkEmail)
                .Map(dest => dest.Department, src => src.DepartmentName)
                .Map(dest => dest.Ntid, src => src.WorkNTID);
            config.ForType<UserInfo, StaffInfo>()
                .Map(dest => dest.Ntid, src => src.NtId);
        }
    }
}
