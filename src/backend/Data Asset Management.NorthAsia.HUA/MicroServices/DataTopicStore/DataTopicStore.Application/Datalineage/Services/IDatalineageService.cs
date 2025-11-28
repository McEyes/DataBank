using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Core.Datalineage.Marquez.Models;

namespace DataTopicStore.Application.Datalineage.Services
{
    public interface IDatalineageService
    {
        Task<LineageGraph> GetLineageDataAsync(string name);

        Task<bool> CreateDatasetAsync(string table_schema,string table_name);
        
    }
}
