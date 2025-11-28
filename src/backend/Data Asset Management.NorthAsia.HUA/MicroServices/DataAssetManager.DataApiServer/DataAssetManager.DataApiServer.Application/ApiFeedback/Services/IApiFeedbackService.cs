using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.Services;


namespace DataAssetManager.DataTableServer.Application
{
    public interface IApiFeedbackService : IBaseService<ApiFeedbackEntity, ApiFeedbackDto, Guid>
    {

    }
}
