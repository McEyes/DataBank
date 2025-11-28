using Furion;

using System.Reflection;

namespace ITPortal.AuthServer.Web.Entry
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
                "ITPortal.AuthServer.Application",
                "ITPortal.AuthServer.Core",
                "ITPortal.AuthServer.Web.Core"
            };
        }
    }
}