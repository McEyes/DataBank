using Furion.DependencyInjection;

using MetadataManagement.Core.Entitys;

namespace MetadataManagement.Application
{
    public class MBaseService<T> : IMBaseService, ITransient
    {
        public MBaseService()
        {

        }

    }
}
