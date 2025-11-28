using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class PermissionApprovalItemDto
    {
        public Guid Id { get; set; }
        public ApprovalStatus Status { get; set; }
        public string Remark { get; set; }
        public string TopicName { get; set; }
        public long TopicId { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
    }
}
