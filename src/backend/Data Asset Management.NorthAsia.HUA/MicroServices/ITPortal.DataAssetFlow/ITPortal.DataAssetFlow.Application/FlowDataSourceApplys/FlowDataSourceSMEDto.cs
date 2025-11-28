using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.FlowDataSourceSMEs
{
    public class FlowDataSourceSMEDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; } = null;
        public Guid? FlowDataSourceApplyId { get; set; } = null;    
    }
}
