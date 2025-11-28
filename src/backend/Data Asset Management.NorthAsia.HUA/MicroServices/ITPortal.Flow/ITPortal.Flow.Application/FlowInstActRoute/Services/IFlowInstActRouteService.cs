using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowInstActRoute.Dtos;

namespace ITPortal.Flow.Application.FlowInstActRoute.Services
{
    public interface IFlowInstActRouteService : IBaseService<FlowInstActRouteEntity, FlowInstActRouteDto, Guid>
    {
    }
}
