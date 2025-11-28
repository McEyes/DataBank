using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTempActRoute.Dtos;

namespace ITPortal.Flow.Application.FlowTempActRoute.Services
{
    public interface IFlowTempActRouteService : IBaseService<FlowTempActRouteEntity, FlowTempActRouteDto, Guid>
    {
    }
}
