using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{
    public class DashbordMetadataRecDto
    {

        public bool? IsDashboard { get; set; }
        public bool? IsIndicator { get; set; }

        public string? IndicatorCode { get; set; }
        
        public string? table_name { get; set; }
        public string? column_id { get; set; }

        public string? column_name { get; set; }

        public string? owner_id { get; set; }

        public string? source_id { get; set; }


    }
    public class apiDashbordDto
    {
        public List<string> listColumnId { get; set; }

        public List<DashbordMetadataRecDto> list { get; set; }

    }

}
