using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class DbSchemaModel
    {
        public string sid { get; set; }
        public string host { get; set; }
        public string port { get; set; }
        public string dbName { get; set; }
    }
}
