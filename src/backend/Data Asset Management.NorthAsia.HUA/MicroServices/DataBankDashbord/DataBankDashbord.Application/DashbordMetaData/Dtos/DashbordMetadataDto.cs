using ITPortal.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBankDashbord.Application.DashbordMetaData.Dtos
{
    public class DashbordMetadataDto : PageEntity<Guid>
    {
        public string SourceId { get; set; }
        public string OwnerId { get; set; }
        public bool? IsIndicator { get; set; }
        public bool? IsDashboard { get; set; }
        public string? TbaleName { get; set; }
        public string? ColumnName { get; set; }

        public string ColumnId { get; set; }
   
        public string IndicatorCode { get; set; }
        


    }
}
