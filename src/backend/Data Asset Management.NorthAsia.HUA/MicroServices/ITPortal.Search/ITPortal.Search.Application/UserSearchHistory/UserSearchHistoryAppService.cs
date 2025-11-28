using ITPortal.Core.Services;
using ITPortal.Search.Application.UserSearchHistory.Dtos;
using ITPortal.Search.Core.Models;

namespace ITPortal.Search.Application.UserSearchHistory.Services
{
    /// <summary>
    /// 用户搜索历史
    /// </summary>
    [AppAuthorize]
    [Route("api/UserSearchHistory/", Name = "全局搜索 UserSearchHistory服务")]
    [ApiDescriptionSettings(GroupName = "全局搜索 UserSearchHistory")]//,Groups =new string[] { "全局搜索" }
    public class UserSearchHistoryAppService : IDynamicApiController
    {
        private readonly IUserSearchHistoryService _service;

        public UserSearchHistoryAppService(IUserSearchHistoryService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取用户搜索历史
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        //[SwaggerOperation(summary: "获取用户搜索历史")]
        public async Task<List<UserSearchHistoryDto>> Get()
        {
            return await _service.GetHistoryListAsync();
        }


        #region base api


        //[HttpPost()]
        //public async Task<int> Create(UserSearchHistoryEntity entity)
        //{
        //    return await _service.Create(entity);
        //}
        /// <summary>
        /// 删除搜索记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        //[SwaggerOperation(summary: "删除搜索记录")]
        public async Task<bool> Delete(Guid id)
        {
            return await _service.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<UserSearchHistoryEntity> Get(Guid id)
        {
            return await _service.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(string[] ids)
        {
            return await _service.Delete(ids);
        }


        public async Task<UserSearchHistoryEntity> Single(UserSearchHistoryDto entity)
        {
            return await _service.Single(entity);
        }

        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, UserSearchHistoryEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;
            return await _service.Modify(entity);
        }

        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(UserSearchHistoryEntity entity)
        {
            return await _service.ModifyHasChange(entity);
        }

        [HttpGet("page")]
        public async Task<PageResult<UserSearchHistoryEntity>> Page([FromQuery] UserSearchHistoryDto filter)
        {
            return await _service.Page(filter);
        }
        [HttpPost("page")]
        public async Task<PageResult<UserSearchHistoryEntity>> Page2(UserSearchHistoryDto filter)
        {
            return await _service.Page(filter);
        }

        [HttpGet("list")]
        public async Task<List<UserSearchHistoryEntity>> Query([FromQuery] UserSearchHistoryDto entity)
        {
            return await _service.Query(entity);
        }

        [HttpPost("list")]
        public async Task<List<UserSearchHistoryEntity>> Query2(UserSearchHistoryDto entity)
        {
            return await _service.Query(entity);
        }

        #endregion base api
    }
}
