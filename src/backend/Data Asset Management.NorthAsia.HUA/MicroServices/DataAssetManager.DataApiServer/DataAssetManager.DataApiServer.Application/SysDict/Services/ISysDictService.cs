using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using ITPortal.Core.Services;

namespace DataAssetManager.DataTableServer.Application
{
    public interface ISysDictService : IBaseService<SysDictEntity, SysDictDto, string>
    {
        Task<List<SysDictDto>> All();
        Task<List<SysDictDto>> AllFromCache(bool clearCache = false);
        ISugarQueryable<SysDictEntity> BuildFilterQuery(SysDictDto filter);
        Task<int> Create(SysDictEntity entity);
        Task<int> Create(SysDictItemEntity entity);
        Task<bool> DeleteItem(string id);
        Task<bool> DeleteItems(string[] ids);
        Task<bool> DeleteItemsByCode(string code);
        Task<bool> DeleteItemsByPid(string pid);
        Task<List<SysDictDto>> GetByCode(string code);
        Task<List<SysDictDto>> GetByCode(string[] codes);
        Task<SysDictDto> GetItem(string id);
        Task<int> Modify(SysDictEntity entity);
        Task<int> Modify(SysDictItemEntity entity);
        Task Refresh();
    }
}
