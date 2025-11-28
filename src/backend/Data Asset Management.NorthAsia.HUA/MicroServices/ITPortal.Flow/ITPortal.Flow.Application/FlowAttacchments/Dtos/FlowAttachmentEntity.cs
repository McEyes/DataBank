using ITPortal.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Flow.Application.FlowAttachments.Dtos
{
    [SugarTable("FlowAttachment")]
    public class FlowAttachmentEntity : AuditEntity<Guid>, ICreateNameEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; set; }
        public Guid? FlowInstId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsNeed { get; set; }
        public Guid FlowTempId { get; set; }
        public string FlowTempName { get; set; }
        public string CreatedByName { get; set; }
        /// <summary>
        /// 必填上传节点,为空时表示开始节点必填
        /// </summary>
        public string NeedActName { get; set; }
    }
}
