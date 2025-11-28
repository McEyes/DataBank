using dotnet_etcd;

using Furion.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd
{
    public interface IEtcdClientManager : ISingleton
    {
        Task<string> GetValAsync(string key);
        Task<Dictionary<string, string>> GetRangeAsync(string prefixKey);
        Task Watch(string key, Action<WatchEvent[]> method);
    }
}
