using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Datalineage.Services;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;
using DataTopicStore.Core.Datalineage.Marquez.Models;

namespace DataTopicStore.Application.Datalineage
{
    [AppAuthorize]
    public class DatalineageAppService : IDynamicApiController
    {
        private readonly IDatalineageService datalineageService;
        public DatalineageAppService(IDatalineageService datalineageService)
        {
            this.datalineageService = datalineageService;
        }

        [HttpGet]
        public Task<LineageGraph> GetLineageDataAsync([FromQuery] string name) => datalineageService.GetLineageDataAsync(name);

        [HttpGet]
        public Task<bool> CreateDatasetAsync([FromQuery] string table_schema, [FromQuery] string table_name) => datalineageService.CreateDatasetAsync(table_schema, table_name);
    }
}
