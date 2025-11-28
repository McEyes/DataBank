using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.TopicDocument.Dtos
{
    public class ConfirmModalParamsDto
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public bool ShowCancel { get; set; } = false;

        public string ConfirmText { get; set; } = "确认";

        public string CancelText { get; set; } = "取消";

    }
}
