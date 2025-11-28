using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class TopicPermissionApproveDto
    {
        public Guid AccessRequestId { get; set; }
        public ApprovalStatus Status { get; set; }
        public string Remark { get; set; }
    }
}
