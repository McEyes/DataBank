using ITPortal.Core.Services;
using DataAssetManager.DataApiServer.Application.DataApi.Dtos;
using DataAssetManager.DataApiServer.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using Furion.JsonSerialization;
using Furion.Logging;
using System.Drawing.Printing;
using DataAssetManager.DataTableServer.Application;
using StackExchange.Profiling.Internal;
using Furion.HttpRemote;
using static Dm.net.buffer.ByteArrayBuffer;
using System.Security.Cryptography;
using SqlSugar;
using ITPortal.Extension.System;

namespace DataAssetManager.DataApiServer.Application.DataAsset
{
    /// <summary>
    /// 数据资产Api 服务
    /// </summary>
    [AppAuthorize]
    [Route("api/dicts/", Name = "数据资产Sys Dict服务")]
    [ApiDescriptionSettings(GroupName = "数据资产Sys Dict")]
    public class SysDictAppService : IDynamicApiController
    {
        private readonly ISysDictService _service;
        public SysDictAppService(ISysDictService service)
        {
            _service = service;
        }

        /// <summary>
        /// 初始化数据数据字典数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitFeed")]
        public async Task InitFeed()
        {
            var dict = new SysDictEntity()
            {
                DictName = "PGSQL数据类型",
                DictCode = "data_type_pgsql"
            };
            await _service.Create(dict);
            //await _service.Create(new SysDictItemEntity()
            //{
            //    DictId = dict.Id,
            //    ItemText = "smallint",//数据库类型
            //    ItemValue = "tinyint微小整数",//类型说明
            //    ItemData = "byte",//C#类型
            //    ItemSort = 1,//序号，第几个
            //    Status = 1,
            //});// PostgreSQL数值类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "smallint",
                ItemValue = "2字节有符号整数，范围-32768到32767",
                ItemData = "short",
                ItemSort = 1,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "int2",
                ItemValue = "smallint的别名",
                ItemData = "short",
                ItemSort = 2,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "integer",
                ItemValue = "int，4字节有符号整数，范围-2147483648到2147483647",
                ItemData = "int",
                ItemSort = 3,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "int4",
                ItemValue = "int,integer的别名",
                ItemData = "int",
                ItemSort = 4,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "bigint",
                ItemValue = "long,8字节有符号整数，范围-9223372036854775808到9223372036854775807",
                ItemData = "long",
                ItemSort = 5,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "int8",
                ItemValue = "long,bigint的别名",
                ItemData = "long",
                ItemSort = 6,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "decimal",
                ItemValue = "精确小数，用户指定精度",
                ItemData = "decimal",
                ItemSort = 7,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "numeric",
                ItemValue = "decimal的别名",
                ItemData = "decimal",
                ItemSort = 8,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "real",
                ItemValue = "float,4字节单精度浮点数",
                ItemData = "float",
                ItemSort = 9,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "float4",
                ItemValue = "float,real的别名",
                ItemData = "float",
                ItemSort = 10,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "double precision",
                ItemValue = "double,8字节双精度浮点数",
                ItemData = "double",
                ItemSort = 11,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "float8",
                ItemValue = "double precision的别名",
                ItemData = "double",
                ItemSort = 12,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "serial",
                ItemValue = "int,自动增长整数，范围同integer",
                ItemData = "int",
                ItemSort = 13,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "serial4",
                ItemValue = "int,serial的别名",
                ItemData = "int",
                ItemSort = 14,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "bigserial",
                ItemValue = "long,自动增长大整数，范围同bigint",
                ItemData = "long",
                ItemSort = 15,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "serial8",
                ItemValue = "long,bigserial的别名",
                ItemData = "long",
                ItemSort = 16,
                Status = 1,
            });

