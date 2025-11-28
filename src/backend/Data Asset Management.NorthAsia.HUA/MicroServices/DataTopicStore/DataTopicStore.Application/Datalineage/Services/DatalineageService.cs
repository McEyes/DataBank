using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.SchemaTables.Services;
using DataTopicStore.Application.Topics.Services;
using DataTopicStore.Core.Datalineage.Marquez;
using DataTopicStore.Core.Datalineage.Marquez.Models;
using DataTopicStore.Core.Datalineage.Openlineage;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;
using static DataTopicStore.Core.Datalineage.Openlineage.Dtos.CreateDatasetDto;

namespace DataTopicStore.Application.Datalineage.Services
{
    public class DatalineageService : IDatalineageService, ITransient
    {
        private MarquezAgent marquezAgent;
        private OpenLineageAgent openLineageAgent;
        private readonly ISchemaTablesService schemaTablesService;
        private readonly Repository<TopicEntity> topicService;
        private readonly Repository<SchemaColumnsEntity> schemaColumnsRepository;
        public DatalineageService(ISchemaTablesService schemaTablesService,
            Repository<TopicEntity> topicService,
            Repository<SchemaColumnsEntity> schemaColumnsRepository)
        {
            this.marquezAgent = new MarquezAgent();
            this.openLineageAgent = new OpenLineageAgent();
            this.schemaTablesService = schemaTablesService;
            this.topicService = topicService;
            this.schemaColumnsRepository = schemaColumnsRepository;
        }

        public async Task<LineageGraph> GetLineageDataAsync(string name)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(name);
            if (!Int64.TryParse(name.Replace("topic", ""), out long topic_id))
                throw Oops.Oh("The parameter name is invalid.");

            var descripton = "";
            var topic_info = await topicService.GetByIdAsync(topic_id);
            if (topic_info != null) descripton = topic_info.name;
            var result = await marquezAgent.GetLineageDataAsync(name);
            foreach (var item in result.graph)
            {
                foreach (var inputItem in item.data.inputs)
                {
                    var info = result.datasets.FirstOrDefault(t=>t.name == inputItem.name);
                    if (info != null)
                        inputItem.description = info.description;
                }

                item.data.id.description = descripton;
            }

            return result;
        }

        public async Task<bool> CreateDatasetAsync(string table_schema, string table_name)
        {
            var jobNamespace = "datatopicstore";
            var dataset = $"{table_schema}.{table_name}";
            var datafields = new List<LineageNode_Data_Field>();
            var list = await schemaColumnsRepository.GetListAsync(a => a.table_schema.ToLower() == table_schema.ToLower() && a.table_name.ToLower() == table_name.ToLower());
            if (list is List<SchemaColumnsEntity> { Count: > 0 })
            {
                foreach (var item in list)
                {
                    var fieldName = item.column_name;
                    datafields.Add(new LineageNode_Data_Field
                    {
                        name = fieldName,
                        type = item.data_type,
                        description = item.column_comment
                    });
                }
            }

            return await openLineageAgent.CreateDataset(jobNamespace, dataset.ToLower(), new Core.Datalineage.Openlineage.Dtos.CreateDatasetDto
            {
                type = "DB_TABLE",
                physicalName = dataset.ToLower(),
                description = "",
                fields = datafields,
                sourceName = "default"
            });
        }
    }
}
