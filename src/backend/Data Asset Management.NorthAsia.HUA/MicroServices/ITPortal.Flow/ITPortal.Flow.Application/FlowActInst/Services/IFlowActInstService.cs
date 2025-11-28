using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowActInst.Dtos;

namespace ITPortal.Flow.Application.FlowActInst
{
    public interface IFlowActInstService : IBaseService<FlowActInstEntity, FlowActInstDto, Guid>
    {
    }
}
