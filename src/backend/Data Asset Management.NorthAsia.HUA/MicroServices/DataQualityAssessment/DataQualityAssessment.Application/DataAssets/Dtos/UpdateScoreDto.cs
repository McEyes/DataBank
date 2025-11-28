using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Application.DataAssets.Dtos
{
    public class UpdateScoreDto
    {
        public string TableId { get; set; }
        public int Score { get; set; }
    }
}
