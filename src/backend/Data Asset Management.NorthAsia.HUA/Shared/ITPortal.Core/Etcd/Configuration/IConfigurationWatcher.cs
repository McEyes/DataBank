using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Core.Etcd.Configuration
{
    public interface IConfigrationWatcher
    {
        void FireChange();
    }
}