            // PostgreSQL字符串类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "varchar",
                ItemValue = "string,可变长度字符串",
                ItemData = "string",
                ItemSort = 17,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "char",
                ItemValue = "string,固定长度字符串",
                ItemData = "string",
                ItemSort = 18,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "character",
                ItemValue = "string,char的别名",
                ItemData = "string",
                ItemSort = 19,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "text",
                ItemValue = "string,变长字符串，无长度限制",
                ItemData = "string",
                ItemSort = 20,
                Status = 1,
            });

            // PostgreSQL二进制类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "bytea",
                ItemValue = "byte[],二进制数据",
                ItemData = "byte[]",
                ItemSort = 21,
                Status = 1,
            });

            // PostgreSQL日期时间类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "timestamp",
                ItemValue = "DateTime，日期和时间，无时区",
                ItemData = "DateTime",
                ItemSort = 22,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "timestamp without time zone",
                ItemValue = "DateTime，带日期和时间，无时区信息",
                ItemData = "DateTime",
                ItemSort = 23,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "timestamp with time zone",
                ItemValue = "DateTimeOffset,带日期、时间和时区信息",
                ItemData = "DateTimeOffset",
                ItemSort = 24,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "date",
                ItemValue = "DateOnly,仅日期部分",
                ItemData = "DateOnly",
                ItemSort = 25,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "time",
                ItemValue = "TimeOnly,仅时间部分，无时区",
                ItemData = "TimeOnly",
                ItemSort = 26,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "time without time zone",
                ItemValue = "TimeOnly,仅时间部分，无时区信息",
                ItemData = "TimeOnly",
                ItemSort = 27,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "time with time zone",
                ItemValue = "DateTimeOffset，仅时间部分，带时区信息",
                ItemData = "DateTimeOffset",
                ItemSort = 28,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "interval",
                ItemValue = "TimeSpan,时间间隔",
                ItemData = "TimeSpan",
                ItemSort = 29,
                Status = 1,
            });

            // PostgreSQL布尔类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "boolean",
                ItemValue = "bool，布尔值，true或false",
                ItemData = "bool",
                ItemSort = 30,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "bool",
                ItemValue = "bool，boolean的别名",
                ItemData = "bool",
                ItemSort = 31,
                Status = 1,
            });

            // PostgreSQL货币类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "money",
                ItemValue = "decimal,货币金额",
                ItemData = "decimal",
                ItemSort = 32,
                Status = 1,
            });

            // PostgreSQL网络地址类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "inet",
                ItemValue = "string，IPv4或IPv6网络地址",
                ItemData = "string",
                ItemSort = 33,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "cidr",
                ItemValue = "string，无类别域间路由地址",
                ItemData = "string",
                ItemSort = 34,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "macaddr",
                ItemValue = "string，MAC地址（6字节）",
                ItemData = "string",
                ItemSort = 35,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "macaddr8",
                ItemValue = "string，MAC地址（8字节）",
                ItemData = "string",
                ItemSort = 36,
                Status = 1,
            });

            // PostgreSQL几何类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "point",
                ItemValue = "ValueTuple<double, double>,几何点（x,y坐标）",
                ItemData = "ValueTuple<double, double>",
                ItemSort = 37,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "line",
                ItemValue = "string,无限长直线",
                ItemData = "string",
                ItemSort = 38,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "lseg",
                ItemValue = "string,线段",
                ItemData = "string",
                ItemSort = 39,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "box",
                ItemValue = "string,矩形框",
                ItemData = "string",
                ItemSort = 40,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "path",
                ItemValue = "string,几何路径",
                ItemData = "string",
                ItemSort = 41,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "polygon",
                ItemValue = "string,多边形",
                ItemData = "string",
                ItemSort = 42,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "circle",
                ItemValue = "string,圆形",
                ItemData = "string",
                ItemSort = 43,
                Status = 1,
            });

            // PostgreSQL文本搜索类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "tsvector",
                ItemValue = "string,文本搜索向量",
                ItemData = "string",
                ItemSort = 44,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "tsquery",
                ItemValue = "string,文本搜索查询",
                ItemData = "string",
                ItemSort = 45,
                Status = 1,
            });

            // PostgreSQL UUID类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "uuid",
                ItemValue = "Guid，通用唯一标识符",
                ItemData = "Guid",
                ItemSort = 46,
                Status = 1,
            });

            // PostgreSQL JSON类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "json",
                ItemValue = "string,JSON数据",
                ItemData = "string",
                ItemSort = 47,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "jsonb",
                ItemValue = "string,二进制JSON数据，支持索引",
                ItemData = "string",
                ItemSort = 48,
                Status = 1,
            });

            // PostgreSQL数组类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "ARRAY",
                ItemValue = "object[]，数组类型，可包含任何基础类型",
                ItemData = "object[]",
                ItemSort = 49,
                Status = 1,
            });

            // PostgreSQL其他特殊类型
            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "oid",
                ItemValue = "uint,对象标识符",
                ItemData = "uint",
                ItemSort = 50,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "xid",
                ItemValue = "uint,事务标识符",
                ItemData = "uint",
                ItemSort = 51,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "cid",
                ItemValue = "uint,命令标识符",
                ItemData = "uint",
                ItemSort = 52,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "xml",
                ItemValue = "string,XML数据",
                ItemData = "string",
                ItemSort = 53,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "pg_lsn",
                ItemValue = "string,PostgreSQL日志序列号",
                ItemData = "string",
                ItemSort = 54,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "txid_snapshot",
                ItemValue = "string,事务ID快照",
                ItemData = "string",
                ItemSort = 55,
                Status = 1,
            });

            await _service.Create(new SysDictItemEntity()
            {
                DictId = dict.Id,
                ItemText = "pg_node_tree",
                ItemValue = "string,PostgreSQL规划器节点树",
                ItemData = "string",
                ItemSort = 56,
                Status = 1,
            });

        }



        #region SysDict

        /// <summary>
        /// 所有数据字典数据
        /// </summary>
        /// <param name="clearCache">是否重新加载缓存</param>
        /// <returns></returns>
        [HttpGet("All")]
        public async Task<List<SysDictDto>> AllFromCache(bool clearCache = false)
        {
            return await _service.All();
        }

        [HttpGet("{id}")]
        public async Task<SysDictEntity> Get(string id)
        {
            return await _service.Get(id);
        }

        [HttpGet("code/{code}")]
        public async Task<List<SysDictDto>> GetByCode(string code)
        {
            return await _service.GetByCode(code);
        }

        [HttpGet("codes/{codes}")]
        public async Task<List<SysDictDto>> GetByCodes(string codes)
        {
            return await _service.GetByCode(codes?.Split(new char[] { ',', ';', '，' }, StringSplitOptions.RemoveEmptyEntries));
        }
        [HttpPost("codes")]
        public async Task<List<SysDictDto>> GetByCodes(string[] codes)
        {
            return await _service.GetByCode(codes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost()]
        public async Task<int> Post(SysDictEntity entity)
        {
            return await _service.Create(entity);
        }

        [HttpPut()]
        public async Task<int> Put(SysDictEntity entity)
        {
            return await _service.Modify(entity);
        }

        [HttpDelete("{id}")]
        public async Task<bool> Delete(string id)
        {
            return await _service.Delete(id);
        }

        [HttpDelete("Batch")]
        public async Task<bool> Delete(string[] ids)
        {
            return await _service.Delete(ids);
        }

        [HttpGet("list")]
        public async Task<List<SysDictEntity>> Query([FromQuery] SysDictDto filter)
        {
           return await _service.Query(filter);
        }

        [HttpPost("list")]
        public async Task<List<SysDictEntity>> Query2(SysDictDto filter)
        {
            return await _service.Query(filter);
        }

        [HttpGet("page")]
        public async Task<PageResult<SysDictEntity>> Page([FromQuery] SysDictDto filter)
        {
            return await _service.Page(filter);
        }

        [HttpPost("page")]
        public async Task<PageResult<SysDictEntity>> Page2(SysDictDto filter)
        {
            return await _service.Page(filter);
        }


        [HttpGet("Refresh")]
        [HttpPost("Refresh")]
        public async Task Refresh()
        {
            await this._service.Refresh();
        }

        public async Task<SysDictEntity> Single(SysDictDto filter)
        {
            return await this._service.Single(filter);
        }


        #endregion SysDict


        #region SysDictItem

        [HttpPost("/api/Dict")]
        public async Task<int> Post(SysDictItemEntity entity)
        {
            return await _service.Create(entity);
        }


        [HttpDelete("/api/dict/{id}")]
        public async Task<bool> DeleteItem(string id)
        {
            var result= await _service.Delete<SysDictItemEntity>(id);
            await Task.Run(this.Refresh);
            return result;
        }

        [HttpDelete("/api/dict/Batch")]
        public async Task<bool> DeleteItems(string[] ids)
        {
            var result = await _service.Delete(ids);
            await Task.Run(this.Refresh);
            return result;
        }

        [HttpDelete("/api/dict/code/{code}")]
        public async Task<bool> DeleteItemsByCode(string code)
        {
            var result = await _service.DeleteItemsByCode(code);
            await Task.Run(this.Refresh);
            return result;
        }

        [HttpDelete("/api/dict/pid/{pid}")]
        public async Task<bool> DeleteItemsByPid(string pid)
        {
            var result = await _service.DeleteItemsByPid(pid);
            await Task.Run(this.Refresh);
            return result;
        }


        [HttpGet("/api/dict/{pid}")]
        public async Task<SysDictDto> GetItem(string id)
        {
            return await _service.GetItem(id);
        }

        [HttpPut("/api/dict")]
        public async Task<int> Put(SysDictItemEntity entity)
        {
            var result = await _service.Modify(entity);
            await Task.Run(this.Refresh);
            return result;
        }

        [HttpPost("/api/dict/list")]
        public async Task<List<SysDictDto>> GetItems(SysDictDto filter)
        {
            var list = await _service.AllFromCache();
            var query = list.Where(f=>f.DictStatus==1).WhereIF(!string.IsNullOrWhiteSpace(filter.DictCode), f => filter.DictCode.Equals(f.DictCode, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.Keyword), f => filter.Keyword.Equals(f.DictCode, StringComparison.InvariantCultureIgnoreCase)
                        || filter.Keyword.Equals(f.DictName, StringComparison.InvariantCultureIgnoreCase) || filter.Keyword.Contains(f.ItemText, StringComparison.InvariantCultureIgnoreCase) || filter.Keyword.Contains(f.ItemTextEn, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.DictName), f => filter.DictName.Equals(f.DictName, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.ItemText), f => filter.ItemText.Equals(f.ItemText, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                        .WhereIF(filter.DictStatus.HasValue, f => f.DictStatus == filter.DictStatus)
                        .OrderBy(f => f.DictCode).ThenBy(f => f.ItemSort);
            return query.ToList();
        }


        [HttpGet("/api/dict/list")]
        public async Task<List<SysDictDto>> GetItems2([FromQuery] SysDictDto filter)
        {
            return await GetItems(filter);
        }


        [HttpPost("/api/dict/page")]
        public async Task<PageResult<SysDictDto>> PageItems(SysDictDto filter)
        {
            var list = await _service.AllFromCache();
            var query = list.Where(f => f.DictStatus == 1).Where(f=>f.Id.IsNotNullOrWhiteSpace()).WhereIF(!string.IsNullOrWhiteSpace(filter.DictCode), f => filter.DictCode.Equals(f.DictCode, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.Keyword), f => filter.Keyword.Equals(f.DictCode, StringComparison.InvariantCultureIgnoreCase)
                        || filter.Keyword.Equals(f.DictName, StringComparison.InvariantCultureIgnoreCase) || filter.Keyword.Contains(f.ItemText, StringComparison.InvariantCultureIgnoreCase) || filter.Keyword.Contains(f.ItemTextEn, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.DictId), f => filter.DictId.Equals(f.DictId))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.DictName), f => filter.DictName.Equals(f.DictName, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(!string.IsNullOrWhiteSpace(filter.ItemText), f => filter.ItemText.Equals(f.ItemText, StringComparison.InvariantCultureIgnoreCase))
                        .WhereIF(filter.Status.HasValue, f => f.Status == filter.Status)
                        .WhereIF(filter.DictStatus.HasValue, f => f.DictStatus == filter.DictStatus)
                        .OrderBy(f => f.DictCode).ThenBy(f => f.ItemSort);

            return new PageResult<SysDictDto>(query.Count(), query.Skip(filter.SkipCount).Take(filter.PageSize).ToList(), filter.PageNum, filter.PageSize);
        }



        [HttpGet("/api/dict/page")]
        public async Task<PageResult<SysDictDto>> PageItems2([FromQuery] SysDictDto filter)
        {
            return await PageItems(filter);
        }

        #endregion SysDictItem
    }
}
