using DataTopicStore.Core.Datalineage.Openlineage;
using DataTopicStore.Core.Datalineage.Openlineage.Events;
using DataTopicStore.Core.Datalineage.Openlineage.Models;
using DataTopicStore.Core.Datalineage.Sqllineage;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataTopicStore.Application;

public class SystemService : ISystemService, ITransient
{
    public string GetDescription()
    {

        return SqllineageTest();

        return "让 .NET 开发更简单，更通用，更流行。";
    }

    private string SqllineageTest()
    {
        SqllineageAgent agent = new SqllineageAgent();


        var sql = @"insert into db1.table1 select * from db2.table2; insert into db3.table3 select * from db1.table1;";

            var ff =   agent.ExtractTables(sql).Result;


        return "1111";
    }

    private string OpenLineageTest()
    {
        OpenLineageAgent agent = new OpenLineageAgent();

        var runEvent = new RunEvent
        {
            EventType = Core.Datalineage.Openlineage.Enums.RunState.COMPLETE,
            EventTime = DateTime.UtcNow,
            Run = new Run { RunId = Guid.NewGuid().ToString() },
            Job = new Job
            {
                Namespace = "test",
                Name = "my-test-job"
            },
            Inputs = new List<InputDataset>
            {
                new InputDataset
                {
                    Namespace = "test",
                    Name = "order-table"
                }
            },
            Outputs = new List<OutputDataset>
            {
                new OutputDataset
                {
                    Namespace = "test",
                    Name = "sum-table"
                }
            },
            Producer = "http://cnhuam0webstg85:6002/#/home/page",
            SchemaURL = "http://cnhuam0webstg85:6002/#/home/page"
        };

        agent.EmitEvent(runEvent).Wait();


        return JsonConvert.SerializeObject(runEvent, new StringEnumConverter());
    }
}
