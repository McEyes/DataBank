using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataTableServer.Application;

using ITPortal.Core;
using ITPortal.Core.DistributedCache;
using ITPortal.Core.Services;

using MapsterMapper;

using Microsoft.Extensions.Logging;

using StackExchange.Profiling.Internal;

using System.Data;

namespace DataAssetManager.DataApiServer.Application
{

    public class SysDictService : BaseService<SysDictEntity, SysDictDto, string>, ISysDictService, ITransient
    {
        private readonly ILogger<DataTableService> _logger;
        private readonly IMapper _mapper;
        public SysDictService(ISqlSugarClient db,
            IDistributedCacheService cache,
              IMapper mapper,
            ILogger<DataTableService> logger) : base(db, cache,false,false,true)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public override ISugarQueryable<SysDictEntity> BuildFilterQuery(SysDictDto filter)
        {
            return CurrentDb.Queryable<SysDictEntity>()
               .WhereIF(!string.IsNullOrWhiteSpace(filter.Id), (sd) => sd.Id == filter.Id)
               .WhereIF(!string.IsNullOrWhiteSpace(filter.DictName), (sd) => SqlFunc.ToLower(sd.DictName) == filter.DictName.ToLower())
               .WhereIF(!string.IsNullOrWhiteSpace(filter.DictCode), (sd) => SqlFunc.ToLower(sd.DictCode) == filter.DictCode.ToLower())
               .WhereIF(filter.Status.HasValue, (sd) => sd.Status == filter.Status)
               .OrderByDescending(sd=>sd.CreateTime);
        }

        public async Task<List<SysDictDto>> All()
        {
            var query = CurrentDb.Queryable<SysDictEntity>()
                    .LeftJoin<SysDictItemEntity>((sd, sdi) => sd.Id == sdi.DictId);
            var queryResult = query.Select((sd, sdi) => new SysDictDto
            {
                Id = sdi.Id,
                ItemText = sdi.ItemText,
                ItemTextEn = sdi.ItemTextEn,
                ItemValue = sdi.ItemValue,
                ItemData = sdi.ItemData,
                ItemSort = sdi.ItemSort,
                Status = sdi.Status,
                Remark = sdi.Remark,
                DictId = sdi.DictId,
                DictCode = sd.DictCode,
                DictName = sd.DictName,
                DictRemark = sd.Remark,
                DictStatus = sd.Status
            });
            return await queryResult.ToListAsync();
        }

        public async Task<List<SysDictDto>> AllFromCache(bool clearCache = false)
        {
            //if (clearCache) await _cache.RemoveAsync(DataAssetManagerConst.SySDict_ListKey);
            return await _cache.GetObjectAsync(DataAssetManagerConst.SySDict_ListKey, All, TimeSpan.FromMinutes(10), clearCache);
        }


        public async Task<List<SysDictDto>> GetByCode(string code)
        {
            var list = new List<SysDictDto>();
            if (code.IsNullOrWhiteSpace()) return list;
            var codeList = code.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var allList = await AllFromCache();
            return allList.Where(f => code.Equals(f.DictCode, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }


        public async Task<List<SysDictDto>> GetByCode(string[] codes)
        {
            var list = new List<SysDictDto>();
            if (codes == null || codes.Count() == 0) return list;
            var allList = await AllFromCache();
            return allList.Where(f => codes.Contains(f.DictCode)).ToList();
        }

        public async Task Refresh()
        {
            await AllFromCache(true);
        }

        public async Task<int> Create(SysDictEntity entity)
        {
            var list = await GetByCode(entity.DictCode);
            if (list.Count > 0) throw new Exception($"[{entity.DictCode}]¸Ã×Öµä±àÂëÒÑ´æÔÚ");
            var result = await base.Create(entity);
            await Task.Run(this.Refresh);
            return result;
        }


        public async Task<int> Create(SysDictItemEntity entity)
        {
            var result = await base.Create(entity);
            await Task.Run(this.Refresh);
            return result;
        }


        public async Task<int> Modify(SysDictEntity entity)
        {
            var list = await AllFromCache();
            list = list.Where(f => f.Id != entity.Id && f.DictCode == entity.DictCode).ToList();
            if (list.Count > 0) throw new Exception($"[{entity.DictCode}]¸Ã×Öµä±àÂëÒÑ´æÔÚ");
            var result = await base.Modify(entity);
            await Task.Run(this.Refresh);
            return result;
        }


        public async Task<int> Modify(SysDictItemEntity entity)
        {
            var result = await base.Modify(entity);
            await Task.Run(this.Refresh);
            return result;
        }

        public async Task<SysDictDto> GetItem(string id)
        {
            var list = await AllFromCache();
            return list.Where(f => f.Id.Equals(id)).FirstOrDefault();
        }


        public override async Task<SysDictEntity> Get(string id)
        {
            var list = await AllFromCache();
            return list.Where(f => f.DictId.Equals(id)).Select(f => new SysDictEntity()
            {
                Id = f.DictId,
                DictCode = f.DictCode,
                DictName = f.DictName,
                Status = f.DictStatus,
                Remark = f.DictRemark
            }).FirstOrDefault();
        }


        public async Task<bool> DeleteItemsByPid(string pid)
        {
            var result = await CurrentDb.Deleteable<SysDictItemEntity>().Where(it => it.DictId.Equals(pid)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            await Task.Run(this.Refresh);
            return result > 0;
        }

        public async Task<bool> DeleteItem(string id)
        {
            var result = await CurrentDb.Deleteable<SysDictItemEntity>().Where(it => it.Id.Equals(id)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            await Task.Run(this.Refresh);
            return result > 0;
        }

        public async Task<bool> DeleteItems(string[] ids)
        {
            var result = await CurrentDb.Deleteable<SysDictItemEntity>().Where(it => ids.Contains(it.Id)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            await Task.Run(this.Refresh);
            return result > 0;
        }

        public async Task<bool> DeleteItemsByCode(string code)
        {
            var list = await AllFromCache();
            var ids = list.Where(f => f.DictCode == code).Select(f => f.DictId).Distinct().ToList();
            var result = await CurrentDb.Deleteable<SysDictItemEntity>().Where(it => ids.Contains(it.DictId)).EnableDiffLogEventIF(EnableDiffLogEvent).ExecuteCommandAsync();
            await Task.Run(this.Refresh);
            return result > 0;
        }
    }
}