using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class InformationSchemaColumnModel
    {
        public string COLUMN_COMMENT { get; set; }
        public string IS_NULLABLE { get; set; }
        public string COLUMN_DEFAULT { get; set; }
        public string COLUMN_TYPE { get; set; }
        public long CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string DATA_TYPE { get; set; }
    }
}
