using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Furion.DatabaseAccessor;

namespace DataQualityAssessment.EntityFramework.Core
{
    public class DataAssetDbProperties
    {
        public const string ConnectionStringName = "DataAsset";
        public const string DBProvider = DbProvider.Npgsql;
        public static string DbTablePrefix { get; set; } = "";
        public static string DbSchema { get; set; } = null;
    }
}
