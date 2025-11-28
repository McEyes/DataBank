using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataQualityAssessment.Core.Models;

namespace DataQualityAssessment.Core.Dappers
{
    public interface IDbHelper
    {
        IDbConnection CreateDbConnection(DbSettings settings);
        void CloseAllConnection();
        string BuildConnectionString(DbSettings settings);
    }
}
