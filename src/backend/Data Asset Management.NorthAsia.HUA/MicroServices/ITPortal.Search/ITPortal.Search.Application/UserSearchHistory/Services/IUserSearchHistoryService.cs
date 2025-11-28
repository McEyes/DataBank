using ITPortal.Core.Services;
using ITPortal.Search.Application.SearchTopic.Dtos;
using ITPortal.Search.Application.UserSearchHistory.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.UserSearchHistory.Services
{
    public interface IUserSearchHistoryService : IBaseService<UserSearchHistoryEntity, UserSearchHistoryDto, Guid>
    {
        Task<bool> Delete(List<Guid> ids);
        Task<List<UserSearchHistoryDto>> GetHistoryListAsync();
    }
}
