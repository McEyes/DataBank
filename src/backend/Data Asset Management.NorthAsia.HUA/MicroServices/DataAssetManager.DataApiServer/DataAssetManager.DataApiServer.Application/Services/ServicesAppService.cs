using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    //[Route("services/", Name = "数据资产services")]
    //[ApiDescriptionSettings(GroupName = "数据资产API服务中心")]
    public abstract class ServicesAppService : IDynamicApiController
    {
        public ServicesAppService()
        {
        }
    }
}
