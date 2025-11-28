using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.SchemaTables.Dtos;

namespace DataTopicStore.Application.SchemaTables.Services
{
    public interface ISchemaTablesService
    {
        Task SyncUpdateAsync();
        SchemaTableDto GetInfoFromCache(string table_schema,string table_name);
    }
}
