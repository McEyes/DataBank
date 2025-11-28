using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Application.Common
{
    public class PagingBase
    {
        public const int MaxPageSize = 100000;
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        // public int SkipCount => (PageIndex - 1) * PageSize;

        protected PagingBase()
        {
        }

        public PagingBase(int pageIndex = 1, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
