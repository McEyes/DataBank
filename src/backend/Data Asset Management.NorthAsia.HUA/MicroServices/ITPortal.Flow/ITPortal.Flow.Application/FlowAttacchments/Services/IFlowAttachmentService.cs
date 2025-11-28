using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowAttachments.Dtos;
using ITPortal.Flow.Application.FlowAuditRecord.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Flow.Application.FlowAttachments.Services
{
    public interface IFlowAttachmentService : IBaseService<FlowAttachmentEntity, FlowAttachmentDto, Guid>
    {
    }
}
