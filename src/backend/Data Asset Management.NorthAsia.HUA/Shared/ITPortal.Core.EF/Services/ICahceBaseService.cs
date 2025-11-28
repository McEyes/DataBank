

namespace ITPortal.Core.Services
{
    public interface ICahceQueryService<T, Tdto, KeyType>: IBaseService<T, Tdto, KeyType> where T : class, IEntity<KeyType>, new() where Tdto : class, IPageEntity<KeyType>, new()
    {
        Task<T> GetByCache(KeyType id);
        //Task<T> SingleByCache(T filter);
        //Task<List<T>> QueryByCache(T filter);
        //Task<List<T>> PageQueryByCache(T filter);
    }
}
