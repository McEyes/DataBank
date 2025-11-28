using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Core.Enums;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SearchApprovalPagingDto : PagingBase
    {
        public ApprovalStatus? Status { get; set; }
        public string TopicName { get; set; }
        public string Ntid { get; set; }
    }
}
