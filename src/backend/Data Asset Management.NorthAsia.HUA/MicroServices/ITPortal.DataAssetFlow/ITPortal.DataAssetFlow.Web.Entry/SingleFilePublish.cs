using Furion;

using System.Reflection;

namespace ITPortal.DataAssetFlow.Web.Entry
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
                "ITPortal.DataAssetFlow.Application",
                "ITPortal.DataAssetFlow.Core",
                "ITPortal.DataAssetFlow.Web.Core"
            };
        }
    }
}