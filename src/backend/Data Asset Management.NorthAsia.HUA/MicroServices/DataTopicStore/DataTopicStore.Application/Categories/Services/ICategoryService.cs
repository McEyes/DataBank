using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Topics.Dtos;

namespace DataTopicStore.Application.Categories.Services
{
    public interface ICategoryService
    {
        Task<bool> SaveAsync(CreateOrUpdateCategoryDto input);
        Task<bool> DeleteAsync(Guid id);
        Task<List<CategoryTreeNodeDto>> GetCategoryTreeNodesAsync();
        Task<List<CategoryListItemDto>> GetChildrenAsync(Guid id, bool includeSelf = true);
    }
}
