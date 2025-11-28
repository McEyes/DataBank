using Furion;

using System.Reflection;

namespace DataBankDashbord.Web.Entry
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
                "DataBankDashbord.Application",
                "DataBankDashbord.Core",
                "DataBankDashbord.Web.Core"
            };
        }
    }
}