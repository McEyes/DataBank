using Furion;

using System.Reflection;

namespace DataAssetManager.DataApiServer.Web.Entry
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
                "DataAssetManager.DataApiServer.Application",
                "DataAssetManager.DataApiServer.Core",
                "DataAssetManager.DataApiServer.Web.Core"
            };
        }
    }
}