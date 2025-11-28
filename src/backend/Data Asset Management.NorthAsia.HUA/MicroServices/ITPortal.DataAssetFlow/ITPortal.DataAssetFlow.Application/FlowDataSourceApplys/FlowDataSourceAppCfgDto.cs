using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jb.home.Application.Contracts.FlowDataSourceAppCfgs
{
    public class FlowDataSourceAppCfgDto
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string Ntid { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string EnglishName { get; set; }

    }
}
