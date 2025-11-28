using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.FlowAttacchments.Dtos
{
    public class FlowAttacchmentDto:PageEntity<Guid>
    {
        public Guid? FlowId { get; set; } = null;
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        /// <summary>
        /// ÊÇ·ñ±ØÌî
        /// </summary>
        public bool IsNeed { get; set; }
        public Guid? FlowTempId { get; set; }
        public string FlowTempName { get; set; }
        //public  DateTime? StartTime { get; set; }
        //public DateTime? EndTime { get; set; }
        public  string CreateBy { get; set; }
        public string CreatedByName { get; set; }
    }
}
