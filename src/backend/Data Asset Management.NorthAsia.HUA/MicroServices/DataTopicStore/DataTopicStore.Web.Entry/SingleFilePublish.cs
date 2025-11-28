using Furion;
using System.Reflection;

namespace DataTopicStore.Web.Entry;

public class SingleFilePublish : ISingleFilePublish
{
    public Assembly[] IncludeAssemblies()
    {
        return Array.Empty<Assembly>();
    }

    public string[] IncludeAssemblyNames()
    {
        return new[]
        {
            "DataTopicStore.Application",
            "DataTopicStore.Core",
            "DataTopicStore.Web.Core"
        };
    }
}