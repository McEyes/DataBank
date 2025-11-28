using Furion.JsonSerialization;
using Furion.TaskQueue;

using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using SqlSugar;

using StackExchange.Redis;

using System.Collections.Concurrent;


namespace ITPortal.Core.DistributedCache
{
    public class RedisCacheService : RedisCache, IDistributedCacheService, IDisposable
    {
        public CacheType CacheType => CacheType.Redis;
        private bool _disposed;
        private IDatabase _cache;
        private readonly string _instance;
        private readonly RedisCacheOptions _options;
        private volatile IConnectionMultiplexer _connection;


        private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(30);

        //private const long NotPresent = -1;
        private static readonly Version ServerVersionWithExtendedSetCommand = new Version(4, 0, 0);
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private readonly ILogger<RedisCacheService> _logger;
        private readonly ITaskQueue _taskQueue;
        //// 异步入队
        //await TaskQueued.EnqueueAsync(async (provider, token) => { }, [delay]);
        public RedisCacheService(IOptions<RedisCacheOptions> optionsAccessor, ILogger<RedisCacheService> logger, ITaskQueue taskQueue) : base(optionsAccessor)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            if (optionsAccessor == null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }
            _options = optionsAccessor.Value;

            // This allows partitioning a single backend cache for use with multiple apps/services.
            _instance = _options.InstanceName ?? string.Empty;
            Connect();
        }

        private void Connect()
        {
            CheckDisposed();
            if (_cache != null)
            {
                _logger.LogInformation("缓存对象已经创建！");
                return;
            }
            _logger.LogInformation($" _connectionLock.Wait");
            _connectionLock.Wait();
            try
            {
                if (_cache == null)
                {
                    if (_options.ConnectionMultiplexerFactory == null)
                    {
                        if (_options.ConfigurationOptions is not null)
                        {
                            _logger.LogInformation($"创建_connection对象By ConfigurationOptions");
                            _connection = ConnectionMultiplexer.Connect(_options.ConfigurationOptions);
                        }
                        else
                        {
                            _logger.LogInformation($"创建_connection对象By Configuration");
                            _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"创建_connection对象By ConnectionMultiplexerFactory");
                        _connection = _options.ConnectionMultiplexerFactory().GetAwaiter().GetResult();
                    }
                    PrepareConnection();
                    _cache = _connection.GetDatabase();
                }
            }
            finally
            {
                _logger.LogInformation($" _connectionLock.Release");
                _connectionLock.Release();
            }
        }

        private void PrepareConnection()
        {
            ValidateServerFeatures();
            TryRegisterProfiler();
        }

        private void ValidateServerFeatures()
        {
            _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

            foreach (var endPoint in _connection.GetEndPoints())
            {
                if (_connection.GetServer(endPoint).Version < ServerVersionWithExtendedSetCommand)
                {
                    return;
                }
            }
        }

