using SqlSugar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.Search.Core
{
    public class UserDbSqlSugarScope : SqlSugarScope, IUserDbSugarClient
    {
        public UserDbSqlSugarScope(ConnectionConfig config) : base(config)
        {
        }

        public UserDbSqlSugarScope(List<ConnectionConfig> configs) : base(configs)
        {
        }

        public UserDbSqlSugarScope(ConnectionConfig config, Action<SqlSugarClient> configAction) : base(config, configAction)
        {
        }

        public UserDbSqlSugarScope(List<ConnectionConfig> configs, Action<SqlSugarClient> configAction) : base(configs, configAction)
        {
        }
    }
}
