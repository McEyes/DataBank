
using DataTopicStore.Application.SchemaTables.Services;

namespace DataTopicStore.Application.SchemaTables
{
    [AppAuthorize]
    public class SchemaTablesAppService : IDynamicApiController
    {
        private readonly ISchemaTablesService schemaTablesService;
        public SchemaTablesAppService(ISchemaTablesService schemaTablesService)
        {
            this.schemaTablesService = schemaTablesService;
        }

        [HttpPost]
        public Task SyncUpdateAsync() => schemaTablesService.SyncUpdateAsync();
    }
}
