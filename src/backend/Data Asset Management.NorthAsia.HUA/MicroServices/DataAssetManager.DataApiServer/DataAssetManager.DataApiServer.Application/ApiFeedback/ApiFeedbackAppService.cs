using System;

using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Application.DataUser.EmployeeInfo.Services;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Emails;
using ITPortal.Core.ProxyApi;
using ITPortal.Core.Services;
using ITPortal.Extension.System;

using MapsterMapper;

using Microsoft.Extensions.Logging;

namespace DataAssetManager.DataApiServer.Application.DataApi
{
    /// <summary>
    /// 问题反馈api集合
    /// </summary>
    [AppAuthorize]
    [Route("api/ApiFeedback/", Name = "数据资产 Feedback服务")]
    [ApiDescriptionSettings(GroupName = "数据资产 Feedback")]
    public class ApiFeedbackAppService : IDynamicApiController
    {
        private readonly IApiFeedbackService _apiFeedbackService;
        private readonly IDataTableService _dataTableService;
        private readonly IDistributedCacheService _cache;
        private readonly IEmailSender _emailSender;
        private readonly IEmployeeBaseInfoService _employeeProxyService;
        private readonly ILogger<ApiFeedbackAppService> _logger;

        public ApiFeedbackAppService(IApiFeedbackService dataApiService, ILogger<ApiFeedbackAppService> logger, IDataTableService dataApiService1,IEmailSender emailSender, IEmployeeBaseInfoService employeeProxyService, IDistributedCacheService cache)
        {
            _apiFeedbackService = dataApiService;
            _cache = cache;
            _dataTableService = dataApiService1;
            _employeeProxyService = employeeProxyService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("GetDate")]
        public dynamic GetDate()
        {
            return new
            {
                Date = DateTime.Now,
                DateToString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OffsetNow = DateTimeOffset.Now,
                OffsetNowToString = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                OffsetNowToLocalTimeToString = DateTimeOffset.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                UtcNow = DateTimeOffset.UtcNow,
                UtcNowToString = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                UtcNowToLocalTimeToString = DateTimeOffset.UtcNow.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
            };
        }
        #region base api

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost()]
        public async Task<int> Create(FeedbackInputDto entity)
        {
            DataTableInput tableInfo = null;
            var userInfo = App.HttpContext.GetCurrUserInfo();
            try
            {
                var data = entity.Adapt<ApiFeedbackEntity>();
                data.UserId = userInfo.UserId;
                data.UserName = userInfo.Name;
                data.UserEmail = userInfo.Email;
                data.ObjectType = "table";
                tableInfo = await _dataTableService.GetInfo(entity.ObjectId);
                if (tableInfo == null)
                    throw new Exception("The table information corresponding to the parameter ObjectId does not exist");
                else
                {
                    data.OwnerId = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerId));
                    data.OwnerName = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerName));
                    //data.OwnerDept = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerDept));
                }
                return await _apiFeedbackService.Create(data);
            }
            finally
            {
                try
                {
                    if (tableInfo != null && tableInfo.OwnerList != null && tableInfo.OwnerList.Count > 0)
                    {
                        var owners = await _employeeProxyService.GetEmployeeListAsync(tableInfo.OwnerList.Select(f => f.OwnerId).ToArray());

                        await _emailSender.SendAsync(new EmailMessage()
                        {
                            MailTo = owners.Where(f => f.WorkEmail.IsNotNullOrWhiteSpace()).Select(f => f.WorkEmail).Distinct().ToArray()
                          , Subject = $"{tableInfo.TableComment}的api反馈-{userInfo.Name}",
                            Html = @$"Hi {owners.First().Name}:<br>     
   收到来自{tableInfo.TableComment}的api反馈信息，详细信息如下：</br>    
{entity.Description}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(tableInfo.TableComment + "的反馈发送邮件异常：" + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<bool> Delete(Guid id)
        {
            return await _apiFeedbackService.Delete(id);
        }


        /// <summary>
        /// 获取详细信息
        /// 根据url的id来获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ApiFeedbackEntity> Get(Guid id)
        {
            return await _apiFeedbackService.Get(id);
        }


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete("batch")]
        public async Task<bool> Batch(dynamic[] ids)
        {
            return await _apiFeedbackService.Delete(ids);
        }

        /// <summary>
        /// 获取单个数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ApiFeedbackEntity> Single(ApiFeedbackDto entity)
        {
            return await _apiFeedbackService.Single(entity);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("{id}")]
        public async Task<int> Put(Guid id, ApiFeedbackEntity entity)
        {
            if (id != Guid.Empty) entity.Id = id;

            var data = await Get(entity.Id);
            var userInfo = App.HttpContext.GetCurrUserInfo();
            data.UserId = userInfo.UserId;
            data.UserName = userInfo.UserName;
            data.UserEmail = userInfo.Email;
            data.ObjectType = "table";
            var tableInfo = await _dataTableService.GetInfo(entity.ObjectId);
            if (tableInfo == null)
                throw new Exception("The table information corresponding to the parameter ObjectId does not exist");
            else
            {
                data.OwnerId = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerId));
                data.OwnerName = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerName));
                //data.OwnerDept = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerDept));
            }
            return await _apiFeedbackService.Modify(entity);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut("ModifyHasChange")]
        public async Task<bool> ModifyHasChange(ApiFeedbackEntity entity)
        {
            var data = await Get(entity.Id);
            var userInfo = App.HttpContext.GetCurrUserInfo();
            data.UserId = userInfo.UserId;
            data.UserName = userInfo.UserName;
            data.UserEmail = userInfo.Email;
            data.ObjectType = "table";
            var tableInfo = await _dataTableService.GetInfo(entity.ObjectId);
            if (tableInfo == null)
                throw new Exception("The table information corresponding to the parameter ObjectId does not exist");
            else
            {
                data.OwnerId = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerId));
                data.OwnerName = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerName));
                //data.OwnerDept = string.Join(",", tableInfo.OwnerList.Select(f => f.OwnerDept));
            }
            return await _apiFeedbackService.ModifyHasChange(entity);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("page")]
        public async Task<PageResult<ApiFeedbackEntity>> Page([FromQuery]ApiFeedbackDto filter)
        {
            return await _apiFeedbackService.Page(filter);
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost("page")]
        public async Task<PageResult<ApiFeedbackEntity>> Page2(ApiFeedbackDto filter)
        {
            return await _apiFeedbackService.Page(filter);
        }
        /// <summary>
        /// 返回满足条件的集合
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<List<ApiFeedbackEntity>> Query([FromQuery] ApiFeedbackDto entity)
        {
            return await _apiFeedbackService.Query(entity);
        }
        /// <summary>
        /// 返回满足条件的集合
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<List<ApiFeedbackEntity>> Query2(ApiFeedbackDto entity)
        {
            return await _apiFeedbackService.Query(entity);
        }

        #endregion base api
    }
}
