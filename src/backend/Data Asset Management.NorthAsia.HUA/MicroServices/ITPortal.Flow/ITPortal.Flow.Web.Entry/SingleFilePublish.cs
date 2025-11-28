using Furion;

using System.Reflection;

namespace ITPortal.Flow.Web.Entry
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
                "ITPortal.Flow.Application",
                "ITPortal.Flow.Core",
                "ITPortal.Flow.Web.Core"
            };
        }
    }
}