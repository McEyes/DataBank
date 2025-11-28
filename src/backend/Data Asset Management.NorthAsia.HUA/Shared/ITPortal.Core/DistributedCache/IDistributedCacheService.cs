using Microsoft.Extensions.Caching.Distributed;

using StackExchange.Redis;

namespace ITPortal.Core.DistributedCache
{
    public interface IDistributedCacheService : IDistributedCache
    {

        public CacheType CacheType { get; }
        ///// <summary>
        ///// 将 key 的值设为 value ，当且仅当 key 不存在
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <param name="expireTime"></param>
        ///// <returns></returns>
        //public Task<bool> SetnxAsync(string key, string value, TimeSpan expireTime);


        /// <summary>
        /// Key存储
        /// </summary>
        public Task<bool> KeySetAsync<T>(string key, T value) where T : class, new();

        /// <summary>
        /// Key获取
        /// </summary>
        public Task<T> KeyGetAsync<T>(string key) where T : class;


        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public Task<long> Increment(string key, string hashField);

        /// <summary>
        /// Key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> KeyExistsAsync(string key);

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public Task<bool> SetHashAsync<T>(string key, string hashField, T t) where T : class;

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public Task<bool> HashRemoveAsync(string key, string hashField);

        /// <summary>
        /// 设置Key过期时间
        /// </summary>
        //public Task<bool> KeyExpireAsync(string key, TimeSpan ttl);

        ///// <summary>
        ///// 存取任意类型的值(hashId与key相同)
        ///// </summary>
        //public Task<List<T>> GetHashAllAsync<T>(string key) where T : class;


        ///// <summary>
        ///// 取字符串类型的值(hashId与key相同)
        ///// </summary>
        //public Task<List<string>> GetHashAllStrAsync<T>(string key) where T : class

        /// <summary>
        /// 获取hash所有key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<List<string>> GetHashKeysAsync(string key);

        
        /// <summary>
        /// 获取hash值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>default null</returns>
        public Task<T> GetHashAsync<T>(string key, string hashField) where T : class;

        ///// <summary>
        ///// 获取hash
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="hashfield"></param>
        ///// <returns>new T</returns>
        //Task<T> GetHash1Async<T>(string key, string hashfield) where T : class, new();
        bool HashSet<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class;
        Task<bool> HashSetAsync<T>(string key, string field, T data, TimeSpan? expiry = null) where T : class;
        Task<bool> HashSetAsync(string key, string field, string data, TimeSpan? expiry = null);
        T HashGet<T>(string key, string field) where T : class;
        //RedisValue[]? HashGetAll(string key, CommandFlags flags = CommandFlags.None);
        List<T> HashGetAll<T>(string key, Func<List<T>>? action = null, CommandFlags flags = CommandFlags.None) where T : class;
        Task<List<T>> HashGetAllAsync<T>(string key, Func<Task<List<T>>>? action = null, CommandFlags flags = CommandFlags.None) where T : class;
        bool HashSet(string key, string field, string data, TimeSpan? expiry = null);
        bool HashDelete(string key, string field);
        string HashGet(string key, string field);
        bool SetObject<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        T? GetObject<T>(string key, Func<T>? action = null, TimeSpan? expiry = null) where T : class, new();
        Task<T> GetObjectAsync<T>(string key, Func<Task<T>>? action = null, TimeSpan? expiry = null, bool update = false) where T : class, new();
        bool Set(string key, string value, TimeSpan? expiry = null);
        string? Get(string key);
        string? Get(string key, Func<string>? action = null, TimeSpan? expiry = null);
        Task<string> GetAsync(string key, Func<Task<string>>? action = null, TimeSpan? expiry = null);

        bool SetInt(string key, int value, TimeSpan? expiry = null);
        int? GetInt(string key);
        int? GetInt(string key, Func<int>? action = null, TimeSpan? expiry = null);
        Task<int?> GetIntAsync(string key, Func<Task<int>>? action = null, TimeSpan? expiry = null);


        bool ListAdd<T>(string key, T item) where T : class, new();
        bool ListRemove<T>(string key, T item) where T : class, new();
        List<T> GetList<T>(string key, T item) where T : class, new();
        Task DelayRemoveAsync(string key);
    }
}
