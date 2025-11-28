using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace DataTopicStore.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("dts_schema_columns")]
    public partial class SchemaColumnsEntity
    {
        public SchemaColumnsEntity()
        {


        }
          
        public string table_schema { get; set; }

        public string table_name { get; set; }

        public string column_name { get; set; }

        public string column_comment { get; set; }
        public string data_type { get; set; }
    }
}
