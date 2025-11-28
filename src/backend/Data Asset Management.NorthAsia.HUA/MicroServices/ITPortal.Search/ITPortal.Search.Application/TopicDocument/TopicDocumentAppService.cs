using ITPortal.Core.Services;
using ITPortal.Search.Application.ElasticManager;
using ITPortal.Search.Application.SearchTopic.Services;
using ITPortal.Search.Application.TopicDocument.Dtos;
using ITPortal.Search.Core.Models.Search;

using Newtonsoft.Json;

namespace ITPortal.Search.Application.TopicDocument
{
    [AppAuthorize]
    [Route("api/TopicDocument/", Name = "全局搜索 主题文档服务")]
    [ApiDescriptionSettings(GroupName = "全局搜索 主题文档")]
    public class TopicDocumentAppService : IDynamicApiController
    {

        private readonly SearchManager _searchManager;
        private readonly ISearchTopicService _searchTopicService;


        public TopicDocumentAppService(
            SearchManager searchManager,
            ISearchTopicService searchTopicService
            )
        {
            _searchManager = searchManager;
            _searchTopicService = searchTopicService;
        }

        /// <summary>
        /// 查询主题文档
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet()]
        //[SwaggerOperation(summary: "查询主题文档")]
        public async Task<PageResult<TopicDocumentDto>> Query(TopicDocumentQueryDto query)
        {
            var queryable = query.Adapt<TopicDocumentQueryModel>();

            if (queryable.Classify == null) query.Classify = null;

            return await _searchManager.QueryTopicDocuments(queryable, null);
        }
        /// <summary>
        /// 根据ID获取主题文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        //[SwaggerOperation(summary: "根据ID获取主题文档")]
        public async Task<TopicDocumentDto> GetTopicDocumentsById(string id)
        {
            var doc = await _searchManager.GetTopicDocumentById(id);
            if (doc == null)
            {
                throw new AppFriendlyException("Topic Document Nount Fount", "500");
                //return Result<TopicDocumentDto>.Fail("Topic Document Nount Fount");
            }
            return doc.Adapt<TopicDocumentDto>();
        }

        /// <summary>
        /// 创建主题文档 - 存在ID记录则更新文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPost()]
        //[SwaggerOperation(summary: "创建主题文档 - 存在ID记录则更新文档")]
        public async Task<string> Create(TopicDocumentCreateDto document)
        {
            string topicName = document.Topic;
            var topic = await _searchTopicService.GetByTopic(topicName);
            if (topic == null || !topic.IsEnable)
            {
                throw new AppFriendlyException("Topic does not exist", "500");
                //return Result<string>.Fail("Topic does not exist");
            }
            var model = document.Adapt<TopicDocumentModel>(); //ObjectMapper.Map<TopicDocumentCreateDto, TopicDocumentModel>(document);
            model.Creator = document.Operator;
            model.CreateTime = DateTimeOffset.Now;
            model.Updater = model.Creator;
            model.UpdateTime = model.CreateTime;
            string id = await _searchManager.CreateTopicDocument(model);
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new AppFriendlyException("Insert Topic Document Failed", "200");
                //return Result<string>.Fail("Insert Topic Document Failed");
            }
            return id;
        }


        /// <summary>
        /// 更新主题文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        [HttpPut()]
        //[SwaggerOperation(summary: "更新主题文档")]
        public async Task Update(TopicDocumentUpdateDto document)
        {
            var topic = await _searchTopicService.GetByTopic(document.Topic);
            if (topic == null || !topic.IsEnable)
            {
                throw new AppFriendlyException("Topic does not exist", "500");
                //return Result<string>.Fail("Topic does not exist");
            }

            var model = document.Adapt<TopicDocumentModel>();

            model.Updater = document.Operator;
            model.UpdateTime = DateTimeOffset.Now;

            var res = await _searchManager.UpdateTopicDocument(model);

            if (!res)
            {
                throw new AppFriendlyException("Update Topic Document Failed", "500");
                //return Result.Fail("Update Topic Document Failed");
            }

            //return Result.Successd();
        }


        /// <summary>
        /// 删除主题文档
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        //[SwaggerOperation(summary: "删除主题文档")]
        public async Task Delete(string id)
        {
            var res = await _searchManager.DeleteTopicDocument(new List<string> { id });

            if (!res)
                throw new AppFriendlyException("delete topic document fail", "500"); 
            //return Result.Fail("delete topic document fail");

            //return Result.Successd();
        }
        /// <summary>
        /// 批量删除主题文档
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("DeleteManay")]
        //[SwaggerOperation(summary: "批量删除主题文档")]
        public async Task DeleteTopicDocumentMandy(List<string> ids)
        {
            var res = await _searchManager.DeleteTopicDocument(ids);

            if (!res)
                throw new AppFriendlyException("delete fail", "500");
            //return Result.Fail("delete fail");

            //return Result.Successd();
        }

    }
}
