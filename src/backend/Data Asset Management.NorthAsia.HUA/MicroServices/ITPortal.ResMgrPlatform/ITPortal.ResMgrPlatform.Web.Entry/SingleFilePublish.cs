using Furion;

using System.Reflection;

namespace ITPortal.ResMgrPlatform.Web.Entry
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
                "ITPortal.ResMgrPlatform.Application",
                "ITPortal.ResMgrPlatform.Core",
                "ITPortal.ResMgrPlatform.Web.Core"
            };
        }
    }
}