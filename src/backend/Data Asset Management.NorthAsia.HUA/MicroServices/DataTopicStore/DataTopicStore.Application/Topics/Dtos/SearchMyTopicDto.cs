using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Common.Dtos;
using DataTopicStore.Core.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DataTopicStore.Application.Topics.Dtos
{
    public class SearchMyTopicDto : PagingBase
    {
        public string TopicName { get; set; }
        public ApprovalStatus?  Status { get; set; }
    }
}
