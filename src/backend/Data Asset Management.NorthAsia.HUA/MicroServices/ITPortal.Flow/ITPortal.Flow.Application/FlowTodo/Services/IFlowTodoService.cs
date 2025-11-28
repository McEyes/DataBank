using DataAssetManager.DataApiServer.Application.DataApi.Dtos;

using ITPortal.Core;
using ITPortal.Core.Services;
using ITPortal.Flow.Application.FlowTodo.Dtos;

namespace ITPortal.Flow.Application.FlowTodo.Services
{
    public interface IFlowTodoService : IBaseService<FlowTodoEntity, FlowTodoDto, Guid>
    {
        Task<PageResult<FlowTodoEntity>> PageDealTodo(FlowTodoDto filter);
        Task<PageResult<FlowTodoEntity>> PageSelfTodo(FlowTodoDto filter);
    }
}
