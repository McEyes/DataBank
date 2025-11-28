using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Furion;
using Furion.DependencyInjection;
using SqlSugar;

namespace DataTopicStore.Core.Repositories
{

    public class Repository<T> : SimpleClient<T> where T : class, new()
    {
        public Repository()
        {
            base.Context = App.GetService<ISqlSugarClient>();
        }

        public Repository(ISqlSugarClient context = null) : base(context)
        {

        }
    }
}
