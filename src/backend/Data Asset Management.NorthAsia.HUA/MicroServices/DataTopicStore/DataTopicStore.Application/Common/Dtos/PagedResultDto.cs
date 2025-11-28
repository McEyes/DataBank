using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.Common.Dtos
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class DapperPagedResultDto
    {
        public IEnumerable<dynamic> Data { get; set; }
        public long TotalCount { get; set; }
    }
}
