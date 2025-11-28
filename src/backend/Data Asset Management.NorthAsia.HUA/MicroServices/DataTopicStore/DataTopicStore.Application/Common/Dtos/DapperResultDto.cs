using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Common.Dtos
{
    public class DapperResultDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<dynamic> Data { get; set; }

    }
}
