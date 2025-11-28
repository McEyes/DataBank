using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace DataQualityAssessment.Core.Models
{
    public class DataassetDbSettingModel
    {
        public string sid { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string dbName { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }
}
