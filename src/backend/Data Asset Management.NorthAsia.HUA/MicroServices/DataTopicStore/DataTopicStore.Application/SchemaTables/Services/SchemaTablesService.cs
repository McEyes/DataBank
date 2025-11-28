using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataTopicStore.Application.SchemaTables.Dtos;
using DataTopicStore.Core.Caches;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;
using Mapster;

namespace DataTopicStore.Application.SchemaTables.Services
{
    public class SchemaTablesService : ApplicationService, ISchemaTablesService
    {
        private readonly IDapperRepository dapperRepository;
        private readonly Repository<SchemaTablesEntity> schemaTablesRepository;
        public SchemaTablesService(IDapperRepository dapperRepository,
           Repository<SchemaTablesEntity> schemaTablesRepository)
        {
            this.dapperRepository = dapperRepository;
            this.schemaTablesRepository = schemaTablesRepository;
        }

        public SchemaTableDto GetInfoFromCache(string table_schema, string table_name)
        {
            var cacheKey = $"SCHEMATABLES_{table_schema.ToLower()}_{table_name.ToLower()}";
            return MemoryCacheUtils.Get(cacheKey, TimeSpan.FromMinutes(10), () =>
            {
                var entity = schemaTablesRepository.AsQueryable().Where(t => t.table_schema == table_schema.ToLower() && t.table_name == table_name.ToLower()).First();
                if (entity != null)
                    return entity.Adapt<SchemaTableDto>();
                else
                    return null;
            });
        }

        public async Task SyncUpdateAsync()
        {
            var sql = "select TABLE_SCHEMA as table_schema , TABLE_NAME as table_name ,TABLE_COMMENT as table_comment  from information_schema.tables;";
            var data = await dapperRepository.QueryAsync<SchemaTableDto>(sql);
            var tables = schemaTablesRepository.AsQueryable().ToList();
            foreach (var table in data)
            {
                var info = tables.Where(t => t.table_schema.ToLower() == table.table_schema.ToLower() && t.table_name.ToLower() == table.table_name.ToLower()).FirstOrDefault();
                if (info != null)
                {
                    //update
                    if (info.is_custom == true)
                        continue;

                    info.table_comment = table.table_comment;
                    await schemaTablesRepository.UpdateAsync(info);
                }
                else
                {
                    //insert
                    info = new SchemaTablesEntity
                    {
                        created_by = CurrentUser.Id,
                        created_time = DateTime.Now,
                        updated_by = CurrentUser.Id,
                        updated_time = DateTime.Now,
                        id = Guid.NewGuid(),
                        is_custom = false,
                        table_comment = table.table_comment,
                        table_name = table.table_name.ToLower(),
                        table_schema = table.table_schema.ToLower()
                    };
                    await schemaTablesRepository.InsertAsync(info);
                }
            }
        }
    }
}
