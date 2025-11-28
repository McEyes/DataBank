using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Application.OpenSearch.Dtos
{
    public class TagRecommendQueryDto
    {

        public List<string> Tags { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;


    }
}
