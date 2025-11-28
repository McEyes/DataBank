using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.ResMgrPlatform.Application.Dtos
{
    public class FileAttachment
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string FileType { get; set; }
        //public string PreviewUrl { get; set; } = $"{DownloadUrl}?act=view";
    }
}
