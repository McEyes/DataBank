using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Categories.Services;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Application.Topics.Services;
using Newtonsoft.Json;

namespace DataTopicStore.Application.Categories
{
    [AppAuthorize]
    [Route("/api/category")]
    public class CategoryAppService : IDynamicApiController
    {
        private readonly ICategoryService categoryService;
        public CategoryAppService(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpPost]
        public Task<bool> SaveAsync(CreateOrUpdateCategoryDto input) => categoryService.SaveAsync(input);

        [HttpDelete]
        public Task<bool> DeleteAsync(Guid id) => categoryService.DeleteAsync(id);

        [HttpGet("tree-nodes")]
        public Task<List<CategoryTreeNodeDto>> GetCategoryTreeNodesAsync() => categoryService.GetCategoryTreeNodesAsync();
    }
}
