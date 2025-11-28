using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTopicStore.Application.SchemaTables.Dtos
{
    public class SchemaTableDto
    {
        public string table_schema { get; set; }
        public string table_name { get; set; }
        public string table_comment { get; set; }
    }
}
