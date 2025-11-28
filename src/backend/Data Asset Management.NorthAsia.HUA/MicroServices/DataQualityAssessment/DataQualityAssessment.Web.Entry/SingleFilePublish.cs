using Furion;
using System.Reflection;

namespace DataQualityAssessment.Web.Entry;

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
            "DataQualityAssessment.Application",
            "DataQualityAssessment.Core",
            "DataQualityAssessment.EntityFramework.Core",
            "DataQualityAssessment.Web.Core"
        };
    }
}