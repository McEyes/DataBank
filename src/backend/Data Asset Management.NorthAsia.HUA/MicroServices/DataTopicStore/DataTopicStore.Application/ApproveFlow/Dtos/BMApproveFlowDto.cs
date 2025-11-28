using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.ApproveFlow.Enums;

namespace DataTopicStore.Application.ApproveFlow.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class BMApproveFlowDto
    {
        public long TopicId { get; set; }
        public string Remark { get; set; }
        public BMFlowAction Action { get; set; }
    }
}
