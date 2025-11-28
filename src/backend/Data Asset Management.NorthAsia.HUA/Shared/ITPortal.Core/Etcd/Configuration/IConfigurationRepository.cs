using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    public interface IConfigrationRepository : IDisposable
    {
        Task<IDictionary<string, string>> GetConfig();

        void Watch(IConfigrationWatcher watcher);
    }
}
