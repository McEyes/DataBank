using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempAct.Dtos;

namespace ITPortal.Flow.Application.FlowTempAct.Services
{
    public interface IFlowTempActService : IBaseService<FlowTempActEntity, FlowTempActDto, Guid>
    {
    }
}
