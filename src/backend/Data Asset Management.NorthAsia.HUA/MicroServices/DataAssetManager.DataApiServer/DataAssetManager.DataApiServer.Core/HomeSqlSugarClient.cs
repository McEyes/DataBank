using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAssetManager.DataApiServer.Core
{
    public class HomeSqlSugarClient: SqlSugarScope
    {
        private HomeSqlSugarClient()
        {
        }

        public HomeSqlSugarClient(ConnectionConfig config)
        {
            _configs = new List<ConnectionConfig> { config };
        }

        public HomeSqlSugarClient(List<ConnectionConfig> configs)
        {
            _configs = configs;
        }

        public HomeSqlSugarClient(ConnectionConfig config, Action<SqlSugarClient> configAction)
        {
            _configs = new List<ConnectionConfig> { config };
            _configAction = configAction;
        }

        public HomeSqlSugarClient(List<ConnectionConfig> configs, Action<SqlSugarClient> configAction)
        {
            _configs = configs;
            _configAction = configAction;
        }

    }
}