        private void TryRegisterProfiler()
        {
            _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

            if (_options.ProfilingSession != null)
            {
                _connection.RegisterProfiler(_options.ProfilingSession);
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        public bool HashSet<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class
        {
            if (data == null)
            {
                return _cache.HashDelete(key, field);
            }
            var json = JSON.Serialize(data);
            var result = _cache.HashSet(key, field, json);
            if (result && expiry != null) _cache.KeyExpire(key, expiry);
            return result;
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry">不传数据不会超时</param>
        public async Task<bool> HashSetAsync<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class
        {
            var json = JSON.Serialize(data);
            var result = await _cache.HashSetAsync(key, field, json);
            if (result && expiry != null) result = await _cache.KeyExpireAsync(key, expiry);
            return result;
        }

        /// <summary>
        /// 设置HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        public async Task<bool> HashSetAsync(string key, string field, string data, TimeSpan? expiry = null)
        {
            var result = await _cache.HashSetAsync(key, field, data);
            if (result && expiry != null) result = await _cache.KeyExpireAsync(key, expiry);
            return result;
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
            var json = _cache.HashGet(key, field);
            if (json.IsNullOrEmpty) return default;
            return JSON.Deserialize<T>(json);
        }

        ///// <summary>
        ///// 获取HashGet
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="field"></param>
        ///// <returns></returns>
        //public RedisValue[]? HashGetAll(string key, CommandFlags flags = CommandFlags.None)
        //{
        //    return _cache.HashValues(key, flags);
        //}

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
            var hashEntries = _cache.HashValues(key, flags);
            List<T>? list = new List<T>();
            foreach (var entry in hashEntries)
            {
                if (entry.HasValue)
                {
                    var data = JSON.Deserialize<T>(entry);
                    if (data != null) list.Add(data);
                }
            }
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
            var hashEntries = _cache.HashValues(key, flags);
            List<T>? list = new List<T>();
            foreach (var entry in hashEntries)
            {
                if (entry.HasValue)
                {
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(entry);
                    if (data != null) list.Add(data);
                }
            }
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
        /// <param name="expiry"></param>
        public bool HashSet(string key, string field, string data, TimeSpan? expiry = null)
        {
            if (data == null)
            {
                return _cache.HashDelete(key, field);
            }
            if (expiry == null) expiry = _defaultExpiry;
            _cache.HashSet(key, field, data);
            return _cache.KeyExpire(key, expiry);
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
            return _cache.HashDelete(key, field);
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
            RedisValue data = _cache.HashGet(key, field);
            if (data.HasValue) return data;
            return string.Empty;
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
            if (value == null)
            {
                return _cache.StringSet(key, string.Empty, TimeSpan.FromMilliseconds(1));
                //一分钟后超时
                //new DistributedCacheEntryOptions
                //{
                //    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                //}
            }
            if (expiry == null) expiry = _defaultExpiry;
            var json = JSON.Serialize(value);
            return _cache.StringSet(key, json, expiry);
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
                var json = _cache.StringGet(key);
                if (json.HasValue) return JSON.Deserialize<T>(json);
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
        public async Task<T> GetObjectAsync<T>(string key, Func<Task<T>>? action = null, TimeSpan? expiry = null, bool update = false) where T : class, new()
        {
            if (update == true && action != null)
            {
                var data = await action();
                SetObject(key, data, expiry);
                return data;
            }
            var json = _cache.StringGet(key);
            if (json.HasValue) return JSON.Deserialize<T>(json.ToString());
            else if (action != null)
            {
                var data = await action();
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
            if (expiry == null) expiry = _defaultExpiry;
            return _cache.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? Get(string key)
        {
            var data = _cache.StringGet(key);
            if (data.HasValue) return data;
            return string.Empty;
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? Get(string key, Func<string>? action = null, TimeSpan? expiry = null)
        {
            var data = _cache.StringGet(key);
            if (data.HasValue) return data;
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
            var data = _cache.StringGet(key);
            if (data.HasValue) return data.ToString();
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
            try
            {
                return await _cache.HashSetAsync(NewKey(key), hashField, JsonConvert.SerializeObject(t));
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return false;
        }

        public async Task<bool> HashRemoveAsync(string key, string hashField)
        {
            try
            {
                return await _cache.HashDeleteAsync(NewKey(key), hashField);
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return false;
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

        //public async Task<List<T>> GetHashAllAsync<T>(string key) where T : class
        //{
        //    var list = new List<T>();
        //    try
        //    {
        //        var result = await _cache.HashGetAllAsync(NewKey(key));
        //        if (result.Any())
        //        {
        //            foreach (var item in result)
        //            {
        //                var value = JSON.Deserialize<T>(item.Value);
        //                list.Add(value);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Redis Exception -> {e.Message}");
        //    }
        //    return list;
        //}

        //public async Task<List<string>> GetHashAllStrAsync<T>(string key)
        //{
        //    var list = new List<string>();
        //    try
        //    {
        //        var result = await _cache.HashGetAllAsync(NewKey(key));
        //        if (result.Any())
        //        {
        //            foreach (var item in result)
        //            {
        //                list.Add(item.Value);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError($"Redis Exception -> {e.Message}");
        //    }
        //    return list;
        //}

        public async Task<List<string>> GetHashKeysAsync(string key)
        {
            var list = new List<string>();
            try
            {
                var result = await _cache.HashKeysAsync(NewKey(key));
                if (result.Any())
                {
                    foreach (var item in result)
                    {
                        list.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return list;
        }

        public async Task<T> GetHashAsync<T>(string key, string hashField) where T : class
        {
            try
            {
                var result = await _cache.HashGetAsync(NewKey(key), hashField);
                if (result.HasValue)
                {
                    return JSON.Deserialize<T>(result.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return default;
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

        private string NewKey(string key)
        {
            return $"{_instance}{key}";
        }
        /// <summary>
        /// 对哈希表中指定字段的值进行原子递增操作
        /// 支持正数和负数（即递增和递减）
        /// 如果字段不存在，会先初始化为 0 再执行操作
        /// 如果哈希表不存在，会先创建哈希表再执行操作
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        public async Task<long> Increment(string key, string hashField)
        {
            return await _cache.HashIncrementAsync(NewKey(key), hashField);
        }

        public Task<bool> KeyExistsAsync(string key)
        {
            return _cache.KeyExistsAsync(key);
        }

        public async Task<bool> KeySetAsync<T>(string key, T value) where T : class, new()
        {
            try
            {
                return await _cache.SetAddAsync(NewKey(key), JsonConvert.SerializeObject(value));
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return false;
        }

        public async Task<T> KeyGetAsync<T>(string key) where T : class
        {
            try
            {
                var result = await _cache.StringGetAsync(NewKey(key));
                if (result.HasValue)
                {
                    return JSON.Deserialize<T>(result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Redis Exception -> {e.Message}");
            }
            return default;
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
            if (expiry == null) expiry = _defaultExpiry;
            return _cache.StringSet(key, value, expiry);
        }

        public int? GetInt(string key)
        {
            var data = _cache.StringGet(key);
            if (data.HasValue && int.TryParse(data, out int value)) return value;
            return null;
        }

        public int? GetInt(string key, Func<int>? action = null, TimeSpan? expiry = null)
        {
            var data = _cache.StringGet(key);
            if (data.HasValue && int.TryParse(data, out int value)) return value;
            else if (action != null)
            {
                var newData = action();
                SetInt(key, newData, expiry);
                return newData;
            }
            return null;
        }

        public async Task<int?> GetIntAsync(string key, Func<Task<int>>? action = null, TimeSpan? expiry = null)
        {
            var data = _cache.StringGet(key);
            if (data.HasValue && int.TryParse(data, out int value)) return value;
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
    }
}
