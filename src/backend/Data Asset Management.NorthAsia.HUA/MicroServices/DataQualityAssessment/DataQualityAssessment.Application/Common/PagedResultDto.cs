using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Application.Common
{
    public class PagedResultDto<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
