using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Flow.Application.FlowAttachments.Dtos
{
    public class FlowAttachmentInfo : CreateEntity<Guid>
    {
        //public Guid? FlowInstId { get; set; } = null;
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        /// <summary>
        /// ÊÇ·ñ±ØÌî
        /// </summary>
        public bool IsNeed { get; set; }=false;
        //public Guid? FlowTempId { get; set; }
        //public string FlowTempName { get; set; }
        //public  DateTimeOffset? StartTime { get; set; }
        //public DateTimeOffset? EndTime { get; set; }
        //public string CreateBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
