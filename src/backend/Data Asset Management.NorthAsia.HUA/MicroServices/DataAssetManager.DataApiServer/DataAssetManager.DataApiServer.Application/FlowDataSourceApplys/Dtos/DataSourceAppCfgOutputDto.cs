using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Application.FlowDataSourceApplys.Dtos
{
    public class DataSourceAppCfgOutputDto
    {
        /// <summary>
        /// 审核人
        /// </summary>
        public List<FlowDataSourceAppCfgDto> ApproverList { get; set; }
        /// <summary>
        /// 名字加(Ntid)
        /// </summary>
        public string ApproverInfos { get; set; }
    }
}
