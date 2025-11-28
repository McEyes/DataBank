using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Dappers;
using DataQualityAssessment.Core.Enums;

namespace DataQualityAssessment.Core.Models
{
    public class RuleDataContext
    {
        public DbSettings DbSetting { get; set; }
        public RuleSettings RuleSetting { get; set; }
        public IDbConnection DbConnection { get; set; }

        public object Data { get; set; }
    }
}
