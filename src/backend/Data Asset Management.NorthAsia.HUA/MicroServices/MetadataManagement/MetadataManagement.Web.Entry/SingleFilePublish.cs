using Furion;

using System.Reflection;

namespace MetadataManagement.Web.Entry
{
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
                "MetadataManagement.Application",
                "MetadataManagement.Core",
                "MetadataManagement.Web.Core"
            };
        }
    }
}