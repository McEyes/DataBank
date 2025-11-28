using Furion.JsonSerialization;
using Furion.TaskQueue;

using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SqlSugar;

using StackExchange.Redis;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITPortal.Core.DistributedCache
{
    public class MemoryCacheService : IDistributedCacheService//, IDisposable
    {
        public CacheType CacheType => CacheType.Memory;

        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(30);
        private readonly ILogger<MemoryCacheService> _logger;
        private readonly ITaskQueue _taskQueue;
        //// 异步入队
        //await TaskQueued.EnqueueAsync(async (provider, token) => { }, [delay]);
        public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger, ITaskQueue taskQueue)
        {
            _logger = logger;
            _cache = memoryCache;
            _taskQueue = taskQueue;

            //var cacheEntryOptions = new MemoryCacheEntryOptions()
            //// 设置缓存项的大小（即使没有SizeLimit，也建议设置）
            //    .SetSize(1024 * 1024)
            //    // 设置滑动过期时间，防止长期占用内存
            //    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            //    // 同时设置绝对过期时间，确保最终会被回收
            //    .SetAbsoluteExpiration(TimeSpan.FromHours(2));
        }


        private static object _hashlock = new object();
        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry">默认一直存储，不传数据不会超时</param>
        public bool HashSet<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class
        {
            ConcurrentDictionary<string, object> hash = null;
            lock (_hashlock)
            {
                if (!_cache.TryGetValue(key, out hash))
                {
                    hash = new ConcurrentDictionary<string, object>();
                    if (expiry.HasValue)
                    {
                        var options = new MemoryCacheEntryOptions();
                        options.AbsoluteExpirationRelativeToNow = expiry;
                        _cache.Set(key, hash, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expiry });
                    }
                    else _cache.Set(key, hash);
                }
            }
            if (data == null)
            {
                if (hash.ContainsKey(field))
                    return hash.TryRemove(field, out _);
                return true;
            }
            var result = hash.AddOrUpdate(field, data, (key, oldValue) => { return data; });
            return true;
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry">默认一直存储，不传数据不会超时</param>
        public async Task<bool> HashSetAsync<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class
        {
            return await Task.FromResult(HashSet(key, field, data, expiry));
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry">默认一直存储，不传数据不会超时</param>
        public async Task<bool> HashSetAsync(string key, string field, string data, TimeSpan? expiry = null)
        {
            return await Task.FromResult(HashSet<string>(key, field, data, expiry));
        }

        /// <summary>
        /// 获取HashGet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string field) where T : class
        {
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, object> hash))
            {
                if (hash.TryGetValue(field, out object data))
                    return (T)data;
                else
                {
                    _logger.LogError($"从内存缓存中获取key={key},field={field}的数据失败");
                }
            }
            else
            {
                _logger.LogError($"从内存缓存中获取key={key}的数据失败");
            }
            return default(T);
        }

        /// <summary>
        /// 获取HashGet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public List<T> HashGetAll<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            List<T> list = new List<T>();
            if (_cache.TryGetValue(key, out ConcurrentDictionary<string, object> hash))
            {
                foreach (var item in hash)
                {
                    if (item.Value != null)
                        list.Add((T)item.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public List<T> HashGetAll<T>(string key, Func<List<T>>? action = null, CommandFlags flags = CommandFlags.None) where T : class
        {
            List<T>? list = HashGetAll<T>(key, flags);
            if (list.Count == 0 && action != null)
            {
                list = action();
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<List<T>> HashGetAllAsync<T>(string key, Func<Task<List<T>>>? action = null, CommandFlags flags = CommandFlags.None) where T : class
        {
            List<T>? list = HashGetAll<T>(key, flags);
            if (list.Count == 0 && action != null)
            {
                list = await action();
            }
            return list;
        }


        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry">默认30分钟</param>
        public bool HashSet(string key, string field, string data, TimeSpan? expiry = null)
        {
            if (expiry == null) expiry = _defaultExpiry;
            return HashSet<string>(key, field, data, expiry);
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        public bool HashDelete(string key, string field)
        {
            ConcurrentDictionary<string, object> hash = null;
            if (_cache.TryGetValue(key, out hash))
            {
                return hash.TryRemove(field, out var value);
            }
            return true;
        }

        /// <summary>
        /// 获取HashGet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string HashGet(string key, string field)
        {
            return HashGet<string>(key, field);
        }


        /// <summary>
        /// 设置对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            if (value == null) return true;
            if (expiry == null) expiry = _defaultExpiry;
            _cache.Set(key, value, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expiry });
            return true;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action">默认值</param>
        /// <returns></returns>
        public T? GetObject<T>(string key, Func<T>? action = null, TimeSpan? expiry = null) where T : class, new()
        {
            try
            {
                if (_cache.TryGetValue<T>(key,out T value)) return value;
                else if (action != null)
                {
                    var data = action();
                    SetObject(key, data, expiry);
                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetObject Key:{key} 异常：{ex.Message}\r\n{ex.StackTrace}");
                //_logger.LogError(ex.Message, ex);
            }
            if (action != null)
            {
                var data = action();
                SetObject(key, data, expiry);
                return data;
            }
            return default;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="action">默认值</param>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string key, Func<Task<T>>? action = null, TimeSpan? expiry = null,bool update=false) where T : class, new()
        {
            if (update == true && action != null)
            {
                var data = await action();
                SetObject(key, data, expiry);
                return data;
            }
            if (_cache.TryGetValue<T>(key, out T value)) return value;
            else if (action != null)
            {
                var data = await action();
                if (data != null)
                    SetObject(key, data, expiry);
                return data;
            }
            return default;
        }


        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan? expiry = null)
        {
            return SetObject(key, value, expiry);
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (value == null) return true;
            if (expiry == null) expiry = _defaultExpiry;
            _cache.Set(key, value, new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = expiry });
            return true;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? Get(string key)
        {
            if (_cache.TryGetValue<string>(key, out string value)) return value;
            return string.Empty;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? Get<T>(string key)
        {
            if (_cache.TryGetValue<T>(key, out T value)) return value;
            return default(T);
        }


        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? Get(string key, Func<string>? action = null, TimeSpan? expiry = null)
        {
            if (_cache.TryGetValue<string>(key, out string value)) return value;
            else if (action != null)
            {
                var newData = action();
                SetObject(key, newData, expiry);
                return newData;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key, Func<Task<string>>? action = null, TimeSpan? expiry = null)
        {
            if (_cache.TryGetValue<string>(key, out string value)) return value;
            else if (action != null)
            {
                var newData = await action();
                SetObject(key, newData, expiry);
                return newData;
            }
            return string.Empty;
        }

        public async Task<bool> SetHashAsync<T>(string key, string hashField, T t) where T : class
        {
            return await Task.FromResult(HashSet(key, hashField, t));
        }

        public async Task<bool> HashRemoveAsync(string key, string hashField)
        {
            return await Task.FromResult(HashDelete(key, hashField));
        }

        //public async Task<bool> KeyExpireAsync(string key, TimeSpan ttl)
        //{
        //    try
        //    {
        //        return await _cache.KeyExpireAsync(NewKey(key), ttl);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Redis Exception -> {e.Message}");
        //    }
        //    return false;
        //}

        public async Task<List<T>> GetHashAllAsync<T>(string key) where T : class
        {
            return await HashGetAllAsync<T>(key);
        }

        //public async Task<List<string>> GetHashAllStrAsync<T>(string key)
        //{

        //    return await HashGetAllAsync<T>(key);
        //}

        public async Task<List<string>> GetHashKeysAsync(string key)
        {
            ConcurrentDictionary<string, object> hash = null;
            if (_cache.TryGetValue(key, out hash))
            {
                return hash.Keys.ToList();
            }
            return new List<string>();
        }

        public async Task<T> GetHashAsync<T>(string key, string hashField) where T : class
        {
            return await Task.FromResult(HashGet<T>(key, hashField));
        }

        //public async Task<T> GetHash1Async<T>(string key, string hashfield) where T : class, new()
        //{
        //    try
        //    {
        //        return await GetHashAsync<T>(key, hashfield) ?? new T();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Redis Exception -> {e.Message}");
        //        return default;
        //    }
        //}

        private static object IncrementObj = new object();
        public async Task<long> Increment(string key, string hashField)
        {
            var realKey = $"{key}_{hashField}";
            long increment = 0;
            lock (IncrementObj)
            {
                increment = Get<long>(realKey);
                if (increment < 0) increment = 0;
                Set(realKey, ++increment);
            }
            return await Task.FromResult(increment);
        }

        public Task<bool> KeyExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out object _));
        }

        public async Task<bool> KeySetAsync<T>(string key, T value) where T : class, new()
        {
            return await Task.FromResult(Set(key, value));
        }

        public async Task<T> KeyGetAsync<T>(string key) where T : class
        {
            return await Task.FromResult(Get<T>(key));
        }

        //public Task<bool> SetnxAsync(string key, string value, TimeSpan expireTime)
        //{
        //    return _cache.StringSetAsync(NewKey(key), value, expireTime, When.NotExists);
        //}


        public bool ListAdd<T>(string key, T item) where T : class, new()
        {
            lock (key)
            {
                var list = GetObject<List<T>>(key);
                if (list == null) list = new List<T>();
                list.Add(item);
                return SetObject(key, list);
            }
        }

        public bool ListRemove<T>(string key, T item) where T : class, new()
        {
            lock (key)
            {
                var list = GetObject<List<T>>(key);
                if (list == null) return true;
                list.Remove(item);
                return SetObject(key, list);
            }
        }

        public List<T> GetList<T>(string key, T item) where T : class, new()
        {
            var list = GetObject<List<T>>(key);
            if (list == null) list = new List<T>();
            return list;
        }
        public bool SetInt(string key, int value, TimeSpan? expiry = null)
        {
            return Set(key, value, expiry);
        }

        public int? GetInt(string key)
        {
            return Get<int>(key);
        }

        public int? GetInt(string key, Func<int>? action = null, TimeSpan? expiry = null)
        {
            if (_cache.TryGetValue<int>(key, out int value)) return value;
            else if (action != null)
            {
                var data = action();
                Set(key, data, expiry);
                return data;
            }
            return null;
        }

        public async Task<int?> GetIntAsync(string key, Func<Task<int>>? action = null, TimeSpan? expiry = null)
        {
            if (_cache.TryGetValue<int>(key, out int value)) return value;
            else if (action != null)
            {
                var newData = await action();
                SetInt(key, newData, expiry);
                return newData;
            }
            return null;
        }

        private ConcurrentDictionary<string, object> DelCacheDict = new ConcurrentDictionary<string, object>();


        public async Task DelayRemoveAsync(string key)
        {
            if (DelCacheDict.TryAdd(key, key))
            {
                await _taskQueue.EnqueueAsync(async (provider, token) =>
                {
                    await RemoveAsync(key);
                    DelCacheDict.TryRemove(key, out _);
                    //await provider.GetService<IDistributedCacheService>().
                }, 1000);
            }
        }

        byte[]? IDistributedCache.Get(string key)
        {
           return Get<byte[]>(key);
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            return await Task.FromResult(Get<byte[]>(key));
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            Set(key, value);
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            Set(key, value);
            await Task.CompletedTask;
        }

        public void Refresh(string key)
        {
            
        }

        public async Task RefreshAsync(string key, CancellationToken token = default)
        {
            await Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task RemoveAsync(string key, CancellationToken token = default)
        {
            _cache.Remove(key);
            await Task.CompletedTask;
        }
    }
}
