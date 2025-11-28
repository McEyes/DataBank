using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Application.Common;

namespace DataQualityAssessment.Application.Assessments.Dtos
{
    public class TableSearchDto : PagingBase
    {
        public string TableId { get; set; }
        public string TableName { get; set; }
        public string Owner { get; set; }
        public string Dept { get; set; }

        public int? ScoreFrom { get; set; }
        public int? ScoreTo { get; set; }
    }
}
