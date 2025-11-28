using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTopicStore.Application.Categories.Dtos;
using DataTopicStore.Application.Topics.Dtos;
using DataTopicStore.Core.Entities;
using DataTopicStore.Core.Repositories;

namespace DataTopicStore.Application.Categories.Services
{
    public class CategoryService : ApplicationService, ICategoryService
    {
        private readonly Repository<CategoryEntity> categoryRepository;
        private List<CategoryEntity> categoryEntities;
        public CategoryService(Repository<CategoryEntity> categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<bool> SaveAsync(CreateOrUpdateCategoryDto input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));
            ArgumentNullException.ThrowIfNullOrWhiteSpace(input.name, nameof(input.name));
            var isAdd = false;

            if (input.id == null || input.id == Guid.Empty)
            {
                isAdd = true;
                input.id = Guid.NewGuid();
            }

            // Check if the parent node is valid
            categoryEntities = await categoryRepository.GetListAsync();
            if (input.id == input.parent_id) throw Oops.Oh("Invalid parent node");

            // Checks if the current node is equal to the parent node 
            if (input.parent_id != null)
            {
                var tnode = categoryEntities.FirstOrDefault(t => t.id == input.parent_id);
                if (tnode == null) throw Oops.Oh("No parent node found");
            }

            // Check if the parent node is a child of the current node
            #region  Check if the parent node is a child of the current node

            List<CategoryEntity> categories = new List<CategoryEntity>();
            RecGetAllNodes(input.id.Value,categories);
            var cate = categories.FirstOrDefault(t => t.id == input.parent_id);
            if (!isAdd && cate != null)
            {
                throw Oops.Oh("The parent node of the current node is invalid");
            }

            #endregion

            var entity = categoryEntities.FirstOrDefault(t => t.id == input.id);
            if (entity == null)
            {
                entity = input.Adapt<CategoryEntity>();
                entity.created_by = "";
                entity.created_time = DateTime.Now;
            }
            else
            {
                entity = input.Adapt(entity);
                entity.updated_by = "";
                entity.updated_time = DateTime.Now;
            }

            return await categoryRepository.InsertOrUpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            categoryEntities = await categoryRepository.GetListAsync();
            var ids = new List<string>() { id.ToString() };
            RecGetIds(id, ids);
            return await categoryRepository.DeleteByIdsAsync(ids.ToArray());
        }

        public async Task<List<CategoryTreeNodeDto>> GetCategoryTreeNodesAsync()
        {
            categoryEntities = await categoryRepository.GetListAsync();
            var treeNodes = categoryEntities.Where(t => t.parent_id == null).Select(t => new CategoryTreeNodeDto
            {
                id = t.id,
                name = t.name,
                parent_id = t.parent_id
            }).ToList();

            RecTree(treeNodes);
            return treeNodes;
        }

        public async Task<List<CategoryListItemDto>> GetChildrenAsync(Guid id, bool includeSelf = true)
        {
            categoryEntities = await categoryRepository.GetListAsync();
            List<CategoryEntity> list = new List<CategoryEntity>();
            if (categoryEntities == null) return null;
            if (includeSelf)
            {
                var currentNode = categoryEntities.FirstOrDefault(t => t.id == id);
                if (currentNode != null)
                {
                    list.Add(currentNode);
                }
                else
                {
                    return null;
                }
            }

            RecGetAllNodes(id, list);

            return list.Adapt<List<CategoryListItemDto>>();
        }

        private void RecGetIds(Guid id, List<string> ids)
        {
            if (categoryEntities == null || categoryEntities.Count == 0) return;
            var nodes = categoryEntities.Where(t => t.parent_id == id);
            ids.AddRange(nodes.Select(t => t.id.ToString()));
            foreach (var item in nodes)
            {
                RecGetIds(item.id, ids);
            }
        }

        private void RecGetAllNodes(Guid id, List<CategoryEntity> list)
        {
            if (categoryEntities == null || categoryEntities.Count == 0)
            {
                return;
            }

            var nodes = categoryEntities.Where(t => t.parent_id == id);
            list.AddRange(nodes.Select(t => t));
            foreach (var item in nodes)
            {
                RecGetAllNodes(item.id, list);
            }
        }

        private void RecTree(List<CategoryTreeNodeDto> list)
        {
            if (categoryEntities == null || categoryEntities.Count == 0) return;
            foreach (var item in list)
            {
                var childrenNodes = categoryEntities.Where(t => t.parent_id == item.id).Select(t => new CategoryTreeNodeDto
                {
                    id = t.id,
                    name = t.name,
                    parent_id = t.parent_id
                }).ToList();
                item.children = childrenNodes;

                RecTree(item.children);
            }
        }
    }
}
