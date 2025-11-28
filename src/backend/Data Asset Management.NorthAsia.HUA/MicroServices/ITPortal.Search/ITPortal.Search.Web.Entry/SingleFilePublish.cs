using Furion;

using System.Reflection;

namespace ITPortal.Search.Web.Entry
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
                "ITPortal.Search.Application",
                "ITPortal.Search.Core",
                "ITPortal.Search.Web.Core"
            };
        }
    }
}