using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.Service.Application.Contracts.ApplyFlowAttacchments
{
    public class ApplyOtherFlowAttacchmentDto
    {
        public Guid? Id { get; set; }
        public Guid? ApplyFlowInfoId { get; set; } = null;
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string Category { get; set; }
        public int ApplyFlowType { get; set; } = 10;
    }
}
